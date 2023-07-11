namespace Spenses.Application.Common.Results;

public record ErrorResult(string? ErrorMessage = null) : ServiceResult
{
    public override bool IsSuccess => false;
}
