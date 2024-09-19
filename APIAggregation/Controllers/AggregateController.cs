using APIAggregation.Models.DTOs;
using APIAggregation.Services.Definitions;
using APIAggregation.Wrappers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using APIAggregation.Models.DTOs.Filters;

namespace APIAggregation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AggregatedController : ControllerBase
    {
        private readonly IAggregatedService _aggregatedService;

        public AggregatedController(IAggregatedService aggregatedService)
        {
            _aggregatedService = aggregatedService;
        }

        [HttpPost]
        public async Task<IActionResult> GetAggregatedData([FromQuery] string countryIsoCode, [FromQuery] string? languageIsoCode, [FromQuery] string validFrom, [FromQuery] string validTo,  [FromQuery] string ip, [FromQuery] string lang, AggregatedDataFilterDto aggregatedDataFilters)
        {
            
            try
            {
                var response = await _aggregatedService.GetAggregatedData(countryIsoCode, languageIsoCode, validFrom, validTo, ip, lang, aggregatedDataFilters);
                
                if (!response.Success)
                {
                    return BadRequest(new { message = response.Message });
                }

                return Ok(response.Data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
            }
        }
    }
}