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

builder.Services.AddDbContext<AppIdentityDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration["ConnectionStrings:IdentityConnection"]));
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppIdentityDbContext>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminRestricted", policy => policy.RequireRole("Admin"));
});

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

app.MapControllerRoute("delete", "Products/Delete/{productID}",
    new { Controller = "Home", action = "Delete" });

app.MapDefaultControllerRoute();

app.MapRazorPages();
//app.MapBlazorHub();
//app.MapFallbackToPage("/admin/{*catchall}", "/Admin/Index");

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

SeedData.EnsurePopulated(app);
IdentitySeedData.EnsurePopulated(app);

app.Run();
