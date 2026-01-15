using System.Diagnostics;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ExpenseTracker.Data;

var builder = WebApplication.CreateBuilder(args);

// -------------------- SERVICES --------------------
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

// Add DB context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// -------------------- KESTREL CONFIG --------------------
builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(System.Net.IPAddress.Loopback, 5000);
});

// -------------------- BUILD APP --------------------
var app = builder.Build();

// -------------------- DATABASE MIGRATION --------------------
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
        logger.LogError(ex, "Error migrating database.");
        throw;
    }

    // Ensure wwwroot exists
    var webRootPath = Path.Combine(app.Environment.ContentRootPath, "wwwroot");
    if (!Directory.Exists(webRootPath))
    {
        Directory.CreateDirectory(webRootPath);
        logger.LogInformation("Created missing web root at {WebRootPath}", webRootPath);
    }
}

// -------------------- MIDDLEWARE --------------------
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// -------------------- ROUTES --------------------
app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}"
);

// -------------------- AUTO OPEN BROWSER --------------------
app.Lifetime.ApplicationStarted.Register(() =>
{
    try
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = "http://127.0.0.1:5000",
            UseShellExecute = true
        });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Failed to open browser automatically: {ex.Message}");
    }
});

// -------------------- RUN APP --------------------
app.Run();
