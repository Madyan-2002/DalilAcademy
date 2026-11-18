namespace DalilAcademy.Core.Payments
{
    public record PayPalOrder(string OrderId, string ApprovalUrl);

    public interface IPaymentGateway
    {
        Task<PayPalOrder> CreateOrderAsync(decimal amountUsd, CancellationToken ct = default);

        // true فقط لو PayPal رد COMPLETED
        Task<bool> CaptureOrderAsync(string orderId, CancellationToken ct = default);
    }
}