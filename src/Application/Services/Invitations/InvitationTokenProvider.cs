using System.Security.Cryptography;
using Hexagrams.Extensions.Common.Serialization;
using Microsoft.AspNetCore.DataProtection;

namespace Spenses.Application.Services.Invitations;

public record InvitationData(Guid InvitationId);

public class InvitationTokenProvider(IDataProtectionProvider dataProtection)
{
    private const string Purpose = "invitation-token";

    public string ProtectInvitationData(InvitationData data)
    {
        var protector = dataProtection.CreateProtector(Purpose);
        var protectedData = protector.Protect(data.ToJson());

        return protectedData;
    }

    public InvitationData UnprotectInvitationData(string token)
    {
        var protector = dataProtection.CreateProtector(Purpose);
        var unprotectedToken = protector.Unprotect(token);

        return unprotectedToken.FromJson<InvitationData>()!;
    }

    public bool TryUnprotectInvitationData(string token, out InvitationData? invitationData)
    {
        invitationData = null;

        try
        {
            invitationData = UnprotectInvitationData(token);

            return true;
        }
        catch (CryptographicException)
        {
            return false;
        }
    }
}
