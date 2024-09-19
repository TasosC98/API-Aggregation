using APIAggregation.Models.DTOs;
using APIAggregation.Wrappers;

namespace APIAggregation.Services.Definitions;

public interface IIpService
{
    Task<Response<IpDataDto>> GetIpInfo(string ip);
}