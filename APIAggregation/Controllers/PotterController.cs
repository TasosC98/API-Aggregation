namespace APIAggregation.Controllers;
using APIAggregation.Models.DTOs;
using APIAggregation.Services.Definitions;
using APIAggregation.Wrappers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class PotterController : ControllerBase
    {
        private readonly IPotterService _bookService;

        public PotterController(IPotterService bookService)
        {
            _bookService = bookService;
        }

        // GET: api/book
        [HttpGet("GetBooks")]
        public async Task<IActionResult> GetBooks(string lang)
        {
            try
            {
                var response = await _bookService.GetBookInfo(lang);

                if (!response.Success)
                {
                    return BadRequest(new { message = response.Message });
                }

                return Ok(response.Data);
            }
            catch (Exception ex)
            {
                // Return a generic error response
                return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
            }
        }
    }