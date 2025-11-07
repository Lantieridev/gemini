namespace complejoDeportivo.DTOs.Dashboard
{
    public class CanchaPopularDto
    {
        public int CanchaId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string TipoCancha { get; set; } = string.Empty;
        public int PosicionRanking { get; set; }
        public int ReservasCount { get; set; }
        public int HorasTotales { get; set; }
        public decimal IngresosTotales { get; set; }
        public decimal OcupacionPorcentaje { get; set; }

        // PROPIEDADES CALCULADAS PARA EL FRONTEND
        public string PosicionDisplay => $"#{PosicionRanking}";
        public string IngresosDisplay => $"${IngresosTotales:N0}";
        public string OcupacionDisplay => $"{OcupacionPorcentaje:F1}%";
        public string HorasDisplay => $"{HorasTotales}h";

        // Para la barra de progreso en el frontend
        public int AnchoBarraOcupacion => (int)Math.Min(OcupacionPorcentaje, 100);

        // Para colores según la posición en el ranking
        public string ColorPosicion => PosicionRanking switch
        {
            1 => "#FFD700", // Oro
            2 => "#C0C0C0", // Plata  
            3 => "#CD7F32", // Bronce
            _ => "#6B7280"  // Gris
        };

        // Para iconos según el tipo de cancha
        public string IconoTipoCancha => TipoCancha.ToLower() switch
        {
            "fútbol 5" or "futbol 5" => "⚽",
            "fútbol 7" or "futbol 7" => "🥅",
            "fútbol 11" or "futbol 11" => "🏟️",
            _ => "⚽"
        };

        // MÉTODOS ESTÁTICOS ÚTILES
        public static List<CanchaPopularDto> AplicarRanking(List<CanchaPopularDto> canchas)
        {
            var ranked = canchas
                .OrderByDescending(c => c.ReservasCount)
                .ThenByDescending(c => c.IngresosTotales)
                .Select((cancha, index) =>
                {
                    cancha.PosicionRanking = index + 1;
                    return cancha;
                })
                .ToList();

            return ranked;
        }


        // Para agrupar por tipo de cancha si se necesita
        public static Dictionary<string, List<CanchaPopularDto>> AgruparPorTipo(List<CanchaPopularDto> canchas)
        {
            return canchas
                .GroupBy(c => c.TipoCancha)
                .ToDictionary(g => g.Key, g => g.ToList());
        }
    }
}