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
                        .HasOne(p => p.ChampionObj)
                        .WithMany(b => b.ChampionSpells)
                        .HasForeignKey(p => p.Champion);


            modelBuilder.Entity<Champion>()
                        .HasOne(b => b.ChampionPassive)
                        .WithOne(i => i.ChampionObj)
                        .HasForeignKey<ChampionPassive>(b => b.Champion);
        }

        public ChampionDbContext(DbContextOptions<ChampionDbContext> options) : base(options)
        {
        }
    }
}
