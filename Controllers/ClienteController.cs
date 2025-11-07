using complejoDeportivo.DTOs;
using complejoDeportivo.Services.Implementations;
using complejoDeportivo.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace complejoDeportivo.Controllers
{
    [Route("api/admin/clientes")]
    [ApiController]
    [Authorize(Roles = "Admin,Empleado")] // Solo Admin y Empleado pueden gestionar clientes
    public class ClienteController : ControllerBase
    {
        private readonly IClienteService _service;

        public ClienteController(IClienteService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClienteDTO>>> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ClienteDTO>> GetById(int id)
        {
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
        public async Task<ActionResult<ClienteDTO>> Create([FromBody] CrearClienteDTO createDto) // <--- [FromBody]
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var nuevoCliente = await _service.CreateAsync(createDto);
                return CreatedAtAction("GetById", new { id = nuevoCliente.ClienteId }, nuevoCliente); // <--- "GetById"
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message }); // Captura duplicados
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ActualizarClienteDTO updateDto) // <--- [FromBody]
        {
            try
            {
                await _service.UpdateAsync(id, updateDto);
                return NoContent(); // 204 No Content (éxito)
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message }); // Captura duplicados
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return NoContent(); // 204 No Content (éxito)
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Captura error de borrado (ej. Foreign Key)
                return BadRequest(new { message = ex.Message }); 
            }
        }
    }
}