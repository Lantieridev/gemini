namespace complejoDeportivo.DTOs
{
	public class CrearCanchaDTO
	{
		public int ComplejoId { get; set; }

		public int TipoCanchaId { get; set; }

		public int TipoSuperficieId { get; set; }

		public required string Nombre { get; set; }

	}
}