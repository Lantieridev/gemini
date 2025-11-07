using System.Collections.Generic;

namespace complejoDeportivo.DTOs
{
    public class CrearReservaDTO
    {
        public int ClienteId { get; set; }
        public List<int> CanchaIds { get; set; } = new List<int>(); // <--- Inicializado
        public DateOnly Fecha { get; set; } 
        public TimeOnly HoraInicio { get; set; } 
        public TimeOnly HoraFin { get; set; }
        public string Ambito { get; set; } = "Web";
    }
}