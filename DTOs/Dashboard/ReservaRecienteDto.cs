namespace complejoDeportivo.DTOs.Dashboard
{
    public class ReservaRecienteDto
    {
        public int ReservaId { get; set; }
        public string ClienteNombre { get; set; } = string.Empty;
        public string ClienteEmail { get; set; } = string.Empty;
        public string CanchaNombre { get; set; } = string.Empty;
        public string TipoCancha { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        public string Estado { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public DateTime FechaCreacion { get; set; }

        // PROPIEDADES CALCULADAS PARA EL FRONTEND
        public string FechaDisplay => Fecha.ToString("dd/MM/yyyy");
        public string HorarioDisplay => $"{HoraInicio:hh\\:mm} - {HoraInicio:hh\\:mm}";
        public string TotalDisplay => $"${Total:N0}";
        public string DuracionDisplay => $"{(HoraFin - HoraInicio).Hours}h";

        public string EstadoColor => Estado.ToLower() switch
        {
            "pendiente" => "#FFA500",     // Naranja
            "confirmada" => "#2E8B57",    // Verde
            "en curso" => "#17A2B8",      // Azul
            "completada" => "#6F42C1",    // Púrpura
            "cancelada" => "#DC3545",     // Rojo
            _ => "#6C757D"                // Gris
        };

        public string EstadoIcono => Estado.ToLower() switch
        {
            "pendiente" => "⏳",
            "confirmada" => "✅",
            "en curso" => "⚽",
            "completada" => "🏁",
            "cancelada" => "❌",
            _ => "📋"
        };

        public bool EsHoy => Fecha.Date == DateTime.Today;
        public bool EsFutura => Fecha.Date > DateTime.Today;
        public bool EsPasada => Fecha.Date < DateTime.Today;

        public string BadgeFecha => EsHoy ? "HOY" : EsFutura ? "PRÓXIMA" : "PASADA";
        public string ColorBadgeFecha => EsHoy ? "bg-warning" : EsFutura ? "bg-info" : "bg-secondary";

        // Para indicar si está en curso en este momento
        public bool EstaEnCurso
        {
            get
            {
                if (Fecha.Date != DateTime.Today) return false;
                var ahora = DateTime.Now.TimeOfDay;
                return ahora >= HoraInicio && ahora <= HoraFin && Estado == "confirmada";
            }
        }

        public string IconoEnCurso => EstaEnCurso ? "🔴 EN CURSO" : "";

        // MÉTODOS ESTÁTICOS ÚTILES
        public static List<ReservaRecienteDto> FiltrarPorEstado(List<ReservaRecienteDto> reservas, string estado)
        {
            return reservas.Where(r => r.Estado.Equals(estado, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public static List<ReservaRecienteDto> FiltrarPorFecha(List<ReservaRecienteDto> reservas, DateTime fecha)
        {
            return reservas.Where(r => r.Fecha.Date == fecha.Date).ToList();
        }

        public static List<ReservaRecienteDto> ObtenerProximas24Horas(List<ReservaRecienteDto> reservas)
        {
            var ahora = DateTime.Now;
            var en24Horas = ahora.AddHours(24);

            return reservas.Where(r =>
                r.Fecha.Date >= ahora.Date &&
                r.Fecha.Date <= en24Horas.Date &&
                r.Estado == "confirmada"
            ).ToList();
        }


        // Ordenar por fecha y hora (más recientes primero)
        public static List<ReservaRecienteDto> OrdenarPorFecha(List<ReservaRecienteDto> reservas, bool descendente = true)
        {
            return descendente
                ? reservas.OrderByDescending(r => r.Fecha).ThenByDescending(r => r.HoraInicio).ToList()
                : reservas.OrderBy(r => r.Fecha).ThenBy(r => r.HoraInicio).ToList();
        }
    }
}