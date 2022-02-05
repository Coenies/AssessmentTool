using Assessment.ORM;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => { //<-- NOTE 'Add' instead of 'Configure'
    c.SwaggerDoc("v3", new OpenApiInfo
    {
        Title = "AseesmentProject",
        Version = "v3"
    });
});
builder.Services.AddSingleton<MemoryCache>(options =>
{
    return new MemoryCache(new MemoryCacheOptions { SizeLimit = 1240000 });
});

builder.Services.AddDbContext<EFModel>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("EFModelConnectionString")));

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
