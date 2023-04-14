using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using SadnaExpress.DomainLayer.Store;

namespace SadnaExpress.DomainLayer.User
{
    public class Orders
    {
        private static readonly object syncRoot = new object();
        private static Dictionary<Guid, Order> userOrders;
        private static Dictionary<Guid, Order> storeOrders;

        private static Orders instance = null;

        private Orders()
        {
            userOrders = new Dictionary<Guid, Order>();
            storeOrders = new Dictionary<Guid, Order> ();
        }
        

        public static Orders Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new Orders();
                        }
                    }
                }

                return instance;
            }
        }
        public void AddOrderToUser(Guid userId, Order order)
        {
            lock (syncRoot)
            {
                userOrders[userId] = order;
            }
        }

        public void AddOrderToStore(Guid storeId, Order order)
        {
            lock (syncRoot)
            {
                storeOrders[storeId] = order;
            }
        }

        public List<Order> GetOrdersByUserId(Guid userId)
        {
            lock (syncRoot)
            {
                List<Order> orders = new List<Order>();
                foreach (KeyValuePair<Guid, Order> pair in userOrders)
                {
                    if (pair.Key == userId)
                    {
                        orders.Add(pair.Value);
                    }
                }
                return orders;
            }
        }

        public List<Order> GetOrdersByStoreId(Guid storeId)
        {
            lock (syncRoot)
            {
                List<Order> orders = new List<Order>();
                foreach (KeyValuePair<Guid, Order> pair in storeOrders)
                {
                    if (pair.Key == storeId)
                    {
                        orders.Add(pair.Value);
                    }
                }
                return orders;
            }
        }

        public Dictionary<Guid, Order> GetUserOrders()
        {
            lock (syncRoot)
            {
                return new Dictionary<Guid, Order>(userOrders);
            }
        }

        public Dictionary<Guid, Order> GetStoreOrders()
        {
            lock (syncRoot)
            {
                return new Dictionary<Guid, Order>(storeOrders);
            }
        }
    }
}