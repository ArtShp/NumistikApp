using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Server.Data;
using Server.Services;
using Shared.Models.Common;
using System.Text;

namespace Server;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        builder.Services.AddDbContext<MyDbContext>(options =>
            options.UseNpgsql(
                builder.Configuration.GetConnectionString("Postgres"), 
                o => o.MapEnum<UserAppRole>()
                      .MapEnum<CollectionRole>()
            )
        );

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["AppSettings:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["AppSettings:Audience"],
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:Token"]!)),
                    ValidateIssuerSigningKey = true
                };
            });

        builder.WebHost.ConfigureKestrel(options =>
        {
            var certPath = builder.Configuration["CertPath"];
            var certPassword = builder.Configuration["CertPassword"];

            if (!string.IsNullOrEmpty(certPath) && !string.IsNullOrEmpty(certPassword) && File.Exists(certPath))
            {
                options.ListenAnyIP(443, listenOptions =>
                {
                    listenOptions.UseHttps(certPath, certPassword);
                });

                Console.WriteLine($"[INFO] HTTPS enabled on port 443 using provided certificate.");
            }
            else
            {
                options.ListenLocalhost(5000, listenOptions =>
                {
                    listenOptions.UseHttps();
                });

                Console.WriteLine("[WARN] No certificate found. Running HTTPS on localhost:5000 (development only).");
            }
        });

        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<CollectionService>();
        builder.Services.AddScoped<ContinentService>();
        builder.Services.AddScoped<CollectionItemTypeService>();
        builder.Services.AddScoped<CollectionItemStatusService>();
        builder.Services.AddScoped<CollectionItemQualityService>();
        builder.Services.AddScoped<CollectionItemSpecialStatusService>();
        builder.Services.AddScoped<CountryService>();
        builder.Services.AddScoped<CollectionItemService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(
                Path.Combine(builder.Environment.ContentRootPath, "static")),
            RequestPath = "/static"
        });

        app.MapControllers();

        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<MyDbContext>();
            DataSeeder.SeedAll(db);
        }

        app.Run();
    }
}
