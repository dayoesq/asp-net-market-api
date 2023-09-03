using Market.ApiBehaviours;
using Market.AutoMapperProfiles;
using Market.DataContext;
using Market.Filters;
using Market.Models;
using Market.OptionsSetup;
using Market.Services.Jwt;
using Market.UnitOfWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers(options =>
{
    options.Filters.Add(typeof(TrimRequestStringsAttribute));
    options.Filters.Add(typeof(GlobalExceptionFilter));
    options.Filters.Add(typeof(BadRequestFilter));
}).ConfigureApiBehaviorOptions(BadRequestBehaviour.Parse);

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ValidateImageAndVideoFilterAttribute>();
builder.Services.AddScoped<IJwtService, JwtProvider>();
builder.Services.AddMvc(options =>
{
    options.Conventions.Add(new ControllerGlobalPrefix());
});

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

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

builder.Services.ConfigureOptions<JwtBearerOptionsSetup>();

builder.Services.AddAuthorization();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


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
