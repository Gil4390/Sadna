using SadnaExpress.DomainLayer.Store;
using SadnaExpress.DomainLayer.User;

namespace ConsoleApp1.DomainLayer;

public class StoreFacade : IStoreFacade
{
    Dictionary<int, Store> stores;

    public StoreFacade()
    {
        stores = new Dictionary<int, Store>();
    }

    public void addItem(string storeName, string itemName, string category, int price)
    {
        throw new NotImplementedException();
    }

    public void closeStore(string storeName)
    {
        throw new NotImplementedException();
    }

    public void editItemCategory(string storeName, string itemName, string category)
    {
        throw new NotImplementedException();
    }

    public void editItemPrice(string storeName, string itemName, int price)
    {
        throw new NotImplementedException();
    }

    public List<Item> getItemsByCategory(string category)
    {
        throw new NotImplementedException();
    }

    public List<Item> getItemsByItemRating(int rating)
    {
        throw new NotImplementedException();
    }

    public List<Item> getItemsByKeysWord(string keyWords)
    {
        throw new NotImplementedException();
    }

    public List<Item> getItemsByName(string itemName)
    {
        throw new NotImplementedException();
    }

    public List<Item> getItemsByPrices(int minPrice, int maxPrice)
    {
        throw new NotImplementedException();
    }

    public List<Item> getItemsByStoreRating(int rating)
    {
        throw new NotImplementedException();
    }

    public void getStoreHistory(string storeName)
    {
        throw new NotImplementedException();
    }

    public void openNewStore(string storeName)
    {
        throw new NotImplementedException();
    }

    public void purchaseItems(string storeName, List<string> itemsName)
    {
        throw new NotImplementedException();
    }

    public void removeItem(string storeName, string itemName, string category, int price)
    {
        throw new NotImplementedException();
    }

    public void reviewItem(string storeName, string itemName, int rating)
    {
        throw new NotImplementedException();
    }
}