namespace WildRiftWebAPI;

public class WildRiftDbContext : DbContext
{
    public DbSet<Item> Items { get; set; }
    public DbSet<Rune> Runes { get; set; }
    public DbSet<Champion> Champions { get; set; }
    public DbSet<ChampionPassive> Champions_Passives { get; set; }
    public DbSet<ChampionSpell> Champions_Spells { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Champion>().HasKey(c => c.Name);

        modelBuilder.Entity<Item>().HasKey(c => c.Name);

        modelBuilder.Entity<Rune>().HasKey(c => c.Name);

        modelBuilder.Entity<User>().HasKey(c => c.Username);

        modelBuilder.Entity<ChampionSpell>()
                    .HasOne(p => p.ChampionObj)
                    .WithMany(b => b.ChampionSpells)
                    .HasForeignKey(p => p.Champion);

        modelBuilder.Entity<Champion>()
                    .HasOne(b => b.ChampionPassive)
                    .WithOne(i => i.ChampionObj)
                    .HasForeignKey<ChampionPassive>(b => b.Champion);

        modelBuilder.Entity<User>()
                    .HasOne(b => b.Role)
                    .WithOne(i => i.User)
                    .HasForeignKey<User>(b => b.Role_id);
    }

    public WildRiftDbContext(DbContextOptions<WildRiftDbContext> options) : base(options)
    {
    }
}
