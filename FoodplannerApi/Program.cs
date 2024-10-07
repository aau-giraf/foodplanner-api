using FoodplannerApi;
using Npgsql;
using FoodplannerDataAccessSql.Account;
using FoodplannerDataAccessSql;
using FoodplannerModels;
using FoodplannerModels.Account;
using FoodplannerModels.Images;
using FoodplannerServices.Account;
using FoodplannerServices.Image;
using Minio;


var builder = WebApplication.CreateBuilder(args);

//Add environment variables for Infisical and configure SecretsLoader
builder.Configuration.AddEnvironmentVariables(prefix: "INFISICAL_");
SecretsLoader.Configure(builder.Configuration, builder.Environment.EnvironmentName);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Configre and add MinIO service
var endpoint = SecretsLoader.GetSecret("MINIO_ENDPOINT");
var accessKey = SecretsLoader.GetSecret("MINIO_ACCESS");
var secretKey = SecretsLoader.GetSecret("MINIO_SECRET");
builder.Services.AddMinio(configureClient => 
    configureClient
        .WithEndpoint(endpoint)
        .WithCredentials(accessKey, secretKey)
        .WithTimeout(10000).WithSSL(false)
        .Build()
);

builder.Services.AddSingleton(serviceProvider => {
    var host = SecretsLoader.GetSecret("DB_HOST");
    var port = SecretsLoader.GetSecret("DB_PORT");
    var username = SecretsLoader.GetSecret("DB_USER");
    var password = SecretsLoader.GetSecret("DB_PASS");
    var database = SecretsLoader.GetSecret("DB_NAME");

    return new PostgreSQLConnectionFactory($"Host={host};Port={port};Username={username};Password={password};Database={database}");
});

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped(typeof(IUserRepository), typeof(UserRepository));

builder.Services.AddScoped<UserService>();
builder.Services.AddSingleton<IImageService, ImageService>();
builder.Services.AddAutoMapper(typeof(UserProfile));

builder.Services.AddControllers();

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

// New endpoint to test database connection
app.MapGet("/test-db-connection", async (PostgreSQLConnectionFactory connectionFactory) =>
    {
        
        var s = app.Services.GetRequiredService < IImageService>();
        var stream = new MemoryStream();
        await s.LoadImage(0, Guid.Parse("1a3462b5-de19-4fba-b17e-b93a0191e96b"), stream);
        Console.WriteLine(stream.Length);
        try
        {
            using (var connection = connectionFactory.Create())
            {
                await connection.OpenAsync();
                return Results.Ok("Database connection successful.");
            }
        }
        catch (NpgsqlException ex)
        {
            return Results.Problem($"Database connection failed: {ex.Message}");
        }
    })
    .WithName("TestDbConnection")
    .WithOpenApi();

// Configure the application to listen on all network interfaces
app.Urls.Add("http://0.0.0.0:80");

app.Run();