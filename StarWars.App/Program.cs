using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using StarWars.App;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services
    .AddStarWarsClient()
    
    .ConfigureHttpClient(client =>
        client.BaseAddress = new Uri("http://localhost:5083/graphql"))
    
    .ConfigureWebSocketClient(client =>
        client.Uri = new Uri("ws://localhost:5083/graphql"));


await builder.Build().RunAsync();