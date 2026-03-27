using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using UrlShortener.Domain.Config;
using UrlShortener.Domain.IRepositories;
using UrlShortener.Domain.IServices;
using UrlShortener.Infra.Repositories;
using UrlShortener.Infra.Services;

namespace UrlShortener.Api.Extensions;

public static class UserContextExtension
{
    public static void AddUserContext(this WebApplicationBuilder builder)
    {
        // Adiciona configuração de envio de email
        // Adicione via user secrets:
        // dotnet user-secrets set "EmailSettings:SmtpUsername" "seuemail@email.com"
        // dotnet user-secrets set "EmailSettings:SmtpPassword" "SuaSenhaRealAqui"

        builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

        var jwtSection = builder.Configuration.GetSection(JwtSettings.SectionName);
        var jwtSecret = jwtSection["Secret"];

        // Support local runs where developers set JWT_SECRET in shell/.env tooling.
        if (string.IsNullOrWhiteSpace(jwtSecret)) jwtSecret = builder.Configuration["JWT_SECRET"];

        builder.Services
            .AddOptions<JwtSettings>()
            .Bind(jwtSection)
            .PostConfigure(options =>
            {
                if (string.IsNullOrWhiteSpace(options.Secret) && !string.IsNullOrWhiteSpace(jwtSecret))
                    options.Secret = jwtSecret;
            })
            .Validate(options => !string.IsNullOrWhiteSpace(options.Secret),
                "JWT secret is required. Configure JwtSettings:Secret or environment variable JWT_SECRET.")
            .Validate(options => options.Secret.Length >= 32,
                "JWT secret must be at least 32 characters long.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.Issuer), "JwtSettings:Issuer is required.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.Audience), "JwtSettings:Audience is required.")
            .ValidateOnStart();

        builder.Services.AddScoped<IAuthService, JwtService>();

        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

        builder.Services.AddScoped<IVerifyUserService, EmailService>();
        builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
        if (string.IsNullOrWhiteSpace(jwtSecret))
            throw new InvalidOperationException(
                "JWT secret is not configured. Set JwtSettings:Secret (or JwtSettings__Secret) or JWT_SECRET.");

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                    ValidAudience = builder.Configuration["JwtSettings:Audience"],

                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSecret)
                    )
                };
            }
        );
    }
}