using System.Net.Http.Headers;
using System.Net.Http.Json;
using Contratos.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace examen2.Pages.Proyectos
{
    public class CreateModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CreateModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public List<JefeOpcion> Jefes { get; set; } = new();

        [BindProperty]
        public ProyectoDto NuevoProyecto { get; set; } = new() { Estado = "Activo" };

        public string? ErrorMessage { get; set; }

        [TempData]
        public string? StatusMessage { get; set; }

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

            await CargarJefesAsync(client);
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
                await CargarJefesAsync(client);
                ErrorMessage = "Completa todos los campos y selecciona un jefe.";
                return Page();
            }

            var response = await client.PostAsJsonAsync("api/Proyecto", NuevoProyecto);
            if (response.IsSuccessStatusCode)
            {
                StatusMessage = "Proyecto creado correctamente.";
                return RedirectToPage("/Proyectos/Index");
            }

            var detalle = await response.Content.ReadAsStringAsync();
            ErrorMessage = string.IsNullOrWhiteSpace(detalle)
                ? "No se pudo crear el proyecto. Revisa nombre único o que el jefe no tenga otro proyecto."
                : detalle.Trim('"');

            await CargarJefesAsync(client);
            return Page();
        }

        private async Task CargarJefesAsync(HttpClient client)
        {
            var jefes = await client.GetFromJsonAsync<List<JefeOpcion>>("api/Jefe");
            Jefes = jefes ?? new List<JefeOpcion>();
        }

        public class JefeOpcion
        {
            public Guid Id { get; set; }
            public string? Nombre { get; set; }
            public string? Cargo { get; set; }
        }
    }
}