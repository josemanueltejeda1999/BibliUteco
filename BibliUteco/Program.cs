using BibliUteco.Areas.Identity;
using BibliUteco.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Registrar ApplicationDbContext usado por Identity
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Usar AddIdentity para soportar roles correctamente y la UI por defecto si se necesita.
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();

// Registrar DbContext principal de la aplicación

 


// Registrar servicios de la aplicación
builder.Services.AddScoped<BibliUteco.Services.Interfaces.IAutorService, BibliUteco.Services.AutorService>();
builder.Services.AddScoped<BibliUteco.Services.Interfaces.ICategoriaService, BibliUteco.Services.CategoriaService>();
builder.Services.AddScoped<BibliUteco.Services.Interfaces.ILibroService, BibliUteco.Services.LibroService>();
builder.Services.AddScoped<BibliUteco.Services.Interfaces.IEstudianteService, BibliUteco.Services.EstudianteService>();
builder.Services.AddScoped<BibliUteco.Services.Interfaces.IPrestamoService, BibliUteco.Services.PrestamoService>();
builder.Services.AddScoped<BibliUteco.Services.Interfaces.IDashboardService, BibliUteco.Services.DashboardService>();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();
builder.Services.AddSingleton<WeatherForecastService>();

var app = builder.Build();
// Inicializar roles y usuario administrador
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Asegúrate de que DbInitializer.Initialize tenga la firma async Task Initialize(IServiceProvider services)
        await BibliUteco.Data.DbInitializer.Initialize(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocurrió un error al crear los roles");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

// Agregar autenticación antes de autorización
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
