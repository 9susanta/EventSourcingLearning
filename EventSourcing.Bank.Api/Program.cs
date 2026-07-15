using EventSourcing.Bank.Application.Services;
using EventSourcing.Bank.Application.Abstractions;
using EventSourcing.Bank.Infrastructure.Repositories;
using EventSourcing.Bank.Infrastructure.CQRS;
using EventSourcing.Bank.Application.CQRS.Commands;
using EventSourcing.Bank.Application.CQRS.Commands.Account.Handlers;
using EventSourcing.Bank.Application.CQRS.Queries;
using EventSourcing.Bank.Application.CQRS.Queries.Account.Handlers;
using Microsoft.EntityFrameworkCore;
using EventSourcing.Bank.Application.CQRS.Queries.Account;

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
builder.Services.AddScoped<EventSourcing.Bank.Application.Services.DomainEventDispatcher>(sp => 
{
    var handlers = sp.GetServices<object>().Where(s => s.GetType().GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(EventSourcing.Bank.Application.Services.IDomainEventHandler<>)));
    return new EventSourcing.Bank.Application.Services.DomainEventDispatcher(handlers);
});
builder.Services.AddScoped<object, EventSourcing.Bank.Application.Services.AccountOverdrawnSmsHandler>(); // Register as object for the dispatcher to find

// CQRS Dispatchers
builder.Services.AddScoped<CommandDispatcher>();
builder.Services.AddScoped<QueryDispatcher>();

// Command Handlers
builder.Services.AddScoped<ICommandHandler<CreateAccountCommand, EventSourcing.Bank.Domain.Aggregates.AccountAggregate>, CreateAccountCommandHandler>();
builder.Services.AddScoped<ICommandHandler<DepositCommand, EventSourcing.Bank.Domain.Aggregates.AccountAggregate>, DepositCommandHandler>();
builder.Services.AddScoped<ICommandHandler<WithdrawCommand, EventSourcing.Bank.Domain.Aggregates.AccountAggregate>, WithdrawCommandHandler>();
builder.Services.AddScoped<ICommandHandler<EventSourcing.Bank.Application.CQRS.Commands.Account.Handlers.TransferCommand, bool>, EventSourcing.Bank.Application.CQRS.Commands.Account.Handlers.TransferCommandHandler>();

// Query Handlers
builder.Services.AddScoped<IQueryHandler<GetAccountQuery, EventSourcing.Bank.Application.DTOs.AccountResponse>, GetAccountQueryHandler>();
builder.Services.AddScoped<IQueryHandler<GetAccountHistoryQuery, IEnumerable<EventSourcing.Bank.Application.DTOs.EventDto>>, GetAccountHistoryQueryHandler>();

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
