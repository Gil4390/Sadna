using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using SadnaExpress.DomainLayer.Store;

namespace SadnaExpress.DomainLayer.Store
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
        
        public void AddOrderToUser(Guid userID, Order order)
        {
            List<Order> orders;
            if (!userOrders.TryGetValue(userID, out orders)) {
                orders = new List<Order>();
                userOrders.TryAdd(userID, orders);
            }
            orders.Add(order);
        }

        public void AddOrders(Guid userID, Dictionary<Guid, Dictionary<Guid, int>> items,
            Dictionary<Guid, double> prices)
        {
            foreach (Guid StoreID in items.Keys)
            {
                Order newOrder = new Order(userID, StoreID, items[StoreID], prices[StoreID]);
                AddOrder(newOrder);
            }
        }
        
        public void AddOrder(Order order)
        {
            AddOrderToStore(order.StoreID, order);
            AddOrderToUser(order.UserID, order);
        }

        public void AddOrderToStore(Guid storeID, Order order)
        {
            List<Order> orders;
            if (!storeOrders.TryGetValue(storeID, out orders)) {
                orders = new List<Order>();
                storeOrders.TryAdd(storeID, orders);
            }
            orders.Add(order);
        }

        public List<Order> GetOrdersByUserId(Guid userId)
        {
            List<Order> orders;
            if (userOrders.TryGetValue(userId, out orders)) {
                return orders;
            }
            return null;
        }

        public List<Order> GetOrdersByStoreId(Guid storeId)
        {
            List<Order> orders;
            if (storeOrders.TryGetValue(storeId, out orders)) {
                return orders;
            }
            return null;
        }

        public Dictionary<Guid, List<Order>> GetUserOrders()
        {
            return new Dictionary<Guid, List<Order>>(userOrders);
        }

        public Dictionary<Guid, List<Order>> GetStoreOrders()
        { 
            return new Dictionary<Guid, List<Order>>(storeOrders);
        }

        public static void CleanUp()
        {
            instance = null;
        }
    }
}