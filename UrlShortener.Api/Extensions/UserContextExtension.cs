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

        builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

        builder.Services.AddScoped<IAuthService, JwtService>();

        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

        builder.Services.AddScoped<IVerifyUserService, EmailService>();
        builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
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
                        Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"]!)
                    )
                };
            }
        );
    }
}