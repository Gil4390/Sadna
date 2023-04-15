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
        private static ConcurrentDictionary<Guid, List<Order>> userOrders;
        private static ConcurrentDictionary<Guid, List<Order>> storeOrders;

        private static Orders instance = null;

        private Orders()
        {
            userOrders = new ConcurrentDictionary<Guid, List<Order>>();
            storeOrders = new ConcurrentDictionary<Guid, List<Order>> ();
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
            List<Order> orders;
            if (!userOrders.TryGetValue(userId, out orders)) {
                orders = new List<Order>();
                userOrders.TryAdd(userId, orders);
            }
            orders.Add(order);
        }

        public void AddOrderToStore(Guid storeId, Order order)
        {
            List<Order> orders;
            if (!storeOrders.TryGetValue(storeId, out orders)) {
                orders = new List<Order>();
                storeOrders.TryAdd(storeId, orders);
            }
            orders.Add(order);
        }

        public List<Order> GetOrdersByUserId(Guid userId)
        {
            List<Order> orders;
            if (userOrders.TryGetValue(userId, out orders)) {
                return orders;
            }
            return new List<Order>();
        }

        public List<Order> GetOrdersByStoreId(Guid storeId)
        {
            List<Order> orders;
            if (storeOrders.TryGetValue(storeId, out orders)) {
                return orders;
            }
            return new List<Order>();
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