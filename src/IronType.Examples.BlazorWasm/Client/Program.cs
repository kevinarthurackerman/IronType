var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddSharedServices();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7032/") });

await builder.Build().RunAsync();
