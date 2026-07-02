using EventSourcing.Bank.Application.Services;
using EventSourcing.Bank.Application.Abstractions;
using EventSourcing.Bank.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var conn = builder.Configuration.GetConnectionString("Events") ?? builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<EventSourcing.Bank.Infrastructure.Persistence.EventStoreDbContext>(options =>
    options.UseSqlServer(conn));

builder.Services.AddScoped<IEventStore, EventSourcing.Bank.Infrastructure.Persistence.SqlServerEventStore>();
builder.Services.AddScoped<IAccountService, EventSourcing.Bank.Application.Services.AccountService>();

// Force a known URL so the Angular proxy can target the API
builder.WebHost.UseUrls("http://localhost:5000");

var app = builder.Build();

// Angular dev server startup removed: run the Angular dev server manually from ClientApp when needed.

// Apply EF Core migrations at startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<EventSourcing.Bank.Infrastructure.Persistence.EventStoreDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
