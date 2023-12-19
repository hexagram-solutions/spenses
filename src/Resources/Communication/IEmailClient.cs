namespace Spenses.Resources.Communication;

public interface IEmailClient
{
    Task SendEmail(string recipientAddress, string subject, string htmlMessage, string? plainTextMessage = null,
        CancellationToken cancellationToken = default);
}
