using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaExpress.DomainLayer.Store.DiscountPolicy;

namespace SadnaExpress.API.ClientRequests
{
    public class ClientRequest
    {
        public Guid UserID { get; set; }
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
    public class StoreRequest : ClientRequest
    {
        public Guid StoreId { get; set; }
    }
    public class ItemCartRequest : ClientRequest
    {
        public Guid StoreId { get; set; }
        public Guid ItemId { get; set; }
        public int ItemAmount { get; set; }
    }
    public class SearchItemRequest : ClientRequest
    {
        //Guid userID, string category, int minPrice = 0, int maxPrice = Int32.MaxValue, int ratingItem = -1, int ratingStore = -1
        public string ItemName { get; set; }
        public string Category { get; set; }
        public int MinPrice { get; set; }
        public int MaxPrice { get; set; }
        public int RatingItem { get; set; }
        public int RatingStore { get; set; }
        public string KeyWord { get; set; }
    }
    public class PurchaseRequest : ClientRequest
    {
        //Guid userID, string category, int minPrice = 0, int maxPrice = Int32.MaxValue, int ratingItem = -1, int ratingStore = -1
        public string PaymentDetails { get; set; }
        public string UsersDetail { get; set; }
    }
}
