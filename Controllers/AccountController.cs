using complejoDeportivo.DTOs;
using complejoDeportivo.Services.Implementations;
using complejoDeportivo.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace complejoDeportivo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public AccountController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<UsuarioDTO>> Register([FromBody] RegisterClienteDTO dto) // <--- [FromBody]
        {
            if (!ModelState.IsValid)
            {
				Console.WriteLine("Modelo invalido");
                return BadRequest(ModelState);
            }

            try
            {
				Console.WriteLine("Se intenta crear cliente");
                var nuevoUsuario = await _usuarioService.RegisterClienteAsync(dto);
                // Devolvemos 200 OK con los datos del usuario creado
                return Ok(nuevoUsuario);
            }
            catch (System.Exception ex)
            {
				Console.WriteLine("Error desconocido al crear usuario cliente");
                // Captura el error "El email ya está registrado" del servicio
                return BadRequest(new { message = ex.Message });
            }
        }

		[HttpGet("tuqui")]
		public string Tuqui()
		{
			return "Tuqui";
		}

		[HttpPost("tuqui")]
		public string TPuqui([FromBody] Truqui le)
		{
			return le.tongo;
		}

        [HttpPost("register-empleado")]
        [AllowAnonymous]
        public async Task<ActionResult<UsuarioDTO>> RegisterEmpleado([FromBody] RegisterClienteDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var nuevoUsuario = await _usuarioService.RegisterEmpleadoAsync(dto);
                return Ok(nuevoUsuario);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}