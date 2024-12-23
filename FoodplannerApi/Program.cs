using System.Security.Claims;
using System.Text;
using FoodplannerApi;
using FoodplannerApi.Controller;
using Npgsql;
using FoodplannerDataAccessSql;
using FoodplannerDataAccessSql.Account;
using FoodplannerDataAccessSql.Lunchbox;
using FoodplannerModels;
using FoodplannerModels.Account;
using FoodplannerServices.Account;
using FoodplannerModels.Lunchbox;
using FoodplannerServices.Lunchbox;
using FoodplannerDataAccessSql.Image;
using FoodplannerModels;
using FoodplannerModels.Account;
using FoodplannerServices.Account;
using FoodplannerServices.Image;
using Minio;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using FoodplannerApi.Helpers;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Postgres;
using FoodplannerDataAccessSql.Migrations;
using FoodplannerModels.FeedbackChat;
using FoodplannerServices.FeedbackChat;
using Microsoft.OpenApi.Models;

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

    options.AddPolicy("Development",
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

builder.Services.AddSingleton(serviceProvider =>
{
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


builder.Services.AddAuthentication(cfg =>
{
    cfg.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    cfg.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    cfg.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.TokenValidationParameters = new TokenValidationParameters
    {
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
    options.AddPolicy("TeacherPolicy", policy => policy.RequireRole("Teacher", "Admin"));
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
});

//Dependency Injection Starts Here !
// Add Repositories
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped(typeof(IUserRepository), typeof(UserRepository));
builder.Services.AddScoped(typeof(IMealRepository), typeof(MealRepository));
builder.Services.AddScoped(typeof(IIngredientRepository), typeof(IngredientRepository));
builder.Services.AddScoped(typeof(IPackedIngredientRepository), typeof(PackedIngredientRepository));
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<MealService>();
builder.Services.AddScoped<IngredientService>();
builder.Services.AddScoped<PackedIngredientService>();
builder.Services.AddScoped(typeof(IFoodImageRepository), typeof(FoodImageRepository));
builder.Services.AddScoped(typeof(IChildrenRepository), typeof(ChildrenRepository));
builder.Services.AddScoped(typeof(IClassroomRepository), typeof(ClassroomRepository));
builder.Services.AddScoped(typeof(IChatRepository), typeof(ChatRepository));

// Add Services
builder.Services.AddScoped<IChildrenService, ChildrenService>();
builder.Services.AddScoped<IClassroomService, ClassroomService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ChildrenService>();
builder.Services.AddSingleton<IImageService, ImageService>();
builder.Services.AddScoped<IFoodImageService, FoodImageService>();
builder.Services.AddScoped<IChatService, ChatService>();

builder.Services.AddAutoMapper(typeof(UserProfile), typeof(PackedIngredientProfile));

builder.Services.AddSingleton<AuthService>();

// Add Automapper
builder.Services.AddAutoMapper(typeof(UserProfile));
builder.Services.AddAutoMapper(typeof(ChatProfile));


// Set up connection to database before running migrations
builder.Services.AddSingleton(serviceProvider =>
{
    var host = SecretsLoader.GetSecret("DB_HOST");
    var port = SecretsLoader.GetSecret("DB_PORT");
    var database = SecretsLoader.GetSecret("DB_NAME");
    var username = SecretsLoader.GetSecret("DB_USER");
    var password = SecretsLoader.GetSecret("DB_PASS");

    return new PostgreSQLConnectionFactory(host, port, database, username, password);
});

builder.Services.AddFluentMigratorCore()
    .ConfigureRunner(rb => rb
        .AddPostgres()
        .WithGlobalConnectionString(
            $"Host={SecretsLoader.GetSecret("DB_HOST")};" +
            $"Port={SecretsLoader.GetSecret("DB_PORT")};" +
            $"Database={SecretsLoader.GetSecret("DB_NAME")};" +
            $"Username={SecretsLoader.GetSecret("DB_USER")};" +
            $"Password={SecretsLoader.GetSecret("DB_PASS")}")
        .ScanIn(typeof(InitTables).Assembly).For.Migrations())
    .AddLogging(lb => lb.AddFluentMigratorConsole()); //Add logging to migrations to see state.


var app = builder.Build();

// Run migrations at application startup
using (var scope = app.Services.CreateScope())
{
    var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

    if (runner.HasMigrationsToApplyUp())
    {
        runner.ListMigrations();
        runner.MigrateUp();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(c =>
    {
        c.RouteTemplate = "swagger/{documentName}/swagger.json";
        c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
        {
            swaggerDoc.Servers = new List<OpenApiServer>
                { new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}" } };
        });
    });
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