using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RestaurantsApi.Api.Middleware;
using RestaurantsApi.Application.Services;
using RestaurantsApi.Infrastructure.Repositories.Implementations;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.PostgreSQL;


var builder = WebApplication.CreateBuilder(args);

// Use Serilog for logging
//logging send to db
var columnWriters = new Dictionary<string, ColumnWriterBase>
{
    ["message"] = new RenderedMessageColumnWriter(NpgsqlTypes.NpgsqlDbType.Text),
    ["message_template"] = new MessageTemplateColumnWriter(NpgsqlTypes.NpgsqlDbType.Text),
    ["level"] = new LevelColumnWriter(true, NpgsqlTypes.NpgsqlDbType.Varchar),
    ["raise_date"] = new TimestampColumnWriter(NpgsqlTypes.NpgsqlDbType.Timestamp),
    ["exception"] = new ExceptionColumnWriter(NpgsqlTypes.NpgsqlDbType.Text),
    ["properties"] = new LogEventSerializedColumnWriter(NpgsqlTypes.NpgsqlDbType.Jsonb)
};

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Error()
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.PostgreSQL(
        connectionString: builder.Configuration.GetConnectionString("DefaultConnection"),
        tableName: "logs",
        columnOptions: columnWriters,
        restrictedToMinimumLevel: LogEventLevel.Error,
        schemaName: "public",
        needAutoCreateTable: false,
        useCopy: true)
    .CreateLogger();
builder.Host.UseSerilog();


// Enable legacy timestamp behavior (PostgreSQL compatibility)
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);


// nivele recomandate (poți ajusta)
builder.Logging.ClearProviders();
builder.Logging.AddConsole(); // ← se vede în Log stream
builder.Logging.AddDebug();


//add services to the container
builder.Services.AddScoped<IRestaurantsRepository, RestaurantsRepository>();
builder.Services.AddScoped<IRestaurantsService, RestaurantsService>();
// If you made your services or repositories Singleton or Transient:
// Singleton → would share the same DbContext across all requests → thread safety issues + stale data risk.
// Transient → could create multiple DbContext instances within the same request, which is wasteful and might break transaction consistency.


// Caching
builder.Services.AddMemoryCache();
builder.Services.AddResponseCaching();


// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Add database context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        builder =>
        {
            // builder.WithOrigins("https://your-frontend-url.azurewebsites.net") // Replace with your React app's URL
            builder.AllowAnyOrigin()
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});


// Add controllers
builder.Services.AddControllers();


// Authentication and Authorization
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),

            RoleClaimType = ClaimTypes.Role,
            NameClaimType = ClaimTypes.Name
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});


// Rate limiter
builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("RateLimiting", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: key => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                AutoReplenishment = true
            }
        )
        );

    options.OnRejected = async (context, token) =>
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            context.HttpContext.Response.ContentType = "application/json";
            await context.HttpContext.Response.WriteAsJsonAsync(new
            {
                error = "Too many requests. Please try agains later."
            });
        };

    // options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
    //   RateLimitPartition.GetFixedWindowLimiter(
    //       partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
    //       factory: partition => new FixedWindowRateLimiterOptions
    //       {
    //           AutoReplenishment = true,
    //           PermitLimit = 10,
    //           QueueLimit = 0,
    //           Window = TimeSpan.FromMinutes(1)
    //       }));
});



//build
var app = builder.Build();

// Custom global exception handling middleware
app.UseGlobalExceptionMiddleware();

// Rate limiter
app.UseRateLimiter();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // app.UseExceptionHandler("/Error"); // Custom error handling
    app.UseHsts(); // Use HSTS for additional security in production
}

// Enable CORS middleware
app.UseCors("AllowReactApp");

// Serve static files for React if frontend is deployed together
// Uncomment the following if deploying React and backend to the same App Service
// app.UseDefaultFiles();
// app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Restaurants API V1");
    c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
});

app.MapGet("/testSerilog", (ILogger<Program> log) =>
{
    log.LogError("Test error from /testSerilog endpoint");
    return Results.Problem("boom");
});

// Ensure database is created and apply migrations
// using (var scope = app.Services.CreateScope())
// {
//     var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//     db.Database.Migrate();
// }

// Map controllers
app.MapControllers();

app.Run();
