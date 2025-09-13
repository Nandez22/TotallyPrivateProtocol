## Introduction

This protocol hopes to serve as a new privacy first standard for TCP. In the current age where every connection we make reveals more about our private identities to the host we connect to, it is more imparitive than ever we take an off ramp towards regaining our personal security. That is precicly my motivation for designing this protocol, to put users information back in their control. Among the privacy benifits offered, simplicity was another motivating factor it's design.

## Message Structure
```json
{
  "Header": {
    "FullName": "string",
    "Address": "string",
    "DateOfBirth": "YYYY-MM-DD",
    "PhoneNumber": "string",
    "Email": "string"
  },
  "Command": "string"
}
```
As it's readily apparent, this protocol collects substantially less personal information about the user during each interaction than current TCP implementations.
> _The header data is validated to ensure that the information is accurate to a real person so it can be confidently sold to databrokers for a higher price in order to fund future development_

All props / objects in the message structure are required and validated by the server.
Invalid messages will be responded to as follows:
```json
{
  "response": "Error",
  "details": "string"
}
```
`details` changes per case:
 - `Invalid Header: {cause}` Header failed validation, `cause` being the value of the failing prop _(props validate in order)_
 - `Unknown Command: {command}` Command is not recognized, `command` being the name of the command recieved
 - JsonException: If the server is unable to process the provided json, the deserialization error will be sent back to the client

### Header Props & Validation Criteria:
 - `FullName` Client's Full Name
    > \> 0 chars
 - `Address` Client's Street Sddress
   > \<HouseNumber> \<StreetName> \<StreetType>, \<City>, \<ST> \<ZIP>(5 digit zip or `-` + 4 digit zip)
 - `DateOfBirth` Client's Date of Birth
   > Most `DateTime` parsable formats
   > DoB must be in the range [14, 99]
- `PhoneNumber` Client's Phone Number
  > Must be a **VALID** & **REGISTERED US** phone number
- `Email` Client's Email Address
  > Must be a valid email format with prefix and domain

### Valid Commands
 - `jamba` will recieve "juice" back from server
 - `wafflesorpancakes` will recieve a response of either "Waffles" or "Pancakes" from the server
 - `quit` closes client / server connection
