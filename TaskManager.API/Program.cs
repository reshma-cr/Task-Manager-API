using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TaskManager.Application;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connString = Environment.GetEnvironmentVariable("DATABASE_URL");

if (!string.IsNullOrEmpty(connString) && (connString.StartsWith("postgres://") || connString.StartsWith("postgresql://")))
{
    // Standardize to postgres:// for consistent Uri parsing
    if (connString.StartsWith("postgresql://"))
    {
        connString = "postgres://" + connString.Substring(13);
    }

    var databaseUri = new Uri(connString);
    var userInfo = databaseUri.UserInfo.Split(':');

    // If Port is missing (-1), default it to 5432
    int port = databaseUri.Port == -1 ? 5432 : databaseUri.Port;

    connString = $"Server={databaseUri.Host};" +
                 $"Port={port};" +
                 $"User Id={userInfo[0]};" +
                 $"Password={userInfo[1]};" +
                 $"Database={databaseUri.AbsolutePath.TrimStart('/')};" +
                 $"SSL Mode=Require;Trust Server Certificate=true;";
}
else
{
    // Fallback to local configuration if DATABASE_URL is missing or standard
    connString ??= builder.Configuration.GetConnectionString("DefaultConnection");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connString));

builder.Services.AddControllers();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<INLTaskService, NLTaskService>();
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000", "https://task-manager-client-navy-rho.vercel.app")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET") 
    ?? builder.Configuration["Jwt:Secret"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
