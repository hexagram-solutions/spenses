namespace Spenses.Application.Common.Results;

public record ConflictedStateErrorResult : ErrorResult
{
    public ConflictedStateErrorResult(string reason)
        : base(reason)
    {
    }

}
