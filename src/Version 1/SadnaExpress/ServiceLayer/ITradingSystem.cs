using SadnaExpress.DomainLayer.Store;
using SadnaExpress.DomainLayer.User;
using SadnaExpress.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaExpress.ServiceLayer
{
    public interface ITradingSystem
    {
        ResponseT<int> Enter();

        ResponseT<int> Login(int id, string email, string password);

        bool CheckSupplierConnection();

        bool CheckPaymentConnection();

        Response Register(int id, string email, string firstName, string lastName, string password);

        void CleanUp();

        void SetPaymentService(IPaymentService paymentService);

        void SetSupplierService(ISupplierService supplierService);
    }
        
}
