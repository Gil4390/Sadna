using System.Collections.Generic;
using SadnaExpress.ServiceLayer.SModels;

namespace SadnaExpress.Services
{
    public interface ISupplierService
    {
        object Send(Dictionary<string, string> content);
        bool Handshake();
        int Supply(SSupplyDetails userDetails);
        bool Cancel_Supply(string transaction_id);

    }
}