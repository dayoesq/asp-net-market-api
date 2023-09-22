namespace Market.Utils;

public static class Errors
{
    public static string Conflict409 => "The entity already exists.";
    public static string NotFound404 => "The entity could not be found.";
    public static string Server500 => "An error occured.";

    public static string UnAuthorized401 { get; set; } = "Unauthorised operation.";
    public static string InvalidCredentials => "Invalid credentials.";
    public static string Repetition => "This action is already performed.";
    public static string InvalidFormat => "Invalid.";
    public static string UnverifiedAccount => "Unverified account.";
    public static string ExpiredToken => "Expired token.";
}
public static class ResponseMessage
{
    public static string Success => "Success.";
}