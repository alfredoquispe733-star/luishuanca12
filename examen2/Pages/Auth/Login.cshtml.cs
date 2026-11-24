using System.Net.Http.Json;
using Contratos.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace examen2.Pages.Auth
{
    public class LoginModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public LoginModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public LoginDto Input { get; set; } = new();

        public string? ErrorMessage { get; set; }

        [TempData]
        public string? StatusMessage { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var client = _httpClientFactory.CreateClient("api");
            var response = await client.PostAsJsonAsync("api/Auth/login", Input);

            if (!response.IsSuccessStatusCode)
            {
                ErrorMessage = "Usuario o contraseña incorrectos.";
                return Page();
            }

            var data = await response.Content.ReadFromJsonAsync<LoginRespuestaDTO>();
            if (data == null || string.IsNullOrWhiteSpace(data.Token))
            {
                ErrorMessage = "No se recibió el token desde el servidor.";
                return Page();
            }

            HttpContext.Session.SetString("JwtToken", data.Token);
            HttpContext.Session.SetString("NombreUsuario", data.NombreUsuario ?? Input.NombreUsuario ?? "");
            HttpContext.Session.SetString("Rol", data.Rol ?? "");

            return RedirectToPage("/Proyectos/Index");
        }
    }
}