using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ShoppingOnline.Data;
using ShoppingOnline.Services.Categories;
using ShoppingOnline.Services.Implementations;
using ShoppingOnline.Services.Interfaces;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// =======================
// DATABASE
// =======================
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// =======================
// DEPENDENCY INJECTION
// =======================
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IAdminProductService, AdminProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductImageService, ProductImageService>();

// =======================
// JWT AUTHENTICATION
// =======================
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSection["Key"];

if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new Exception("JWT Key is missing in appsettings.json");
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey!)
            ),

            RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.Zero
        };

        // DEBUG
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine("JWT AUTH FAILED: " + context.Exception.Message);
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("JWT TOKEN VALIDATED");
                return Task.CompletedTask;
            }
        };
    });

// =======================
// AUTHORIZATION
// =======================
builder.Services.AddAuthorization();

// =======================
// CONTROLLERS
// =======================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// =======================
// SWAGGER + JWT
// =======================
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ShoppingOnline API",
        Version = "v1"
    });

    // FIX 1: KHAI BÁO SERVER HTTPS CỤ THỂ
    c.AddServer(new OpenApiServer
    {
        Url = "https://localhost:7011"
    });

    //FIX 2: BEARER SCHEME CHUẨN
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Nhập: Bearer {JWT token}"
    });

    //FIX 3: ÉP SWAGGER GẮN TOKEN VÀO REQUEST
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// =======================
// MIDDLEWARE PIPELINE
// =======================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ShoppingOnline API v1");
        c.RoutePrefix = "swagger";
    });
}
//img
app.UseStaticFiles();

//Giữ HTTPS
app.UseHttpsRedirection();

// BẮT BUỘC ĐÚNG THỨ TỰ
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();