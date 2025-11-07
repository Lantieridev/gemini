namespace complejoDeportivo.DTOs.Dashboard
{
    public class AlertaStockDto
    {
        public int ProductoId { get; set; }
        public string ProductoNombre { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string UnidadMedida { get; set; } = string.Empty;
        public int StockActual { get; set; }
        public int StockMinimo { get; set; }
        public int StockMaximo { get; set; }
        public DateTime? UltimoMovimiento { get; set; }
        public string ProveedorPrincipal { get; set; } = string.Empty;

        // PROPIEDADES CALCULADAS
        public int DiferenciaStock => StockActual - StockMinimo;
        public decimal PorcentajeStock => StockMaximo > 0 ? (StockActual / (decimal)StockMaximo) * 100 : 0;

        public string NivelAlerta
        {
            get
            {
                if (StockActual <= 0) return "agotado";
                if (StockActual <= StockMinimo * 0.3) return "crítico";
                if (StockActual <= StockMinimo) return "bajo";
                if (PorcentajeStock >= 90) return "exceso";
                return "normal";
            }
        }

        public string ColorAlerta => NivelAlerta switch
        {
            "agotado" => "#DC3545",    // Rojo
            "crítico" => "#FF2D00",    // Rojo intenso  
            "bajo" => "#FFA500",       // Naranja
            "exceso" => "#17A2B8",     // Azul
            _ => "#2E8B57"             // Verde
        };

        public string IconoAlerta => NivelAlerta switch
        {
            "agotado" => "🆘",
            "crítico" => "🚨",
            "bajo" => "⚠️",
            "exceso" => "📦",
            _ => "✅"
        };

        public string TextoAlerta => NivelAlerta switch
        {
            "agotado" => "AGOTADO",
            "crítico" => "STOCK CRÍTICO",
            "bajo" => "STOCK BAJO",
            "exceso" => "STOCK ALTO",
            _ => "NORMAL"
        };

        public int DiasSinMovimiento => UltimoMovimiento.HasValue ?
            (DateTime.Now - UltimoMovimiento.Value).Days : -1;

        public bool NecesitaReposicionUrgente => NivelAlerta is "agotado" or "crítico";
        public bool NecesitaAtencion => NivelAlerta is "bajo";

        // Para la barra de progreso en el frontend
        public int AnchoBarraStock => (int)Math.Min(PorcentajeStock, 100);
        public string ColorBarraStock => PorcentajeStock switch
        {
            < 20 => "#DC3545",
            < 50 => "#FFA500",
            < 80 => "#17A2B8",
            _ => "#2E8B57"
        };

        // MÉTODOS ESTÁTICOS
        public static List<AlertaStockDto> FiltrarPorNivel(List<AlertaStockDto> alertas, string nivel)
        {
            return alertas.Where(a => a.NivelAlerta == nivel).ToList();
        }

        public static List<AlertaStockDto> ObtenerAlertasUrgentes(List<AlertaStockDto> alertas)
        {
            return alertas.Where(a => a.NecesitaReposicionUrgente).ToList();
        }

        public static Dictionary<string, List<AlertaStockDto>> AgruparPorCategoria(List<AlertaStockDto> alertas)
        {
            return alertas
                .GroupBy(a => a.Categoria)
                .ToDictionary(g => g.Key, g => g.ToList());
        }

        public static List<AlertaStockDto> OrdenarPorUrgencia(List<AlertaStockDto> alertas)
        {
            var ordenUrgencia = new Dictionary<string, int>
            {
                ["agotado"] = 1,
                ["crítico"] = 2,
                ["bajo"] = 3,
                ["exceso"] = 4,
                ["normal"] = 5
            };

            return alertas.OrderBy(a => ordenUrgencia[a.NivelAlerta]).ToList();
        }
    }
}