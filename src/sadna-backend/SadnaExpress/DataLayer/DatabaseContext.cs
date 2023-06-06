using Microsoft.EntityFrameworkCore;
using SadnaExpress.DomainLayer;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.DomainLayer.Store.Policy;
using SadnaExpress.DomainLayer.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaExpress.DataLayer
{

    public class DatabaseContext : DbContext
    {
        public DbSet<Item> Items { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Order> orders { get; set; }
        public DbSet<ItemForOrder> ItemForOrders { get; set; }
        public DbSet<Member> members { get; set; }
        public DbSet<User> users { get; set; }
        public DbSet<PromotedMember> promotedMembers { get; set; }
        public DbSet<Macs> macs { get; set; }
        public DbSet<ShoppingCart> shoppingCarts { get; set; }
        public DbSet<ShoppingBasket> shoppingBaskets { get; set; }
        public DbSet<Bid> bids { get; set; }
        public DbSet<InitializeSystem> initializeSystems { get; set; }

        public DbSet<Notification> notfications { get; set; }
        public DbSet<ConditionDB> conditions { get; set; }

        public DbSet<PolicyDB> policies { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseMySql("Server=MYSQL5045.site4now.net;Database=db_a995b0_sadnadb;Uid=a995b0_sadnadb;Pwd=Sadna123");
            
            optionsBuilder.UseSqlServer("Server=SQL5106.site4now.net;Database=db_a995b0_sadnadbver4;Uid=db_a995b0_sadnadbver4_admin;Pwd=Sadna123");
            
            //Data Source=;Initial Catalog=;User Id=;Password=YOUR_DB_PASSWORD
            //optionsBuilder.UseSqlite("Data Source=database.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // required to connect between user -> shoppingCart -> Basket
            modelBuilder.Entity<ShoppingCart>(entity =>
            {
                entity.HasKey(e => e.ShoppingCartId);
                entity.HasMany(e => e.Baskets)
                    .WithOne()
                    .HasForeignKey("ShoppingCartId")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // required to connect between store and inventory
            modelBuilder.Entity<Store>(entity =>
            {
                entity.HasKey(e => e.StoreID);
                entity.HasOne(e => e.itemsInventory);
            });


            modelBuilder.Entity<ConditionDB>()
                .Property(e => e.ID)
                .ValueGeneratedNever();

            modelBuilder.Entity<PolicyDB>()
                .Property(p => p.complex_policys)
                .HasConversion(
                v => string.Join(",", v),
                v => v.Split(',', (char)StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse).ToArray());


            base.OnModelCreating(modelBuilder);
        }

        public DatabaseContext() : base()
        {
            Database.EnsureCreated();
        }
    }


    public class DatabaseTestsContext : DatabaseContext
    {

        #region Copy of DatabaseContext fields

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseMySql("Server=MYSQL5045.site4now.net;Database=db_a995b0_sadnadb;Uid=a995b0_sadnadb;Pwd=Sadna123");
            //optionsBuilder.UseSqlServer("Server=SQL5110.site4now.net;Database=db_a995b0_demo2;Uid=db_a995b0_demo2_admin;Pwd=Sadna123");

            optionsBuilder.UseSqlite("Data Source=databaseTests.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        #endregion

        public DatabaseTestsContext()
        {
            Database.EnsureCreated();
        }
    }

    public static class DatabaseContextFactory
    {
        public static bool TestMode = false;
        public static DatabaseContext ConnectToDatabase()
        {
            if (!TestMode)
                return new DatabaseContext();
            else
                return new DatabaseTestsContext();
        }
    }
}
