using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using SistemaProyectos.Data;
using SistemaProyectos.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});



builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", config =>
    {
        config.LoginPath = "/Cuenta/Login";

        config.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        config.SlidingExpiration = false;
        config.Cookie.IsEssential = true;
    });

builder.Services.AddSingleton<AcumuladorTareas>();

builder.Services.AddControllersWithViews();


var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//FRAGMENTO DE CÓDIGO DE Program.cs
// Migrar contraseñas en texto plano a hash
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var passwordHasher = new PasswordHasher<Usuario>();

    var usuarios = context.Usuario.ToList();

    bool cambios = false;

    foreach (var u in usuarios)
    {
        // Si no parece un hash de ASP.NET Identity, lo migramos
        if (!u.Contrasena.StartsWith("AQAAAA"))
        {
            u.Contrasena = passwordHasher.HashPassword(u, u.Contrasena);
            cambios = true;
        }
    }

    if (cambios)
    {
        context.SaveChanges();
        Console.WriteLine("Migración de contraseñas completada.");
    }
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Cuenta}/{action=Login}/{id?}");

app.Run();
