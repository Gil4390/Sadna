using SadnaExpress.DomainLayer.Store;

namespace ConsoleApp1.DomainLayer;

public interface IStoreFacade
{
    public void openNewShop(string storeName);
    public void closeStore(string storeName);
    public void purchaseItems(string storeName, List<string> itemsName);
    public void getStoreHistory(string storeName);
    public void addProduct(string storeName, string itemName, string category, int price);
    public void removeProduct(string storeName, string itemName, string category, int price);
    public void editProductCategory(string storeName, string itemName, string category);
    public void editProductPrice(string storeName, string itemName, int price);
    public List<Item> getProductsByName(string itemName);
    public List<Item> getProductsByCategory(string category);
    public List<Item> getProductsByKeysWord(string keyWords);
    public List<Item> getProductsByPrices(int minPrice, int maxPrice);
    public List<Item> getProductsByItemRating(int rating);
    public List<Item> getProductsByStoreRating(int rating);
    public void reviewProduct(string storeName, string itemName, int rating);

}