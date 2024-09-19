using APIAggregation.Models.DTOs;
using APIAggregation.Services.Definitions;
using APIAggregation.Wrappers;
using System.Net.Http;
using System.Text.Json;

namespace APIAggregation.Services;

public class HolidayService : IHolidayService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpClientFactory _httpClientFactory;
    
    public HolidayService(HttpClient httpClient, IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClient;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<Response<List<HolidayDataDto>>> GetHolidayInfo(string countryIsoCode, string languageIsoCode, string validFrom, string validTo)
    {
        // Validate input
        if (string.IsNullOrEmpty(countryIsoCode))
        {
            return new Response<List<HolidayDataDto>>
            {
                Success = false,
                Message = "Country Iso Code cannot be null or empty"
            };
        }

        if (string.IsNullOrEmpty(validFrom))
        {
            return new Response<List<HolidayDataDto>>
            {
                Success = false,
                Message = "Valid From cannot be null or empty"
            };
        }

        if (string.IsNullOrEmpty(validTo))
        {
            return new Response<List<HolidayDataDto>>
            {
                Success = false,
                Message = "Valid To cannot be null or empty"
            };
        }

        // Fetch data from external API
        var result = await FetchFromOpenHolidays(countryIsoCode.ToUpper(), languageIsoCode, validFrom, validTo);

        if (result.Success)
        {
            return new Response<List<HolidayDataDto>>
            {
                Success = true,
                Message = result.Message,
                Data = result.Data
            };
        }

        return new Response<List<HolidayDataDto>>
        {
            Success = false,
            Message = "Failed to fetch Holiday data from external service"
        };
    }

    private async Task<Response<List<HolidayDataDto>>> FetchFromOpenHolidays(string countryIsoCode, string languageIsoCode, string validFrom, string validTo)
    {
        var response = new Response<List<HolidayDataDto>>();
        try
        {
            var httpClient = _httpClientFactory.CreateClient();
            string url = $"https://openholidaysapi.org/SchoolHolidays?countryIsoCode={countryIsoCode}&languageIsoCode={languageIsoCode}&validFrom={validFrom}&validTo={validTo}";

            var httpResponse = await httpClient.GetAsync(url);

            if (httpResponse.IsSuccessStatusCode)
            {
                var jsonResponse = await httpResponse.Content.ReadAsStringAsync();
                
                // Deserialize the JSON response into List<HolidayDataDto>
                var holidayDataList = JsonSerializer.Deserialize<List<HolidayDataDto>>(jsonResponse, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                });

                response.Success = true;
                response.Data = holidayDataList;
            }
            else
            {
                response.Success = false;
                response.Message = $"Failed to retrieve Holiday data: {httpResponse.ReasonPhrase}";
            }
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"An error occurred while fetching Holiday data: {ex.Message}";
        }

        return response;
    }
}
