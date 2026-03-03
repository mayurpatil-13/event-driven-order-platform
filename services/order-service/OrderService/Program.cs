
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OrderService.Data;
using OrderService.Interfaces;
using OrderService.Kafka;
using OrderService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
// Read JWT secret from environment variable
var secret = builder.Configuration["Jwt:Secret"]
             ?? throw new Exception("JWT_SECRET is missing");

var key = Encoding.UTF8.GetBytes(secret);

// 🔐 Add Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // options.MapInboundClaims = false; // Keep claim names as-is

    options.RequireHttpsMetadata = false;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),

        ValidateIssuer = false,
        ValidateAudience = false,

        RoleClaimType = "role", 
        // NameClaimType = "id" 
    };
});

// 🔐 Add Authorization
builder.Services.AddAuthorization();

builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("Default"),
    new MySqlServerVersion(new Version(8, 0, 26))));

builder.Services.AddHttpClient<IInventoryClient, InventoryClientService>();
builder.Services.AddScoped<IOrderService, OrderService.Services.OrderService>();
builder.Services.AddScoped<KafkaProducer>();

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
