using App_Azure_OpenId.Models;
using App_Azure_OpenId.Services;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<NotesDbContext>((options) =>
    options.UseMySql(builder.Configuration.GetConnectionString("MySqlConnection"),
    new MySqlServerVersion(new Version(8, 0, 25))));

// Agregar configuración de servicios.

builder.Services.Configure<CookiePolicyOptions>(options =>
{
	// This lambda determines whether user consent for non-essential cookies is needed for a given request.
	options.CheckConsentNeeded = context => true;
	options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
	// Handling SameSite cookie according to https://learn.microsoft.com/aspnet/core/security/samesite?view=aspnetcore-3.1
	options.HandleSameSiteCookieCompatibility();
});

// Configuration to sign-in users with Azure AD B2C
builder.Services.AddMicrosoftIdentityWebAppAuthentication(builder.Configuration, "AzureAdB2C");

builder.Services.AddControllersWithViews()
	.AddMicrosoftIdentityUI();

builder.Services.AddRazorPages();

//Configuring appsettings section AzureAdB2C, into IOptions
builder.Services.AddOptions();
builder.Services.Configure<OpenIdConnectOptions>(builder.Configuration.GetSection("AzureAdB2C"));

builder.Services.AddSingleton<AzureB2CUserManagement>(sp =>
    new AzureB2CUserManagement(
        builder.Configuration["AzureAdB2C:ClientId"],
        builder.Configuration["AzureAdB2C:ClientSecret"],
        builder.Configuration["AzureAdB2C:TenantId"],
        builder.Configuration["AzureAdB2C:Domain"] // ejemplo: "tudominio.onmicrosoft.com"
    ));

builder.Services.AddSingleton<GraphUserService>(sp =>
	new GraphUserService(
		builder.Configuration["AzureAdB2C:ClientId"],
		builder.Configuration["AzureAdB2C:ClientSecret"],
		builder.Configuration["AzureAdB2C:TenantId"],
        builder.Configuration["AzureAdB2C:Domain"]
    ));

var app = builder.Build();

// Configuración del pipeline de la aplicación.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // Configuración para producción.
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCookiePolicy(); // Asegura que la política de cookies se aplique.
app.UseAuthentication(); // Middleware de autenticación.
app.UseAuthorization(); // Middleware de autorización.

app.UseRouting()
    .UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
    endpoints.MapRazorPages(); // Habilita las Razor Pages.
});

// Ejecutar la aplicación.
app.Run();
