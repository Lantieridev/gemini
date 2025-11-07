using complejoDeportivo.Models;

namespace complejoDeportivo.Repositories.Interfaces
{
    public interface IComplejoRepository
    {
        Task<IEnumerable<Complejo>> GetAllAsync(string? searchTerm = null);
        Task<Complejo?> GetByIdAsync(int id);
        Task<Complejo> CreateAsync(Complejo complejo, Direccion direccion);
        Task<bool> UpdateAsync(Complejo complejo, Direccion direccion);
        Task<bool> DeleteAsync(int id);
    }
}