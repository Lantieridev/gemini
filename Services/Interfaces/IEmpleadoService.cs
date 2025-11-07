using complejoDeportivo.DTOs;

namespace complejoDeportivo.Services.Interfaces
{
    public interface IEmpleadoService
    {
        Task<IEnumerable<EmpleadoDTO>> GetAllAsync(string? searchTerm = null);
        Task<EmpleadoDTO> GetByIdAsync(int id);
        Task<EmpleadoDTO> CreateAsync(CrearEmpleadoDTO createDto);
        Task UpdateAsync(int id, ActualizarEmpleadoDTO updateDto);
        Task DeleteAsync(int id);
    }
}