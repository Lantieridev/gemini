using complejoDeportivo.DTOs;
using complejoDeportivo.Models;
using complejoDeportivo.Repositories.Interfaces;
using complejoDeportivo.Services.Interfaces;

namespace complejoDeportivo.Services.Implementations
{
    public class EmpleadoService : IEmpleadoService
    {
        private readonly IEmpleadoRepository _empleadoRepo;

        public EmpleadoService(IEmpleadoRepository empleadoRepo)
        {
            _empleadoRepo = empleadoRepo;
        }

        private EmpleadoDTO MapToDTO(Empleado empleado)
        {
            return new EmpleadoDTO
            {
                EmpleadoId = empleado.EmpleadoId,
                Nombre = empleado.Nombre,
                Apellido = empleado.Apellido,
                Email = empleado.Email,
                Telefono = empleado.Telefono,
                Cargo = empleado.Cargo,
                FechaIngreso = empleado.FechaIngreso
            };
        }

        public async Task<IEnumerable<EmpleadoDTO>> GetAllAsync(string? searchTerm = null)
        {
            var empleados = await _empleadoRepo.GetAllAsync(searchTerm);
            return empleados.Select(MapToDTO);
        }

        public async Task<EmpleadoDTO> GetByIdAsync(int id)
        {
            var empleado = await _empleadoRepo.GetByIdAsync(id);
            if (empleado == null)
            {
                throw new NotFoundException($"Empleado con ID {id} no encontrado.");
            }
            return MapToDTO(empleado);
        }

        public async Task<EmpleadoDTO> CreateAsync(CrearEmpleadoDTO createDto)
        {
            if (!string.IsNullOrEmpty(createDto.Email) && await _empleadoRepo.GetByEmailAsync(createDto.Email) != null)
            {
                throw new Exception("El email ya está registrado.");
            }

            var empleado = new Empleado
            {
                Nombre = createDto.Nombre,
                Apellido = createDto.Apellido,
                Email = createDto.Email,
                Telefono = createDto.Telefono,
                Cargo = createDto.Cargo,
                FechaIngreso = DateOnly.FromDateTime(DateTime.Now)
            };

            var nuevoEmpleado = await _empleadoRepo.CreateAsync(empleado);
            return MapToDTO(nuevoEmpleado);
        }

        public async Task UpdateAsync(int id, ActualizarEmpleadoDTO updateDto)
        {
            var empleado = await _empleadoRepo.GetByIdAsync(id);
            if (empleado == null)
            {
                throw new NotFoundException($"Empleado con ID {id} no encontrado.");
            }

            if (!string.IsNullOrEmpty(updateDto.Email))
            {
                var empleadoEmail = await _empleadoRepo.GetByEmailAsync(updateDto.Email);
                if (empleadoEmail != null && empleadoEmail.EmpleadoId != id)
                    throw new Exception("El email ya está registrado por otro empleado.");
            }

            empleado.Nombre = updateDto.Nombre;
            empleado.Apellido = updateDto.Apellido;
            empleado.Email = updateDto.Email;
            empleado.Telefono = updateDto.Telefono;
            empleado.Cargo = updateDto.Cargo;

            await _empleadoRepo.UpdateAsync(empleado);
        }

        public async Task DeleteAsync(int id)
        {
            var empleado = await _empleadoRepo.GetByIdAsync(id);
            if (empleado == null)
            {
                throw new NotFoundException($"Empleado con ID {id} no encontrado.");
            }

            // Advertencia: Borrar un empleado puede fallar si tiene un Usuario asociado.
            await _empleadoRepo.DeleteAsync(id);
        }
    }
}