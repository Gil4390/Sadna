using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaExpress.API.ClientRequests
{
    public class LoginRequest : ClientRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
