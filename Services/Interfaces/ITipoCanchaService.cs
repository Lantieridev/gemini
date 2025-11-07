using complejoDeportivo.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace complejoDeportivo.Services.Interfaces
{


	public interface ITipoCanchaService
	{
		Task<IEnumerable<TipoCanchaDTO>> GetAllAsync();
		Task<TipoCanchaDTO> GetByIdAsync(int id);
		Task<TipoCanchaDTO> CreateAsync(CreateTipoCanchaDTO createDto);
		Task UpdateAsync(int id, TipoCanchaDTO updateDto);
		Task DeleteAsync(int id);
	}
}