using complejoDeportivo.DTOs;

namespace complejoDeportivo.Services.Interfaces
{
    // Nombre en singular
    public interface IUsuarioService
    {
        Task<IEnumerable<UsuarioDTO>> GetAllAsync();
        Task<UsuarioDTO> GetByIdAsync(int id);
        Task<UsuarioDTO> CreateAsync(CreateUsuarioDTO createDto);
        Task UpdateAsync(int id, UsuarioDTO updateDto);
        Task DeleteAsync(int id);
        Task<UsuarioDTO> RegisterClienteAsync(RegisterClienteDTO dto);
        Task<UsuarioDTO> RegisterEmpleadoAsync(RegisterClienteDTO dto);
    }
}