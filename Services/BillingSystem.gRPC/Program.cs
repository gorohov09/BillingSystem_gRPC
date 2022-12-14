using BillingSystem.DAL.Context;
using BillingSystem.gRPC.Services;
using BillingSystem.Interfaces;
using BillingSystem.Services.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddGrpc();

services.AddTransient<IDbInitializer, DbInitializer>();
services.AddDbContext<BillingSystemDB>(opt => 
    opt.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));
services.AddScoped<IBillingRepository, BillingRepository>();

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var db_initializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
    await db_initializer.InitializeAsync(false);
}

app.MapGrpcService<GreeterService>();
app.MapGrpcService<BillingService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
