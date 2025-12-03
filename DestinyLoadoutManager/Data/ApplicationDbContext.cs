using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DestinyLoadoutManager.Models;

namespace DestinyLoadoutManager.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Weapon> Weapons { get; set; }
        public DbSet<Loadout> Loadouts { get; set; }
        public DbSet<LoadoutWeapon> LoadoutWeapons { get; set; }
        public DbSet<Champion> Champions { get; set; }
        public DbSet<ChampionWeaponType> ChampionWeaponTypes { get; set; }
        public DbSet<Surge> Surges { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Weapon tábla
            modelBuilder.Entity<Weapon>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Type).IsRequired();
                entity.Property(e => e.Element).IsRequired();
                entity.Property(e => e.Slot).IsRequired();
                entity.Property(e => e.AmmoType).IsRequired();
            });

            // Loadout tábla
            modelBuilder.Entity<Loadout>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(e => e.User)
                    .WithMany(u => u.Loadouts)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // LoadoutWeapon tábla (N:M kapcsolat)
            modelBuilder.Entity<LoadoutWeapon>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Slot).IsRequired();

                entity.HasOne(e => e.Loadout)
                    .WithMany(l => l.LoadoutWeapons)
                    .HasForeignKey(e => e.LoadoutId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Weapon)
                    .WithMany(w => w.LoadoutWeapons)
                    .HasForeignKey(e => e.WeaponId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Champion tábla
            modelBuilder.Entity<Champion>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
            });

            // ChampionWeaponType tábla
            modelBuilder.Entity<ChampionWeaponType>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.WeaponType).IsRequired();

                entity.HasOne(e => e.Champion)
                    .WithMany(c => c.ChampionWeaponTypes)
                    .HasForeignKey(e => e.ChampionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Surge tábla
            modelBuilder.Entity<Surge>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
            });
        }
    }
}
