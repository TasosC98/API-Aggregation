using APIAggregation.Services.Definitions;
using APIAggregation.Models.DTOs;
using APIAggregation.Services.Definitions;
using APIAggregation.Wrappers;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace APIAggregation.Services;

public class PotterService : IPotterService
{
        private readonly HttpClient _httpClient;
        private readonly IHttpClientFactory _httpClientFactory;

        public PotterService(HttpClient httpClient, IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClient;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<Response<List<PotterDataDto>>> GetBookInfo(string lang)
        {
            var result = await FetchFromPotterApi(lang);

            if (result.Success)
            {
                return new Response<List<PotterDataDto>>
                {
                    Success = true,
                    Message = result.Message,
                    Data = result.Data
                };
            }

            return new Response<List<PotterDataDto>>
            {
                Success = false,
                Message = "Failed to fetch book details from the Potter API"
            };
        }

        public async Task<Response<List<PotterDataDto>>> FetchFromPotterApi(string lang)
        {
            var response = new Response<List<PotterDataDto>>();
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                string url = $"https://potterapi-fedeperin.vercel.app/{lang.ToLower()}/books";

                var httpResponse = await httpClient.GetAsync(url);

                if (httpResponse.IsSuccessStatusCode)
                {
                    var jsonResponse = await httpResponse.Content.ReadAsStringAsync();

                    var bookList = JsonSerializer.Deserialize<List<PotterDataDto>>(jsonResponse, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        PropertyNameCaseInsensitive = true
                    });

                    response.Success = true;
                    response.Data = bookList;
                }
                else
                {
                    response.Success = false;
                    response.Message = $"Failed to retrieve book details: {httpResponse.ReasonPhrase}";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"An error occurred while fetching book details: {ex.Message}";
            }

            return response;
        }

}

