using Contratos.DTOs;
using examen1.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace examen1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthInterface _authService;

        public AuthController(IAuthInterface authService)
        {
            _authService = authService;
        }

        [HttpPost("registrar")]
        public IActionResult Registrar([FromBody] LoginDto dto)
        {
            try
            {
                _authService.Registrar(dto);
                return Ok("Usuario registrado correctamente.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            var respuesta = _authService.Login(dto);
            if (respuesta == null)
            {
                return Unauthorized("Usuario o contraseña incorrectos.");
            }

            return Ok(respuesta);
        }
    }
}