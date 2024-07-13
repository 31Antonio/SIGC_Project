using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SIGC_PROJECT.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

// Add services to the container.
builder.Services.AddControllersWithViews();

// Conexión a la Base de Datos
builder.Services.AddDbContext<SigcProjectContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("conexion")));

builder.Services.AddAuthentication(options =>
{
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie(options => {
    options.LoginPath = "/Login/Login";
    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.Redirect("/Login/Login");
        return Task.CompletedTask;
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    var user = context.User;

    //Si el usuario esta autenticado
    if (user.Identity.IsAuthenticated)
    {
        if (context.Request.Path.StartsWithSegments("/") || context.Request.Path.StartsWithSegments("/PrincipalPage"))
        {
            context.Response.Redirect("/Home/Index");
            return;
        }
    }
    //Si el usuario no esta autenticado
    else
    {
        if (context.Request.Path.StartsWithSegments("/Home"))
        {
            context.Response.Redirect("/PrincipalPage/Index");
            return;
        }
    }

    await next();
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=PrincipalPage}/{action=Index}/{id?}");

app.Run();
