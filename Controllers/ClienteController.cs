using complejoDeportivo.DTOs;
using complejoDeportivo.Services.Implementations;
using complejoDeportivo.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims; // AGREGADO
using complejoDeportivo.Repositories.Interfaces; // AGREGADO

namespace complejoDeportivo.Controllers
{
    // [Authorize] AHORA SE MANEJA POR MÉTODO
    [Route("api/admin/clientes")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteService _service;
        private readonly IUsuarioRepository _usuarioRepository; // AGREGADO

        public ClienteController(IClienteService service, IUsuarioRepository usuarioRepository) // MODIFICADO
        {
            _service = service;
            _usuarioRepository = usuarioRepository; // AGREGADO
        }

        // --- MÉTODO PRIVADO DE SEGURIDAD ---
        private async Task<bool> EsClienteValido(int idSolicitado)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole == "Cliente")
            {
                var claimEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                if (string.IsNullOrEmpty(claimEmail)) return false; // Token inválido

                var usuario = await _usuarioRepository.GetByEmailAsync(claimEmail);

                // Si es un cliente, su ClienteId DEBE coincidir con el ID solicitado
                if (usuario == null || usuario.ClienteId != idSolicitado)
                {
                    return false; // No es el dueño del perfil
                }
            }
            // Si es Admin/Empleado, o si es un Cliente y el ID coincide, es válido.
            return true;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Empleado")] // <-- MANTENIDO SOLO PARA ADMIN/EMPLEADO
        public async Task<ActionResult<IEnumerable<ClienteDTO>>> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id}")]
        [Authorize] // <-- AHORA PERMITE CLIENTES (con validación)
        public async Task<ActionResult<ClienteDTO>> GetById(int id)
        {
            // --- VALIDACIÓN DE CLIENTE ---
            if (!await EsClienteValido(id))
            {
                return Forbid(); // 403 Prohibido si un cliente pide datos de otro
            }
            // --- FIN VALIDACIÓN ---

            try
            {
                var cliente = await _service.GetByIdAsync(id);
                return Ok(cliente);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Empleado")] // <-- MANTENIDO SOLO PARA ADMIN/EMPLEADO
        public async Task<ActionResult<ClienteDTO>> Create([FromBody] CrearClienteDTO createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var nuevoCliente = await _service.CreateAsync(createDto);
                return CreatedAtAction("GetById", new { id = nuevoCliente.ClienteId }, nuevoCliente);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize] // <-- AHORA PERMITE CLIENTES (con validación)
        public async Task<IActionResult> Update(int id, [FromBody] ActualizarClienteDTO updateDto)
        {
            // --- VALIDACIÓN DE CLIENTE ---
            if (!await EsClienteValido(id))
            {
                return Forbid(); // 403 Prohibido si un cliente edita datos de otro
            }
            // --- FIN VALIDACIÓN ---

            try
            {
                await _service.UpdateAsync(id, updateDto);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Empleado")] // <-- MANTENIDO SOLO PARA ADMIN/EMPLEADO
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}