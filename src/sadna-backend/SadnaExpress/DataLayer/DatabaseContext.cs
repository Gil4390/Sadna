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
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
using Microsoft.Data.SqlClient;
using SadnaExpress.DataLayer;

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
        public DbSet<Visit> visits { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connection = System.Configuration.ConfigurationManager.ConnectionStrings["MasterConnectionString"].ConnectionString;
            optionsBuilder.UseSqlServer(connection);
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

        public static bool onFunc = false;
        public static bool CanConnect()
        {
            try
            {
                var connection = System.Configuration.ConfigurationManager.ConnectionStrings["MasterConnectionString"].ConnectionString;
                using (var con = new SqlConnection(connection))
                {
                    con.Open();
                    con.Close();
                    DBHandler.Instance.internetRunning = true;
                    return true;
                }
            }
            catch (Exception)
            {
                DBHandler.Instance.internetRunning = false;
                return false;
            }
        }
        public DatabaseContext() : base()
        {

            if (onFunc)
            {
                // we deal with internet connection using retrying policy
                // simply means when internet connection is back we update the db as neccessary
                DBHandler.interntConnectionIsBack1 = false;
                while (!DBHandler.interntConnectionIsBack1)
                {
                    DBHandler.interntConnectionIsBack1 = CanConnect();
                }
            }
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            lock (this)
            {
                DBHandler.interntConnectionIsBack2 = false;

                try
                {
                    var result = base.SaveChanges(acceptAllChangesOnSuccess);
                    DBHandler.interntConnectionIsBack2 = true;
                    return result;
                }
                catch (Exception ex)
                {

                    // we deal with this bug using retrying policy
                    // simply means when internet connection is back we update the db as neccessary
                    while (!DBHandler.interntConnectionIsBack2)
                    {

                        DBHandler.interntConnectionIsBack2 = CanConnect();
                        var result = 0;
                        if (DBHandler.interntConnectionIsBack2)
                        {
                            try
                            {
                                result = base.SaveChanges(acceptAllChangesOnSuccess);
                            }
                            catch (Exception ex2)
                            {

                            }
                            return result;
                        }
                    }
                    DBHandler.interntConnectionIsBack2 = true;
                    throw new Exception(ex.Message);
                }
            }
        }
    }

}


    public class DatabaseTestsContext : DatabaseContext
    {

        #region Copy of DatabaseContext fields

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source = databaseTests.db");
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
