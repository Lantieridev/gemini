using complejoDeportivo.Models;
using complejoDeportivo.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace complejoDeportivo.Repositories.Implementations
{
    public class ComplejoRepository : IComplejoRepository
    {
        private readonly ComplejoDeportivoContext _context;

        public ComplejoRepository(ComplejoDeportivoContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Complejo>> GetAllAsync()
        {
            // Incluimos la Dirección
            return await _context.Complejos.Include(c => c.Direccion).ToListAsync();
        }

        public async Task<Complejo?> GetByIdAsync(int id)
        {
            // Incluimos la Dirección
            return await _context.Complejos
                .Include(c => c.Direccion)
                .FirstOrDefaultAsync(c => c.ComplejoId == id);
        }

        public async Task<Complejo> CreateAsync(Complejo complejo, Direccion direccion)
        {
            // Usamos una transacción para asegurar que ambos (Direccion y Complejo) se creen correctamente.
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Crear la Dirección
                _context.Direccions.Add(direccion);
                await _context.SaveChangesAsync();

                // 2. Asignar el ID de la nueva dirección al complejo
                complejo.DireccionId = direccion.DireccionId;

                // 3. Crear el Complejo
                _context.Complejos.Add(complejo);
                await _context.SaveChangesAsync();

                // Confirmar la transacción
                await transaction.CommitAsync();

                return complejo;
            }
            catch (Exception)
            {
                // Si algo falla, revertir todo
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> UpdateAsync(Complejo complejo, Direccion direccion)
        {
             using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Actualizar la Dirección
                _context.Entry(direccion).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                // 2. Actualizar el Complejo
                complejo.DireccionId = direccion.DireccionId; // Asegurarse que el ID esté asignado
                _context.Entry(complejo).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var complejo = await GetByIdAsync(id);
            if (complejo == null)
            {
                return false;
            }
            
            // Borrar un complejo requiere borrar su dirección y puede fallar si tiene canchas (Foreign Key).
            // Por simplicidad, se intenta el borrado.
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Complejos.Remove(complejo);
                await _context.SaveChangesAsync();
                
                // Borramos también la dirección asociada
                if (complejo.Direccion != null)
                {
                    _context.Direccions.Remove(complejo.Direccion);
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                 await transaction.RollbackAsync();
                 // Lanzamos un error específico si falla (ej. por tener canchas asociadas)
                 throw new InvalidOperationException("No se puede eliminar el complejo. Asegúrese de que no tenga canchas u otros elementos asociados.");
            }
        }
    }
}