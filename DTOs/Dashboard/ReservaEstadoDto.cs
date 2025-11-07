namespace complejoDeportivo.DTOs.Dashboard
{
    public class ReservaEstadoDto
    {
        public string Estado { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal Porcentaje { get; set; }
        public string Color { get; set; } = string.Empty;
        public string Icono { get; set; } = string.Empty;

        // Método para calcular colores automáticamente basado en el estado
        public static string ObtenerColorPorEstado(string estado)
        {
            return estado.ToLower() switch
            {
                "pendiente" or "pending" => "#FFA500", // Naranja
                "confirmada" or "confirmed" => "#2E8B57", // Verde
                "cancelada" or "cancelled" => "#DC3545", // Rojo
                "completada" or "completed" => "#17A2B8", // Azul
                "en curso" or "in progress" => "#6F42C1", // Púrpura
                "no show" => "#6C757D", // Gris
                _ => "#6C757D" // Gris por defecto
            };
        }

        // Método para obtener iconos automáticamente
        public static string ObtenerIconoPorEstado(string estado)
        {
            return estado.ToLower() switch
            {
                "pendiente" or "pending" => "⏳",
                "confirmada" or "confirmed" => "✅",
                "cancelada" or "cancelled" => "❌",
                "completada" or "completed" => "🏁",
                "en curso" or "in progress" => "⚽",
                "no show" => "👤",
                _ => "📋"
            };
        }

        // Método para calcular porcentajes de una lista
        public static void CalcularPorcentajes(List<ReservaEstadoDto> estados)
        {
            if (estados == null || !estados.Any()) return;

            var total = estados.Sum(e => e.Cantidad);

            foreach (var estado in estados)
            {
                estado.Porcentaje = total > 0 ? Math.Round((estado.Cantidad / (decimal)total) * 100, 1) : 0;
                estado.Color = ObtenerColorPorEstado(estado.Estado);
                estado.Icono = ObtenerIconoPorEstado(estado.Estado);
            }
        }

        // Método para crear datos de ejemplo (útil para testing)
        public static List<ReservaEstadoDto> CrearDatosEjemplo()
        {
            var datos = new List<ReservaEstadoDto>
            {
                new() { Estado = "Pendiente", Cantidad = 5 },
                new() { Estado = "Confirmada", Cantidad = 12 },
                new() { Estado = "En Curso", Cantidad = 3 },
                new() { Estado = "Completada", Cantidad = 8 },
                new() { Estado = "Cancelada", Cantidad = 2 }
            };

            CalcularPorcentajes(datos);
            return datos;
        }
    }
}