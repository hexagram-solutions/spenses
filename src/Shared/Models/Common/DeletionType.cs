namespace Spenses.Shared.Models.Common;

public record DeletionResult<T>(DeletionType Type, T Model);

public enum DeletionType
{
    Deleted,
    Deactivated
}
