namespace SadnaExpress.DomainLayer.User
{
    public interface IUserFacade
    {
        void enter(int id);
        void exit(int id);
        void register(int id, string email, string firstName, string lastLame, string password);
        void login(string email, string password);
        void logout();
        void addItemToBag(string storeName, string itemName);
        Dictionary<string,List<string>> getDetailsOnCart();
        void purchaseCart();
        void addItemCart(string storeName, string itemName);
        void removeCart(string storeName, string itemName);
        void editItemCart(string storeName, string itemName);
        void openStore(string storeName);
        void addReview(string storeName, string itemName);
        void addItemInventory(string storeName, string itemName);
        void removeItemInventory(string storeName, string itemName);
        void editItemInventory(string storeName, string itemName);
        void addOwner(string storeName, string email);
        void addManager(string storeName, string email);
        void addPermissionsToManager(string storeName, string email, string Permission);
        void closeStore(string storeName);
        void getDetailsOnStore(string storeName);
    }
}