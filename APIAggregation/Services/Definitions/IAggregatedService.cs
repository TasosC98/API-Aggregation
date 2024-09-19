using APIAggregation.Models.DTOs;
using APIAggregation.Models.DTOs.Filters;
using APIAggregation.Wrappers;

namespace APIAggregation.Services.Definitions;

public interface IAggregatedService
{
    Task<Response<AggregatedDataDto>> GetAggregatedData(string countryIsoCode, string languageIsoCode, string validFrom, string validTo, string ip, string lang, AggregatedDataFilterDto aggregatedDataFilters);

}