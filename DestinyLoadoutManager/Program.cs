using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DestinyLoadoutManager.Data;
using DestinyLoadoutManager.Models;
using DestinyLoadoutManager.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

// Add Identity
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

// Add custom services
builder.Services.AddScoped<IWeaponService, WeaponService>();
builder.Services.AddScoped<ILoadoutService, LoadoutService>();
builder.Services.AddScoped<IRecommendationService, RecommendationService>();

// Add Session support
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new IgnoreAntiforgeryTokenAttribute());
});
builder.Services.AddRazorPages(options =>
{
    options.Conventions.ConfigureFilter(new IgnoreAntiforgeryTokenAttribute());
});

var app = builder.Build();

// Initialize database and roles
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        
        // Simple database creation
        context.Database.EnsureCreated();
        
        // Create roles if they don't exist
        string[] roleNames = { "Admin", "User" };
        foreach (var roleName in roleNames)
        {
            var roleExist = roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult();
            if (!roleExist)
            {
                roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
                Console.WriteLine($"Role '{roleName}' created successfully!");
            }
        }

        // Seed Surges if not exists
        if (!context.Surges.Any())
        {
            var surges = new List<Surge>
            {
                new Surge { Name = "Solar", ElementType = ElementType.Solar },
                new Surge { Name = "Arc", ElementType = ElementType.Arc },
                new Surge { Name = "Void", ElementType = ElementType.Void },
                new Surge { Name = "Kinetic", ElementType = ElementType.Kinetic }
            };
            context.Surges.AddRange(surges);
            context.SaveChanges();
            Console.WriteLine("Surges seeded successfully!");
        }

        // Seed Champions if not exists
        if (!context.Champions.Any())
        {
            var champions = new List<Champion>
            {
                new Champion { Name = "Anti-Barrier", Description = "This champion cannot be damaged by standard weapons" },
                new Champion { Name = "Overload", Description = "This champion can interrupt and regenerate" },
                new Champion { Name = "Unstoppable", Description = "This champion cannot be slowed or stunned" }
            };
            context.Champions.AddRange(champions);
            context.SaveChanges();
            Console.WriteLine("Champions seeded successfully!");
        }
        
        Console.WriteLine("Database initialized successfully!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database error: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
