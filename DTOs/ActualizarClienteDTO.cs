using System.ComponentModel.DataAnnotations;

namespace complejoDeportivo.DTOs
{
    public class ActualizarClienteDTO
    {
        [Required]
        [StringLength(100)]
        public required string Nombre { get; set; }

        [Required]
        [StringLength(100)]
        public required string Apellido { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        public string? Telefono { get; set; }
        
        public string? Documento { get; set; }
    }
}