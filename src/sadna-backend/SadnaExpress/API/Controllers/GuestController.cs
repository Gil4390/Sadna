using SadnaExpress.API.ClientRequests;
using SadnaExpress.API.Controllers;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace SadnaExpress.API.Controllers
{
    [RoutePrefix(APIConstants.ApiRoot + APIConstants.GuestData.root)]
    public class GuestController : ApiController
    {
        ITradingSystem tradingSystem;

        public GuestController()
        {
            tradingSystem = TradingSystem.Instance;
        }

        [Route(APIConstants.GuestData.enter)]
        [ResponseType(typeof(Guid))]
        [HttpGet]
        public IHttpActionResult Enter()
        {
            return Ok(tradingSystem.Enter().Value);
        }

        [Route(APIConstants.GuestData.exit)]
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult Exit(Guid id)
        {
            tradingSystem.Exit(id);
            return Ok();
        }

        [Route(APIConstants.GuestData.register)]
        [ResponseType(typeof(Response))]
        [HttpPost]
        public IHttpActionResult Register([FromBody] RegisterRequest request)
        {
            Response res=tradingSystem.Register(request.userID, request.Email, request.FirstName, request.LastName, request.Password);
            return Ok(res);
        }

        [Route(APIConstants.GuestData.login)]
        [ResponseType(typeof(ResponseT<Guid>))]
        [HttpPost]
        public IHttpActionResult Login([FromBody] LoginRequest request)
        {
            ResponseT<Guid> res = tradingSystem.Login(request.userID, request.Email, request.Password);
            return Ok(res);
        }

        [Route(APIConstants.GuestData.storeInfo)]
        [ResponseType(typeof(List<Store>))]
        [HttpGet]
        public IHttpActionResult GetAllStoreInfo(Guid userID)
        {
            return Ok(tradingSystem.GetAllStoreInfo());
        }

        [Route(APIConstants.GuestData.itemByName)]
        [ResponseType(typeof(List<Item>))]
        [HttpGet]
        public IHttpActionResult GetItemsByName(Guid userID, string itemName, int minPrice = 0, int maxPrice = Int32.MaxValue, int ratingItem = -1, string category = null, int ratingStore = -1)
        {
            return Ok(tradingSystem.GetItemsByName(userID, itemName, minPrice, maxPrice, ratingItem, category, ratingStore));
        }

        [Route(APIConstants.GuestData.itemByCategory)]
        [ResponseType(typeof(List<Item>))]
        [HttpGet]
        public IHttpActionResult GetItemsByCategory(Guid userID, string category, int minPrice = 0, int maxPrice = Int32.MaxValue, int ratingItem = -1, int ratingStore = -1)
        {
            return Ok(tradingSystem.GetItemsByCategory(userID, category, minPrice, maxPrice, ratingItem, ratingStore));
        }

        [Route(APIConstants.GuestData.itemByKeysWord)]
        [ResponseType(typeof(List<Item>))]
        [HttpGet]
        public IHttpActionResult GetItemsByKeysWord(Guid userID, string keyWords, int minPrice = 0, int maxPrice = Int32.MaxValue, int ratingItem = -1, string category = null, int ratingStore = -1)
        {
            return Ok(tradingSystem.GetItemsByKeysWord(userID, keyWords, minPrice, maxPrice, ratingItem, category, ratingStore));
        }

        [Route(APIConstants.GuestData.addItemCart)]
        [ResponseType(typeof(void))]
        [HttpGet]
        public IHttpActionResult AddItemToCart(Guid userID, Guid storeID, Guid itemID, int itemAmount)
        {
            return Ok(tradingSystem.AddItemToCart(userID, storeID, itemID, itemAmount));
        }

        [Route(APIConstants.GuestData.removeItemCart)]
        [ResponseType(typeof(void))]
        [HttpGet]
        public IHttpActionResult RemoveItemFromCart(Guid userID, Guid storeID, Guid itemID)
        {
            return Ok(tradingSystem.RemoveItemFromCart(userID, storeID, itemID));
        }

        [Route(APIConstants.GuestData.editItemCart)]
        [ResponseType(typeof(void))]
        [HttpGet]
        public IHttpActionResult EditItemFromCart(Guid userID, Guid storeID, Guid itemID, int itemAmount)
        {
            return Ok(tradingSystem.EditItemFromCart(userID, storeID, itemID, itemAmount));
        }

        [Route(APIConstants.GuestData.shoppingCart)]
        [ResponseType(typeof(ShoppingCart))]
        [HttpGet]
        public IHttpActionResult GetDetailsOnCart(Guid userID)
        {
            return Ok(tradingSystem.GetDetailsOnCart(userID));
        }

        [Route(APIConstants.GuestData.purchaseCart)]
        [ResponseType(typeof(void))]
        [HttpGet]
        public IHttpActionResult PurchaseCart(Guid userID, string paymentDetails, string usersDetail)
        {
            return Ok(tradingSystem.PurchaseCart(userID, paymentDetails, usersDetail));
        }


    }
}
