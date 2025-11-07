namespace complejoDeportivo.DTOs
{
    public class ClienteDTO
    {
        public int ClienteId { get; set; }
        public required string Nombre { get; set; }
        public required string Apellido { get; set; }
        public string? Email { get; set; }
        public string? Telefono { get; set; }
        public string? Documento { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}