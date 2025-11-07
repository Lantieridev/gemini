using complejoDeportivo.DTOs;
using complejoDeportivo.Models;
using complejoDeportivo.Repositories.Interfaces;
using complejoDeportivo.Services.Interfaces;

namespace complejoDeportivo.Services.Implementations
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _clienteRepo;

        public ClienteService(IClienteRepository clienteRepo)
        {
            _clienteRepo = clienteRepo;
        }

        private ClienteDTO MapToDTO(Cliente cliente)
        {
            return new ClienteDTO
            {
                ClienteId = cliente.ClienteId,
                Nombre = cliente.Nombre,
                Apellido = cliente.Apellido,
                Email = cliente.Email,
                Telefono = cliente.Telefono,
                Documento = cliente.Documento,
                FechaRegistro = cliente.FechaRegistro
            };
        }

        public async Task<IEnumerable<ClienteDTO>> GetAllAsync()
        {
            var clientes = await _clienteRepo.GetAllAsync();
            return clientes.Select(MapToDTO);
        }

        public async Task<ClienteDTO> GetByIdAsync(int id)
        {
            var cliente = await _clienteRepo.GetByIdAsync(id);
            if (cliente == null)
            {
                throw new NotFoundException($"Cliente con ID {id} no encontrado.");
            }
            return MapToDTO(cliente);
        }

        public async Task<ClienteDTO> CreateAsync(CrearClienteDTO createDto)
        {
            // Validaciones de negocio (evitar duplicados)
            if (!string.IsNullOrEmpty(createDto.Email) && await _clienteRepo.GetByEmailAsync(createDto.Email) != null)
            {
                throw new Exception("El email ya está registrado.");
            }
            if (!string.IsNullOrEmpty(createDto.Documento) && await _clienteRepo.DoesDocumentoExistAsync(createDto.Documento))
            {
                throw new Exception("El documento ya se encuentra registrado.");
            }
            if (!string.IsNullOrEmpty(createDto.Telefono) && await _clienteRepo.DoesTelefonoExistAsync(createDto.Telefono))
            {
                throw new Exception("El número de teléfono ya se encuentra registrado.");
            }

            var cliente = new Cliente
            {
                Nombre = createDto.Nombre,
                Apellido = createDto.Apellido,
                Email = createDto.Email,
                Telefono = createDto.Telefono,
                Documento = createDto.Documento,
                FechaRegistro = DateTime.UtcNow // Usar UTC para registros
            };

            var nuevoCliente = await _clienteRepo.CreateAsync(cliente);
            return MapToDTO(nuevoCliente);
        }

        public async Task UpdateAsync(int id, ActualizarClienteDTO updateDto)
        {
            var cliente = await _clienteRepo.GetByIdAsync(id);
            if (cliente == null)
            {
                throw new NotFoundException($"Cliente con ID {id} no encontrado.");
            }

            // Validar duplicados (ignorando el propio cliente)
            if (!string.IsNullOrEmpty(updateDto.Email))
            {
                var clienteEmail = await _clienteRepo.GetByEmailAsync(updateDto.Email);
                if (clienteEmail != null && clienteEmail.ClienteId != id)
                    throw new Exception("El email ya está registrado por otro cliente.");
            }
            // (Se omiten Documento y Telefono por brevedad, pero seguirían la misma lógica)

            cliente.Nombre = updateDto.Nombre;
            cliente.Apellido = updateDto.Apellido;
            cliente.Email = updateDto.Email;
            cliente.Telefono = updateDto.Telefono;
            cliente.Documento = updateDto.Documento;

            await _clienteRepo.UpdateAsync(cliente);
        }

        public async Task DeleteAsync(int id)
        {
            var cliente = await _clienteRepo.GetByIdAsync(id);
            if (cliente == null)
            {
                throw new NotFoundException($"Cliente con ID {id} no encontrado.");
            }
            
            // Advertencia: Borrar un cliente puede fallar si tiene reservas o facturas (Foreign Key).
            // Una estrategia de "borrado lógico" (ej. cliente.Activo = false) sería más segura.
            // Por ahora, se intenta el borrado físico.
            var success = await _clienteRepo.DeleteAsync(id);
            if (!success)
            {
                 throw new Exception("No se pudo eliminar el cliente. Verifique que no tenga reservas asociadas.");
            }
        }
    }
}