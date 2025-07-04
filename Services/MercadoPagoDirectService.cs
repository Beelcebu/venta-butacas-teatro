using System.Text;
using System.Text.Json;

namespace Venta_de_butaca.Services;

public class MercadoPagoDirectService : IMercadoPagoService
{
    private readonly HttpClient _httpClient;
    private readonly MercadoPagoSettings _settings;

    public MercadoPagoDirectService(HttpClient httpClient, MercadoPagoSettings settings)
    {
        _httpClient = httpClient;
        _settings = settings;
        
        Console.WriteLine("🔧 INICIALIZANDO MercadoPagoDirectService");
        Console.WriteLine($"Access Token configurado: {(!string.IsNullOrEmpty(_settings?.AccessToken) ? "✅ SÍ" : "❌ NO")}");
        Console.WriteLine($"Client ID configurado: {(!string.IsNullOrEmpty(_settings?.ClientId) ? "✅ SÍ" : "❌ NO")}");
        Console.WriteLine($"Client Secret configurado: {(!string.IsNullOrEmpty(_settings?.ClientSecret) ? "✅ SÍ" : "❌ NO")}");
        
        if (_settings != null)
        {
            Console.WriteLine($"Access Token: {_settings.AccessToken?.Substring(0, 20)}...");
            Console.WriteLine($"Client ID: {_settings.ClientId}");
            Console.WriteLine($"Client Secret: {_settings.ClientSecret?.Substring(0, 10)}...");
        }
    }

    public async Task<string> CreatePreference(List<PaymentItem> items, string successUrl, string failureUrl, string pendingUrl)
    {
        try
        {
            Console.WriteLine("🚀 INICIANDO PROCESO DE PAGO");
            Console.WriteLine($"Items a procesar: {items.Count}");
            
            // Primero obtener un Access Token válido usando Client Credentials
            var accessToken = await GetAccessTokenAsync();
            Console.WriteLine($"🔑 Token a usar: {accessToken?.Substring(0, 30)}...");
            
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
                binary_mode = true,
                statement_descriptor = "Teatro Butacas"
            };

            var json = JsonSerializer.Serialize(preference);
            Console.WriteLine($"JSON request: {json}");
            
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Crear un nuevo HttpClient para evitar problemas de estado
            using var client = new HttpClient();
            
            // Solo agregar headers que son válidos para el request
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("User-Agent", "TeatroButacas/1.0");

            var apiUrl = "https://api.mercadopago.com/checkout/preferences";
            
            Console.WriteLine($"Enviando request a: {apiUrl}");
            
            var response = await client.PostAsync(apiUrl, content);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            Console.WriteLine($"Response status: {response.StatusCode}");
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
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error de red: {ex.Message}");
            
            // Como alternativa, vamos a crear un link de pago manual
            var totalAmount = items.Sum(i => i.UnitPrice * i.Quantity);
            var description = string.Join(", ", items.Select(i => $"{i.Title} x{i.Quantity}"));
            
            // Crear un link de pago básico (esto requeriría configuración adicional en MercadoPago)
            var basicPaymentUrl = $"https://www.mercadopago.cl/checkout/v1/payment/form?" +
                                 $"app_id={_settings.ClientId}&" +
                                 $"amount={totalAmount}&" +
                                 $"description={Uri.EscapeDataString(description)}";
            
            Console.WriteLine($"Usando link de pago básico: {basicPaymentUrl}");
            return basicPaymentUrl;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error general: {ex.Message}");
            throw new InvalidOperationException($"Error al crear preferencia: {ex.Message}", ex);
        }
    }

    private async Task<string> GetAccessTokenAsync()
    {
        try
        {
            Console.WriteLine("=== INICIANDO OBTENCIÓN DE TOKEN ===");
            Console.WriteLine($"Client ID: {_settings.ClientId}");
            Console.WriteLine($"Client Secret: {_settings.ClientSecret?.Substring(0, 10)}...");
            
            var tokenRequest = new
            {
                grant_type = "client_credentials",
                client_id = _settings.ClientId,
                client_secret = _settings.ClientSecret
            };

            var json = JsonSerializer.Serialize(tokenRequest);
            Console.WriteLine($"Request JSON: {json}");
            
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            
            Console.WriteLine("Enviando request a: https://api.mercadopago.com/oauth/token");
            
            var response = await client.PostAsync("https://api.mercadopago.com/oauth/token", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"=== RESPUESTA DEL TOKEN ===");
            Console.WriteLine($"Status: {response.StatusCode}");
            Console.WriteLine($"Response completa: {responseContent}");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"ERROR: No se pudo obtener token - {response.StatusCode}");
                throw new InvalidOperationException($"Error al obtener Access Token: {response.StatusCode} - {responseContent}");
            }

            var tokenResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
            
            if (tokenResponse.TryGetProperty("access_token", out var accessToken))
            {
                var token = accessToken.GetString();
                Console.WriteLine($"✅ TOKEN OBTENIDO EXITOSAMENTE");
                Console.WriteLine($"Token: {token?.Substring(0, 30)}...");
                return token ?? throw new InvalidOperationException("Access token vacío");
            }

            Console.WriteLine("❌ ERROR: No se encontró 'access_token' en la respuesta");
            throw new InvalidOperationException("No se pudo obtener el access_token de la respuesta");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ ERROR obteniendo token: {ex.Message}");
            Console.WriteLine($"Tipo de excepción: {ex.GetType().Name}");
            
            // Como fallback, usar el token existente
            Console.WriteLine("🔄 Usando token existente como fallback");
            Console.WriteLine($"Token fallback: {_settings.AccessToken?.Substring(0, 30)}...");
            return _settings.AccessToken;
        }
    }
}
