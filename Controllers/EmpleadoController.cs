using complejoDeportivo.DTOs;
using complejoDeportivo.Services.Implementations;
using complejoDeportivo.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace complejoDeportivo.Controllers
{
    [Route("api/admin/empleados")]
    [ApiController]
    [Authorize(Roles = "Admin")] // Solo Admin puede gestionar empleados
    public class EmpleadoController : ControllerBase
    {
        private readonly IEmpleadoService _service;

        public EmpleadoController(IEmpleadoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmpleadoDTO>>> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EmpleadoDTO>> GetById(int id)
        {
            try
            {
                var empleado = await _service.GetByIdAsync(id);
                return Ok(empleado);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<EmpleadoDTO>> Create([FromBody] CrearEmpleadoDTO createDto) // <--- [FromBody]
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var nuevoEmpleado = await _service.CreateAsync(createDto);
                return CreatedAtAction("GetById", new { id = nuevoEmpleado.EmpleadoId }, nuevoEmpleado); // <--- "GetById"
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ActualizarEmpleadoDTO updateDto) // <--- [FromBody]
        {
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