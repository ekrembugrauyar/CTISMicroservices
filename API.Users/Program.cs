using APP.Users;
using APP.Users.Domain;
using APP.Users.Features;
using CTISMicroservices.ServiceDefaults;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
var usersDbConnectionString = builder.Configuration.GetConnectionString("UsersDB");
builder.Services.AddDbContext<UsersDb>(options =>
    options.UseNpgsql(usersDbConnectionString));
builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(UsersDbHandler).Assembly));

// AppSettings:
var section = builder.Configuration.GetSection(nameof(AppSettings));
AppSettings.Issuer = section["Issuer"];
AppSettings.Audience = section["Audience"];
AppSettings.ExpirationInMinutes = int.Parse(section["ExpirationInMinutes"]);
AppSettings.SecurityKey = section["SecurityKey"];
AppSettings.RefreshTokenExpirationInDays = int.Parse(section["RefreshTokenExpirationInDays"]);

// Debug logging
var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();
logger.LogInformation("Users API JWT Configuration:");
logger.LogInformation("Issuer: {Issuer}", AppSettings.Issuer);
logger.LogInformation("Audience: {Audience}", AppSettings.Audience);
logger.LogInformation("ExpirationInMinutes: {ExpirationInMinutes}", AppSettings.ExpirationInMinutes);
logger.LogInformation("SecurityKey Length: {SecurityKeyLength}", AppSettings.SecurityKey?.Length ?? 0);
logger.LogInformation("RefreshTokenExpirationInDays: {RefreshTokenExpirationInDays}", AppSettings.RefreshTokenExpirationInDays);

// Enable JWT Bearer authentication as the default scheme.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(config =>
    {
        // Define rules for validating JWT tokens.
        config.TokenValidationParameters = new TokenValidationParameters
        {
            // Match the token's issuer to the expected issuer from AppSettings.
            ValidIssuer = AppSettings.Issuer,

            // Match the token's audience to the expected audience.
            ValidAudience = AppSettings.Audience,

            // Use the symmetric key defined in AppSettings to verify the token's signature.
            IssuerSigningKey = AppSettings.SigningKey,

            // These flags ensure thorough validation of the token.
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true
        };

        config.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                logger.LogError("Authentication failed: {Exception}", context.Exception);
                logger.LogError("Token received: {Token}", context.Request.Headers["Authorization"]);
                logger.LogError("Validation parameters: Issuer={Issuer}, Audience={Audience}, ValidateIssuer={ValidateIssuer}, ValidateAudience={ValidateAudience}, ValidateLifetime={ValidateLifetime}, ValidateIssuerSigningKey={ValidateIssuerSigningKey}",
                    config.TokenValidationParameters.ValidIssuer,
                    config.TokenValidationParameters.ValidAudience,
                    config.TokenValidationParameters.ValidateIssuer,
                    config.TokenValidationParameters.ValidateAudience,
                    config.TokenValidationParameters.ValidateLifetime,
                    config.TokenValidationParameters.ValidateIssuerSigningKey);
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                logger.LogInformation("Token validated successfully");
                logger.LogInformation("Token claims: {Claims}", string.Join(", ", context.Principal.Claims.Select(c => $"{c.Type}: {c.Value}")));
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                logger.LogWarning("Challenge issued: {Error}, {ErrorDescription}", context.Error, context.ErrorDescription);
                logger.LogWarning("Token received: {Token}", context.Request.Headers["Authorization"]);
                return Task.CompletedTask;
            },
            OnMessageReceived = context =>
            {
                logger.LogInformation("Token received: {Token}", context.Request.Headers["Authorization"]);
                return Task.CompletedTask;
            },
            OnForbidden = context =>
            {
                logger.LogWarning("Access forbidden: {Path}", context.Request.Path);
                return Task.CompletedTask;
            }
        };

        // Add token validation parameters logging
        logger.LogInformation("JWT Token Validation Parameters:");
        logger.LogInformation("ValidIssuer: {ValidIssuer}", config.TokenValidationParameters.ValidIssuer);
        logger.LogInformation("ValidAudience: {ValidAudience}", config.TokenValidationParameters.ValidAudience);
        logger.LogInformation("ValidateIssuer: {ValidateIssuer}", config.TokenValidationParameters.ValidateIssuer);
        logger.LogInformation("ValidateAudience: {ValidateAudience}", config.TokenValidationParameters.ValidateAudience);
        logger.LogInformation("ValidateLifetime: {ValidateLifetime}", config.TokenValidationParameters.ValidateLifetime);
        logger.LogInformation("ValidateIssuerSigningKey: {ValidateIssuerSigningKey}", config.TokenValidationParameters.ValidateIssuerSigningKey);
    });


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger/OpenAPI documentation, including JWT auth support in the UI.
builder.Services.AddSwaggerGen(c =>
{
    // Define the basic information for your API.
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Users API",
        Version = "v1",
        Description = "API for managing users, roles, and skills"
    });

    // Add the JWT Bearer scheme to the Swagger UI so tokens can be tested in requests.
    c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. "
        + "Enter your token as: Bearer your_token_here " +
        " Example: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...\""
    });

    // Add the security requirement globally so all endpoints are secured unless specified otherwise.
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


var app = builder.Build();

app.UseCors("AllowAll");  // MUST be before UseAuthorization or UseEndpoints

app.MapDefaultEndpoints();

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

app.Run();
