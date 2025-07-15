using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Logging;

var builder = WebApplication.CreateBuilder(args);

// Habilitar logging detallado de Identity Model (solo en desarrollo)
if (builder.Environment.IsDevelopment())
{
    IdentityModelEventSource.ShowPII = true;
}

// Configurar servicios
builder.Services.AddControllersWithViews();

// Configurar sesiones (necesario para el testing)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Configurar Microsoft Identity Web (nueva forma en .NET 9)
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(options =>
    {
        builder.Configuration.GetSection("AzureAd").Bind(options);
        // Configurar para preservar tokens
        options.SaveTokens = true;
        options.UseTokenLifetime = true;
        options.Events = new OpenIdConnectEvents
        {
            OnTokenValidated = context =>
            {
                // Asegurar que los tokens se guarden
                return Task.CompletedTask;
            }
        };
    });

// Configurar autorización
builder.Services.AddAuthorization(options =>
{
    // Política por defecto: usuario debe estar autenticado
    options.FallbackPolicy = options.DefaultPolicy;
});

// Configurar Razor Pages (necesario para Microsoft.Identity.Web)
builder.Services.AddRazorPages();

var app = builder.Build();

// Configurar pipeline de la aplicación
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // HSTS por defecto en .NET 9
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Configurar sesiones (debe ir después de UseRouting y antes de UseAuthentication)
app.UseSession();

// ⚠️ ORDEN CRÍTICO en .NET 9
app.UseAuthentication();
app.UseAuthorization();

// Configurar rutas
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Mapear Razor Pages (requerido por Microsoft.Identity.Web)
app.MapRazorPages();

app.Run();
