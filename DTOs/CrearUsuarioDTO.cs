using System.ComponentModel.DataAnnotations;

namespace complejoDeportivo.DTOs
{
    public class CreateUsuarioDTO
    {
        [EmailAddress]
        public required string Email { get; set; }

        [MinLength(6)]
        public required string Password { get; set; }
        public required string TipoUsuario { get; set; }
        public required int ClienteId { get; set; }
        public required int EmpleadoId { get; set; }
    }
}