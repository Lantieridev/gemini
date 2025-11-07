namespace complejoDeportivo.DTOs
{
    public class CrearCanchaDTO
    {
        // [NUEVO] (Problema 3a) - Propiedad añadida
        public int ComplejoId { get; set; }

        public int TipoCanchaId { get; set; }

        public int TipoSuperficieId { get; set; }

        public required string Nombre { get; set; }

    }
}