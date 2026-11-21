using System.Text;
using CarRental.Data;
using CarRental.Models;
using CarRental.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("AuthDatabase")
    ?? throw new InvalidOperationException("Falta ConnectionStrings:AuthDatabase.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 0))));

var jwt = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
    ?? throw new InvalidOperationException("Falta la configuración Jwt.");
if (Encoding.UTF8.GetByteCount(jwt.Key) < 32)
    throw new InvalidOperationException("Jwt:Key debe tener al menos 32 bytes y configurarse como secreto local.");
if (string.IsNullOrWhiteSpace(jwt.Issuer) || string.IsNullOrWhiteSpace(jwt.Audience) || jwt.ExpireMinutes <= 0)
    throw new InvalidOperationException("Jwt requiere Issuer, Audience y ExpireMinutes válidos.");

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));
builder.Services.AddScoped<JwtTokenService>();
builder.Services.AddScoped<IPasswordHasher<AppUser>, PasswordHasher<AppUser>>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt.Issuer,
            ValidateAudience = true,
            ValidAudience = jwt.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30),
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
            NameClaimType = System.Security.Claims.ClaimTypes.Name,
            RoleClaimType = System.Security.Claims.ClaimTypes.Role
        };
    });
builder.Services.AddAuthorization();

// Existing SQL Server and MySQL repositories remain available for the car rental data.
builder.Services.AddScoped<CarRentalServices.Contracts.IDataService, CarRentalServices.Services.DataService>();
builder.Services.AddScoped<CarRentalServices.Contracts.IMySqlDataService, CarRentalServices.Services.MySqlDataService>();
builder.Services.AddKeyedScoped<CarRentalServices.Contracts.ICategoryRepository, CarRentalServices.Repositories.MySqlCategoryRepository>("MySql");
builder.Services.AddKeyedScoped<CarRentalServices.Contracts.ICategoryRepository, CarRentalServices.Repositories.CategoryRepository>("MsSql");
builder.Services.AddKeyedScoped<CarRentalServices.Contracts.ICarsRepository, CarRentalServices.Repositories.CarRepository>("MsSql");
builder.Services.AddKeyedScoped<CarRentalServices.Contracts.ICarsRepository, CarRentalServices.Repositories.CarsRepository>("MySql");
builder.Services.AddScoped<CarRentalServices.Contracts.ICountryRepository, CarRentalServices.Repositories.CountryRepository>();
builder.Services.AddScoped<CarRentalServices.Contracts.ICustomerRepository, CarRentalServices.Repositories.CustomerRepository>();
builder.Services.AddScoped<CarRentalServices.Contracts.ILocationRepository, CarRentalServices.Repositories.LocationRepository>();
builder.Services.AddKeyedScoped<CarRentalServices.Contracts.IColorRepository, CarRentalServices.Repositories.ColorRepository>("MySql");
builder.Services.AddKeyedScoped<CarRentalServices.Contracts.IBrandRepository, CarRentalServices.Repositories.BrandRepository>("MySql");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.OperationFilter<AuthorizeCheckOperationFilter>();
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Pega el token JWT obtenido en /api/auth/login."
    });
});

var app = builder.Build();
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
