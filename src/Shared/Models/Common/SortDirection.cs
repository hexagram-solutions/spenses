using System.Runtime.Serialization;

namespace Spenses.Application.Models.Common;

public enum SortDirection
{
    [EnumMember(Value = @"asc")]
    Asc = 0,

    [EnumMember(Value = @"desc")]
    Desc = 1
}
