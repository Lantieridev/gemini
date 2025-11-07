using complejoDeportivo.DTOs.Dashboard;
using complejoDeportivo.Repositories.Dashboard;
using complejoDeportivo.Services.Interfaces;

namespace complejoDeportivo.Services.Implementations
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _repository;

        public DashboardService(IDashboardRepository repository)
        {
            _repository = repository;
        }

        public async Task<DashboardCompletoDto> GetDashboardCompletoAsync(FiltrosDashboardDto filtros)
        {
            // Determinar agrupación (ej. si el rango es > 31 días, agrupar mensual)
            var (inicio, fin) = filtros.ObtenerRangoFechas();
            string agrupacion = (fin - inicio).TotalDays > 31 ? "mensual" : "diario";

            // Usar Task.WhenAll para ejecutar consultas en paralelo
            var resumenTask = _repository.GetResumenAsync(filtros);
            var ingresosTask = _repository.GetIngresosPorPeriodoAsync(filtros, agrupacion);
            var estadosTask = _repository.GetEstadosReservasAsync(filtros);
            var recientesTask = _repository.GetReservasRecientesAsync(filtros);
            var clientesTask = _repository.GetClientesFrecuentesAsync(filtros);
            var canchasTask = _repository.GetCanchasPopularesAsync(filtros);
            var ocupacionTask = _repository.GetOcupacionPorHorarioAsync(filtros);
            var alertasTask = _repository.GetAlertasStockAsync(filtros);

            await Task.WhenAll(
                resumenTask, ingresosTask, estadosTask, 
                recientesTask, clientesTask, canchasTask, 
                ocupacionTask, alertasTask
            );

            // Formatear períodos de ingresos
            var ingresos = ingresosTask.Result;
            ingresos.ForEach(i => i.Periodo = IngresoPeriodoDto.FormatearPeriodo(i.Fecha, agrupacion));

            var dashboard = new DashboardCompletoDto
            {
                Resumen = resumenTask.Result,
                IngresosPorPeriodo = ingresos,
                EstadosDeReservas = estadosTask.Result,
                ReservasRecientes = recientesTask.Result,
                ClientesFrecuentes = clientesTask.Result,
                CanchasPopulares = canchasTask.Result,
                OcupacionPorHorario = ocupacionTask.Result,
                AlertasDeStock = alertasTask.Result
            };

            return dashboard;
        }
    }
}