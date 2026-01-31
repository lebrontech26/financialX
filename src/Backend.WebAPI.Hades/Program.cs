using Backend.Data;
using Backend.Data.Repositories.ClienteRepository;
using Backend.Data.Repositories.ScoringRepository;
using Backend.Infrastructure.BcraGateway;
using Backend.Infrastructure.BcraGateway.Services;
using Backend.WebAPI.Hades.Features.Clients.Scoring.Services;
using Backend.WebAPI.Hades.Features.Clients.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
//Agregar Swagger
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();

builder.Services.AddControllers();

// =======================
// CORS CONFIG
// =======================
const string FrontendCorsPolicy = "FrontendDev";

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? new[] { "http://localhost:5173" };

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: FrontendCorsPolicy, policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

//Entity Framework - Db
var connectionString = builder.Configuration.GetConnectionString("Sqlite");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
   options.UseSqlite(
        connectionString,
        sqliteOptions =>
        {
            sqliteOptions.CommandTimeout(30);
        });

    if(builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

//AutoMapper
builder.Services.AddAutoMapper(cfg =>{}, typeof(Program).Assembly);

//Infrastructure
builder.Services.AddBcraGateway(builder.Configuration);
builder.Services.AddScoped<IBcraDataService, BcraDataService>();

//Repositories
builder.Services.AddScoped<IScoringRulesRepository, ScoringRulesRepository>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();

//Services
builder.Services.AddScoped<IScoringService, ScoringService>();
builder.Services.AddScoped<IClienteService, ClienteService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwagger();
    app.UseSwaggerUI(opt =>
    {
       opt.SwaggerEndpoint("/swagger/v1/swagger.json", "Financiera API v1");
       opt.RoutePrefix = "swagger"; 
    });
}

app.UseHttpsRedirection();

app.UseCors(FrontendCorsPolicy);

app.UseAuthorization();

app.MapControllers();

app.Run();
