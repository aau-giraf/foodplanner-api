using FoodplannerApi;
using Npgsql;
using FoodplannerDataAccessSql;
using FoodplannerDataAccessSql.Account;
using FoodplannerDataAccessSql.Lunchbox;
using FoodplannerModels;
using FoodplannerModels.Account;
using FoodplannerServices.Account;
using FoodplannerModels.Lunchbox;
using FoodplannerServices.Lunchbox;

var builder = WebApplication.CreateBuilder(args);

//Add environment variables for Infisical and configure SecretsLoader
builder.Configuration.AddEnvironmentVariables(prefix: "INFISICAL_");
SecretsLoader.Configure(builder.Configuration, builder.Environment.EnvironmentName);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



builder.Services.AddSingleton(serviceProvider => {
    var host = SecretsLoader.GetSecret("DB_HOST", "/SW-5-10/");
    var port = SecretsLoader.GetSecret("DB_PORT", "/SW-5-10/");
    var database = SecretsLoader.GetSecret("DB_NAME", "/SW-5-10/");
    var username = SecretsLoader.GetSecret("DB_USER", "/SW-5-10/");
    var password = SecretsLoader.GetSecret("DB_PASS", "/SW-5-10/");

    return new PostgreSQLConnectionFactory(host, port, database, username, password);
});

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped(typeof(IUserRepository), typeof(UserRepository));
builder.Services.AddScoped(typeof(IMealRepository), typeof(MealRepository));
builder.Services.AddScoped(typeof(IIngredientRepository), typeof(IngredientRepository));

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<MealService>();
builder.Services.AddScoped<IngredientService>();
builder.Services.AddAutoMapper(typeof(UserProfile));
builder.Services.AddAutoMapper(typeof(MealProfile));

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
app.MapGet("/test-db-connection", async (PostgreSQLConnectionFactory connectionFactory) => {
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
