namespace complejoDeportivo.DTOs
{
    public class DisponibilidadCanchaDTO
    {
        public int CanchaId { get; set; }
        public DateOnly Fecha { get; set; }
        public TimeOnly HoraInicio { get; set; }
        public TimeOnly HoraFin { get; set; } // HoraInicio + 1h
    }
}
