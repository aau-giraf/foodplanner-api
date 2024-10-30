using System.Security.Claims;
using System.Text;
using FoodplannerApi;
using FoodplannerApi.Controller;
using Npgsql;
using FoodplannerDataAccessSql.Account;
using FoodplannerDataAccessSql;
using FoodplannerDataAccessSql.Image;
using FoodplannerModels;
using FoodplannerModels.Account;
using FoodplannerServices.Account;
using FoodplannerServices.Image;
using Minio;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using FoodplannerApi.Helpers;


var builder = WebApplication.CreateBuilder(args);


//Add environment variables for Infisical and configure SecretsLoader
builder.Configuration.AddEnvironmentVariables(prefix: "INFISICAL_");
SecretsLoader.Configure(builder.Configuration, builder.Environment.EnvironmentName);


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

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
// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        policy =>
        {
            policy.WithOrigins("http://localhost:8081") // Replace with your client's URL
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    
    options.AddPolicy("Develompmen", 
        policy =>
    {
        policy.WithOrigins("*")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

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
            var statusClaim = claimsIdentity?.FindFirst("RoleApproved")?.Value;

            if (statusClaim != true.ToString())
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
builder.Services.AddScoped(typeof(IFoodImageRepository), typeof(FoodImageRepository));


builder.Services.AddScoped<UserService>();
builder.Services.AddSingleton<IImageService, ImageService>();
builder.Services.AddScoped<IFoodImageService, FoodImageService>();
builder.Services.AddAutoMapper(typeof(UserProfile));

builder.Services.AddSingleton<AuthService>();
builder.Services.AddScoped<ImagesController.AuthoriseImageOwnerFilter>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Apply CORS policy
if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development") 
    app.UseCors("Development");
else app.UseCors("AllowSpecificOrigins");

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// New endpoint to test database connection
app.MapGet("/test-db-connection", async (PostgreSQLConnectionFactory connectionFactory) =>
    {
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
var backendPort = SecretsLoader.GetSecret("BACKEND_PORT");
app.Urls.Add($"http://0.0.0.0:{backendPort}");

app.Run();