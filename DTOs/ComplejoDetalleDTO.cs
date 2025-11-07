namespace complejoDeportivo.DTOs
{
    public class ComplejoDetalleDTO
    {
        public int ComplejoId { get; set; }
        public required string Nombre { get; set; }
        public required DireccionDTO Direccion { get; set; }
    }
}