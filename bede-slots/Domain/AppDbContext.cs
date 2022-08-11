using bede_slots.Models;
using Microsoft.EntityFrameworkCore;

namespace bede_slots.Domain
{
    public interface IAppDbContext
    {
        DbSet<SlotItem> SlotItems { get; set; }
        DbSet<Player> Player { get; set; }

        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        Task<int> SaveChangesAsync();
    }

    public class AppDbContext : DbContext, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<SlotItem> SlotItems { get; set; }
        public DbSet<Player> Player { get; set; }

        //may not require
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
        }

        public Task<int> SaveChangesAsync()
        {
            return base.SaveChangesAsync();
        }

        public virtual new DbSet<TEntity> Set<TEntity>() where TEntity : class { return base.Set<TEntity>(); }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // create object 

            //modelbuilder.Entity<class>().HasData(objec)

            // may not need

            List<SlotItem> slotItemSeedData = new List<SlotItem>();

            slotItemSeedData.Add(new SlotItem(){Id = 1 ,Name = "Apple", Symbol = 'A', Coefficent = 0.4m, ProbabilityOfAppearance = 45 });
            slotItemSeedData.Add(new SlotItem(){Id = 2 ,Name = "Banana", Symbol = 'B', Coefficent = 0.6m, ProbabilityOfAppearance = 35 });
            slotItemSeedData.Add(new SlotItem(){Id = 3 ,Name = "Pineapple", Symbol = 'C', Coefficent = 0.8m, ProbabilityOfAppearance = 15 });
            slotItemSeedData.Add(new SlotItem(){Id = 4 ,Name = "Wildcard", Symbol = '*', Coefficent = 0.0m, ProbabilityOfAppearance = 5 });

            modelBuilder.Entity<Player>().HasData(new Player(){Id = 1});

            modelBuilder.Entity<SlotItem>().HasData(slotItemSeedData);

            

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }

    }
}
