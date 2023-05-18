using Microsoft.EntityFrameworkCore;
using SadnaExpress.DomainLayer.Store;
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


        //public DbSet<Condition> conditions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseMySql("Server=MYSQL5045.site4now.net;Database=db_a995b0_sadnadb;Uid=a995b0_sadnadb;Pwd=Sadna123");
            //optionsBuilder.UseSqlServer("Server=SQL5110.site4now.net;Database=db_a995b0_sadnadb;Uid=db_a995b0_sadnadb_admin;Pwd=Sadna123");
            optionsBuilder.UseSqlite("Data Source=database.db");
        }

        public DatabaseContext() : base()
        {
            Database.EnsureCreated();
        }

    }
}
