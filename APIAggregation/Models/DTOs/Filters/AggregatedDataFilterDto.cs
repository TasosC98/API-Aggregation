namespace APIAggregation.Models.DTOs.Filters;

public class AggregatedDataFilterDto
{
    public HolidayFilters? HolidayFilters { get; set; }
    public PotterFilters? PotterFilters { get; set; }
    public AggregatedDataSortingDto? Sorting { get; set; }
}