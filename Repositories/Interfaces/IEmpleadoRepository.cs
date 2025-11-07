using complejoDeportivo.Models;

namespace complejoDeportivo.Repositories.Interfaces
{
    public interface IEmpleadoRepository
    {
        Task<IEnumerable<Empleado>> GetAllAsync();
        Task<Empleado?> GetByIdAsync(int id);
        Task<Empleado> CreateAsync(Empleado empleado);
        Task<bool> UpdateAsync(Empleado empleado);
        Task<bool> DeleteAsync(int id);
        Task<Empleado?> GetByEmailAsync(string email);
    }
}