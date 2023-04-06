using System.Collections.Generic;

namespace SadnaExpress.DomainLayer.Store
{
    public interface IStoreFacade
    {
        void OpenNewStore(string storeName);
        void CloseStore(string storeName);
        void PurchaseItems(string storeName, List<string> itemsName);
        void GetStoreHistory(string storeName);
        void AddItem(string storeName, string itemName, string category, int price);
        void RemoveItem(string storeName, string itemName, string category, int price);
        void EditItemCategory(string storeName, string itemName, string category);
        void EditItemPrice(string storeName, string itemName, int price); 
        List<Item> GetItemsByName(string itemName);
        List<Item> GetItemsByCategory(string category);
        List<Item> GetItemsByKeysWord(string keyWords);
        List<Item> GetItemsByPrices(int minPrice, int maxPrice);
        List<Item> GetItemsByItemRating(int rating);
        List<Item> GetItemsByStoreRating(int rating);
        void ReviewItem(string storeName, string itemName, int rating);
        void CleanUp();

    }
}