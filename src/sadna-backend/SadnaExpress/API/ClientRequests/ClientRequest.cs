using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaExpress.DomainLayer.Store.Policy;

namespace SadnaExpress.API.ClientRequests
{
    public class ClientRequest
    {
        public Guid userID { get; set; }
    }
    public class LoginRequest : ClientRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public class RegisterRequest : ClientRequest
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
    }

    public class OpenStoreRequest : ClientRequest
    {
        public string storeName { get; set; }
    }

    public class StoreRequest : ClientRequest
    {
        public Guid storeId { get; set; }
        public string storeName{ get; set; }
    }
    public class ItemCartRequest : ClientRequest
    {
        public Guid storeId { get; set; }
        public Guid itemID { get; set; }
        public int ItemAmount { get; set; }
    }
    public class SearchItemRequest : ClientRequest
    {
        //Guid userID, string category, int minPrice = 0, int maxPrice = Int32.MaxValue, int ratingItem = -1, int ratingStore = -1
        public string KeyWord { get; set; }
        public int MinPrice { get; set; }
        public int MaxPrice { get; set; }
        public int RatingItem { get; set; }
        public string Category { get; set; }
        public int RatingStore { get; set; }
       
    }
    public class PurchaseRequest : ClientRequest
    {
        public string PaymentDetails { get; set; }
        public string UsersDetails { get; set; }
    }
    public class WriteItemReviewRequest : ClientRequest
    {
        public Guid StoreId { get; set; }
        public Guid ItemId { get; set; }
        public string Review { get; set; }
    }

    public class ItemIdRequest : ClientRequest
    {
        public Guid StoreID { get; set; }
        public Guid ItemID { get; set; }
    }
    public class ItemRequest : ClientRequest
    {
        public Guid storeID { get; set; }
        public string itemName { get; set; }
        public string itemCategory { get; set; }
        public double itemPrice { get; set; }
        public int quantity { get; set; }

    }
    
    public class ItemEditRequest : ItemRequest
    {
        public Guid itemID { get; set; }

    }
    
    public class StoreIDRequest : ClientRequest
    {
        public Guid storeID { get; set; }

    }

    public class StoreManagerRequest : ClientRequest
    {
        public Guid storeID { get; set; }
        public string userEmail { get; set; }

    }
    public class StoreManagerPerRequest : StoreManagerRequest
    {
        public string permission { get; set; }
    }
    
    public class SQARequest : ClientRequest
    {
        public string field { get; set; }
        public string field2 { get; set; }

    }
    public class ListGuidRequest : ClientRequest
    {
        public List<Guid> storeID { get; set; }
    }
    
    
    public class ConditionRequest: ClientRequest
    {
        public Guid storeID { get; set; }
        public string entity { get; set; }
        public string entityName { get; set; }
        public string type { get; set; }
        public object value { get; set; }
        public string op { get; set; }
        public string entityRes { get; set; }
        public string entityNameRes { get; set; }
        public string typeRes { get; set; }
        public int valueRes { get; set; }
        
        public int opCond{ get; set; }
    }
    
    public class PolicyRequest : ClientRequest
    {
        public Guid storeID{ get; set; }
        public string level{ get; set; }
        public int percent { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
    }
    public class ComplexConditionRequest : ClientRequest
    {
        public Guid storeID{ get; set; }
        public string op{ get; set; }
        public int[] policys{ get; set; }
    }
    public class DiscountPolicyRequest : ClientRequest
    {
        public Guid storeID{ get; set; }
        public int discountPolicy{ get; set; }
    }
    public class DiscountPolicyRemoveRequest : DiscountPolicyRequest
    {
        public string type{ get; set; }
    }
    public class NotificationRequest : ClientRequest
    { public Guid notificationID { get; set;} }
    
    public class ConditionIDRequest : ClientRequest
    {
        public Guid storeID{ get; set; }
        public int condID{ get; set; }
    }

    public class StoreRevenueRequest : ClientRequest
    {
        public Guid storeID { get; set; }
        public DateTime date { get; set; }
    }
    public class SystemRevenueRequest : ClientRequest
    {
        public DateTime date { get; set; }
    }
}
