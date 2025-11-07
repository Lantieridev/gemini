using complejoDeportivo.DTOs;
using complejoDeportivo.Models;
using Microsoft.EntityFrameworkCore; 
using System.Threading.Tasks; 
using System.Collections.Generic; // Agregado
using System.Linq; // Agregado
using System; // Agregado

namespace complejoDeportivo.Repositories
{
    public class ReservaRepository : IReservaRepository
    {
        private readonly ComplejoDeportivoContext _contexto;
        private readonly TimeOnly _horaLuz = new TimeOnly(19, 0, 0); // Hora a la que entra la tarifa nocturna

        public ReservaRepository(ComplejoDeportivoContext contexto)
        {
            _contexto = contexto;
        }

        public List<DisponibilidadCanchaDTO> ObtenerTurnosDisponibles(int canchaId, DateOnly fecha, TimeOnly apertura, TimeOnly cierre)
        {
            var turnos = new List<DisponibilidadCanchaDTO>();
            var hora = apertura;

            while (hora.AddHours(1) <= cierre)
            {
                var horaFin = hora.AddHours(1);

                if (!ExisteReservaSuperpuesta(canchaId, fecha, hora, horaFin)
                    && !ExisteBloqueo(canchaId, fecha, hora, horaFin))
                {
                    turnos.Add(new DisponibilidadCanchaDTO
                    {
                        CanchaId = canchaId,
                        Fecha = fecha,
                        HoraInicio = hora,
                        HoraFin = horaFin
                    });
                }

                hora = hora.AddHours(1);
            }

            return turnos;
        }

        public bool ExisteReservaSuperpuesta(int canchaId, DateOnly fecha, TimeOnly inicio, TimeOnly fin)
        {
            return (from r in _contexto.Reservas
                    join d in _contexto.DetalleReservas on r.ReservaId equals d.ReservaId
                    where d.CanchaId == canchaId
                    && r.Fecha == fecha
                    && r.HoraInicio < fin
                    && r.HoraFin > inicio
                    select r).Any();
        }

        public bool ExisteBloqueo(int canchaId, DateOnly fecha, TimeOnly inicio, TimeOnly fin)
        {
            return _contexto.BloqueoCanchas
                .Any(b => b.CanchaId == canchaId
                    && b.Fecha == fecha
                    && b.HoraInicio < fin
                    && b.HoraFin > inicio);
        }

        public Tarifa ObtenerTarifaVigente(int canchaId, DateOnly fecha, TimeOnly hora)
        {
            bool requiereLuz = hora >= _horaLuz;

            var tarifa = _contexto.Tarifas
                .Where(t => t.CanchaId == canchaId 
                            && t.EsActual 
                            && t.ContratoLuz == requiereLuz 
                            && t.FechaVigencia <= fecha)
                .OrderByDescending(t => t.FechaVigencia)
                .FirstOrDefault();

            if (tarifa == null && requiereLuz)
            {
                tarifa = _contexto.Tarifas
                    .Where(t => t.CanchaId == canchaId 
                                && t.EsActual 
                                && t.ContratoLuz == false 
                                && t.FechaVigencia <= fecha)
                    .OrderByDescending(t => t.FechaVigencia)
                    .FirstOrDefault();
            }

            if (tarifa == null)
            {
			    var fallback = _contexto.Tarifas
				    .Where(t => t.CanchaId == canchaId && t.FechaVigencia <= fecha)
				    .OrderByDescending(t => t.FechaVigencia)
				    .FirstOrDefault();
                if (fallback != null) return fallback;
            }

			if (tarifa != null) return tarifa;

			throw new InvalidOperationException($"No se encontró tarifa vigente para la cancha {canchaId} en la fecha {fecha} a las {hora}.");
        }

        public Reserva ObtenerReservaPorId(int reservaId)
        {
			var reserva = _contexto.Reservas.FirstOrDefault(r => r.ReservaId == reservaId);
			if (reserva == null)
				throw new InvalidOperationException($"No se encontró reserva con id {reservaId}.");
			return reserva;
        }

        public async Task<List<Reserva>> ObtenerReservasPorCliente(int clienteId)
        {
            return await _contexto.Reservas
                .Where(r => r.ClienteId == clienteId)
                .Include(r => r.EstadoReserva) 
                .Include(r => r.DetalleReservas) 
                    .ThenInclude(d => d.Cancha) 
                .OrderByDescending(r => r.Fecha)
                .ToListAsync(); 
        }

        public void AgregarReserva(Reserva reserva)
        {
            _contexto.Reservas.Add(reserva);
        }

        public void AgregarDetalle(DetalleReserva detalle)
        {
            _contexto.DetalleReservas.Add(detalle);
        }
        public List<ComplejoDTO> ObtenerComplejos()
        {
            return _contexto.Complejos
                .Select(c => new ComplejoDTO
                {
                    ComplejoId = c.ComplejoId,
                    Nombre = c.Nombre
                }).ToList();
        }

        public List<CanchaDTO> ObtenerCanchasPorComplejo(int complejoId)
        {
            return _contexto.Canchas
                .Where(c => c.ComplejoId == complejoId)
                .Select(c => new CanchaDTO(c) 
                {
                    CanchaId = c.CanchaId,
                    Nombre = c.Nombre
                }).ToList();
        }
        public List<HorarioLibreDTO> ObtenerHorariosDisponiblesCancha(int canchaId, DateOnly fecha, TimeOnly apertura, TimeOnly cierre)
        {
            List<HorarioLibreDTO> resultado = new List<HorarioLibreDTO>();

            for (TimeOnly hora = apertura; hora < cierre; hora = hora.AddHours(1))
            {
                TimeOnly siguiente = hora.Add(TimeSpan.FromHours(1));

                bool ocupadoPorReserva = _contexto.Reservas
                    .Any(r => r.DetalleReservas.Any(d => d.CanchaId == canchaId) && r.Fecha == fecha &&
                         hora < r.HoraFin && siguiente > r.HoraInicio);

                if (!ocupadoPorReserva)
                {
                    resultado.Add(new HorarioLibreDTO
                    {
                        HoraInicio = hora,
                        HoraFin = siguiente
                    });
                }
            }

            return resultado;
        }

        public async Task GuardarAsync()
        {
            await _contexto.SaveChangesAsync();
        }
    }
}