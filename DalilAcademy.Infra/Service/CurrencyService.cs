using DalilAcademy.Core.DTOs;
using DalilAcademy.Core.Service;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace DalilAcademy.Infra.Service
{
    public class CurrencyService(HttpClient http) : ICurrencyService
    {
        private record PairRespones([property: JsonPropertyName("conversion_rate")] decimal ConversionRate);
        public async Task<ConvertPriceResponseDto> ConvertAsync(ConvertPriceRequestDto r)
        {
           var data = await http.GetFromJsonAsync<PairRespones>($"pair/{r.FromCurrency}/{r.ToCurrency}") 
            ?? throw new HttpRequestException("Empty response from ExchangeRate-API.");

            var converted = Math.Round(r.Amount * data.ConversionRate, 2);
            return new ConvertPriceResponseDto(r.Amount, r.FromCurrency, converted, r.ToCurrency, data.ConversionRate.ToString());
        }
    }
}
