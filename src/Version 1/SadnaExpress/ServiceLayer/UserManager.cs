using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.DomainLayer.User;
using SadnaExpress.ServiceLayer.ServiceObjects;
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
                Logger.Instance.Error(userID,nameof(UserManager)+": "+nameof(Login)+": "+ex.Message);
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

        public Response RemoveStoreManagerPermissions(Guid userID, Guid storeID, string userEmail, string permission)
        {
            try
            {
                userFacade.RemoveStoreManagerPermissions(userID, storeID, userEmail, permission);
                return new Response();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(userID,nameof(UserManager)+": "+nameof(RemoveStoreManagerPermissions)+": "+ex.Message);
                return new Response(ex.Message);
            }
        }

        public ResponseT<List<PromotedMember>> GetEmployeeInfoInStore(Guid userID, Guid storeID)
        {
            try
            {
                List<PromotedMember> employees = userFacade.GetEmployeeInfoInStore(userID, storeID);
                return new ResponseT<List<PromotedMember>>(employees);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(userID,nameof(UserManager)+": "+nameof(GetEmployeeInfoInStore)+": "+ex.Message);
                return new ResponseT<List<PromotedMember>>(ex.Message);
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
        public ConcurrentDictionary<Guid , Member> GetMembers()
        {
            try
            {
                return userFacade.GetMembers();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message + " in " + nameof(GetMembers));
                return new ConcurrentDictionary<Guid, Member>();
            }
        }
        public ResponseT<ShoppingCart> ShowShoppingCart(Guid userID)
        {
            try
            {
                ShoppingCart sc = userFacade.ShowShoppingCart(userID);
                return new ResponseT<ShoppingCart>(sc);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(userID,nameof(UserManager)+": "+nameof(ShowShoppingCart)+": "+ex.Message);
                return new ResponseT<ShoppingCart>(ex.Message);
            }
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


        public ResponseT<ShoppingCart> GetShoppingCartById(Guid userID)
        {
            try
            {
                ShoppingCart Cart = userFacade.GetShoppingCartById(userID);
                return new ResponseT<ShoppingCart>(Cart);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(userID,nameof(UserManager)+": "+nameof(GetShoppingCartById)+": "+ex.Message);
                return new ResponseT<ShoppingCart>(ex.Message);
            }
        }

        public bool isLoggedIn(Guid userID)
        {
            return userFacade.isLoggedIn(userID);
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
                return new ResponseT<Member>(member);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(userID , nameof(UserManager)+": "+nameof(GetMember)+": "+ex.Message);
                return new ResponseT<Member>(ex.Message);
            }
        }

        public ResponseT<ShoppingCart> GetUserShoppingCart(Guid userID)
        {
            try
            {
                ShoppingCart shoppingCart = userFacade.GetUserShoppingCart(userID);
                return new ResponseT<ShoppingCart>(shoppingCart);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(userID , nameof(UserManager)+": "+nameof(GetUserShoppingCart)+": "+ex.Message);
                return new ResponseT<ShoppingCart>(ex.Message);
            }
        }
    }
}