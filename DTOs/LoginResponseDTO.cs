namespace complejoDeportivo.DTOs
{
    public class LoginResponseDTO
    {
        public required string Email { get; set; }
        public required string Token { get; set; }
        public required string Rol { get; set; }
        public int? ClienteId { get; set; }

    }
}