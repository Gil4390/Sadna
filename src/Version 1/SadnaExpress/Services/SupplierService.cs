using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaExpress.Services
{
    public interface SupplierService
    {
        bool connect();

        bool validateAddress(string address);

        string shipOrder(string address);

        void cancelOrder(string orderNum);

    }
}
