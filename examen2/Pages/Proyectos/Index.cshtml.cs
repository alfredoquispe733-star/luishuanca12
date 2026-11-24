using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Contratos.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace examen2.Pages.Proyectos
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public List<ProyectoItem> Proyectos { get; set; } = new();
        public List<JefeOpcion> Jefes { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public Guid? BuscarId { get; set; }

        public ProyectoConJefeDto? ProyectoBuscado { get; set; }
        public string? BuscarError { get; set; }

        [BindProperty]
        public ProyectoDto NuevoProyecto { get; set; } = new() { Estado = "Activo" };

        [TempData]
        public string? StatusMessage { get; set; }

        [TempData]
        public string? ErrorMessage { get; set; }

        private HttpClient? CrearClienteAutenticado()
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrWhiteSpace(token))
            {
                return null;
            }

            var client = _httpClientFactory.CreateClient("api");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var client = CrearClienteAutenticado();
            if (client == null)
            {
                return RedirectToPage("/Auth/Login");
            }

            await CargarDatosAsync(client);

            if (BuscarId.HasValue && BuscarId.Value != Guid.Empty)
            {
                var resp = await client.GetAsync($"api/Proyecto/{BuscarId.Value}");
                if (resp.IsSuccessStatusCode)
                {
                    ProyectoBuscado = await resp.Content.ReadFromJsonAsync<ProyectoConJefeDto>();
                }
                else if (resp.StatusCode == HttpStatusCode.NotFound)
                {
                    BuscarError = "No se encontró ningún proyecto con ese ID.";
                }
                else
                {
                    BuscarError = "No se pudo consultar el proyecto.";
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var client = CrearClienteAutenticado();
            if (client == null)
            {
                return RedirectToPage("/Auth/Login");
            }

            if (!ModelState.IsValid || NuevoProyecto.IdJefe == Guid.Empty)
            {
                await CargarDatosAsync(client);
                ErrorMessage = "Completa todos los campos y selecciona un jefe.";
                return Page();
            }

            var response = await client.PostAsJsonAsync("api/Proyecto", NuevoProyecto);
            if (response.IsSuccessStatusCode)
            {
                StatusMessage = "Proyecto creado correctamente.";
                return RedirectToPage();
            }

            var detalle = await response.Content.ReadAsStringAsync();
            ErrorMessage = string.IsNullOrWhiteSpace(detalle)
                ? "No se pudo crear el proyecto. Revisa nombre único o que el jefe no tenga otro proyecto."
                : detalle.Trim('"');

            await CargarDatosAsync(client);
            return Page();
        }

        private async Task CargarDatosAsync(HttpClient client)
        {
            var proyectos = await client.GetFromJsonAsync<List<ProyectoItem>>("api/Proyecto");
            Proyectos = proyectos ?? new List<ProyectoItem>();

            var jefes = await client.GetFromJsonAsync<List<JefeOpcion>>("api/Jefe");
            Jefes = jefes ?? new List<JefeOpcion>();
        }

        public class JefeOpcion
        {
            public Guid Id { get; set; }
            public string? Nombre { get; set; }
            public string? Cargo { get; set; }
        }

        public class ProyectoItem
        {
            public Guid Id { get; set; }
            public string? Nombre { get; set; }
            public string? Descripcion { get; set; }
            public string? Estado { get; set; }
            public Guid IdJefe { get; set; }
            public JefeOpcion? Jefe { get; set; }
        }
    }
}