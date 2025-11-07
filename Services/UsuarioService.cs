using BCrypt.Net;
using complejoDeportivo.DTOs;
using complejoDeportivo.Models;
using complejoDeportivo.Repositories.Implementations;
using complejoDeportivo.Repositories.Interfaces;
using complejoDeportivo.Services.Interfaces;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;

namespace complejoDeportivo.Services.Implementations
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepo;
        private readonly IClienteRepository _clienteRepo;
        private readonly IEmpleadoRepository _empleadoRepository;

        public UsuarioService(IUsuarioRepository usuarioRepository, IClienteRepository clienteRepository, IEmpleadoRepository empleadoRepository)
        {
            _usuarioRepo = usuarioRepository;
            _clienteRepo = clienteRepository;
            _empleadoRepository = empleadoRepository;


        }

        public async Task<IEnumerable<UsuarioDTO>> GetAllAsync()
        {
            var usuarios = await _usuarioRepo.GetAllAsync();
            return usuarios.Select(u => new UsuarioDTO
            {
                UsuarioId = u.UsuarioId,
                Email = u.Email,
                TipoUsuario = u.TipoUsuario,
                ClienteId = u.ClienteId,
                EmpleadoId = u.EmpleadoId
            });
        }

        public async Task<UsuarioDTO> GetByIdAsync(int id)
        {
            var u = await _usuarioRepo.GetByIdAsync(id);
            if (u == null)
            {
                throw new NotFoundException($"Usuario con ID {id} no encontrado.");
            }
            return new UsuarioDTO
            {
                UsuarioId = u.UsuarioId,
                Email = u.Email,
                TipoUsuario = u.TipoUsuario,
                ClienteId = u.ClienteId,
                EmpleadoId = u.EmpleadoId
            };
        }

        public async Task<UsuarioDTO> CreateAsync(CreateUsuarioDTO createDto)
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(createDto.Password);

            var usuario = new Usuario
            {
                Email = createDto.Email,
                PasswordHash = passwordHash,
                TipoUsuario = createDto.TipoUsuario,
                ClienteId = createDto.ClienteId,
                EmpleadoId = createDto.EmpleadoId,
                FechaRegistro = DateTime.UtcNow
            };

            var nuevoUsuario = await _usuarioRepo.CreateAsync(usuario);

            return new UsuarioDTO
            {
                UsuarioId = nuevoUsuario.UsuarioId,
                Email = nuevoUsuario.Email,
                TipoUsuario = nuevoUsuario.TipoUsuario,
                ClienteId = nuevoUsuario.ClienteId,
                EmpleadoId = nuevoUsuario.EmpleadoId
            };
        }

        public async Task UpdateAsync(int id, UsuarioDTO updateDto)
        {
            var usuario = await _usuarioRepo.GetByIdAsync(id);
            if (usuario == null)
            {
                throw new NotFoundException($"Usuario con ID {id} no encontrado.");
            }

            usuario.Email = updateDto.Email;
            usuario.TipoUsuario = updateDto.TipoUsuario;
            usuario.ClienteId = updateDto.ClienteId;
            usuario.EmpleadoId = updateDto.EmpleadoId;

            await _usuarioRepo.UpdateAsync(usuario);
        }

        public async Task DeleteAsync(int id)
        {
            var usuario = await _usuarioRepo.GetByIdAsync(id);
            if (usuario == null)
            {
                throw new NotFoundException($"Usuario con ID {id} no encontrado.");
            }
            await _usuarioRepo.DeleteAsync(id);
        }

        public async Task<UsuarioDTO> RegisterClienteAsync(RegisterClienteDTO dto)
        {
			Console.WriteLine("Empezamos con las validaciones");
            if (await _usuarioRepo.GetByEmailAsync(dto.Email) != null)
            {
				Console.WriteLine("Email");
                throw new System.Exception("El email ya está registrado.");
            }

				Console.WriteLine("!Email");
            if (await _clienteRepo.DoesDocumentoExistAsync(dto.Documento))
            {
				Console.WriteLine("DNI");
                throw new System.Exception("El documento ya se encuentra registrado.");
            }

				Console.WriteLine("!DNI");
            if (await _clienteRepo.DoesTelefonoExistAsync(dto.Telefono))
            {
				Console.WriteLine("TEL");
                throw new System.Exception("El número de teléfono ya se encuentra registrado.");
            }

				Console.WriteLine("!TEL");
			Console.WriteLine("Se pasaron las primeras validaciones... Creando el usuario");
            var nuevoCliente = new Cliente
            {
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Email = dto.Email,
                Telefono = dto.Telefono,
                Documento = dto.Documento,
                FechaRegistro = DateTime.UtcNow
            };

			Console.WriteLine("Se creo el cliente... Intentando guardar en repositorio");
            var clienteCreado = await _clienteRepo.CreateAsync(nuevoCliente);

			Console.WriteLine("Se guardo en repositorio");
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
			Console.WriteLine("Se hasheo pass");

            var nuevoUsuario = new Usuario
            {
                Email = dto.Email,
                PasswordHash = passwordHash,
                TipoUsuario = "Cliente",
                ClienteId = clienteCreado.ClienteId,
                EmpleadoId = null,
                FechaRegistro = DateTime.UtcNow
            };
            var usuarioCreado = await _usuarioRepo.CreateAsync(nuevoUsuario);
			Console.WriteLine("Se guardo usuario en repo");

            return new UsuarioDTO
            {
                UsuarioId = usuarioCreado.UsuarioId,
                Email = usuarioCreado.Email,
                TipoUsuario = usuarioCreado.TipoUsuario,
                ClienteId = usuarioCreado.ClienteId,
                EmpleadoId = usuarioCreado.EmpleadoId
            };

        }
        public async Task<UsuarioDTO> RegisterEmpleadoAsync(RegisterClienteDTO dto)
        {
            // 1. Verificar si el email ya existe
            var existingUser = await _usuarioRepo.GetByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                throw new Exception("El email ya está registrado");
            }

            // 2. Crear la entidad Empleado
            var nuevoEmpleado = new Empleado
            {
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Email = dto.Email,
                // Puedes añadir más campos por defecto si son obligatorios
                Cargo = "Admin Temporal",
                FechaIngreso = DateOnly.FromDateTime(DateTime.UtcNow)
            };
            await _empleadoRepository.CreateAsync(nuevoEmpleado);
            // 'nuevoEmpleado' ahora tiene el EmpleadoId generado por la DB

            // 3. Hashear la contraseña
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            // 4. Crear la entidad Usuario
            var nuevoUsuario = new Usuario
            {
                Email = dto.Email,
                PasswordHash = passwordHash,
                TipoUsuario = "Empleado", // <-- LA CLAVE
                FechaRegistro = DateTime.UtcNow,
                EmpleadoId = nuevoEmpleado.EmpleadoId // <-- Link al empleado
            };

            await _usuarioRepo.CreateAsync(nuevoUsuario);

            // 5. Devolver el DTO
            return new UsuarioDTO
            {
                UsuarioId = nuevoUsuario.UsuarioId,
                Email = nuevoUsuario.Email,
                TipoUsuario = nuevoUsuario.TipoUsuario,
                ClienteId = nuevoUsuario.ClienteId,
                EmpleadoId = nuevoUsuario.EmpleadoId
            };
        }
    }
}