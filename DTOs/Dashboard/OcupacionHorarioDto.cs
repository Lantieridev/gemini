namespace complejoDeportivo.DTOs.Dashboard
{
    public class OcupacionHorarioDto
    {
        public string Horario { get; set; } = string.Empty; // "08:00-10:00"
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        public int CanchasOcupadas { get; set; }
        public int CanchasTotales { get; set; }
        public int AsadoresOcupados { get; set; }
        public int AsadoresTotales { get; set; }

        // PROPIEDADES CALCULADAS
        public decimal PorcentajeOcupacionCanchas =>
            CanchasTotales > 0 ? (CanchasOcupadas / (decimal)CanchasTotales) * 100 : 0;

        public decimal PorcentajeOcupacionAsadores =>
            AsadoresTotales > 0 ? (AsadoresOcupados / (decimal)AsadoresTotales) * 100 : 0;

        public decimal OcupacionTotalPromedio =>
            (PorcentajeOcupacionCanchas + PorcentajeOcupacionAsadores) / 2;

        public string NivelOcupacionCanchas => PorcentajeOcupacionCanchas switch
        {
            >= 80 => "Alta",
            >= 50 => "Media",
            >= 20 => "Baja",
            _ => "Muy Baja"
        };

        public string NivelOcupacionAsadores => PorcentajeOcupacionAsadores switch
        {
            >= 80 => "Alta",
            >= 50 => "Media",
            >= 20 => "Baja",
            _ => "Muy Baja"
        };

        public string ColorOcupacionCanchas => NivelOcupacionCanchas switch
        {
            "Alta" => "#DC3545",     // Rojo
            "Media" => "#FFA500",    // Naranja
            "Baja" => "#2E8B57",     // Verde
            _ => "#6C757D"           // Gris
        };

        public string ColorOcupacionAsadores => NivelOcupacionAsadores switch
        {
            "Alta" => "#DC3545",
            "Media" => "#FFA500",
            "Baja" => "#2E8B57",
            _ => "#6C757D"
        };

        public string IconoOcupacionCanchas => NivelOcupacionCanchas switch
        {
            "Alta" => "🔥",
            "Media" => "⚡",
            "Baja" => "🌱",
            _ => "💤"
        };

        public string IconoOcupacionAsadores => NivelOcupacionAsadores switch
        {
            "Alta" => "🔥",
            "Media" => "⚡",
            "Baja" => "🌱",
            _ => "💤"
        };

        // Para gráficos de barras
        public int AnchoBarraCanchas => (int)Math.Min(PorcentajeOcupacionCanchas, 100);
        public int AnchoBarraAsadores => (int)Math.Min(PorcentajeOcupacionAsadores, 100);

        public bool EsHorarioPico => PorcentajeOcupacionCanchas >= 70;
        public bool EsHorarioValle => PorcentajeOcupacionCanchas <= 30;

        // MÉTODOS ESTÁTICOS
        public static List<OcupacionHorarioDto> CrearFranjasHorarias()
        {
            var franjas = new List<OcupacionHorarioDto>();
            var horaInicio = new TimeSpan(8, 0, 0); // 8:00 AM

            for (int i = 0; i < 7; i++) // 7 franjas de 2 horas
            {
                var inicio = horaInicio.Add(TimeSpan.FromHours(i * 2));
                var fin = inicio.Add(TimeSpan.FromHours(2));

                franjas.Add(new OcupacionHorarioDto
                {
                    Horario = $"{inicio:hh\\:mm}-{fin:hh\\:mm}",
                    HoraInicio = inicio,
                    HoraFin = fin,
                    CanchasTotales = 5,  // Ejemplo: 5 canchas en el complejo
                    AsadoresTotales = 3  // Ejemplo: 3 asadores en el complejo
                });
            }

            return franjas;
        }


        public static OcupacionHorarioDto? ObtenerFranjaMasOcupada(List<OcupacionHorarioDto> franjas)
        {
            return franjas.OrderByDescending(f => f.PorcentajeOcupacionCanchas).FirstOrDefault();
        }

        public static OcupacionHorarioDto? ObtenerFranjaMenosOcupada(List<OcupacionHorarioDto> franjas)
        {
            return franjas.OrderBy(f => f.PorcentajeOcupacionCanchas).FirstOrDefault();
        }

        public static List<OcupacionHorarioDto> FiltrarHorariosPico(List<OcupacionHorarioDto> franjas)
        {
            return franjas.Where(f => f.EsHorarioPico).ToList();
        }

        public static List<OcupacionHorarioDto> FiltrarHorariosValle(List<OcupacionHorarioDto> franjas)
        {
            return franjas.Where(f => f.EsHorarioValle).ToList();
        }
    }
}