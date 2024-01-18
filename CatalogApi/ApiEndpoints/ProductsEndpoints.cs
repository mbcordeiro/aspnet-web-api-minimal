using CatalogApi.Context;
using CatalogApi.Model;
using Microsoft.EntityFrameworkCore;

namespace CatalogApi.ApiEndpoints
{
    public static class ProductsEndpoints
    {
        public static void MapProductsEndpoints(this WebApplication app)
        {
            app.MapPost("/products", async (Product product, AppDbContext db) => {
                db.Products.Add(product);
                await db.SaveChangesAsync();
                return Results.Created($"/products/{product.ProductId}", product);
            }).RequireAuthorization();

            app.MapGet("/products", async (AppDbContext db) => await db.Products.ToListAsync());

            app.MapGet("/products/{id:int}", async (int id, AppDbContext db)
                => {
                    return await db.Products.FindAsync(id)
                    is Product product
                    ? Results.Ok(product) : Results.NotFound();
                }).RequireAuthorization();

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
            }).RequireAuthorization();

            app.MapDelete("/products/{id:int}", async (int id, AppDbContext db) =>
            {
                var products = await db.Products.FindAsync(id);
                if (products is null) return Results.NotFound();
                db.Products.Remove(products);
                await db.SaveChangesAsync();
                return Results.NoContent();
            }).RequireAuthorization();
        }
    }
}
