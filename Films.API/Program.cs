using Scalar.AspNetCore;
using Films.Application;
using Films.Application.Database;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

//register services
builder.Services.AddControllers();
builder.Services.AddApplicationServices();
builder.Services.AddDatabase(config["Database:FilmsConnectionString"]!);
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

//start database
var dbInitializer = app.Services.GetRequiredService<DbInitializer>();
await dbInitializer.InitializeAsync();
app.Run();