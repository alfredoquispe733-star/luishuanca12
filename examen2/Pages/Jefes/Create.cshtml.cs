using System.Net.Http.Headers;
using System.Net.Http.Json;
using Contratos.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace examen2.Pages.Jefes
{
    public class CreateModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CreateModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public JefeDto NuevoJefe { get; set; } = new();

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

        public IActionResult OnGet()
        {
            if (CrearClienteAutenticado() == null)
            {
                return RedirectToPage("/Auth/Login");
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

            if (!ModelState.IsValid)
            {
                ErrorMessage = "Completa todos los campos del jefe.";
                return Page();
            }

            var response = await client.PostAsJsonAsync("api/Jefe", NuevoJefe);
            if (response.IsSuccessStatusCode)
            {
                StatusMessage = "Jefe registrado correctamente.";
                return RedirectToPage("/Jefes/Index");
            }

            var detalle = await response.Content.ReadAsStringAsync();
            ErrorMessage = string.IsNullOrWhiteSpace(detalle)
                ? "No se pudo registrar el jefe. Verifica que el nombre no esté repetido."
                : detalle.Trim('"');

            return Page();
        }
    }
}