using System.Text;
using AspNetCoreRateLimit;
using Market.ApiBehaviours;
using Market.AutoMapperProfiles;
using Market.DataContext;
using Market.Filters;
using Market.Models;
using Market.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.Filters.Add(typeof(TrimRequestStringsAttribute));
    options.Filters.Add(typeof(ExceptionFilter));
    options.Filters.Add(typeof(BadRequestFilter));
});

builder.Services.AddScoped<ValidateImageAndVideoFilterAttribute>();

//ConfigureApiBehaviorOptions(BadRequestBehaviour.Parse);

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

// Rate limiting
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.Configure<IpRateLimitPolicies>(builder.Configuration.GetSection("IpRateLimitPolicies"));
builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Constants.ADMIN, policy => policy.RequireClaim("role", "admin"));
});


builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseIpRateLimiting();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
