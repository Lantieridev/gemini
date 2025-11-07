using System.ComponentModel.DataAnnotations;

namespace complejoDeportivo.DTOs
{
    public class CreateTipoCanchaDTO
    {
        [Required]
        [StringLength(50)]
        public required string Nombre { get; set; }
    }
}