using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using UrlShortener.Api.ExceptionHandling;
using UrlShortener.Api.Extensions;
using UrlShortener.Infra.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
            // Se usar cookies ou autenticação, precisa ser:
            .WithOrigins("http://localhost:5106")
            .AllowCredentials()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    {
        var conn = builder.Configuration.GetConnectionString("DefaultConnection");
        if (!string.IsNullOrWhiteSpace(conn)) options.UseMySql(conn, ServerVersion.AutoDetect(conn));
    }
);

builder.Services.AddHttpContextAccessor();

// AI (Gemini) - API key vem de configuration (User Secrets em Development): Gemini:ApiKey
builder.AddAiContext();

// Registrar Mediator (descoberta automática de handlers)
// Handlers devem ser `Scoped` (ou `Transient`) quando dependem de serviços Scoped
builder.Services.AddMediator(options => { options.ServiceLifetime = ServiceLifetime.Scoped; });

// Swagger configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "URL shortener API",
        Version = "v1"
    });

    // Security Definition
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Digite apenas seu token JWT o prefixo 'Bearer' já está embutido"
    });

    // Security Requirement
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


builder.AddUserContext();
builder.AddUrlContext();

var app = builder.Build();

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseCors("AllowSwagger");
app.UseCors("AllowAll");
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Scalar UI
app.MapScalarApiReference(options =>
{
    options
        .WithTitle("Agent API")
        .WithTheme(ScalarTheme.Default)
        .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI();
    app.UseSwagger();
}

app.MapControllers();
// Check database connectivity on startup and attempt migrate in Development
try
{
    using var scope = app.Services.CreateScope();
    var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");

    var conn = config.GetConnectionString("DefaultConnection");
    if (string.IsNullOrWhiteSpace(conn))
        logger.LogWarning(
            "No DefaultConnection connection string found. If you expect a database, set ConnectionStrings:DefaultConnection in appsettings.json or environment variables.");
    else
        try
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            // Try to connect to database
            if (!db.Database.CanConnect())
            {
                logger.LogError(
                    "Unable to connect to the database using DefaultConnection. Check that the database server is running and the connection string is correct: {Conn}",
                    conn);
            }
            else
            {
                logger.LogInformation("Database connection established.");
                // Apply migrations in Development environment
                var env = scope.ServiceProvider.GetRequiredService<IHostEnvironment>();
                if (env.IsDevelopment())
                {
                    logger.LogInformation("Applying migrations (Development)...");
                    db.Database.Migrate();
                }
            }
        }
        catch (Exception ex)
        {
            var logger2 = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            logger2.LogError(ex, "Error while checking or migrating the database.");
        }
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "Error during startup checks.");
}

app.Run();