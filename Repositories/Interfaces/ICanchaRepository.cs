using complejoDeportivo.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace complejoDeportivo.Repositories.Interfaces
{
    public interface ICanchaRepository
    {
        Task<IEnumerable<Cancha>> GetAllAsync();
        Task<Cancha> GetByIdAsync(int id);
        Task<Cancha> CreateAsync(Cancha Cancha);
        Task<bool> UpdateAsync(Cancha Cancha);
        Task<bool> DeleteAsync(int id);
        Task<bool> ActivarAsync(int id);
        Task<bool> DesactivarAsync(int id);

        // [NUEVO] (Problema 4)
        Task<IEnumerable<Cancha>> GetByComplejoIdAsync(int complejoId);
    }
}