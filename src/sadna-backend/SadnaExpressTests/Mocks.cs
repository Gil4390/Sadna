using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.ServiceLayer.SModels;
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

            public virtual bool Cancel_Supply(string orderNum)
            {
                return true;
            }

            public object Send(Dictionary<string, string> content)
            {
                throw new NotImplementedException();
            }

            public virtual bool Handshake()
            {
                return isConnected;
            }


            public virtual int Supply(SSupplyDetails userDetails)
            {
                return 1000;
            }
            
        }
        public class Mock_Bad_PaymentService : Mock_PaymentService
        {
            public override int Pay(double amount, SPaymentDetails transactionDetails)
            {
                Thread.Sleep(11000); // Wait for 11 seconds
                return -1; // Return true after waiting
            }
        }
        public class Mock_Bad_Credit_Limit_Payment : Mock_PaymentService
        {
            public override int Pay(double amount, SPaymentDetails transactionDetails)
            {
                if (amount > 100) 
                    return -1;
                return -1;
            }
        }

        public class Mock_5sec_PaymentService : Mock_PaymentService
        {
            public override int Pay(double amount, SPaymentDetails transactionDetails)
            {
                Thread.Sleep(5000); // Wait for 5 seconds
                return 1; // Return true after waiting
            }

        }

        public class Mock_Bad_SupplierService : Mock_SupplierService
        {
            public override int Supply(SSupplyDetails userDetails)
            {
                Thread.Sleep(11000); // Wait for 11 seconds
                return 100; // Return true after waiting
            }

        }
        
        public class Mock_Bad_Address_SupplierService : Mock_SupplierService
        {
            public override int Supply(SSupplyDetails userDetails)
            {
                if (!userDetails.ValidationSettings())
                    return -1;
                return 100; 
            }
        }

        public class Mock_5sec_SupplierService : Mock_SupplierService
        {
            public override int Supply(SSupplyDetails userDetails)
            {
                Thread.Sleep(5000); // Wait for 5 seconds
                return 100; // Return true after waiting
            }

        }

        public class Mock_PaymentService : IPaymentService
        {
            bool isConnected = false;

            public Mock_PaymentService()
            {
                isConnected = true;
            }


            public object Send(Dictionary<string, string> content)
            {
                throw new NotImplementedException();
            }

            public virtual string Handshake()
            {
                return "OK";
            }

            public virtual int Pay(double amount, SPaymentDetails transactionDetails)
            {
                return 10000;
            }
            public virtual bool Cancel_Pay(double amount, int transaction_id)
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
                if (userOrders.ContainsKey(userID) == false)
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
