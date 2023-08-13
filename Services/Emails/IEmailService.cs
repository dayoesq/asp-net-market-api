namespace Market.Services.Emails;

public interface IEmailService
{
    Task SendVerificationEmail(string email, int verificationCode);
}