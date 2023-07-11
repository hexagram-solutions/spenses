namespace Spenses.Application.Common.Results;

public record ValueResult(object? Value) : ServiceResult
{
    public Type? ValueType { get; set; }

    public override bool IsSuccess => true;
}
