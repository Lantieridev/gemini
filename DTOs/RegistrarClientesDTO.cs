using System.ComponentModel.DataAnnotations;

namespace complejoDeportivo.DTOs
{
    // DTO para que un invitado se registre.
    // Solo puede registrarse como Cliente.
    public class RegisterClienteDTO
    {

        [EmailAddress]
        public required string Email { get; set; }

        [MinLength(6)]
        public required string Password { get; set; }

        [StringLength(100)]
        public required string Nombre { get; set; }

        [StringLength(100)]
        public required string Apellido { get; set; }
        public required string Telefono { get; set; }
        public required string Documento { get; set; }
    }
}