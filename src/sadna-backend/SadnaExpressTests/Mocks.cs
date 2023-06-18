﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SadnaExpress.DataLayer;
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

            public virtual string Handshake()
            {
                return "OK";
            }

            public virtual int Supply(SSupplyDetails userDetails)
            {
                return 1000;
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

        public class Mock_4sec_SupplierService : Mock_SupplierService
        {
            public override int Supply(SSupplyDetails userDetails)
            {
                Thread.Sleep(4000); // Wait for 4 seconds
                return 100; // Return true after waiting
            }

        }

        public class Mock_bad_6sec_SupplierService : Mock_SupplierService
        {
            public override string Handshake()
            {
                Thread.Sleep(6000); // Wait for 15 seconds
                return "OK";
            }
        }

        public class Mock_bad_HandshakeFalse_SupplierService : Mock_SupplierService
        {
            public override string Handshake()
            {
                Thread.Sleep(3000); // Wait for 15 seconds
                return "Not OK";
            }
        }

        public class Mock_bad_BadHandshake_SupplierService : Mock_SupplierService
        {
            public override string Handshake()
            {
                Thread.Sleep(6000); // Wait for 6 seconds
                return "OK";
            }

            public override int Supply(SSupplyDetails userDetails)
            {
                return 100; 
            }
        }

        public class Mock_bad_BadSupply_SupplierService : Mock_SupplierService
        {
            public override int Supply(SSupplyDetails userDetails)
            {
                return -1;
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

        public class Mock_Bad_PaymentService : Mock_PaymentService
        {
            public override int Pay(double amount, SPaymentDetails transactionDetails)
            {
                Thread.Sleep(11000); // Wait for 11 seconds
                return -1; // Return true after waiting
            }
        }

        public class Mock_Bad_PaymentServicePayReturnError : Mock_PaymentService
        {
            public override int Pay(double amount, SPaymentDetails transactionDetails)
            {
                Thread.Sleep(2000); // Wait for 11 seconds
                return -1; // Return true after waiting
            }
        }

        public class Mock_Bad_Credit_Limit_Payment : Mock_PaymentService
        {
            public override int Pay(double amount, SPaymentDetails transactionDetails)
            {
                if (amount > 100)
                    return -1;
                return 3;
            }
        }

        public class Mock_4sec_PaymentService : Mock_PaymentService
        {
            public override int Pay(double amount, SPaymentDetails transactionDetails)
            {
                Thread.Sleep(4000); // Wait for 4 seconds
                return 1; // Return true after waiting
            }

        }

        public class Mock_bad_PaymentService : Mock_PaymentService
        {
            public override string Handshake()
            {
                return "NOT OK";
            }

            public override  int Pay(double amount, SPaymentDetails transactionDetails)
            {
                return -1;
            }

            public override bool Cancel_Pay(double amount, int transaction_id)
            {
                return false;
            }

        }

        public class Mock_bad_PaymentService_CancelPayToLong : Mock_PaymentService
        {
            public override string Handshake()
            {
                return "OK";
            }

            public override int Pay(double amount, SPaymentDetails transactionDetails)
            {
                return 1;
            }

            public override bool Cancel_Pay(double amount, int transaction_id)
            {
                Thread.Sleep(6000); 
                return true;
            }
        }

        public class Mock_bad_PaymentService_CancelPayFalse : Mock_PaymentService
        {
            public override string Handshake()
            {
                return "OK";
            }

            public override int Pay(double amount, SPaymentDetails transactionDetails)
            {
                return 1;
            }

            public override bool Cancel_Pay(double amount, int transaction_id)
            {
                Thread.Sleep(1000);
                return false;
            }
        }

        public class Mock_bad_BadHandshake_PaymentService : Mock_PaymentService
        {
            public override string Handshake()
            {
                Thread.Sleep(6000); // Wait for 6 seconds
                return "OK";
            }
            public override int Pay(double amount, SPaymentDetails transactionDetails)
            {
                return 2;
            }
        }

        public class Mock_bad_BadHandshakeFalse_PaymentService : Mock_PaymentService
        {
            public override string Handshake()
            {
                Thread.Sleep(2000); // Wait for 6 seconds
                return "Not OK";
            }
        }

        public class Mock_bad_15sec_PaymentService : Mock_PaymentService
        {
            public override string Handshake()
            {
                Thread.Sleep(10000); // Wait for 15 seconds
                return "OK";
            }
            public override  int Pay(double amount, SPaymentDetails transactionDetails)
            {
                Thread.Sleep(10000); // Wait for 15 seconds
                return 2;
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

            public void AddOrder(Guid userID, List<ItemForOrder> itemForOrders, bool AddToDB = true, DatabaseContext db=null)
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
