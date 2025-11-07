using complejoDeportivo.DTOs;

namespace complejoDeportivo.Services.Interfaces
{
    public interface IClienteService
    {
        Task<IEnumerable<ClienteDTO>> GetAllAsync();
        Task<ClienteDTO> GetByIdAsync(int id);
        Task<ClienteDTO> CreateAsync(CrearClienteDTO createDto);
        Task UpdateAsync(int id, ActualizarClienteDTO updateDto);
        Task DeleteAsync(int id);
    }
}