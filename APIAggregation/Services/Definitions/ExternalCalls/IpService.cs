using APIAggregation.Models.DTOs;
using APIAggregation.Services.Definitions;
using APIAggregation.Wrappers;

namespace APIAggregation.Services;
public class IpService : IIpService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpClientFactory _httpClientFactory;


    public IpService(HttpClient httpClient,  IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClient;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<Response<IpDataDto>> GetIpInfo(string ip)
    {

        if (string.IsNullOrEmpty(ip))
        {
            return new Response<IpDataDto>
            {
                Success = false,
                Message = "IP address cannot be null or empty"
            };
        }
        
        //Fetch from External API (IP2C)
        var result = await FetchFromIp2C(ip);

        if (result.Success)
        {
            return new Response<IpDataDto>
            {
                Success = true,
                Message = result.Message,
                Data = result.Data
            };
        }

        // Return failure if API call fails
        return new Response<IpDataDto>
        {
            Success = false,
            Message = "Failed to fetch IP details from external service"
        };
    }
    
    public async Task<Response<IpDataDto>> FetchFromIp2C(string ip)
{
    var response = new Response<IpDataDto>();
    try
    {
        var httpClient = _httpClientFactory.CreateClient();
        string url = $"https://ip2c.org/{ip}";

        var httpResponse = await httpClient.GetAsync(url);

        if (httpResponse.IsSuccessStatusCode)
        {
            var result = await httpResponse.Content.ReadAsStringAsync();
            var parts = result.Split(';');

            // The first digit of the response indicates the status.
            switch (parts[0])
            {
                case "0": // WRONG INPUT
                    response.Success = false;
                    response.Message = "Invalid IP address or input. Please provide a valid IPv4 address.";
                    break;

                case "1": // Successful response with country data
                    if (parts.Length == 4)
                    {
                        response.Data = new IpDataDto()
                        {
                            CountryCode = parts[1],
                            CountryCode3 = parts[2],
                            CountryName = parts[3],
                            IpDetails = ip
                        };
                        response.Success = true;
                    }
                    else
                    {
                        response.Success = false;
                        response.Message = "Unexpected response format from IP2C service.";
                    }
                    break;

                case "2": // Deprecated / Unknown IP
                    response.Success = false;
                    response.Message = "The given IP address was not found in the database or is unassigned.";
                    break;

                default: // Catch-all for unexpected responses
                    response.Success = false;
                    response.Message = "Unknown response code received from the IP2C service.";
                    break;
            }
        }
        else
        {
            response.Success = false;
            response.Message = $"Failed to retrieve IP details: {httpResponse.ReasonPhrase}";
        }
    }
    catch (Exception ex)
    {
        response.Success = false;
        response.Message = $"An error occurred while fetching IP details: {ex.Message}";
    }

    return response;
    }
}



