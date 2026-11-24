using System.Net.Http.Headers;
using System.Net.Http.Json;
using Contratos.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace examen2.Pages.Jefes
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public List<JefeItem> Jefes { get; set; } = new();

        [BindProperty]
        public JefeDto NuevoJefe { get; set; } = new();

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

            if (!ModelState.IsValid)
            {
                await CargarJefesAsync(client);
                ErrorMessage = "Completa todos los campos del jefe.";
                return Page();
            }

            var response = await client.PostAsJsonAsync("api/Jefe", NuevoJefe);
            if (response.IsSuccessStatusCode)
            {
                StatusMessage = "Jefe registrado correctamente.";
                return RedirectToPage();
            }

            var detalle = await response.Content.ReadAsStringAsync();
            ErrorMessage = string.IsNullOrWhiteSpace(detalle)
                ? "No se pudo registrar el jefe. Verifica que el nombre no esté repetido."
                : detalle.Trim('"');

            await CargarJefesAsync(client);
            return Page();
        }

        private async Task CargarJefesAsync(HttpClient client)
        {
            var lista = await client.GetFromJsonAsync<List<JefeItem>>("api/Jefe");
            Jefes = lista ?? new List<JefeItem>();
        }

        public class JefeItem
        {
            public Guid Id { get; set; }
            public string? Nombre { get; set; }
            public string? Cargo { get; set; }
            public string? Correo { get; set; }
        }
    }
}