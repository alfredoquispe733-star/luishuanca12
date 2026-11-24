using Bogus;
using examen1.Data;
using examen1.Models;

namespace examen1.Factories
{
    public static class JefesFactories
    {
        public static List<Jefe> Seed(BaseDatos context)
        {
            if (context.Jefes.Any())
            {
                return context.Jefes.Take(10).ToList();
            }

            var faker = new Faker<Jefe>("es")
                .RuleFor(j => j.Id, f => Guid.NewGuid())
                .RuleFor(j => j.Nombre, f => $"{f.Name.FullName()} #{f.UniqueIndex}")
                .RuleFor(j => j.Cargo, f => f.PickRandom(new[]
                {
                    "Jefe de Proyectos",
                    "Gerente Técnico",
                    "Coordinador General",
                    "Líder de Operaciones",
                    "Director de Área"
                }))
                .RuleFor(j => j.Correo, (f, j) =>
                {
                    var partes = (j.Nombre ?? "Usuario").Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    var nombre = partes.FirstOrDefault() ?? "usuario";
                    var apellido = partes.Length > 1 ? partes.Last() : "jefe";
                    return f.Internet.Email(nombre, apellido);
                });

            var jefes = faker.Generate(10);
            context.Jefes.AddRange(jefes);
            context.SaveChanges();

            return jefes;
        }
    }
}