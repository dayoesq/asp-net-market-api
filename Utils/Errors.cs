namespace Market.Utils;

public static class Errors
{
    public static string Conflict409 { get; set; } = "The entity already exists.";
    public static string NotFound404 { get; set; } = "The entity could not be found.";
    public static string Server500 { get; set; } = "An error occured.";
    public static string UnAuthorized401 { get; set; } = "Unauthorised operation.";
    public static string InvalidCredentials { get; set; } = "Invalid credentials.";
    public static string Repetition { get; set; } = "This action is already performed.";
    public static string InvalidFormat { get; set; } = "Invalid.";
    public static string UnverifiedAccount { get; set; } = "Unverified account.";
    public static string ExpiredToken { get; set; } = "Expired token.";

}
public static class ResponseMessage
{
    public static string Success { get; set; } = "Success.";

}