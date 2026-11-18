using System;
using System.Collections.Generic;
using System.Text;

namespace DalilAcademy.Core.DTOs
{
    public record ConvertPriceRequestDto(decimal Amount, string FromCurrency, string ToCurrency);
    public record ConvertPriceResponseDto(decimal OriginalAmount, string FromCurrency, decimal ConvertedAmount, string ToCurrency,string ExchangeRate);
}
