using Assessment.Models;
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
builder.Services.AddSingleton<Assessment.CoinMarketCapAPI.API>();
builder.Services.AddTransient<Assessment.Models.DataStore>();
builder.Services.AddMemoryCache();
builder.Services.AddScoped<Assessment.Models.IDataItemRepository, Assessment.Models.DateItemEFRepository>();

builder.Services.AddDbContext<EFModel>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("EFModelConnectionString"));
});



var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<EFModel>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseAuthorization();


app.MapControllers();

app.Run();
