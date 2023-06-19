﻿using SadnaExpress.API.Controllers;
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
using SadnaExpress.DomainLayer.Store.Policy;
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

        #region user operation
        [Route(APIConstants.MemberData.logout)]
        [ResponseType(typeof(ResponseT<Guid>))]
        [HttpPost]
        public IHttpActionResult Logout([FromBody] ClientRequest request)
        {
            Response res = tradingSystem.Logout(request.userID);
            return Ok(res);
        }

        #endregion

        #region Reviews
        [Route(APIConstants.MemberData.writeItemReview)]
        [ResponseType(typeof(Response))]
        [HttpPost]
        public IHttpActionResult WriteItemReview([FromBody] WriteItemReviewRequest request)
        {
            var res = tradingSystem.WriteItemReview(request.userID, request.ItemId, request.Review);
            return Ok(res);
        }

        [Route(APIConstants.MemberData.itemReviews)]
        [ResponseType(typeof(ResponseT<List<SReview>>))]
        [HttpPost]
        public IHttpActionResult GetItemReviews([FromBody] ItemIdRequest request)
        {
            var res = tradingSystem.GetItemReviews(request.ItemID);
            return Ok(res);
        }
        #endregion

        #region store manage
        [Route(APIConstants.MemberData.openStore)]
        [ResponseType(typeof(ResponseT<Guid>))]
        [HttpPost]
        public IHttpActionResult OpenNewStore([FromBody] OpenStoreRequest request)
        {
            return Ok(tradingSystem.OpenNewStore(request.userID, request.storeName));
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
            return Ok(tradingSystem.AppointStoreManager(request.userID, request.storeID, request.userEmail));
        }
        
        [Route(APIConstants.MemberData.addStoreManagerPer)]
        [ResponseType(typeof(Response))]
        [HttpPost]
        public IHttpActionResult AddStoreManagerPermissions([FromBody] StoreManagerPerRequest request)
        {
            Response res = tradingSystem.AddStoreManagerPermissions(request.userID, request.storeID, request.userEmail,
                request.permission);
            return Ok(res);
        }

        [Route(APIConstants.MemberData.appointStoreOwner)]
        [ResponseType(typeof(Response))]
        [HttpPost]
        public IHttpActionResult AppointStoreOwner([FromBody] StoreManagerRequest request)
        {
            Response res = tradingSystem.AppointStoreOwner(request.userID, request.storeID, request.userEmail);
            return Ok(res);
        }

        [Route(APIConstants.MemberData.removeStorePer)]
        [ResponseType(typeof(Response))]
        [HttpPost]
        public IHttpActionResult RemovePermission([FromBody] StoreManagerPerRequest request)
        {
            return Ok(tradingSystem.RemovePermission(request.userID, request.storeID, request.userEmail, request.permission));
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
        [ResponseType(typeof(ResponseT<List<SMemberForStore>>))]
        [HttpPost]
        public IHttpActionResult GetEmployeeInfoInStore([FromBody] StoreIDRequest request)
        {
            var res = tradingSystem.GetEmployeeInfoInStore(request.userID, request.storeID);
            return Ok(res);
        }

        [Route(APIConstants.MemberData.getAllConditions)]
        [ResponseType(typeof(ResponseT<SPolicy[]>))]
        [HttpPost]
        public IHttpActionResult GetAllConditions([FromBody] StoreIDRequest request)
        {
            ResponseT<SPolicy[] > a = tradingSystem.GetAllConditions(request.userID,request.storeID);
            return Ok(a);
        }
        
        [Route(APIConstants.MemberData.addCondition)]
        [ResponseType(typeof(Response))]
        [HttpPost]
        public IHttpActionResult AddCondition([FromBody] ConditionRequest request)
        {
            Response a = (Response)tradingSystem.AddCondition(request.userID,request.storeID, request.entity, request.entityName,
                request.type, request.value, DateTime.MaxValue, request.entityRes, request.entityNameRes,
                request.typeRes,
                request.valueRes, request.op, request.opCond);
            Response res = new Response(a.ErrorMessage);
            return Ok(res);
        }

        [Route(APIConstants.MemberData.addConditionForDiscount)]
        [ResponseType(typeof(Response))]
        [HttpPost]
        public IHttpActionResult AddConditionForDiscount([FromBody] ConditionRequest request)
        {
            Response a = tradingSystem.AddCondition(request.userID, request.storeID, request.entity, request.entityName,
                request.type, request.value, new DateTime(), request.entityRes, request.entityNameRes, request.typeRes,
                request.valueRes, request.op, request.opCond);
            Response res = new Response(a.ErrorMessage);
            return Ok(res);
        }
        
        [Route(APIConstants.MemberData.removeCondition)]
        [ResponseType(typeof(Response))]
        [HttpPost]
        public IHttpActionResult RemoveCondition([FromBody] ConditionIDRequest request)
        {
            return Ok(tradingSystem.RemoveCondition(request.userID,request.storeID,request.condID));
        }
        
        [Route(APIConstants.MemberData.createSimplePolicy)]
        [ResponseType(typeof(Response))]
        [HttpPost]
        public IHttpActionResult CreateSimplePolicy([FromBody] PolicyRequest request)
        {
            Response a = tradingSystem.CreateSimplePolicy(request.userID,request.storeID, request.level, request.percent, request.startDate,
                request.endDate);
            return Ok(new Response(a.ErrorMessage));
        }
        
        [Route(APIConstants.MemberData.createComplexPolicy)]
        [ResponseType(typeof(Response))]
        [HttpPost]
        public IHttpActionResult CreateComplexPolicy([FromBody] ComplexConditionRequest request)
        {
            Response res = tradingSystem.CreateComplexPolicy(request.userID,request.storeID,request.op, request.policys);
            return Ok(new Response(res.ErrorMessage));
        }
        
        [Route(APIConstants.MemberData.addPolicy)]
        [ResponseType(typeof(Response))]
        [HttpPost]
        public IHttpActionResult AddPolicy([FromBody] DiscountPolicyRequest request)
        {
            Response res = tradingSystem.AddPolicy(request.userID,request.storeID, request.discountPolicy);
            return Ok(res);
        }
        
        [Route(APIConstants.MemberData.removePolicy)]
        [ResponseType(typeof(Response))]
        [HttpPost]
        public IHttpActionResult RemovePolicy([FromBody] DiscountPolicyRemoveRequest request)
        {
            Response res = tradingSystem.RemovePolicy(request.userID,request.storeID, request.discountPolicy, request.type);
            return Ok(res);
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

        [Route(APIConstants.MemberData.getStorePurchases)]
        [ResponseType(typeof(ResponseT<Dictionary<Guid, List<Order>>>))]
        [HttpPost]
        public IHttpActionResult GetPurchasesOfStore([FromBody] StoreIDRequest request)
        {
            var res = tradingSystem.GetStorePurchases(request.userID, request.storeID);
            return Ok(res);
        }

        [Route(APIConstants.MemberData.getAllPolicy)]
        [ResponseType(typeof(List<SPolicy>))]
        [HttpPost]
        public IHttpActionResult GetAllPolicy([FromBody] StoreIDRequest request)
        {
            List<SPolicy> res = tradingSystem.GetAllPolicy(request.userID, request.storeID).Value;
            return Ok(res);
        }

        [Route(APIConstants.MemberData.getStoreRevenue)]
        [ResponseType(typeof(ResponseT<double>))]
        [HttpPost]
        public IHttpActionResult GetStoreRevenue([FromBody] StoreRevenueRequest request)
        {
            double res = tradingSystem.GetStoreRevenue(request.userID, request.storeID, request.date).Value;
            return Ok(res);
        }

        [Route(APIConstants.MemberData.getBidsInStore)]
        [ResponseType(typeof(SBid))]
        [HttpPost]
        public IHttpActionResult GetBidsInStore([FromBody] BidsInStoreRequest request)
        {
            ResponseT<SBid[]> res = tradingSystem.GetBidsInStore(request.userID, request.storeID);
            return Ok(res);
        }
        #endregion

        #region user data
        [Route(APIConstants.MemberData.getUserPurchases)]
        [ResponseType(typeof(ResponseT<Dictionary<Guid, List<Order>>>))]
        [HttpPost]
        public IHttpActionResult GetPurchasesOfUser([FromBody] ClientRequest request)
        {
            var res = tradingSystem.GetPurchasesOfUser(request.userID);
            return Ok(res);
        }

        [Route(APIConstants.MemberData.getMemberPermissions)]
        [ResponseType(typeof(ResponseT<Dictionary<Guid, SPermission>>))]
        [HttpPost]
        public IHttpActionResult GetMemberPermissions([FromBody] ClientRequest request)
        {
            return Ok(tradingSystem.GetMemberPermissions(request.userID));
        }

        [Route(APIConstants.MemberData.getMemberName)]
        [ResponseType(typeof(ResponseT<string>))]
        [HttpPost]
        public IHttpActionResult GetMemberName([FromBody] ClientRequest request)
        {
            var res = tradingSystem.GetMemberName(request.userID);
            return Ok(res);
        }
        #endregion

        #region Bids manage
        [Route(APIConstants.MemberData.reactToBid)]
        [ResponseType(typeof(Response))]
        [HttpPost]
        public IHttpActionResult ReactToBid([FromBody] ReactToBidRequest request)
        {
            Response res = tradingSystem.ReactToBid(request.userID, request.itemID, request.bidID, request.bidResponse);
            return Ok(res);
        }


        [Route(APIConstants.MemberData.reactToJobOffer)]
        [ResponseType(typeof(Response))]
        [HttpPost]
        public IHttpActionResult ReactToJobOffer([FromBody] ReactToJobOfferRequest request)
        {
            Response res = tradingSystem.ReactToJobOffer(request.userID, request.storeID, request.newEmpID, request.offerResponse);
            return Ok(res);
        }
        #endregion

        #region Notifications
        [Route(APIConstants.MemberData.getNotifications)]
        [ResponseType(typeof(ResponseT<List<Notification>>))]
        [HttpPost]
        public IHttpActionResult GetNotifications([FromBody] ClientRequest request)
        {
            var res = tradingSystem.GetNotifications(request.userID);
            return Ok(res);
        }

        [Route(APIConstants.MemberData.MarkNotificationAsRead)]
        [ResponseType(typeof(Response))]
        [HttpPost]
        public IHttpActionResult MarkNotificationAsRead([FromBody] NotificationRequest request)
        {
            var res = tradingSystem.MarkNotificationAsRead(request.userID, request.notificationID);
            return Ok(res);
        }
        #endregion

    }
}
