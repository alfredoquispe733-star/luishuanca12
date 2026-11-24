using System.Net.Http.Json;
using Contratos.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace examen2.Pages.Auth
{
    public class RegistroModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public RegistroModel(IHttpClientFactory httpClientFactory)
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
            var response = await client.PostAsJsonAsync("api/Auth/registrar", Input);

            if (!response.IsSuccessStatusCode)
            {
                ErrorMessage = "No se pudo registrar el usuario.";
                return Page();
            }

            StatusMessage = "Usuario registrado correctamente. Ahora inicia sesión.";
            return RedirectToPage("/Auth/Login");
        }
    }
}