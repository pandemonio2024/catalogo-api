using Catalogo.Api.Middleware;
using Catalogo.Aplicacion;
using Catalogo.Infraestructura;
using Catalogo.Infraestructura.Persistencia;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AgregarAplicacion();
builder.Services.AgregarInfraestructura(builder.Configuration);

var app = builder.Build();

// Aplica las migraciones al arrancar: crea la base y las tablas si no existen.
using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<CatalogoDbContext>().Database.Migrate();
}

app.UseMiddleware<ManejoErroresMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();

public partial class Program { }
