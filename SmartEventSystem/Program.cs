using Microsoft.EntityFrameworkCore;
using SmartEventSystem.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllersWithViews();
builder.Services.AddSession();   // ✅ Add session service

builder.Services.AddDbContext<SmartEventSystemContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("SmartEventSystemContext")));

var app = builder.Build();   // ✅ Build FIRST

// Configure middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();   // ✅ Use session AFTER app is built

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

