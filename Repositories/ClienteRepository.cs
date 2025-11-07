using complejoDeportivo.Models;
using complejoDeportivo.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace complejoDeportivo.Repositories.Implementations
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly ComplejoDeportivoContext _context;

        public ClienteRepository(ComplejoDeportivoContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Cliente>> GetAllAsync()
        {
            return await _context.Clientes.ToListAsync();
        }

        public async Task<Cliente?> GetByIdAsync(int id)
        {
            return await _context.Clientes.FindAsync(id);
        }
        
        public async Task<Cliente?> GetByEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email)) return null;
            return await _context.Clientes.FirstOrDefaultAsync(c => c.Email != null && c.Email.ToLower() == email.ToLower());
        }

        public async Task<Cliente> CreateAsync(Cliente cliente)
        {
            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();
            return cliente; // Devuelve el cliente con el ID ya asignado
        }

        public async Task<bool> UpdateAsync(Cliente cliente)
        {
            _context.Entry(cliente).State = EntityState.Modified;
            // No se debe modificar la fecha de registro
            _context.Entry(cliente).Property(p => p.FechaRegistro).IsModified = false;
            
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
            {
                return false;
            }

            // Considerar borrado lógico si hay dependencias (Reservas, Facturas)
            // Por ahora, borrado físico:
            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DoesDocumentoExistAsync(string documento)
        {
            // Si el documento no es nulo o vacío, busca si algún cliente ya lo tiene
            if (!string.IsNullOrEmpty(documento))
            {
                return await _context.Clientes.AnyAsync(c => c.Documento == documento);
            }
            return false;
        }

        public async Task<bool> DoesTelefonoExistAsync(string telefono)
        {
            // Si el teléfono no es nulo o vacío, busca si algún cliente ya lo tiene
            if (!string.IsNullOrEmpty(telefono))
            {
                return await _context.Clientes.AnyAsync(c => c.Telefono == telefono);
            }
            return false;
        }
    }
}