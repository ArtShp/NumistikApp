using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Server.Data;
using Server.Entities;
using Server.Services;
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
                      .MapEnum<CollectionItemType>()
                      .MapEnum<CollectionItemStatus>()
                      .MapEnum<CollectionItemQuality>()
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

        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<CollectionService>();
        builder.Services.AddScoped<ContinentService>();
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
