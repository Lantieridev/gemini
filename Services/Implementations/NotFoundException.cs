namespace complejoDeportivo.Services.Implementations
{
    // Esta excepción ahora puede ser usada por CUALQUIER servicio
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }
}