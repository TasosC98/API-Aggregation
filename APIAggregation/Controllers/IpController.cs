using APIAggregation.Services.Definitions;
using Microsoft.AspNetCore.Mvc;

namespace APIAggregation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IpController : ControllerBase
{
    private readonly IIpService _ipService;

    public IpController(IIpService ipService)
    {
        _ipService = ipService;
    }
    
    [HttpGet("GetIpDetails")]
    public async Task<IActionResult> GetIpDetails()
    {
        var ipResponse = await _ipService.GetIpInfo("46.177.240.162");

        if (ipResponse.Success && ipResponse.Data != null)
        {
            return Ok(ipResponse.Data);
        }

        return BadRequest(ipResponse.Message);

    }

}