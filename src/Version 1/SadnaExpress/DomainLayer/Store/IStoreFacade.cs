using SadnaExpress.DomainLayer.Store;

namespace ConsoleApp1.DomainLayer;

public interface IStoreFacade
{
    public void openNewStore(string storeName);
    public void closeStore(string storeName);
    public void purchaseItems(string storeName, List<string> itemsName);
    public void getStoreHistory(string storeName);
    public void addItem(string storeName, string itemName, string category, int price);
    public void removeItem(string storeName, string itemName, string category, int price);
    public void editItemCategory(string storeName, string itemName, string category);
    public void editItemPrice(string storeName, string itemName, int price);
    public List<Item> getItemsByName(string itemName);
    public List<Item> getItemsByCategory(string category);
    public List<Item> getItemsByKeysWord(string keyWords);
    public List<Item> getItemsByPrices(int minPrice, int maxPrice);
    public List<Item> getItemsByItemRating(int rating);
    public List<Item> getItemsByStoreRating(int rating);
    public void reviewItem(string storeName, string itemName, int rating);

}