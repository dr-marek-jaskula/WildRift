using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;

namespace WildRiftWebAPI
{
    public class ChampionDbContext : DbContext
    {
        public DbSet<Champion> Champions { get; set; }
        public DbSet<ChampionPassive> Champions_Passives { get; set; }
        public DbSet<ChampionSpell> Champions_Spells { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Champion>().HasKey(c => c.Name);

            modelBuilder.Entity<ChampionSpell>()
                        .HasOne(p => p.Champion)
                        .WithMany(b => b.ChampionSpell)
                        .HasForeignKey(p => p.Id);

            modelBuilder.Entity<ChampionPassive>()
                        .HasOne(p => p.Champion)
                        .WithOne(b => b.ChampionPassive);
        }

        public ChampionDbContext(DbContextOptions<ChampionDbContext> options) : base(options)
        {

        }
    }
}
