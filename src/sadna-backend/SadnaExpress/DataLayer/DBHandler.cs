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
using SadnaExpress.DomainLayer;
using System.Security.Cryptography;
using SadnaExpress.DomainLayer.Store.Policy;

namespace SadnaExpress.DataLayer
{
    public class DBHandler
    {
        #region properties
        private static readonly object databaseLock = new object();

        private readonly string DbErrorMessage="Unfortunatly connecting to the db faild, please try again in a get minutes";

        private static DBHandler instance = null;
        #endregion

        #region constructor
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
        #endregion

        #region Methods

        #region Clean DB
        public void CleanDB()
        {


            lock (this)
            {
                try
                {
                    //DatabaseContextFactory.TestMode = true;
                    using (var db = DatabaseContextFactory.ConnectToDatabase())
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
                            db.Reviews.RemoveRange(db.Reviews);
                            db.notfications.RemoveRange(db.notfications);
                            db.orders.RemoveRange(db.orders);
                            db.ItemForOrders.RemoveRange(db.ItemForOrders);
                            db.conditions.RemoveRange(db.conditions);
                            db.policies.RemoveRange(db.policies);

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
                    throw new Exception(DbErrorMessage);
                }
            }
        }
        #endregion

        #region Members managment

        public bool memberExistsByEmail(string email)
        {
            bool result = false;

            lock (this)
            {
                try
                {
                    using (var db = DatabaseContextFactory.ConnectToDatabase())
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
                    throw new Exception(DbErrorMessage);
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
                    using (var db = DatabaseContextFactory.ConnectToDatabase())
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
                    throw new Exception(DbErrorMessage);
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
                    using (var db = DatabaseContextFactory.ConnectToDatabase())
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
                    throw new Exception(DbErrorMessage);
                }
            }
        }

        public void RemoveMember(Guid memberId)
        {
            lock (this)
            {
                try
                {
                    using (var db = DatabaseContextFactory.ConnectToDatabase())
                    {
                        try
                        {
                            var memberFound = db.members.FirstOrDefault(m => m.UserId.Equals(memberId));

                            if (memberFound != null)
                            {
                                db.members.Remove(memberFound);
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
                    throw new Exception(DbErrorMessage);
                }
            }
        }

        public Member CheckMemberValidLogin(Guid id, string email, string password, IPasswordHash _ph) //load logged in member to logic layer
        {
            Member result = null;
            lock (this)
            {
                try
                {
                    using (var db = DatabaseContextFactory.ConnectToDatabase())
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
                                    var PromotedmemberExist = db.promotedMembers.FirstOrDefault(m => m.Email.ToLower().Equals(email.ToLower()));

                                    #region load direcetive
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
                                    #endregion

                                    #region load appoint
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
                                    #endregion

                                    #region load bids offers
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
                                                if (memberBid != null)
                                                {
                                                    if (memberBid.Discriminator.Equals("Member"))
                                                        bidFromDB.User = memberBid;
                                                    else if (memberBid.Discriminator.Equals("PromotedMember"))
                                                        bidFromDB.User = db.promotedMembers.FirstOrDefault(m =>
                                                            m.UserId.Equals(bidFromDB.UserID));
                                                    list.Add(bidFromDB);
                                                }
                                            }
                                        }
                                        loadedBids.TryAdd(storeId, list);
                                    }
                                    PromotedmemberExist.BidsOffers = loadedBids;
                                    #endregion

                                    memberExist = PromotedmemberExist;
                                }

                                #region load shopping cart
                                result.ShoppingCart = db.shoppingCarts.FirstOrDefault(m => m.UserId.Equals(memberExist.UserId));
                                var userBaskets = db.shoppingBaskets.Where(basket => basket.ShoppingCartId.Equals(memberExist.ShoppingCart.ShoppingCartId)).ToList();
                                foreach(var basket in userBaskets)
                                {
                                    result.ShoppingCart.Baskets.Add(basket);
                                }
                                #endregion

                                #region load bids
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
                                #endregion

                                #region load notifications

                                var notifications = db.notfications.Where(notification => notification.SentTo.Equals(memberExist.UserId));
                                // todo get all notification from database and update them in the user
                                memberExist.AwaitingNotification = new List<DomainLayer.Notification>();

                                if (notifications != null)
                                {
                                    foreach (DomainLayer.Notification n in notifications)
                                    {
                                        memberExist.AwaitingNotification.Add(n);
                                    }
                                }

                                #endregion

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
                    throw new Exception(DbErrorMessage);
                }
                return result;
            }
        }

        public Member GetMemberFromDBByEmail(string email) // same as getting member values when logging in
        {
            Member result = null;
            lock (this)
            {
                try
                {
                    using (var db = DatabaseContextFactory.ConnectToDatabase())
                    {
                        try
                        {
                            var memberExist = db.members.FirstOrDefault(m => m.Email.ToLower().Equals(email.ToLower()));
                            
                            if (memberExist != null)
                            {
                                if (memberExist.Discriminator.Equals("PromotedMember"))
                                {
                                    var PromotedmemberExist = db.promotedMembers.FirstOrDefault(m => m.Email.ToLower().Equals(email.ToLower()));

                                    #region load direcetive
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
                                    #endregion

                                    #region load appoint
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
                                    #endregion

                                    #region load bids offers
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
                                    #endregion 

                                    memberExist = PromotedmemberExist;
                                }

                                result = memberExist;

                                #region load shopping cart
                                result.ShoppingCart = db.shoppingCarts.FirstOrDefault(m => m.UserId.Equals(memberExist.UserId));
                                var userBaskets = db.shoppingBaskets.Where(basket => basket.ShoppingCartId.Equals(memberExist.ShoppingCart.ShoppingCartId)).ToList();
                                foreach (var basket in userBaskets)
                                {
                                    result.ShoppingCart.Baskets.Add(basket);
                                }
                                #endregion

                                #region load bids
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
                                #endregion

                                #region load notifications

                                var notifications = db.notfications.Where(notification => notification.SentTo.Equals(memberExist.UserId));
                                // todo get all notification from database and update them in the user
                                memberExist.AwaitingNotification = new List<DomainLayer.Notification>();

                                if (notifications != null)
                                {
                                    foreach(DomainLayer.Notification n in notifications)
                                    {
                                        memberExist.AwaitingNotification.Add(n);
                                    }
                                }

                                #endregion
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
                    throw new Exception(DbErrorMessage);
                }
                return result;
            }
        }

        public Member GetMemberFromDBById(Guid id) // same as getting member values when logging in
        {
            Member result = null;
            lock (this)
            {
                try
                {
                    using (var db = DatabaseContextFactory.ConnectToDatabase())
                    {
                        try
                        {
                            var memberExist = db.members.FirstOrDefault(m => m.UserId.Equals(id));

                            if (memberExist != null)
                            {
                                if (memberExist.Discriminator.Equals("PromotedMember"))
                                {
                                    var PromotedmemberExist = db.promotedMembers.FirstOrDefault(m => m.UserId.Equals(id));

                                    #region load direcetive
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
                                    #endregion

                                    #region load appoint
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
                                    #endregion

                                    #region load bids offers
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
                                    #endregion 

                                    memberExist = PromotedmemberExist;
                                }

                                result = memberExist;

                                #region load shopping cart
                                result.ShoppingCart = db.shoppingCarts.FirstOrDefault(m => m.UserId.Equals(memberExist.UserId));
                                var userBaskets = db.shoppingBaskets.Where(basket => basket.ShoppingCartId.Equals(memberExist.ShoppingCart.ShoppingCartId)).ToList();
                                foreach (var basket in userBaskets)
                                {
                                    result.ShoppingCart.Baskets.Add(basket);
                                }
                                #endregion

                                #region load bids
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
                                #endregion

                                #region load notifications

                                var notifications = db.notfications.Where(notification => notification.SentTo.Equals(memberExist.UserId));
                                // todo get all notification from database and update them in the user
                                memberExist.AwaitingNotification = new List<DomainLayer.Notification>();

                                if (notifications != null)
                                {
                                    foreach (DomainLayer.Notification n in notifications)
                                    {
                                        memberExist.AwaitingNotification.Add(n);
                                    }
                                }

                                #endregion
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
                    throw new Exception(DbErrorMessage);
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
                    using (var db = DatabaseContextFactory.ConnectToDatabase())
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
                    throw new Exception(DbErrorMessage);
                }
                return result;
            }
        }

        public void MemberLogout(Member member)
        {
            lock (this)
            {
                try
                {
                    using (var db = DatabaseContextFactory.ConnectToDatabase())
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
                    throw new Exception(DbErrorMessage);
                }
            }
        }

        public void MemberLogIn(Member member)
        {
            lock (this)
            {
                try
                {
                    using (var db = DatabaseContextFactory.ConnectToDatabase())
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
                    throw new Exception(DbErrorMessage);
                }
            }
        }

        public void UpgradeMemberToPromotedMember(PromotedMember pm)
        {
            lock (this)
            {
                try
                {
                    using (var db = DatabaseContextFactory.ConnectToDatabase())
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
                    throw new Exception(DbErrorMessage);
                }
            }
        }

        public void UpdatePromotedMember(PromotedMember pm)
        {
            lock (this)
            {
                try
                {
                    using (var db = DatabaseContextFactory.ConnectToDatabase())
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
                    throw new Exception(DbErrorMessage);
                }
            }
        }

        public List<PromotedMember> GetAllEmployees()
        {
            lock (this)
            {
                try
                {
                    using (var db = DatabaseContextFactory.ConnectToDatabase())
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
                    throw new Exception(DbErrorMessage);
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
                    using (var db = DatabaseContextFactory.ConnectToDatabase())
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
                    throw new Exception(DbErrorMessage);
                }
            }
        }

        public ConcurrentDictionary<Guid, Member> GetAllMembers()
        {
            ConcurrentDictionary<Guid, Member> result = new ConcurrentDictionary<Guid, Member>();
            lock (this)
            {
                try
                {
                    using (var db = DatabaseContextFactory.ConnectToDatabase())
                    {
                        try
                        {
                            var allMembers = db.members;
                            foreach (var member in allMembers)
                            {
                                if(member.Discriminator.Equals("PromotedMember"))
                                {
                                    result.TryAdd(member.UserId, (PromotedMember)member);
                                }
                                result.TryAdd(member.UserId, member);
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
                    throw new Exception(DbErrorMessage);
                }
                return result;
            }
        }

        #endregion

        #region Stores managment

        public bool IsStoreNameExist(string storeName)
        {
            bool result = false;

            lock (this)
            {
                try
                {
                    using (var db = DatabaseContextFactory.ConnectToDatabase())
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
                    throw new Exception(DbErrorMessage);
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
                    using (var db = DatabaseContextFactory.ConnectToDatabase())
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
                    throw new Exception(DbErrorMessage);
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
                    using (var db = DatabaseContextFactory.ConnectToDatabase())
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
                    throw new Exception(DbErrorMessage);
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
                    using (var db = DatabaseContextFactory.ConnectToDatabase())
                    {
                        try
                        {
                            result = db.Stores.FirstOrDefault(m => m.StoreID.Equals(storeID));
                            var inv = db.Inventories.FirstOrDefault(m => m.StoreID.Equals(result.StoreID));
                            if (result != null)
                            {
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

                                int saveStorePurchasePolicyCounter = result.PurchasePolicyCounter;
                                int saveStoreDiscountPolicyCounter = result.DiscountPolicyCounter;

                                // todo get store condition from DB
                                List<ConditionDB> condsFromDB = db.conditions.Where(c => c.StoreID.Equals(result.StoreID)).ToList();
                                result.PurchasePolicyCounter = 0;
                                foreach (ConditionDB c in condsFromDB)
                                {
                                    result.AddCondition(c.EntityStr, c.EntityName, c.Type, c.Value, c.Dt, c.Op, c.OpCond, false, c.ID);
                                }

                                // todo get all policies from DB
                                result.DiscountPolicyCounter = 0;
                                List<PolicyDB> simplePolFromDB = db.policies.Where(c => c.StoreId.Equals(result.StoreID) && c.Discriminator.Equals("Simple")).ToList();
                                foreach(PolicyDB pol in simplePolFromDB)
                                {
                                    result.CreateSimplePolicy<string>(pol.simple_level, pol.simple_percent, pol.simple_startDate, pol.simple_endDate, false, pol.ID);
                                    if(pol.activated)
                                    {
                                        result.AddPolicy(pol.ID, false);
                                    }
                                }
                                List<PolicyDB> complexPolFromDB = db.policies.Where(c => c.StoreId.Equals(result.StoreID) && c.Discriminator.Equals("Complex")).ToList();
                                foreach (PolicyDB pol in complexPolFromDB)
                                {
                                    result.CreateComplexPolicyFromDB(pol.complex_op, pol.ID, pol.complex_policys);
                                    if (pol.activated)
                                    {
                                        result.AddPolicy(pol.ID, false);
                                    }
                                }


                                result.PurchasePolicyCounter = saveStorePurchasePolicyCounter;
                                result.DiscountPolicyCounter = saveStoreDiscountPolicyCounter;
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
                    throw new Exception(DbErrorMessage);
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
                    using (var db = DatabaseContextFactory.ConnectToDatabase())
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

                                int saveStorePurchasePolicyCounter = s.PurchasePolicyCounter;
                                int saveStoreDiscountPolicyCounter = s.DiscountPolicyCounter;

                                // todo get store condition from DB
                                List<ConditionDB> condsFromDB = db.conditions.Where(c => c.StoreID.Equals(s.StoreID)).ToList();
                                s.PurchasePolicyCounter = 0;
                                foreach (ConditionDB c in condsFromDB)
                                {
                                    s.AddCondition(c.EntityStr, c.EntityName, c.Type, c.Value, c.Dt, c.Op, c.OpCond, false, c.ID);
                                }

                                // todo get all policies from DB
                                s.DiscountPolicyCounter = 0;
                                List<PolicyDB> simplePolFromDB = db.policies.Where(c => c.StoreId.Equals(s.StoreID) && c.Discriminator.Equals("Simple")).ToList();
                                foreach (PolicyDB pol in simplePolFromDB)
                                {
                                    s.CreateSimplePolicy<string>(pol.simple_level, pol.simple_percent, pol.simple_startDate, pol.simple_endDate, false, pol.ID);
                                    if (pol.activated)
                                    {
                                        s.AddPolicy(pol.ID, false);
                                    }
                                }
                                List<PolicyDB> complexPolFromDB = db.policies.Where(c => c.StoreId.Equals(s.StoreID) && c.Discriminator.Equals("Complex")).ToList();
                                foreach (PolicyDB pol in complexPolFromDB)
                                {
                                    s.CreateComplexPolicyFromDB(pol.complex_op, pol.ID, pol.complex_policys);
                                    if (pol.activated)
                                    {
                                        s.AddPolicy(pol.ID, false);
                                    }
                                }


                                s.PurchasePolicyCounter = saveStorePurchasePolicyCounter;
                                s.DiscountPolicyCounter = saveStoreDiscountPolicyCounter;



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
                    throw new Exception(DbErrorMessage);
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
                    using (var db = DatabaseContextFactory.ConnectToDatabase())
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
                    throw new Exception(DbErrorMessage);
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
                    using (var db = DatabaseContextFactory.ConnectToDatabase())
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
                    throw new Exception(DbErrorMessage);
                }
            }
        }

        public void UpdateStoreInTransaction(DatabaseContext db, Store store)
        {
            lock (this)
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

        #endregion

        #region Reviews managment
        public void AddReview(Review review)
        {
            lock (this)
            {
                try
                {
                    using (var db = DatabaseContextFactory.ConnectToDatabase())
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
                    throw new Exception(DbErrorMessage);
                }
            }
        }

        public Review GetReviewById(Guid reviewID)
        {
            Review result = null;

            lock (this)
            {
                try
                {
                    using (var db = DatabaseContextFactory.ConnectToDatabase())
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
                    throw new Exception(DbErrorMessage);
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
                    using (var db = DatabaseContextFactory.ConnectToDatabase())
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
                    throw new Exception(DbErrorMessage);
                }
                return result;
            }
        }

        #endregion

        #region Shopping cart managment

        public void UpdateMemberShoppingCart(Member member)
        {
            lock (this)
            {
                try
                {
                    using (var db = DatabaseContextFactory.ConnectToDatabase())
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
                    throw new Exception(DbErrorMessage);
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
                    if (memberFound != null)
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

        #endregion

        #region Items managment

        internal void AddItem(Item newItem)
        {
            lock (this)
            {
                try
                {
                    using (var db = DatabaseContextFactory.ConnectToDatabase())
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
                    throw new Exception(DbErrorMessage);
                }
            }
        }

        public void RemoveItem(Guid itemToRemove)
        {
            lock (this)
            {
                try
                {
                    using (var db = DatabaseContextFactory.ConnectToDatabase())
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
                    throw new Exception(DbErrorMessage);
                }
            }
        }

        public void UpdateItem(Item editedItem)
        {
            lock (this)
            {
                try
                {
                    using (var db = DatabaseContextFactory.ConnectToDatabase())
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
                    throw new Exception(DbErrorMessage);
                }
            }
        }
       
        public void UpdateItemAfterEdit(Store store, Guid itemID, string itemName, string itemCategory, double itemPrice)
        {
            lock (this)
            {
                try
                {
                    using (var db = DatabaseContextFactory.ConnectToDatabase())
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
                    throw new Exception(DbErrorMessage);
                }
            }
        }

        public void UpdateAfterRemovingItem(Store store, Guid itemId)
        {
            lock (this)
            {
                try
                {
                    using (var db = DatabaseContextFactory.ConnectToDatabase())
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
                    throw new Exception(DbErrorMessage);
                }
            }
        }

        public void UpdateStoreInventory(Store store)
        {
            lock (this)
            {
                try
                {
                    using (var db = DatabaseContextFactory.ConnectToDatabase())
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
                    throw new Exception(DbErrorMessage);
                }
            }
        }

        #endregion

        #region Bids managment
        public void UpdateBidAndUser(Bid bid, User user)
        {
            lock (this)
            {
                try
                {
                    using (var db = DatabaseContextFactory.ConnectToDatabase())
                    {
                        try
                        {
                            var bidFound = db.bids.FirstOrDefault(m => m.BidId.Equals(bid.BidId));
                            try
                            {
                                bid.UserID = bid.User.UserId;
                                bid.DecisionDB = bid.DecisionJson;
                                if (bidFound != null)
                                {
                                    db.Entry(bidFound).State = EntityState.Detached;
                                    db.bids.Update(bid);
                                    db.SaveChanges(true);
                                }
                                else
                                {
                                    db.bids.Add(bid);
                                    db.SaveChanges(true);
                                }

                                user.BidsDB = user.BidsJson;
                                if (user is Member)
                                        db.members.Update((Member)user);
                                    else
                                        db.promotedMembers.Update((PromotedMember)user);
                                    db.SaveChanges(true);
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
                    throw new Exception(DbErrorMessage);
                }
            }
        }

        public void RemoveBid(Bid bid)
        {
            lock (this)
            {
                try
                {
                    using (var db = DatabaseContextFactory.ConnectToDatabase())
                    {
                        try
                        {
                            var bidFound = db.bids.FirstOrDefault(m => m.BidId.Equals(bid.BidId));
                           
                            if (bidFound != null)
                            {
                                db.bids.Remove(bidFound);
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
                    throw new Exception(DbErrorMessage);
                }
            }
        }
        #endregion

        #region System init managment
        public bool LoadSystemInit()
        {
            lock (this)
            {
                try
                {
                    using (var db = DatabaseContextFactory.ConnectToDatabase())
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
                    throw new Exception(DbErrorMessage);
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
                    using (var db = DatabaseContextFactory.ConnectToDatabase())
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
                    throw new Exception(DbErrorMessage);
                }
            }
        }

        #endregion

        #region Orders managment
        public void AddOrder(DatabaseContext db,Order newOrder)
        {
            lock (this)
            {
                try
                {
                    foreach (ItemForOrder itemForOrder in newOrder.ListItems)
                    {
                        db.ItemForOrders.Add(itemForOrder);
                    }

                    newOrder.ListItemsDB = newOrder.OrderIDsJson;
                    db.orders.Add(newOrder);
                    db.SaveChanges(true);
                         
                }
                catch (Exception ex)
                {
                    throw new Exception(DbErrorMessage);
                }
            }
        }

        public List<Order> GetAllOrders()
        {
            List<Order> result = null;

            lock (this)
            {
                try
                {
                    using (var db = DatabaseContextFactory.ConnectToDatabase())
                    {
                        try
                        {
                            var allOrders = db.orders;
                            result = new List<Order>();
                            foreach (Order o in allOrders)
                            {
                                List<ItemForOrder> ItemsForOrder = new List<ItemForOrder>();
                                List <Guid> ItemsForOrderIds = JsonConvert.DeserializeObject<List<Guid>>(o.ListItemsDB);
                                foreach(Guid itemId in ItemsForOrderIds)
                                {
                                    var item = db.ItemForOrders.FirstOrDefault(i => i.ItemForOrderId.Equals(itemId));
                                    if (item != null)
                                        ItemsForOrder.Add(item);
                                }
                                o.ListItems = ItemsForOrder;
                                result.Add(o);
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
                    throw new Exception(DbErrorMessage);
                }
                return result;
            }
        }

        #endregion

        #region Notification managment
        public void AddNotification(DomainLayer.Notification notification, DatabaseContext db)
        {
            lock (this)
            {
                if (db != null)
                {
                    try
                    {
                        var notifications = db.notfications;
                        notifications.Add(notification);
                        db.SaveChanges(true);
                    }
                    catch (Exception ex1)
                    {
                        throw new Exception(DbErrorMessage);
                    }
                }
                else
                {
                    using (var initdb = DatabaseContextFactory.ConnectToDatabase())
                    {
                        try
                        {
                            var notifications = initdb.notfications;
                            notifications.Add(notification);
                            initdb.SaveChanges(true);
                        }
                        catch (Exception ex2)
                        {
                            throw new Exception(DbErrorMessage);
                        }
                    }
                }

            }
        }

        public Dictionary<Guid, List<Member>> LoadNotificationOfficialsFromDB()
        {
            Dictionary<Guid, List<Member>> result = new Dictionary<Guid, List<Member>>();
            lock (this)
            {
                try
                {
                    using (var db = DatabaseContextFactory.ConnectToDatabase())
                    {
                        try
                        {
                            var allPromotedMembers = db.promotedMembers;
                            foreach(PromotedMember pm in allPromotedMembers)
                            {
                                foreach(Guid storeId in pm.Permission.Keys)
                                {
                                    if(pm.Permission[storeId].Contains("owner permissions")|| pm.Permission[storeId].Contains("founder permissions"))
                                    {
                                        if (result.ContainsKey(storeId) == false)
                                            result.Add(storeId, new List<Member>());
                                        pm.AwaitingNotification = new List<DomainLayer.Notification>(); //cause we update the notification list of the members that notificationSystemHolds
                                        result[storeId].Add(pm);
                                    }

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
                    throw new Exception(DbErrorMessage);
                }
                return result;
            }
        }

        public void MarkNotificatinAsRead(DomainLayer.Notification notification)
        {
            lock (this)
            {
                try
                {
                    using (var db = DatabaseContextFactory.ConnectToDatabase())
                    {
                        try
                        {
                            var notificationExist = db.notfications.FirstOrDefault(n => n.NotificationID.Equals(notification.NotificationID));
                            if (notificationExist != null)
                            {
                                db.Entry(notificationExist).State = EntityState.Detached;
                                db.notfications.Update(notification);
                                db.SaveChanges(true);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("failed to interact with notification table");
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(DbErrorMessage);
                }
            }
        }

        public List<DomainLayer.Notification> GetNotifications(Guid userId)
        {
            List<DomainLayer.Notification> result = new List<Notification>() ;

            lock (this)
            {
                try
                {
                    using (var db = DatabaseContextFactory.ConnectToDatabase())
                    {
                        try
                        {
                            var notifications = db.notfications.Where(notification => notification.SentTo.Equals(userId));

                            if (notifications != null)
                            {
                                foreach (DomainLayer.Notification n in notifications)
                                {
                                    result.Add(n);
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
                    throw new Exception(DbErrorMessage);
                }
                return result;
            }
        }
        #endregion

        #endregion



        public void addCond( ConditionDB cond, Store store)
        {
            lock (this)
            {
                try
                {
                    using (var db = DatabaseContextFactory.ConnectToDatabase())
                    {

                        try
                        {
                            // update store cond counter
                            //var storeInDB = db.Stores.FirstOrDefault(s => s.StoreID.Equals(store.StoreID));
                            try
                            {
                                db.Stores.Update(store);
                                db.SaveChanges(true);
                            }
                            catch (Exception ex)
                            {
                                // store was not add to db
                                AddStore(store);
                            }
                            var condition = db.conditions;
                            condition.Add(cond);
                            db.SaveChanges(true);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("failed to add cond");
                        }
                    }
                }
                catch(Exception ex)
                {
                    throw new Exception(DbErrorMessage);
                }
            }
        }

        public void RemoveCond(int condID, Store store)
        {
            lock (this)
            {
                try
                {
                    using (var db = DatabaseContextFactory.ConnectToDatabase())
                    {
                        try
                        {
                            // update store cond counter
                            //var storeInDB = db.Stores.FirstOrDefault(s => s.StoreID.Equals(store.StoreID));
                            try
                            {
                                db.Stores.Update(store);
                                db.SaveChanges(true);
                            }
                            catch (Exception ex)
                            {
                                // store was not add to db
                                AddStore(store);
                            }

                            var cond = db.conditions.FirstOrDefault(c => c.ID.Equals(condID) && c.StoreID.Equals(store.StoreID));
                            if (cond != null)
                            {
                                db.conditions.Remove(cond);
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
                    throw new Exception(DbErrorMessage);
                }
            }
        }

        public bool IsSystemInitialized()
        {
            bool result = false;
            lock (this)
            {
                try
                {
                    using (var db = DatabaseContextFactory.ConnectToDatabase())
                    {
                        try
                        {
                            var initialized = db.initializeSystems.FirstOrDefault();
                            if (initialized != null)
                                result = initialized.IsInit;
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("failed to interact with system initialized table");
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(DbErrorMessage);
                }
            }
            return result;
        }

        public void AddPolicy(int id, Store store)
        {
            lock (this)
            {
                try
                {
                    using (var db = DatabaseContextFactory.ConnectToDatabase())
                    {
                        try
                        {
                            var pol = db.policies.FirstOrDefault(c => c.ID.Equals(id) && c.StoreId.Equals(store.StoreID));
                            if (pol != null)
                            {
                                pol.activated = true;
                                db.policies.Update(pol);
                                db.SaveChanges(true);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("failed to interact with policies table");
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(DbErrorMessage);
                }
            }
        }

        public void InsertNewPolicy(PolicyDB pol, Store store)
        {
            lock (this)
            {
                try
                {
                    using (var db = DatabaseContextFactory.ConnectToDatabase())
                    {
                        try
                        {
                            
                            var policies = db.policies;
                            policies.Add(pol);
                            db.SaveChanges(true);

                            // update store cond counter
                            //var storeInDB = db.Stores.FirstOrDefault(s => s.StoreID.Equals(store.StoreID));
                            try
                            {
                                db.Stores.Update(store);
                                db.SaveChanges(true);
                            }
                            catch (Exception ex)
                            {
                                // store was not add to db
                                AddStore(store);
                            }

                        }
                        catch (Exception ex)
                        {
                            throw new Exception("failed to add cond");
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(DbErrorMessage);
                }
            }
        }

        public void RemovePolicy(DiscountPolicy policy, Store store)
        {
            lock (this)
            {
                try
                {
                    using (var db = DatabaseContextFactory.ConnectToDatabase())
                    {
                        int iD = policy.ID;
                        try
                        {
                            var pol = db.policies.FirstOrDefault(c => c.StoreId.Equals(store.StoreID) && c.ID.Equals(iD));
                            if (pol != null && pol.Discriminator.Equals("Complex"))
                            {
                                // remove all related policies
                                List<int> idsToRemove = policy.relatedDiscountPoliciesIds();
                                if(idsToRemove != null)
                                {
                                    foreach(int id in idsToRemove)
                                    {
                                        var rpol = db.policies.FirstOrDefault(c => c.StoreId.Equals(store.StoreID) && c.ID.Equals(id));
                                        if(rpol != null)
                                        {
                                            db.policies.Remove(rpol);
                                            db.SaveChanges(true);
                                        }

                                    }
                                }

                                db.policies.Remove(pol);
                                db.SaveChanges(true);
                            }
                            else if(pol != null)
                            {
                                //bool shouldRemove = true;
                                //// check if there is another policy involved
                                //int checkId = pol.ID;
                                //List<PolicyDB> complexPolices = db.policies.Where(p => p.StoreId.Equals(store.StoreID) && p.Discriminator.Equals("Complex")).ToList();
                                //foreach(PolicyDB p in complexPolices)
                                //{
                                //    if (p.complex_policys.Contains(checkId))
                                //        shouldRemove = false;
                                //}
                                //if(shouldRemove)
                                //{
                                    db.policies.Remove(pol);
                                    db.SaveChanges(true);
                                //}
                            }

                            // update store cond counter
                            //var storeInDB = db.Stores.FirstOrDefault(s => s.StoreID.Equals(store.StoreID));
                            try
                            {
                                db.Stores.Update(store);
                                db.SaveChanges(true);
                            }
                            catch (Exception ex)
                            {
                                // store was not add to db
                                AddStore(store);
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
                    throw new Exception(DbErrorMessage);
                }
            }
        }
    }
}
