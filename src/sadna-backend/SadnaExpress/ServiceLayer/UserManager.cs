using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using SadnaExpress.DomainLayer;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.DomainLayer.User;
using SadnaExpress.ServiceLayer.ServiceObjects;
using SadnaExpress.ServiceLayer.SModels;
using SadnaExpress.Services;

namespace SadnaExpress.ServiceLayer
{
    public class UserManager: IUserManager
    {
        private IUserFacade userFacade;
        

        public IUserFacade GetUserFacade()
        {
            return userFacade;
        }

        public UserManager(IUserFacade uf)
        {
            userFacade = uf;
        }

        public UserManager()
        {
            this.userFacade = new UserFacade();
        }
        public ResponseT<Guid> Enter()
        {
            try
            {
                Guid userID = userFacade.Enter();
                return new ResponseT<Guid>(userID);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(nameof(UserManager)+": "+nameof(Enter)+": "+ex.Message);
                return new ResponseT<Guid>(ex.Message);
            }
        }

        public Response Exit(Guid userID)
        {
            try
            {
                userFacade.Exit(userID);
                return new Response();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(userID,nameof(UserManager)+": "+nameof(Exit)+": "+ex.Message);
                return new Response(ex.Message);
            }
        }

        public Response Register(Guid userID, string email, string firstName, string lastName, string password)
        {
            try
            {
                userFacade.Register(userID, email, firstName, lastName, password);
                return new Response();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(userID,nameof(UserManager)+": "+nameof(Register)+": "+ex.Message);
                return new Response(ex.Message);
            }
        }

        public ResponseT<Guid> Login(Guid userID, string email, string password)
        {
            try
            {
                Guid newID = userFacade.Login(userID, email, password);
                return new ResponseT<Guid>(newID);

            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"{userID} {email} {nameof(UserManager) }"+nameof(Login)+": "+ex.Message);
                return new ResponseT<Guid>(ex.Message);
            }
        }
        public ResponseT<Guid> Logout(Guid userID)
        {
            try
            {
                Guid newID = userFacade.Logout(userID);
                return new ResponseT<Guid>(newID);

            }
            catch (Exception ex)
            {
                Logger.Instance.Error(userID,nameof(UserManager)+": "+nameof(Logout)+": "+ex.Message);
                return new ResponseT<Guid>(ex.Message);
            }
        }

        public Response RemovePermission(Guid userID, Guid storeID, string userEmail, string permission)
        {
            try
            {
                userFacade.RemovePermission(userID, storeID, userEmail, permission);
                // new function here
                userFacade.UpdateCurrentMemberFromDb(userID);
                return new Response();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(userID,nameof(UserManager)+": "+nameof(AppointStoreOwner)+": "+ex.Message);
                return new Response(ex.Message);
            }
        }
        public Response AppointStoreOwner(Guid userID, Guid storeID, string userEmail)
        {
            try
            {
                userFacade.AppointStoreOwner(userID, storeID, userEmail);
                return new Response();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(userID,nameof(UserManager)+": "+nameof(AppointStoreOwner)+": "+ex.Message);
                return new Response(ex.Message);
            }
        }
        public Response AppointStoreManager(Guid userID, Guid storeID, string userEmail)
        {
            try
            {
                userFacade.AppointStoreManager(userID, storeID, userEmail);
                return new Response();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(userID,nameof(UserManager)+": "+nameof(AppointStoreManager)+": "+ex.Message);
                return new Response(ex.Message);
            }
        }

        public Response AddStoreManagerPermissions(Guid userID, Guid storeID,  string userEmail, string permission)
        {
            try
            {
                userFacade.AddStoreManagerPermissions(userID, storeID, userEmail, permission);
                return new Response();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(userID,nameof(UserManager)+": "+nameof(AddStoreManagerPermissions)+": "+ex.Message);
                return new Response(ex.Message);
            }
        }

        public ResponseT<List<SMemberForStore>> GetEmployeeInfoInStore(Guid userID, Guid storeID)
        {
            try
            {
                List<PromotedMember> employees = userFacade.GetEmployeeInfoInStore(userID, storeID);
                List<SMemberForStore> sMembers = new List<SMemberForStore>();

                foreach (PromotedMember member in employees)
                {
                    SMemberForStore sMember = new SMemberForStore(member, storeID);
                    sMembers.Add(sMember);
                }
                return new ResponseT<List<SMemberForStore>>(sMembers);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(userID,nameof(UserManager)+": "+nameof(GetEmployeeInfoInStore)+": "+ex.Message);
                return new ResponseT<List<SMemberForStore>>(ex.Message);
            }
        }

        public Response RemoveUserMembership(Guid userID, string email)
        {
            try
            {
                userFacade.RemoveUserMembership(userID, email);
                Logger.Instance.Info($"User id: {userID} requested to removing user {email} membership");
                return new Response();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(userID,nameof(UserManager)+": "+nameof(RemoveUserMembership)+": "+ex.Message);
                return new Response(ex.Message);
            }
        }

        public void CleanUp()
        {
            userFacade.CleanUp();
        }

        public ResponseT<bool> InitializeTradingSystem(Guid userID)
        {
            try
            {
                Logger.Instance.Info("User id: " + userID + " requested to initialize trading system");

                return new ResponseT<bool>(userFacade.InitializeTradingSystem(userID));
                
               // return new ResponseT<bool>(paymentService.Connect() && supplierService.Connect());
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message + " in " + nameof(InitializeTradingSystem));
                return new ResponseT<bool>(ex.Message);
            }
        }
        public ConcurrentDictionary<Guid , User> GetCurrent_Users()
        {
            try
            {
                return userFacade.GetCurrent_Users();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message + " in " + nameof(GetCurrent_Users));
                return new ConcurrentDictionary<Guid, User>();
            }
        }
        public ConcurrentDictionary<Guid, Member> GetMembers(Guid userID)
        {
            return userFacade.GetMembers(userID);
        }

        public ResponseT<Guid> SetSecurityQA(Guid userID, string q, string a)
        {
            try
            {
                userFacade.SetSecurityQA(userID,q,a);
                return new ResponseT<Guid>(userID);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(userID,nameof(UserManager)+": "+nameof(SetSecurityQA)+": "+ex.Message);
                return new ResponseT<Guid>(ex.Message);
            }
        }

        public ResponseT<Guid> UpdateFirst(Guid userID, string newFirst)
        {
            try
            {
                userFacade.UpdateFirst(userID, newFirst);
                return new ResponseT<Guid>(userID);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(userID,nameof(UserManager)+": "+nameof(UpdateFirst)+": "+ex.Message);
                return new ResponseT<Guid>(ex.Message);
            }
        }

        public ResponseT<Guid> UpdateLast(Guid userID, string newLast)
        {
            try
            {
                userFacade.UpdateLast(userID, newLast);
                return new ResponseT<Guid>(userID);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(userID,nameof(UserManager)+": "+nameof(UpdateLast)+": "+ex.Message);
                return new ResponseT<Guid>(ex.Message);
            }
        }

        public ResponseT<Guid> UpdatePassword(Guid userID, string newPassword)
        {
            try
            {
                userFacade.UpdatePassword(userID, newPassword);
                return new ResponseT<Guid>(userID);
                
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(userID,nameof(UserManager)+": "+nameof(UpdatePassword)+": "+ex.Message);
                return new ResponseT<Guid>(ex.Message);
            }
        }

        public void SetPaymentService(IPaymentService paymentService)
        {
            try
            {
                userFacade.SetPaymentService(paymentService);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(nameof(UserManager)+": "+nameof(SetPaymentService)+": "+ex.Message);
            }
        }

        public void SetSupplierService(ISupplierService supplierService)
        {
            try
            {
                userFacade.SetSupplierService(supplierService);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(nameof(UserManager)+": "+nameof(SetSupplierService)+": "+ex.Message);
            }
        }

        public void SetIsSystemInitialize(bool isInitialize)
        {
            try
            {
                userFacade.SetIsSystemInitialize(isInitialize);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(nameof(UserManager)+": "+nameof(SetIsSystemInitialize)+": "+ex.Message);
            }
        }

        public ResponseT<User> GetUser(Guid userID)
        {
            try
            {
                User user = userFacade.GetUser(userID);
                Logger.Instance.Info(userID , nameof(UserManager)+": "+nameof(GetUser));
                return new ResponseT<User>(user);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(userID , nameof(UserManager)+": "+nameof(GetUser)+": "+ex.Message);
                return new ResponseT<User>(ex.Message);
            }
        }

        public ResponseT<Member> GetMember(Guid userID)
        {
            try
            {
                Member member = userFacade.GetMember(userID);
                Logger.Instance.Info(userID , nameof(UserManager)+": "+nameof(GetMember));
                return new ResponseT<Member>(member);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(userID , nameof(UserManager)+": "+nameof(GetMember)+": "+ex.Message);
                return new ResponseT<Member>(ex.Message);
            }
        }
        
        public ResponseT<List<Notification>> GetNotifications(Guid userId)
        {
            try
            {
                List<Notification> notifications =  userFacade.GetNotifications(userId);
                return new ResponseT<List<Notification>>(notifications);

            }
            catch (Exception ex)
            {
                Logger.Instance.Error(userId , nameof(UserManager)+": "+nameof(GetNotifications)+": "+ex.Message);
                return new ResponseT<List<Notification>>(ex.Message);
            }
        }

        public ResponseT<SBid[]> GetBidsInStore(Guid userID, Guid storeID)
        {
            try
            {
                List<Bid> bids=  userFacade.GetBidsInStore(userID, storeID);
                List<SBid> sbids = new List<SBid>();
                foreach (Bid bid in bids)
                {
                    sbids.Add(new SBid(bid));
                }
                return new ResponseT<SBid[]>(sbids.ToArray());
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(userID , nameof(UserManager)+": "+nameof(GetNotifications)+": "+ex.Message);
                return new ResponseT<SBid[]>(ex.Message);
            }
        }

        public Dictionary<Guid, KeyValuePair<double, bool>> GetBidsOfUser(Guid userID)
        {
            return userFacade.GetBidsOfUser(userID);
        }

        public ResponseT<List<Member>> getAllStoreOwners(ConcurrentDictionary<Guid, Store> stores)
        {
            try
            {
                List<Member> storesOwners =  userFacade.getAllStoreOwners(stores);
                return new ResponseT<List<Member>>(storesOwners);

            }
            catch (Exception ex)
            {
                Logger.Instance.Error(  nameof(UserManager)+": "+nameof(getAllStoreOwners)+": "+ex.Message);
                return new ResponseT<List<Member>>(ex.Message);
            }
            
        }

        public ResponseT<List<Member>> GetStoreOwnerOfStores(List<Guid> stores)
        {
            try
            {
                List<Member> storesOwners = userFacade.GetStoreOwnerOfStores(stores);
                return new ResponseT<List<Member>>(storesOwners);

            }
            catch (Exception ex)
            {
                Logger.Instance.Error(nameof(UserManager) + ": " + nameof(GetStoreOwnerOfStores) + ": " + ex.Message);
                return new ResponseT<List<Member>>(ex.Message);
            }
        }

        public bool IsSystemInitialize()
        {
            return userFacade.IsSystemInitialize();
        }

        public ResponseT<bool> isAdmin(Guid userID)
        {
            try
            {
                var response = userFacade.IsUserAdmin(userID);
                return new ResponseT<bool>(response);

            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"{userID}" + nameof(isAdmin) + ": " + ex.Message);
                return new ResponseT<bool>(ex.Message);
            }
        }

        public ResponseT<Dictionary<Guid, SPermission>> GetMemberPermissions(Guid userID)
        {
            try
            {
                ConcurrentDictionary<Guid, List<String>> permissions = userFacade.GetMemberPermissions(userID);
                Dictionary<Guid, SPermission> Spermissions = new Dictionary<Guid, SPermission>();

                foreach (Guid storeID in permissions.Keys)
                {
                    Spermissions.Add(storeID, new SPermission(permissions[storeID]));
                }
                return new ResponseT<Dictionary<Guid, SPermission>>(Spermissions);

            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"{userID}" + nameof(GetMemberPermissions) + ": " + ex.Message);
                return new ResponseT<Dictionary<Guid, SPermission>>(ex.Message);
            }
        }

        public Response MarkNotificationAsRead(Guid userID, Guid notificationID)
        {
            try
            {
                userFacade.MarkNotificationAsRead(userID, notificationID);
                return new Response();

            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"{userID}" + nameof(MarkNotificationAsRead) + ": " + ex.Message);
                return new Response(ex.Message);
            }
            
        }
        
        public Response Handshake()
        {
            try
            {
                return new Response(userFacade.Handshake());
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(nameof(Handshake) + ": " + ex.Message);
                return new Response(ex.Message);
            }
            
        }

        public void CreateSystemManager(Guid userID)
        {
            userFacade.CreateSystemManager(userID);
        }
    }
}