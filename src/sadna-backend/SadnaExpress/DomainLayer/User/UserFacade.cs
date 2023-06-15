using SadnaExpress.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using SadnaExpress.DomainLayer.Store;
using System.Security.Cryptography.X509Certificates;
using SadnaExpress.ServiceLayer;
using System.Threading.Tasks;
using NodaTime;
using SadnaExpress.DataLayer;
using SadnaExpress.ServiceLayer.Obj;
using SadnaExpress.API.SignalR;
using SadnaExpress.ServiceLayer.SModels;
using System.Linq;

namespace SadnaExpress.DomainLayer.User
{
    public class UserFacade : IUserFacade
    {
        private const int MaxExternalServiceWaitTime = 10000; //10 seconds is 10,000 mili seconds
        private ConcurrentDictionary<Guid, User> current_Users; //users that are in the system and not login
        public ConcurrentDictionary<Guid, Member> members; //all the members that are registered to the system
        private ConcurrentDictionary<Guid, string> macs;

        private readonly string guestEmail = "guest";
        private readonly string purchaseNotificationForBuyer = "Your purchase completed successfully, thank you for buying at Sadna Express!";
        private bool _isTSInitialized;
        private IPasswordHash _ph = new PasswordHash();
        private IRegistration _reg = new Registration();
        private IPaymentService paymentService;
        public IPaymentService PaymentService { get => paymentService; set => paymentService = value; }
        private ISupplierService supplierService;
        public ISupplierService SupplierService { get => supplierService; set => supplierService = value; }

        public UserFacade(IPaymentService paymentService = null, ISupplierService supplierService = null)
        {
            current_Users = new ConcurrentDictionary<Guid, User>();
            members = new ConcurrentDictionary<Guid, Member>();
            macs = new ConcurrentDictionary<Guid, string>();
            this.paymentService = paymentService;
            this.supplierService = supplierService;
            _isTSInitialized = false;
        }

        public UserFacade(ConcurrentDictionary<Guid, User> current_Users, ConcurrentDictionary<Guid, Member> members, ConcurrentDictionary<Guid, string> macs, PasswordHash ph, IPaymentService paymentService=null, ISupplierService supplierService = null)
        {
            this.current_Users = current_Users;
            this.members = members;
            this.macs = macs;
            _ph = ph;
            this.paymentService = paymentService;
            this.supplierService = supplierService;
            _isTSInitialized = false;
        }

        public Guid Enter()
        {
            User user = new User();
            current_Users.TryAdd(user.UserId, user);
            UserUsageData.Instance.AddGuestVisit(user);
            Logger.Instance.Info(user.UserId, nameof(UserFacade) + ": " + nameof(Enter) + ": enter the system as guest.");
            return user.UserId;
        }

        public void Exit(Guid id)
        {
            User user;
            Member member;
            if (current_Users.TryRemove(id, out user))
            {
                Logger.Instance.Info(id, nameof(UserFacade) + ": " + nameof(Exit) + ": exited from the system.");
                user.RemoveBids();
            }
            else if (members.ContainsKey(id))
            {
                lock (members[id])
                {
                    Logout(id, true);
                    Logger.Instance.Info(id, nameof(UserFacade) + ": " + nameof(Exit) + ": exited from the system.");
                }
            }
            else
                throw new Exception("Error with exiting system with this id- " + id);

        }

        public void Register(Guid id, string email, string firstName, string lastName, string password)
        {
            IsTsInitialized();
            if (current_Users.ContainsKey(id) == false)
                throw new Exception("User should enter the system before preforming this action");

            if (DBHandler.Instance.memberExistsById(id))
                throw new Exception("Member with this id already registered");

            if (members.ContainsKey(id))
                throw new Exception("Member with this id already registered");

            if (!_reg.ValidateEmail(email))
                throw new Exception("Email does not meet the system criteria");

            if (DBHandler.Instance.memberExistsByEmail(email.ToLower()))
                throw new Exception("Member with this email already exists");

            if (!_reg.ValidateStrongPassword(password))
                throw new Exception("Password is not strong enough according to the system's criteria");

            String internedKey = String.Intern(email.ToLower());

            lock (internedKey)
            {

                foreach (Member m in members.Values)
                {
                    if (m.Email.ToLower().Equals(email.ToLower()))
                        throw new Exception("Member with this email already exists");

                }


                Guid newID = Guid.NewGuid();
                string newMac = _ph.Mac();
                macs.TryAdd(newID, newMac);
                string hashPassword = _ph.Hash(password + newMac);
                Member newMember = new Member(newID, email, firstName, lastName, hashPassword);

                members.TryAdd(newID, newMember);

                // add member to database
                DBHandler.Instance.AddMember(newMember, newMac);

                Logger.Instance.Info(newMember.UserId, nameof(UserFacade) + ": " + nameof(Register) + ": registered with " + email + ".");
            }
        }

        public Guid Login(Guid id, string email, string password)
        {
            bool foundInMembers = false;

            if (members.ContainsKey(id) && members[id].LoggedIn)
                throw new Exception($"Hi {members[id].FirstName} you are already logged in!");

            if (current_Users.ContainsKey(id) == false)
                throw new Exception("User should enter the system before logging in");

            email = email.ToLower();
            foreach (Member member in members.Values)
            {
                if (member.Email.ToLower().Equals(email.ToLower()))
                {
                    foundInMembers = true;
                    if (!_ph.Rehash(password + macs[member.UserId], member.Password))
                    {
                        throw new Exception("Incorrect email or password");
                    }
                    //correct email & password:
                    if (member.LoggedIn == true)
                        throw new Exception($"Hi {member.FirstName} you are already logged in!");

                    String internedKey = String.Intern(member.UserId.ToString());

                    lock (internedKey)
                    {
                        if (members.ContainsKey(member.UserId))
                        {
                            if (_isTSInitialized == false) //if user id not system manager and system is not initialized user cannot login
                                IsTSSystemManagerID(member.UserId);
                            member.LoggedIn = true;
                            User user;
                            current_Users.TryRemove(id, out user);

                            member.deepCopy(user); //handle shopping cart and bids

                            //members.TryAdd(member.UserId, member);

                            DBHandler.Instance.MemberLogIn(member);
                            // add member ShoppingCart and *todo add bids in DB
                            DBHandler.Instance.UpdateMemberShoppingCart(member);

                        }
                        else
                        {
                            throw new Exception("Incorrect email or password");
                        }
                    }

                    UserUsageData.Instance.AddMemberVisit(id, member);
                    Logger.Instance.Info($"{member} {member.Email} logged in");
                    return member.UserId;
                }
            }

            if (foundInMembers == false) //user is not in members list - maybe user is in db
            {
                var memberFromDB = DBHandler.Instance.CheckMemberValidLogin(id, email, password, _ph);
                if (memberFromDB != null)
                {
                    if (_isTSInitialized == false) //if user id not system manager and system is not initialized user cannot login
                        IsTSSystemManagerID(memberFromDB.UserId);

                    memberFromDB.LoggedIn = true;
                    User user;
                    current_Users.TryRemove(id, out user);

                    memberFromDB.deepCopy(user); //handle shopping cart and bids

                    // add member ShoppingCart and *todo add bids in DB
                    DBHandler.Instance.UpdateMemberShoppingCart(memberFromDB);

                    if (memberFromDB.Discriminator.Equals("PromotedMember")) // check if this member is store founder and add him to founders
                    {
                        PromotedMember pm = (PromotedMember)memberFromDB;
                        members.TryAdd(pm.UserId, pm);
                        macs.TryAdd(pm.UserId, DBHandler.Instance.GetMacById(pm.UserId));
                        LoadPromotedMemberCoWorkersFromDB((PromotedMember)members[pm.UserId]); //we need that each member will hold valid info on their superriors and appointers
                    }
                    else
                    {
                        members.TryAdd(memberFromDB.UserId, memberFromDB);
                        macs.TryAdd(memberFromDB.UserId, DBHandler.Instance.GetMacById(memberFromDB.UserId));
                    }

                    UserUsageData.Instance.AddMemberVisit(id, members[memberFromDB.UserId]);
                    Logger.Instance.Info($"{memberFromDB} {memberFromDB.Email} logged in");
                    return memberFromDB.UserId;
                }
            }

            //email not found
            throw new Exception("Incorrect email or password");
        }

        public Guid Logout(Guid id, bool exited = false)
        {
            if (!members.ContainsKey(id))
                throw new Exception("member with id dosen't exist");
            lock (members[id])
            {
                Member member = members[id];
                if (member.LoggedIn == false)
                    throw new Exception("member is already logged out!");
                member.LoggedIn = false;
                DBHandler.Instance.MemberLogout(member);
                Logger.Instance.Info(member.UserId, nameof(UserFacade) + ": " + nameof(Logout) + " logged out as member");
            }
            if (exited == false) //member logout but did not exited
                return Enter(); //member logs out and a regular user enters the system instead  

            return Guid.Empty; //if member exited we do not need to send new id
        }

        public void AddItemToCart(Guid userID, Guid storeID, Guid itemID, int itemAmount)
        {
            IsTsInitialized();
            if (members.ContainsKey(userID))
            {
                isLoggedIn(userID);
                members[userID].AddItemToCart(storeID, itemID, itemAmount);
                DBHandler.Instance.UpdateMemberShoppingCart(members[userID]);
            }
            else
                current_Users[userID].AddItemToCart(storeID, itemID, itemAmount);
            Logger.Instance.Info(userID, nameof(UserFacade) + ": " + nameof(AddItemToCart) + "Item " + itemID + "X" + itemAmount + " to store " + storeID + " by user " + userID);
        }

        public void RemoveItemFromCart(Guid userID, Guid storeID, Guid itemID)
        {
            IsTsInitialized();

            if (members.ContainsKey(userID) && isLoggedIn(userID))
            {
                if (!isLoggedIn(userID))
                    throw new Exception("the user is not logged in");
                members[userID].RemoveItemFromCart(storeID, itemID);
                DBHandler.Instance.UpdateMemberShoppingCart(members[userID]);
            }
            else
                current_Users[userID].RemoveItemFromCart(storeID, itemID);
            Logger.Instance.Info(userID, nameof(UserFacade) + ": " + nameof(RemoveItemFromCart) + "Item " + itemID + "removed from store " + storeID + " by user " + userID);
        }

        public void EditItemFromCart(Guid userID, Guid storeID, Guid itemID, int itemAmount)
        {
            IsTsInitialized();

            if (members.ContainsKey(userID))
            {
                if (!isLoggedIn(userID))
                    throw new Exception("the user is not logged in");
                members[userID].EditItemFromCart(storeID, itemID, itemAmount);
                DBHandler.Instance.UpdateMemberShoppingCart(members[userID]);
            }
            else
                current_Users[userID].EditItemFromCart(storeID, itemID, itemAmount);
            Logger.Instance.Info(userID, nameof(UserFacade) + ": " + nameof(EditItemFromCart) + "Item " + itemID + "X" + itemAmount + " updated in store " + storeID + " by user " + userID);
        }

        public ShoppingCart GetDetailsOnCart(Guid userID)
        {
            IsTsInitialized();
            if (members.ContainsKey(userID))
                return members[userID].ShoppingCart;
            Logger.Instance.Info(userID, nameof(UserFacade) + ": " + nameof(GetDetailsOnCart) + " ask to displays his shopping cart");
            return current_Users[userID].ShoppingCart;
        }

        public void PurchaseCart(DatabaseContext db, Guid userID)
        {
            String internedKey = String.Intern(userID.ToString());

            lock (internedKey)
            {
                if (members.ContainsKey(userID))
                {
                    Guid oldShopCartId = members[userID].ShoppingCart.ShoppingCartId;
                    members[userID].ShoppingCart = new ShoppingCart();
                    members[userID].ShoppingCart.ShoppingCartId = oldShopCartId;
                    DBHandler.Instance.UpdateMemberShoppingCartInTransaction(db, members[userID]);
                }
                else
                    current_Users[userID].ShoppingCart = new ShoppingCart();
            }
        }

        public void OpenNewStore(Guid userID, Guid storeID)
        {
            IsTsInitialized();
            isLoggedIn(userID);
            PromotedMember founder = members[userID].openNewStore(storeID);
            if (founder != null)
                members[userID] = founder;
            DBHandler.Instance.UpgradeMemberToPromotedMember((PromotedMember)members[userID]);

            NotificationSystem.Instance.RegisterObserver(storeID, members[userID]);


            Logger.Instance.Info(userID, nameof(UserFacade) + ": " + nameof(OpenNewStore) + " opened new store with id- " + storeID);
        }

        public void CheckIsValidMemberOperation(Guid userID)
        {
            IsTsInitialized();
            isLoggedIn(userID);
        }

        public void AddItemToStore(Guid id, Guid storeID)
        {
            IsTsInitialized();
            isLoggedIn(id);
            if (!members[id].hasPermissions(storeID, new List<string> { "product management permissions", "owner permissions", "founder permissions" }))
                throw new Exception("The user unauthorised to add add item to store");

        }

        public void RemoveItemFromStore(Guid id, Guid storeID)
        {
            IsTsInitialized();
            isLoggedIn(id);
            if (!members[id].hasPermissions(storeID, new List<string> { "product management permissions", "owner permissions", "founder permissions" }))
                throw new Exception("The user unauthorised to add add item to store");
        }

        public void EditItem(Guid id, Guid storeID)
        {
            IsTsInitialized();
            isLoggedIn(id);
            if (!members[id].hasPermissions(storeID, new List<string> { "product management permissions", "owner permissions", "founder permissions" }))
                throw new Exception("The user unauthorised to add add item to store");
        }

        public void RemovePermission(Guid userID, Guid storeID, string userEmail, string permission)
        {
            IsTsInitialized();
            isLoggedIn(userID);
            if (permission.Equals("owner permissions"))
                RemoveStoreOwner(userID, storeID, userEmail);
            else
            {
                RemoveStoreManagerPermissions(userID, storeID, userEmail, permission);
            }
        }

        public void AppointStoreOwner(Guid userID, Guid storeID, string email)
        {
            IsTsInitialized();
            isLoggedIn(userID);
            Guid newOwnerID = IsMember(email).UserId;
            //bring new owner member obj from members list or db (if from db new member is added to member's list)

            String internedKey = String.Intern(newOwnerID.ToString());

            lock (internedKey)
            {
                PromotedMember owner = members[userID].AppointStoreOwner(storeID, members[newOwnerID]);
                if (owner != null)
                {
                    members[newOwnerID] = owner;
                    DBHandler.Instance.UpgradeMemberToPromotedMember((PromotedMember)members[newOwnerID]);
                    NotificationSystem.Instance.RegisterObserver(storeID, owner);
                }
            }
            Logger.Instance.Info(userID, nameof(UserFacade) + ": " + nameof(AppointStoreOwner) + " appoints " + newOwnerID + " to new store owner");

        }
        public void ReactToJobOffer(Guid userID, Guid storeID, Guid newEmpID, bool offerResponse)
        {
            IsTsInitialized();
            isLoggedIn(userID);

            String internedKey = String.Intern(newEmpID.ToString());

            lock (internedKey)
            {
                PromotedMember owner = members[userID].ReactToJobOffer(storeID, members[newEmpID], offerResponse);
                if (owner != null)
                {
                    members[newEmpID] = owner;
                    DBHandler.Instance.UpgradeMemberToPromotedMember((PromotedMember)members[newEmpID]);
                    NotificationSystem.Instance.RegisterObserver(storeID, owner);
                }
            }
        }

        public void RemoveStoreOwner(Guid userID, Guid storeID, string email)
        {
            IsTsInitialized();
            isLoggedIn(userID);
            Guid storeOwnerID = IsMember(email).UserId;

            Logger.Instance.Info(userID, nameof(UserFacade) + ": " + nameof(AppointStoreManager) + " appoints " + storeOwnerID + " removed as store owner");

            Tuple<List<Member>, List<Member>, HashSet<Guid>> result = members[userID].RemoveStoreOwner(storeID, members[storeOwnerID]);
            List<Member> membersList = result.Item1;
            List<Member> StoreOwnersDeleted = result.Item2;
            HashSet<Guid> pendingPer = result.Item3;

            foreach (Member mem in membersList)
            {
                String internedKey = String.Intern(mem.UserId.ToString());
                lock (internedKey)
                {
                    members[mem.UserId] = mem;
                }
            }

            foreach (Guid memID in pendingPer)
            {
                foreach (Member oldOwner in StoreOwnersDeleted)
                {
                    GetMember(memID).RemoveEmployeeFromDecisions(storeID, oldOwner.Email);
                    PromotedMember mem = Permissions.Instance.PermissionApproved(storeID, (PromotedMember)members[userID], oldOwner);
                    if (mem != null)
                        members[memID] = mem;
                }
            }
            NotificationSystem.Instance.NotifyObservers(StoreOwnersDeleted, storeID, "Yow where removed as store owner", userID);
            NotificationSystem.Instance.RemoveObservers(storeID, StoreOwnersDeleted);


            Logger.Instance.Info(userID, nameof(UserFacade) + ": " + nameof(RemoveStoreOwner) + " appoints " + storeOwnerID + " removed as store owner");
        }

        public void AppointStoreManager(Guid userID, Guid storeID, string email)
        {
            IsTsInitialized();
            isLoggedIn(userID);

            Guid newManagerID = IsMember(email).UserId;

            String internedKey = String.Intern(newManagerID.ToString());
            lock (internedKey)
            {
                IsMember(email);
                PromotedMember manager = members[userID].AppointStoreManager(storeID, members[newManagerID]);
                members[newManagerID] = manager;
                DBHandler.Instance.UpgradeMemberToPromotedMember((PromotedMember)members[newManagerID]);
            }

            Logger.Instance.Info(userID, nameof(UserFacade) + ": " + nameof(AppointStoreManager) + " appoints " + newManagerID + " to new store manager");
        }

        public void AddStoreManagerPermissions(Guid userID, Guid storeID, string email, string permission)
        {
            IsTsInitialized();
            isLoggedIn(userID);
            Member manager = IsMember(email);

            String internedKey = String.Intern(manager.UserId.ToString());
            lock (internedKey)
            {
                members[userID].AddStoreManagerPermissions(storeID, manager, permission);
            }
            Logger.Instance.Info(userID, nameof(UserFacade) + ": " + nameof(AddStoreManagerPermissions) + "System added " + userID + " manager permissions in store " + storeID + ": " + permission);
        }

        public void RemoveStoreManagerPermissions(Guid userID, Guid storeID, string email, string permission)
        {
            IsTsInitialized();
            isLoggedIn(userID);
            Member manager = IsMember(email);
            String internedKey = String.Intern(manager.UserId.ToString());
            lock (internedKey)
            {
                Member member = members[userID].RemoveStoreManagerPermissions(storeID, manager, permission);
                if (member != null)
                    members[manager.UserId] = member;
            }
            Logger.Instance.Info(userID, nameof(UserFacade) + ": " + nameof(RemoveStoreManagerPermissions) + "System removed " + userID + " manager permissions in store " + storeID + ": " + permission);
        }

        public List<PromotedMember> GetEmployeeInfoInStore(Guid userID, Guid storeID)
        {
            IsTsInitialized();
            isLoggedIn(userID);
            List<PromotedMember> employees = members[userID].GetEmployeeInfoInStore(storeID);
            return employees;
        }

        public Bid PlaceBid(Guid userID, Guid storeID, Guid itemID, string itemName, double price)
        {
            IsTsInitialized();
            Bid bid = null;
            if (members.ContainsKey(userID))
            {
                isLoggedIn(userID);
                bid = members[userID].PlaceBid(storeID, itemID, itemName, price);
            }
            else
                bid = current_Users[userID].PlaceBid(storeID, itemID, itemName, price);
            
            Logger.Instance.Info(userID,
                nameof(UserFacade) + ": " + nameof(PlaceBid) + "Item " + itemName + "asked for new price " + price + " by user " + userID);
            return bid;
        }

        public void ReactToBid(Guid userID, Guid storeID, Guid bid, string bidResponse)
        {
            IsTsInitialized();
            isLoggedIn(userID);

            members[userID].ReactToBid(storeID, bid, bidResponse);

            Logger.Instance.Info(userID,
                nameof(UserFacade) + ": " + nameof(ReactToBid) + "Bid " + bid + "get bid response " + bidResponse + " by user " + userID);
        }

        public List<Bid> GetBidsInStore(Guid userID, Guid storeID)
        {
            IsTsInitialized();
            isLoggedIn(userID);

            Logger.Instance.Info(userID,
                nameof(UserFacade) + ": " + nameof(ReactToBid) + "get bid in store " + storeID + " by user " + userID);
            List<Bid> bidsWithoutNotExistsGuest = new List<Bid>();
            foreach (Bid bid in members[userID].GetBidsInStore(storeID))
            {
                if (bid.User.GetType() != typeof(User) || current_Users.ContainsKey(bid.User.UserId))
                    bidsWithoutNotExistsGuest.Add(bid);
            }

            return bidsWithoutNotExistsGuest;
        }

        public Dictionary<Guid, KeyValuePair<double, bool>> GetBidsOfUser(Guid userID)
        {
            IsTsInitialized();

            if (members.ContainsKey(userID))
            {
                isLoggedIn(userID);
                return members[userID].GetBidsOfUser();
            }
            return current_Users[userID].GetBidsOfUser();
        }

        public void RemoveUserMembership(Guid userID, string email)
        {
            IsTsInitialized();
            isLoggedIn(userID);
            if (members[userID].hasPermissions(Guid.Empty, new List<string> { "system manager permissions" }))
            {
                Member memberToRemove = IsMember(email);
                String internedKey = String.Intern(memberToRemove.UserId.ToString());

                lock (internedKey)
                {
                    if (memberToRemove is PromotedMember)
                    {
                        throw new Exception($"The user {email} has permissions, its illegal to remove him");
                    }

                    members.TryRemove(memberToRemove.UserId, out memberToRemove);
                    DBHandler.Instance.RemoveMember(memberToRemove.UserId);
                    if (memberToRemove.LoggedIn)
                        current_Users.TryAdd(memberToRemove.UserId, new User(memberToRemove));
                }
            }
            else
                throw new Exception($"The user {members[userID].Email} is not system manager");
        }
        public void CloseStore(Guid userID, Guid storeID)
        {
            IsTsInitialized();
            isLoggedIn(userID);
            members[userID].CloseStore(storeID);
            Logger.Instance.Info(userID, nameof(UserFacade) + ": " + nameof(CloseStore) + " Closed store " + storeID);
        }

        public void GetStorePurchases(Guid userId, Guid storeId)
        {
            IsTsInitialized();
            isLoggedIn(userId);
            if (!members[userId].hasPermissions(storeId,
                    new List<string> { "get store history", "owner permissions", "founder permissions" }))
                throw new Exception("The member doesn’t have permissions to get store purchases");
        }

        public void GetAllStorePurchases(Guid userId)
        {
            IsTsInitialized();
            isLoggedIn(userId);
            if (!members[userId].hasPermissions(Guid.Empty, new List<string> { "system manager permissions" }))
                throw new Exception("The member doesn’t have permissions to get all stores purchases");
        }

        public void CleanUp()
        {
            current_Users.Clear();
            members.Clear();
            paymentService = null;
            supplierService = null;

            DBHandler.Instance.CleanDB();
        }

        public bool InitializeTradingSystem(Guid userID)
        {
            //functions steps:
            //1. check that this is id member
            //2. check that member is log in
            //3. check that member is system manager
            //4. check that there is connection to payment and supply services
            isLoggedIn(userID);
            
            //impl of 3- throw error if not
            if (members[userID].hasPermissions(Guid.Empty, new List<string> { "system manager permissions" }) == false)
                throw new Exception("Only the system manager can preform this action");

            if (_isTSInitialized)
                throw new Exception("Trading system is already initialized");
            bool servicesConnected = false;
            try //fort tal
            {
                servicesConnected = paymentService.Handshake() == "OK" && supplierService.Handshake()== "OK";
            }
            catch (Exception e)
            {
                throw new Exception("Communication with the external services is temporarily down, please try again in a few minutes");

            }

            if (servicesConnected)
                _isTSInitialized = true;
            else
                throw new Exception("Trading system cannot be initialized");

            Logger.Instance.Info(userID, nameof(UserFacade) + ": " + nameof(InitializeTradingSystem) + "System Initialized");
            return servicesConnected;
        }

        public bool hasPermissions(Guid userID, Guid storeId, List<string> permissions)
        {
            if (members.ContainsKey(userID))
                if (members[userID].hasPermissions(storeId, permissions))
                    return true;
            return false;
        }

        public bool isLoggedIn(Guid userID)
        {
            if (members.ContainsKey(userID))
            {
                lock (members[userID])
                {
                    if (members[userID].LoggedIn)
                        return true;
                    throw new Exception("member need to login");
                }
            }
            throw new Exception("User need to register first");
        }

        private Member IsMember(string email)
        {
            foreach (Member member in members.Values)
                if (member.Email.ToLower() == email.ToLower())
                    return member;

            //members is not in members list so we should check in db
            Member memberFromDb = DBHandler.Instance.GetMemberFromDBByEmail(email);

            if (memberFromDb != null)
            {
                if (memberFromDb is PromotedMember)
                {
                    members.TryAdd(memberFromDb.UserId, (PromotedMember)memberFromDb);
                    LoadPromotedMemberCoWorkersFromDB((PromotedMember)members[memberFromDb.UserId]); //we need that each member will hold valid info on their superriors and appointers
                }
                else
                    members.TryAdd(memberFromDb.UserId, memberFromDb);
                macs.TryAdd(memberFromDb.UserId, DBHandler.Instance.GetMacById(memberFromDb.UserId));

                return members[memberFromDb.UserId];
            }

            throw new Exception($"There isn't a member with the email: {email}");
        }

        public ConcurrentDictionary<Guid, User> GetCurrent_Users()
        {
            return current_Users;
        }

        public ConcurrentDictionary<Guid, Member> GetMembers(Guid userID)
        {
            if (members[userID].hasPermissions(Guid.Empty, new List<string> { "system manager permissions" }))
            {
                Logger.Instance.Info(userID, nameof(UserFacade) + ": requested to display all members");

                return DBHandler.Instance.GetAllMembers();
                //return members;
            }

            throw new Exception("User doesn't have permissions to preform this operation");
        }

        public void SetPaymentService(IPaymentService paymentService)
        {
            this.paymentService = paymentService;
        }

        public void SetSupplierService(ISupplierService supplierService)
        {
            this.supplierService = supplierService;
        }

        public int PlacePayment(double amount, SPaymentDetails transactionDetails)
        {
            try
            {
                if (!transactionDetails.ValidationSettings())
                    throw new TimeoutException("Details of payment isn't valid");
                Logger.Instance.Info(nameof(paymentService) + ": request to preform a payment with details : " + transactionDetails);

                var task = Task.Run(() =>
                {
                    if (paymentService.Handshake() != "OK")
                        throw new Exception("Communication with the external services is temporarily down, please try again in a few minutes");
                    return paymentService.Pay(amount, transactionDetails);
                });

                if (task.Result == -1)
                    throw new Exception("Error in payment details , please try again");

                bool isCompletedSuccessfully = task.Wait(TimeSpan.FromMilliseconds(MaxExternalServiceWaitTime)) && task.Result != -1;

                if (isCompletedSuccessfully)
                {
                    int transaction_id = task.Result;
                    Logger.Instance.Info(nameof(UserFacade) + ": " + nameof(PlacePayment) + "Place payment completed with amount of " + amount + " and " + transactionDetails);
                    return transaction_id;
                }
                else
                {
                    throw new TimeoutException("Payment external service action has taken longer than the maximum time allowed.");
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return -1;
            }
        }

        public bool CancelPayment(double amount, int transaction_id)
        {
            return paymentService.Cancel_Pay(amount, transaction_id);
        }

        public List<Notification> GetNotifications(Guid userId)
        {
            if (members.ContainsKey(userId))
                return members[userId].AwaitingNotification;
            throw new Exception("Member with id " + userId + " does not exist");
        }

        public void MarkNotificationAsRead(Guid userID, Guid notificationID)
        {
            members[userID].MarkNotificationAsRead(notificationID);
        }

        public List<Member> getAllStoreOwners(ConcurrentDictionary<Guid, Store.Store> stores)
        {
            List<Member> storeOwners = new List<Member>();
            foreach (Member member in members.Values)
            {
                foreach (Guid storeID in stores.Keys)
                {
                    if (members[member.UserId].hasPermissions(storeID, new List<string> { "owner permissions" })) ;
                    storeOwners.Add(member);
                }
            }

            return storeOwners;
        }

        public List<Member> GetStoreOwnerOfStores(List<Guid> stores)
        {
            List<Member> storeOwners = new List<Member>();
            foreach (Member member in members.Values)
            {
                foreach (Guid storeID in stores)
                {
                    if (members[member.UserId].hasPermissions(storeID, new List<string> { "owner permissions" })) ;
                    storeOwners.Add(member);
                }
            }

            return storeOwners;

        }

        public int PlaceSupply(SSupplyDetails userDetails)
        {
            try
            {
                if (!userDetails.ValidationSettings())
                    throw new TimeoutException("Details of payment isn't valid");
                Logger.Instance.Info(nameof(supplierService) + ": user: " + userDetails + " request to preform a supply for order: ");//add SSupplyDetails.toString();

                var task = Task.Run(() =>
                {
                    if (supplierService.Handshake() != "OK")
                        throw new Exception("Communication with the external services is temporarily down, please try again in a few minutes");
                    return supplierService.Supply(userDetails);
                });

                bool isCompletedSuccessfully = task.Wait(TimeSpan.FromMilliseconds(MaxExternalServiceWaitTime)) && task.Result != -1; ;

                if (isCompletedSuccessfully)
                {
                    int transaction_id = task.Result;
                    Logger.Instance.Info(nameof(UserFacade) + ": " + nameof(PlaceSupply) + "Place supply completed: " + userDetails + " , "); //add SSupplyDetails.toString();
                    return transaction_id;
                }
                else
                {
                    throw new TimeoutException("Supply external service action has taken longer than the maximum time allowed.");
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return -1;
            }
        }

        private bool IsTSSystemManagerID(Guid userID)
        {
            if (members.ContainsKey(userID))
                if (members[userID].hasPermissions(Guid.Empty, new List<string> { "system manager permissions" }) == false)
                    throw new Exception("System is not initialized");
            return true;
        }

        public bool IsUserAdmin(Guid userID)
        {
            if (members.ContainsKey(userID))
                if (members[userID].hasPermissions(Guid.Empty, new List<string> { "system manager permissions" }))
                    return true;
            return false;
        }

        private void IsTsInitialized()
        {
            if (_isTSInitialized == false)
                throw new Exception("Cannot preform any action because system trading is closed");
        }

        public void SetIsSystemInitialize(bool isInitialize)
        {
            _isTSInitialized = isInitialize;
        }

        public User GetUser(Guid userID)
        {
            if (current_Users.ContainsKey(userID))
                return current_Users[userID];
            throw new Exception("User with id " + userID + " does not exist");
        }

        public Member GetMember(Guid userID)
        {
            if (members.ContainsKey(userID))
                return members[userID];

            //members is not in members list so we should check in db
            Member memberFromDb = DBHandler.Instance.GetMemberFromDBById(userID);

            if (memberFromDb != null)
            {
                if (memberFromDb is PromotedMember)
                {
                    members.TryAdd(memberFromDb.UserId, (PromotedMember)memberFromDb);
                    LoadPromotedMemberCoWorkersFromDB((PromotedMember)members[memberFromDb.UserId]); //we need that each member will hold valid info on their superriors and appointers
                }
                else
                    members.TryAdd(memberFromDb.UserId, memberFromDb);
                macs.TryAdd(memberFromDb.UserId, DBHandler.Instance.GetMacById(memberFromDb.UserId));

                return members[memberFromDb.UserId];
            }

            throw new Exception("Member with id " + userID + " does not exist");
        }

        public Member GetMember(String email)
        {
            foreach(Member member in members.Values) {
                if (member.Email.Equals(email))
                    return member;
            }

      
            //members is not in members list so we should check in db
            Member memberFromDb = DBHandler.Instance.GetMemberFromDBByEmail(email);

            if (memberFromDb != null)
            {
                if (memberFromDb is PromotedMember)
                {
                    members.TryAdd(memberFromDb.UserId, (PromotedMember)memberFromDb);
                    LoadPromotedMemberCoWorkersFromDB((PromotedMember)members[memberFromDb.UserId]); //we need that each member will hold valid info on their superriors and appointers
                }
                else
                    members.TryAdd(memberFromDb.UserId, memberFromDb);
                macs.TryAdd(memberFromDb.UserId, DBHandler.Instance.GetMacById(memberFromDb.UserId));

                return members[memberFromDb.UserId];
            }

            throw new Exception("Member with id " + email + " does not exist");
        }
    
        public bool IsSystemInitialize()
        {
            return _isTSInitialized;
        }

        public int GetItemQuantityInCart(Guid userID, Guid storeID, Guid itemID)
        {
            if (current_Users.ContainsKey(userID))
                return current_Users[userID].ShoppingCart.GetItemQuantityInCart(storeID,itemID);
            if (isLoggedIn(userID))
                return members[userID].ShoppingCart.GetItemQuantityInCart(storeID, itemID);

            throw new Exception("User with id " + userID + " does not exist");
        }
        
        public string GetUserEmail(Guid userID)
        {
            if (members.ContainsKey(userID))
                return members[userID].Email;
            return guestEmail;

        }

        public ConcurrentDictionary<Guid, List<String>> GetMemberPermissions(Guid userID)
        {
            var member = GetMember(userID);
            if (member is PromotedMember)
            {
                var promoted = (PromotedMember)member;
                return promoted.Permission;
            }
            else
            {
                return new ConcurrentDictionary<Guid, List<string>>();
            }

        }

        public string Handshake()
        {
            return paymentService.Handshake();
        }

        public List<int> GetSystemUserActivity(Guid userID, DateTime fromDate, DateTime toDate)
        {

            if (members[userID].hasPermissions(Guid.Empty, new List<string> { "system manager permissions" }))
            {
                Logger.Instance.Info(userID, $" {nameof(UserFacade)} {nameof(GetSystemUserActivity)} system administrator requested to get System User Activity from {fromDate} to {toDate}");

                return UserUsageData.Instance.GetUserUsageData(fromDate, toDate);
            }

            throw new Exception("User doesn't have permissions to preform this operation");
        }

        public void NotifyBuyerPurchase(Guid userID, DatabaseContext db=null)
        {
            NotificationNotifier.GetInstance().SendNotification(userID, purchaseNotificationForBuyer);
            if (members.ContainsKey(userID))
                members[userID].Update(new Notification(DateTime.Now, userID, purchaseNotificationForBuyer, userID), db);
        }
        
        public void CreateSystemManager(Guid userID)
        {
            isLoggedIn(userID);
            PromotedMember systemManager = members[userID].promoteToMember();
            systemManager.createSystemManager();
            members[userID] = systemManager;
        }

        private void LoadPromotedMemberCoWorkersFromDB(PromotedMember pm) //LOAD all the tree of directive/appoint for the promoted member
        {
           
            foreach (Guid storeId in pm.DirectSupervisor.Keys)
            {
                PromotedMember directive = pm.DirectSupervisor[storeId];

                if (directive != null)
                {
                    if(members.ContainsKey(directive.UserId) == false) 
                    { //members list does not hold the member's directive
                        PromotedMember newDirective = (PromotedMember)DBHandler.Instance.GetMemberFromDBByEmail(directive.Email);
                        members.TryAdd(directive.UserId, newDirective);
                        macs.TryAdd(directive.UserId, DBHandler.Instance.GetMacById(directive.UserId));
                        PromotedMember toRemove;
                        pm.DirectSupervisor.TryRemove(storeId, out toRemove);
                        pm.DirectSupervisor.TryAdd(storeId, newDirective);
                        LoadPromotedMemberCoWorkersFromDB(newDirective);
                    }
                    else //direvtive is in members - means that directive is updated
                    {
                        PromotedMember toRemove;
                        pm.DirectSupervisor.TryRemove(storeId, out toRemove);
                        pm.DirectSupervisor.TryAdd(storeId, (PromotedMember)members[directive.UserId]);
                    }
                }
            }


            foreach (Guid storeId in pm.Appoint.Keys)
            {
                foreach (PromotedMember appoint in pm.Appoint[storeId].ToList())
                {
                    if (appoint != null)
                    {  
                        if (members.ContainsKey(appoint.UserId) == false) 
                        { //members list does not hold the member's appoint
                            PromotedMember newAppoint = (PromotedMember)DBHandler.Instance.GetMemberFromDBByEmail(appoint.Email);
                            members.TryAdd(appoint.UserId, newAppoint);
                            macs.TryAdd(newAppoint.UserId, DBHandler.Instance.GetMacById(newAppoint.UserId));
                            pm.Appoint[storeId].Remove(appoint);
                            pm.Appoint[storeId].Add(newAppoint);
                            LoadPromotedMemberCoWorkersFromDB((PromotedMember)members[appoint.UserId]);
                        }
                        else ////appoint is in members - means that directive is updated
                        {
                            pm.Appoint[storeId].Remove(appoint);
                            pm.Appoint[storeId].Add((PromotedMember)members[appoint.UserId]);
                        }
                    }
                }
            }
            
        }

    }
}