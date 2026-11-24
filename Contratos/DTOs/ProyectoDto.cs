namespace Contratos.DTOs
{
    public class ProyectoDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public Guid IdJefe { get; set; }
    }
}