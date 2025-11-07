namespace complejoDeportivo.DTOs
{
    public class DireccionDTO
    {
        public int DireccionId { get; set; }
        public required string Calle { get; set; }
        public required string Numero { get; set; }
        public required string Ciudad { get; set; }
        public required string Provincia { get; set; }
        public required string CodigoPostal { get; set; }
    }
}