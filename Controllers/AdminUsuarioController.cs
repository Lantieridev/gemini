using complejoDeportivo.DTOs;
using complejoDeportivo.Services.Implementations; // Para la NotFoundException
using complejoDeportivo.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace complejoDeportivo.Controllers
{
    [Route("api/admin/usuarios")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminUsuarioController : ControllerBase
    {
        private readonly IUsuarioService _service;

        public AdminUsuarioController(IUsuarioService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioDTO>>> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioDTO>> GetById(int id)
        {
            try
            {
                var usuario = await _service.GetByIdAsync(id);
                return Ok(usuario);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<UsuarioDTO>> Create([FromBody] CreateUsuarioDTO createDto) // <--- [FromBody]
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var nuevoUsuario = await _service.CreateAsync(createDto);
            return CreatedAtAction("GetById", new { id = nuevoUsuario.UsuarioId }, nuevoUsuario); // <--- "GetById"
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UsuarioDTO updateDto) // <--- [FromBody]
        {
            if (id != updateDto.UsuarioId)
            {
                return BadRequest("El ID de la URL no coincide con el ID del cuerpo.");
            }

            try
            {
                await _service.UpdateAsync(id, updateDto);
                return NoContent(); // 204 No Content (éxito)
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
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
        }
    }
}