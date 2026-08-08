using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// builder.Services.AddOpenApi(); // OpenApi nativo do .NET 10 (ainda básico)
// Vamos usar o Swagger tradicional para facilitar testes:
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        policy =>
        {
            policy.WithOrigins(
                "http://localhost:5173",
                "https://www.dokidokitranslationclub.com.br",
                "https://ddtc.squareweb.app",
                "https://thaleskaua66.github.io"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
});

builder.Services.AddDbContext<DdtcApi.Data.AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}


app.UseCors("AllowSpecificOrigins");

app.UseAuthorization();

app.MapControllers();

// Garante que o banco foi criado e cria o admin inicial se não existir
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DdtcApi.Data.AppDbContext>();
    db.Database.EnsureCreated(); // Cria o banco se não existir

    if (!db.Admins.Any())
    {
        db.Admins.Add(new DdtcApi.Models.Admin 
        { 
            Name = "admin", 
            Key = "sua_senha_secreta" // Você pode mudar isso depois
        });
        db.SaveChanges();
    }
}

app.Run();
