namespace Protocol;

public class Header {
    public string FullName { get; set; }
    public string Address { get; set; }
    public string DateOfBirth { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
}

public class Message {
    public Header Header { get; set; }
    public string Command { get; set; }
}