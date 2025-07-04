using System.Text;
using System.Text.Json;

namespace Venta_de_butaca.Services;

public class MercadoPagoServerService : IMercadoPagoService
{
    private readonly HttpClient _httpClient;

    public MercadoPagoServerService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> CreatePreference(List<PaymentItem> items, string successUrl, string failureUrl, string pendingUrl)
    {
        try
        {
            var request = new
            {
                Items = items.Select(item => new
                {
                    Title = item.Title,
                    UnitPrice = item.UnitPrice,
                    Quantity = item.Quantity
                }).ToList(),
                SuccessUrl = successUrl,
                FailureUrl = failureUrl,
                PendingUrl = pendingUrl
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            Console.WriteLine($"Enviando request al servidor: {json}");

            var response = await _httpClient.PostAsync("api/payment/create-preference", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"Response del servidor: {response.StatusCode} - {responseContent}");

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Error del servidor: {response.StatusCode} - {responseContent}");
            }

            var result = JsonSerializer.Deserialize<JsonElement>(responseContent);
            
            if (result.TryGetProperty("initPoint", out var initPoint))
            {
                var initPointUrl = initPoint.GetString();
                Console.WriteLine($"Init point obtenido: {initPointUrl}");
                return initPointUrl ?? throw new InvalidOperationException("Init point vacío");
            }

            throw new InvalidOperationException($"Respuesta inválida del servidor: {responseContent}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en MercadoPagoServerService: {ex.Message}");
            throw new InvalidOperationException($"Error al crear preferencia: {ex.Message}", ex);
        }
    }
}
