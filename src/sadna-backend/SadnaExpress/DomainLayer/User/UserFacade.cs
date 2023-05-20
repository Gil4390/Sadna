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

namespace SadnaExpress.DomainLayer.User
{
    public class UserFacade : IUserFacade
    {
        private const int MaxExternalServiceWaitTime = 10000; //10 seconds is 10,000 mili seconds
        private ConcurrentDictionary<Guid, User> current_Users; //users that are in the system and not login
        private ConcurrentDictionary<Guid, Member> members; //all the members that are registered to the system
        private ConcurrentDictionary<Guid, string> macs;

        private readonly string guestEmail = "guest";
        private bool _isTSInitialized;
        private IPasswordHash _ph = new PasswordHash();
        private IRegistration _reg = new Registration();
        private IPaymentService paymentService;
        public IPaymentService PaymentService { get => paymentService; set => paymentService = value; }
        private ISupplierService supplierService;
        public ISupplierService SupplierService { get => supplierService; set => supplierService = value; }


        public UserFacade(IPaymentService paymentService=null, ISupplierService supplierService =null)
        {
            current_Users = new ConcurrentDictionary<Guid, User>();
            members = new ConcurrentDictionary<Guid, Member>();
            macs = new ConcurrentDictionary<Guid, string>();
            this.paymentService = paymentService;
            this.supplierService = supplierService;
            _isTSInitialized = false;
        }

        public UserFacade(ConcurrentDictionary<Guid, User> current_Users, ConcurrentDictionary<Guid, Member> members,ConcurrentDictionary<Guid, string> macs, PasswordHash ph, IPaymentService paymentService=null, ISupplierService supplierService = null)
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
            Logger.Instance.Info(user.UserId , nameof(UserFacade)+": "+nameof(Enter)+": enter the system as guest.");
            return user.UserId;
            
        }

        public void Exit(Guid id)
        { 
            User user;
            Member member;
            if (current_Users.TryRemove(id, out user))
                Logger.Instance.Info(id, nameof(UserFacade) + ": " + nameof(Exit) + ": exited from the system.");
            else if (members.ContainsKey(id))
            {
                lock (members[id])
                {
                    Logout(id);
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
            if(DBHandler.Instance.memberExistsById(id))
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

                Logger.Instance.Info(newMember.UserId, nameof(UserFacade) + ": " + nameof(Register) + ": registered with " + email +".");
            }
        }

        public Guid Login(Guid id, string email, string password)
        {
            var memberFromDB = DBHandler.Instance.CheckMemberValidLogin(id, email, password, _ph);
            if (memberFromDB != null)
            {
                members.TryAdd(memberFromDB.UserId, memberFromDB);
                macs.TryAdd(memberFromDB.UserId, DBHandler.Instance.GetMacById(memberFromDB.UserId));
            }
            if (members.ContainsKey(id) && members[id].LoggedIn)
                throw new Exception($"Hi {members[id].FirstName} you are already logged in!");

            if (current_Users.ContainsKey(id)==false)
                throw new Exception("User should enter the system before logging in");

            email = email.ToLower();
            foreach (Member member in members.Values)
            {
                if (member.Email.ToLower().Equals(email.ToLower()))
                {
                    if (!_ph.Rehash(password+macs[member.UserId], member.Password))
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
                            member.ShoppingCart.AddUserShoppingCart(user.ShoppingCart);
                            
                            // add member ShoppingCart in DB
                            DBHandler.Instance.UpdateMemberShoppingCart(member);
                        }
                        else
                        {
                            throw new Exception("Incorrect email or password");
                        }
                    }
                    Logger.Instance.Info($"{member} {member.Email} logged in");
                    return member.UserId;
                }
            }
            //email not found
            throw new Exception("Incorrect email or password");
        }

        public Guid Logout(Guid id)
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
            return Enter(); //member logs out and a regular user enters the system instead  
        }

        public void AddItemToCart(Guid userID, Guid storeID, Guid itemID, int itemAmount)
        {
            IsTsInitialized();
            if (members.ContainsKey(userID)){
                if (!isLoggedIn(userID))
                        throw new Exception("the user is not logged in");
                members[userID].AddItemToCart(storeID, itemID, itemAmount);
                DBHandler.Instance.UpdateMemberShoppingCart(members[userID]);
            }
            else 
                current_Users[userID].AddItemToCart(storeID, itemID, itemAmount);
            Logger.Instance.Info(userID, nameof(UserFacade)+": "+nameof(AddItemToCart)+"Item "+itemID+"X"+itemAmount +" to store "+storeID+ " by user "+userID);
        }

        public void RemoveItemFromCart(Guid userID, Guid storeID, Guid itemID)
        {
            IsTsInitialized();
            
            if (members.ContainsKey(userID)&& isLoggedIn(userID))
            {
                if (!isLoggedIn(userID))
                    throw new Exception("the user is not logged in");
                members[userID].RemoveItemFromCart(storeID, itemID);
                DBHandler.Instance.UpdateMemberShoppingCart(members[userID]);
            }
            else 
                current_Users[userID].RemoveItemFromCart(storeID, itemID);
            Logger.Instance.Info(userID, nameof(UserFacade)+": "+nameof(RemoveItemFromCart)+"Item "+itemID+"removed from store "+storeID+ " by user "+userID);
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
            Logger.Instance.Info(userID, nameof(UserFacade)+": "+nameof(EditItemFromCart)+"Item "+itemID+"X"+itemAmount +" updated in store "+storeID+ " by user "+userID);
        }

        public ShoppingCart GetDetailsOnCart(Guid userID)
        {
            IsTsInitialized();
            if (members.ContainsKey(userID))
                return members[userID].ShoppingCart;
            Logger.Instance.Info(userID, nameof(UserFacade)+": "+nameof(GetDetailsOnCart)+" ask to displays his shopping cart");
            return current_Users[userID].ShoppingCart;
        }
        
        public void PurchaseCart(Guid userID)
        {
            String internedKey = String.Intern(userID.ToString());

            lock (internedKey)
            {
                if (members.ContainsKey(userID))
                    members[userID].ShoppingCart = new ShoppingCart();
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
            DBHandler.Instance.UpdatePromotedMember((PromotedMember)members[userID]);

            NotificationSystem.Instance.RegisterObserver(storeID, members[userID]);

            // todo register observer in database

            Logger.Instance.Info(userID, nameof(UserFacade)+": "+nameof(OpenNewStore)+" opened new store with id- " + storeID);

        }

    
        public void AddItemToStore(Guid id, Guid storeID)
        {
            IsTsInitialized();
            isLoggedIn(id);
            if (!members[id].hasPermissions(storeID, new List<string>{"product management permissions","owner permissions","founder permissions"}))
                throw new Exception("The user unauthorised to add add item to store");

        }

        public void RemoveItemFromStore(Guid id, Guid storeID)
        {
            IsTsInitialized();
            isLoggedIn(id);
            if (!members[id].hasPermissions(storeID, new List<string>{"product management permissions","owner permissions","founder permissions"}))
                throw new Exception("The user unauthorised to add add item to store");
        }

        public void EditItem(Guid id, Guid storeID)
        {
            IsTsInitialized();
            isLoggedIn(id);
            if (!members[id].hasPermissions(storeID, new List<string>{"product management permissions","owner permissions","founder permissions"}))
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

            String internedKey = String.Intern(newOwnerID.ToString());

            lock (internedKey)
            {
                IsMember(email);
                PromotedMember owner = members[userID].AppointStoreOwner(storeID, members[newOwnerID]);
                members[newOwnerID] = owner;
                NotificationSystem.Instance.RegisterObserver(storeID, owner);
            }
            Logger.Instance.Info(userID, nameof(UserFacade)+": "+nameof(AppointStoreOwner)+" appoints " +newOwnerID +" to new store owner");
            
        }

        public void RemoveStoreOwner(Guid userID, Guid storeID, string email)
        {
            IsTsInitialized();
            isLoggedIn(userID);
            Guid storeOwnerID = IsMember(email).UserId;
            
            Logger.Instance.Info(userID, nameof(UserFacade)+": "+nameof(AppointStoreManager)+" appoints " +storeOwnerID +" removed as store owner");
            
            Tuple<List<Member>, List<Member>> result = members[userID].RemoveStoreOwner(storeID, members[storeOwnerID]);
            List<Member> membersList = result.Item1;
            List<Member> StoreOwnersDeleted = result.Item2;

            
            foreach (Member mem in membersList)
            {
                String internedKey = String.Intern(mem.UserId.ToString());
                lock (internedKey)
                {
                    members[mem.UserId] = mem;
                }
            }

            foreach (Member mem in StoreOwnersDeleted)
            {
                NotificationSystem.Instance.NotifyObservers(StoreOwnersDeleted, storeID, "Yow where removed as store owner", userID);
            }

              
            NotificationSystem.Instance.RemoveObservers(storeID, StoreOwnersDeleted);
            

            Logger.Instance.Info(userID, nameof(UserFacade)+": "+nameof(RemoveStoreOwner)+" appoints " +storeOwnerID +" removed as store owner");
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
            }

            Logger.Instance.Info(userID, nameof(UserFacade)+": "+nameof(AppointStoreManager)+" appoints " +newManagerID +" to new store manager");
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
            Logger.Instance.Info(userID, nameof(UserFacade)+": "+nameof(AddStoreManagerPermissions)+"System added "+userID+" manager permissions in store "+storeID+": "+permission);
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
            Logger.Instance.Info(userID, nameof(UserFacade)+": "+nameof(RemoveStoreManagerPermissions)+"System removed "+userID+" manager permissions in store "+storeID+": "+permission);
        }

        public List<PromotedMember> GetEmployeeInfoInStore(Guid userID, Guid storeID)
        {
            IsTsInitialized();
            isLoggedIn(userID);
            List<PromotedMember> employees = members[userID].GetEmployeeInfoInStore(storeID);
            return employees;
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
                    if (IsMember(email).GetType() != typeof(Member))
                    {
                        throw new Exception($"The user {email} has permissions, its illegal to remove him");
                    }

                    members.TryRemove(memberToRemove.UserId, out memberToRemove);
                    if (memberToRemove.LoggedIn)
                        current_Users.TryAdd(memberToRemove.UserId, new User(memberToRemove));
                }
            }
            else
              throw new Exception($"The user {members[userID].Email} is not system manager");
        }
        public void UpdateFirst(Guid userID, string newFirst)
        {
            IsTsInitialized();
            isLoggedIn(userID);
            if (!members.ContainsKey(userID))
                throw new Exception("member with id dosen't exist");
            members[userID].FirstName = newFirst;
            Logger.Instance.Info(userID, nameof(UserFacade)+": "+nameof(UpdateFirst)+"First name updated");
        }

        public void UpdateLast(Guid userID, string newLast)
        {
            IsTsInitialized();
            isLoggedIn(userID);
            if (!members.ContainsKey(userID))
                throw new Exception("member with id dosen't exist");
            members[userID].LastName = newLast;
            Logger.Instance.Info(userID, nameof(UserFacade)+": "+nameof(UpdateLast)+"Last name updated");
        }

        public void UpdatePassword(Guid userID, string newPassword)
        {
            IsTsInitialized();
            isLoggedIn(userID);
            if (!members.ContainsKey(userID))
                throw new Exception("member with id dosen't exist");
            members[userID].Password = _ph.Hash(newPassword+macs[userID]);
            Logger.Instance.Info(userID, nameof(UserFacade)+": "+nameof(UpdatePassword)+"Password updated");
        }

        public void CloseStore(Guid userID, Guid storeID)
        {
            IsTsInitialized();
            isLoggedIn(userID);
            members[userID].CloseStore(storeID);
            Logger.Instance.Info(userID, nameof(UserFacade)+": "+nameof(CloseStore)+" Closed store "+storeID);
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
            if (!members[userId].hasPermissions(Guid.Empty, new List<string>{"system manager permissions"}))
                throw new Exception("The member doesn’t have permissions to get all stores purchases");   
        }

        public void CleanUp()
        {
            current_Users.Clear();
            members.Clear();
            paymentService = null;
            supplierService = null;

            DBHandler.Instance.TestMode();
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

            bool servicesConnected = paymentService.Connect() && supplierService.Connect();

            if(servicesConnected)
                _isTSInitialized = true;
            else
                throw new Exception("Trading system cannot be initialized");
            Logger.Instance.Info(userID, nameof(UserFacade)+": "+nameof(InitializeTradingSystem)+"System Initialized");
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
                Logger.Instance.Info(userID, nameof(UserFacade)+": requested to display all members");
                return members;
            }

            throw new Exception("Don't have permissions");
        }

        public void SetSecurityQA(Guid userID, string q, string a)
        {
            IsTsInitialized();
            isLoggedIn(userID);
            if (!members.ContainsKey(userID))
                throw new Exception("member with id dosen't exist");
            members[userID].SetSecurityQA(q,_ph.Hash(a+macs[userID]));
            Logger.Instance.Info(userID, nameof(UserFacade)+": "+nameof(SetSecurityQA)+"Security Q&A set");
        }

        public void SetPaymentService(IPaymentService paymentService)
        {
            this.paymentService = paymentService;
        }

        public void SetSupplierService(ISupplierService supplierService)
        {
            this.supplierService = supplierService;
        }

        public bool PlacePayment(double amount, string transactionDetails)
        {
            try
            {
                Logger.Instance.Info(nameof(paymentService)+": request to preform a payment with details : "+transactionDetails);

                var task = Task.Run(() =>
                {
                    return paymentService.Pay(amount,transactionDetails);
                });

                bool isCompletedSuccessfully = task.Wait(TimeSpan.FromMilliseconds(MaxExternalServiceWaitTime)) && task.Result;

                if (isCompletedSuccessfully)
                {
                    Logger.Instance.Info(nameof(UserFacade)+": "+nameof(SetSecurityQA)+"Place payment completed with amount of "+amount+" and "+transactionDetails);
                    return true;
                }
                else
                {
                    throw new TimeoutException("Payment external service action has taken longer than the maximum time allowed.");
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return false;
            }
        }

        public bool CancelPayment(double amount, string transactionDetails)
        {
            return paymentService.cancel(amount, transactionDetails);
        }

        public List<Notification> GetNotifications(Guid userId)
        {
            if(members.ContainsKey(userId))
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
            foreach(Member member in members.Values)
            {
                foreach(Guid storeID in stores.Keys)
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
            foreach(Member member in members.Values)
            {
                foreach(Guid storeID in stores)
                {
                    if (members[member.UserId].hasPermissions(storeID, new List<string> { "owner permissions" })) ;
                    storeOwners.Add(member);
                }
            }

            return storeOwners;
            
        }

        public bool PlaceSupply(string orderDetails, string userDetails)
        {
            try
            {
                Logger.Instance.Info(nameof(supplierService) + ": user: " + userDetails + " request to preform a supply for order: " + orderDetails);

                var task = Task.Run(() =>
                {
                    return supplierService.ShipOrder(orderDetails, userDetails);
                });

                bool isCompletedSuccessfully = task.Wait(TimeSpan.FromMilliseconds(MaxExternalServiceWaitTime))&& task.Result;;

                if (isCompletedSuccessfully)
                {
                    Logger.Instance.Info(nameof(UserFacade)+": "+nameof(SetSecurityQA)+"Place supply completed: "+ userDetails +" , "+orderDetails);
                    return true;
                }
                else
                {
                    throw new TimeoutException("Supply external service action has taken longer than the maximum time allowed.");
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return false;
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
            throw new Exception("Member with id " + userID + " does not exist");
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

        public void LoadData(Guid storeid1, Guid storeid2)
        {
            Guid systemManagerid = Guid.NewGuid();
            Guid memberId = Guid.NewGuid();
            Guid memberId2 = Guid.NewGuid();
            Guid memberId3 = Guid.NewGuid();
            Guid memberId4 = Guid.NewGuid();
            Guid memberId5 = Guid.NewGuid();
            Guid storeOwnerid1 = Guid.NewGuid();
            Guid storeManagerid1 = Guid.NewGuid();
            Guid storeOwnerid2 = Guid.NewGuid();
            Guid storeManagerid2 = Guid.NewGuid();
            Guid storeOwnerid3 = Guid.NewGuid();

            string newMac = _ph.Mac();
            
            macs.TryAdd(memberId, newMac);
            Member member = new Member(memberId, "gil@gmail.com", "Gil", "Gil", _ph.Hash("asASD876!@" + newMac));
            newMac = _ph.Mac();
            macs.TryAdd(memberId2, newMac);
            Member member2 = new Member(memberId2, "sebatian@gmail.com", "Sebatian", "Sebatian", _ph.Hash("asASD123!@" + newMac));
            newMac = _ph.Mac();
            macs.TryAdd(memberId3, newMac);
            Member member3 = new Member(memberId3, "amihai@gmail.com", "Amihai", "Amihai", _ph.Hash("asASD753!@" + newMac));
            newMac = _ph.Mac();
            macs.TryAdd(memberId4, newMac);
            Member member4 = new Member(memberId4, "bar@gmail.com", "Bar", "Bar", _ph.Hash("asASD159!@" + newMac));
            newMac = _ph.Mac();
            macs.TryAdd(memberId5, newMac);
            Member member5 = new Member(memberId5, "tal@gmail.com", "Tal", "Galmor", _ph.Hash("w3ka!Tal" + newMac));


            newMac = _ph.Mac();
            macs.TryAdd(systemManagerid, newMac);
            PromotedMember systemManager = new PromotedMember(systemManagerid, "RotemSela@gmail.com", "Rotem", "Sela", _ph.Hash("AS87654askj" + newMac));
            systemManager.createSystemManager();

            newMac = _ph.Mac();
            macs.TryAdd(storeOwnerid1, newMac);
            PromotedMember storeOwner1 = new PromotedMember(storeOwnerid1, "AsiAzar@gmail.com", "Asi", "Azar", _ph.Hash("A#!a12345678" + newMac));
            storeOwner1.createFounder(storeid1);

            newMac = _ph.Mac();
            macs.TryAdd(storeOwnerid3, newMac);
            PromotedMember storeOwner3 = new PromotedMember(storeOwnerid3, "nogaschw@post.bgu.ac.il", "Asi", "Azar", _ph.Hash("A#!a12345678" + newMac));
            storeOwner3.createFounder(storeid1);
            
            newMac = _ph.Mac();
            macs.TryAdd(storeOwnerid2, newMac);
            PromotedMember storeOwner2 = new PromotedMember(storeOwnerid2, "dani@gmail.com", "dani", "dani", _ph.Hash("A#!a12345678" + newMac));
            storeOwner2.createFounder(storeid2);

            newMac = _ph.Mac();
            macs.TryAdd(storeManagerid1, newMac);
            Member storeManager1 = new Member(storeManagerid1, "kobi@gmail.com", "kobi", "kobi", _ph.Hash("A#!a12345678" + newMac));

            newMac = _ph.Mac();
            macs.TryAdd(storeManagerid2, newMac);
            Member storeManager2 = new Member(storeManagerid2, "Yael@gmail.com", "Yael", "Yael", _ph.Hash("A#!a12345678" + newMac));


            members.TryAdd(systemManagerid, systemManager);
            members.TryAdd(memberId, member);
            members.TryAdd(memberId2, member2);
            members.TryAdd(memberId3, member3);
            members.TryAdd(memberId4, member4);
            members.TryAdd(memberId5, member5);
            members.TryAdd(storeOwnerid1, storeOwner1);
            members.TryAdd(storeOwnerid2, storeOwner2);
            members.TryAdd(storeManagerid1, storeManager1);
            members.TryAdd(storeManagerid2, storeManager2);
            
            //add managers
            storeOwner1.LoggedIn = true;
            storeOwner2.LoggedIn = true;
            AppointStoreManager(storeOwnerid1, storeid1, "kobi@gmail.com");
            AppointStoreManager(storeOwnerid2, storeid2, "Yael@gmail.com");
            storeOwner1.LoggedIn = false;
            storeOwner2.LoggedIn = false;



            members[memberId].AwaitingNotification.Add(new Notification(DateTime.Now, Guid.Empty, "helooooo", memberId));

            NotificationSystem.Instance.RegisterObserver(storeid1, storeOwner1);
            NotificationSystem.Instance.RegisterObserver(storeid2, storeOwner2);
        }
    }
}