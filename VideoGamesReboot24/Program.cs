using Microsoft.EntityFrameworkCore;
using VideoGamesReboot24.Models;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<GameStoreDbContext>(opts => {
    opts.UseSqlServer(
    builder.Configuration["ConnectionStrings:GameStoreConnection"]);
});

builder.Services.AddScoped<IStoreRepository, Repository>();

builder.Services.AddRazorPages();
//builder.Services.AddServerSideBlazor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.MapControllerRoute("catpage", "{category}/{productPage:int}",
    new { Controller = "Home", action = "Index" });

app.MapControllerRoute("page", "{productPage:int}",
    new { Controller = "Home", action = "Index", productPage = 1 });

app.MapControllerRoute("category", "{category}",
    new { Controller = "Home", action = "Index", productPage = 1 });

app.MapControllerRoute("pagination", "Products/{productPage}",
    new { Controller = "Home", action = "Index", productPage = 1 });

app.MapControllerRoute("details", "Products/Details/{productID}",
    new { Controller = "Home", action = "Details"});

app.MapControllerRoute("edit", "Products/Edit/{productID}",
    new { Controller = "Home", action = "Edit" });


app.MapDefaultControllerRoute();
app.MapRazorPages();
//app.MapBlazorHub();
//app.MapFallbackToPage("/admin/{*catchall}", "/Admin/Index");

app.UseRouting();

app.UseAuthorization();

SeedData.EnsurePopulated(app);

app.Run();
