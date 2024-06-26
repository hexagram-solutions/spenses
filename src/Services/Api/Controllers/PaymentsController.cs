using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Spenses.Api.Infrastructure;
using Spenses.Application.Features.Payments.Requests;
using Spenses.Shared.Models.Payments;

namespace Spenses.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("/homes/{homeId:guid}/payments")]
public class PaymentsController(ISender sender) : ControllerBase
{

    /// <summary>
    /// Create a new payment.
    /// </summary>
    /// <param name="homeId">The home identifier.</param>
    /// <param name="props">The payment properties.</param>
    /// <returns>The newly created payment.</returns>
    [HttpPost]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Post))]
    public async Task<ActionResult<Payment>> PostPayment(Guid homeId, PaymentProperties props)
    {
        var payment = await sender.Send(new CreatePaymentCommand(homeId, props));

        return CreatedAtAction(nameof(GetPayment), new { homeId, paymentId = payment.Id }, payment);
    }

    /// <summary>
    /// Query payments with various parameters.
    /// </summary>
    /// <param name="homeId">The home identifier.</param>
    /// <param name="query">The parameters to query payments with.</param>
    /// <returns>The filtered and sorted payments.</returns>
    [HttpGet]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.GetAll))]
    public async Task<ActionResult<IEnumerable<PaymentDigest>>> GetPayments(Guid homeId,
        [FromQuery] FilteredPaymentsQuery query)
    {
        var payments = await sender.Send(new PaymentsQuery(homeId)
        {
            Skip = query.Skip,
            Take = query.Take,
            OrderBy = query.OrderBy,
            SortDirection = query.SortDirection,
            MinDate = query.MinDate,
            MaxDate = query.MaxDate,
        });

        return Ok(payments);
    }

    /// <summary>
    /// Fetch a payment.
    /// </summary>
    /// <param name="homeId">The home identifier.</param>
    /// <param name="paymentId">The payment identifier.</param>
    /// <returns>The payment.</returns>
    [HttpGet("{paymentId:guid}")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Get))]
    public async Task<ActionResult<Payment>> GetPayment(Guid homeId, Guid paymentId)
    {
        var payment = await sender.Send(new PaymentQuery(homeId, paymentId));

        return Ok(payment);
    }

    /// <summary>
    /// Updates a payment.
    /// </summary>
    /// <param name="homeId">The home identifier.</param>
    /// <param name="paymentId">The payment identifier.</param>
    /// <param name="props">The new payment properties.</param>
    /// <returns></returns>
    [HttpPut("{paymentId:guid}")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Put))]
    public async Task<ActionResult<Payment>> PutPayment(Guid homeId, Guid paymentId, PaymentProperties props)
    {
        var payment = await sender.Send(new UpdatePaymentCommand(homeId, paymentId, props));

        return Ok(payment);
    }

    /// <summary>
    /// Deletes a payment.
    /// </summary>
    /// <param name="homeId">The home identifier.</param>
    /// <param name="paymentId">The payment identifier.</param>
    [HttpDelete("{paymentId:guid}")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Delete))]
    public async Task<ActionResult> DeletePayment(Guid homeId, Guid paymentId)
    {
        await sender.Send(new DeletePaymentCommand(homeId, paymentId));

        return NoContent();
    }
}
