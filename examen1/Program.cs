using System.Text;
using examen1.Data;
using examen1.Factories;
using examen1.Interfaces;
using examen1.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

namespace examen1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Base de datos
            var db = builder.Configuration.GetConnectionString("ConexionDB");
            builder.Services.AddDbContext<BaseDatos>(options => options.UseSqlServer(db));

            // Controladores y Servicios
            builder.Services.AddControllers();
            builder.Services.AddScoped<IAuthInterface, AuthService>();
            builder.Services.AddScoped<IJefeInterface, JefeService>();
            builder.Services.AddScoped<IProyectoInterface, ProyectoService>();

            // Configuración JWT Bearer
            var jwtKey = builder.Configuration["Jwt:Key"] ?? "ClaveSecretaSuperLargaParaExamen2026_123456";
            var issuer = builder.Configuration["Jwt:Issuer"] ?? "examen1";
            var audience = builder.Configuration["Jwt:Audience"] ?? "examen2";

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = issuer,
                        ValidAudience = audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                    };
                });

            builder.Services.AddAuthorization();

            // CORS para permitir peticiones desde examen2
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("PermitirFrontend", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            // Swagger + botón Authorize (compatible con Microsoft.OpenApi v2)
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "examen1", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Pega aquí tu token JWT (sin escribir Bearer adelante)"
                });

                c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("Bearer", document)] = []
                });
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "examen1 v1");
                    c.RoutePrefix = string.Empty;
                });
            }

            // Seed seguro (no borra datos existentes)
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<BaseDatos>();
                ProyectosFactories.Seed(context);
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseCors("PermitirFrontend");

            // Orden obligatorio
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}