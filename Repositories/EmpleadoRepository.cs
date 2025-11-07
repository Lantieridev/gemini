using complejoDeportivo.Models;
using complejoDeportivo.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace complejoDeportivo.Repositories.Implementations
{
    public class EmpleadoRepository : IEmpleadoRepository
    {
        private readonly ComplejoDeportivoContext _context;

        public EmpleadoRepository(ComplejoDeportivoContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Empleado>> GetAllAsync()
        {
            return await _context.Empleados.ToListAsync();
        }

        public async Task<Empleado?> GetByIdAsync(int id)
        {
            return await _context.Empleados.FindAsync(id);
        }
        
        public async Task<Empleado?> GetByEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email)) return null;
            return await _context.Empleados.FirstOrDefaultAsync(e => e.Email != null && e.Email.ToLower() == email.ToLower());
        }

        public async Task<Empleado> CreateAsync(Empleado empleado)
        {
            _context.Empleados.Add(empleado);
            await _context.SaveChangesAsync();
            return empleado;
        }

        public async Task<bool> UpdateAsync(Empleado empleado)
        {
            _context.Entry(empleado).State = EntityState.Modified;
            _context.Entry(empleado).Property(p => p.FechaIngreso).IsModified = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var empleado = await _context.Empleados.FindAsync(id);
            if (empleado == null)
            {
                return false;
            }

            _context.Empleados.Remove(empleado);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}