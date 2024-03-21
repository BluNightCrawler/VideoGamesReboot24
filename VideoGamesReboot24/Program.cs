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
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

builder.Services.AddDbContext<AppIdentityDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration["ConnectionStrings:IdentityConnection"]));
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<AppIdentityDbContext>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminRestricted", policy => policy.RequireRole("Admin"));
    options.AddPolicy("LoginRestricted", policy => policy.RequireRole("User"));
});

builder.Services.AddTransient<UserManager<AppUser>>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseRequestLocalization(opts => {
    opts.AddSupportedCultures("en-US")
        .AddSupportedUICultures("en-US")
        .SetDefaultCulture("en-US");
});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();

app.MapControllerRoute("catpage", "Products/Catalog/{category}/{productPage:int}",
    new { Controller = "Home", action = "Catalog" });

app.MapControllerRoute("page", "Products/Catalog/{productPage:int}",
    new { Controller = "Home", action = "Catalog", productPage = 1 });

app.MapControllerRoute("category", "Products/Catalog/{category}",
    new { Controller = "Home", action = "Catalog", productPage = 1 });

app.MapControllerRoute("pagination", "Products/Catalog/{productPage}",
    new { Controller = "Home", action = "Catalog", productPage = 1 });

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
