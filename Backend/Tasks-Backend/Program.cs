using Microsoft.EntityFrameworkCore;
using Tasks_Backend.Data;
using Tasks_Backend.Services;

var builder = WebApplication.CreateBuilder(args);

// Serviços
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=tasks.db"));

builder.Services.AddScoped<UsuarioService>();

var app = builder.Build();

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware e endpoints
// app.UseHttpsRedirection(); 

app.MapControllers(); 
app.MapGet("/health", () => "API rodando...");

app.Run();