namespace Contratos.DTOs
{
    public class LoginRespuestaDTO
    {
        public string NombreUsuario { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}