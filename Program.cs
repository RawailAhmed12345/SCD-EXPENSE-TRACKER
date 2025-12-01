using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ExpenseTracker.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

// Add DbContext explicitly specifying the type argument
builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")
?? "Data Source=expenses.db"));

// Configure Kestrel for dynamic ports on localhost
builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(System.Net.IPAddress.Loopback, 5000); // fixed HTTP port
});


var app = builder.Build();

// --- DATABASE INITIALIZATION ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();


try
    {
        var db = services.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate();
        logger.LogInformation("Applied database migrations.");
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "Migration failed; trying EnsureCreated().");
        try
        {
            var db = services.GetRequiredService<ApplicationDbContext>();
            db.Database.EnsureCreated();
            logger.LogInformation("Ensured database created.");
        }
        catch (Exception ex2)
        {
            logger.LogError(ex2, "Database initialization failed.");
        }
    }

    // Create wwwroot if missing
    var webRootPath = Path.Combine(app.Environment.ContentRootPath, "wwwroot");
    if (!Directory.Exists(webRootPath))
    {
        Directory.CreateDirectory(webRootPath);
        logger.LogInformation("Created missing web root at {WebRootPath}", webRootPath);
    }


}

// --- ERROR HANDLING / HTTPS ---
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    app.UseHttpsRedirection();
}

// --- PIPELINE ---
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();
app.MapControllerRoute(
name: "default",
pattern: "{controller=Expenses}/{action=Index}/{id?}"
);

// --- AUTO OPEN BROWSER AFTER START ---
app.Lifetime.ApplicationStarted.Register(() =>
{
    try
    {
        var url = app.Urls.FirstOrDefault() ?? "[http://127.0.0.1:5000](http://127.0.0.1:5000)";
        Console.WriteLine($"Opening browser at {url}");
        var psi = new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        };
        Process.Start(psi);
    }
    catch (Exception ex)
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogWarning(ex, "Failed to launch browser automatically.");
    }
});

app.Run();
