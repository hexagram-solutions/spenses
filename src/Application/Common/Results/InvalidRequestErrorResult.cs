namespace Spenses.Application.Common.Results;

public record InvalidRequestErrorResult : ErrorResult
{
    public InvalidRequestErrorResult(IEnumerable<(string propertyName, string errorMessage)> validationErrors)
        : base("One or more validation errors occurred.")
    {
        ValidationErrors = validationErrors;
    }

    public InvalidRequestErrorResult(string propertyName, string errorMessage)
        : this(new[] { (propertyName, errorMessage) })
    {
    }

    public IEnumerable<(string propertyName, string errorMessage)>? ValidationErrors { get; set; }
}
