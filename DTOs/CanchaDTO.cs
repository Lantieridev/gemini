using complejoDeportivo.Models;

namespace complejoDeportivo.DTOs
{
    public class CanchaDTO(Cancha cancha)
	{
		public int CanchaId { get; set; } = cancha.CanchaId;

		public int ComplejoId { get; set; } = cancha.ComplejoId;

		public int TipoCanchaId { get; set; } = cancha.TipoCanchaId;

		public int TipoSuperficieId { get; set; } = cancha.TipoSuperficieId;

		public string Nombre { get; set; } = cancha.Nombre;

		public bool Activa { get; set; } = cancha.Activa;
	}
}
