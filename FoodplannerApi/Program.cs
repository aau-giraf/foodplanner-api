using FoodplannerDataAccessSql;
using FoodplannerDataAccessSql.Account;
using FoodplannerDataAccessSql.Lunchbox;
using FoodplannerDataAccessSql.Migrations;
using FoodplannerDataAccessSql.Image;
using FoodplannerDataAccessSql.Codes;
using FoodplannerModels;
using FoodplannerModels.Account;
using FoodplannerModels.FeedbackChat;
using FoodplannerModels.Lunchbox;
using FoodplannerModels.Auth;
using FoodplannerModels.Image;
using FoodplannerModels.Codes;
using FoodplannerServices.Account;
using FoodplannerServices.Lunchbox;
using FoodplannerServices.Image;
using FoodplannerServices.Auth;
using FoodplannerServices.FeedbackChat;
using FoodplannerServices.Secret;
using FoodplannerServices.Codes;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using Swashbuckle.AspNetCore.SwaggerGen;
using FluentMigrator.Runner;
using Npgsql;
using Minio;


// Create the base builder for the web application
var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllers();


// Configure Dapper to match column names with underscores to property names with PascalCase
Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;


// Add environment variables to the builder for Infisical and setup SecretsLoader
builder.Configuration.AddEnvironmentVariables(prefix: "INFISICAL_");
var secretsLoader = new SecretsLoader(builder.Configuration, builder.Environment.EnvironmentName);


// Add MinIO to builder and configure it using secrets from SecretsLoader
var endpoint = secretsLoader.GetSecret("MINIO_ENDPOINT");
var accessKey = secretsLoader.GetSecret("MINIO_ACCESS");
var secretKey = secretsLoader.GetSecret("MINIO_SECRET");
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
    // Allows requests from specific origins (intended for the frontend of foodplanner)
    options.AddPolicy("AllowSpecificOrigins",
        policy =>
        {
            policy.WithOrigins("http://localhost:8081") // Replace with your client's URL
                .AllowAnyHeader()
                .AllowAnyMethod();
        });

    // Allows requests from any origin (for development purposes)
    options.AddPolicy("Development",
        policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


// Add Swagger/OpenAPI support to the builder
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Setup Swagger document information
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Foodplanner API",
        Version = "v1"
    });

    // Configure JWT authentication in Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your token in the text input below. Example: \"Bearer 12345abcdef\"",
    });

    // Configure Swagger to allow JWT authentication for API endpoints in bearer scheme
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });

    // Configure Swagger to use inline definitions for enums
    options.SchemaGeneratorOptions = new SchemaGeneratorOptions
    {
        UseInlineDefinitionsForEnums = false
    };
});


// Add PostgreSQL connection factory as a singleton service using secrets from SecretsLoader
builder.Services.AddSingleton(serviceProvider =>
{
    var host = secretsLoader.GetSecret("DB_HOST");
    var port = secretsLoader.GetSecret("DB_PORT");
    var database = secretsLoader.GetSecret("DB_NAME");
    var username = secretsLoader.GetSecret("DB_USER");
    var password = secretsLoader.GetSecret("DB_PASS");
    return new PostgreSQLConnectionFactory(host, port, database, username, password);
});


// Configure user authentication
var configuration = builder.Configuration;
builder.Services.AddAuthentication(cfg =>
{
    // Set JWT bearer authentication to the default authentication sheme
    cfg.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    cfg.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    cfg.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    // Configure which parameters are checked for authentication
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
            Encoding.UTF8.GetBytes(secretsLoader.GetSecret("JWT_SECRET"))
        ),
        ClockSkew = TimeSpan.Zero
    };

    // Adds an additional check for user approval status after JWT validation
    x.Events = new JwtBearerEvents
    {
        OnTokenValidated = context =>
        {
            // Get the claims principal created from the JWT
            var principal = context.Principal;
            if(principal == null)
            {
                throw new Exception("Principal claim is null");
            }

            // Check whether users role is approved for given scenario
            var claimsIdentity = principal.Identity as ClaimsIdentity;
            var statusClaim = claimsIdentity?.FindFirst("RoleApproved")?.Value;
            if (statusClaim != true.ToString())
            {
                context.Fail("User role is not approved");
            }

            return Task.CompletedTask;
        }
    };
});


// Configure authorization policies and which roles live up to them
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ChildPolicy", policy => policy.RequireRole("Child"));
    options.AddPolicy("ParentPolicy", policy => policy.RequireRole("Parent"));
    options.AddPolicy("TeacherChildPolicy", policy => policy.RequireRole("Child", "Teacher"));
    options.AddPolicy("TeacherPolicy", policy => policy.RequireRole("Teacher", "Admin"));
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
});


// Adds FluentMigrator to the builder, which is responsible for migrating the database
builder.Services.AddFluentMigratorCore()
    .ConfigureRunner(rb => rb
        .AddPostgres()
        .WithGlobalConnectionString(
            $"Host={secretsLoader.GetSecret("DB_HOST")};" +
            $"Port={secretsLoader.GetSecret("DB_PORT")};" +
            $"Database={secretsLoader.GetSecret("DB_NAME")};" +
            $"Username={secretsLoader.GetSecret("DB_USER")};" +
            $"Password={secretsLoader.GetSecret("DB_PASS")}")
        .ScanIn(typeof(InitTables).Assembly).For.Migrations())
    .AddLogging(lb => lb.AddFluentMigratorConsole());


//Dependency Injection Starts Here !
// Add Repositories
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped(typeof(IUserRepository), typeof(UserRepository));
builder.Services.AddScoped(typeof(IMealRepository), typeof(MealRepository));
builder.Services.AddScoped(typeof(IIngredientRepository), typeof(IngredientRepository));
builder.Services.AddScoped(typeof(IPackedIngredientRepository), typeof(PackedIngredientRepository));
builder.Services.AddScoped(typeof(IFoodImageRepository), typeof(FoodImageRepository));
builder.Services.AddScoped(typeof(IChildrenRepository), typeof(ChildrenRepository));
builder.Services.AddScoped(typeof(IClassroomRepository), typeof(ClassroomRepository));
builder.Services.AddScoped(typeof(IChatRepository), typeof(ChatRepository));
builder.Services.AddScoped<ISubIngredientRepository, SubIngredientRepository>();
builder.Services.AddScoped<ISubIngredientRelationRepository, SubIngredientRelationRepository>();
builder.Services.AddScoped(typeof(IOneTimePasswordRepository), typeof(OneTimePasswordRepository));

// Add Services
builder.Services.AddScoped<IChildrenService, ChildrenService>();
builder.Services.AddScoped<IClassroomService, ClassroomService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddSingleton<IImageService, ImageService>();
builder.Services.AddScoped<IFoodImageService, FoodImageService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<ISubIngredientService, SubIngredientService>();
builder.Services.AddScoped<ISubIngredientRelationService, SubIngredientRelationService>();
builder.Services.AddScoped<IPasswordHandler, PasswordHandler>();
builder.Services.AddScoped<IIngredientService, IngredientService>();
builder.Services.AddScoped<IMealService, MealService>();
builder.Services.AddScoped<IPackedIngredientService, PackedIngredientService>();
builder.Services.AddScoped<IOneTimePasswordService, OneTimePasswordService>();
builder.Services.AddSingleton<ISecretLoader, SecretsLoader>(_ => secretsLoader);
builder.Services.AddSingleton<IAuthService, AuthService>();

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(UserProfile));
builder.Services.AddAutoMapper(typeof(ChatProfile));
builder.Services.AddAutoMapper(typeof(PackedIngredientProfile));
builder.Services.AddAutoMapper(typeof(IngredientProfile));
builder.Services.AddAutoMapper(typeof(MealProfile));
builder.Services.AddAutoMapper(typeof(ImageProfile));
builder.Services.AddAutoMapper(typeof(UserProfile), typeof(PackedIngredientProfile));
builder.Services.AddAutoMapper(typeof(SubIngredientProfile));






// Builds the application
var app = builder.Build();


// Run migrations at application startup
using (var scope = app.Services.CreateScope())
{
    // Finds the FluentMigrator service
    var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

    // Goes through any migrations not yet applied
    if (runner.HasMigrationsToApplyUp())
    {
        runner.ListMigrations();
        runner.MigrateUp();
    }
}


// Generates swagger page if the application is in development mode
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


// Initializes various parts of the application
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();


// Creates a new end-point to test PostgreSQL connection
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
var backendPort = secretsLoader.GetSecret("BACKEND_PORT");
app.Urls.Add($"http://0.0.0.0:{backendPort}");


// Runs the finalized application
app.Run();