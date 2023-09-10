using System.Reflection;
using Api.Extensions;
using Api.Features.Product.Services;
using Api.Infrastructure.Persistence;
using Api.Options;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("AppDbContext"),
        optionsBuilder => { optionsBuilder.EnableRetryOnFailure(3); }));

builder.Services.Configure<DefaultValuesOptions>(
    builder.Configuration.GetSection(DefaultValuesOptions.Key));

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    // options.ConfigurationOptions = new ConfigurationOptions
    // {
    //     ConnectRetry = 4,
    //     ConnectTimeout = 3000,
    // };
});

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.Decorate<IProductService, CachedProductService>();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.ConfigureExceptionHandler();

app.MapControllers();

using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
{
    var context = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate();
}

app.Run();

public partial class Program
{
}