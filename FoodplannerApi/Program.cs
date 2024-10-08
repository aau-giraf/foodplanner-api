using System.Text;
using FoodplannerApi;
using Npgsql;
using FoodplannerDataAccessSql.Account;
using FoodplannerDataAccessSql;
using FoodplannerModels;
using FoodplannerModels.Account;
using FoodplannerServices;
using FoodplannerServices.Account;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

//Add environment variables for Infisical and configure SecretsLoader
builder.Configuration.AddEnvironmentVariables(prefix: "INFISICAL_");
SecretsLoader.Configure(builder.Configuration, builder.Environment.EnvironmentName);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



builder.Services.AddSingleton(serviceProvider => {
    var connectionString = SecretsLoader.GetSecret("DB_CONNECTION_STRING");

    return new PostgreSQLConnectionFactory(connectionString);
});

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped(typeof(IUserRepository), typeof(UserRepository));

builder.Services.AddScoped<UserService>();
builder.Services.AddAutoMapper(typeof(UserProfile));

builder.Services.AddControllers();

var app = builder.Build();

builder.Services.AddAuthentication(cfg => {
    cfg.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    cfg.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    cfg.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x => {
    x.RequireHttpsMetadata = false;
    x.SaveToken = false;
    x.TokenValidationParameters = new TokenValidationParameters {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8
                .GetBytes(configuration["ApplicationSettings:JWT_Secret"])
        ),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.MapGet("/test", () => {
    var connectionString = SecretsLoader.GetSecret("DB_CONNECTION_STRING");
    return Results.Text($"Connection string: {connectionString}");
})
.WithName("GetTest")
.WithOpenApi();


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
