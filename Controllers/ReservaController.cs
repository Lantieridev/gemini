using Microsoft.AspNetCore.Mvc;
using complejoDeportivo.Models;
using complejoDeportivo.Services;
using complejoDeportivo.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks; // Agregado
using System.Security.Claims; // Agregado
using complejoDeportivo.Repositories.Interfaces; // Agregado
using System.Collections.Generic; // Agregado

namespace complejoDeportivo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservaController : ControllerBase
    {
        private readonly IReservaServicie _reservaService;
        private readonly IUsuarioRepository _usuarioRepository; // Agregado para seguridad

        public ReservaController(IReservaServicie reservaService, IUsuarioRepository usuarioRepository) // Modificado
        {
            _reservaService = reservaService;
            _usuarioRepository = usuarioRepository; // Agregado
        }

        [HttpGet("complejos")]
        [AllowAnonymous]
        public ActionResult<IEnumerable<ComplejoDTO>> GetComplejos()
        {
            var complejos = _reservaService.ListarComplejos();
            return Ok(complejos);
        }

        [HttpGet("canchas/{complejoId}")]
        [AllowAnonymous]
        public ActionResult<IEnumerable<CanchaDTO>> GetCanchasPorComplejo(int complejoId)
        {
            var canchas = _reservaService.ListarCanchasPorComplejo(complejoId);
            return Ok(canchas);
        }

        [HttpGet("disponibilidad")]
        [AllowAnonymous]
        public ActionResult<IEnumerable<HorarioLibreDTO>> GetHorariosDisponibles(
            [FromQuery] int canchaId,
            [FromQuery] DateOnly fecha)
        {
            var horarios = _reservaService.ObtenerHorariosDisponiblesCancha(canchaId, fecha);
            return Ok(horarios);
        }

        [HttpPost]
        [Authorize(Roles = "Cliente,Admin,Empleado")]
        public async Task<ActionResult<ReservaDTO>> CrearReserva([FromBody] CrearReservaDTO dto) // <--- [FromBody]
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // --- Mejora de Seguridad: Validar que el cliente solo reserve para sí mismo ---
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                if (userRole == "Cliente")
                {
                    var claimEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                    if (string.IsNullOrEmpty(claimEmail)) return Unauthorized("Token inválido.");
                    
                    var usuario = await _usuarioRepository.GetByEmailAsync(claimEmail);
                    if (usuario == null || usuario.ClienteId != dto.ClienteId)
                    {
                        return Forbid(); // 403 Prohibido. No puede reservar por otro cliente.
                    }
                }
                // --- Fin de validación ---

                var nuevaReserva = await _reservaService.CrearReserva(dto); 
                
                // Corregido: Usar string "GetReservasCliente"
                return CreatedAtAction("GetReservasCliente", new { clienteId = nuevaReserva.ClienteId }, nuevaReserva);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("cliente/{clienteId}")]
        [Authorize(Roles = "Cliente,Admin,Empleado")]
        public async Task<ActionResult<IEnumerable<ReservaDTO>>> GetReservasCliente(int clienteId) 
        {
            // --- Cambio de Seguridad: Validar que el cliente solo vea sus reservas ---
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole == "Cliente")
            {
                var claimEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                if (string.IsNullOrEmpty(claimEmail)) return Unauthorized("Token inválido.");

                var usuario = await _usuarioRepository.GetByEmailAsync(claimEmail);
                
                if (usuario == null || usuario.ClienteId != clienteId)
                {
                    return Forbid(); // 403 Prohibido. No puede ver reservas ajenas.
                }
            }
            // --- Fin de validación ---

            var reservas = await _reservaService.ListarReservasCliente(clienteId); 
            return Ok(reservas);
        }

        [HttpPut("cancelar")]
        [Authorize(Roles = "Cliente,Admin,Empleado")]
        public async Task<IActionResult> CancelarReserva([FromBody] CancelarReservaDTO dto) // <--- [FromBody]
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // --- Mejora de Seguridad: Validar que el cliente solo cancele para sí mismo ---
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                if (userRole == "Cliente")
                {
                    var claimEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                    if (string.IsNullOrEmpty(claimEmail)) return Unauthorized("Token inválido.");
                    
                    var usuario = await _usuarioRepository.GetByEmailAsync(claimEmail);
                    if (usuario == null || usuario.ClienteId != dto.ClienteId)
                    {
                        return Forbid(); // 403 Prohibido. No puede cancelar por otro cliente.
                    }
                }
                // --- Fin de validación ---

                var cancelada = await _reservaService.CancelarReserva(dto); 
                if (cancelada)
                {
                    return NoContent(); 
                }
                else
                {
                    return NotFound(new { message = "Reserva no encontrada o no pertenece al cliente." });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}