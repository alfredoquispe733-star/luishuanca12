using Contratos.DTOs;
using examen1.Models;

namespace examen1.Interfaces
{
    public interface IProyectoInterface
    {
        IEnumerable<Proyecto> ObtenerTodos();
        Proyecto? ObtenerPorId(Guid id);
        ProyectoConJefeDto? ObtenerPorIdConJefe(Guid id);
        void Crear(ProyectoDto proyectoDto);
    }
}