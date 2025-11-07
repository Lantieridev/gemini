namespace complejoDeportivo.DTOs.Dashboard
{
    public class ClienteFrecuenteDto
    {
        public int ClienteId { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public int TotalReservas { get; set; }
        public decimal TotalGastado { get; set; }
        public DateTime UltimaReserva { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int PosicionRanking { get; set; }

        // PROPIEDADES CALCULADAS PARA EL FRONTEND
        public string PosicionDisplay => $"#{PosicionRanking}";
        public string TotalGastadoDisplay => $"${TotalGastado:N0}";
        public string UltimaReservaDisplay => UltimaReserva.ToString("dd/MM/yyyy");
        public string AntiguedadDisplay => $"Hace {((DateTime.Now - FechaRegistro).Days / 30)} meses";

        public decimal TicketPromedio => TotalReservas > 0 ? TotalGastado / TotalReservas : 0;
        public string TicketPromedioDisplay => $"${TicketPromedio:N0}";

        // Para badges de categoría según gasto total
        public string CategoriaCliente => TotalGastado switch
        {
            > 200000 => "VIP",
            > 100000 => "Frecuente",
            > 50000 => "Regular",
            _ => "Ocasional"
        };

        public string ColorCategoria => CategoriaCliente switch
        {
            "VIP" => "#FFD700",
            "Frecuente" => "#C0C0C0",
            "Regular" => "#CD7F32",
            _ => "#6B7280"
        };

        // Para indicador de actividad reciente
        public bool EsClienteActivo => (DateTime.Now - UltimaReserva).Days <= 30;
        public string IconoActividad => EsClienteActivo ? "🟢" : "⚫";

        // MÉTODOS ESTÁTICOS ÚTILES
        public static List<ClienteFrecuenteDto> AplicarRanking(List<ClienteFrecuenteDto> clientes)
        {
            var ranked = clientes
                .OrderByDescending(c => c.TotalReservas)
                .ThenByDescending(c => c.TotalGastado)
                .Select((cliente, index) =>
                {
                    cliente.PosicionRanking = index + 1;
                    return cliente;
                })
                .ToList();

            return ranked;
        }


        // Filtrar clientes activos (últimos 30 días)
        public static List<ClienteFrecuenteDto> FiltrarActivos(List<ClienteFrecuenteDto> clientes)
        {
            return clientes.Where(c => c.EsClienteActivo).ToList();
        }

        // Agrupar por categoría
        public static Dictionary<string, List<ClienteFrecuenteDto>> AgruparPorCategoria(List<ClienteFrecuenteDto> clientes)
        {
            return clientes
                .GroupBy(c => c.CategoriaCliente)
                .OrderByDescending(g => g.Key == "VIP" ? 4 : g.Key == "Frecuente" ? 3 : g.Key == "Regular" ? 2 : 1)
                .ToDictionary(g => g.Key, g => g.ToList());
        }
    }
}