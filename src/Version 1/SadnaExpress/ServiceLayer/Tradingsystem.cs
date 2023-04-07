using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.DomainLayer.User;
using SadnaExpress.ServiceLayer.ServiceObjects;
using SadnaExpress.Services;

namespace SadnaExpress.ServiceLayer
{
    public class TradingSystem : ITradingSystem
    {
        private ISupplierService supplierService;
        private IPaymentService paymentService;
        private IStoreFacade storeFacade;
        private IUserFacade userFacade;
        private const int ExternalServiceWaitTimeInSeconds=10000; //10 seconds is 10,000 mili seconds
        public IPaymentService PaymentService { get => paymentService; set => paymentService = value; }
        public ISupplierService SupplierService { get => supplierService; set => supplierService = value; }

        public TradingSystem(ISupplierService supplierService=null, IPaymentService paymentService=null)
        {
            storeFacade = new StoreFacade();
            userFacade = new UserFacade();
            this.paymentService = paymentService;
            this.supplierService = supplierService;
        }

        public int GetMaximumWaitServiceTime()
        {
            return ExternalServiceWaitTimeInSeconds;
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

        public ResponseT<List<S_Store>> GetAllStoreInfo(int id)
        {
            //get list of buissnes stores and convert them to service stores
            throw new NotImplementedException();
        }

        public Response AddItemToCart(int id, int itemID, int itemAmount)
        {
            throw new NotImplementedException();
        }

        public Response PurchaseCart(int id, string paymentDetails)
        {
            throw new NotImplementedException();
        }

        public Response CreateStore(int id, string storeName)
        {
            throw new NotImplementedException();
        }

        public Response WriteReview(int id, int itemID, string review)
        {
            throw new NotImplementedException();
        }

        public Response RateItem(int id, int itemID, int score)
        {
            throw new NotImplementedException();
        }

        public Response WriteMessageToStore(int id, int storeID, string message)
        {
            throw new NotImplementedException();
        }

        public Response ComplainToAdmin(int id, string message)
        {
            throw new NotImplementedException();
        }

        public Response GetPurchasesInfo(int id)
        {
            throw new NotImplementedException();
        }

        public Response AddItemToStore(int id, int storeID, string itemName, string itemCategory, float itemPrice)
        {
            throw new NotImplementedException();
        }

        public Response RemoveItemFromStore(int id, int storeID, int itemID)
        {
            throw new NotImplementedException();
        }

        public Response AppointStoreOwner(int id, int storeID, int newUserID)
        {
            throw new NotImplementedException();
        }

        public Response AppointStoreManager(int id, int storeID, int newUserID)
        {
            throw new NotImplementedException();
        }

        public Response RemoveStoreOwner(int id, int storeID, int userID)
        {
            throw new NotImplementedException();
        }

        public Response RemovetStoreManager(int id, int storeID, int userID)
        {
            throw new NotImplementedException();
        }

        public Response CloseStore(int id, int storeID)
        {
            throw new NotImplementedException();
        }

        public Response ReopenStore(int id, int storeID)
        {
            throw new NotImplementedException();
        }

        public ResponseT<List<S_Member>> GetEmployeeInfoInStore(int id, int storeID)
        {
            throw new NotImplementedException();
        }

        public Response GetPurchasesInfo(int id, int storeID)
        {
            throw new NotImplementedException();
        }

        public Response DeleteStore(int id, int storeID)
        {
            throw new NotImplementedException();
        }

        public bool CheckSupplierConnection()
        {
            bool result = this.supplierService.Connect();
            if (!result)
            {
                Logger.Instance.Error("Supplier Service Connection Failed");
            }
            return result;
        }

        public Response DeleteMember(int id, int userID)
        {
            throw new NotImplementedException();
        }

        public bool CheckPaymentConnection()
        {
            bool result = this.paymentService.Connect();
            if (!result)
            {
                Logger.Instance.Error("Payment Service Connection Failed");
            }
            return result;
        }

        public void CleanUp() // for the tests
        {
            storeFacade.CleanUp();
            userFacade.CleanUp();
            supplierService = null;
            paymentService = null;
        }

        public void SetPaymentService(IPaymentService paymentService)
        {
            this.paymentService = paymentService;
        }

        public void SetSupplierService(ISupplierService supplierService)
        {
            this.supplierService = supplierService;
        }

        public ResponseT<bool> PlacePayment(string transactionDetails)
        {
            try
            {
                var task = Task.Run(() =>
                {
                    return paymentService.ValidatePayment(transactionDetails);
                });

                bool isCompletedSuccessfully = task.Wait(TimeSpan.FromMilliseconds(ExternalServiceWaitTimeInSeconds));

                if (isCompletedSuccessfully)
                {
                    return new ResponseT<bool>(task.Result);
                }
                else
                {
                    throw new TimeoutException("Payment external service action has taken longer than the maximum time allowed.");
                }
            }
            catch(Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return new ResponseT<bool>(ex.Message);
            }
        }

        public ResponseT<bool> PlaceSupply(string orderDetails, string userDetails)
        {
            try
            {
                var task = Task.Run(() =>
                {
                    return supplierService.ShipOrder(orderDetails,userDetails);
                });

                bool isCompletedSuccessfully = task.Wait(TimeSpan.FromMilliseconds(ExternalServiceWaitTimeInSeconds));

                if (isCompletedSuccessfully)
                {
                    return new ResponseT<bool>(task.Result);
                }
                else
                {
                    throw new TimeoutException("Supply external service action has taken longer than the maximum time allowed.");
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return new ResponseT<bool>(ex.Message);
            }
        }

        public ResponseT<bool> InitializeTradingSystem(int id)
        {
            try
            {
                Logger.Instance.Info("User id: " + id + "requested to initialize trading system");
                userFacade.InitializeTradingSystem(id);

                return new ResponseT<bool>(paymentService.Connect() && supplierService.Connect());
            }
            catch(Exception ex)
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
    }
}