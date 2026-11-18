using DalilAcademy.Core.DTOs;
using DalilAcademy.Core.Service;
using Microsoft.AspNetCore.Mvc;

namespace DalilAcademy.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyController(ICurrencyService currencyService) : ControllerBase
    {
        [HttpPost("convert")]
        public async Task<ActionResult<ConvertPriceResponseDto>> Convert(ConvertPriceRequestDto dto)
        {
            var result = await currencyService.ConvertAsync(dto);
            return Ok(result);
        }
    }
}
