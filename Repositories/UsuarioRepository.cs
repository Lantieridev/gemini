using complejoDeportivo.Models;
using complejoDeportivo.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace complejoDeportivo.Repositories.Implementations
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly ComplejoDeportivoContext _context;

        public UsuarioRepository(ComplejoDeportivoContext context)
        {
            _context = context;
        }

        public async Task<Usuario?> GetByEmailAsync(string email)
        {
            return await _context.Usuarios
                .Include(u => u.Empleado)
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<IEnumerable<Usuario>> GetAllAsync()
        {
            return await _context.Usuarios.ToListAsync();
        }

        public async Task<Usuario?> GetByIdAsync(int id)
        {
            // Usamos FindAsync para buscar por Primary Key
            return await _context.Usuarios.FindAsync(id);
        }

        public async Task<Usuario> CreateAsync(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        public async Task<bool> UpdateAsync(Usuario usuario)
        {
            _context.Entry(usuario).State = EntityState.Modified;

            _context.Entry(usuario).Property(p => p.PasswordHash).IsModified = false;
            _context.Entry(usuario).Property(p => p.FechaRegistro).IsModified = false;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return false;
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}