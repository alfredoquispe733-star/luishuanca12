using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace examen1.Models
{
    public class Proyecto
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        public string? Nombre { get; set; }

        [Required(ErrorMessage = "La descripción es requerida")]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "El estado es requerido")]
        public string? Estado { get; set; }

        public Guid IdJefe { get; set; }

        [ForeignKey(nameof(IdJefe))]
        public virtual Jefe? Jefe { get; set; }
    }
}