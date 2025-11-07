using complejoDeportivo.DTOs;

namespace complejoDeportivo.Services.Interfaces
{
    public interface IComplejoService
    {
        Task<IEnumerable<ComplejoDetalleDTO>> GetAllAsync();
        Task<ComplejoDetalleDTO> GetByIdAsync(int id);
        Task<ComplejoDetalleDTO> CreateAsync(CrearComplejoDTO createDto);
        Task UpdateAsync(int id, ActualizarComplejoDTO updateDto);
        Task DeleteAsync(int id);
    }
}