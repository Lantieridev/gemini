namespace complejoDeportivo.DTOs
{
    public class UsuarioDTO
    {
        public int UsuarioId { get; set; }
        public required string Email { get; set; }
        public required string TipoUsuario { get; set; }
        public int? ClienteId { get; set; }
        public int? EmpleadoId { get; set; }
    }
}