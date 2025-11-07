using complejoDeportivo.DTOs;
using complejoDeportivo.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace complejoDeportivo.Controllers
{
	[ApiController]
    [Route("api/[controller]")]
	[Authorize(Roles = "Admin,Empleado")]
    public class TiposCanchaController : ControllerBase
	{
		ITipoCanchaService _tipoCanchaService;
		public TiposCanchaController(ITipoCanchaService tipoCanchaService)
		{
			_tipoCanchaService = tipoCanchaService;
		}

		[HttpGet]
		public async Task<IActionResult> GetTiposCancha()
		{
			var tiposCancha = await _tipoCanchaService.GetAllAsync();
			return Ok(tiposCancha);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetTipoCancha(int id)
		{
			var tipoCancha = await _tipoCanchaService.GetByIdAsync(id);
			return Ok(tipoCancha);
		}

		[HttpPost]
		public async Task<IActionResult> CreateTipoCancha([FromBody] CreateTipoCanchaDTO createDto) // <--- [FromBody]
		{
			var tipoCancha = await _tipoCanchaService.CreateAsync(createDto);
			// Este controlador no tiene "GetById", por lo que devolvemos Ok() en lugar de CreatedAtAction
			return Ok(tipoCancha);
		}
	}
}