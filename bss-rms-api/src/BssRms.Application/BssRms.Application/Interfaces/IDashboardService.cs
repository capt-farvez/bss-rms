using BssRms.Application.DTOs.Dashboard;

namespace BssRms.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardStatsDto> GetStatsAsync();
}
