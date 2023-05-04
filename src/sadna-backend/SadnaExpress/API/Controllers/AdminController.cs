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
    [RoutePrefix(APIConstants.ApiRoot + APIConstants.AdminData.root)]
    public class AdminController : ApiController
    {
        ITradingSystem tradingSystem;

        public AdminController()
        {
            tradingSystem = TradingSystem.Instance;
        }

        [Route(APIConstants.AdminData.isInit)]
        [ResponseType(typeof(bool))]
        [HttpGet]
        public IHttpActionResult IsSystemInitialize()
        {
            bool res = tradingSystem.IsSystemInitialize();
            return Ok(res);
        }
    }
}
