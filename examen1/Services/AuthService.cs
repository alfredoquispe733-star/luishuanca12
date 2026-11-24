using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Contratos.DTOs;
using examen1.Data;
using examen1.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ModeloUsuario = examen1.Models.Usuario;

namespace examen1.Services
{
    public class AuthService : IAuthInterface
    {
        private readonly BaseDatos _context;
        private readonly IConfiguration _config;
        private readonly PasswordHasher<ModeloUsuario> _hasher = new();

        public AuthService(BaseDatos context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public void Registrar(LoginDto dto)
        {
            var existe = _context.Usuarios.Any(u => u.NombreUsuario == dto.NombreUsuario);
            if (existe)
            {
                throw new InvalidOperationException("El nombre de usuario ya existe.");
            }

            var usuario = new ModeloUsuario
            {
                Id = Guid.NewGuid(),
                NombreUsuario = dto.NombreUsuario,
                Rol = "Usuario"
            };

            usuario.PasswordHash = _hasher.HashPassword(usuario, dto.Password);

            _context.Usuarios.Add(usuario);
            _context.SaveChanges();
        }

        public LoginRespuestaDTO? Login(LoginDto dto)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.NombreUsuario == dto.NombreUsuario);
            if (usuario == null)
            {
                return null;
            }

            var resultado = _hasher.VerifyHashedPassword(usuario, usuario.PasswordHash, dto.Password);
            if (resultado == PasswordVerificationResult.Failed)
            {
                return null;
            }

            return new LoginRespuestaDTO
            {
                NombreUsuario = usuario.NombreUsuario,
                Rol = usuario.Rol,
                Token = GenerarToken(usuario)
            };
        }

        public string GenerarToken(ModeloUsuario usuario)
        {
            var jwtKey = _config["Jwt:Key"] ?? "ClaveSecretaSuperLargaParaExamen2026_123456";
            var issuer = _config["Jwt:Issuer"] ?? "examen1";
            var audience = _config["Jwt:Audience"] ?? "examen2";

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.NombreUsuario),
                new Claim(ClaimTypes.Role, usuario.Rol)
            };

            var tokenDescriptor = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}