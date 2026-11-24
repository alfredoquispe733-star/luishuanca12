using Contratos.DTOs;
using examen1.Data;
using examen1.Interfaces;
using examen1.Models;
using Microsoft.EntityFrameworkCore;

namespace examen1.Services
{
    public class ProyectoService : IProyectoInterface
    {
        private readonly BaseDatos _context;

        public ProyectoService(BaseDatos context)
        {
            _context = context;
        }

        public IEnumerable<Proyecto> ObtenerTodos()
        {
            return _context.Proyectos.Include(p => p.Jefe).ToList();
        }

        public Proyecto? ObtenerPorId(Guid id)
        {
            return _context.Proyectos.Include(p => p.Jefe).FirstOrDefault(p => p.Id == id);
        }

        public ProyectoConJefeDto? ObtenerPorIdConJefe(Guid id)
        {
            var proyecto = _context.Proyectos
                .Include(p => p.Jefe)
                .FirstOrDefault(p => p.Id == id);

            if (proyecto == null)
            {
                return null;
            }

            return new ProyectoConJefeDto
            {
                Id = proyecto.Id,
                Nombre = proyecto.Nombre ?? string.Empty,
                Descripcion = proyecto.Descripcion ?? string.Empty,
                Estado = proyecto.Estado ?? string.Empty,
                IdJefe = proyecto.IdJefe,
                Jefe = proyecto.Jefe == null
                    ? null
                    : new JefeDto
                    {
                        Nombre = proyecto.Jefe.Nombre ?? string.Empty,
                        Cargo = proyecto.Jefe.Cargo ?? string.Empty,
                        Correo = proyecto.Jefe.Correo ?? string.Empty
                    }
            };
        }

        public void Crear(ProyectoDto proyectoDto)
        {
            var mismoNombre = _context.Proyectos.Any(p => p.Nombre == proyectoDto.Nombre);
            if (mismoNombre)
            {
                throw new InvalidOperationException("Ya existe un proyecto con ese nombre.");
            }

            var jefeExiste = _context.Jefes.Any(j => j.Id == proyectoDto.IdJefe);
            if (!jefeExiste)
            {
                throw new InvalidOperationException("El jefe asignado no existe.");
            }

            var jefeYaTieneProyecto = _context.Proyectos.Any(p => p.IdJefe == proyectoDto.IdJefe);
            if (jefeYaTieneProyecto)
            {
                throw new InvalidOperationException("El jefe ya está a cargo de un proyecto.");
            }

            var proyecto = new Proyecto
            {
                Id = Guid.NewGuid(),
                Nombre = proyectoDto.Nombre,
                Descripcion = proyectoDto.Descripcion,
                Estado = proyectoDto.Estado,
                IdJefe = proyectoDto.IdJefe
            };

            _context.Proyectos.Add(proyecto);
            _context.SaveChanges();
        }
    }
}