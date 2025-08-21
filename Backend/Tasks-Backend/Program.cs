using Microsoft.EntityFrameworkCore;
using Tasks_Backend.Data;
using Tasks_Backend.Services;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;


var key = "4WTH-51ZiC_ra1gU27q2_Tw61YdzxNiloZh5UHfRN82NcE-1wUThESXc7iWasGvPyYrjswNRWEn3PwAuwuv0dA";
var builder = WebApplication.CreateBuilder(args);

// Serviços
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Tasks API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Scheme = "Bearer",
        Description = "Informe o token JWT: Bearer token"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=tasks.db"));

builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<TarefaService>();

// JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
    };

    options.Events = new JwtBearerEvents
    {
        // Rejeita Token sem prefixo se UI não for o Swagger 
        OnMessageReceived = context =>
        {
            var token = context.Request.Headers["Authorization"].ToString();

            if (!token.StartsWith("Bearer "))
            {
                Console.WriteLine("Token sem prefixo 'Bearer' rejeitado.");
                context.NoResult(); 
            }

            return Task.CompletedTask;
        },

        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"Erro de autenticação: {context.Exception.Message}");
            return Task.CompletedTask;
        }
    };
});

var app = builder.Build();

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

// Middleware e endpoints
// app.UseHttpsRedirection(); 

app.MapControllers(); 
app.MapGet("/Health", () => "API rodando...");

app.Run();