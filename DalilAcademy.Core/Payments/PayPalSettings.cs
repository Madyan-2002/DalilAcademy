namespace DalilAcademy.Core.Payments
{
    public class PayPalSettings
    {
        public string BaseUrl { get; set; } = string.Empty;
        public string ReturnUrl { get; set; } = string.Empty;
        public decimal JodToUsdRate { get; set; }
        public string ClientId { get; set; } = string.Empty;   // من User Secrets
        public string Secret { get; set; } = string.Empty;     // من User Secrets
    }
}