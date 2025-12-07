using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using DestinyLoadoutManager.Data;
using DestinyLoadoutManager.Models;
using DestinyLoadoutManager.Services;
using System.IO;

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

        // Seed Champion Weapon Type mappings if not exists
        if (!context.Set<ChampionWeaponType>().Any())
        {
            var antiBarrier = context.Champions.First(c => c.Name == "Anti-Barrier");
            var overload = context.Champions.First(c => c.Name == "Overload");
            var unstoppable = context.Champions.First(c => c.Name == "Unstoppable");

            var championWeaponTypes = new List<ChampionWeaponType>
            {
                // Anti-Barrier: Scout Rifles, Pulse Rifles, Sniper Rifles, Linear Fusion Rifles
                new ChampionWeaponType { ChampionId = antiBarrier.Id, WeaponType = WeaponType.ScoutRifle },
                new ChampionWeaponType { ChampionId = antiBarrier.Id, WeaponType = WeaponType.PulseRifle },
                new ChampionWeaponType { ChampionId = antiBarrier.Id, WeaponType = WeaponType.SniperRifle },
                new ChampionWeaponType { ChampionId = antiBarrier.Id, WeaponType = WeaponType.LinearFusionRifle },
                
                // Overload: Auto Rifles, SMGs, Machine Guns, Bows
                new ChampionWeaponType { ChampionId = overload.Id, WeaponType = WeaponType.AutoRifle },
                new ChampionWeaponType { ChampionId = overload.Id, WeaponType = WeaponType.SubmachineGun },
                new ChampionWeaponType { ChampionId = overload.Id, WeaponType = WeaponType.MachineGun },
                new ChampionWeaponType { ChampionId = overload.Id, WeaponType = WeaponType.Bow },
                
                // Unstoppable: Hand Cannons, Fusion Rifles, Shotguns, Rocket Launchers
                new ChampionWeaponType { ChampionId = unstoppable.Id, WeaponType = WeaponType.HandCannon },
                new ChampionWeaponType { ChampionId = unstoppable.Id, WeaponType = WeaponType.FusionRifle },
                new ChampionWeaponType { ChampionId = unstoppable.Id, WeaponType = WeaponType.Shotgun },
                new ChampionWeaponType { ChampionId = unstoppable.Id, WeaponType = WeaponType.RocketLauncher }
            };
            context.Set<ChampionWeaponType>().AddRange(championWeaponTypes);
            context.SaveChanges();
            Console.WriteLine("Champion weapon type mappings seeded successfully!");
        }

        // Seed Weapons if not exists
        if (!context.Weapons.Any())
        {
            var weapons = new List<Weapon>
            {
                // Auto Rifles
                new Weapon { Name = "Gnawing Hunger", Type = WeaponType.AutoRifle, Element = ElementType.Void, Slot = EquipSlot.Primary, AmmoType = AmmoType.Primary },
                new Weapon { Name = "Gallu RR3", Type = WeaponType.AutoRifle, Element = ElementType.Solar, Slot = EquipSlot.Primary, AmmoType = AmmoType.Primary },
                new Weapon { Name = "Reckless Oracle", Type = WeaponType.AutoRifle, Element = ElementType.Arc, Slot = EquipSlot.Primary, AmmoType = AmmoType.Primary },
                new Weapon { Name = "Duty Bound", Type = WeaponType.AutoRifle, Element = ElementType.Kinetic, Slot = EquipSlot.Primary, AmmoType = AmmoType.Primary },
                
                // Pulse Rifles
                new Weapon { Name = "Graviton Lance", Type = WeaponType.PulseRifle, Element = ElementType.Void, Slot = EquipSlot.Primary, AmmoType = AmmoType.Primary },
                new Weapon { Name = "Bygones", Type = WeaponType.PulseRifle, Element = ElementType.Arc, Slot = EquipSlot.Primary, AmmoType = AmmoType.Primary },
                new Weapon { Name = "Solar Pulse", Type = WeaponType.PulseRifle, Element = ElementType.Solar, Slot = EquipSlot.Primary, AmmoType = AmmoType.Primary },
                new Weapon { Name = "Gridskipper", Type = WeaponType.PulseRifle, Element = ElementType.Kinetic, Slot = EquipSlot.Primary, AmmoType = AmmoType.Primary },
                
                // Scout Rifles
                new Weapon { Name = "Hung Jury SR4", Type = WeaponType.ScoutRifle, Element = ElementType.Solar, Slot = EquipSlot.Primary, AmmoType = AmmoType.Primary },
                new Weapon { Name = "Contingency Plan", Type = WeaponType.ScoutRifle, Element = ElementType.Arc, Slot = EquipSlot.Primary, AmmoType = AmmoType.Primary },
                new Weapon { Name = "Night Watch", Type = WeaponType.ScoutRifle, Element = ElementType.Kinetic, Slot = EquipSlot.Primary, AmmoType = AmmoType.Primary },
                new Weapon { Name = "Eternal Blazon", Type = WeaponType.ScoutRifle, Element = ElementType.Void, Slot = EquipSlot.Primary, AmmoType = AmmoType.Primary },
                
                // Hand Cannons
                new Weapon { Name = "Palindrome", Type = WeaponType.HandCannon, Element = ElementType.Void, Slot = EquipSlot.Primary, AmmoType = AmmoType.Primary },
                new Weapon { Name = "Annual Skate", Type = WeaponType.HandCannon, Element = ElementType.Arc, Slot = EquipSlot.Primary, AmmoType = AmmoType.Primary },
                new Weapon { Name = "Sunshot", Type = WeaponType.HandCannon, Element = ElementType.Solar, Slot = EquipSlot.Primary, AmmoType = AmmoType.Primary },
                new Weapon { Name = "Austringer", Type = WeaponType.HandCannon, Element = ElementType.Kinetic, Slot = EquipSlot.Primary, AmmoType = AmmoType.Primary },
                
                // Submachine Guns
                new Weapon { Name = "Ikelos SMG", Type = WeaponType.SubmachineGun, Element = ElementType.Solar, Slot = EquipSlot.Primary, AmmoType = AmmoType.Primary },
                new Weapon { Name = "Calus Mini-Tool", Type = WeaponType.SubmachineGun, Element = ElementType.Solar, Slot = EquipSlot.Primary, AmmoType = AmmoType.Primary },
                new Weapon { Name = "Funnelweb", Type = WeaponType.SubmachineGun, Element = ElementType.Void, Slot = EquipSlot.Primary, AmmoType = AmmoType.Primary },
                new Weapon { Name = "Taraxippos", Type = WeaponType.SubmachineGun, Element = ElementType.Kinetic, Slot = EquipSlot.Primary, AmmoType = AmmoType.Primary },
                
                // Sniper Rifles
                new Weapon { Name = "Frozen Orbit", Type = WeaponType.SniperRifle, Element = ElementType.Solar, Slot = EquipSlot.Special, AmmoType = AmmoType.Special },
                new Weapon { Name = "Uzume RR4", Type = WeaponType.SniperRifle, Element = ElementType.Arc, Slot = EquipSlot.Special, AmmoType = AmmoType.Special },
                new Weapon { Name = "Ikelos SR", Type = WeaponType.SniperRifle, Element = ElementType.Void, Slot = EquipSlot.Special, AmmoType = AmmoType.Special },
                new Weapon { Name = "Eye of Sol", Type = WeaponType.SniperRifle, Element = ElementType.Kinetic, Slot = EquipSlot.Special, AmmoType = AmmoType.Special },
                
                // Shotguns
                new Weapon { Name = "Seventh Seraph CQC", Type = WeaponType.Shotgun, Element = ElementType.Solar, Slot = EquipSlot.Special, AmmoType = AmmoType.Special },
                new Weapon { Name = "Without Remorse", Type = WeaponType.Shotgun, Element = ElementType.Arc, Slot = EquipSlot.Special, AmmoType = AmmoType.Special },
                new Weapon { Name = "Fractethyst", Type = WeaponType.Shotgun, Element = ElementType.Void, Slot = EquipSlot.Special, AmmoType = AmmoType.Special },
                new Weapon { Name = "Found Verdict", Type = WeaponType.Shotgun, Element = ElementType.Kinetic, Slot = EquipSlot.Special, AmmoType = AmmoType.Special },
                
                // Fusion Rifles
                new Weapon { Name = "Plug One.1", Type = WeaponType.FusionRifle, Element = ElementType.Arc, Slot = EquipSlot.Special, AmmoType = AmmoType.Special },
                new Weapon { Name = "Epicurean", Type = WeaponType.FusionRifle, Element = ElementType.Void, Slot = EquipSlot.Special, AmmoType = AmmoType.Special },
                new Weapon { Name = "Midha's Reckoning", Type = WeaponType.FusionRifle, Element = ElementType.Solar, Slot = EquipSlot.Special, AmmoType = AmmoType.Special },
                new Weapon { Name = "Riptide", Type = WeaponType.FusionRifle, Element = ElementType.Kinetic, Slot = EquipSlot.Special, AmmoType = AmmoType.Special },
                
                // Bows
                new Weapon { Name = "Tyranny of Heaven", Type = WeaponType.Bow, Element = ElementType.Arc, Slot = EquipSlot.Primary, AmmoType = AmmoType.Primary },
                new Weapon { Name = "Biting Winds", Type = WeaponType.Bow, Element = ElementType.Kinetic, Slot = EquipSlot.Primary, AmmoType = AmmoType.Primary },
                new Weapon { Name = "Accrued Redemption", Type = WeaponType.Bow, Element = ElementType.Void, Slot = EquipSlot.Primary, AmmoType = AmmoType.Primary },
                new Weapon { Name = "Imperial Needle", Type = WeaponType.Bow, Element = ElementType.Solar, Slot = EquipSlot.Primary, AmmoType = AmmoType.Primary },
                
                // Machine Guns (Heavy, no Kinetic)
                new Weapon { Name = "Commemoration", Type = WeaponType.MachineGun, Element = ElementType.Void, Slot = EquipSlot.Heavy, AmmoType = AmmoType.Heavy },
                new Weapon { Name = "The Title", Type = WeaponType.MachineGun, Element = ElementType.Arc, Slot = EquipSlot.Heavy, AmmoType = AmmoType.Heavy },
                new Weapon { Name = "Fixed Odds", Type = WeaponType.MachineGun, Element = ElementType.Solar, Slot = EquipSlot.Heavy, AmmoType = AmmoType.Heavy },
                
                // Rocket Launchers (Heavy, no Kinetic)
                new Weapon { Name = "Ascendancy", Type = WeaponType.RocketLauncher, Element = ElementType.Arc, Slot = EquipSlot.Heavy, AmmoType = AmmoType.Heavy },
                new Weapon { Name = "Palmyra-B", Type = WeaponType.RocketLauncher, Element = ElementType.Void, Slot = EquipSlot.Heavy, AmmoType = AmmoType.Heavy },
                new Weapon { Name = "Hothead", Type = WeaponType.RocketLauncher, Element = ElementType.Solar, Slot = EquipSlot.Heavy, AmmoType = AmmoType.Heavy },
                
                // Grenade Launchers (Heavy)
                new Weapon { Name = "Explosive Personality", Type = WeaponType.GrenadeGauncher, Element = ElementType.Solar, Slot = EquipSlot.Heavy, AmmoType = AmmoType.Heavy },
                new Weapon { Name = "Edge of Concurrence", Type = WeaponType.GrenadeGauncher, Element = ElementType.Arc, Slot = EquipSlot.Heavy, AmmoType = AmmoType.Heavy },
                new Weapon { Name = "Deafening Whisper", Type = WeaponType.GrenadeGauncher, Element = ElementType.Void, Slot = EquipSlot.Heavy, AmmoType = AmmoType.Heavy },
                
                // Linear Fusion Rifles (Heavy)
                new Weapon { Name = "Tarantula", Type = WeaponType.LinearFusionRifle, Element = ElementType.Arc, Slot = EquipSlot.Heavy, AmmoType = AmmoType.Heavy },
                new Weapon { Name = "Threaded Needle", Type = WeaponType.LinearFusionRifle, Element = ElementType.Void, Slot = EquipSlot.Heavy, AmmoType = AmmoType.Heavy },
                new Weapon { Name = "Stormchaser", Type = WeaponType.LinearFusionRifle, Element = ElementType.Solar, Slot = EquipSlot.Heavy, AmmoType = AmmoType.Heavy },
                
                // Swords (Heavy, no Kinetic)
                new Weapon { Name = "Falling Guillotine", Type = WeaponType.Sword, Element = ElementType.Void, Slot = EquipSlot.Heavy, AmmoType = AmmoType.Heavy },
                new Weapon { Name = "Crown-Splitter", Type = WeaponType.Sword, Element = ElementType.Arc, Slot = EquipSlot.Heavy, AmmoType = AmmoType.Heavy },
                new Weapon { Name = "The Other Half", Type = WeaponType.Sword, Element = ElementType.Solar, Slot = EquipSlot.Heavy, AmmoType = AmmoType.Heavy }
            };
            context.Weapons.AddRange(weapons);
            context.SaveChanges();
            Console.WriteLine("Weapons seeded successfully!");
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

var iconPath = Path.Combine(builder.Environment.ContentRootPath, "Icons");
if (Directory.Exists(iconPath))
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(iconPath),
        RequestPath = "/icons"
    });
}

app.UseSession();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
