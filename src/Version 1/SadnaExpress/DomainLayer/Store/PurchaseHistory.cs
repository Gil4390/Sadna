using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaExpress.DomainLayer.Store
{
    public class PurchaseHistory
    {
        private int purchaseId;
        private string username;
        private int storeId;
        //                itemId,    amount , price
        private List<Pair<int, Pair<int, int>>> items;
        private DateTime date;

        public PurchaseHistory(int purchaseId, string username, int storeId, DateTime date)
        {
            this.PurchaseId = purchaseId;
            this.Username = username;
            this.storeId = storeId;
            this.Date = date;
            Items = new List<Pair<int, Pair<int, int>>>();
        }

        public int PurchaseId { get => purchaseId; set => purchaseId = value; }
        public string Username { get => username; set => username = value; }
        public int StoreId { get => storeId; set => storeId = value; }
        public List<Pair<int, Pair<int, int>>> Items { get => items; set => items = value; }
        public DateTime Date { get => date; set => date = value; }

    }
}
