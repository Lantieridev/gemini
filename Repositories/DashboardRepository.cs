using complejoDeportivo.DTOs.Dashboard;
using complejoDeportivo.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace complejoDeportivo.Repositories.Dashboard
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly ComplejoDeportivoContext _context;

        public DashboardRepository(ComplejoDeportivoContext context)
        {
            _context = context;
        }

        // [MODIFICADO] (Problema 2c) - Método 'AplicarFiltros' actualizado
        private IQueryable<Reserva> AplicarFiltros(FiltrosDashboardDto filtros)
        {
            var (inicio, fin) = filtros.ObtenerRangoFechas();

            // Query base filtrada por rango de fechas
            var query = _context.Reservas
                .Include(r => r.DetalleReservas)
                    .ThenInclude(d => d.Cancha)
                .Where(r => r.Fecha >= DateOnly.FromDateTime(inicio) && r.Fecha <= DateOnly.FromDateTime(fin));

            // Filtro principal (OBLIGATORIO) por ComplejoId
            // Se aplica sobre los detalles de la reserva
            query = query.Where(r => r.DetalleReservas.Any(d => d.Cancha.ComplejoId == filtros.ComplejoId));

            // --- [NUEVO] Aplicación de filtros adicionales del DTO avanzado ---

            if (filtros.CanchaId.HasValue)
            {
                query = query.Where(r => r.DetalleReservas.Any(d => d.CanchaId == filtros.CanchaId.Value));
            }

            if (filtros.TipoCanchaId.HasValue)
            {
                query = query.Where(r => r.DetalleReservas.Any(d => d.Cancha.TipoCanchaId == filtros.TipoCanchaId.Value));
            }

            if (filtros.EstadoReservaId.HasValue)
            {
                query = query.Where(r => r.EstadoReservaId == filtros.EstadoReservaId.Value);
            }

            if (filtros.ClienteId.HasValue)
            {
                query = query.Where(r => r.ClienteId == filtros.ClienteId.Value);
            }

            // Nota: Los filtros de AsadorId y EstadoPagoId requerirían joins adicionales
            // que no están en el modelo de Reserva. Se omiten por ahora
            // a menos que modifiques la consulta para incluir 'Facturas' y 'Pagos'.

            return query;
        }

        // [MODIFICADO] (Problema 2a) - Método 'GetResumenAsync' optimizado
        public async Task<DashboardResumenDto> GetResumenAsync(FiltrosDashboardDto filtros)
        {
            var (inicio, fin) = filtros.ObtenerRangoFechas();
            var hoy = DateOnly.FromDateTime(DateTime.Today);
            var mesActualInicio = new DateOnly(hoy.Year, hoy.Month, 1);
            var mesAnteriorInicio = mesActualInicio.AddMonths(-1);
            var mesAnteriorFin = mesActualInicio.AddDays(-1);

            // Query base para métricas del complejo
            var queryBaseComplejo = _context.Reservas
                .Include(r => r.DetalleReservas)
                    .ThenInclude(d => d.Cancha)
                .Where(r => r.DetalleReservas.Any(d => d.Cancha.ComplejoId == filtros.ComplejoId));

            // Query base para métricas filtradas
            var queryFiltrada = AplicarFiltros(filtros);

            // Tarea 1: Métricas de Hoy (paralelo)
            var ingresosHoyTask = queryBaseComplejo.Where(r => r.Fecha == hoy).SumAsync(r => r.Total);
            var reservasHoyTask = queryBaseComplejo.CountAsync(r => r.Fecha == hoy);
            var clientesNuevosHoyTask = _context.Clientes.CountAsync(c => c.FechaRegistro.Date == DateTime.Today);

            // Tarea 2: Métricas de Estado (paralelo)
            var estadosTask = queryFiltrada
                .Include(r => r.EstadoReserva)
                .GroupBy(r => r.EstadoReserva.Nombre)
                .Select(g => new { Estado = g.Key, Cantidad = g.Count() })
                .ToListAsync();

            // Tarea 3: Infraestructura (paralelo)
            var canchasTask = _context.Canchas
                .Where(c => c.ComplejoId == filtros.ComplejoId)
                .GroupBy(c => 1) // Agrupación ficticia para sumar
                .Select(g => new
                {
                    Totales = g.Count(),
                    Activas = g.Count(c => c.Activa)
                })
                .FirstOrDefaultAsync();

            // Tarea 4: Comparativas (paralelo)
            var ingresosMesActualTask = queryBaseComplejo.Where(r => r.Fecha >= mesActualInicio && r.Fecha <= hoy).SumAsync(r => r.Total);
            var ingresosMesAnteriorTask = queryBaseComplejo.Where(r => r.Fecha >= mesAnteriorInicio && r.Fecha <= mesAnteriorFin).SumAsync(r => r.Total);
            var reservasMesActualTask = queryBaseComplejo.CountAsync(r => r.Fecha >= mesActualInicio && r.Fecha <= hoy);
            var reservasMesAnteriorTask = queryBaseComplejo.CountAsync(r => r.Fecha >= mesAnteriorInicio && r.Fecha <= mesAnteriorFin);

            // Tarea 5: Total Clientes (paralelo)
            var totalClientesTask = _context.Clientes.CountAsync();

            // Ejecutar todas las tareas en paralelo
            await Task.WhenAll(
                ingresosHoyTask, reservasHoyTask, clientesNuevosHoyTask,
                estadosTask,
                canchasTask,
                ingresosMesActualTask, ingresosMesAnteriorTask, reservasMesActualTask, reservasMesAnteriorTask,
                totalClientesTask
            );

            // Procesar resultados
            var estados = estadosTask.Result;
            var canchas = canchasTask.Result ?? new { Totales = 0, Activas = 0 }; // Manejar nulo si no hay canchas

            var resumen = new DashboardResumenDto
            {
                // Métricas de Hoy
                IngresosHoy = ingresosHoyTask.Result,
                ReservasHoy = reservasHoyTask.Result,
                ClientesNuevosHoy = clientesNuevosHoyTask.Result,

                // Métricas de Estado (basadas en filtros)
                ReservasPendientes = estados.FirstOrDefault(e => e.Estado == "Pendiente")?.Cantidad ?? 0,
                ReservasConfirmadas = estados.FirstOrDefault(e => e.Estado == "Confirmada")?.Cantidad ?? 0,
                ReservasCanceladas = estados.FirstOrDefault(e => e.Estado == "Cancelada")?.Cantidad ?? 0,

                // Infraestructura
                CanchasTotales = canchas.Totales,
                CanchasActivas = canchas.Activas,

                // Comparativas
                IngresosMesActual = ingresosMesActualTask.Result,
                IngresosMesAnterior = ingresosMesAnteriorTask.Result,
                ReservasMesActual = reservasMesActualTask.Result,
                ReservasMesAnterior = reservasMesAnteriorTask.Result,

                // Clientes
                TotalClientesRegistrados = totalClientesTask.Result
            };

            return resumen;
        }

        public async Task<List<IngresoPeriodoDto>> GetIngresosPorPeriodoAsync(FiltrosDashboardDto filtros, string tipoAgrupacion = "diario")
        {
            var query = AplicarFiltros(filtros);

            if (tipoAgrupacion == "diario")
            {
                return await query
                   .GroupBy(r => r.Fecha)
                   .Select(g => new IngresoPeriodoDto
                   {
                       Fecha = g.Key.ToDateTime(TimeOnly.MinValue),
                       Total = g.Sum(r => r.Total),
                       CantidadReservas = g.Count(),
                       CantidadClientes = g.Select(r => r.ClienteId).Distinct().Count()
                   })
                   .OrderBy(dto => dto.Fecha)
                   .ToListAsync();
            }

            // Agrupación mensual
            return await query
                .GroupBy(r => new { r.Fecha.Year, r.Fecha.Month })
                .Select(g => new IngresoPeriodoDto
                {
                    Fecha = new DateTime(g.Key.Year, g.Key.Month, 1),
                    Total = g.Sum(r => r.Total),
                    CantidadReservas = g.Count(),
                    CantidadClientes = g.Select(r => r.ClienteId).Distinct().Count()
                })
                .OrderBy(dto => dto.Fecha)
                .ToListAsync();
        }

        public async Task<List<ReservaEstadoDto>> GetEstadosReservasAsync(FiltrosDashboardDto filtros)
        {
            var query = AplicarFiltros(filtros);

            var datos = await query
                .Include(r => r.EstadoReserva)
                .GroupBy(r => r.EstadoReserva.Nombre)
                .Select(g => new ReservaEstadoDto
                {
                    Estado = g.Key,
                    Cantidad = g.Count()
                })
                .ToListAsync();

            ReservaEstadoDto.CalcularPorcentajes(datos);
            return datos;
        }

        public async Task<List<CanchaPopularDto>> GetCanchasPopularesAsync(FiltrosDashboardDto filtros)
        {
            var query = AplicarFiltros(filtros);

            var canchas = await query
                .SelectMany(r => r.DetalleReservas)
                .Include(d => d.Cancha.TipoCancha)
                // [MODIFICADO] Asegurarse de que el filtro de complejo se aplique aquí también
                .Where(d => d.Cancha.ComplejoId == filtros.ComplejoId)
                .GroupBy(d => new { d.CanchaId, d.Cancha.Nombre, TipoCancha = d.Cancha.TipoCancha.Nombre })
                .Select(g => new CanchaPopularDto
                {
                    CanchaId = g.Key.CanchaId,
                    Nombre = g.Key.Nombre,
                    TipoCancha = g.Key.TipoCancha,
                    ReservasCount = g.Count(), // Esto cuenta detalles de reserva
                    IngresosTotales = g.Sum(d => d.Subtotal),
                    HorasTotales = g.Sum(d => d.CantidadHoras)
                })
                .OrderByDescending(dto => dto.ReservasCount)
                .Take(10)
                .ToListAsync();

            return CanchaPopularDto.AplicarRanking(canchas);
        }

        public async Task<List<ClienteFrecuenteDto>> GetClientesFrecuentesAsync(FiltrosDashboardDto filtros)
        {
            var query = AplicarFiltros(filtros);

            var clientes = await query
                .Include(r => r.Cliente)
                .GroupBy(r => r.Cliente)
                .Select(g => new ClienteFrecuenteDto
                {
                    ClienteId = g.Key.ClienteId,
                    NombreCompleto = g.Key.Nombre + " " + g.Key.Apellido,
                    Email = g.Key.Email ?? string.Empty,
                    Telefono = g.Key.Telefono ?? string.Empty,
                    TotalReservas = g.Count(),
                    TotalGastado = g.Sum(r => r.Total),
                    UltimaReserva = g.Max(r => r.Fecha).ToDateTime(TimeOnly.MinValue),
                    FechaRegistro = g.Key.FechaRegistro
                })
                .OrderByDescending(dto => dto.TotalReservas)
                .Take(10)
                .ToListAsync();

            return ClienteFrecuenteDto.AplicarRanking(clientes);
        }

        // [CONFIRMADO] (Problema 2b)
        // La corrección para CS8072 (NullReferenceException) ya está aplicada
        // en el archivo que subiste. Esta implementación es segura.
        public Task<List<ReservaRecienteDto>> GetReservasRecientesAsync(FiltrosDashboardDto filtros)
        {
            var query = AplicarFiltros(filtros);

            return query
                .Include(r => r.Cliente)
                .Include(r => r.EstadoReserva)
                .Include(r => r.DetalleReservas)
                    .ThenInclude(d => d.Cancha)
                        .ThenInclude(c => c.TipoCancha)
                .OrderByDescending(r => r.FechaCreacion)
                .Take(filtros.TamanoPagina) // [NUEVO] Usar paginación del DTO
                .Select(r => new ReservaRecienteDto
                {
                    ReservaId = r.ReservaId,
                    ClienteNombre = (r.Cliente != null) ? (r.Cliente.Nombre + " " + r.Cliente.Apellido) : "N/A",
                    ClienteEmail = (r.Cliente != null) ? (r.Cliente.Email ?? string.Empty) : string.Empty,

                    // --- INICIO DE LA CORRECCIÓN (CS8072) ---
                    // Esta proyección es segura y ya estaba en tu archivo
                    CanchaNombre = r.DetalleReservas.FirstOrDefault() != null ?
                                     r.DetalleReservas.First().Cancha.Nombre :
                                     "N/A",

                    TipoCancha = r.DetalleReservas.FirstOrDefault() != null && r.DetalleReservas.First().Cancha.TipoCancha != null ?
                                   r.DetalleReservas.First().Cancha.TipoCancha.Nombre :
                                   "N/A",
                    // --- FIN DE LA CORRECCIÓN ---

                    Fecha = r.Fecha.ToDateTime(TimeOnly.MinValue),
                    HoraInicio = r.HoraInicio.ToTimeSpan(),
                    HoraFin = r.HoraFin.ToTimeSpan(),
                    Estado = (r.EstadoReserva != null) ? r.EstadoReserva.Nombre : "Desconocido",
                    Total = r.Total,
                    FechaCreacion = r.FechaCreacion
                })
                .ToListAsync();
        }

        public Task<List<OcupacionHorarioDto>> GetOcupacionPorHorarioAsync(FiltrosDashboardDto filtros)
        {
            // Lógica omitida por brevedad, como en el original
            return Task.FromResult(OcupacionHorarioDto.CrearFranjasHorarias());
        }

        public Task<List<AlertaStockDto>> GetAlertasStockAsync(FiltrosDashboardDto filtros)
        {
            // Lógica omitida por brevedad, como en el original
            return Task.FromResult(new List<AlertaStockDto>());
        }
    }
}