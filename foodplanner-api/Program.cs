using Dapper;
using foodplanner_api.Controller;
using foodplanner_api.Models;
using foodplanner_api.Services;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton(serviceProvider => {
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();

    var connectionString = configuration.GetConnectionString("DefaultConnection") ?? 
        throw new ApplicationException("the connection string is null");

    return new PostgreSQLConnectionFactory(connectionString);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapUserControllers();

app.MapGet("/test", () => "Testing sdhashaSCVHK!")
.WithName("GetTest")
.WithOpenApi();

// Configure the application to listen on all network interfaces
app.Urls.Add("http://0.0.0.0:80");

app.Run();
