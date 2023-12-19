using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Spenses.Resources.Communication.Smtp;

public class SmtpEmailClient(IOptions<EmailClientOptions> clientOptions, IOptions<SmtpOptions> smtpOptions)
    : IEmailClient
{
    public async Task SendEmail(string recipientAddress, string subject, string htmlMessage, string? plainTextMessage = null,
        CancellationToken cancellationToken = default)
    {
        var bodyBuilder = new BodyBuilder { HtmlBody = htmlMessage, }; // 💪

        if (plainTextMessage is not null)
            bodyBuilder.TextBody = plainTextMessage;

        var fromAddress = new MailboxAddress(
            clientOptions.Value.FromDisplayName,
            clientOptions.Value.FromEmailAddress);

        var toAddress = new MailboxAddress(recipientAddress, recipientAddress);

        var message = new MimeMessage
        {
            From = { fromAddress },
            To = { toAddress },
            Subject = subject,
            Body = bodyBuilder.ToMessageBody()
        };

        using var client = new SmtpClient();

        await client.ConnectAsync(
            smtpOptions.Value.Host,
            smtpOptions.Value.Port,
            smtpOptions.Value.UseSsl,
            cancellationToken);

        await client.SendAsync(message, cancellationToken);

        await client.DisconnectAsync(true, cancellationToken);
    }
}
