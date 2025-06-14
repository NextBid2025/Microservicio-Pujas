using MongoDB.Bson.IO;
using Puja.Application.Interfaces;
using Newtonsoft.Json;

namespace Puja.Infraestructura.Services;

public class SubastaService : ISubastaService
{
    private readonly HttpClient _httpClient;

    public SubastaService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> SubastaEstaActivaAsync(string subastaId)
    {
        var response = await _httpClient.GetAsync($"/api/subastas/{subastaId}/estado");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        dynamic estado = Newtonsoft.Json.JsonConvert.DeserializeObject(content);
        return estado.activa == true;
    }
    public async Task ActualizarPrecioSubastaAsync(string subastaId, decimal nuevoPrecio)
    {
        var body = new { precioActual = nuevoPrecio };
        var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(body), System.Text.Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync($"/api/subastas/{subastaId}/precio", content);
        response.EnsureSuccessStatusCode();
    }
}