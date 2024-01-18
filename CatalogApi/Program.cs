﻿using CatalogApi.Context;
using CatalogApi.Model;
using CatalogApi.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString,ServerVersion.AutoDetect(connectionString)));
builder.Services.AddSingleton<ITokenService>(new TokenService());
builder.Services.AddAuthentication
                 (JwtBearerDefaults.AuthenticationScheme)
                 .AddJwtBearer(options =>
                 {
                     options.TokenValidationParameters = new TokenValidationParameters
                     {
                         ValidateIssuer = true,
                         ValidateAudience = true,
                         ValidateLifetime = true,
                         ValidateIssuerSigningKey = true,
                         ValidIssuer = builder.Configuration["Jwt:Issuer"],
                         ValidAudience = builder.Configuration["Jwt:Audience"],
                         IssuerSigningKey = new SymmetricSecurityKey
                         (Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                     };
                 });

builder.Services.AddAuthorization();

var app = builder.Build();

app.MapPost("/login", [AllowAnonymous] (User user, ITokenService tokenService) =>
{
    if (user == null) return Results.BadRequest("Invalid Login");
    if (user.Username == "mbcordeiro" && user.Password == "pass#123")
    {
        var tokenString = tokenService.GenerateToken(app.Configuration["Jwt:Key"],
            app.Configuration["Jwt:Issuer"],
            app.Configuration["Jwt:Audience"],
            user);
        return Results.Ok(new { token = tokenString });
    }
    else return Results.BadRequest("Invalid Login");
}).Produces(StatusCodes.Status400BadRequest)
              .Produces(StatusCodes.Status200OK)
              .WithName("Login")
              .WithTags("Authentication");

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

app.MapPost("/products", async (Product product, AppDbContext db)
 => {
     db.Products.Add(product);
     await db.SaveChangesAsync();
     return Results.Created($"/products/{product.ProductId}", product);
 });

app.MapGet("/products", async (AppDbContext db) => await db.Products.ToListAsync());

app.MapGet("/products/{id:int}", async (int id, AppDbContext db)
    => {
        return await db.Products.FindAsync(id) 
        is Product product 
        ? Results.Ok(product): Results.NotFound();
    });

app.MapPut("/products/{id:int}", async (int id, Product product, AppDbContext db) =>
{
    if (product.ProductId != id) return Results.BadRequest();
    var existProduct = await db.Products.FindAsync(id);
    if (existProduct is null) return Results.NotFound();
    existProduct.Name = product.Name;
    existProduct.Description = product.Description;
    existProduct.Price = product.Price;
    existProduct.ImageUrl = product.ImageUrl;
    existProduct.RegistrationDate = product.RegistrationDate;
    existProduct.Stock = product.Stock;
    existProduct.CategoryId = product.CategoryId;
    await db.SaveChangesAsync();
    return Results.Ok(existProduct);
});

app.MapDelete("/products/{id:int}", async (int id, AppDbContext db) =>
{
    var products = await db.Products.FindAsync(id);
    if (products is null) return Results.NotFound();
    db.Products.Remove(products);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.Run();