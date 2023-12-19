using Azure;
using Azure.Communication.Email;
using Microsoft.Extensions.Options;

namespace Spenses.Resources.Communication.Azure;

public class AzureEmailClient(EmailClient emailClient, IOptions<EmailClientOptions> options) : IEmailClient
{
    public Task SendEmail(string recipientAddress, string subject, string htmlMessage, string? plainTextMessage = null,
        CancellationToken cancellationToken = default)
    {
        var content = new EmailContent(subject) { Html = htmlMessage, PlainText = plainTextMessage };

        var message = new EmailMessage(options.Value.FromEmailAddress, recipientAddress, content);

        return emailClient.SendAsync(WaitUntil.Started, message, cancellationToken);
    }
}
