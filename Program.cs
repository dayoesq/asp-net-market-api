using System.Text;
using Market.ApiBehaviours;
using Market.AutoMapperProfiles;
using Market.DataContext;
using Market.Filters;
using Market.Models;
using Market.Services.Jwt;
using Market.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Controller services
builder.Services.AddControllers(options =>
{
    options.Filters.Add(typeof(TrimRequestStringsAttribute));
    options.Filters.Add(typeof(GlobalExceptionFilter));
    options.Filters.Add(typeof(BadRequestFilter));
}).ConfigureApiBehaviorOptions(BadRequestBehaviour.Parse);

builder.Services.AddScoped<ValidateImageAndVideoFilterAttribute>();
builder.Services.AddScoped<IJwtService, JwtProvider>();
builder.Services.AddMvc(options =>
{
    options.Conventions.Add(new ControllerGlobalPrefix());
});

// Mapper services
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddCors(options =>
{
    var clientBaseUrl = builder.Configuration.GetValue<string>("ClientBaseUrl");
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(clientBaseUrl!).AllowAnyMethod().AllowAnyHeader();
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new
            SymmetricSecurityKey
            (Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("JWT:Secret")!))
    };

});

builder.Services.AddAuthorization();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


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
