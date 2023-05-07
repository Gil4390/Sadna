using SadnaExpress.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaExpressTests.Integration_Tests
{
    public class TradingSystemIT
    {
        protected TradingSystem trading;
        protected Guid userID;
        protected Guid buyerID;
        protected Guid storeID1;
        protected Guid storeID2;
        protected Guid itemID1;
        protected Guid itemID2;

        public virtual void Setup()
        {
            trading = new TradingSystem();
            trading.SetIsSystemInitialize(true);
            trading.TestMode = true;
            // create member
            userID = trading.Enter().Value;
            trading.Register(userID, "RotemSela@gmail.com", "noga", "schwartz", "asASD876!@");
            userID = trading.Login(userID, "RotemSela@gmail.com", "asASD876!@").Value;
            // open stores
            storeID1 = trading.OpenNewStore(userID, "hello").Value;
            storeID2 = trading.OpenNewStore(userID, "hello2").Value;
            // add items to stores
            
            itemID1 = trading.AddItemToStore(userID, storeID1, "ipad 32", "electronic", 4000, 3).Value;
            itemID2 = trading.AddItemToStore(userID, storeID2, "ipad 32", "electronic", 3000, 1).Value;
            // create guest
            buyerID = trading.Enter().Value;
            // add items to cart
            trading.AddItemToCart(buyerID, storeID1, itemID1, 2);
            trading.AddItemToCart(buyerID, storeID2, itemID2, 1);
        }

        public virtual void CleanUp()
        {
            trading.CleanUp();
        }
    }
}
