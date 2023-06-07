// run script from assets folder
// install nu:get packages:
// Microsoft.EntityFrameworkCore.Sqlite into Persistence.csproj (using nuget gallery)
// Microsoft.EntityFrameworkCore.Design (install into API)
// MediatR.Extensions.Microsoft.DependencyInjection by Jimmy Bogard (install into Application)
// AutoMapper.Extensions.Microsoft.DependencyInjection (into Application)


using API.Extensions;
using Microsoft.EntityFrameworkCore;
using Persistence;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CorsPolicy");

app.UseAuthorization();

app.MapControllers();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;


// the below updates (or creates new if not exisiting) database on each run
// and catches errors
try
{
    var context = services.GetRequiredService<DataContext>();
    await context.Database.MigrateAsync();
    await Seed.SeedData(context);
}
catch (Exception ex)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occured during migration");
}

app.Run();
