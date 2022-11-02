using Microsoft.OpenApi.Models;
using Asp.Versioning;
using CapitalBroker;
using CapitalBroker.Filters;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseKestrel(option => option.AddServerHeader = false);

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = new HeaderApiVersionReader("api-version");
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen( setup =>
{
    setup.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo()
    {
        Description = "Broker service of open-api.capital.com",
        Title = "CapitalBroker",
        Version = "v1",
        Contact = new OpenApiContact()
        {
            Name = "Aleksandar"
        }
    });

    setup.OperationFilter<AddAPIKeyHeaderOperationFilter>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI( c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CapitalBroker v1");
    });
}

app.MapGeneralEndPoints();
app.MapSessionEndPoints();
app.MapAccountsEndPoints();
app.MapTradingEndPoints();
app.MapWatchListsEndPoints();

app.UseWebSockets();
app.UseRouting(); 

//app.UseHttpsRedirection();

//app.MapGet("/", () => "Hello CapitalBroker!");

app.Run();
