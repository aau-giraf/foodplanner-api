using System.Security.Claims;
using System.Text;
using FoodplannerApi;
using Npgsql;
using FoodplannerDataAccessSql.Account;
using FoodplannerDataAccessSql;
using FoodplannerModels;
using FoodplannerModels.Account;
using FoodplannerServices.Account;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using FoodplannerApi.Helpers;



var builder = WebApplication.CreateBuilder(args);


//Add environment variables for Infisical and configure SecretsLoader
builder.Configuration.AddEnvironmentVariables(prefix: "INFISICAL_");
SecretsLoader.Configure(builder.Configuration, builder.Environment.EnvironmentName);


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Foodplanner API",
        Version = "v1"
    });

    // Add JWT authentication to Swagger
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your token in the text input below. Example: \"Bearer 12345abcdef\"",
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});


builder.Services.AddSingleton(serviceProvider => {
    var host = SecretsLoader.GetSecret("DB_HOST");
    var port = SecretsLoader.GetSecret("DB_PORT");
    var database = SecretsLoader.GetSecret("DB_NAME");
    var username = SecretsLoader.GetSecret("DB_USER");
    var password = SecretsLoader.GetSecret("DB_PASS");

    return new PostgreSQLConnectionFactory(host, port, database, username, password);
});



// Add services to the container.
builder.Services.AddControllers();

// Configure JWT authentication
var configuration = builder.Configuration;


builder.Services.AddAuthentication(cfg => {
    cfg.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    cfg.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    cfg.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x => {
    x.TokenValidationParameters = new TokenValidationParameters {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = configuration["ApplicationSettings:JWT_Issuer"],
        ValidAudience = configuration["ApplicationSettings:JWT_Audience"],
        RoleClaimType = ClaimTypes.Role,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(configuration["ApplicationSettings:JWT_Secret"])
        ),
        ClockSkew = TimeSpan.Zero
    };
    x.Events = new JwtBearerEvents
    {
        OnTokenValidated = context =>
        {
            var claimsIdentity = context.Principal.Identity as ClaimsIdentity;

            // Get the Status claim
            var statusClaim = claimsIdentity?.FindFirst("Status")?.Value;

            if (statusClaim != "Active")
            {
                // If status is not active, fail the authentication
                context.Fail("Inactive user status");
            }

            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ChildPolicy", policy => policy.RequireRole("Child"));
    options.AddPolicy("ParentPolicy", policy => policy.RequireRole("Parent"));
    options.AddPolicy("TeacherPolicy", policy => policy.RequireRole("Teacher"));
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
});

//Dependency Injection Starts Here !
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped(typeof(IUserRepository), typeof(UserRepository));

builder.Services.AddScoped<UserService>();
builder.Services.AddAutoMapper(typeof(UserProfile));

builder.Services.AddSingleton<AuthService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
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
