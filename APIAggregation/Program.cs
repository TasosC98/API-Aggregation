using APIAggregation.Data;
using APIAggregation.Services;
using APIAggregation.Services.Definitions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IAggregatedService, AggregatedService>();
builder.Services.AddScoped<IPotterService, PotterService>();
builder.Services.AddScoped<IIpService, IpService>();
builder.Services.AddScoped<IHolidayService, HolidayService>();

// builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();
builder.Services.AddControllers();

builder.Services.AddHttpContextAccessor();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.UseHttpsRedirection();


app.Run();
