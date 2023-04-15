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
                Logger.Instance.Error(ex.Message);
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
                Logger.Instance.Error(ex.Message);
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
                Logger.Instance.Error(ex.Message);
                return new Response(ex.Message);
            }
        }

        public ResponseT<Guid> Login(Guid userID, string email, string password)
        {
            try
            {
                Guid newID = userFacade.Login(userID, email, password);
                return new ResponseT<Guid>(userID);

            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return new ResponseT<Guid>(ex.Message);
            }
        }

        public Response AddItemToCart(Guid userID, Guid storeID, int itemID, int itemAmount)
        {
            try
            {
               userFacade.AddItemToCart(userID, storeID, itemID, itemAmount);
                return new Response();

            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return new Response(ex.Message);
            }
        }

        public Response RemoveItemFromCart(Guid userID, Guid storeID, int itemID)
        {
            try
            {
                userFacade.RemoveItemFromCart(userID, storeID, itemID);
                return new Response();

            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return new Response(ex.Message);
            }
        }

        public Response EditItemFromCart(Guid userID, int itemID, int itemAmount)
        {
            throw new System.NotImplementedException();
        }

        public ResponseT<Dictionary<string, List<string>>> getDetailsOnCart()
        {
            throw new System.NotImplementedException();
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
                Logger.Instance.Error(ex.Message);
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
                Logger.Instance.Error(ex.Message);
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
                Logger.Instance.Error(ex.Message);
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
                Logger.Instance.Error(ex.Message);
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
                Logger.Instance.Error(ex.Message);
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
                Logger.Instance.Error(ex.Message);
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
                Logger.Instance.Error(ex.Message);
                return new ResponseT<bool>(ex.Message);
            }
        }
        public ConcurrentDictionary<Guid , User> GetCurrent_Users()
        {
            return userFacade.GetCurrent_Users();
        }
        public ConcurrentDictionary<Guid , Member> GetMembers()
        {
            return userFacade.GetMembers();
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
                Logger.Instance.Error(ex.Message);
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
                Logger.Instance.Error(ex.Message);
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
                Logger.Instance.Error(ex.Message);
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
                Logger.Instance.Error(ex.Message);
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
                Logger.Instance.Error(ex.Message);
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
                Logger.Instance.Error(ex.Message);
                return new ResponseT<Guid>(ex.Message);
            }
        }

        public void SetPaymentService(IPaymentService paymentService)
        {
            userFacade.SetPaymentService(paymentService);
        }

        public void SetSupplierService(ISupplierService supplierService)
        {
            userFacade.SetSupplierService(supplierService);
        }

        public void SetIsSystemInitialize(bool isInitialize)
        {
            userFacade.SetIsSystemInitialize(isInitialize);
        }
    }
}