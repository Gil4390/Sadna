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
        private string storeName;
        //                itemId,    amount , price
        private List<Pair<int, Pair<int, int>>> items;
        private DateTime date;

        public PurchaseHistory(int purchaseId, string username, string storeName, DateTime date)
        {
            this.PurchaseId = purchaseId;
            this.Username = username;
            this.storeName = storeName;
            this.Date = date;
            Items = new List<Pair<int, Pair<int, int>>>();
        }

        public int PurchaseId { get => purchaseId; set => purchaseId = value; }
        public string Username { get => username; set => username = value; }
        public string StoreName { get => storeName; set => storeName = value; }
        public List<Pair<int, Pair<int, int>>> Items { get => items; set => items = value; }
        public DateTime Date { get => date; set => date = value; }

    }
}
