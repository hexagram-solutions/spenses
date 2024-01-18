using System.Runtime.Serialization;

namespace Spenses.Shared.Models.Common;

public enum SortDirection
{
    [EnumMember(Value = @"asc")]
    Asc = 0,

    [EnumMember(Value = @"desc")]
    Desc = 1
}
