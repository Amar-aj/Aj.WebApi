using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using WebApi.Context;
using WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddSingleton<DapperDbContext>();
builder.Services.AddScoped<IDapperService, DapperService>();
builder.Services.AddScoped<IAccountService, AccountService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true, // Verify the issuer (who created the token)
                ValidateAudience = true, // Verify the intended recipient
                ValidateLifetime = true, // Check if the token is expired
                ValidateIssuerSigningKey = true, // Validate the signature
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.Default.GetBytes("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")), // Set the secret key used for signing (replace with actual key generation)
                ValidIssuer = "http://localhost:50191", // Set the expected issuer value
                ValidAudience = "http://localhost:50192" // Set the expected audience value
            };
        });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    // Add Bearer token security scheme
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });

    // Add security requirement (optional, but recommended for protected resources)
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            { new OpenApiSecurityScheme { Reference = new OpenApiReference { Id = "Bearer", Type = ReferenceType.SecurityScheme } }, new List<string>() }
        });
});

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

app.Run();
