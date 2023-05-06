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
using SadnaExpress.ServiceLayer.SModels;

namespace SadnaExpress.API.Controllers
{
    [RoutePrefix(APIConstants.ApiRoot + APIConstants.AdminData.root)]
    public class AdminController : ApiController
    {
        ITradingSystem tradingSystem;

        public AdminController()
        {
            tradingSystem = TradingSystem.Instance;
        }

        [Route(APIConstants.AdminData.isInit)]
        [ResponseType(typeof(ResponseT<bool>))]
        [HttpGet]
        public IHttpActionResult IsSystemInitialize()
        {
            return Ok(tradingSystem.IsSystemInitialize());
        }

        [Route(APIConstants.AdminData.InitTradingSystem)]
        [ResponseType(typeof(ResponseT<bool>))]
        [HttpPost]
        public IHttpActionResult Initialize([FromBody] ClientRequest request)
        {
            var res = tradingSystem.InitializeTradingSystem(request.userID);
            return Ok(res);
        }
        [Route(APIConstants.AdminData.allMembers)]
        [ResponseType(typeof(ResponseT<List<SMember>>))]
        [HttpPost]
        public IHttpActionResult GetAllMembers([FromBody] ClientRequest request)
        {
            
            var res = tradingSystem.GetMembers(request.userID);
            return Ok(res);
        }
        [Route(APIConstants.AdminData.allpurchases)]
        [ResponseType(typeof(ResponseT<List<Order>>))]
        [HttpPost]
        public IHttpActionResult GetAllPurchases([FromBody] ClientRequest request)
        {
            //all stores
            return Ok(tradingSystem.GetAllStorePurchases(request.userID).Value.Values.ToList());
        }
        
        [Route(APIConstants.AdminData.allpurchasesStore)]
        [ResponseType(typeof(ResponseT<List<Order>>))]
        [HttpPost]
        public IHttpActionResult GetAllPurchasesFromStore([FromBody] StoreIDRequest request)
        {
            //per stores
            return Ok(tradingSystem.GetAllStorePurchases(request.userID).Value[request.storeID]);
        }
        
        [Route(APIConstants.AdminData.allpurchasesUsers)]
        [ResponseType(typeof(ResponseT<Dictionary<Guid, List<Order>>>))]
        [HttpPost]
        public IHttpActionResult GetAllPurchasesFromUser([FromBody] ClientRequest request)
        {
            //all users
            var res = tradingSystem.GetPurchasesInfoUser(request.userID);
            return Ok(res);
        }
        
        [Route(APIConstants.AdminData.allpurchasesUser)]
        [ResponseType(typeof(ResponseT<List<ItemForOrder>>))]
        [HttpPost]
        public IHttpActionResult GetPurchasesInfoUserOnlu([FromBody] ClientRequest request)
        {
            //per user
            return Ok(tradingSystem.GetPurchasesInfoUserOnlu(request.userID));
        }
        
        [Route(APIConstants.AdminData.removeMember)]
        [ResponseType(typeof(Response))]
        [HttpPost]
        public IHttpActionResult RemoveMember([FromBody] RegisterRequest request)
        {
            var res = tradingSystem.RemoveUserMembership(request.userID, request.Email);
            return Ok(res);
        }

    }
}
