namespace APIAggregation.Models.DTOs;

public class HolidayDataDto
{
    public string Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Type { get; set; }
    public List<HolidayNameDto> Name { get; set; }
    public bool Nationwide { get; set; }
    public List<HolidayCommentDto> Comment { get; set; }
}