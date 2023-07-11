namespace Spenses.Application.Common.Results;

public record SuccessResult : ServiceResult
{
    public override bool IsSuccess => true;
}
