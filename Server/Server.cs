using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Text.Json;
using System.Text.RegularExpressions;
using PhoneNumbers;

namespace Server;

public class Server {
    private static readonly Regex addyRegex = new(
        @"^\d{1,6}\s+[A-Za-z0-9\s.,'-]+(?:\s+(Street|St|Avenue|Ave|Boulevard|Blvd|Road|Rd|Lane|Ln|Drive|Dr|Court|Ct|Square|Sq|Trail|Trl|Parkway|Pkwy|Circle|Cir))\.?,?\s+[A-Za-z\s]+,?\s+[A-Za-z]{2}\s+\d{5}(-\d{4})?$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);
    
    public static void Run(int port) {
        Random random = new();
        TcpListener listener = new(IPAddress.Any, port);
        listener.Start();

        while(true) {
            TcpClient client = listener.AcceptTcpClient();
            Console.WriteLine($"Client connected: {client.Client.RemoteEndPoint}");
            
            using NetworkStream stream = client.GetStream();
            using StreamReader reader = new(stream);
            using StreamWriter writer = new(stream) { AutoFlush = true };
            
            writer.WriteLine(JsonSerializer.Serialize(new { response = "Glutentag!"}));
            Console.WriteLine("Welcome message sent");
            
            bool running = true;
            while(running && client.Connected) {
                string? message = reader.ReadLine();
                if(message == null) break;

                try {
                    Message? msg = JsonSerializer.Deserialize<Message>(message);
                    Console.WriteLine($"Received message: {message}");

                    string? cause = null;
                    if(msg == null || !ValidateHeader(msg.Header, out cause)) {
                        writer.WriteLine(JsonSerializer.Serialize(new { response = "invalid header", details = cause ?? "null" }));
                        break;
                    }

                    Console.WriteLine($"Received command: {msg.Command}");
                    switch(msg.Command.ToLowerInvariant()) {
                        case "jamba":
                            var response = new { response = "juice" };
                            writer.WriteLine(JsonSerializer.Serialize(response));
                            break;
                        
                        case "wafflesorpancakes":
                            string choice = random.Next(0,2) == 0? "Waffles" : "Pancakes";
                            writer.WriteLine(JsonSerializer.Serialize(
                                new { response = choice }
                            ));
                            break;
                        
                        case "quit":
                            writer.WriteLine(JsonSerializer.Serialize(new { response = "bye" }));
                            running = false;
                            break;
                        
                        default:
                            writer.WriteLine(JsonSerializer.Serialize(new { response = "error", details = "Unknown Command" }));
                            break;
                    }
                }
                catch(JsonException e) {
                    writer.WriteLine(JsonSerializer.Serialize(new { response = "error", details = e.Message }));
                }
            }
        }
    }

    private static bool ValidateHeader(Header header, out string cause) {
        cause = header.FullName;
        if(string.IsNullOrWhiteSpace(header.FullName)) return false;

        //Could verify using googlemaps API but that adds a bunch of async stuff which leads to message queueing and I'm too sick to process all that at the moment :)
        cause = header.Address;
        if(!addyRegex.IsMatch(header.Address ?? "")) return false;

        //Does not work for people over 100
        cause = header.DateOfBirth;
        if(!DateTime.TryParse(header.DateOfBirth, out DateTime dt)) return false; 
        DateTime today = DateTime.Today;
        int age =  today.Year - dt.Year;
        if(age is < 14 or > 100) return false;
        
        //Only works in the US lol
        cause = header.PhoneNumber;
        try {
            PhoneNumberUtil phoneUtil = PhoneNumberUtil.GetInstance();
            PhoneNumber number = phoneUtil.Parse(header.PhoneNumber, "US");
            if(!phoneUtil.IsValidNumber(number)) return false;
        } catch { return false; }

        cause = header.Email;
        if(string.IsNullOrEmpty(header.Email)) return false;
        try { MailAddress _ = new(header.Email); }
        catch { return false; }

        return true;
    }
}