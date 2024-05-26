namespace API.DTO;

public class ErrorMessage
{
    public ErrorMessage(string message)
    {
        Message = message;
    }

    public string Message { get; set; }
}