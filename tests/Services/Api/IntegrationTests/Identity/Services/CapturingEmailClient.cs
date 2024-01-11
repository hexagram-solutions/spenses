using Spenses.Resources.Communication;

namespace Spenses.Api.IntegrationTests.Identity.Services;

public class CapturingEmailClient : IEmailClient
{
    public List<CapturedEmailMessage> EmailMessages { get; } = [];

    public Task SendEmail(string recipientAddress, string subject, string htmlMessage, string? plainTextMessage = null,
        CancellationToken cancellationToken = default)
    {
        EmailMessages.Add(new CapturedEmailMessage
        {
            RecipientAddress = recipientAddress,
            Subject = subject,
            HtmlMessage = htmlMessage,
            PlainTextMessage = plainTextMessage,
        });

        return Task.CompletedTask;
    }
}

public class CapturedEmailMessage
{
    public required string RecipientAddress { get; set; }

    public required string Subject { get; set; }

    public required string HtmlMessage { get; set; }

    public string? PlainTextMessage { get; set; }
}
