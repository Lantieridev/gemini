using complejoDeportivo.Models;
using complejoDeportivo.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using complejoDeportivo.Repositories.Interfaces;
using complejoDeportivo.Services.Interfaces;


namespace complejoDeportivo.Services.Implementations
{

	public class TipoCanchaService : ITipoCanchaService
	{
		private readonly ITipoCanchaRepository _repository;

		public TipoCanchaService(ITipoCanchaRepository repository)
		{
			_repository = repository;
		}

		public async Task<IEnumerable<TipoCanchaDTO>> GetAllAsync()
		{
			var tiposCancha = await _repository.GetAllAsync();
			return tiposCancha.Select(tc => new TipoCanchaDTO
			{
				TipoCanchaID = tc.TipoCanchaId,
				Nombre = tc.Nombre
			});
		}

		public async Task<TipoCanchaDTO> GetByIdAsync(int id)
		{
			var tc = await _repository.GetByIdAsync(id);
			if (tc == null)
			{
				throw new NotFoundException($"Tipo de cancha con ID {id} no encontrado.");
			}
			return new TipoCanchaDTO
			{
				TipoCanchaID = tc.TipoCanchaId,
				Nombre = tc.Nombre
			};
		}

		public async Task<TipoCanchaDTO> CreateAsync(CreateTipoCanchaDTO createDto)
		{
			var tipoCancha = new TipoCancha
			{
				Nombre = createDto.Nombre
			};
			var nuevoTipoCancha = await _repository.CreateAsync(tipoCancha);
			return new TipoCanchaDTO
			{
				TipoCanchaID = nuevoTipoCancha.TipoCanchaId,
				Nombre = nuevoTipoCancha.Nombre
			};
		}

		public async Task UpdateAsync(int id, TipoCanchaDTO updateDto)
		{
			var tipoCancha = await _repository.GetByIdAsync(id);
			if (tipoCancha == null)
			{
				throw new NotFoundException($"Tipo de cancha con ID {id} no encontrado.");
			}

			tipoCancha.Nombre = updateDto.Nombre;
			await _repository.UpdateAsync(tipoCancha);
		}

		public async Task DeleteAsync(int id)
		{
			var tipoCancha = await _repository.GetByIdAsync(id);
			if (tipoCancha == null)
			{
				throw new NotFoundException($"Tipo de cancha con ID {id} no encontrado.");
			}
			await _repository.DeleteAsync(id);
		}
	}
}