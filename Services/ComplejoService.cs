using complejoDeportivo.DTOs;
using complejoDeportivo.Models;
using complejoDeportivo.Repositories.Interfaces;
using complejoDeportivo.Services.Interfaces;

namespace complejoDeportivo.Services.Implementations
{
    public class ComplejoService : IComplejoService
    {
        private readonly IComplejoRepository _complejoRepo;

        public ComplejoService(IComplejoRepository complejoRepo)
        {
            _complejoRepo = complejoRepo;
        }

        private ComplejoDetalleDTO MapToDTO(Complejo complejo)
        {
            return new ComplejoDetalleDTO
            {
                ComplejoId = complejo.ComplejoId,
                Nombre = complejo.Nombre,
                Direccion = new DireccionDTO
                {
                    DireccionId = complejo.Direccion.DireccionId,
                    Calle = complejo.Direccion.Calle,
                    Numero = complejo.Direccion.Numero,
                    Ciudad = complejo.Direccion.Ciudad,
                    Provincia = complejo.Direccion.Provincia,
                    CodigoPostal = complejo.Direccion.CodigoPostal
                }
            };
        }

        public async Task<IEnumerable<ComplejoDetalleDTO>> GetAllAsync(string? searchTerm = null)
        {
            var complejos = await _complejoRepo.GetAllAsync(searchTerm);
            return complejos.Select(MapToDTO);
        }

        public async Task<ComplejoDetalleDTO> GetByIdAsync(int id)
        {
            var complejo = await _complejoRepo.GetByIdAsync(id);
            if (complejo == null)
            {
                throw new NotFoundException($"Complejo con ID {id} no encontrado.");
            }
            return MapToDTO(complejo);
        }

        public async Task<ComplejoDetalleDTO> CreateAsync(CrearComplejoDTO createDto)
        {
            var direccion = new Direccion
            {
                Calle = createDto.Calle,
                Numero = createDto.Numero,
                Ciudad = createDto.Ciudad,
                Provincia = createDto.Provincia,
                CodigoPostal = createDto.CodigoPostal
            };

            var complejo = new Complejo
            {
                Nombre = createDto.Nombre
            };

            var nuevoComplejo = await _complejoRepo.CreateAsync(complejo, direccion);

            // Recargamos el objeto con la dirección incluida para el DTO
            nuevoComplejo.Direccion = direccion;

            return MapToDTO(nuevoComplejo);
        }

        public async Task UpdateAsync(int id, ActualizarComplejoDTO updateDto)
        {
            var complejo = await _complejoRepo.GetByIdAsync(id);
            if (complejo == null)
            {
                throw new NotFoundException($"Complejo con ID {id} no encontrado.");
            }

            // Actualizar el complejo
            complejo.Nombre = updateDto.Nombre;

            // Actualizar la dirección existente
            complejo.Direccion.Calle = updateDto.Calle;
            complejo.Direccion.Numero = updateDto.Numero;
            complejo.Direccion.Ciudad = updateDto.Ciudad;
            complejo.Direccion.Provincia = updateDto.Provincia;
            complejo.Direccion.CodigoPostal = updateDto.CodigoPostal;

            await _complejoRepo.UpdateAsync(complejo, complejo.Direccion);
        }

        public async Task DeleteAsync(int id)
        {
            var complejo = await _complejoRepo.GetByIdAsync(id);
            if (complejo == null)
            {
                throw new NotFoundException($"Complejo con ID {id} no encontrado.");
            }

            await _complejoRepo.DeleteAsync(id);
        }
    }
}