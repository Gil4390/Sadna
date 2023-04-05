using System;
using System.Collections.Generic;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.DomainLayer.User;
using SadnaExpress.ServiceLayer.ServiceObjects;
using SadnaExpress.Services;

namespace SadnaExpress.ServiceLayer
{
    public class Tradingsystem
    {
        private IStoreFacade storeFacade;
        private IUserFacade userFacade;
        
        // added by radwan
        private ISupplierService supplierService;
        private IPaymentService paymentService;

        public Tradingsystem(ISupplierService supplierService, IPaymentService paymentService)
        {
            storeFacade = new StoreFacade();
            userFacade = new UserFacade();

            this.paymentService = paymentService;
            this.supplierService = supplierService;

            //userFacade.Register(0, adminEmail, adminFName, adminLName, adminPassword);
            //userFacade.Login(0, adminEmail, adminPassword);

        }
        public int enter()
        {
            return userFacade.Enter();
        }
        public void exit(int id)
        {
            try
            {
                userFacade.Exit(id);

            }
            catch (Exception e)
            {

            }

        }
        public void register(int id, string email, string firstName, string lastName, string password)
        {
            try
            {
                userFacade.Register(id, email, firstName, lastName, password);

            }
            catch (Exception e)
            {

            }
        }
        public int login(int id, string email, string password)
        {
            try
            {
                return userFacade.Login(id, email, password);

            }
            catch (Exception e)
            {
                return -1; //need to check this
            }

        }
        public int logout(int id)
        {
            try
            {
                return userFacade.Logout(id);

            }
            catch (Exception e)
            {
                return -1; //need to check this
            }

        }

        internal List<S_Store> getAllStoreInfo(int id)
        {
            //get list of buissnes stores and convert them to service stores
            throw new NotImplementedException();
        }

        internal void addItemToCart(int id, int itemID, int itemAmount)
        {
            throw new NotImplementedException();
        }

        internal void purchaseCart(int id, string paymentDetails)
        {
            throw new NotImplementedException();
        }

        internal void createStore(int id, string storeName)
        {
            try
            {
                //todo
            }
            catch (Exception e)
            {

            }
        }

        internal void writeReview(int id, int itemID, string review)
        {
            throw new NotImplementedException();
        }

        internal void rateItem(int id, int itemID, int score)
        {
            throw new NotImplementedException();
        }

        internal void writeMessageToStore(int id, int storeID, string message)
        {
            throw new NotImplementedException();
        }

        internal void complainToAdmin(int id, string message)
        {
            throw new NotImplementedException();
        }

        internal void getPurchasesInfo(int id)
        {
            throw new NotImplementedException();
        }

        internal void addItemToStore(int id, int storeID, string itemName, string itemCategory, float itemPrice)
        {
            throw new NotImplementedException();
        }

        internal void removeItemFromStore(int id, int storeID, int itemID)
        {
            throw new NotImplementedException();
        }

        internal void appointStoreOwner(int id, int storeID, int newUserID)
        {
            throw new NotImplementedException();
        }

        internal void appointStoreManager(int id, int storeID, int newUserID)
        {
            throw new NotImplementedException();
        }

        internal void removeStoreOwner(int id, int storeID, int userID)
        {
            throw new NotImplementedException();
        }

        internal void removetStoreManager(int id, int storeID, int userID)
        {
            throw new NotImplementedException();
        }

        internal void closeStore(int id, int storeID)
        {
            throw new NotImplementedException();
        }

        internal void reopenStore(int id, int storeID)
        {
            throw new NotImplementedException();
        }

        internal List<S_Member> getEmployeeInfoInStore(int id, int storeID)
        {
            throw new NotImplementedException();
        }

        internal void getPurchasesInfo(int id, int storeID)
        {
            throw new NotImplementedException();
        }

        internal void deleteStore(int id, int storeID)
        {
            throw new NotImplementedException();
        }

        internal void deleteMember(int id, int userID)
        {
            throw new NotImplementedException();
        }

        public bool checkSupplierConnection()
        {
            bool result = this.supplierService.Connect();
            if(!result)
            {
                throw new SadnaException("Supplier Service Connection Failed", "TradingSystem", "checkSupplierConnection");
            }
            return result;
        }

        public bool checkPaymentConnection()
        {
            bool result = this.paymentService.Connect();
            if (!result)
            {
                throw new SadnaException("Payment Service Connection Failed", "TradingSystem", "checkSupplierConnection");
            }
            return result;
        }

        public void CleanUp() // for the tests
        {
            storeFacade = null;
            //userFacade = null;
            userFacade.CleanUp();
            supplierService = null;
            paymentService = null;
        }
    }
}