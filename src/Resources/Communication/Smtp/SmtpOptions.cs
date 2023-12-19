namespace Spenses.Resources.Communication;

public class SmtpOptions
{
    public string Host { get; set; } = null!;

    public int Port { get; set; }

    public bool UseSsl { get; set; }
}
