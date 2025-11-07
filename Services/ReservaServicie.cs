using complejoDeportivo.DTOs;
using complejoDeportivo.Models;
using complejoDeportivo.Repositories;
using Microsoft.EntityFrameworkCore; 
using System.Threading.Tasks; 
using System.Linq; 
using System.Collections.Generic; 
using System; // Agregado

namespace complejoDeportivo.Services
{
    public class ReservaServicie : IReservaServicie
    {
        private readonly IReservaRepository _repo;
        private readonly ComplejoDeportivoContext _context; 
        private readonly TimeOnly _apertura = new TimeOnly(8, 0, 0);
        private readonly TimeOnly _cierre = new TimeOnly(23, 0, 0);

        public ReservaServicie(IReservaRepository repo, ComplejoDeportivoContext context) 
        {
            _repo = repo;
            _context = context; 
        }

        public List<DisponibilidadCanchaDTO> ObtenerTurnosDisponibles(int canchaId, DateOnly fecha)
        {
            return _repo.ObtenerTurnosDisponibles(canchaId, fecha, _apertura, _cierre);
        }

        public async Task<ReservaDTO> CrearReserva(CrearReservaDTO dto)
        {
            if (dto.CanchaIds == null || !dto.CanchaIds.Any())
                throw new Exception("Debe seleccionar al menos una cancha.");

            var horas = dto.HoraFin - dto.HoraInicio;
            if (horas.TotalHours < 1 || horas.TotalHours % 1 != 0)
                throw new Exception("Las reservas deben ser en bloques de 1 hora.");

            decimal total = 0;
            var detallesParaCrear = new List<(int CanchaId, Tarifa Tarifa, int CantidadHoras, decimal Subtotal)>();

            foreach (var canchaId in dto.CanchaIds.Distinct())
            {
                decimal subtotalCancha = 0;
                var h = dto.HoraInicio;
                while (h < dto.HoraFin)
                {
                    var fin = h.AddHours(1);
                    if (_repo.ExisteReservaSuperpuesta(canchaId, dto.Fecha, h, fin)
                        || _repo.ExisteBloqueo(canchaId, dto.Fecha, h, fin))
                        throw new Exception($"Horario ocupado en cancha ID {canchaId}: {h} - {fin}");

                    var tarifa = _repo.ObtenerTarifaVigente(canchaId, dto.Fecha, h);
                    subtotalCancha += tarifa.Precio;
                    h = fin;
                }

                var tarifaReferencia = _repo.ObtenerTarifaVigente(canchaId, dto.Fecha, dto.HoraInicio);
                detallesParaCrear.Add((canchaId, tarifaReferencia, (int)horas.TotalHours, subtotalCancha));
                total += subtotalCancha;
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var reserva = new Reserva
                {
                    ClienteId = dto.ClienteId,
                    Fecha = dto.Fecha,
                    HoraInicio = dto.HoraInicio,
                    HoraFin = dto.HoraFin,
                    EstadoReservaId = 1, // 1 = Confirmada
                    Ambito = dto.Ambito,
                    FechaCreacion = DateTime.Now,
                    Total = total
                };

                _repo.AgregarReserva(reserva);
                await _repo.GuardarAsync(); 

                foreach (var detalleInfo in detallesParaCrear)
                {
                    var detalle = new DetalleReserva
                    {
                        ReservaId = reserva.ReservaId,
                        CanchaId = detalleInfo.CanchaId,
                        TarifaHoraId = detalleInfo.Tarifa.TarifaId,
                        CantidadHoras = detalleInfo.CantidadHoras,
                        Descuento = 0,
                        Recargo = 0,
                        Subtotal = detalleInfo.Subtotal
                    };
                    _repo.AgregarDetalle(detalle);
                }
                
                await _repo.GuardarAsync(); 
                await transaction.CommitAsync();

                var detallesDto = detallesParaCrear.Select(d => new DetalleReservaDTO
                {
                    CanchaId = d.CanchaId,
                    NombreCancha = _context.Canchas.Find(d.CanchaId)?.Nombre ?? "N/A",
                    Subtotal = d.Subtotal,
                    CantidadHoras = d.CantidadHoras
                }).ToList();

                return new ReservaDTO
                {
                    ReservaId = reserva.ReservaId,
                    ClienteId = reserva.ClienteId,
                    Fecha = reserva.Fecha,
                    HoraInicio = reserva.HoraInicio,
                    HoraFin = reserva.HoraFin,
                    Total = reserva.Total,
                    Estado = "Confirmada", 
                    FechaCreacion = reserva.FechaCreacion,
                    Detalles = detallesDto
                };
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw; 
            }
        }

        public async Task<List<ReservaDTO>> ListarReservasCliente(int clienteId)
        {
            var reservas = await _repo.ObtenerReservasPorCliente(clienteId); 
            
            return reservas.Select(r => new ReservaDTO
            {
                ReservaId = r.ReservaId,
                ClienteId = r.ClienteId,
                Fecha = r.Fecha,
                HoraInicio = r.HoraInicio,
                HoraFin = r.HoraFin,
                Total = r.Total,
                Estado = r.EstadoReserva?.Nombre ?? "Desconocido", 
                FechaCreacion = r.FechaCreacion,
                Detalles = r.DetalleReservas.Select(d => new DetalleReservaDTO 
                {
                    DetalleReservaId = d.DetalleReservaId,
                    CanchaId = d.CanchaId,
                    NombreCancha = d.Cancha?.Nombre ?? "N/A",
                    Subtotal = d.Subtotal,
                    CantidadHoras = d.CantidadHoras
                }).ToList()
            }).ToList();
        }

        public async Task<bool> CancelarReserva(CancelarReservaDTO dto)
        {
            var reserva = _repo.ObtenerReservaPorId(dto.ReservaId);
            if (reserva == null || reserva.ClienteId != dto.ClienteId) return false;

            if (reserva.EstadoReservaId != 1) 
            {
                throw new Exception("La reserva no se puede cancelar porque no está 'Confirmada'.");
            }

            var tiempoRestante = reserva.Fecha.ToDateTime(reserva.HoraInicio) - DateTime.Now;
            if (tiempoRestante.TotalHours < 24)
                throw new Exception("La reserva solo se puede cancelar con 24 horas de anticipación.");

            reserva.EstadoReservaId = 2; // 2 = Cancelada
            await _repo.GuardarAsync(); 
            return true;
        }

        public List<ComplejoDTO> ListarComplejos()
        {
            return _repo.ObtenerComplejos();
        }

        public List<CanchaDTO> ListarCanchasPorComplejo(int complejoId)
        {
            return _repo.ObtenerCanchasPorComplejo(complejoId);
        }

        public List<HorarioLibreDTO> ObtenerHorariosDisponiblesCancha(int canchaId, DateOnly fecha)
        {
            return _repo.ObtenerHorariosDisponiblesCancha(canchaId, fecha, _apertura, _cierre);
        }
    }
}