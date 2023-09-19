using System.Text;
using Market.ApiBehaviours;
using Market.AutoMapperProfiles;
using Market.DataContext;
using Market.Filters;
using Market.Models;
using Market.OptionsSetup;
using Market.OptionsSetup.Jwt;
using Market.Utils.Constants;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers(options =>
{
    options.Filters.Add(typeof(TrimRequestStringsAttribute));
    options.Filters.Add(typeof(GlobalExceptionFilter));
    options.Filters.Add(typeof(BadRequestFilter));
}).ConfigureApiBehaviorOptions(BadRequestBehaviour.Parse);

builder.Services.AddScoped<ValidateImageAndVideoFilterAttribute>();

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

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration.GetValue<string>("Jwt:Issuer"),
        ValidAudience = builder.Configuration.GetValue<string>("Jwt:Audience"),
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("Jwt:SecretKey")!)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddSingleton(config => config.GetRequiredService<IOptions<JwtOptions>>().Value);

builder.Services.ConfigureOptions<JwtOptionsSetup>();

builder.Services.ConfigureOptions<AuthorizationOptionsSetup>();

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();


app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
