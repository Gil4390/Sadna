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
using SadnaExpress.DomainLayer.User;

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
        
        
        [Route(APIConstants.MemberData.appointStoreManager)]
        [ResponseType(typeof(Response))]
        [HttpPost]
        public IHttpActionResult AppointStoreManager([FromBody] StoreManagerRequest request)
        {
            return Ok(tradingSystem.AppointStoreManager(request.UserID, request.storeId, request.email));
        }
        
        [Route(APIConstants.MemberData.appointStoreManagerPer)]
        [ResponseType(typeof(Response))]
        [HttpPost]
        public IHttpActionResult AddStoreManagerPermissions([FromBody] StoreManagerPerRequest request)
        {
            return Ok(tradingSystem.AddStoreManagerPermissions(request.UserID, request.storeId, request.email,request.permission));
        }
        
        [Route(APIConstants.MemberData.appointStoreOwner)]
        [ResponseType(typeof(Response))]
        [HttpPost]
        public IHttpActionResult AppointStoreOwner([FromBody] StoreManagerRequest request)
        {
            return Ok(tradingSystem.AppointStoreOwner(request.UserID, request.storeId, request.email));
        }
        
        [Route(APIConstants.MemberData.removeStoreManagerPer)]
        [ResponseType(typeof(Response))]
        [HttpPost]
        public IHttpActionResult RemoveStoreManagerPermissions([FromBody] StoreManagerPerRequest request)
        {
            return Ok(tradingSystem.RemoveStoreManagerPermissions(request.UserID, request.storeId, request.email,request.permission));
        }
        
        [Route(APIConstants.MemberData.removeStoreOwner)]
        [ResponseType(typeof(Response))]
        [HttpPost]
        public IHttpActionResult RemoveStoreOwner([FromBody] StoreManagerRequest request)
        {
            return Ok(tradingSystem.RemoveStoreOwner(request.UserID, request.storeId, request.email));
        }
        
        [Route(APIConstants.MemberData.closeStore)]
        [ResponseType(typeof(Response))]
        [HttpPost]
        public IHttpActionResult CloseStore([FromBody] StoreOnlyRequest request)
        {
            return Ok(tradingSystem.CloseStore(request.UserID, request.storeId));
        }
        
        [Route(APIConstants.MemberData.getEmployee)]
        [ResponseType(typeof(ResponseT<List<PromotedMember>>))]
        [HttpPost]
        public IHttpActionResult GetEmployeeInfoInStore([FromBody] StoreOnlyRequest request)
        {
            return Ok(tradingSystem.GetEmployeeInfoInStore(request.UserID, request.storeId));
        }
        
        [Route(APIConstants.MemberData.getStorePurchases)]
        [ResponseType(typeof(ResponseT<List<Order>>))]
        [HttpPost]
        public IHttpActionResult GetStorePurchases([FromBody] StoreOnlyRequest request)
        {
            return Ok(tradingSystem.GetStorePurchases(request.UserID, request.storeId));
        }
        
        [Route(APIConstants.MemberData.deleteStore)]
        [ResponseType(typeof(Response))]
        [HttpPost]
        public IHttpActionResult DeleteStore([FromBody] StoreOnlyRequest request)
        {
            return Ok(tradingSystem.DeleteStore(request.UserID, request.storeId));
        }
        
        [Route(APIConstants.MemberData.updateFirst)]
        [ResponseType(typeof(ResponseT<Guid>))]
        [HttpPost]
        public IHttpActionResult UpdateFirst([FromBody] SQARequest request)
        {
            return Ok(tradingSystem.UpdateFirst(request.UserID, request.field));
        }
        
        [Route(APIConstants.MemberData.updateLast)]
        [ResponseType(typeof(ResponseT<Guid>))]
        [HttpPost]
        public IHttpActionResult UpdateLast([FromBody] SQARequest request)
        {
            return Ok(tradingSystem.UpdateLast(request.UserID, request.field));
        }
        
        [Route(APIConstants.MemberData.updatePass)]
        [ResponseType(typeof(ResponseT<Guid>))]
        [HttpPost]
        public IHttpActionResult UpdatePassword([FromBody] SQARequest request)
        {
            return Ok(tradingSystem.UpdatePassword(request.UserID, request.field));
        }
        
        [Route(APIConstants.MemberData.setSQA)]
        [ResponseType(typeof(ResponseT<Guid>))]
        [HttpPost]
        public IHttpActionResult SetSecurityQA([FromBody] SQARequest request)
        {
            return Ok(tradingSystem.SetSecurityQA(request.UserID, request.field, request.field2));
        }

    }
}
