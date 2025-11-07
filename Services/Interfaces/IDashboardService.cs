using complejoDeportivo.DTOs.Dashboard;

namespace complejoDeportivo.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardCompletoDto> GetDashboardCompletoAsync(FiltrosDashboardDto filtros);
    }
}