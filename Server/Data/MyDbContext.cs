using Microsoft.EntityFrameworkCore;
using Server.Entities;

namespace Server.Data;

public partial class MyDbContext(DbContextOptions<MyDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<InviteToken> InviteTokens { get; set; }
}
