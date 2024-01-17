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

app.MapGet("/categories/{id:int}", async (int id, AppDbContext db) =>
{
    return await db.Categories.FindAsync(id) 
    is Category category 
    ? Results.Ok() : Results.NotFound();
});

app.MapPut("/categories/{id:int}", async (int id, Category category, AppDbContext db) =>
{
    if (category.CategoryId != id) return Results.BadRequest();
    var categoryExist = await db.Categories.FindAsync(id);
    if (categoryExist is null) return Results.NotFound();
    categoryExist.Name = category.Name;
    categoryExist.Name = category.Description;
    await db.SaveChangesAsync();
    return Results.Ok(categoryExist);
});

app.MapDelete("/categories/{id:int}", async (int id, AppDbContext db) =>
{
    var category = await db.Categories.FindAsync(id);
    if (category is null) return Results.NotFound();
    db.Categories.Remove(category);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();