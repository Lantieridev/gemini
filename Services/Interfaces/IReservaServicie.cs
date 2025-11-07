using complejoDeportivo.DTOs;
using complejoDeportivo.Models;
using System.Threading.Tasks; 
using System.Collections.Generic; // Agregado

namespace complejoDeportivo.Services
{
    public interface IReservaServicie
    {
        List<DisponibilidadCanchaDTO> ObtenerTurnosDisponibles(int canchaId, DateOnly fecha);
        Task<ReservaDTO> CrearReserva(CrearReservaDTO dto); 
        Task<List<ReservaDTO>> ListarReservasCliente(int clienteId); 
        Task<bool> CancelarReserva(CancelarReservaDTO dto); 
        List<ComplejoDTO> ListarComplejos();
        List<CanchaDTO> ListarCanchasPorComplejo(int complejoId);
        List<HorarioLibreDTO> ObtenerHorariosDisponiblesCancha(int canchaId, DateOnly fecha);
    }
}