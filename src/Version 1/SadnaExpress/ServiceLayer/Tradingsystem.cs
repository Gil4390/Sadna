using System;
using System.Collections.Generic;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.DomainLayer.User;
using SadnaExpress.ServiceLayer.ServiceObjects;

namespace SadnaExpress.ServiceLayer
{
    public class TradingSystem
    {
        private IStoreFacade storeFacade;
        private IUserFacade userFacade;
        
        public TradingSystem()
        {
            storeFacade = new StoreFacade();
            userFacade = new UserFacade();
        }
        internal ResponseT<int> enter()
        {
            try
            {
                int id = userFacade.Enter();
                return new ResponseT<int>(id);
            }
            catch (Exception ex)
            {
                return new ResponseT<int>(ex.Message);
            }
        }
        internal Response exit(int id)
        {
            try
            {
                userFacade.Exit(id);
                return new Response();
            }
            catch (Exception ex)
            {
                return new Response(ex.Message);
            }

        }
        internal Response register(int id, string email, string firstName, string lastName, string password)
        {
            try
            {
                userFacade.Register(id, email, firstName, lastName, password);
                return new Response();
            }
            catch (Exception ex)
            {
                return new Response(ex.Message);
            }
        }
        internal ResponseT<int> login(int id, string email, string password)
        {
            try
            {
                int newID = userFacade.Login(id, email, password);
                return new ResponseT<int>(id);

            }
            catch (Exception ex)
            {
                return new ResponseT<int>(ex.Message);
            }

        }
        internal ResponseT<int> logout(int id)
        {
            try
            {
                int newID = userFacade.Logout(id);
                return new ResponseT<int>(id);

            }
            catch (Exception ex)
            {
                return new ResponseT<int>(ex.Message);
            }

        }

        internal ResponseT<List<S_Store>> getAllStoreInfo(int id)
        {
            //get list of buissnes stores and convert them to service stores
            throw new NotImplementedException();
        }

        internal Response addItemToCart(int id, int itemID, int itemAmount)
        {
            throw new NotImplementedException();
        }

        internal Response purchaseCart(int id, string paymentDetails)
        {
            throw new NotImplementedException();
        }

        internal Response createStore(int id, string storeName)
        {
            throw new NotImplementedException();
        }

        internal Response writeReview(int id, int itemID, string review)
        {
            throw new NotImplementedException();
        }

        internal Response rateItem(int id, int itemID, int score)
        {
            throw new NotImplementedException();
        }

        internal Response writeMessageToStore(int id, int storeID, string message)
        {
            throw new NotImplementedException();
        }

        internal Response complainToAdmin(int id, string message)
        {
            throw new NotImplementedException();
        }

        internal Response getPurchasesInfo(int id)
        {
            throw new NotImplementedException();
        }

        internal Response addItemToStore(int id, int storeID, string itemName, string itemCategory, float itemPrice)
        {
            throw new NotImplementedException();
        }

        internal Response removeItemFromStore(int id, int storeID, int itemID)
        {
            throw new NotImplementedException();
        }

        internal Response appointStoreOwner(int id, int storeID, int newUserID)
        {
            throw new NotImplementedException();
        }

        internal Response appointStoreManager(int id, int storeID, int newUserID)
        {
            throw new NotImplementedException();
        }

        internal Response removeStoreOwner(int id, int storeID, int userID)
        {
            throw new NotImplementedException();
        }

        internal Response removetStoreManager(int id, int storeID, int userID)
        {
            throw new NotImplementedException();
        }

        internal Response closeStore(int id, int storeID)
        {
            throw new NotImplementedException();
        }

        internal Response reopenStore(int id, int storeID)
        {
            throw new NotImplementedException();
        }

        internal ResponseT<List<S_Member>> getEmployeeInfoInStore(int id, int storeID)
        {
            throw new NotImplementedException();
        }

        internal Response getPurchasesInfo(int id, int storeID)
        {
            throw new NotImplementedException();
        }

        internal Response deleteStore(int id, int storeID)
        {
            throw new NotImplementedException();
        }

        internal Response deleteMember(int id, int userID)
        {
            throw new NotImplementedException();
        }
    }
}