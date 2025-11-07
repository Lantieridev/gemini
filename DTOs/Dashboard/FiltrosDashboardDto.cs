using System.ComponentModel.DataAnnotations;

namespace complejoDeportivo.DTOs.Dashboard
{
    public class FiltrosDashboardDto
    {
        // FILTRO PRINCIPAL OBLIGATORIO
        [Required(ErrorMessage = "El complejo es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "El complejo ID debe ser válido")]
        [Display(Name = "Complejo")]
        public int ComplejoId { get; set; }

        // FILTROS DE FECHA
        [DataType(DataType.Date)]
        [Display(Name = "Fecha desde")]
        public DateTime? Desde { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha hasta")]
        [CustomValidation(typeof(FiltrosDashboardDto), nameof(ValidarRangoFechas))]
        public DateTime? Hasta { get; set; }

        // FILTROS ESPECÍFICOS DEL COMPLEJO
        [Display(Name = "Cancha")]
        public int? CanchaId { get; set; }

        [Display(Name = "Tipo de cancha")]
        public int? TipoCanchaId { get; set; }

        [Display(Name = "Estado reserva")]
        public int? EstadoReservaId { get; set; }

        [Display(Name = "Cliente")]
        public int? ClienteId { get; set; }

        [Display(Name = "Asador")]
        public int? AsadorId { get; set; }

        [Display(Name = "Estado pago")]
        public int? EstadoPagoId { get; set; }

        // CONFIGURACIÓN DE PERIODO PREDEFINIDO
        [Display(Name = "Período")]
        public string? PeriodoPredefinido { get; set; }

        // PAGINACIÓN (para tablas del dashboard)
        [Range(1, int.MaxValue, ErrorMessage = "La página debe ser mayor a 0")]
        public int Pagina { get; set; } = 1;

        [Range(1, 100, ErrorMessage = "El tamaño de página debe estar entre 1 y 100")]
        public int TamanoPagina { get; set; } = 10;

        // ORDENAMIENTO
        public string OrdenarPor { get; set; } = "fecha";
        public bool OrdenDescendente { get; set; } = true;

        // MÉTODOS DE VALIDACIÓN
        public static ValidationResult? ValidarRangoFechas(DateTime? hasta, ValidationContext context)
        {
            var instance = (FiltrosDashboardDto)context.ObjectInstance;

            if (instance.Desde.HasValue && hasta.HasValue && hasta.Value < instance.Desde.Value)
            {
                return new ValidationResult("La fecha 'Hasta' no puede ser menor que la fecha 'Desde'");
            }

            // Validar que el rango no sea mayor a 1 año
            if (instance.Desde.HasValue && hasta.HasValue &&
                (hasta.Value - instance.Desde.Value).TotalDays > 365)
            {
                return new ValidationResult("El rango de fechas no puede ser mayor a 1 año");
            }

            return ValidationResult.Success;
        }

        // MÉTODOS HELPER ESPECÍFICOS PARA COMPLEJO
        public (DateTime inicio, DateTime fin) ObtenerRangoFechas()
        {
            // Si ya tiene fechas específicas, usarlas
            if (Desde.HasValue && Hasta.HasValue)
                return (Desde.Value.Date, Hasta.Value.Date);

            // Si no, calcular según período predefinido
            var hoy = DateTime.Today;
            return PeriodoPredefinido?.ToLower() switch
            {
                "hoy" => (hoy, hoy),
                "ayer" => (hoy.AddDays(-1), hoy.AddDays(-1)),
                "esta_semana" => (hoy.AddDays(-(int)hoy.DayOfWeek + 1), hoy),
                "semana_pasada" => (hoy.AddDays(-(int)hoy.DayOfWeek - 6), hoy.AddDays(-(int)hoy.DayOfWeek)),
                "este_mes" => (new DateTime(hoy.Year, hoy.Month, 1), hoy),
                "mes_pasado" => (new DateTime(hoy.Year, hoy.Month, 1).AddMonths(-1),
                               new DateTime(hoy.Year, hoy.Month, 1).AddDays(-1)),
                "este_anio" => (new DateTime(hoy.Year, 1, 1), hoy),
                "anio_pasado" => (new DateTime(hoy.Year - 1, 1, 1), new DateTime(hoy.Year - 1, 12, 31)),
                "ultimos_7_dias" => (hoy.AddDays(-7), hoy),
                "ultimos_30_dias" => (hoy.AddDays(-30), hoy),
                "ultimos_90_dias" => (hoy.AddDays(-90), hoy),
                _ => (hoy.AddDays(-30), hoy) // Por defecto: últimos 30 días
            };
        }

        public bool TieneFiltrosActivos()
        {
            return TieneFiltrosDeFecha() || TieneFiltrosEspecificos();
        }

        public bool TieneFiltrosDeFecha()
        {
            return Desde.HasValue || Hasta.HasValue || !string.IsNullOrEmpty(PeriodoPredefinido);
        }

        public bool TieneFiltrosEspecificos()
        {
            return CanchaId.HasValue || TipoCanchaId.HasValue ||
                   EstadoReservaId.HasValue || ClienteId.HasValue ||
                   AsadorId.HasValue || EstadoPagoId.HasValue;
        }

        // MÉTODO PARA OBTENER DESCRIPCIÓN DE FILTROS
        public string ObtenerDescripcionFiltros(string nombreComplejo)
        {
            var filtros = new List<string>();

            // Siempre mostrar el complejo
            filtros.Add($"Complejo: {nombreComplejo}");

            if (TieneFiltrosDeFecha())
            {
                var (inicio, fin) = ObtenerRangoFechas();
                filtros.Add($"Período: {inicio:dd/MM/yyyy} - {fin:dd/MM/yyyy}");
            }

            if (CanchaId.HasValue) filtros.Add("Filtrado por cancha");
            if (TipoCanchaId.HasValue) filtros.Add("Filtrado por tipo de cancha");
            if (EstadoReservaId.HasValue) filtros.Add("Filtrado por estado de reserva");
            if (ClienteId.HasValue) filtros.Add("Filtrado por cliente");
            if (AsadorId.HasValue) filtros.Add("Filtrado por asador");
            if (EstadoPagoId.HasValue) filtros.Add("Filtrado por estado de pago");

            return filtros.Any() ? string.Join(" | ", filtros) : $"Complejo: {nombreComplejo} (Sin filtros adicionales)";
        }

        // MÉTODO PARA LIMPIAR FILTROS (manteniendo el complejo)
        public void LimpiarFiltros()
        {
            Desde = null;
            Hasta = null;
            CanchaId = null;
            TipoCanchaId = null;
            EstadoReservaId = null;
            ClienteId = null;
            AsadorId = null;
            EstadoPagoId = null;
            PeriodoPredefinido = null;
            Pagina = 1;
            // No limpiamos ComplejoId ni configuración de paginación/orden
        }

        // MÉTODO PARA CREAR FILTROS POR DEFECTO
        public static FiltrosDashboardDto PorDefecto(int complejoId)
        {
            return new FiltrosDashboardDto
            {
                ComplejoId = complejoId,
                PeriodoPredefinido = "ultimos_30_dias",
                Pagina = 1,
                TamanoPagina = 10,
                OrdenarPor = "fecha",
                OrdenDescendente = true
            };
        }

        // MÉTODO PARA COPIAR FILTROS (útil cuando cambia solo el complejo)
        public FiltrosDashboardDto CopiarConNuevoComplejo(int nuevoComplejoId)
        {
            return new FiltrosDashboardDto
            {
                ComplejoId = nuevoComplejoId,
                Desde = this.Desde,
                Hasta = this.Hasta,
                CanchaId = null, // Resetear cancha específica al cambiar complejo
                TipoCanchaId = this.TipoCanchaId,
                EstadoReservaId = this.EstadoReservaId,
                ClienteId = this.ClienteId,
                AsadorId = null, // Resetear asador específico
                EstadoPagoId = this.EstadoPagoId,
                PeriodoPredefinido = this.PeriodoPredefinido,
                Pagina = this.Pagina,
                TamanoPagina = this.TamanoPagina,
                OrdenarPor = this.OrdenarPor,
                OrdenDescendente = this.OrdenDescendente
            };
        }
    }
}