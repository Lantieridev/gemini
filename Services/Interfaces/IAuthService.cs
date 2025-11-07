using complejoDeportivo.DTOs;

namespace complejoDeportivo.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDTO> LoginAsync(LoginRequestDTO loginRequest);
    }
}