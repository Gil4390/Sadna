using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.Services;

namespace SadnaExpressTests
{
    public class Mocks
    {
        public class Mock_SupplierService : ISupplierService
        {
            bool isConnected = false;

            public Mock_SupplierService()
            {
                isConnected = true;
            }

            public virtual void CancelOrder(string orderNum)
            {

            }

            public virtual bool Connect()
            {
                return isConnected;
            }


            public virtual bool ShipOrder(string orderDetails, string userDetails)
            {
                return true;
            }

            public virtual bool ValidateAddress(string address)
            {
                return true;
            }
        }


        public class Mock_PaymentService : IPaymentService
        {
            bool isConnected = false;

            public Mock_PaymentService()
            {
                isConnected = true;
            }

            public virtual bool Connect()
            {
                return isConnected;
            }

            public virtual bool Pay(double amount, string transactionDetails)
            {
                return true;
            }
        }

        public class Mock_Orders : IOrders
        {
            protected ConcurrentDictionary<Guid, List<Order>> userOrders;
            protected ConcurrentDictionary<Guid, List<Order>> storeOrders;
            public Mock_Orders()
            {
                userOrders = new ConcurrentDictionary<Guid, List<Order>>();
                storeOrders = new ConcurrentDictionary<Guid, List<Order>>();
            }

            public void AddOrder(Guid userID, List<ItemForOrder> itemForOrders)
            {
                throw new NotImplementedException();
            }

            public void AddOrderToStore(Guid storeID, Order order)
            {
                if (storeOrders.ContainsKey(storeID) == false)
                    storeOrders.TryAdd(storeID, new List<Order>());
                storeOrders[storeID].Add(order);
            }

            public void AddOrderToUser(Guid userID, Order order)
            {
                if(userOrders.ContainsKey(userID)==false)
                    userOrders.TryAdd(userID, new List<Order>());
                userOrders[userID].Add(order);          
            }

            public void CleanUp()
            {
                userOrders.Clear();
                storeOrders.Clear();
            }

            public List<Order> GetOrdersByStoreId(Guid storeId)
            {
                return storeOrders[storeId];
            }

            public List<Order> GetOrdersByUserId(Guid userId)
            {
                return userOrders[userId];
            }

            public Dictionary<Guid, List<Order>> GetUserOrders()
            {
                return new Dictionary<Guid, List<Order>>(userOrders);
            }

            public Dictionary<Guid, List<Order>> GetStoreOrders()
            {
                return new Dictionary<Guid, List<Order>>(storeOrders);
            }
        }
    }
}
