using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using DalilAcademy.Core.Payments;
using Microsoft.Extensions.Options;

namespace DalilAcademy.Infra.Payments
{
    public class PayPalPaymentGateway(HttpClient http, IOptions<PayPalSettings> options) : IPaymentGateway
    {
        private readonly PayPalSettings settings = options.Value;

        // 1) Access token (client credentials)
        private async Task<string> GetAccessTokenAsync(CancellationToken ct)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, "v1/oauth2/token")
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["grant_type"] = "client_credentials"
                })
            };

            var basic = Convert.ToBase64String(
                Encoding.UTF8.GetBytes($"{settings.ClientId}:{settings.Secret}"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", basic);

            using var response = await http.SendAsync(request, ct);
            await EnsureSuccessAsync(response, ct);

            var json = await response.Content.ReadFromJsonAsync<JsonElement>(ct);
            return json.GetProperty("access_token").GetString()!;
        }

        // 2) Create order
        // 2) Create order
        public async Task<PayPalOrder> CreateOrderAsync(decimal amountUsd, CancellationToken ct = default)
        {
            var token = await GetAccessTokenAsync(ct);

            var payload = new
            {
                intent = "CAPTURE",
                purchase_units = new[]
                {
                    new
                    {
                        reference_id = Guid.NewGuid().ToString(), // تم إضافة هذا الحقل لحل مشكلة 422
                        amount = new
                        {
                            currency_code = "USD",
                            value = amountUsd.ToString("0.00", CultureInfo.InvariantCulture)
                        }
                    }
                },
                application_context = new
                {
                    return_url = settings.ReturnUrl,
                    cancel_url = settings.ReturnUrl
                }
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, "v2/checkout/orders")
            {
                Content = JsonContent.Create(payload)
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var response = await http.SendAsync(request, ct);
            await EnsureSuccessAsync(response, ct);

            var json = await response.Content.ReadFromJsonAsync<JsonElement>(ct);
            var orderId = json.GetProperty("id").GetString()!;

            var approvalUrl = json.GetProperty("links").EnumerateArray()
                .Where(l => l.GetProperty("rel").GetString() is "approve" or "payer-action")
                .Select(l => l.GetProperty("href").GetString())
                .FirstOrDefault()
                ?? throw new HttpRequestException("PayPal did not return an approval link.");

            return new PayPalOrder(orderId, approvalUrl);
        }

        // 3) Capture
        public async Task<bool> CaptureOrderAsync(string orderId, CancellationToken ct = default)
        {
            var token = await GetAccessTokenAsync(ct);

            using var request = new HttpRequestMessage(
                HttpMethod.Post, $"v2/checkout/orders/{orderId}/capture")
            {
                Content = new StringContent("{}", Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var response = await http.SendAsync(request, ct);

            // أخطاء السيرفر عند PayPal ← 503. أخطاء الطلب (غير موافَق عليه، ملتقط، منتهي) ← فشل عادي
            if ((int)response.StatusCode >= 500)
                await EnsureSuccessAsync(response, ct);

            if (!response.IsSuccessStatusCode)
                return false;

            var json = await response.Content.ReadFromJsonAsync<JsonElement>(ct);
            return json.TryGetProperty("status", out var status)
                   && status.GetString() == "COMPLETED";
        }

        private static async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken ct)
        {
            if (response.IsSuccessStatusCode) return;
            var body = await response.Content.ReadAsStringAsync(ct);
            throw new HttpRequestException($"PayPal returned {(int)response.StatusCode}: {body}");
        }
    }
}