namespace Spenses.Application.Common.Results;

public record UnauthorizedErrorResult(string ErrorMessage = "Unauthorized access") : ErrorResult(ErrorMessage);
