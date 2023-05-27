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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Header;
using System.Diagnostics;
using SadnaExpress.ServiceLayer.Obj;

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

        public void CleanDB()
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
                            //db.Database.EnsureDeleted();
                            //db.Database.EnsureCreated();

                            // external database test mode
                            // cleaning all tables rows
                            db.shoppingBaskets.RemoveRange(db.shoppingBaskets);
                            db.shoppingCarts.RemoveRange(db.shoppingCarts);
                            db.users.RemoveRange(db.users);
                            db.members.RemoveRange(db.members);
                            db.promotedMembers.RemoveRange(db.promotedMembers);
                            db.macs.RemoveRange(db.macs);
                            db.Stores.RemoveRange(db.Stores);
                            db.Inventories.RemoveRange(db.Inventories);
                            db.Items.RemoveRange(db.Items);
                            db.bids.RemoveRange(db.bids);
                            db.initializeSystems.RemoveRange(db.initializeSystems);


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
                            
                            var macs = db.macs;
                            macs.Add(new Macs { id = newMember.UserId, mac = newMac });
                            db.SaveChanges(true);

                            newMember.BidsDB = newMember.BidsJson;
                            memberss.Add(newMember);

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
                                string mac1 = db.macs.Find(memberExist.UserId).mac;
                                
                                bool memberCorrectDetails = _ph.Rehash(password + mac1, memberExist.Password);

                                if (memberCorrectDetails)
                                {
                                    result = memberExist;
                                    memberExist.LoggedIn = true;
                                    db.SaveChanges();
                                }
                                else
                                {
                                    throw new Exception("Incorrect email or password");
                                }

                                if (memberExist.Discriminator.Equals("PromotedMember"))
                                {
                                    // *************** need to fix here
                                    // error 1 when getting this DirectSupervisor Values *************************
                                    var PromotedmemberExist = db.promotedMembers.FirstOrDefault(m => m.Email.ToLower().Equals(email.ToLower()));

                                    // todo: now load DirecrSupervisor from database
                                    ConcurrentDictionary<Guid, PromotedMember> loadedDirect = new ConcurrentDictionary<Guid, PromotedMember>();
                                    ConcurrentDictionary<Guid, string> directs = JsonConvert.DeserializeObject<ConcurrentDictionary<Guid, string>>(PromotedmemberExist.DirectSupervisorDB);
                                    foreach (Guid storeId in directs.Keys)
                                    {
                                        foreach (Guid id1 in directs.Keys)
                                        {
                                            var promotedMemberInDictionary = db.promotedMembers.FirstOrDefault(pm => pm.Email.ToLower().Equals(directs[id1].ToLower()));
                                            if (promotedMemberInDictionary != null)
                                                loadedDirect.TryAdd(storeId, promotedMemberInDictionary);
                                            else
                                                loadedDirect.TryAdd(storeId, null);
                                        }
                                    }
                                    PromotedmemberExist.DirectSupervisor = loadedDirect;

                                    // todo: now load appoint from database
                                    ConcurrentDictionary<Guid, List<PromotedMember>> loadedAppoint = new ConcurrentDictionary<Guid, List<PromotedMember>>();
                                    ConcurrentDictionary<Guid, List<string>> loadeds = JsonConvert.DeserializeObject<ConcurrentDictionary<Guid, List<string>>>(PromotedmemberExist.AppointDB);
                                    foreach (Guid storeId in loadeds.Keys)
                                    {
                                        List<string> listOfProMembers = loadeds[storeId];
                                        List<PromotedMember> list = new List<PromotedMember>();
                                        foreach(string pmEmail in listOfProMembers)
                                        {
                                            var promotedMemberInDictionary = db.promotedMembers.FirstOrDefault(pm => pm.Email.ToLower().Equals(pmEmail.ToLower()));
                                            if (promotedMemberInDictionary != null)
                                                list.Add(promotedMemberInDictionary);
                                        }
                                        loadedAppoint.TryAdd(storeId, list);
                                    }
                                    PromotedmemberExist.Appoint = loadedAppoint;

                                    // todo: now load bidsOffers from database
                                    ConcurrentDictionary<Guid, List<Bid>> loadedBids = new ConcurrentDictionary<Guid, List<Bid>>();
                                    ConcurrentDictionary<Guid, List<Guid>> helper = JsonConvert.DeserializeObject<ConcurrentDictionary<Guid, List<Guid>>>(PromotedmemberExist.BidsOffersDB);
                                    foreach (Guid storeId in helper.Keys)
                                    {
                                        List<Guid> listOfBids = helper[storeId];
                                        List<Bid> list = new List<Bid>();
                                        foreach (Guid BidId in listOfBids)
                                        {
                                            var bidFromDB = db.bids.FirstOrDefault(b => b.BidId.Equals(BidId));
                                            if (bidFromDB != null)
                                            {
                                                // now get decision
                                                Dictionary<PromotedMember, string> addDecetion = new Dictionary<PromotedMember, string>();
                                                var helperDecision = JsonConvert.DeserializeObject<Dictionary<Guid, string>>(bidFromDB.DecisionDB);
                                                foreach(Guid userId in helperDecision.Keys)
                                                {
                                                    var pmInDecition = db.promotedMembers.FirstOrDefault(m=>m.UserId.Equals(userId));
                                                    if(pmInDecition != null)
                                                    {
                                                        addDecetion.Add(pmInDecition, helperDecision[userId]);
                                                    }
                                                }
                                                bidFromDB.Decisions = addDecetion;

                                                var memberBid = db.members.FirstOrDefault(m => m.UserId.Equals(bidFromDB.UserID));
                                                if (memberBid.Discriminator.Equals("Member"))
                                                    bidFromDB.User = memberBid;
                                                else if (memberBid.Discriminator.Equals("PromotedMember"))
                                                    bidFromDB.User = db.promotedMembers.FirstOrDefault(m => m.UserId.Equals(bidFromDB.UserID));
                                                else
                                                {
                                                    bidFromDB.User = new User(); // bid was from guest
                                                    bidFromDB.User.UserId = bidFromDB.UserID;
                                                }


                                                list.Add(bidFromDB);
                                            }
                                        }
                                        loadedBids.TryAdd(storeId, list);
                                    }
                                    PromotedmemberExist.BidsOffers = loadedBids;

                                    memberExist = PromotedmemberExist;
                                }
                               
                            
                               
                                // now get member cart from DB
                                result.ShoppingCart = db.shoppingCarts.FirstOrDefault(m => m.UserId.Equals(memberExist.UserId));
                                var userBaskets = db.shoppingBaskets.Where(basket => basket.ShoppingCartId.Equals(memberExist.ShoppingCart.ShoppingCartId)).ToList();
                                foreach(var basket in userBaskets)
                                {
                                    result.ShoppingCart.Baskets.Add(basket);
                                }

                                // todo now get all bids
                                List<Guid> bidsIds = JsonConvert.DeserializeObject<List<Guid>>(memberExist.BidsDB);
                                List<Bid> bidsFromDB = db.bids.Where(b => bidsIds.Contains(b.BidId)).ToList();
                                // now update all Bids Decetions
                                foreach(Bid b in bidsFromDB)
                                {
                                    Dictionary<PromotedMember, string> addDecetion = new Dictionary<PromotedMember, string>();
                                    var helperDecision = JsonConvert.DeserializeObject<Dictionary<Guid, string>>(b.DecisionDB);
                                    foreach (Guid userId in helperDecision.Keys)
                                    {
                                        var pmInDecition = db.promotedMembers.FirstOrDefault(m => m.UserId.Equals(userId));
                                        if (pmInDecition != null)
                                        {
                                            addDecetion.Add(pmInDecition, helperDecision[userId]);
                                        }
                                    }
                                    b.Decisions = addDecetion;


                                    var memberBid = db.members.FirstOrDefault(m => m.UserId.Equals(b.UserID));
                                    if (memberBid.Discriminator.Equals("Member"))
                                        b.User = memberBid;
                                    else if (memberBid.Discriminator.Equals("PromotedMember"))
                                        b.User = db.promotedMembers.FirstOrDefault(m => m.UserId.Equals(b.UserID));
                                    else
                                    {
                                        b.User = new User(); // bid was from guest
                                        b.User.UserId = b.UserID;
                                    }
                                }

                                memberExist.Bids = bidsFromDB;

                                // ********************************
                                // need to implment this and get all member notfication from DB
                                // : todo get all notification from database and update them in the user
                                memberExist.AwaitingNotification = new List<DomainLayer.Notification>();
                               
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

        public void AddReview(Review review)
        {
            lock (this)
            {
                try
                {
                    using (var db = new DatabaseContext())
                    {
                        try
                        {
                            var reviews = db.Reviews;
                            reviews.Add(review);
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

        public void UpgradeMemberToPromotedMember(PromotedMember pm)
        {
            lock (this)
            {
                try
                {
                    using (var db = new DatabaseContext())
                    {
                        try
                        {
                            var memberExist = db.members.FirstOrDefault(m => m.Email.ToLower().Equals(pm.Email.ToLower()));
                            if (memberExist != null)
                                if (memberExist.Discriminator.Equals("Member"))
                                    db.members.Remove(memberExist);
                                else
                                {
                                    var proMemberExist = db.promotedMembers.FirstOrDefault(m => m.Email.ToLower().Equals(pm.Email.ToLower()));
                                    //memberExist = founder;
                                    db.promotedMembers.Remove(proMemberExist);
                                }
                            db.SaveChanges(true);

                            pm.DirectSupervisorDB = pm.DirectSupervisorJson;
                            pm.AppointDB = pm.AppointJson;
                            pm.BidsOffersDB = pm.BidsOffersJson;
                            pm.BidsDB = pm.BidsJson;

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
                            var inv = db.Inventories.FirstOrDefault(m => m.StoreID.Equals(result.StoreID));

                            if (inv != null)
                            {
                                result.itemsInventory = inv;
                                string quanity_ItemsDB = inv.Items_quantityDB;
                                
                                if (quanity_ItemsDB != null) // add quantity item for store
                                {
                                    ConcurrentDictionary<Guid, int> items_quantityHelper = JsonConvert.DeserializeObject<ConcurrentDictionary<Guid, int>>(quanity_ItemsDB);

                                    foreach (Guid id in items_quantityHelper.Keys)
                                    {
                                        Item item = db.Items.FirstOrDefault(m => m.ItemID.Equals(id));
                                        
                                        result.itemsInventory.items_quantity.TryAdd(item, items_quantityHelper[id]);
                                    }
                                }
                            }
                           
                        }
                        catch (Exception ex)
                        {
                           throw new Exception("failed to interact with stores table");
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

        public Review GetReviewById(Guid reviewID)
        {
            Review result = null;

            lock (this)
            {
                try
                {
                    using (var db = new DatabaseContext())
                    {
                        try
                        {
                            result = db.Reviews.FirstOrDefault(m => m.ReviewID.Equals(reviewID));
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("failed to interact with stores table");
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
                                var inv = db.Inventories.FirstOrDefault(m => m.StoreID.Equals(s.StoreID));
                                string quanity_ItemsDB = null;
                                if (inv != null)
                                    quanity_ItemsDB = inv.Items_quantityDB;
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

        public List<Guid> GetTSStoreIds()
        {
            List<Guid> result = null;

            lock (this)
            {
                try
                {
                    using (var db = new DatabaseContext())
                    {
                        try
                        {
                            var allStores = db.Stores;
                            result = new List<Guid>();
                            foreach (Store s in allStores)
                            {
                                result.Add(s.StoreID);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("failed to interact with stores table");
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

        public List<Guid> GetTSReviewsIds()
        {
            List<Guid> result = null;

            lock (this)
            {
                try
                {
                    using (var db = new DatabaseContext())
                    {
                        try
                        {
                            var allReviews = db.Reviews;
                            result = new List<Guid>();
                            foreach (Review review in allReviews)
                            {
                                result.Add(review.ReviewID);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("failed to interact with stores table");
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

        public void UpdateStore(Store editedStore)
        {
            lock (this)
            {
                try
                {
                    using (var db = new DatabaseContext())
                    {
                        try
                        {
                            var storeExist = db.Stores.FirstOrDefault((m => m.StoreID.Equals(editedStore.StoreID)));
                            if (storeExist != null)
                            {
                                storeExist.StoreName = editedStore.StoreName;
                                storeExist.Active = editedStore.Active;
                                storeExist.StoreRating = editedStore.StoreRating;
                                db.Stores.Update(storeExist);
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

        public void UpdateStoreInTransaction(DatabaseContext db, Store store)
        {
            lock (this)
            {
                try
                {

                    //var invetories = db.Inventories;
                    //var inventoryFound = db.Inventories.FirstOrDefault(m => m.StoreID.Equals(store.StoreID));
                    //inventoryFound.Items_quantityDB = store.itemsInventory.Items_quantityJson;
                    //invetories.Update(inventoryFound);
                    //db.SaveChanges(true);

                    var storeFound = db.Stores.FirstOrDefault(m => m.StoreID.Equals(store.StoreID));

                    var invetories = db.Inventories;
                    var inventoryFound = db.Inventories.FirstOrDefault(m => m.StoreID.Equals(storeFound.StoreID));
                    invetories.Remove(inventoryFound);
                    db.SaveChanges(true);

                    var stores = db.Stores;
                    stores.Remove(storeFound);
                    db.SaveChanges(true);

                    store.itemsInventory.Items_quantityDB = store.itemsInventory.Items_quantityJson;
                    stores.Add(store);
                    db.SaveChanges(true);
                }
                catch (Exception ex)
                {
                    //throw new Exception("failed to interact with stores table");
                }
            }
        }

        internal void AddItem(Item newItem)
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
                            throw new Exception("failed to interact with stores table");
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

        public void MemberLogIn(Member member)
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
                                    memberExist.LoggedIn = true;
                                    db.SaveChanges();
                                }
                                memberExist.LoggedIn = true;
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

        public void RemoveItem(Guid itemToRemove)
        {
            lock (this)
            {
                try
                {
                    using (var db = new DatabaseContext())
                    {
                        try
                        {
                            var itemExist = db.Items.FirstOrDefault(m => m.ItemID.Equals(itemToRemove));
                            if (itemExist!= null)
                            {
                                db.Items.Remove(itemExist);
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

        public void UpdateItem(Item editedItem)
        {
            lock (this)
            {
                try
                {
                    using (var db = new DatabaseContext())
                    {
                        try
                        {
                            var itemExist = db.Items.FirstOrDefault(m => m.ItemID.Equals(editedItem.ItemID));
                            if (itemExist != null)
                            {
                                db.Entry(itemExist).State = EntityState.Detached;
                                db.Items.Update(editedItem);
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

        public void AddBidAndUpdateUserBids(Bid newBid, User user)
        {
            lock (this)
            {
                try
                {
                    using (var db = new DatabaseContext())
                    {
                        try
                        {
                            var bids = db.bids;
                            newBid.UserID = newBid.User.UserId;
                            newBid.DecisionDB = newBid.DecisionJson;
                            bids.Add(newBid);
                            db.SaveChanges(true);

                            if(user.Discriminator.Equals("Member"))
                            {
                                var userFound = db.members.FirstOrDefault(u => u.UserId.Equals(user.UserId));
                                if (userFound != null)
                                {
                                    //userFound.BidsDB = user.BidsJson;
                                    db.Entry(userFound).State = EntityState.Detached;
                                    userFound.BidsDB = user.BidsJson;
                                    db.members.Update((Member)userFound);
                                    db.SaveChanges(true);
                                }
                            }
                            else
                            {
                                var userFound = db.promotedMembers.FirstOrDefault(u => u.UserId.Equals(user.UserId));
                                if (userFound != null)
                                {
                                    //userFound.BidsDB = user.BidsDB;
                                    db.Entry(userFound).State = EntityState.Detached;
                                    userFound.BidsDB = user.BidsJson;
                                    db.promotedMembers.Update((PromotedMember)userFound);
                                    db.SaveChanges(true);
                                }
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
                            var promotedMembers = db.promotedMembers;
                            pm.BidsDB = pm.BidsJson;
                            pm.DirectSupervisorDB = pm.DirectSupervisorJson;
                            pm.AppointDB = pm.AppointJson;
                            pm.BidsOffersDB = pm.BidsOffersJson;
                            
                            promotedMembers.Update(pm);
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

        public void UpdateMemberShoppingCartInTransaction(DatabaseContext db, Member member)
        {
            lock (this)
            {
                try
                {
                    var memberFound = db.members.FirstOrDefault(m => m.Email.ToLower().Equals(member.Email.ToLower()));
                    if(memberFound != null)
                    {
                        var userBasket = db.shoppingBaskets.Where(b => b.ShoppingCartId.Equals(memberFound.ShoppingCart.ShoppingCartId)).ToList();
                        if (userBasket.Count > 0)
                        {
                            db.shoppingBaskets.RemoveRange(userBasket);
                            db.SaveChanges(true);
                        }

                    }

                }
                catch (Exception ex)
                {
                    //throw new Exception("failed to interact with stores table");
                }
            }
        }

        public void UpdateItemAfterEdit(Store store, Guid itemID, string itemName, string itemCategory, double itemPrice)
        {
            lock (this)
            {
                try
                {
                    using (var db = new DatabaseContext())
                    {
                        try
                        {
                            var inv = db.Inventories.FirstOrDefault(m => m.StoreID.Equals(store.StoreID));
                            try
                            {
                                if (inv != null)
                                {
                                    db.Entry(inv).State = EntityState.Detached;
                                    store.itemsInventory.Items_quantityDB = store.itemsInventory.Items_quantityJson;
                                    db.Inventories.Update(store.itemsInventory);
                                    db.SaveChanges(true);
                                }
                            }
                            catch(Exception ex)
                            {
                                // no change in inventory so continue
                            }

                            var itemFound = db.Items.FirstOrDefault(it => it.ItemID.Equals(itemID));
                            if(itemFound!=null)
                            {
                                itemFound.Name = itemName;
                                itemFound.Category = itemCategory;
                                itemFound.Price = itemPrice;
                                db.Items.Update(itemFound);
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

        public void UpdateAfterRemovingItem(Store store, Guid itemId)
        {
            lock (this)
            {
                try
                {
                    using (var db = new DatabaseContext())
                    {
                        try
                        {
                            var inv = db.Inventories.FirstOrDefault(m => m.StoreID.Equals(store.StoreID));
                            try
                            {
                                if (inv != null)
                                {
                                    db.Entry(inv).State = EntityState.Detached;
                                    store.itemsInventory.Items_quantityDB = store.itemsInventory.Items_quantityJson;
                                    db.Inventories.Update(store.itemsInventory);
                                    db.SaveChanges(true);
                                    // item removed automaticaly from this operation
                                }
                            }
                            catch (Exception ex)
                            {
                                // no change in inventory so continue
                            }
                            var itemFound = db.Items.FirstOrDefault(it => it.ItemID.Equals(itemId));
                            if (itemFound != null)
                            {
                                db.Items.Remove(itemFound);
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

        public void UpdateStoreInventory(Store store)
        {
            lock (this)
            {
                try
                {
                    using (var db = new DatabaseContext())
                    {
                        try
                        {
                            var inv = db.Inventories.FirstOrDefault(m => m.StoreID.Equals(store.StoreID));
                            try
                            {
                                if (inv != null)
                                {
                                    db.Entry(inv).State = EntityState.Detached;
                                    store.itemsInventory.Items_quantityDB = store.itemsInventory.Items_quantityJson;
                                    db.Inventories.Update(store.itemsInventory);
                                    db.SaveChanges(true);
                                    // item removed automaticaly from this operation
                                }
                            }
                            catch (Exception ex)
                            {
                                // no change in inventory so continue
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

        public void UpdateBidAndUser(Bid bid, User user)
        {
            lock (this)
            {
                try
                {
                    using (var db = new DatabaseContext())
                    {
                        try
                        {
                            var bidFound = db.bids.FirstOrDefault(m => m.BidId.Equals(bid.BidId));
                            try
                            {
                                if (bidFound != null)
                                {
                                    db.Entry(bidFound).State = EntityState.Detached;
                                    db.bids.Update(bid);
                                    db.SaveChanges(true);

                                    if (user is Member)
                                        db.members.Update((Member)user);
                                    else
                                        db.promotedMembers.Update((PromotedMember)user);
                                    db.SaveChanges(true);
                                }
                            }
                            catch (Exception ex)
                            {
                                // no change in inventory so continue
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

        public void RemoveBid(Bid bid)
        {
            lock (this)
            {
                try
                {
                    using (var db = new DatabaseContext())
                    {
                        try
                        {
                            var bidFound = db.bids.FirstOrDefault(m => m.BidId.Equals(bid.BidId));
                            try
                            {
                                if (bidFound != null)
                                {
                                    db.Entry(bidFound).State = EntityState.Detached;
                                    db.bids.Remove(bid);
                                    db.SaveChanges(true);
                                }
                            }
                            catch (Exception ex)
                            {
                                // no change in inventory so continue
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

        public Member GetMemberFromDBByEmail(string email) // same as getting member values when logging in
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
                            var memberExist = db.members.FirstOrDefault(m => m.Email.ToLower().Equals(email.ToLower()));
                            if (memberExist != null)
                            {
                                if (!memberExist.Discriminator.Equals("Member"))
                                {
                                    // here it means that he is promotedMember
                                    var PromotedmemberExist = db.promotedMembers.FirstOrDefault(m => m.Email.ToLower().Equals(email.ToLower()));

                                    // todo: now load DirecrSupervisor from database
                                    ConcurrentDictionary<Guid, PromotedMember> loadedDirect = new ConcurrentDictionary<Guid, PromotedMember>();
                                    ConcurrentDictionary<Guid, string> directs = JsonConvert.DeserializeObject<ConcurrentDictionary<Guid, string>>(PromotedmemberExist.DirectSupervisorDB);
                                    foreach (Guid storeId in directs.Keys)
                                    {
                                        foreach (Guid id1 in directs.Keys)
                                        {
                                            var promotedMemberInDictionary = db.promotedMembers.FirstOrDefault(pm => pm.Email.ToLower().Equals(directs[id1].ToLower()));
                                            if (promotedMemberInDictionary != null)
                                                loadedDirect.TryAdd(storeId, promotedMemberInDictionary);
                                            else
                                                loadedDirect.TryAdd(storeId, null);
                                        }
                                    }
                                    PromotedmemberExist.DirectSupervisor = loadedDirect;

                                    // todo: now load appoint from database
                                    ConcurrentDictionary<Guid, List<PromotedMember>> loadedAppoint = new ConcurrentDictionary<Guid, List<PromotedMember>>();
                                    ConcurrentDictionary<Guid, List<string>> loadeds = JsonConvert.DeserializeObject<ConcurrentDictionary<Guid, List<string>>>(PromotedmemberExist.AppointDB);
                                    foreach (Guid storeId in loadeds.Keys)
                                    {
                                        List<string> listOfProMembers = loadeds[storeId];
                                        List<PromotedMember> list = new List<PromotedMember>();
                                        foreach (string pmEmail in listOfProMembers)
                                        {
                                            var promotedMemberInDictionary = db.promotedMembers.FirstOrDefault(pm => pm.Email.ToLower().Equals(pmEmail.ToLower()));
                                            if (promotedMemberInDictionary != null)
                                                list.Add(promotedMemberInDictionary);
                                        }
                                        loadedAppoint.TryAdd(storeId, list);
                                    }
                                    PromotedmemberExist.Appoint = loadedAppoint;

                                    // todo: now load bidsOffers from database
                                    ConcurrentDictionary<Guid, List<Bid>> loadedBids = new ConcurrentDictionary<Guid, List<Bid>>();
                                    ConcurrentDictionary<Guid, List<Guid>> helper = JsonConvert.DeserializeObject<ConcurrentDictionary<Guid, List<Guid>>>(PromotedmemberExist.BidsOffersDB);
                                    foreach (Guid storeId in helper.Keys)
                                    {
                                        List<Guid> listOfBids = helper[storeId];
                                        List<Bid> list = new List<Bid>();
                                        foreach (Guid BidId in listOfBids)
                                        {
                                            var bidFromDB = db.bids.FirstOrDefault(b => b.BidId.Equals(BidId));
                                            if (bidFromDB != null)
                                            {
                                                // now get decision
                                                Dictionary<PromotedMember, string> addDecetion = new Dictionary<PromotedMember, string>();
                                                var helperDecision = JsonConvert.DeserializeObject<Dictionary<Guid, string>>(bidFromDB.DecisionDB);
                                                foreach (Guid userId in helperDecision.Keys)
                                                {
                                                    var pmInDecition = db.promotedMembers.FirstOrDefault(m => m.UserId.Equals(userId));
                                                    if (pmInDecition != null)
                                                    {
                                                        addDecetion.Add(pmInDecition, helperDecision[userId]);
                                                    }
                                                }
                                                bidFromDB.Decisions = addDecetion;

                                                var memberBid = db.members.FirstOrDefault(m => m.UserId.Equals(bidFromDB.UserID));
                                                if (memberBid.Discriminator.Equals("Member"))
                                                    bidFromDB.User = memberBid;
                                                else if (memberBid.Discriminator.Equals("PromotedMember"))
                                                    bidFromDB.User = db.promotedMembers.FirstOrDefault(m => m.UserId.Equals(bidFromDB.UserID));
                                                else
                                                {
                                                    bidFromDB.User = new User(); // bid was from guest
                                                    bidFromDB.User.UserId = bidFromDB.UserID;
                                                }


                                                list.Add(bidFromDB);
                                            }
                                        }
                                        loadedBids.TryAdd(storeId, list);
                                    }
                                    PromotedmemberExist.BidsOffers = loadedBids;

                                    memberExist = PromotedmemberExist;
                                }
                                if (!memberExist.LoggedIn || true) // remove || true later
                                {
                                    memberExist.LoggedIn = false; // remove this line later 
                                    Guid id1 = memberExist.UserId;
                                    string mac1 = db.macs.Find(id1).mac;

                                    result = memberExist;
                                    
                                    // now get member cart from DB
                                    result.ShoppingCart = db.shoppingCarts.FirstOrDefault(m => m.UserId.Equals(memberExist.UserId));
                                    var userBaskets = db.shoppingBaskets.Where(basket => basket.ShoppingCartId.Equals(memberExist.ShoppingCart.ShoppingCartId)).ToList();
                                    foreach (var basket in userBaskets)
                                    {
                                        result.ShoppingCart.Baskets.Add(basket);
                                    }

                                    // todo now get all bids
                                    List<Guid> bidsIds = JsonConvert.DeserializeObject<List<Guid>>(memberExist.BidsDB);
                                    List<Bid> bidsFromDB = db.bids.Where(b => bidsIds.Contains(b.BidId)).ToList();
                                    // now update all Bids Decetions
                                    foreach (Bid b in bidsFromDB)
                                    {
                                        Dictionary<PromotedMember, string> addDecetion = new Dictionary<PromotedMember, string>();
                                        var helperDecision = JsonConvert.DeserializeObject<Dictionary<Guid, string>>(b.DecisionDB);
                                        foreach (Guid userId in helperDecision.Keys)
                                        {
                                            var pmInDecition = db.promotedMembers.FirstOrDefault(m => m.UserId.Equals(userId));
                                            if (pmInDecition != null)
                                            {
                                                addDecetion.Add(pmInDecition, helperDecision[userId]);
                                            }
                                        }
                                        b.Decisions = addDecetion;


                                        var memberBid = db.members.FirstOrDefault(m => m.UserId.Equals(b.UserID));
                                        if (memberBid.Discriminator.Equals("Member"))
                                            b.User = memberBid;
                                        else if (memberBid.Discriminator.Equals("PromotedMember"))
                                            b.User = db.promotedMembers.FirstOrDefault(m => m.UserId.Equals(b.UserID));
                                        else
                                        {
                                            b.User = new User(); // bid was from guest
                                            b.User.UserId = b.UserID;
                                        }
                                    }

                                    memberExist.Bids = bidsFromDB;
                                    // todo get all notification from database and update them in the user
                                    memberExist.AwaitingNotification = new List<DomainLayer.Notification>();
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

        public List<PromotedMember> GetAllEmployees()
        {
            lock (this)
            {
                try
                {
                    using (var db = new DatabaseContext())
                    {
                        try
                        {
                            var allMembers = db.promotedMembers; 
                            return allMembers.ToList();
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
                return null;
            }
        }
        
        public void DowngradePromotedMemberToReg(Member pm)
        {
            lock (this)
            {
                try
                {
                    using (var db = new DatabaseContext())
                    {
                        try
                        {
                            var memberExist = db.promotedMembers.FirstOrDefault(m => m.Email.ToLower().Equals(pm.Email.ToLower()));
                            if (memberExist != null)
                                if (memberExist.Discriminator.Equals("Member"))
                                    db.members.Remove(memberExist);
                                else
                                {
                                    var proMemberExist = db.promotedMembers.FirstOrDefault(m => m.Email.ToLower().Equals(pm.Email.ToLower()));
                                    //memberExist = founder;
                                    db.promotedMembers.Remove(proMemberExist);
                                }
                            db.SaveChanges(true);

                            pm.BidsDB = pm.BidsJson;

                            db.members.Add(pm);
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

        public bool LoadSystemInit()
        {
            lock (this)
            {
                try
                {
                    using (var db = new DatabaseContext())
                    {
                        try
                        {
                            var init = db.initializeSystems.ToList();
                           
                            if(init!=null && init.Count == 0)
                            {
                                InitializeSystem initializeSystem = new InitializeSystem();
                                initializeSystem.IsInit = ApplicationOptions.InitTradingSystem;
                                db.initializeSystems.Add(initializeSystem);
                                db.SaveChanges(true);

                                return initializeSystem.IsInit;
                            }

                            foreach (InitializeSystem initSys in init)
                            {
                                return initSys.IsInit;
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("failed to interact with stores table");
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to Connect With Database");
                }
                return false;
         
            }

        }

        public void SetSystemInit(bool isInit)
        {
            lock (this)
            {
                try
                {
                    using (var db = new DatabaseContext())
                    {
                        try
                        {
                            db.initializeSystems.RemoveRange(db.initializeSystems);
                            InitializeSystem initializeSystem = new InitializeSystem();
                            initializeSystem.IsInit = isInit;
                            db.initializeSystems.Add(initializeSystem);
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


        public void AddOrder(Order newOrder)
        {
            lock (this)
            {
                try
                {
                    using (var db = new DatabaseContext())
                    {
                        try
                        {
                            newOrder.ListItemsDB = newOrder.OrderIDsJson;
                            db.orders.Add(newOrder);
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








    }
}
