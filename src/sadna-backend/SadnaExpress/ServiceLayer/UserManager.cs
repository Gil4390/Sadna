using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.AspNet.SignalR.Hosting;
using SadnaExpress.DataLayer;
using SadnaExpress.DomainLayer;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.DomainLayer.User;
using SadnaExpress.ServiceLayer;
using SadnaExpress.ServiceLayer.ServiceObjects;
using SadnaExpress.ServiceLayer.SModels;
using SadnaExpress.Services;

namespace SadnaExpress.ServiceLayer
{
    public class UserManager: IUserManager
    {
        private IUserFacade userFacade;

        #region Constructor
        public UserManager(IUserFacade uf)
        {
            userFacade = uf;
        }

        public UserManager()
        {
            this.userFacade = new UserFacade();
        }
        #endregion

        #region User operatrions
        public ResponseT<Guid> Enter()
        {
            try
            {
                DBHandler.Instance.CanConnectToDatabase();
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
                DBHandler.Instance.CanConnectToDatabase();
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
                DBHandler.Instance.CanConnectToDatabase();
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
                DBHandler.Instance.CanConnectToDatabase();
                Guid newID = userFacade.Logout(userID);
                return new ResponseT<Guid>(newID);

            }
            catch (Exception ex)
            {
                Logger.Instance.Error(userID,nameof(UserManager)+": "+nameof(Logout)+": "+ex.Message);
                return new ResponseT<Guid>(ex.Message);
            }
        }
        #endregion

        #region Permissions Store manage
        public Response RemovePermission(Guid userID, Guid storeID, string userEmail, string permission)
        {
            try
            {
                DBHandler.Instance.CanConnectToDatabase();
                userFacade.RemovePermission(userID, storeID, userEmail, permission);
                return new Response();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(userID,nameof(UserManager)+": "+nameof(AppointStoreOwner)+": "+ex.Message);
                return new Response(ex.Message);
            }
        }
        public ResponseT<List<ItemForOrder>> GetPurchasesOfUser(Guid userID)
        {
            try
            {
                List<ItemForOrder> list = new List<ItemForOrder>();
            userFacade.GetMember(userID);
            if (Orders.Instance.GetUserOrders().ContainsKey(userID))
            {
                foreach (Order order in Orders.Instance.GetUserOrders()[userID])
                {
                    list.AddRange(order.ListItems);
                }
            }
            return new ResponseT<List<ItemForOrder>>(list);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(userID, nameof(UserManager) + ": " + nameof(AppointStoreOwner) + ": " + ex.Message);
                return new ResponseT<List<ItemForOrder>>(ex.Message);
            }
        }
        public Response AppointStoreOwner(Guid userID, Guid storeID, string userEmail)
        {
            try
            {
                DBHandler.Instance.CanConnectToDatabase();
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
                DBHandler.Instance.CanConnectToDatabase();
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
                DBHandler.Instance.CanConnectToDatabase();
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
                List<Member> pendding = new List<Member>();

                foreach (PromotedMember member in employees)
                {
                    if (member.PermissionsOffers.ContainsKey(storeID))
                        foreach (Guid mem in member.PermissionsOffers[storeID])
                        { 
                            Member m = userFacade.GetMember(mem);
                            if (!pendding.Contains(m) && !(m.GetType() == typeof(PromotedMember) && ((PromotedMember)m).Permission.ContainsKey(storeID)))
                            {
                                SMemberForStore sPenndingMember = new SMemberForStore(m, storeID, userFacade.GetMember(userID));
                                sMembers.Add(sPenndingMember);
                                pendding.Add(m);
                            }
                        }
                    
                    SMemberForStore sMember = new SMemberForStore(member, storeID, userFacade.GetMember(userID));
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

        public Response ReactToJobOffer(Guid userID, Guid storeID, Guid newEmpID, bool offerResponse)
        {
            try
            {
                userFacade.ReactToJobOffer(userID, storeID, newEmpID, offerResponse);
                Logger.Instance.Info($"User id: {userID} requested to react to job offer user {newEmpID}");
                return new Response();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(userID, nameof(UserManager) + ": " + nameof(ReactToJobOffer) + ": " + ex.Message);
                return new Response(ex.Message);
            }
        }

        public ResponseT<SBid[]> GetBidsInStore(Guid userID, Guid storeID)
        {
            try
            {
                List<Bid> bids = userFacade.GetBidsInStore(userID, storeID);
                List<SBid> sbids = new List<SBid>();
                foreach (Bid bid in bids)
                {
                    sbids.Add(new SBid(bid));
                }
                return new ResponseT<SBid[]>(sbids.ToArray());
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(userID, nameof(UserManager) + ": " + nameof(GetNotifications) + ": " + ex.Message);
                return new ResponseT<SBid[]>(ex.Message);
            }
        }

        #endregion

        #region Admin operations
        public Response RemoveUserMembership(Guid userID, string email)
        {
            try
            {
                DBHandler.Instance.CanConnectToDatabase();
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

        public void CreateSystemManager(Guid userID)
        {
            userFacade.CreateSystemManager(userID);
        }

        public ConcurrentDictionary<Guid, Member> GetMembers(Guid userID)
        {
            return userFacade.GetMembers(userID);
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

        public ResponseT<List<int>> GetSystemUserActivity(Guid userID, DateTime fromDate, DateTime toDate)
        {
            try
            {
                DBHandler.Instance.CanConnectToDatabase();
                List<int> results = userFacade.GetSystemUserActivity(userID, fromDate, toDate);
                return new ResponseT<List<int>>(results);

            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"{userID}" + nameof(GetSystemUserActivity) + ": " + ex.Message);
                return new ResponseT<List<int>>(ex.Message);
            }
        }

        #endregion

        #region User data

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


        public Dictionary<Guid, KeyValuePair<double, bool>> GetBidsOfUser(Guid userID)
        {
            return userFacade.GetBidsOfUser(userID);
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
                DBHandler.Instance.CanConnectToDatabase();
                userFacade.MarkNotificationAsRead(userID, notificationID);
                return new Response();

            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"{userID}" + nameof(MarkNotificationAsRead) + ": " + ex.Message);
                return new Response(ex.Message);
            }
            
        }

        #endregion

        #region Getters & Setters

        public bool IsSystemInitialize()
        {
            return userFacade.IsSystemInitialize();
        }

        public void SetPaymentService(IPaymentService paymentService)
        {
            try
            {
                userFacade.SetPaymentService(paymentService);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(nameof(UserManager) + ": " + nameof(SetPaymentService) + ": " + ex.Message);
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
                Logger.Instance.Error(nameof(UserManager) + ": " + nameof(SetSupplierService) + ": " + ex.Message);
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
                Logger.Instance.Error(nameof(UserManager) + ": " + nameof(SetIsSystemInitialize) + ": " + ex.Message);
            }
        }

        public ResponseT<User> GetUser(Guid userID)
        {
            try
            {
                User user = userFacade.GetUser(userID);
                Logger.Instance.Info(userID, nameof(UserManager) + ": " + nameof(GetUser));
                return new ResponseT<User>(user);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(userID, nameof(UserManager) + ": " + nameof(GetUser) + ": " + ex.Message);
                return new ResponseT<User>(ex.Message);
            }
        }

        public ResponseT<Member> GetMember(Guid userID)
        {
            try
            {
                DBHandler.Instance.CanConnectToDatabase();
                Member member = userFacade.GetMember(userID);
                Logger.Instance.Info(userID, nameof(UserManager) + ": " + nameof(GetMember));
                return new ResponseT<Member>(member);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(userID, nameof(UserManager) + ": " + nameof(GetMember) + ": " + ex.Message);
                return new ResponseT<Member>(ex.Message);
            }
        }


        public ResponseT<Member> GetMember(String email)
        {
            try
            {
                Member member = userFacade.GetMember(email);
                Logger.Instance.Info(member, nameof(UserManager) + ": " + nameof(GetMember));
                return new ResponseT<Member>(member);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(nameof(UserManager) + ": " + nameof(GetMember) + ": " + ex.Message);
                return new ResponseT<Member>(ex.Message);
            }
        }

        #endregion

        public void CleanUp()
        {
            userFacade.CleanUp();
        }
    }
}