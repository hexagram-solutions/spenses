using Refit;
using Spenses.Shared.Models.Common;
using Spenses.Shared.Models.Payments;

namespace Spenses.Client.Http;

public interface IPaymentsApi
{
    [Post("/homes/{homeId}/payments")]
    Task<IApiResponse<Payment>> PostPayment(Guid homeId, PaymentProperties props);

    [Get("/homes/{homeId}/payments")]
    Task<IApiResponse<PagedResult<PaymentDigest>>> GetPayments(Guid homeId, [Query] FilteredPaymentQuery query);

    [Get("/homes/{homeId}/payments/{paymentId}")]
    Task<IApiResponse<Payment>> GetPayment(Guid homeId, Guid paymentId);

    [Put("/homes/{homeId}/payments/{paymentId}")]
    Task<IApiResponse<Payment>> PutPayment(Guid homeId, Guid paymentId, PaymentProperties props);

    [Delete("/homes/{homeId}/payments/{paymentId}")]
    Task<IApiResponse> DeletePayment(Guid homeId, Guid paymentId);
}
