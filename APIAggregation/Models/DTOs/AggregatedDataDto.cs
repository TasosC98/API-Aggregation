namespace APIAggregation.Models.DTOs;

public class AggregatedDataDto
{
        public List<HolidayDataDto> Holidays { get; set; }
        public IpDataDto IpData { get; set; }
        public List<PotterDataDto> Books { get; set; }
}