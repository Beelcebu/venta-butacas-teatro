using MercadoPago.Client.Preference;
using MercadoPago.Config;
using MercadoPago.Resource.Preference;
using System.Net.Http;

namespace Venta_de_butaca.Services;

public class MercadoPagoService : IMercadoPagoService, IDisposable
{
    private readonly MercadoPagoSettings _settings;
    private readonly HttpClient _httpClient;

    public MercadoPagoService(MercadoPagoSettings settings)
    {
        try
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));

            if (string.IsNullOrEmpty(_settings.AccessToken))
                throw new InvalidOperationException("MercadoPago AccessToken no está configurado");

            Console.WriteLine($"Inicializando MercadoPago con AccessToken: {_settings.AccessToken.Substring(0, 10)}...");
            
            _httpClient = new HttpClient();
            MercadoPagoConfig.AccessToken = _settings.AccessToken;
            
            Console.WriteLine("MercadoPago inicializado correctamente");
        }
        catch (Exception ex)
        {
            var innerMessage = ex.InnerException?.Message ?? "No inner exception";
            Console.WriteLine($"Error en constructor MercadoPagoService: {ex.Message} | Inner: {innerMessage}");
            throw new InvalidOperationException($"Error al inicializar MercadoPago: {ex.Message} | Inner: {innerMessage}", ex);
        }
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }

    public async Task<string> CreatePreference(List<PaymentItem> items, string successUrl, string failureUrl, string pendingUrl)
    {
        if (!items.Any())
            throw new ArgumentException("La lista de items no puede estar vacía", nameof(items));

        try
        {
            Console.WriteLine("Iniciando creación de preferencia...");
            Console.WriteLine($"Items: {items.Count}");
            Console.WriteLine($"Success URL: {successUrl}");
            
            var client = new PreferenceClient();
            Console.WriteLine("Cliente PreferenceClient creado");

            var preference = new PreferenceRequest
            {
                Items = items.Select(item => new PreferenceItemRequest
                {
                    Title = item.Title,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    CurrencyId = "CLP"
                }).ToList(),
                BackUrls = new PreferenceBackUrlsRequest
                {
                    Success = new Uri(successUrl).AbsoluteUri,
                    Failure = new Uri(failureUrl).AbsoluteUri,
                    Pending = new Uri(pendingUrl).AbsoluteUri
                },
                AutoReturn = "approved",
                PaymentMethods = new PreferencePaymentMethodsRequest
                {
                    Installments = 1,
                    ExcludedPaymentMethods = new List<PreferencePaymentMethodRequest>()
                },
                BinaryMode = true
            };

            Console.WriteLine("Objeto PreferenceRequest creado, llamando a CreateAsync...");
            var preferenceResult = await client.CreateAsync(preference);
            Console.WriteLine($"Preferencia creada exitosamente. InitPoint: {preferenceResult.InitPoint}");
            return preferenceResult.InitPoint;        }
        catch (Exception ex)
        {
            var innerMessage = ex.InnerException?.Message ?? "No inner exception";
            var stackTrace = ex.StackTrace ?? "No stack trace";
            var fullMessage = $"Error: {ex.Message} | Inner: {innerMessage} | Stack: {stackTrace}";
            throw new InvalidOperationException($"Error al crear la preferencia de pago: {fullMessage}", ex);
        }
    }
}
