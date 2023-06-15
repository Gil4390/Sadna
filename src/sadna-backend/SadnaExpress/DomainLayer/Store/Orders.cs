using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using SadnaExpress.DataLayer;
using SadnaExpress.DomainLayer.Store;

namespace SadnaExpress.DomainLayer.Store
{
    public class Orders: IOrders
    {
        private static readonly object syncRoot = new object();
        private static ConcurrentDictionary<Guid, List<Order>> userOrders;
        private static ConcurrentDictionary<Guid, List<Order>> storeOrders;

        public ConcurrentDictionary<Guid, List<Order>> UserOrders { get => userOrders; set=>userOrders=value; }
        public ConcurrentDictionary<Guid, List<Order>> StoreOrders { get => storeOrders; set => storeOrders = value; }

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
        
         
        public void AddOrder(Guid userID, List<ItemForOrder> itemForOrders, bool AddToDB=true, DatabaseContext db=null)
        {
            Order userOrder = new Order(itemForOrders, userID);
            AddOrderToUser(userID, userOrder);

            Dictionary<Guid, List<ItemForOrder>> ordersForStores = new Dictionary<Guid, List<ItemForOrder>>();
            foreach (ItemForOrder item in itemForOrders)
            {
                if (!ordersForStores.ContainsKey(item.StoreID))
                    ordersForStores.Add(item.StoreID, new List<ItemForOrder>());
                ordersForStores[item.StoreID].Add(item);
            }

            foreach (Guid storeID in ordersForStores.Keys)
            {
                Order storeOrder = new Order(ordersForStores[storeID], userID);
                AddOrderToStore(storeID, storeOrder);
            }

            if(AddToDB)
                 DBHandler.Instance.AddOrder(db,userOrder);
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

        public double GetStoreRevenue(Guid storeID, DateTime date)
        {
            double sum = 0;
            foreach (Order order in GetOrdersByStoreId(storeID))
            {
                if (order.OrderTime.Date == date.Date)
                {
                    sum += order.CalculatorAmount();
                }
            }
            return sum;
        }
        
        public double GetSystemRevenue(DateTime date)
        {
            double sum = 0;
            foreach (List<Order> orderList in storeOrders.Values)
            {
                foreach (Order order in orderList)
                {
                    if (order.OrderTime.Date == date.Date)
                    {
                        sum += order.CalculatorAmount();
                    }
                }
            }
            return sum;
        }

        public void CleanUp()
        {
            instance = null;
        }

        public void LoadOrdersFromDB()
        {
            DBHandler.Instance.CanConnectToDatabase();
            List<Order> allOrders=DBHandler.Instance.GetAllOrders();

            foreach (Order order in allOrders)
            {
                AddOrder(order.UserID, order.ListItems,false);
            }
        }
    }
}