using SadnaExpress.API.Controllers;
using SadnaExpress.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using SadnaExpress.API.ClientRequests;
using SadnaExpress.DomainLayer.Store;

namespace SadnaExpress.API.Controllers
{
    [RoutePrefix(APIConstants.MemberData.root)]
    public class MemberController : ApiController
    {
        ITradingSystem tradingSystem;

        public MemberController()
        {
            tradingSystem = TradingSystem.Instance;
        }

        [Route(APIConstants.MemberData.logout)]
        [ResponseType(typeof(ResponseT<Guid>))]
        [HttpPost]
        public IHttpActionResult Logout([FromBody] ClientRequest request)
        {
            return Ok(tradingSystem.Logout(request.UserID));
        }

        [Route(APIConstants.MemberData.openStore)]
        [ResponseType(typeof(ResponseT<Guid>))]
        [HttpPost]
        
        public IHttpActionResult OpenNewStore([FromBody] StoreRequest request)
        {
            return Ok(tradingSystem.OpenNewStore(request.UserID, request.storeName));
        }
        [Route(APIConstants.MemberData.writeItemReview)]
        [ResponseType(typeof(Response))]
        [HttpPost]
        public IHttpActionResult WriteItemReview([FromBody] WriteItemReviewRequest request)
        {
            return Ok(tradingSystem.WriteItemReview(request.UserID ,request.StoreId,request.ItemId,request.Review ));
        }

        [Route(APIConstants.MemberData.itemReviews)]
        [ResponseType(typeof(ResponseT<List<Review>>))]
        [HttpPost]
        public IHttpActionResult GetItemReviews([FromBody] ItemReview request)
        {
            return Ok(tradingSystem.GetItemReviews(request.StoreId,request.ItemId ));
        }
        
        [Route(APIConstants.MemberData.addItemToStore)]
        [ResponseType(typeof(ResponseT<Guid>))]
        [HttpPost]
        public IHttpActionResult AddItemToStore([FromBody] ItemRequest request)
        {
            return Ok(tradingSystem.AddItemToStore(request.UserID, request.storeID, request.itemName, request.itemCategory, request.itemPrice, request.quantity ));
        }
        
        [Route(APIConstants.MemberData.removeItemToStore)]
        [ResponseType(typeof(Response))]
        [HttpPost]
        public IHttpActionResult RemoveItemFromStore([FromBody] ItemReview request)
        {
            return Ok(tradingSystem.RemoveItemFromStore(request.UserID, request.StoreId , request.ItemId));
        }
        
                
        [Route(APIConstants.MemberData.editItemToStore)]
        [ResponseType(typeof(Response))]
        [HttpPost]
        public IHttpActionResult RemoveItemFromStore([FromBody] ItemEditRequest request)
        {
            return Ok(tradingSystem.EditItem(request.UserID, request.storeID, request.itemId,request.itemName, request.itemCategory, request.itemPrice, request.quantity ));
        }
        
    }
}
