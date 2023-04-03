using System.Collections.Generic;

namespace SadnaExpress.DomainLayer.Store
{
    public class StoreFacade : IStoreFacade
    {
        Dictionary<int, Store> stores;
        
        public StoreFacade()
        {
            stores = new Dictionary<int, Store>();
        }
        
        public void OpenNewStore(string storeName)
        {
            throw new System.NotImplementedException();
        }

        public void CloseStore(string storeName)
        {
            throw new System.NotImplementedException();
        }

        public void PurchaseItems(string storeName, List<string> itemsName)
        {
            throw new System.NotImplementedException();
        }

        public void GetStoreHistory(string storeName)
        {
            throw new System.NotImplementedException();
        }

        public void AddItem(string storeName, string itemName, string category, int price)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveItem(string storeName, string itemName, string category, int price)
        {
            throw new System.NotImplementedException();
        }

        public void EditItemCategory(string storeName, string itemName, string category)
        {
            throw new System.NotImplementedException();
        }

        public void EditItemPrice(string storeName, string itemName, int price)
        {
            throw new System.NotImplementedException();
        }

        public List<Item> GetItemsByName(string itemName)
        {
            throw new System.NotImplementedException();
        }

        public List<Item> GetItemsByCategory(string category)
        {
            throw new System.NotImplementedException();
        }

        public List<Item> GetItemsByKeysWord(string keyWords)
        {
            throw new System.NotImplementedException();
        }

        public List<Item> GetItemsByPrices(int minPrice, int maxPrice)
        {
            throw new System.NotImplementedException();
        }

        public List<Item> GetItemsByItemRating(int rating)
        {
            throw new System.NotImplementedException();
        }

        public List<Item> GetItemsByStoreRating(int rating)
        {
            throw new System.NotImplementedException();
        }

        public void ReviewItem(string storeName, string itemName, int rating)
        {
            throw new System.NotImplementedException();
        }
    }
}