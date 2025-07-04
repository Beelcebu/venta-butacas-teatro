namespace Venta_de_butaca.Services;

public class MercadoPagoSettings
{
    public string AccessToken { get; set; } = string.Empty;
    public string PublicKey { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
}

public class PaymentItem
{
    public string Title { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
}

public interface IMercadoPagoService
{
    Task<string> CreatePreference(List<PaymentItem> items, string successUrl, string failureUrl, string pendingUrl);
}
