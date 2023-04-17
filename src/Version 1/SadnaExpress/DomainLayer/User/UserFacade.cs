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

namespace SadnaExpress.DomainLayer.User
{
    public class UserFacade : IUserFacade
    {
        private const int MaxExternalServiceWaitTime = 10000; //10 seconds is 10,000 mili seconds
        private ConcurrentDictionary<Guid, User> current_Users; //users that are in the system and not login
        private ConcurrentDictionary<Guid, Member> members; //all the members that are registered to the system
        private bool _isTSInitialized;
        private IPasswordHash _ph = new PasswordHash();
        private IRegistration _reg = new Registration();
        private IPaymentService paymentService;
        public IPaymentService PaymentService { get => paymentService; set => paymentService = value; }
        private ISupplierService supplierService;
        public ISupplierService SupplierService { get => supplierService; set => supplierService = value; }
        object enterLock = new object();
        object registerLock = new object();

        public UserFacade(IPaymentService paymentService=null, ISupplierService supplierService =null)
        {
            current_Users = new ConcurrentDictionary<Guid, User>();
            members = new ConcurrentDictionary<Guid, Member>();
            this.paymentService = paymentService;
            this.supplierService = supplierService;
            _isTSInitialized = false;
        }

        public UserFacade(ConcurrentDictionary<Guid, User> current_Users, ConcurrentDictionary<Guid, Member> members, PasswordHash ph, IPaymentService paymentService=null, ISupplierService supplierService = null)
        {
            this.current_Users = current_Users;
            this.members = members;
            _ph = ph;
            this.paymentService = paymentService;
            this.supplierService = supplierService;
            _isTSInitialized = false;
        }

        public Guid Enter()
        {
            lock (enterLock)
            {
                User user = new User();
                current_Users.TryAdd(user.UserId, user);
                Logger.Instance.Info(user.UserId , nameof(UserFacade)+": "+nameof(Enter)+": enter the system as guest.");
                return user.UserId;
            }
        }

        public void Exit(Guid id)
        { 
            User user;
            Member member;
            if (current_Users.TryRemove(id, out user))
                Logger.Instance.Info(id, nameof(UserFacade) + ": " + nameof(Exit) + ": exited from the system.");
            else if (members.ContainsKey(id))
            {
                Logout(id);
                Logger.Instance.Info(id, nameof(UserFacade) + ": " + nameof(Exit) + ": exited from the system.");
            }
            else
                throw new Exception("Error with exiting system with this id- " + id);
            
        }

        public void Register(Guid id, string email, string firstName, string lastName, string password)
        {
            lock (registerLock)
            {
                IsTsInitialized();
                if (current_Users.ContainsKey(id) == false)
                    throw new Exception("User should enter the system before preforming this action");

                if (members.ContainsKey(id))
                    throw new Exception("Member with this id already registered");

                foreach (Member m in members.Values)
                {
                    if (m.Email == email)
                        throw new Exception("Member with this email already exists");
                }
                if (!_reg.ValidateEmail(email))
                    throw new Exception("Email does not meet the system criteria");
                if (!_reg.ValidateStrongPassword(password))
                    throw new Exception("Password is not strong enough according to the system's criteria");
                
                string hashPassword = _ph.Hash(password);
                Member newMember = new Member(id, email, firstName, lastName, hashPassword);
                members.TryAdd(id, newMember);
                Logger.Instance.Info(newMember.UserId, nameof(UserFacade)+": "+nameof(Register)+": registered with " + email + " and password "+password+".");
            }
        }

        public Guid Login(Guid id, string email, string password)
        {
            if (_isTSInitialized == false) //if user id not system manager and system is not initialized user cannot login
                IsTSSystemManagerID(id);

            if(current_Users.ContainsKey(id)==false)
                throw new Exception("User should enter the system before logging in");
           
            foreach (Member member in members.Values)
            {
                if (member.Email.Equals(email))
                {
                    if (!_ph.Rehash(password, member.Password))
                    {
                        throw new Exception(password + " is wrong password for email");
                    }
                    else
                    {
                        //correct email & password:
                        if (member.LoggedIn == true)
                            throw new Exception("member is already logged in!");
                        member.LoggedIn = true;
                        User user;
                        current_Users.TryRemove(id, out user);
                        member.ShoppingCart.AddUserShoppingCart(user.ShoppingCart);

                        Logger.Instance.Info(member, "logged in");

                        return member.UserId;
                    }
                }
            }
            //email not found
            throw new Exception(email +" doesn't exist");
        }

        public Guid Logout(Guid id)
        {
            if (!members.ContainsKey(id))
                throw new Exception("member with id dosen't exist");

            Member member = members[id];
            if(member.LoggedIn==false)
                throw new Exception("member is already logged out!");
            member.LoggedIn = false;
            Logger.Instance.Info(member.UserId, nameof(UserFacade)+": "+nameof(Logout)+" logged out as member");

            return Enter(); //member logs out and a regular user enters the system instead  
        }

        public void AddItemToCart(Guid userID, Guid storeID, Guid itemID, int itemAmount)
        {
            IsTsInitialized();
            if (members.ContainsKey(userID)){
                if (!isLoggedIn(userID))
                        throw new Exception("the user is not logged in");
                members[userID].AddItemToCart(storeID, itemID, itemAmount);
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
            if (members.ContainsKey(userID))
                members[userID].ShoppingCart = new ShoppingCart();
            else
                current_Users[userID].ShoppingCart = new ShoppingCart();;
        }
        
        public void OpenNewStore(Guid userID, Guid storeID)
        {
            IsTsInitialized();
            isLoggedIn(userID);
            PromotedMember founder = members[userID].openNewStore(storeID);
            if (founder != null)
                members[userID] = founder;
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

        public void AppointStoreOwner(Guid userID, Guid storeID, string email)
        {
            IsTsInitialized();
            isLoggedIn(userID);
            Guid newOwnerID = Guid.Empty;
            foreach (Member member in members.Values)
                if (member.Email == email)
                {
                    newOwnerID = member.UserId;
                }

            if (newOwnerID.Equals(Guid.Empty))
                throw new Exception($"There isn't a member with {email}");
            lock (members[newOwnerID])
            {
                PromotedMember owner = members[userID].AppointStoreOwner(storeID, members[newOwnerID]);
                members[newOwnerID] = owner;
            }
            Logger.Instance.Info(userID, nameof(UserFacade)+": "+nameof(AppointStoreOwner)+" appoints " +newOwnerID +" to new store owner");
        }

        public void AppointStoreManager(Guid userID, Guid storeID, string email)
        {
            IsTsInitialized();
            isLoggedIn(userID);
            Guid newManagerID = Guid.Empty;

            foreach (Member member in members.Values)
                if (member.Email == email)
                {
                    newManagerID = member.UserId;
                }

            if (newManagerID.Equals(Guid.Empty))
                throw new Exception($"There isn't a member with {email}");

            lock (members[newManagerID])
            {
                PromotedMember manager = members[userID].AppointStoreManager(storeID, members[newManagerID]);
                members[newManagerID] = manager;
            }


            Logger.Instance.Info(userID, nameof(UserFacade)+": "+nameof(AppointStoreManager)+" appoints " +newManagerID +" to new store manager");
        }

        public void AddStoreManagerPermissions(Guid userID, Guid storeID, string email, string permission)
        {
            IsTsInitialized();
            isLoggedIn(userID);
            Member manager = null;
            foreach (Member member in members.Values)
                if (member.Email == email)
                {
                    manager = member;
                    break;
                }
            if (manager == null)
                throw new Exception($"There isn't a member with {email}");

            members[userID].AddStoreManagerPermissions(storeID, manager, permission);
            Logger.Instance.Info(userID, nameof(UserFacade)+": "+nameof(AddStoreManagerPermissions)+"System added "+userID+" manager permissions in store "+storeID+": "+permission);
        }
        public void RemoveStoreManagerPermissions(Guid userID, Guid storeID, string email, string permission)
        {
            IsTsInitialized();
            isLoggedIn(userID);
            Member manager = null;
            foreach (Member member in members.Values)
                if (member.Email == email)
                    manager = member;

            if (manager == null)
                throw new Exception($"There isn't a member with {email}");
            members[userID].RemoveStoreManagerPermissions(storeID, manager,permission);
            Logger.Instance.Info(userID, nameof(UserFacade)+": "+nameof(RemoveStoreManagerPermissions)+"System removed "+userID+" manager permissions in store "+storeID+": "+permission);
        }
        public List<PromotedMember> GetEmployeeInfoInStore(Guid userID, Guid storeID)
        {
            IsTsInitialized();
            isLoggedIn(userID);
            List<PromotedMember> employees = members[userID].GetEmployeeInfoInStore(storeID);
            return employees;
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
            members[userID].Password = _ph.Hash(newPassword);
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

        public ConcurrentDictionary<Guid, User> GetCurrent_Users()
        {
            return current_Users;
        }
        public ConcurrentDictionary<Guid, Member> GetMembers()
        {
            return members;
        }
        public ShoppingCart ShowShoppingCart(Guid userID)
        {
            IsTsInitialized();
            isLoggedIn(userID);
            Logger.Instance.Info(userID, nameof(UserFacade)+": "+nameof(ShowShoppingCart)+" requested to display his shopping cart");
            if (current_Users.ContainsKey(userID))
                return current_Users[userID].ShoppingCart;
            return members[userID].ShoppingCart;
        }

        public void SetSecurityQA(Guid userID, string q, string a)
        {
            IsTsInitialized();
            isLoggedIn(userID);
            if (!members.ContainsKey(userID))
                throw new Exception("member with id dosen't exist");
            members[userID].SetSecurityQA(q,_ph.Hash(a));
            Logger.Instance.Info(userID, nameof(UserFacade)+": "+nameof(SetSecurityQA)+"Security Q&A set");
        }

        public ShoppingCart GetShoppingCartById(Guid userID)
        {
            IsTsInitialized();
            if (current_Users.ContainsKey(userID))
            {
                return current_Users[userID].ShoppingCart;
            }
            else if (members.ContainsKey(userID))
            {
                return members[userID].ShoppingCart;
            }
            throw new Exception("no cart for this user id");
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

                bool isCompletedSuccessfully = task.Wait(TimeSpan.FromMilliseconds(MaxExternalServiceWaitTime));

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

        public bool PlaceSupply(string orderDetails, string userDetails)
        {
            try
            {
                Logger.Instance.Info(nameof(supplierService) + ": user: " + userDetails + " request to preform a supply for order: " + orderDetails);

                var task = Task.Run(() =>
                {
                    return supplierService.ShipOrder(orderDetails, userDetails);
                });

                bool isCompletedSuccessfully = task.Wait(TimeSpan.FromMilliseconds(MaxExternalServiceWaitTime));

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

        private void IsTSSystemManagerID(Guid userID)
        {
            if (members.ContainsKey(userID))
                if (members[userID].hasPermissions(Guid.Empty, new List<string> { "system manager permissions" }) == false)                 
                   throw new Exception("System is not initialized");
                else
                    throw new Exception("System is not initialized");
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

        public ShoppingCart GetUserShoppingCart(Guid userID)
        {
            
            if (current_Users.ContainsKey(userID))
                return current_Users[userID].ShoppingCart;
            if (isLoggedIn(userID))
                return members[userID].ShoppingCart;
            
            
            throw new Exception("User with id " + userID + " does not exist");
        }
    }
}