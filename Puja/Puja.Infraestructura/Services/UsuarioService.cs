using System;
using Puja.Application.Interfaces;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Puja.Infraestructura.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly HttpClient _httpClient;

        public UsuarioService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> UsuarioExisteAsync(string userId)
        {
            var response = await _httpClient.GetAsync($"api/usuario/existe/{userId}");

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            var content = await response.Content.ReadAsStringAsync();
            var userExists = JsonConvert.DeserializeObject<bool>(content);

            return userExists;
        }
    }
}