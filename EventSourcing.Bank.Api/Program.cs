using EventSourcing.Bank.Application.Services;
using EventSourcing.Bank.Application.Abstractions;
using EventSourcing.Bank.Infrastructure.Repositories;
using EventSourcing.Bank.Application.CQRS.Queries.Account;
using Microsoft.EntityFrameworkCore;
using EventSourcing.Bank.Application.CQRS.Commands.Account.Handlers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Configure CORS for Angular dev server
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDevServer", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var conn = builder.Configuration.GetConnectionString("Events") ?? builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<EventSourcing.Bank.Infrastructure.Persistence.EventStoreDbContext>(options =>
    options.UseSqlServer(conn));

// Infrastructure
builder.Services.AddScoped<EventSourcing.Bank.Infrastructure.Persistence.ReadModels.AccountProjection>();
builder.Services.AddScoped<IEventStore, EventSourcing.Bank.Infrastructure.Persistence.SqlServerEventStore>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();

// Domain & Application Services
builder.Services.AddScoped<EventSourcing.Bank.Domain.Services.FundsTransferDomainService>();

// Add MediatR
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(CreateAccountCommand).Assembly);
});

var app = builder.Build();

// Apply EF Core migrations at startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<EventSourcing.Bank.Infrastructure.Persistence.EventStoreDbContext>();
    // WIPE DB FOR LEARNING PURPOSES (Since Value Objects broke JSON schemas)
    db.Database.EnsureDeleted();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Enable CORS for requests from the Angular dev server
app.UseCors("AllowAngularDevServer");

app.UseAuthorization();

app.MapControllers();

app.Run();
