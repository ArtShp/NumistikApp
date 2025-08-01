using Microsoft.EntityFrameworkCore;
using Server.Entities;

namespace Server.Data;

public partial class MyDbContext(DbContextOptions<MyDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<InviteToken> InviteTokens { get; set; }
    public DbSet<Collection> Collections { get; set; }
    public DbSet<UserCollection> UserCollections { get; set; }
    public DbSet<Continent> Continents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<InviteToken>()
            .Property(t => t.Token)
            .HasDefaultValueSql("gen_random_uuid()");
    }
}
