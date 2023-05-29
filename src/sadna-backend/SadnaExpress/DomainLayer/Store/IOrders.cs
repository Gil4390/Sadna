using SadnaExpress.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaExpress.DomainLayer.Store
{
    public interface IOrders
    {
        void AddOrder(Guid userID, List<ItemForOrder> itemForOrders, bool AddToDB=false, DatabaseContext db=null);
        void AddOrderToUser(Guid userID, Order order);
        void AddOrderToStore(Guid storeID, Order order);
        List<Order> GetOrdersByUserId(Guid userId);
        List<Order> GetOrdersByStoreId(Guid storeId);
        Dictionary<Guid, List<Order>> GetUserOrders();
        Dictionary<Guid, List<Order>> GetStoreOrders();
        void CleanUp();
    }
}
