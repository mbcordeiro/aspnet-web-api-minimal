using CatalogApi.Context;
using CatalogApi.Model;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString,ServerVersion.AutoDetect(connectionString)));
var app = builder.Build();

app.MapPost("/categories", async (Category categroy, AppDbContext db) =>
{
    db.Categories.Add(categroy);
    await db.SaveChangesAsync();
    return Results.Created($"/categories/{categroy.CategoryId}", categroy);
});

app.MapGet("/categories", async (AppDbContext db) => await db.Categories.ToListAsync());

app.MapGet("/categories/{id}", async (int id, AppDbContext db) =>
{
    return await db.Categories.FindAsync(id) 
    is Category category 
    ? Results.Ok() : Results.NotFound();
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();