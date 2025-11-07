namespace complejoDeportivo.DTOs.Dashboard
{
    public class DashboardResumenDto
    {
        // MÉTRICAS PRINCIPALES - HOY
        public int ReservasHoy { get; set; }
        public decimal IngresosHoy { get; set; }
        public int ClientesNuevosHoy { get; set; }

        // MÉTRICAS DE ESTADO
        public int ReservasPendientes { get; set; }
        public int ReservasConfirmadas { get; set; }
        public int ReservasCanceladas { get; set; }

        // MÉTRICAS DE INVENTARIO
        public int AlertasStock { get; set; }
        public int ProductosStockBajo { get; set; }
        public int ProductosStockCritico { get; set; }

        // MÉTRICAS DE INFRAESTRUCTURA
        public int CanchasActivas { get; set; }
        public int CanchasTotales { get; set; }
        public int AsadoresActivos { get; set; }
        public int AsadoresTotales { get; set; }

        // MÉTRICAS COMPARATIVAS
        public decimal IngresosMesActual { get; set; }
        public decimal IngresosMesAnterior { get; set; }
        public int ReservasMesActual { get; set; }
        public int ReservasMesAnterior { get; set; }

        // MÉTRICAS DE OCUPACIÓN
        public decimal OcupacionHoyPorcentaje { get; set; }
        public decimal OcupacionMesPorcentaje { get; set; }

        // MÉTRICAS DE CLIENTES
        public int ClientesFrecuentes { get; set; }
        public int TotalClientesRegistrados { get; set; }

        // PROPIEDADES CALCULADAS
        public decimal VariacionIngresos =>
            IngresosMesAnterior > 0 ?
            ((IngresosMesActual - IngresosMesAnterior) / IngresosMesAnterior) * 100 :
            (IngresosMesActual > 0 ? 100 : 0);

        public decimal VariacionReservas =>
            ReservasMesAnterior > 0 ?
            ((ReservasMesActual - ReservasMesAnterior) / (decimal)ReservasMesAnterior) * 100 :
            (ReservasMesActual > 0 ? 100 : 0);

        public bool TendenciaIngresosPositiva => VariacionIngresos >= 0;
        public bool TendenciaReservasPositiva => VariacionReservas >= 0;

        public decimal OcupacionCanchasPorcentaje =>
            CanchasTotales > 0 ? (CanchasActivas / (decimal)CanchasTotales) * 100 : 0;

        public decimal OcupacionAsadoresPorcentaje =>
            AsadoresTotales > 0 ? (AsadoresActivos / (decimal)AsadoresTotales) * 100 : 0;

        // MÉTODOS HELPER
        public string ObtenerIconoTendenciaIngresos()
        {
            return TendenciaIngresosPositiva ? "📈" : "📉";
        }

        public string ObtenerIconoTendenciaReservas()
        {
            return TendenciaReservasPositiva ? "📈" : "📉";
        }

        public string ObtenerColorAlertaStock()
        {
            if (ProductosStockCritico > 0) return "danger";
            if (ProductosStockBajo > 0) return "warning";
            return "success";
        }

        public string ObtenerTextoResumen()
        {
            var puntos = new List<string>();

            if (ReservasHoy > 0)
                puntos.Add($"{ReservasHoy} reservas para hoy");

            if (IngresosHoy > 0)
                puntos.Add($"${IngresosHoy:N0} ingresados hoy");

            if (AlertasStock > 0)
                puntos.Add($"{AlertasStock} alertas de stock");

            if (ReservasPendientes > 0)
                puntos.Add($"{ReservasPendientes} reservas pendientes");

            return puntos.Any() ? string.Join(" • ", puntos) : "Sin actividad hoy";
        }

        // MÉTODO PARA CREAR RESUMEN VACÍO (útil para inicialización)
        public static DashboardResumenDto CrearVacio(int complejoId)
        {
            return new DashboardResumenDto
            {
                ReservasHoy = 0,
                IngresosHoy = 0,
                ClientesNuevosHoy = 0,
                ReservasPendientes = 0,
                ReservasConfirmadas = 0,
                ReservasCanceladas = 0,
                AlertasStock = 0,
                ProductosStockBajo = 0,
                ProductosStockCritico = 0,
                CanchasActivas = 0,
                CanchasTotales = 0,
                AsadoresActivos = 0,
                AsadoresTotales = 0,
                IngresosMesActual = 0,
                IngresosMesAnterior = 0,
                ReservasMesActual = 0,
                ReservasMesAnterior = 0,
                OcupacionHoyPorcentaje = 0,
                OcupacionMesPorcentaje = 0,
                ClientesFrecuentes = 0,
                TotalClientesRegistrados = 0
            };
        }
    }
}