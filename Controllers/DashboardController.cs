using complejoDeportivo.DTOs.Dashboard;
using complejoDeportivo.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace complejoDeportivo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Empleado")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet]
        public async Task<ActionResult<DashboardCompletoDto>> GetDashboardCompleto([FromQuery] FiltrosDashboardDto filtros)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Si no se proporciona ComplejoId, asignar uno por defecto (ej. 1)
            if (filtros.ComplejoId <= 0)
            {
                filtros.ComplejoId = 1; // O manejar como error
            }

            var dashboardData = await _dashboardService.GetDashboardCompletoAsync(filtros);
            return Ok(dashboardData);
        }
    }
}