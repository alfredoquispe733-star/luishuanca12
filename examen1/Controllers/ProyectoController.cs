using Contratos.DTOs;
using examen1.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace examen1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize]
    public class ProyectoController : ControllerBase
    {
        private readonly IProyectoInterface _proyectoService;

        public ProyectoController(IProyectoInterface proyectoService)
        {
            _proyectoService = proyectoService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_proyectoService.ObtenerTodos());
        }

        [HttpGet("{id:guid}")]
        public IActionResult Get(Guid id)
        {
            var proyecto = _proyectoService.ObtenerPorIdConJefe(id);
            if (proyecto == null)
            {
                return NotFound("El Id no se encuentra en Proyecto");
            }
            return Ok(proyecto);
        }

        [HttpPost]
        public IActionResult Post([FromBody] ProyectoDto proyectoDto)
        {
            try
            {
                _proyectoService.Crear(proyectoDto);
                return Ok("Proyecto registrado correctamente");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}