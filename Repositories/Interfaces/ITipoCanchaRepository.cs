namespace complejoDeportivo.Repositories.Interfaces
{
    using complejoDeportivo.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ITipoCanchaRepository
    {
        Task<IEnumerable<TipoCancha>> GetAllAsync();
        Task<TipoCancha> GetByIdAsync(int id);
        Task<TipoCancha> CreateAsync(TipoCancha tipoCancha);
        Task<bool> UpdateAsync(TipoCancha tipoCancha);
        Task<bool> DeleteAsync(int id);
    }
}