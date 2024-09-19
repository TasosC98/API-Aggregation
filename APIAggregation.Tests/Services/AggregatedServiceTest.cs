using System;
using Xunit;
using Moq;
using APIAggregation.Services;
using APIAggregation.Models.DTOs;
using APIAggregation.Models.DTOs.Filters;
using APIAggregation.Wrappers;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using APIAggregation.Services.Definitions;

public class AggregatedServiceTests
{
    private readonly Mock<IHolidayService> _holidayServiceMock;
    private readonly Mock<IIpService> _ipServiceMock;
    private readonly Mock<IPotterService> _bookServiceMock;
    private readonly AggregatedService _aggregatedService;

    public AggregatedServiceTests()
    {
        _holidayServiceMock = new Mock<IHolidayService>();
        _ipServiceMock = new Mock<IIpService>();
        _bookServiceMock = new Mock<IPotterService>();

        _aggregatedService = new AggregatedService(
            _holidayServiceMock.Object,
            _ipServiceMock.Object,
            _bookServiceMock.Object
        );
    }

    [Fact]
    public async Task GetAggregatedData_Success_ReturnsAggregatedData()
    {
        // Arrange
        var holidayData = new Response<List<HolidayDataDto>> { Success = true, Data = new List<HolidayDataDto>() };
        var ipData = new Response<IpDataDto> { Success = true, Data = new IpDataDto() };
        var bookData = new Response<List<PotterDataDto>> { Success = true, Data = new List<PotterDataDto>() };

        _holidayServiceMock.Setup(s => s.GetHolidayInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(holidayData);

        _ipServiceMock.Setup(s => s.GetIpInfo(It.IsAny<string>()))
            .ReturnsAsync(ipData);

        _bookServiceMock.Setup(s => s.GetBookInfo(It.IsAny<string>()))
            .ReturnsAsync(bookData);

        // Act
        var result = await _aggregatedService.GetAggregatedData("US", "en", "2023-01-01", "2023-12-31", "123.123.123.123", "en", null);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.NotNull(result.Data.Holidays);
        Assert.NotNull(result.Data.IpData);
        Assert.NotNull(result.Data.Books);
    }

    [Fact]
    public async Task GetAggregatedData_FailureInService_ReturnsFailureResponse()
    {
        // Arrange
        var holidayData = new Response<List<HolidayDataDto>> { Success = false };
        var ipData = new Response<IpDataDto> { Success = true, Data = new IpDataDto() };
        var bookData = new Response<List<PotterDataDto>> { Success = true, Data = new List<PotterDataDto>() };

        _holidayServiceMock.Setup(s => s.GetHolidayInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(holidayData);

        _ipServiceMock.Setup(s => s.GetIpInfo(It.IsAny<string>()))
            .ReturnsAsync(ipData);

        _bookServiceMock.Setup(s => s.GetBookInfo(It.IsAny<string>()))
            .ReturnsAsync(bookData);

        // Act
        var result = await _aggregatedService.GetAggregatedData("US", "en", "2023-01-01", "2023-12-31", "123.123.123.123", "en", null);

        // Assert
        Assert.False(result.Success);
        Assert.Null(result.Data);
        Assert.Equal("Failed to fetch data from one or more services.", result.Message);
    }

    [Fact]
    public async Task GetAggregatedData_HolidayFilterApplied_ReturnsFilteredHolidays()
    {
        // Arrange
        var holidayData = new Response<List<HolidayDataDto>>
        {
            Success = true,
            Data = new List<HolidayDataDto>
            {
                new HolidayDataDto
                {
                    Name = new List<HolidayNameDto>
                    {
                        new HolidayNameDto { Text = "Christmas" }
                    }
                },
                new HolidayDataDto
                {
                    Name = new List<HolidayNameDto>
                    {
                        new HolidayNameDto { Text = "New Year" }
                    }
                }
            }
        };

        var ipData = new Response<IpDataDto> { Success = true, Data = new IpDataDto() };
        var bookData = new Response<List<PotterDataDto>> { Success = true, Data = new List<PotterDataDto>() };

        var holidayFilters = new AggregatedDataFilterDto
        {
            HolidayFilters = new HolidayFilters
            {
                Name = "Christmas"
            }
        };

        _holidayServiceMock.Setup(s => s.GetHolidayInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(holidayData);

        _ipServiceMock.Setup(s => s.GetIpInfo(It.IsAny<string>()))
            .ReturnsAsync(ipData);

        _bookServiceMock.Setup(s => s.GetBookInfo(It.IsAny<string>()))
            .ReturnsAsync(bookData);

        // Act
        var result = await _aggregatedService.GetAggregatedData("US", "en", "2023-01-01", "2023-12-31", "123.123.123.123", "en", holidayFilters);

        // Assert
        Assert.True(result.Success);
        Assert.Single(result.Data.Holidays);
        Assert.Equal("Christmas", result.Data.Holidays.First().Name.First().Text);
    }

    [Fact]
    public async Task GetAggregatedData_SortingApplied_SortsByDateAscending()
    {
        // Arrange
        var holidayData = new Response<List<HolidayDataDto>>
        {
            Success = true,
            Data = new List<HolidayDataDto>
            {
                new HolidayDataDto { StartDate = new DateTime(2023, 12, 25) },
                new HolidayDataDto { StartDate = new DateTime(2023, 1, 1) }
            }
        };

        var ipData = new Response<IpDataDto> { Success = true, Data = new IpDataDto() };
        var bookData = new Response<List<PotterDataDto>> { Success = true, Data = new List<PotterDataDto>() };

        var filters = new AggregatedDataFilterDto
        {
            Sorting = new AggregatedDataSortingDto
            {
                SortBy = "date",
                SortOrder = "asc"
            }
        };

        _holidayServiceMock.Setup(s => s.GetHolidayInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(holidayData);

        _ipServiceMock.Setup(s => s.GetIpInfo(It.IsAny<string>()))
            .ReturnsAsync(ipData);

        _bookServiceMock.Setup(s => s.GetBookInfo(It.IsAny<string>()))
            .ReturnsAsync(bookData);

        // Act
        var result = await _aggregatedService.GetAggregatedData("US", "en", "2023-01-01", "2023-12-31", "123.123.123.123", "en", filters);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data.Holidays);
        Assert.Equal(new DateTime(2023, 1, 1), result.Data.Holidays.First().StartDate);
    }
}
