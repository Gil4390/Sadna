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
using SadnaExpress.DomainLayer;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.DomainLayer.Store.DiscountPolicy;
using SadnaExpress.DomainLayer.User;
using SadnaExpress.ServiceLayer.SModels;
using SadnaExpress.ServiceLayer.Obj;

namespace SadnaExpress.API.Controllers
{
    [RoutePrefix(APIConstants.ApiRoot + APIConstants.MemberData.root)]
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
            return Ok(tradingSystem.Logout(request.userID));
        }

        [Route(APIConstants.MemberData.openStore)]
        [ResponseType(typeof(ResponseT<Guid>))]
        [HttpPost]
        
        public IHttpActionResult OpenNewStore([FromBody] OpenStoreRequest request)
        {
            return Ok(tradingSystem.OpenNewStore(request.userID, request.storeName));
        }

        [Route(APIConstants.MemberData.writeItemReview)]
        [ResponseType(typeof(Response))]
        [HttpPost]
        public IHttpActionResult WriteItemReview([FromBody] WriteItemReviewRequest request)
        {
            return Ok(tradingSystem.WriteItemReview(request.userID ,request.StoreId,request.ItemId,request.Review ));
        }

        [Route(APIConstants.MemberData.itemReviews)]
        [ResponseType(typeof(ResponseT<List<Review>>))]
        [HttpPost]
        public IHttpActionResult GetItemReviews([FromBody] ItemIdRequest request)
        {
            return Ok(tradingSystem.GetItemReviews(request.StoreID,request.ItemID ));
        }
        
        [Route(APIConstants.MemberData.addItemToStore)]
        [ResponseType(typeof(ResponseT<Guid>))]
        [HttpPost]
        public IHttpActionResult AddItemToStore([FromBody] ItemRequest request)
        {
            return Ok(tradingSystem.AddItemToStore(request.userID, request.storeID, request.itemName, request.itemCategory, request.itemPrice, request.quantity ));
        }
        
        [Route(APIConstants.MemberData.removeItemToStore)]
        [ResponseType(typeof(Response))]
        [HttpPost]
        public IHttpActionResult RemoveItemFromStore([FromBody] ItemIdRequest request)
        {
            return Ok(tradingSystem.RemoveItemFromStore(request.userID, request.StoreID , request.ItemID));
        }
        
                
        [Route(APIConstants.MemberData.editItemToStore)]
        [ResponseType(typeof(Response))]
        [HttpPost]
        public IHttpActionResult EditItemFromStore([FromBody] ItemEditRequest request)
        {
            return Ok(tradingSystem.EditItem(request.userID, request.storeID, request.itemID,request.itemName, request.itemCategory, request.itemPrice, request.quantity ));
        }
        
        
        [Route(APIConstants.MemberData.appointStoreManager)]
        [ResponseType(typeof(Response))]
        [HttpPost]
        public IHttpActionResult AppointStoreManager([FromBody] StoreManagerRequest request)
        {
            return Ok(tradingSystem.AppointStoreManager(request.userID, request.storeID, request.email));
        }
        
        [Route(APIConstants.MemberData.appointStoreManagerPer)]
        [ResponseType(typeof(Response))]
        [HttpPost]
        public IHttpActionResult AddStoreManagerPermissions([FromBody] StoreManagerPerRequest request)
        {
            return Ok(tradingSystem.AddStoreManagerPermissions(request.userID, request.storeID, request.email,request.permission));
        }
        
        [Route(APIConstants.MemberData.appointStoreOwner)]
        [ResponseType(typeof(Response))]
        [HttpPost]
        public IHttpActionResult AppointStoreOwner([FromBody] StoreManagerRequest request)
        {
            return Ok(tradingSystem.AppointStoreOwner(request.userID, request.storeID, request.email));
        }
        
        [Route(APIConstants.MemberData.removeStoreManagerPer)]
        [ResponseType(typeof(Response))]
        [HttpPost]
        public IHttpActionResult RemoveStoreManagerPermissions([FromBody] StoreManagerPerRequest request)
        {
            return Ok(tradingSystem.RemoveStoreManagerPermissions(request.userID, request.storeID, request.email,request.permission));
        }
        
        [Route(APIConstants.MemberData.removeStoreOwner)]
        [ResponseType(typeof(Response))]
        [HttpPost]
        public IHttpActionResult RemoveStoreOwner([FromBody] StoreManagerRequest request)
        {
            return Ok(tradingSystem.RemoveStoreOwner(request.userID, request.storeID, request.email));
        }
        
        [Route(APIConstants.MemberData.closeStore)]
        [ResponseType(typeof(Response))]
        [HttpPost]
        public IHttpActionResult CloseStore([FromBody] StoreIDRequest request)
        {
            Response res = tradingSystem.CloseStore(request.userID, request.storeID);
            return Ok(res);
        }
        
        [Route(APIConstants.MemberData.getEmployee)]
        [ResponseType(typeof(ResponseT<List<PromotedMember>>))]
        [HttpPost]
        public IHttpActionResult GetEmployeeInfoInStore([FromBody] StoreIDRequest request)
        {
            return Ok(tradingSystem.GetEmployeeInfoInStore(request.userID, request.storeID));
        }
        
        [Route(APIConstants.MemberData.getStorePurchases)]
        [ResponseType(typeof(ResponseT<List<Order>>))]
        [HttpPost]
        public IHttpActionResult GetStorePurchases([FromBody] StoreIDRequest request)
        {
            return Ok(tradingSystem.GetStorePurchases(request.userID, request.storeID));
        }
        
        [Route(APIConstants.MemberData.deleteStore)]
        [ResponseType(typeof(Response))]
        [HttpPost]
        public IHttpActionResult DeleteStore([FromBody] StoreIDRequest request)
        {
            return Ok(tradingSystem.DeleteStore(request.userID, request.storeID));
        }
        
        [Route(APIConstants.MemberData.updateFirst)]
        [ResponseType(typeof(ResponseT<Guid>))]
        [HttpPost]
        public IHttpActionResult UpdateFirst([FromBody] SQARequest request)
        {
            return Ok(tradingSystem.UpdateFirst(request.userID, request.field));
        }
        
        [Route(APIConstants.MemberData.updateLast)]
        [ResponseType(typeof(ResponseT<Guid>))]
        [HttpPost]
        public IHttpActionResult UpdateLast([FromBody] SQARequest request)
        {
            return Ok(tradingSystem.UpdateLast(request.userID, request.field));
        }
        
        [Route(APIConstants.MemberData.updatePass)]
        [ResponseType(typeof(ResponseT<Guid>))]
        [HttpPost]
        public IHttpActionResult UpdatePassword([FromBody] SQARequest request)
        {
            return Ok(tradingSystem.UpdatePassword(request.userID, request.field));
        }
        
        [Route(APIConstants.MemberData.setSQA)]
        [ResponseType(typeof(ResponseT<Guid>))]
        [HttpPost]
        public IHttpActionResult SetSecurityQA([FromBody] SQARequest request)
        {
            return Ok(tradingSystem.SetSecurityQA(request.userID, request.field, request.field2));
        }
        
        [Route(APIConstants.MemberData.getStores)]
        [ResponseType(typeof(ResponseT<List<Store>>))]
        [HttpPost]
        public IHttpActionResult GetStores([FromBody] ClientRequest request)
        {
            return Ok(tradingSystem.GetStores());
        }
        
        [Route(APIConstants.MemberData.getStoreOwners)]
        [ResponseType(typeof(ResponseT<List<Member>>))]
        [HttpPost]
        public IHttpActionResult GetStoreOwners([FromBody] ClientRequest request)
        {
            return Ok(tradingSystem.GetStoreOwners());
        }
        
        [Route(APIConstants.MemberData.getStoresOwnerList)]
        [ResponseType(typeof(ResponseT<List<Member>>))]
        [HttpPost]
        public IHttpActionResult GetStoreOwnerOfStores([FromBody] ListGuidRequest request)
        {
            return Ok(tradingSystem.GetStoreOwnerOfStores(request.storeID));
        }
        
        [Route(APIConstants.MemberData.getUser)]
        [ResponseType(typeof(ResponseT<User>))]
        [HttpPost]
        public IHttpActionResult GetUser([FromBody] ClientRequest request)
        {
            return Ok(tradingSystem.GetUser(request.userID));
        }
        
        [Route(APIConstants.MemberData.getMember)]
        [ResponseType(typeof(ResponseT<Member>))]
        [HttpPost]
        public IHttpActionResult GetMember([FromBody] ClientRequest request)
        {
            return Ok(tradingSystem.GetMember(request.userID));
        }
        
        [Route(APIConstants.MemberData.getNotifications)]
        [ResponseType(typeof(ResponseT<List<Notification>>))]
        [HttpPost]
        public IHttpActionResult GetNotifications([FromBody] ClientRequest request)
        {
            return Ok(tradingSystem.GetNotifications(request.userID));
        }
        
        [Route(APIConstants.MemberData.getAllConditions)]
        [ResponseType(typeof(ResponseT<List<Condition>>))]
        [HttpPost]
        public IHttpActionResult GetAllConditions([FromBody] StoreIDRequest request)
        {
            return Ok(tradingSystem.GetAllConditions(request.storeID));
        }
        
        [Route(APIConstants.MemberData.getCondition)]
        [ResponseType(typeof(ResponseT<Condition>))]
        [HttpPost]
        public IHttpActionResult GetCondition<T , M>([FromBody] ConditionRequest<T , M> request)
        {
            return Ok(tradingSystem.GetCondition(request.storeID, request.entity, request.type,
                request.value, request.dt, request.entityRes, request.typeRes, request.valueRes));
        }
        
        [Route(APIConstants.MemberData.addCondition)]
        [ResponseType(typeof(ResponseT<Condition>))]
        [HttpPost]
        public IHttpActionResult AddCondition<T , M>([FromBody] ConditionRequest<T , M> request)
        {
            return Ok(tradingSystem.AddCondition(request.storeID, request.entity, request.type,
                request.value, request.dt, request.entityRes, request.typeRes, request.valueRes));
        }
        [Route(APIConstants.MemberData.removeCondition)]
        [ResponseType(typeof(Response))]
        [HttpPost]
        public IHttpActionResult RemoveCondition<T , M>([FromBody] ConditionRequest<T , M> request)
        {
            tradingSystem.RemoveCondition(request.storeID, request.entity, request.type,
                request.value, request.dt, request.entityRes, request.typeRes, request.valueRes);
            return Ok(new Response());
        }
        
        [Route(APIConstants.MemberData.addDiscountCondition)]
        [ResponseType(typeof(ResponseT<Condition>))]
        [HttpPost]
        public IHttpActionResult AddDiscountCondition<T , M>([FromBody] ConditionRequest<T , M> request)
        {
            return Ok(tradingSystem.AddDiscountCondition(request.storeID, request.entity, request.type, request.value));
        }
        
        [Route(APIConstants.MemberData.createSimplePolicy)]
        [ResponseType(typeof( ResponseT<DiscountPolicy>))]
        [HttpPost]
        public IHttpActionResult createSimplePolicy<T>([FromBody] PolicyRequest<T> request)
        {
            return Ok(tradingSystem.CreateSimplePolicy(request.storeID,request.level,request.percent,request.startDate,request.endDate));
        }
        
        [Route(APIConstants.MemberData.createComplexPolicy)]
        [ResponseType(typeof( ResponseT<DiscountPolicy>))]
        [HttpPost]
        public IHttpActionResult CreateComplexPolicy([FromBody] ComplexConditionRequest request)
        {
            return Ok(tradingSystem.CreateComplexPolicy(request.storeID,request.op, request.policys));
        }
        
        [Route(APIConstants.MemberData.addPolicy)]
        [ResponseType(typeof( ResponseT<DiscountPolicyTree>))]
        [HttpPost]
        public IHttpActionResult AddPolicy([FromBody] DiscountPolicyRequest request)
        {
            return Ok(tradingSystem.AddPolicy(request.storeID,request.discountPolicy));
        }
        
        [Route(APIConstants.MemberData.removePolicy)]
        [ResponseType(typeof( ResponseT<DiscountPolicyTree>))]
        [HttpPost]
        public IHttpActionResult RemovePolicy([FromBody] DiscountPolicyRequest request)
        {
            tradingSystem.RemovePolicy(request.storeID,request.discountPolicy);
            return Ok(new Response());
        }
        
        [Route(APIConstants.MemberData.getItems)]
        [ResponseType(typeof( ResponseT<List<Item>>))]
        [HttpPost]
        public IHttpActionResult GetItemsInStore([FromBody] StoreIDRequest request)
        {
            ResponseT<List<Item>> res = tradingSystem.GetItemsInStore(request.userID, request.storeID);
            return Ok(res);
        }

        [Route(APIConstants.MemberData.getStoreInfo)]
        [ResponseType(typeof(ResponseT<SStore>))]
        [HttpPost]
        public IHttpActionResult GetStoreInfo([FromBody] StoreIDRequest request)
        {
            return Ok(tradingSystem.GetStoreInfo(request.userID,request.storeID));
        }

        [Route(APIConstants.MemberData.getMemberPermissions)]
        [ResponseType(typeof(ResponseT<Dictionary<Guid, SPermission>>))]
        [HttpPost]
        public IHttpActionResult GetMemberPermissions([FromBody] ClientRequest request)
        {
            return Ok(tradingSystem.GetMemberPermissions(request.userID));
        }

        [Route(APIConstants.MemberData.getUserPurchases)]
        [ResponseType(typeof(ResponseT<Dictionary<Guid, List<Order>>>))]
        [HttpPost]
        public IHttpActionResult GetPurchasesOfUser([FromBody] ClientRequest request)
        {
            var res = tradingSystem.GetPurchasesInfoUserOnlu(request.userID);
            return Ok(res);
        }

        [Route(APIConstants.MemberData.getStorePurchases)]
        [ResponseType(typeof(ResponseT<Dictionary<Guid, List<Order>>>))]
        [HttpPost]
        public IHttpActionResult GetPurchasesOfStore([FromBody] StoreIDRequest request)
        {
            var res = tradingSystem.GetStorePurchases(request.userID, request.storeID);
            return Ok(res);
        }

    }
}
