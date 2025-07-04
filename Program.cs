using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Venta_de_butaca;
using Venta_de_butaca.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure HTTP client
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Configure MercadoPago with hardcoded settings for now
var mercadoPagoSettings = new MercadoPagoSettings
{
    AccessToken = "APP_USR-6779823633990825-070417-c79a55a850588511f3655b03f37083a1-2533320339",
    PublicKey = "APP_USR-1d08dc45-fcdc-4503-8a43-58c20e2be1d9",
    ClientId = "6779823633990825",
    ClientSecret = "DkBwBDHwslmq5bAQuyFokafASk18gJSQ"
};

builder.Services.AddSingleton(mercadoPagoSettings);
builder.Services.AddScoped<IMercadoPagoService, MercadoPagoDirectService>();

await builder.Build().RunAsync();
