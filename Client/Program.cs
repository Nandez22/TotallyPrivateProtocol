namespace Protocol;

public class Program {
    public async static Task Main() {
        Header header = new Header {
            FullName = "Bob Marley",
            Address = "6353 Juan Tabo Blvd, Albuquerque, NM 87111",
            DateOfBirth = "11/17/2003",
            Email = "savewalterwhite@ymail.com",
            PhoneNumber = "2025550125"
        };

        Message jamba = new Message {
            Header = header,
            Command = "Jamba"
        };
        await Client.Run("127.0.0.1", 42069, jamba);


        Message pancakes = new Message {
            Header = header,
            Command = "wafflesorpancakes"
        };
        await Client.Run("127.0.0.1", 42069, pancakes);
    }
}