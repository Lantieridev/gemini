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

            // --- INICIO DE LA CORRECCIÓN ---
            // Ejecutamos las tareas secuencialmente para evitar el error de DbContext.
            // La optimización de Task.WhenAll se mantiene DENTRO de GetResumenAsync, 
            // pero el servicio debe esperar a que termine antes de empezar la siguiente.

            var resumen = await _repository.GetResumenAsync(filtros);
            var ingresos = await _repository.GetIngresosPorPeriodoAsync(filtros, agrupacion);
            var estados = await _repository.GetEstadosReservasAsync(filtros);
            var recientes = await _repository.GetReservasRecientesAsync(filtros);
            var clientes = await _repository.GetClientesFrecuentesAsync(filtros);
            var canchas = await _repository.GetCanchasPopularesAsync(filtros);
            var ocupacion = await _repository.GetOcupacionPorHorarioAsync(filtros);
            var alertas = await _repository.GetAlertasStockAsync(filtros);

            // --- FIN DE LA CORRECCIÓN ---

            // Formatear períodos de ingresos
            ingresos.ForEach(i => i.Periodo = IngresoPeriodoDto.FormatearPeriodo(i.Fecha, agrupacion));

            var dashboard = new DashboardCompletoDto
            {
                Resumen = resumen,
                IngresosPorPeriodo = ingresos,
                EstadosDeReservas = estados,
                ReservasRecientes = recientes,
                ClientesFrecuentes = clientes,
                CanchasPopulares = canchas,
                OcupacionPorHorario = ocupacion,
                AlertasDeStock = alertas
            };

            return dashboard;
        }
    }
}