using RealStateApi.Configurations;
using RealStateApi.Data;
using RealStateApi.Mapping;
using RealStateApi.Repositories.Owners;
using RealStateApi.Repositories.Properties;
using RealStateApi.Repositories.propertyImages;
using RealStateApi.Repositories.PropertyTraces;
using RealStateApi.Seed;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

builder.Services.AddSingleton<MongoContext>();
builder.Services.AddScoped<DataSeeder>();
builder.Services.AddScoped<IPropertyRepository, PropertyRepository>();
builder.Services.AddScoped<IOwnerRepository, OwnerRepository>();
builder.Services.AddScoped<IPropertyImageRepository, PropertyImageRepository>();
builder.Services.AddScoped<IPropertyTraceRepository, PropertyTraceRepository>();
builder.Services.AddAutoMapper(typeof(PropertyProfile));
builder.Services.AddAutoMapper(typeof(OwnerProfile));
builder.Services.AddAutoMapper(typeof(PropertyImageProfile));
builder.Services.AddAutoMapper(typeof(PropertyTraceProfile));

using (var scope = builder.Services.BuildServiceProvider().CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<MongoContext>();
    var seeder = new DataSeeder(context);
    await seeder.SeedAsync();
}

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
