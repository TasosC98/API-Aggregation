using APIAggregation.Models.DTOs;
using APIAggregation.Wrappers;

namespace APIAggregation.Services.Definitions;

public interface IHolidayService
{
    Task<Response<List<HolidayDataDto>>> GetHolidayInfo(string countryIsoCode, string languageIsoCode, string validFrom,
        string validTo);

}