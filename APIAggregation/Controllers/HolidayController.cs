using APIAggregation.Models.DTOs;
using APIAggregation.Services.Definitions;
using APIAggregation.Wrappers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APIAggregation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HolidayController : ControllerBase
    {
        private readonly IHolidayService _holidayService;

        public HolidayController(IHolidayService holidayService)
        {
            _holidayService = holidayService;
        }

        // GET: api/Holiday?countryIsoCode=US&languageIsoCode=EN&validFrom=2022-01-01&validTo=2022-12-31
        [HttpGet("GetHolidays")]
        public async Task<IActionResult> GetHolidays([FromQuery] string countryIsoCode, [FromQuery] string? languageIsoCode, [FromQuery] string validFrom, [FromQuery] string validTo)
        {
            try
            {
                var response = await _holidayService.GetHolidayInfo(countryIsoCode, languageIsoCode, validFrom, validTo);
                
                if (!response.Success)
                {
                    return BadRequest(new { message = response.Message });
                }

                return Ok(response.Data);
            }
            catch (Exception ex)
            {
                // Log the exception (this is just an example, adapt it to your logging mechanism)
                return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
            }
        }
    }
}