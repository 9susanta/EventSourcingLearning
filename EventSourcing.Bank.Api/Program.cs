using EventSourcing.Bank.Api.Services;
using EventSourcing.Bank.Api.Store;

// Event-sourcing style: Program startup ka kaam hai services aur middleware wire-up karna.
// Role & responsibility:
// - Configure DI aur register core infrastructure (jaise EventStore)
// - Register controllers and OpenAPI for API surface
// - Build aur run the application (serve HTTP requests)
var builder = WebApplication.CreateBuilder(args);

// Services add kar raha hai
// Controllers ko register karta hai taaki controller-based endpoints kaam karein
builder.Services.AddControllers();
// Register OpenAPI/Swagger services (docs + UI)
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// InMemoryEventStore ko singleton ke roop mein register kar raha hai
// Role: EventStore ko DI container mein register karna. Event-sourcing flow mein
// responsibility yehi hai ke ek shared store available ho jahan aggregates apne events save/LOAD kar sakein.
builder.Services.AddSingleton<InMemoryEventStore>();
builder.Services.AddSingleton<AccountService>();

// WebApplication build karta hai — DI finalize aur middleware pipeline ready
var app = builder.Build();

// HTTP request pipeline configure karte hain
if (app.Environment.IsDevelopment())
{
    // Development mode mein Swagger/OpenAPI UI enable kar deta hai
    app.MapOpenApi();
}

// HTTP ko HTTPS par redirect karta hai
app.UseHttpsRedirection();

// Authorization middleware add karta hai (policies alag se configure karne honge)
app.UseAuthorization();

// Controller routes ko endpoints se map karta hai
app.MapControllers();

// App run karke requests sunna start karta hai
app.Run();
