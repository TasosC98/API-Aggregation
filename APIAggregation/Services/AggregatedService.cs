using APIAggregation.Models.DTOs;
using APIAggregation.Services.Definitions;
using APIAggregation.Wrappers;
using System.Collections.Generic;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;
using APIAggregation.Models.DTOs.Filters;
using System.Globalization;

namespace APIAggregation.Services
{
    public class AggregatedService : IAggregatedService
    {
        private readonly IHolidayService _holidayService;
        private readonly IIpService _ipService;
        private readonly IPotterService _bookService;

        public AggregatedService(IHolidayService holidayService, IIpService ipService, IPotterService bookService)
        {
            _holidayService = holidayService;
            _ipService = ipService;
            _bookService = bookService;
        }

        public async Task<Response<AggregatedDataDto>> GetAggregatedData(string holidayCountryCode, string languageIsoCode, string validFrom, string validTo, string ip, string lang, AggregatedDataFilterDto? aggregatedDataFilters)
        {
            
            // Call the HolidayService
            var holidayTask =  _holidayService.GetHolidayInfo(holidayCountryCode.ToUpper(), languageIsoCode, validFrom, validTo);
            
            // Call the IpService
            var ipTask =  _ipService.GetIpInfo(ip);

            // Call the PotterService
            var bookTask =  _bookService.GetBookInfo(lang);

            await Task.WhenAll(holidayTask, ipTask, bookTask);
            
            // Combine the Tasks
            var holidayData = await holidayTask;
            var ipData = await ipTask;
            var bookData = await bookTask;
            
            if (holidayData.Success && ipData.Success && bookData.Success)
            {
                var aggregatedData = new AggregatedDataDto
                {
                    Holidays = holidayData.Data,
                    IpData = ipData.Data,
                    Books = bookData.Data
                };

                if (aggregatedDataFilters != null)
                {
                    if (aggregatedDataFilters.HolidayFilters != null)
                    {
                        var name = aggregatedDataFilters.HolidayFilters.Name;
                        
                        if (!string.IsNullOrEmpty(name) && aggregatedData.Holidays != null)
                        {
                            aggregatedData.Holidays = aggregatedData.Holidays
                                .Where(h => h.Name.Any(n => n.Text.Contains(name.Trim(), StringComparison.OrdinalIgnoreCase)))
                                .ToList();
                        }
                    }

                    if (aggregatedDataFilters.PotterFilters != null)
                    {
                        if (aggregatedDataFilters.PotterFilters.Date != null)
                        {
                            var date = DateTime.Parse(aggregatedDataFilters.PotterFilters.Date);
                            
                            if (aggregatedData.Books != null)
                            {
                                var formats = new[] { "MMM d, yyyy", "MMM dd, yyyy" };

                                aggregatedData.Books = aggregatedData.Books
                                    .Where(b => DateTime.TryParseExact(b.ReleaseDate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate) && parsedDate >= date)
                                    .ToList();
                            }
                        }
                    }
                    
                    if (aggregatedDataFilters.Sorting != null)
                    {
                        var sortBy = aggregatedDataFilters.Sorting.SortBy;
                        var sortOrder = aggregatedDataFilters.Sorting.SortOrder;
                        
                        if (!string.IsNullOrEmpty(sortBy) && !string.IsNullOrEmpty(sortOrder))
                        {
                            switch (sortBy)
                            {
                                case "date" :
                                    switch (sortOrder)
                                    {
                                        case "asc" :
                                            if (aggregatedData.Holidays != null)
                                                aggregatedData.Holidays = aggregatedData.Holidays
                                                    .OrderBy(h => h.StartDate)
                                                    .ToList();
                                            if (aggregatedData.Books != null)
                                                aggregatedData.Books = aggregatedData.Books
                                                    .OrderBy(b => DateTime.Parse(b.ReleaseDate)).ToList();
                                            break;
                                        case "desc" :
                                            if (aggregatedData.Holidays != null)
                                                aggregatedData.Holidays = aggregatedData.Holidays
                                                    .OrderByDescending(h => h.StartDate)
                                                    .ToList();
                                            if (aggregatedData.Books != null)
                                                aggregatedData.Books = aggregatedData.Books
                                                    .OrderByDescending(b => DateTime.Parse(b.ReleaseDate)).ToList();
                                            break;
                                    }
                                    break;
                            }
                        }
                    }
                }

                return new Response<AggregatedDataDto>
                {
                    Success = true,
                    Data = aggregatedData,
                };
            }
            
            return new Response<AggregatedDataDto>
            {
                Success = false,
                Message = "Failed to fetch data from one or more services."
            };
        }
    }
}