using System.ComponentModel.DataAnnotations;

namespace complejoDeportivo.DTOs
{
    public class CrearEmpleadoDTO
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
        
        [Required]
        public required string Cargo { get; set; }
    }
}