namespace complejoDeportivo.DTOs
{
    public class DetalleReservaDTO
    {
        public int DetalleReservaId { get; set; }
        public int CanchaId { get; set; }
        public string NombreCancha { get; set; } = string.Empty; // <--- Inicializado
        public decimal Subtotal { get; set; }
        public int CantidadHoras { get; set; }
    }
}