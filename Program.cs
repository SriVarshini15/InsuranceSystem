
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using VehicleInsuranceSystem.Data;
using VehicleInsuranceSystem.Services.Mapping;
using VehicleInsuranceSystem.Interfaces;
using VehicleInsuranceSystem.Services.Auth;
using VehicleInsuranceSystem.Services.Users;
using VehicleInsuranceSystem.Services.Vehicles;
using VehicleInsuranceSystem.Services.Policies;
using VehicleInsuranceSystem.Services.Proposals;
using VehicleInsuranceSystem.Services.Payments;
using VehicleInsuranceSystem.Services.Claims;
using VehicleInsuranceSystem.Services.Notifications;
using VehicleInsuranceSystem.Middleware;

namespace VehicleInsuranceSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter your JWT token."
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();

            var jwtSettings = builder.Configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings.GetValue<string>("SecretKey") ?? "DefaultSecretKey123456789012345!";

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.GetValue<string>("Issuer") ?? "VehicleInsuranceSystem",
                    ValidAudience = jwtSettings.GetValue<string>("Audience") ?? "VehicleInsuranceApi",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    ClockSkew = TimeSpan.Zero
                };
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
                options.AddPolicy("OfficerOnly", policy => policy.RequireRole("Officer", "Admin"));
                options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
                options.AddPolicy("UserOrOfficer", policy => policy.RequireRole("User", "Officer", "Admin"));
            });

            
            builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IVehicleService, VehicleService>();
            builder.Services.AddScoped<IPolicyService, PolicyService>();
            builder.Services.AddScoped<IProposalService, ProposalService>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddScoped<IClaimService, ClaimService>();
            builder.Services.AddScoped<INotificationService, NotificationService>();

            // 1. Extract the connection string from appsettings.json
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            // 2. Register your ApplicationDbContext with SQL Server
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseHttpsRedirection();

            app.UseExceptionHandler();

            app.Use(async (context, next) =>
            {
                context.Response.OnStarting(() =>
                {
                    if (context.Request.Path.StartsWithSegments("/api/v1"))
                    {
                        context.Response.Headers.TryAdd("X-Api-Version", "1.0");
                    }

                    return Task.CompletedTask;
                });

                await next();
            });

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
