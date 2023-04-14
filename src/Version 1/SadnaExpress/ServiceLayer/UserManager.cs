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
        public ResponseT<int> Enter()
        {
            try
            {
                int id = userFacade.Enter();
                return new ResponseT<int>(id);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return new ResponseT<int>(ex.Message);
            }
        }

        public Response Exit(int id)
        {
            try
            {
                userFacade.Exit(id);
                return new Response();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return new Response(ex.Message);
            }
        }

        public Response Register(int id, string email, string firstName, string lastName, string password)
        {
            try
            {
                userFacade.Register(id, email, firstName, lastName, password);
                return new Response();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return new Response(ex.Message);
            }
        }

        public ResponseT<int> Login(int id, string email, string password)
        {
            try
            {
                int newID = userFacade.Login(id, email, password);
                return new ResponseT<int>(id);

            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return new ResponseT<int>(ex.Message);
            }
        }

        public Response AddItemToCart(int id, Guid storeID, int itemID, int itemAmount)
        {
            try
            {
               userFacade.AddItemToCart(id, storeID, itemID, itemAmount);
                return new Response();

            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return new Response(ex.Message);
            }
        }

        public Response RemoveItemFromCart(int id, Guid storeID, int itemID)
        {
            try
            {
                userFacade.RemoveItemFromCart(id, storeID, itemID);
                return new Response();

            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return new Response(ex.Message);
            }
        }

        public Response EditItemFromCart(int id, int itemID, int itemAmount)
        {
            throw new System.NotImplementedException();
        }

        public ResponseT<Dictionary<string, List<string>>> getDetailsOnCart()
        {
            throw new System.NotImplementedException();
        }

        public ResponseT<int> Logout(int id)
        {
            try
            {
                int newID = userFacade.Logout(id);
                return new ResponseT<int>(id);

            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return new ResponseT<int>(ex.Message);
            }
        }

        public Response AppointStoreOwner(int id, Guid storeID, string userEmail)
        {
            try
            {
               // return userFacade.AppointStoreOwner(id, storeID, userEmail);
                return new Response();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return new Response(ex.Message);
            }
        }

        public ResponseT<List<S_Order>> GetStorePurchases(int id, Guid storeID, string email)
        {
            try
            {
                // after gil adds service objects - need to change this
                
               // List<S_Order> purchases = userFacade.GetStorePurchases(id, storeID,email);
               // return new ResponseT<List<S_Order>>(purchases);
               
                return new ResponseT<List<S_Order>>();

            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return new ResponseT<List<S_Order>>(ex.Message);
            }
        }
       
        public ResponseT<Dictionary<Guid,S_Order>> GetAllAStorePurchases(int id, Guid storeID, string email)
        {
            try
            {
                // after gil adds service objects - need to change this
                
                //Dictionary<Guid,Order> purchases = userFacade.GetStorePurchases(id, storeID,email);
                //return new ResponseT<Dictionary<Guid,S_Order>>(purchases);
               
                return new ResponseT<Dictionary<Guid,S_Order>>();

            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return new ResponseT<Dictionary<Guid,S_Order>>(ex.Message);
            }
        }
        
        
        public Response AppointStoreManager(int id, Guid storeID, string userEmail)
        {
            try
            {
                userFacade.AppointStoreManager(id, storeID, userEmail);
                return new Response();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return new Response(ex.Message);
            }
        }

        public Response AddStoreManagerPermissions(int id, Guid storeID,  string userEmail, string permission)
        {
            try
            {
                userFacade.AddStoreManagerPermissions(id, storeID, userEmail, permission);
                return new Response();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return new Response(ex.Message);
            }
        }

        public Response RemoveStoreManagerPermissions(int id, Guid storeID, string userEmail, string permission)
        {
            try
            {
                userFacade.RemoveStoreManagerPermissions(id, storeID, userEmail, permission);
                return new Response();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return new Response(ex.Message);
            }
        }

        public ResponseT<List<S_Member>> GetEmployeeInfoInStore(int id, Guid storeID)
        {
            try
            {
                List<PromotedMember> employees = userFacade.GetEmployeeInfoInStore(id, storeID);
                return new ResponseT<List<S_Member>>();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return new ResponseT<List<S_Member>>(ex.Message);
            }
        }

     
        public void CleanUp()
        {
            userFacade.CleanUp();
        }

        public ResponseT<bool> InitializeTradingSystem(int id)
        {
            try
            {
                Logger.Instance.Info("User id: " + id + " requested to initialize trading system");

                return new ResponseT<bool>(userFacade.InitializeTradingSystem(id));
                
               // return new ResponseT<bool>(paymentService.Connect() && supplierService.Connect());
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return new ResponseT<bool>(ex.Message);
            }
        }
        public ConcurrentDictionary<int , User> GetCurrent_Users()
        {
            return userFacade.GetCurrent_Users();
        }
        public ConcurrentDictionary<int , Member> GetMembers()
        {
            return userFacade.GetMembers();
        }
        public ResponseT<ShoppingCart> ShowShoppingCart(int id)
        {
            try
            {
                ShoppingCart sc = userFacade.ShowShoppingCart(id);
                return new ResponseT<ShoppingCart>(sc);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return new ResponseT<ShoppingCart>(ex.Message);
            }
        }

        public ResponseT<int> SetSecurityQA(int id, string q, string a)
        {
            try
            {
                userFacade.SetSecurityQA(id,q,a);
                return new ResponseT<int>(id);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return new ResponseT<int>(ex.Message);
            }
        }


        public ResponseT<ShoppingCart> GetShoppingCartById(int id)
        {
            try
            {
                ShoppingCart Cart = userFacade.GetShoppingCartById(id);
                return new ResponseT<ShoppingCart>(Cart);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return new ResponseT<ShoppingCart>(ex.Message);
            }
        }

      public bool  isLogin(int idx)
        {
            return userFacade.isLogin(idx);
        }


        public ResponseT<int> UpdateFirst(int id, string newFirst)
        {
            try
            {
                userFacade.UpdateFirst(id, newFirst);
                return new ResponseT<int>(id);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return new ResponseT<int>(ex.Message);
            }
        }

        public ResponseT<int> UpdateLast(int id, string newLast)
        {
            try
            {
                userFacade.UpdateLast(id, newLast);
                return new ResponseT<int>(id);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return new ResponseT<int>(ex.Message);
            }
        }

        public ResponseT<int> UpdatePassword(int id, string newPassword)
        {
            try
            {
                userFacade.UpdatePassword(id, newPassword);
                return new ResponseT<int>(id);
                
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return new ResponseT<int>(ex.Message);
            }
        }
    }
}