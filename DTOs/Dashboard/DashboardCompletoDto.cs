using System.Collections.Generic;

namespace complejoDeportivo.DTOs.Dashboard
{
    public class DashboardCompletoDto
    {
        // Resumen general (KPIs / Tarjetas)
        public DashboardResumenDto? Resumen { get; set; }

        // Gráfico de Ingresos
        public List<IngresoPeriodoDto>? IngresosPorPeriodo { get; set; }

        // Gráfico de Estados de Reserva
        public List<ReservaEstadoDto>? EstadosDeReservas { get; set; }

        // Lista de Reservas Recientes
        public List<ReservaRecienteDto>? ReservasRecientes { get; set; }

        // Lista de Clientes Frecuentes
        public List<ClienteFrecuenteDto>? ClientesFrecuentes { get; set; }

        // Lista de Canchas Populares (como la de tu captura)
        public List<CanchaPopularDto>? CanchasPopulares { get; set; }

        // Gráfico de Ocupación por Horario
        public List<OcupacionHorarioDto>? OcupacionPorHorario { get; set; }

        // Alertas (Ej. Stock bajo)
        public List<AlertaStockDto>? AlertasDeStock { get; set; }
    }
}