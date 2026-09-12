using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using SistemaTramites.Application.Services;
using SistemaTramites.Infrastructure.Data;
using SistemaTramites.Infrastructure.Identity;

var builder = WebApplication.CreateBuilder(args);

// Capa de datos: SQLite vía Entity Framework Core.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=tramites.db";
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

// Autenticación y roles (ASP.NET Core Identity) sobre la misma base SQLite.
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 8;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
});

// Capa de aplicación: lógica de negocio (registrar solicitud, registrar pago).
builder.Services.AddScoped<ITramiteService, TramiteService>();
builder.Services.AddScoped<IPagoService, PagoService>();

// Capa de presentación: MVC con vistas Razor. Se exige sesión iniciada en
// todos los controladores salvo los marcados explícitamente con [AllowAnonymous].
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AuthorizeFilter());
});

var app = builder.Build();

// Crear la base de datos SQLite, cargar datos de ejemplo, roles y usuarios de acceso.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    DbInitializer.Seed(db);

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    await IdentityDataInitializer.SeedAsync(roleManager, userManager, db);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
