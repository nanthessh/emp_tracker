using Blazored.LocalStorage;
using EmpTracker.Web.Components;
using EmpTracker.Web.Services;
using Microsoft.AspNetCore.Components.Server;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.Configure<CircuitOptions>(o => o.DetailedErrors = true);

builder.Services.AddBlazoredLocalStorage();

var apiBase = builder.Configuration["ApiBaseUrl"]!;
builder.Services.AddHttpClient<AuthService>(c => c.BaseAddress = new Uri(apiBase));
builder.Services.AddHttpClient<TaskApiService>(c => c.BaseAddress = new Uri(apiBase));
builder.Services.AddHttpClient("api", c => c.BaseAddress = new Uri(apiBase));
builder.Services.AddScoped(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    return factory.CreateClient("api");
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
