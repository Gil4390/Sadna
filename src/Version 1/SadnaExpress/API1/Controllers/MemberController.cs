using SadnaExpress.API1.Controllers;
using SadnaExpress.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace SadnaExpress.API1.Controllers
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
        [ResponseType(typeof(Guid))]
        [HttpPost]
        public IHttpActionResult Logout(Guid userid)
        {
            return Ok(tradingSystem.Logout(userid).Value);
        }

        [Route(APIConstants.MemberData.openStore)]
        [ResponseType(typeof(Guid))]
        [HttpGet]
        public IHttpActionResult OpenNewStore(Guid userID, string storeName)
        {
            return Ok(tradingSystem.OpenNewStore(userID, storeName).Value);
        }
    }
}
