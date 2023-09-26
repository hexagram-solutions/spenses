using MediatR;
using Microsoft.AspNetCore.Mvc;
using Spenses.Api.Infrastructure;
using Spenses.Application.Common.Results;

namespace Spenses.Api.Controllers;

public abstract class ApiControllerBase : ControllerBase
{
    protected ApiControllerBase(IMediator mediator)
    {
        Mediator = mediator;
    }

    protected readonly IMediator Mediator;

    protected async Task<ActionResult> GetCommandResult<TRequest>(TRequest request, Func<ActionResult> successAction)
        where TRequest : IRequest<ServiceResult>
    {
        var result = await Mediator.Send(request);

        return !result.IsSuccess
            ? (result as ErrorResult)!.ToActionResult()
            : successAction();
    }

    protected async Task<ActionResult<TResult>> GetCommandResult<TResult, TRequest>(TRequest request,
        Func<TResult, ActionResult> successAction)
        where TRequest : IRequest<ServiceResult<TResult>>
    {
        var result = await Mediator.Send(request);

        return !result.IsSuccess
            ? (result.Result as ErrorResult)!.ToActionResult()
            : successAction(result.Value);
    }
}
