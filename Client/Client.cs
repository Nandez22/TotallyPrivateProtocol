using System.Net.Sockets;
using System.Text.Json;

namespace Protocol;

public class Client {
    public static async Task Run(string ip, int port, Message message) {
        using TcpClient client = new();
        await client.ConnectAsync(ip, port);
        Console.WriteLine("Connected to server");
        
        using NetworkStream stream = client.GetStream();
        using StreamReader reader = new(stream);
        using StreamWriter writer = new(stream) { AutoFlush = true };
        
        string? greeting = await reader.ReadLineAsync();
        Console.WriteLine($"[Server] {greeting}");
        
        string msg = JsonSerializer.Serialize(message);
        await writer.WriteLineAsync(msg);
        Console.WriteLine($"[Client] {message.Command}");
        
        string? response = await reader.ReadLineAsync();
        Console.WriteLine($"[Server] {response}");
        
        Message quit = new();
        quit.Header = message.Header;
        quit.Command = "Quit";
        await writer.WriteLineAsync(JsonSerializer.Serialize(quit));
        Console.WriteLine($"[Client] {quit.Command}");
        
        string? adios = await reader.ReadLineAsync();
        Console.WriteLine($"[Server] {adios}");
    }
}