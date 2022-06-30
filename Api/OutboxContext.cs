using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Api
{
  public class OutboxContext : DbContext
  {

    public OutboxContext(DbContextOptions<OutboxContext> options)
    : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);
      modelBuilder.AddInboxStateEntity();
      modelBuilder.AddOutboxMessageEntity();
      modelBuilder.AddOutboxStateEntity();

      modelBuilder.Entity<Test>().HasKey(x => x.Id);
      modelBuilder.Entity<Test>().ToTable("Tests");
    }
  }
}
