using Microsoft.EntityFrameworkCore;
using StarWars.Api.Controllers;
using StarWars.Api.GraphQl;
using StarWars.Bl.Interface;
using StarWars.Bl.Services;
using StarWars.Domain.Interface;
using StarWars.Domain.Repository;
using StarWars.Model.Configuration;
using StarWars.Model.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//graphQL
builder.Services.AddGraphQLServer()
    .AddQueryType<Query>()
    .AddProjections()
    .AddFiltering()
    .AddSorting()
    .AddMutationType<Mutation>()
    .AddInMemorySubscriptions()
    .AddSubscriptionType<Subscription>();


builder.Services.AddCors();

builder.Services.AddDbContextFactory<StarWarsContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Dependency Injection
// Registering the repository
builder.Services.AddTransient<IRepository<Character>, CharacterRepository>();

// Registering the service
builder.Services.AddTransient<IService<Character>, CharacterService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<StarWarsContext>();
    await context.Database.MigrateAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapGraphQL(path: "/graphql");
app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseWebSockets();

CharacterController.Map(app);

app.Run();

public partial class Program
{
}