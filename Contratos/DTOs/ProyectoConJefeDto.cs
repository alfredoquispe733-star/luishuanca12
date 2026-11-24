namespace Contratos.DTOs
{
    public class ProyectoConJefeDto
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public Guid IdJefe { get; set; }
        public JefeDto? Jefe { get; set; }
    }
}