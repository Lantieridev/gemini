namespace complejoDeportivo.DTOs.Dashboard
{
    public class IngresoPeriodoDto
    {
        public string Periodo { get; set; } = string.Empty; // "Ene", "Feb", "15/01", etc.
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public int CantidadReservas { get; set; }
        public int CantidadClientes { get; set; }
        public decimal PromedioPorReserva => CantidadReservas > 0 ? Total / CantidadReservas : 0;

        // Métodos para formatear periodos según el tipo de agrupación
        public static string FormatearPeriodo(DateTime fecha, string tipoAgrupacion)
        {
            return tipoAgrupacion?.ToLower() switch
            {
                "diario" or "daily" => fecha.ToString("dd/MM"),
                "semanal" or "weekly" => $"Sem {GetWeekOfYear(fecha)}",
                "mensual" or "monthly" => fecha.ToString("MMM"), // Ene, Feb, etc.
                "anual" or "yearly" => fecha.ToString("yyyy"),
                _ => fecha.ToString("dd/MM") // Por defecto diario
            };
        }

        private static int GetWeekOfYear(DateTime fecha)
        {
            var culture = System.Globalization.CultureInfo.CurrentCulture;
            return culture.Calendar.GetWeekOfYear(fecha, culture.DateTimeFormat.CalendarWeekRule, culture.DateTimeFormat.FirstDayOfWeek);
        }

        // Método para crear datos de ejemplo
        public static List<IngresoPeriodoDto> CrearDatosEjemploMensual()
        {
            var random = new Random();
            var meses = new[] { "Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic" };

            return meses.Select((mes, index) => new IngresoPeriodoDto
            {
                Periodo = mes,
                Fecha = new DateTime(DateTime.Now.Year, index + 1, 1),
                Total = random.Next(50000, 200000),
                CantidadReservas = random.Next(50, 200),
                CantidadClientes = random.Next(30, 150)
            }).ToList();
        }

        public static List<IngresoPeriodoDto> CrearDatosEjemploDiario(int dias = 30)
        {
            var random = new Random();
            var datos = new List<IngresoPeriodoDto>();
            var fechaBase = DateTime.Today.AddDays(-dias);

            for (int i = 0; i < dias; i++)
            {
                var fecha = fechaBase.AddDays(i);
                datos.Add(new IngresoPeriodoDto
                {
                    Periodo = fecha.ToString("dd/MM"),
                    Fecha = fecha,
                    Total = random.Next(1000, 15000),
                    CantidadReservas = random.Next(5, 25),
                    CantidadClientes = random.Next(3, 20)
                });
            }

            return datos;
        }
    }
}