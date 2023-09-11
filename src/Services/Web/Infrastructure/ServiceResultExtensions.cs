using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Spenses.Application.Common.Results;

namespace Spenses.Web.Infrastructure;

public static class ServiceResultExtensions
{
    public static ActionResult ToActionResult(this ErrorResult result)
    {
        switch (result)
        {
            case InvalidRequestErrorResult invalidRequestResult:
                if (invalidRequestResult.ValidationErrors?.Any() != true)
                    return new BadRequestObjectResult(invalidRequestResult.ErrorMessage);

                var modelStateDictionary = new ModelStateDictionary();

                foreach (var (propertyName, errorMessage) in invalidRequestResult.ValidationErrors)
                    modelStateDictionary.AddModelError(propertyName, errorMessage);

                var validationProblemDetails = new ValidationProblemDetails(modelStateDictionary);

                return new BadRequestObjectResult(validationProblemDetails);

            case UnauthorizedErrorResult insufficientPermissionsResult:
                return new UnauthorizedObjectResult(new ProblemDetails
                {
                    Title = "Insufficient permissions.",
                    Status = StatusCodes.Status403Forbidden,
                    Detail = insufficientPermissionsResult.ErrorMessage,
                });

            case NotFoundErrorResult notFoundResult:
                return new NotFoundObjectResult(new ProblemDetails
                {
                    Title = "Not found.",
                    Status = StatusCodes.Status404NotFound,
                    Detail = notFoundResult.ErrorMessage,
                });

            case ConflictedStateErrorResult conflictedStateResult:
                return new ConflictObjectResult(new ProblemDetails
                {
                    Title = "Conflicted state encountered.",
                    Status = StatusCodes.Status409Conflict,
                    Detail = conflictedStateResult.ErrorMessage,
                });

            default:
                var problemDetails = new ProblemDetails
                {
                    Title = "An error occurred on the server.",
                    Status = StatusCodes.Status500InternalServerError,
                    Detail = result.ErrorMessage,
                };

                return new ObjectResult(problemDetails) { StatusCode = StatusCodes.Status500InternalServerError, };
        }
    }
}
