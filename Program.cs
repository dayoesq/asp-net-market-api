using Market.ApiBehaviours;
using Market.DataContext;
using Market.Filters;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.Filters.Add(typeof(ExceptionFilter));
    options.Filters.Add(typeof(BadRequestFilter));
}).ConfigureApiBehaviorOptions(BadRequestBehaviour.Parse);

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddCors(options =>
{
    var clientBaseUrl = builder.Configuration.GetValue<string>("client_base_url");
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(clientBaseUrl!).AllowAnyMethod().AllowAnyHeader();
    });
});

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
