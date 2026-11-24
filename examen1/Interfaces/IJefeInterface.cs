using Contratos.DTOs;
using examen1.Models;

namespace examen1.Interfaces
{
    public interface IJefeInterface
    {
        IEnumerable<Jefe> ObtenerTodos();
        Jefe? ObtenerPorId(Guid id);
        void Crear(JefeDto jefeDto);
    }
}