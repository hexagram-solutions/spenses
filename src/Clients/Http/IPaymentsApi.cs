using Refit;
using Spenses.Application.Models.Common;
using Spenses.Application.Models.Payments;

namespace Spenses.Client.Http;

public interface IPaymentsApi
{
    [Post("/homes/{homeId}/payments")]
    Task<ApiResponse<Payment>> PostPayment(Guid homeId, PaymentProperties props);

    [Get("/homes/{homeId}/payments")]
    Task<ApiResponse<PagedResult<PaymentDigest>>> GetPayments(Guid homeId, [Query] FilteredPaymentQuery query);

    [Get("/homes/{homeId}/payments/{paymentId}")]
    Task<ApiResponse<Payment>> GetPayment(Guid homeId, Guid paymentId);

    [Put("/homes/{homeId}/payments/{paymentId}")]
    Task<ApiResponse<Payment>> PutPayment(Guid homeId, Guid paymentId, PaymentProperties props);

    [Delete("/homes/{homeId}/payments/{paymentId}")]
    Task DeletePayment(Guid homeId, Guid paymentId);
}
