namespace Spenses.Application.Common.Results;

public record NotFoundErrorResult(string ResourceIdentifier, string? ResourceType = "")
    : ErrorResult(!string.IsNullOrEmpty(ResourceType) ?
        $"The resource for {ResourceType} with identifier '{ResourceIdentifier}' could not be found." :
        $"The resource with identifier '{ResourceIdentifier}' could not be found.")
{
    public NotFoundErrorResult(Guid resourceIdentifier, string? resourceType = "")
        : this(resourceIdentifier.ToString(), resourceType)
    {
    }
}
