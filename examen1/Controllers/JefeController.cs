using Contratos.DTOs;
using examen1.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace examen1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize]
    public class JefeController : ControllerBase
    {
        private readonly IJefeInterface _jefeService;

        public JefeController(IJefeInterface jefeService)
        {
            _jefeService = jefeService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_jefeService.ObtenerTodos());
        }

        [HttpGet("{id:guid}")]
        public IActionResult Get(Guid id)
        {
            var jefe = _jefeService.ObtenerPorId(id);
            if (jefe == null)
            {
                return NotFound("El Id no se encuentra en Jefe");
            }
            return Ok(jefe);
        }

        [HttpPost]
        public IActionResult Post([FromBody] JefeDto jefeDto)
        {
            try
            {
                _jefeService.Crear(jefeDto);
                return Ok("Jefe registrado correctamente");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}