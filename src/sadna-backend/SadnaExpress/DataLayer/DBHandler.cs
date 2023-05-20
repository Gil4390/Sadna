using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.DomainLayer.User;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Http.Results;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using Item = SadnaExpress.DomainLayer.Store.Item;
using SadnaExpress.ServiceLayer.SModels;

namespace SadnaExpress.DataLayer
{
    public class DBHandler
    {
        private static readonly object databaseLock = new object();

        private static DBHandler instance = null;

        public static DBHandler Instance
        {
            get
            {
                lock (databaseLock)
                {
                    if (instance == null)
                    {
                        instance = new DBHandler();
                    }
                    return instance;
                }
            }
        }

        public DBHandler()
        {

        }

        public void TestMode()
        {
            

            lock (this)
            {
                try
                {
                    using (var db = new DatabaseContext())
                    {
                        try
                        {
                            // local databases test mode
                            db.Database.EnsureDeleted();
                            db.Database.EnsureCreated();

                            // external database test mode
                            // cleaning all tables rows
                            //db.shoppingBaskets.RemoveRange(db.shoppingBaskets);
                            //db.shoppingCarts.RemoveRange(db.shoppingCarts);
                            //db.users.RemoveRange(db.users);
                            //db.members.RemoveRange(db.members);
                            //db.promotedMembers.RemoveRange(db.promotedMembers);
                            //db.macs.RemoveRange(db.macs);
                            //db.Stores.RemoveRange(db.Stores);
                            //db.Inventories.RemoveRange(db.Inventories);
                            //db.Items.RemoveRange(db.Items);


                            db.SaveChanges(true);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("setting up database for tests failed!");
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to Connect With Database");
                }
            }
        }

        // user / member / promotedmember database functions
        
        public bool memberExistsByEmail(string email)
        {
            bool result = false;

            lock (this)
            {
                try
                {
                    using (var db = new DatabaseContext())
                    {
                        try
                        {
                            result = db.members.FirstOrDefault(m => m.Email.ToLower().Equals(email.ToLower())) != null;
                            result = result || db.promotedMembers.FirstOrDefault(m => m.Email.ToLower().Equals(email.ToLower())) != null;
                        }
                        catch (Exception ex)
                        {
                            //throw new Exception("failed to interact with members table");
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to Connect With Database");
                }

                return result;
            }
        }


        public bool memberExistsById(Guid id)
        {
            bool result = false;

            lock (this)
            {
                try
                {
                    using (var db = new DatabaseContext())
                    {
                        try
                        {
                            result = db.members.FirstOrDefault(m => m.UserId.Equals(id)) != null;
                        }
                        catch (Exception ex)
                        {
                            //throw new Exception("failed to interact with members table");
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to Connect With Database");
                }
                return result;
            }
        }

        public void AddMember(Member newMember, string newMac)
        {
            lock (this)
            {
                try
                {
                    using (var db = new DatabaseContext())
                    {
                        try
                        {
                            var memberss = db.members;
                            newMember.ShoppingCart.UserId = newMember.UserId; // update shopping cart id
                            foreach (ShoppingBasket sb in newMember.ShoppingCart.Baskets)
                            {
                                sb.ShoppingCartId = newMember.ShoppingCart.ShoppingCartId;
                            }
                            memberss.Add(newMember);
                            var macs = db.macs;
                            macs.Add(new Macs { id = newMember.UserId, mac = newMac });

                            var shoppingCarts = db.shoppingCarts;
                            
                            db.SaveChanges(true);
                        }
                        catch (Exception ex)
                        {
                            //throw new Exception("failed to interact with members table");
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to Connect With Database");
                }
            }
        }

        public Member CheckMemberValidLogin(Guid id, string email, string password, IPasswordHash _ph)
        {
            Member result = null;
            lock (this)
            {
                try
                {
                    using (var db = new DatabaseContext())
                    {
                        try
                        {
                            var memberExist = db.members.FirstOrDefault(m=>m.Email.ToLower().Equals(email.ToLower()));
                            if (memberExist != null)
                            {
                                if (!memberExist.Discriminator.Equals("Member"))
                                    memberExist = db.promotedMembers.FirstOrDefault(m => m.Email.ToLower().Equals(email.ToLower()));
                                if (!memberExist.LoggedIn || true) // remove || true later
                                {
                                    memberExist.LoggedIn = false; // remove this line later 
                                    Guid id1 = memberExist.UserId;
                                    string mac1 = db.macs.Find(id1).mac;

                                    bool memberCorrectDetails = _ph.Rehash(password + mac1, memberExist.Password);

                                    if (memberCorrectDetails)
                                    {
                                        result = memberExist;
                                    }
                                    // now get member cart from DB
                                    result.ShoppingCart = db.shoppingCarts.FirstOrDefault(m => m.UserId.Equals(memberExist.UserId));
                                    var userBaskets = db.shoppingBaskets.Where(basket => basket.ShoppingCartId.Equals(memberExist.ShoppingCart.ShoppingCartId)).ToList();
                                    foreach(var basket in userBaskets)
                                    {
                                        result.ShoppingCart.Baskets.Add(basket);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            //throw new Exception("failed to interact with members table");
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to Connect With Database");
                }
                return result;
            }
        }

        public string GetMacById(Guid userId)
        {
            string result = null;
            lock (this)
            {
                try
                {
                    using (var db = new DatabaseContext())
                    {
                        try
                        {
                            var macFound = db.macs.FirstOrDefault(m => m.id.Equals(userId));
                            if (macFound != null)
                                result = macFound.mac;
                        }
                        catch (Exception ex)
                        {
                            //throw new Exception("failed to interact with members table");
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to Connect With Database");
                }
                return result;
            }
        }

        // store Database Functions
        public bool IsStoreNameExist(string storeName)
        {
            bool result = false;

            lock (this)
            {
                try
                {
                    using (var db = new DatabaseContext())
                    {
                        try
                        {
                            result = db.Stores.FirstOrDefault(m => m.StoreName.ToLower().Equals(storeName.ToLower())) != null;
                        }
                        catch (Exception ex)
                        {
                            //throw new Exception("failed to interact with stores table");
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to Connect With Database");
                }
                return result;
            }
        }

        public void AddStore(Store store)
        {
            lock (this)
            {
                try
                {
                    using (var db = new DatabaseContext())
                    {
                        try
                        {
                            //var Inventories = db.Inventories;
                            //Inventories.Add(store.itemsInventory);
                            //db.SaveChanges(true);

                            var stores = db.Stores;
                            store.itemsInventory.StoreID = store.StoreID;
                            stores.Add(store);
                            db.SaveChanges(true);
                        }
                        catch (Exception ex)
                        {
                            //throw new Exception("failed to interact with stores table");
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to Connect With Database");
                }
            }
        }

        public void UpdatePromotedMember(PromotedMember pm)
        {
            lock (this)
            {
                try
                {
                    using (var db = new DatabaseContext())
                    {
                        try
                        {
                            var proMemberExist = db.promotedMembers.FirstOrDefault(m => m.UserId.Equals(pm.UserId));
                            //memberExist = founder;
                            db.promotedMembers.Remove(proMemberExist);
                            db.SaveChanges(true);

                            db.promotedMembers.Add(pm);
                            //db.promotedMembers.Update(pm);
                            db.SaveChanges(true);
                        }
                        catch (Exception ex)
                        {
                            //throw new Exception("failed to interact with members table");
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to Connect With Database");
                }
            }
        }

        public bool IsStoreExist(Guid storeID)
        {
            bool result = false;

            lock (this)
            {
                try
                {
                    using (var db = new DatabaseContext())
                    {
                        try
                        {
                            result = db.Stores.FirstOrDefault(m => m.StoreID.Equals(storeID)) != null;
                        }
                        catch (Exception ex)
                        {
                            //throw new Exception("failed to interact with stores table");
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to Connect With Database");
                }
                return result;
            }
        }

        public Store GetStoreById(Guid storeID)
        {
            Store result = null;

            lock (this)
            {
                try
                {
                    using (var db = new DatabaseContext())
                    {
                        try
                        {
                            result = db.Stores.FirstOrDefault(m => m.StoreID.Equals(storeID));
                        }
                        catch (Exception ex)
                        {
                            //throw new Exception("failed to interact with stores table");
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to Connect With Database");
                }
                return result;
            }
        }

        public ConcurrentDictionary<Guid, Store> GetAllStores()
        {
            ConcurrentDictionary<Guid, Store> result = null;

            lock (this)
            {
                try
                {
                    using (var db = new DatabaseContext())
                    {
                        try
                        {
                            var allStores = db.Stores;
                            result = new ConcurrentDictionary<Guid, Store>();
                            foreach (Store s in allStores)
                            {
                                // get store item_quantity
                                string quanity_ItemsDB = db.Inventories.FirstOrDefault(m => m.StoreID.Equals(s.StoreID)).Items_quantityDB;

                                if (quanity_ItemsDB != null) // add quantity item for store
                                {
                                    ConcurrentDictionary<Guid, int> items_quantityHelper = JsonConvert.DeserializeObject<ConcurrentDictionary<Guid, int>>(quanity_ItemsDB);

                                    foreach (Guid id in items_quantityHelper.Keys)
                                    {
                                        Item item = db.Items.FirstOrDefault(m => m.ItemID.Equals(id));
                                        s.itemsInventory.items_quantity.TryAdd(item, items_quantityHelper[id]);
                                    }
                                }
                                result.TryAdd(s.StoreID, s);
                            }
                        }
                        catch (Exception ex)
                        {
                            //throw new Exception("failed to interact with stores table");
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to Connect With Database");
                }
                return result;
            }
        }

        public void AddItemToStore(Store store)
        {
            lock (this)
            {
                try
                {
                    using (var db = new DatabaseContext())
                    {
                        try
                        {
                            
                            var storeFound = db.Stores.FirstOrDefault(m => m.StoreID.Equals(store.StoreID));

                            var invetories = db.Inventories;
                            var inventoryFound = db.Inventories.FirstOrDefault(m => m.StoreID.Equals(storeFound.StoreID));
                            invetories.Remove(inventoryFound);
                            db.SaveChanges(true);

                            var stores = db.Stores;
                            stores.Remove(storeFound);
                            db.SaveChanges(true);


                            ConcurrentDictionary<Guid, int> helper = new ConcurrentDictionary<Guid, int>();
                            foreach (Item it in store.itemsInventory.items_quantity.Keys)
                            {
                                helper.TryAdd(it.ItemID, store.itemsInventory.items_quantity[it]);
                            }

                            string jsonItemQuantity = JsonConvert.SerializeObject(helper);

                            store.itemsInventory.Items_quantityDB = jsonItemQuantity;
                            stores.Add(store);
                            db.SaveChanges(true);

                        }
                        catch (Exception ex)
                        {
                            //throw new Exception("failed to interact with stores table");
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to Connect With Database");
                }
            }
        }

        internal void AddItem(DomainLayer.Store.Item newItem)
        {
            lock (this)
            {
                try
                {
                    using (var db = new DatabaseContext())
                    {
                        try
                        {

                            var item = db.Items.FirstOrDefault(m => m.ItemID.Equals(newItem.ItemID));

                            var items = db.Items;
                            if (item == null)
                            {
                                
                                items.Add(newItem);
                            }
                            else
                            {
                                items.Remove(item);
                                db.SaveChanges(true);
                                items.Add(newItem);
                            }

                            db.SaveChanges(true);
                        }
                        catch (Exception ex)
                        {
                            //throw new Exception("failed to interact with stores table");
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to Connect With Database");
                }
            }
        }

        public void UpdateMemberShoppingCart(Member member)
        {
            lock (this)
            {
                try
                {
                    using (var db = new DatabaseContext())
                    {
                        try
                        {
                            var userBasket = db.shoppingBaskets.Where(basket=>basket.ShoppingCartId.Equals(member.ShoppingCart.ShoppingCartId)).ToList();
                            if(userBasket!= null && userBasket.Count > 0)
                            {
                                db.shoppingBaskets.RemoveRange(userBasket);
                                
                                db.SaveChanges(true);
                            }

                            var baskets = db.shoppingBaskets;
                            foreach (ShoppingBasket bs in member.ShoppingCart.Baskets)
                            {
                                bs.ShoppingCartId = member.ShoppingCart.ShoppingCartId;
                                baskets.Add(bs);
                                db.SaveChanges(true);
                            }

                            
                        }
                        catch (Exception ex)
                        {
                            //throw new Exception("failed to interact with stores table");
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to Connect With Database");
                }
            }
        }

        public void MemberLogout(Member member)
        {
            lock (this)
            {
                try
                {
                    using (var db = new DatabaseContext())
                    {
                        try
                        {
                            var memberExist = db.members.FirstOrDefault(m => m.Email.ToLower().Equals(member.Email.ToLower()));
                            if (memberExist != null)
                            {
                                if (!memberExist.Discriminator.Equals("Member"))
                                {
                                    memberExist = db.promotedMembers.FirstOrDefault(m => m.Email.ToLower().Equals(member.Email.ToLower()));
                                    memberExist.LoggedIn = false;
                                    db.SaveChanges();
                                }
                                memberExist.LoggedIn = false;
                                db.SaveChanges();
                            }
                        }
                        catch (Exception ex)
                        {
                            //throw new Exception("failed to interact with stores table");
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to Connect With Database");
                }
            }
        }

        public void EditItemPrice(Guid itemID, int price)
        {
            lock (this)
            {
                try
                {
                    using (var db = new DatabaseContext())
                    {
                        try
                        {
                            var itemExist = db.Items.FirstOrDefault(m => m.ItemID.Equals(itemID));
                            if (itemExist != null)
                            {
                                itemExist.Price = price;
                                db.SaveChanges();
                            }
                        }
                        catch (Exception ex)
                        {
                            //throw new Exception("failed to interact with stores table");
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to Connect With Database");
                }
            }
        }
    }
}
