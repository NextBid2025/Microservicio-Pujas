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
}