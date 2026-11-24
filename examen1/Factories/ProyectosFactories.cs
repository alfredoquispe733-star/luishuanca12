using Bogus;
using examen1.Data;
using examen1.Models;

namespace examen1.Factories
{
    public static class ProyectosFactories
    {
        public static void Seed(BaseDatos context)
        {
            if (context.Proyectos.Any())
            {
                return;
            }

            var jefes = JefesFactories.Seed(context);
            var jefesDisponibles = jefes
                .Where(j => !context.Proyectos.Any(p => p.IdJefe == j.Id))
                .Take(10)
                .ToList();

            var indiceJefe = 0;

            var faker = new Faker<Proyecto>("es")
                .RuleFor(p => p.Id, f => Guid.NewGuid())
                .RuleFor(p => p.Nombre, f => $"Proyecto {f.Commerce.ProductName()} #{f.UniqueIndex}")
                .RuleFor(p => p.Descripcion, f => f.Lorem.Sentence(8))
                .RuleFor(p => p.Estado, f => f.PickRandom(new[] { "Activo", "En progreso", "Pendiente", "Finalizado" }))
                .RuleFor(p => p.IdJefe, f =>
                {
                    if (indiceJefe < jefesDisponibles.Count)
                    {
                        return jefesDisponibles[indiceJefe++].Id;
                    }
                    return Guid.NewGuid();
                });

            var proyectos = faker
                .Generate(jefesDisponibles.Count)
                .Where(p => p.IdJefe != Guid.Empty)
                .ToList();

            context.Proyectos.AddRange(proyectos);
            context.SaveChanges();
        }
    }
}