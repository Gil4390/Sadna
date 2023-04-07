using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.DomainLayer.User;
using SadnaExpress.ServiceLayer.ServiceObjects;

namespace SadnaExpress.ServiceLayer
{
    public class UserManager: IUserManager
    {
        private IUserFacade userFacade;
        
        public UserManager()
        {
            userFacade = new UserFacade();
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

        public Response AddItemToCart(int id, int itemID, int itemAmount)
        {   
            throw new System.NotImplementedException();
        }

        public Response RemoveItemFromCart(int id, int itemID, int itemAmount)
        {
            throw new System.NotImplementedException();
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
                userFacade.AppointStoreOwner(id, storeID, userEmail);
                return new Response();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return new Response(ex.Message);
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

        public Response AddStoreManagerPermissions(int id, Guid storeID, int newUserID, string permission)
        {
            throw new System.NotImplementedException();
        }

        public Response RemoveStoreManagerPermissions(int id, Guid storeID, int newUserID, string permission)
        {
            throw new System.NotImplementedException();
        }

        public ResponseT<List<S_Member>> GetEmployeeInfoInStore(int id, Guid storeID)
        {
            throw new System.NotImplementedException();
        }

        public void CleanUp()
        {
            userFacade.CleanUp();
        }

        public bool InitializeTradingSystem(int id)
        {
            return userFacade.InitializeTradingSystem(id);
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
    }
}