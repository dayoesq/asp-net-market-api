namespace Market.Models.DTOS;

public class ErrorResponse
{
    public string Message { get; set; }

    public ErrorResponse(string errorMessage)
    {
        Message = errorMessage;
    }
}


