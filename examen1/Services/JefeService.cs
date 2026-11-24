using Contratos.DTOs;
using examen1.Data;
using examen1.Interfaces;
using examen1.Models;

namespace examen1.Services
{
    public class JefeService : IJefeInterface
    {
        private readonly BaseDatos _context;

        public JefeService(BaseDatos context)
        {
            _context = context;
        }

        public IEnumerable<Jefe> ObtenerTodos()
        {
            return _context.Jefes.ToList();
        }

        public Jefe? ObtenerPorId(Guid id)
        {
            return _context.Jefes.Find(id);
        }

        public void Crear(JefeDto jefeDto)
        {
            var existeNombre = _context.Jefes.Any(j => j.Nombre == jefeDto.Nombre);
            if (existeNombre)
            {
                throw new InvalidOperationException("Ya existe un jefe con ese nombre.");
            }

            var jefe = new Jefe
            {
                Id = Guid.NewGuid(),
                Nombre = jefeDto.Nombre,
                Cargo = jefeDto.Cargo,
                Correo = jefeDto.Correo
            };

            _context.Jefes.Add(jefe);
            _context.SaveChanges();
        }
    }
}