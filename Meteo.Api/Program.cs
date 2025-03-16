using Library.Identifiers;
using Meteo.Application;
using Meteo.Infrastructure;
using Meteo.Persistence.Json;
using Meteo.Persistence.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Set base path to /app and load configuration files
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

bool usePostgreSqlDb = builder.Configuration.GetValue<bool>("DatabaseSettings:RunPostgreSqlDb");
bool useJsonDb = builder.Configuration.GetValue<bool>("DatabaseSettings:RunJsonDb");

// Add services to the container
builder.Services
    .AddSingleton<TimeProvider>(TimeProvider.System)
    .AddDefaultIdentifierProvider();

builder.Services.AddOpenApi();
builder.Services.AddApplication(builder.Configuration, builder.Environment);
builder.Services.AddInfrastructure(builder.Configuration, builder.Environment);
if(useJsonDb) builder.Services.AddJsonPersistence(builder.Configuration, builder.Environment);
if(usePostgreSqlDb) builder.Services.AddPostgreSqlPersistence(builder.Configuration, builder.Environment);
builder.Services.AddControllers();

WebApplication app = builder.Build();

bool updatePostgreSqlDb = builder.Configuration.GetValue<bool>("DatabaseSettings:UpdatePostgreSqlDb");

if (updatePostgreSqlDb && usePostgreSqlDb)
{
    await using (var scope = app.Services.CreateAsyncScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            await context.Database.MigrateAsync(cancellationToken: app.Lifetime.ApplicationStarted);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while applying migrations: {ex.Message}");
            throw;
        }
    }
}


app.MapOpenApi();
app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("My API")
            .WithDefaultHttpClient(ScalarTarget.JavaScript, ScalarClient.Axios)
            .AddServer(new ScalarServer("http://localhost:5001", "Docker Dev"));
    });

app.UseHttpsRedirection();

// Map weather API group
app.MapGroup("/api/weather")
   .AddWeatherDefinitionsApi();

app.UseRouting();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();
