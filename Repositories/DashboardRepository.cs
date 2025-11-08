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

        private IQueryable<Reserva> AplicarFiltros(FiltrosDashboardDto filtros)
        {
            var (inicio, fin) = filtros.ObtenerRangoFechas();

            // Query base filtrada por rango de fechas
            var query = _context.Reservas
                .Include(r => r.DetalleReservas)
                    .ThenInclude(d => d.Cancha)
                .Where(r => r.Fecha >= DateOnly.FromDateTime(inicio) && r.Fecha <= DateOnly.FromDateTime(fin));

            // Filtro principal (OBLIGATORIO) por ComplejoId
            query = query.Where(r => r.DetalleReservas.Any(d => d.Cancha.ComplejoId == filtros.ComplejoId));

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

            return query;
        }

        // Este es el método que estaba bien (secuencial)
        public async Task<DashboardResumenDto> GetResumenAsync(FiltrosDashboardDto filtros)
        {
            var (inicio, fin) = filtros.ObtenerRangoFechas();
            var hoy = DateOnly.FromDateTime(DateTime.Today);
            var mesActualInicio = new DateOnly(hoy.Year, hoy.Month, 1);
            var mesAnteriorInicio = mesActualInicio.AddMonths(-1);
            var mesAnteriorFin = mesActualInicio.AddDays(-1);

            var queryBaseComplejo = _context.Reservas
                .Include(r => r.DetalleReservas)
                    .ThenInclude(d => d.Cancha)
                .Where(r => r.DetalleReservas.Any(d => d.Cancha.ComplejoId == filtros.ComplejoId));

            var queryFiltrada = AplicarFiltros(filtros);

            // Tarea 1: Métricas de Hoy
            var ingresosHoy = await queryBaseComplejo.Where(r => r.Fecha == hoy).SumAsync(r => r.Total);
            var reservasHoy = await queryBaseComplejo.CountAsync(r => r.Fecha == hoy);
            var clientesNuevosHoy = await _context.Clientes.CountAsync(c => c.FechaRegistro.Date == DateTime.Today);

            // Tarea 2: Métricas de Estado
            var estados = await queryFiltrada
                .Include(r => r.EstadoReserva)
                .GroupBy(r => r.EstadoReserva.Nombre)
                .Select(g => new { Estado = g.Key, Cantidad = g.Count() })
                .ToListAsync();

            // Tarea 3: Infraestructura
            var canchas = await _context.Canchas
                .Where(c => c.ComplejoId == filtros.ComplejoId)
                .GroupBy(c => 1)
                .Select(g => new
                {
                    Totales = g.Count(),
                    Activas = g.Count(c => c.Activa)
                })
                .FirstOrDefaultAsync();

            // Tarea 4: Comparativas
            var ingresosMesActual = await queryBaseComplejo.Where(r => r.Fecha >= mesActualInicio && r.Fecha <= hoy).SumAsync(r => r.Total);
            var ingresosMesAnterior = await queryBaseComplejo.Where(r => r.Fecha >= mesAnteriorInicio && r.Fecha <= mesAnteriorFin).SumAsync(r => r.Total);
            var reservasMesActual = await queryBaseComplejo.CountAsync(r => r.Fecha >= mesActualInicio && r.Fecha <= hoy);
            var reservasMesAnterior = await queryBaseComplejo.CountAsync(r => r.Fecha >= mesAnteriorInicio && r.Fecha <= mesAnteriorFin);

            // Tarea 5: Total Clientes
            var totalClientes = await _context.Clientes.CountAsync();

            var canchasInfo = canchas ?? new { Totales = 0, Activas = 0 };

            var resumen = new DashboardResumenDto
            {
                IngresosHoy = ingresosHoy,
                ReservasHoy = reservasHoy,
                ClientesNuevosHoy = clientesNuevosHoy,
                ReservasPendientes = estados.FirstOrDefault(e => e.Estado == "Pendiente")?.Cantidad ?? 0,
                ReservasConfirmadas = estados.FirstOrDefault(e => e.Estado == "Confirmada")?.Cantidad ?? 0,
                ReservasCanceladas = estados.FirstOrDefault(e => e.Estado == "Cancelada")?.Cantidad ?? 0,
                CanchasTotales = canchasInfo.Totales,
                CanchasActivas = canchasInfo.Activas,
                IngresosMesActual = ingresosMesActual,
                IngresosMesAnterior = ingresosMesAnterior,
                ReservasMesActual = reservasMesActual,
                ReservasMesAnterior = reservasMesAnterior,
                TotalClientesRegistrados = totalClientes
            };

            return resumen;
        }

        // [CORREGIDO] Este es el método que fallaba
        public async Task<List<IngresoPeriodoDto>> GetIngresosPorPeriodoAsync(FiltrosDashboardDto filtros, string tipoAgrupacion = "diario")
        {
            var query = AplicarFiltros(filtros);

            if (tipoAgrupacion == "diario")
            {
                // --- INICIO CORRECCIÓN DIARIO ---
                // 1. Traemos los datos agrupados a memoria, PERO trayendo la lista de IDs de cliente
                var datosDiarios = await query
                    .GroupBy(r => r.Fecha)
                    .Select(g => new // Objeto anónimo temporal
                    {
                        FechaKey = g.Key,
                        Total = g.Sum(r => r.Total),
                        CantidadReservas = g.Count(),
                        ClienteIds = g.Select(r => r.ClienteId).ToList() // Traemos los IDs
                    })
                    .OrderBy(dto => dto.FechaKey)
                    .ToListAsync(); // <-- Traemos a memoria

                // 2. Ahora calculamos el Distinct().Count() en C# (LINQ-to-Objects)
                return datosDiarios.Select(dto => new IngresoPeriodoDto
                {
                    Fecha = dto.FechaKey.ToDateTime(TimeOnly.MinValue),
                    Total = dto.Total,
                    CantidadReservas = dto.CantidadReservas,
                    CantidadClientes = dto.ClienteIds.Distinct().Count() // <-- Esto ahora es C#, no SQL
                }).ToList();
                // --- FIN CORRECCIÓN DIARIO ---
            }

            // --- INICIO CORRECCIÓN MENSUAL ---
            // 1. Hacemos lo mismo para la agrupación mensual
            var datosMensuales = await query
                .GroupBy(r => new { r.Fecha.Year, r.Fecha.Month })
                .Select(g => new // Objeto anónimo temporal
                {
                    FechaKey = g.Key,
                    Total = g.Sum(r => r.Total),
                    CantidadReservas = g.Count(),
                    ClienteIds = g.Select(r => r.ClienteId).ToList() // Traemos los IDs
                })
                .OrderBy(dto => dto.FechaKey.Year).ThenBy(dto => dto.FechaKey.Month)
                .ToListAsync(); // <-- Traemos a memoria

            // 2. Ahora calculamos en C#
            return datosMensuales.Select(dto => new IngresoPeriodoDto
            {
                Fecha = new DateTime(dto.FechaKey.Year, dto.FechaKey.Month, 1),
                Total = dto.Total,
                CantidadReservas = dto.CantidadReservas,
                CantidadClientes = dto.ClienteIds.Distinct().Count() // <-- Esto ahora es C#, no SQL
            }).ToList();
            // --- FIN CORRECCIÓN MENSUAL ---
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
                .Where(d => d.Cancha.ComplejoId == filtros.ComplejoId)
                .GroupBy(d => new { d.CanchaId, d.Cancha.Nombre, TipoCancha = d.Cancha.TipoCancha.Nombre })
                .Select(g => new CanchaPopularDto
                {
                    CanchaId = g.Key.CanchaId,
                    Nombre = g.Key.Nombre,
                    TipoCancha = g.Key.TipoCancha,
                    ReservasCount = g.Count(),
                    IngresosTotales = g.Sum(d => d.Subtotal),
                    HorasTotales = g.Sum(d => d.CantidadHoras)
                })
                .OrderByDescending(dto => dto.ReservasCount)
                .Take(5)
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
                .Take(filtros.TamanoPagina)
                .Select(r => new ReservaRecienteDto
                {
                    ReservaId = r.ReservaId,
                    ClienteNombre = (r.Cliente != null) ? (r.Cliente.Nombre + " " + r.Cliente.Apellido) : "N/A",
                    ClienteEmail = (r.Cliente != null) ? (r.Cliente.Email ?? string.Empty) : string.Empty,

                    CanchaNombre = r.DetalleReservas.FirstOrDefault() != null ?
                                     r.DetalleReservas.First().Cancha.Nombre :
                                     "N/A",

                    TipoCancha = r.DetalleReservas.FirstOrDefault() != null && r.DetalleReservas.First().Cancha.TipoCancha != null ?
                                   r.DetalleReservas.First().Cancha.TipoCancha.Nombre :
                                   "N/A",

                    Fecha = r.Fecha.ToDateTime(TimeOnly.MinValue),
                    HoraInicio = r.HoraInicio.ToTimeSpan(),
                    HoraFin = r.HoraFin.ToTimeSpan(),
                    Estado = (r.EstadoReserva != null) ? r.EstadoReserva.Nombre : "Desconocido",
                    Total = r.Total,
                    FechaCreacion = r.FechaCreacion
                })
                .Take(5)
                .ToListAsync();
        }

        public Task<List<OcupacionHorarioDto>> GetOcupacionPorHorarioAsync(FiltrosDashboardDto filtros)
        {
            return Task.FromResult(OcupacionHorarioDto.CrearFranjasHorarias());
        }

        public Task<List<AlertaStockDto>> GetAlertasStockAsync(FiltrosDashboardDto filtros)
        {
            return Task.FromResult(new List<AlertaStockDto>());
        }
    }
}