namespace complejoDeportivo.DTOs
{
    public class EmpleadoDTO
    {
        public int EmpleadoId { get; set; }
        public required string Nombre { get; set; }
        public required string Apellido { get; set; }
        public string? Email { get; set; }
        public string? Telefono { get; set; }
        public required string Cargo { get; set; }
        public DateOnly FechaIngreso { get; set; }
    }
}