using System.Text;
using System.Text.Json;

namespace Venta_de_butaca.Services;

public class MercadoPagoAuthService
{
    private readonly HttpClient _httpClient;
    private readonly MercadoPagoSettings _settings;
    private string? _cachedAccessToken;
    private DateTime _tokenExpiration;

    public MercadoPagoAuthService(HttpClient httpClient, MercadoPagoSettings settings)
    {
        _httpClient = httpClient;
        _settings = settings;
    }

    public async Task<string> GetAccessTokenAsync()
    {
        // Si tenemos un token válido en caché, lo usamos
        if (!string.IsNullOrEmpty(_cachedAccessToken) && DateTime.UtcNow < _tokenExpiration)
        {
            return _cachedAccessToken;
        }

        try
        {
            // Implementar Client Credentials flow
            var tokenRequest = new
            {
                grant_type = "client_credentials",
                client_id = _settings.ClientId,
                client_secret = _settings.ClientSecret
            };

            var json = JsonSerializer.Serialize(tokenRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            Console.WriteLine($"Solicitando token con Client ID: {_settings.ClientId}");

            var response = await _httpClient.PostAsync("https://api.mercadopago.com/oauth/token", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"Response status: {response.StatusCode}");
            Console.WriteLine($"Response: {responseContent}");

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Error al obtener Access Token: {response.StatusCode} - {responseContent}");
            }

            var tokenResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
            
            if (tokenResponse.TryGetProperty("access_token", out var accessToken))
            {
                _cachedAccessToken = accessToken.GetString();
                
                // El token expira en 6 horas según la documentación
                _tokenExpiration = DateTime.UtcNow.AddHours(5.5); // Un poco antes para estar seguros
                
                Console.WriteLine($"Token obtenido exitosamente, expira en: {_tokenExpiration}");
                return _cachedAccessToken!;
            }

            throw new InvalidOperationException("No se pudo obtener el access_token de la respuesta");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error obteniendo token: {ex.Message}");
            throw;
        }
    }
}
