using System.ComponentModel.DataAnnotations;

namespace examen1.Models
{
    public class Jefe
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        public string? Nombre { get; set; }

        [Required(ErrorMessage = "El cargo es requerido")]
        public string? Cargo { get; set; }

        [Required(ErrorMessage = "El correo es requerido")]
        [EmailAddress(ErrorMessage = "El formato del correo no es válido")]
        public string? Correo { get; set; }
    }
}