using Microsoft.EntityFrameworkCore;
using System;
using NewDawn.Models;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace NewDawn
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<NewDawnContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddControllersWithViews();
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Usuarios/Login"; // Ruta al login
                    options.AccessDeniedPath = "/Usuarios/AccessDenied"; // Ruta de acceso denegado
                });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("CanAccessHabitaciones", policy =>
                    policy.RequireClaim("Permission","AccesoTotal"));
            });
            // 🔹 Agregar soporte para sesiones
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Expira en 30 minutos
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
           

            app.UseHttpsRedirection();
            app.UseRouting();

            // 🔹 Activar middleware de sesión
            app.UseSession();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "recuperar",
                pattern: "Usuarios/Recuperar",
                defaults: new { controller = "Usuarios", action = "Recuperar" });

            app.MapControllerRoute(
                name: "restablecer",
                pattern: "Usuarios/Restablecer",
                defaults: new { controller = "Usuarios", action = "Restablecer" });
            builder.Services.AddSession();
            app.UseSession();


            app.Run();
        }
    }
}
