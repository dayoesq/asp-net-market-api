namespace Market.Services.Emails;

public class MailgunEmailService : IEmailService
{
    // You'll need to install the Mailgun nuget package to use this service.

    public Task SendVerificationEmail(string email, int verificationCode)
    {
        return Task.CompletedTask;
        // Implement the code to send the verification email using Mailgun.
        // This could involve calling the Mailgun API with your API key, domain, and message content.
        // For the purpose of this example, I'll leave the implementation details empty.
    }
}