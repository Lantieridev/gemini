using complejoDeportivo.DTOs.Dashboard;

namespace complejoDeportivo.Repositories.Dashboard
{
    public interface IDashboardRepository
    {
        Task<DashboardResumenDto> GetResumenAsync(FiltrosDashboardDto filtros);
        Task<List<IngresoPeriodoDto>> GetIngresosPorPeriodoAsync(FiltrosDashboardDto filtros, string tipoAgrupacion);
        Task<List<ReservaEstadoDto>> GetEstadosReservasAsync(FiltrosDashboardDto filtros);
        Task<List<ReservaRecienteDto>> GetReservasRecientesAsync(FiltrosDashboardDto filtros);
        Task<List<ClienteFrecuenteDto>> GetClientesFrecuentesAsync(FiltrosDashboardDto filtros);
        Task<List<CanchaPopularDto>> GetCanchasPopularesAsync(FiltrosDashboardDto filtros);
        Task<List<OcupacionHorarioDto>> GetOcupacionPorHorarioAsync(FiltrosDashboardDto filtros);
        Task<List<AlertaStockDto>> GetAlertasStockAsync(FiltrosDashboardDto filtros);
    }
}