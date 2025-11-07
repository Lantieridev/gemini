using complejoDeportivo.DTOs;
using complejoDeportivo.Services.Implementations;
using complejoDeportivo.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace complejoDeportivo.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize(Roles = "Admin,Empleado")]
	public class CanchaController : ControllerBase
	{
		ICanchaService _CanchaService;

		public CanchaController(ICanchaService canchaService)
		{
			_CanchaService = canchaService;
		}

		[HttpGet]
		[AllowAnonymous]
		public async Task<ActionResult<IEnumerable<CanchaDTO>>> GetAll()
		{
			return Ok(await _CanchaService.GetAllAsync());
		}

		[HttpGet("{id}")]
		[AllowAnonymous]
		public async Task<ActionResult<CanchaDTO>> GetById(int id)
		{
			try
			{
				var cancha = await _CanchaService.GetByIdAsync(id);
				return Ok(cancha);
			}
			catch (NotFoundException ex)
			{
				return NotFound(new { message = ex.Message });
			}
		}

		[HttpPost]
		public async Task<ActionResult<CanchaDTO>> Create([FromBody] CrearCanchaDTO createDto) // <--- [FromBody]
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			var nuevaCancha = await _CanchaService.CreateAsync(createDto);
			return CreatedAtAction("GetById", new { id = nuevaCancha.CanchaId }, nuevaCancha); // <--- "GetById"
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> Update(int id, [FromBody] CrearCanchaDTO updateDto) // <--- [FromBody]
		{
			try
			{
				await _CanchaService.UpdateAsync(id, updateDto);
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
				await _CanchaService.DeleteAsync(id);
				return NoContent(); // 204 No Content (éxito)
			}
			catch (NotFoundException ex)
			{
				return NotFound(new { message = ex.Message });
			}
		}

		[HttpPut("{id}/activar")]
		public async Task<IActionResult> Activar(int id)
		{
			try
			{
				await _CanchaService.ActivarAsync(id);
				return NoContent();
			}
			catch (NotFoundException ex)
			{
				return NotFound(new { message = ex.Message });
			}
		}

		[HttpPut("{id}/desactivar")]
		public async Task<IActionResult> Desactivar(int id)
		{
			try
			{
				await _CanchaService.DesactivarAsync(id);
				return NoContent();
			}
			catch (NotFoundException ex)
			{
				return NotFound(new { message = ex.Message });
			}
		}
	}
}