using System.Text;
using System.Text.Json;

namespace Venta_de_butaca.Services;

public class MercadoPagoRestService : IMercadoPagoService
{
    private readonly HttpClient _httpClient;
    private readonly MercadoPagoSettings _settings;
    private readonly MercadoPagoAuthService _authService;

    public MercadoPagoRestService(HttpClient httpClient, MercadoPagoSettings settings)
    {
        _httpClient = httpClient;
        _settings = settings;
        _authService = new MercadoPagoAuthService(httpClient, settings);
        
        // Configurar timeout
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
    }

    public async Task<string> CreatePreference(List<PaymentItem> items, string successUrl, string failureUrl, string pendingUrl)
    {
        try
        {
            // Obtener un Access Token válido usando Client Credentials
            var accessToken = await _authService.GetAccessTokenAsync();
            Console.WriteLine($"Usando Access Token dinámico: {accessToken.Substring(0, 20)}...");
            
            var preference = new
            {
                items = items.Select(item => new
                {
                    title = item.Title,
                    quantity = item.Quantity,
                    unit_price = item.UnitPrice,
                    currency_id = "CLP"
                }).ToArray(),
                back_urls = new
                {
                    success = successUrl,
                    failure = failureUrl,
                    pending = pendingUrl
                },
                auto_return = "approved",
                payment_methods = new
                {
                    installments = 1
                },
                binary_mode = true,
                statement_descriptor = "Teatro Butacas"
            };

            var json = JsonSerializer.Serialize(preference, new JsonSerializerOptions 
            { 
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower 
            });
            
            Console.WriteLine($"JSON request: {json}");
            
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Limpiar headers previos
            _httpClient.DefaultRequestHeaders.Clear();
            
            // Configurar headers con el token dinámico
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "TeatroButacas/1.0");

            var apiUrl = "https://api.mercadopago.com/checkout/preferences";
            
            Console.WriteLine($"Enviando request a: {apiUrl}");
            Console.WriteLine($"Authorization: Bearer {accessToken.Substring(0, 20)}...");
            
            var response = await _httpClient.PostAsync(apiUrl, content);
            
            Console.WriteLine($"Response status: {response.StatusCode}");
            
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Response content: {responseContent}");
            
            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Error de API: {response.StatusCode} - {responseContent}");
            }

            var result = JsonSerializer.Deserialize<JsonElement>(responseContent);
            
            if (result.TryGetProperty("init_point", out var initPoint))
            {
                var initPointUrl = initPoint.GetString();
                Console.WriteLine($"Init point obtenido: {initPointUrl}");
                return initPointUrl ?? throw new InvalidOperationException("No se pudo obtener el init_point");
            }

            throw new InvalidOperationException($"Respuesta de API inválida: {responseContent}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception details: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
            }
            throw new InvalidOperationException($"Error al crear preferencia: {ex.Message}", ex);
        }
    }
}
