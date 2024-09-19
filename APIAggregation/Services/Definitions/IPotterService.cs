using APIAggregation.Models.DTOs;
using APIAggregation.Wrappers;

namespace APIAggregation.Services.Definitions;

public interface IPotterService
{
    Task<Response<List<PotterDataDto>>> GetBookInfo(string lang);

}