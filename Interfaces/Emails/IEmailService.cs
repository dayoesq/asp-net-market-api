namespace Market.Interfaces.Emails;

public interface IEmailService
{
    Task SendVerificationEmail(string email, int verificationCode);
}


