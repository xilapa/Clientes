using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Clientes.Infra;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    });
builder.Services.Configure<ApiBehaviorOptions>(opt => opt.SuppressModelStateInvalidFilter = true);
builder.Services.Configure<RouteOptions>(opt =>
{
    opt.LowercaseUrls = true;
    opt.LowercaseQueryStrings = true;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opts =>
{
    opts.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Clientes API v1",
        Description = "API para gerenciamento de clientes",
        Contact = new OpenApiContact
        {
            Name = "Dirceu da Silva Junior",
            Email = "dirceu.srj@gmail.com",
            Url = new Uri("https://github.com/xilapa")
        }
    });
    
    opts.UseInlineDefinitionsForEnums();

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    opts.IncludeXmlComments(xmlPath);
});

builder.Services.AddOptions<Settings>()
    .BindConfiguration("")
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services
    .AddInfra()
    .AddApplication();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI(opts =>
{
    opts.DefaultModelsExpandDepth(-1);
    opts.SwaggerEndpoint("/swagger/v1/swagger.json", "Clientes API v1");
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.Services.AplicarMigrations();

app.Run();