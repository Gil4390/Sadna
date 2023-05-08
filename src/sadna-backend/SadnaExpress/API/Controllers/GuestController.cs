using Microsoft.AspNet.SignalR;
using SadnaExpress.API.ClientRequests;
using SadnaExpress.API.Controllers;
using SadnaExpress.API.SignalR;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.ServiceLayer;
using SadnaExpress.ServiceLayer.Obj;
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
        private readonly IHubContext<NotificationHub> _notificationHubContext;

        public GuestController()
        {
            tradingSystem = TradingSystem.Instance;
        }

        [Route(APIConstants.GuestData.enter)]
        [ResponseType(typeof(Guid))]
        [HttpGet]
        public IHttpActionResult Enter()
        {
            ResponseT<Guid> res = tradingSystem.Enter();
            
            return Ok(res.Value);
        }

        [Route(APIConstants.GuestData.exit)]
        [ResponseType(typeof(Response))]
        [HttpPost]
        public IHttpActionResult Exit([FromBody] ClientRequest request)
        {
            tradingSystem.Exit(request.userID);
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

        [Route(APIConstants.GuestData.isAdmin)]
        [ResponseType(typeof(ResponseT<Guid>))]
        [HttpPost]
        public IHttpActionResult IsAdmin([FromBody] ClientRequest request)
        {
            ResponseT<bool> res = tradingSystem.IsAdmin(request.userID);
            return Ok(res);
        }

        [Route(APIConstants.GuestData.storeInfo)]
        [ResponseType(typeof(ResponseT<Store>))]
        [HttpPost]
        public IHttpActionResult GetStoreInfo([FromBody] StoreRequest request)
        {
            return Ok(tradingSystem.GetStore(request.storeId));
        }

        [Route(APIConstants.GuestData.searchItems)]
        [ResponseType(typeof(ResponseT<List<SItem>>))]
        [HttpPost]
        public IHttpActionResult GetItemsByKeysWord([FromBody] SearchItemRequest request)
        {
            if (request.MaxPrice == -1)
                request.MaxPrice = Int32.MaxValue;
            ResponseT<List<SItem>> res = tradingSystem.GetItemsForClient(request.userID, request.KeyWord, request.MinPrice, request.MaxPrice, request.RatingItem, request.Category, request.RatingStore);
            return Ok(res);
        }

        [Route(APIConstants.GuestData.addItemCart)]
        [ResponseType(typeof(Response))]
        [HttpPost]
        public IHttpActionResult AddItemToCart([FromBody] ItemCartRequest request)
        {
            Response res = tradingSystem.AddItemToCart(request.userID, request.storeId, request.itemID, request.ItemAmount);
            return Ok(res);
        }

        [Route(APIConstants.GuestData.removeItemCart)]
        [ResponseType(typeof(Response))]
        [HttpPost]
        public IHttpActionResult RemoveItemFromCart([FromBody] ItemCartRequest request)
        {
            Response res = tradingSystem.RemoveItemFromCart(request.userID, request.storeId, request.itemID);
            return Ok(res);
        }

        [Route(APIConstants.GuestData.editItemCart)]
        [ResponseType(typeof(Response))]
        [HttpPost]
        public IHttpActionResult EditItemFromCart([FromBody] ItemCartRequest request)
        {
            Response res = tradingSystem.EditItemFromCart(request.userID, request.storeId, request.itemID, request.ItemAmount);
            return Ok(res);
        }

        [Route(APIConstants.GuestData.shoppingCart)]
        [ResponseType(typeof(ResponseT<List<SItem>>))]
        [HttpPost]
        public IHttpActionResult GetDetailsOnCart([FromBody] ClientRequest request)
        {
            ResponseT<List<SItem>> res = tradingSystem.GetCartItems(request.userID);
            return Ok(res);
        }

        [Route(APIConstants.GuestData.purchaseCart)]
        [ResponseType(typeof(Response))]
        [HttpPost]
        public IHttpActionResult PurchaseCart([FromBody] PurchaseRequest request)
        {
            ResponseT<List<ItemForOrder>> res = tradingSystem.PurchaseCart(request.userID, request.PaymentDetails, request.UsersDetails);
            return Ok(new Response(res.ErrorMessage));
        }


    }
}
