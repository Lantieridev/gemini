using complejoDeportivo.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace complejoDeportivo.Services.Interfaces
{
	public interface ICanchaService
	{
		Task<IEnumerable<CanchaDTO>> GetAllAsync();
		Task<CanchaDTO> GetByIdAsync(int id);
		Task<CanchaDTO> CreateAsync(CrearCanchaDTO createDto);
		Task UpdateAsync(int id, CrearCanchaDTO updateDto);
		Task DeleteAsync(int id);
		Task ActivarAsync(int id);
		Task DesactivarAsync(int id);
        Task<IEnumerable<CanchaDTO>> GetCanchasByComplejoAsync(int complejoId);
    }
}