using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SustainableFDPbackend.Data;
using SustainableFDPbackend.Interfaces;
using SustainableFDPbackend.Services;
using System.Text;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using SustainableFDPbackend.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options =>
  options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IDonationService, DonationService>();

builder.Services.AddAuthentication(options =>
{
  options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
  options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
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
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
  };
});

builder.Services.AddAuthorization(options =>
{
  options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
  options.AddPolicy("DonorPolicy", policy => policy.RequireRole("Donor"));
  options.AddPolicy("UserPolicy", policy => policy.RequireRole("User"));
});

// Add CORS policy
builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowSpecificOrigin",
      builder => builder.WithOrigins("http://localhost:5173")
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
  c.SwaggerDoc("v1", new OpenApiInfo { Title = "SustainableFDPbackend API", Version = "v1" });

  // Add JWT Authentication
  c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
  {
    In = ParameterLocation.Header,
    Description = "Please enter JWT with Bearer into field",
    Name = "Authorization",
    Type = SecuritySchemeType.ApiKey,
    Scheme = "Bearer"
  });

  c.AddSecurityRequirement(new OpenApiSecurityRequirement {
    {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        },
        new string[] { }
    }});

  // Configure Swagger to display enum values as integers
  c.MapType<Role>(() => new OpenApiSchema
  {
    Type = "integer",
    Enum = new List<IOpenApiAny>
        {
            new OpenApiInteger((int)Role.Admin),
            new OpenApiInteger((int)Role.Donor),
            new OpenApiInteger((int)Role.User)
        }
  });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//   app.UseDeveloperExceptionPage();
//   app.UseSwagger();
//   app.UseSwaggerUI(c =>
//   {
//     c.SwaggerEndpoint("/swagger/v1/swagger.json", "SustainableFDPbackend API V1");
//     c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
//   });
// }

if (app.Environment.IsDevelopment() || app.Configuration.GetValue<bool>("EnableSwaggerInProduction"))
{
  app.UseSwagger();
  app.UseSwaggerUI(c =>
  {
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SustainableFDPbackend API V1");
    c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
  });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Apply the CORS policy
app.UseCors("AllowSpecificOrigin");

app.MapControllers();

app.Run();
