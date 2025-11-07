namespace complejoDeportivo.DTOs
{
    public class CancelarReservaDTO
    {
        public int ReservaId { get; set; }
        public int ClienteId { get; set; } 
        public string Motivo { get; set; } = "";
    }
}
