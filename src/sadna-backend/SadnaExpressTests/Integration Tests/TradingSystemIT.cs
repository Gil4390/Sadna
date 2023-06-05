using SadnaExpress.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaExpress.API.SignalR;
using SadnaExpress.DataLayer;

namespace SadnaExpressTests.Integration_Tests
{
    public class TradingSystemIT
    {
        protected TradingSystem trading;
        protected Guid userID;
        protected Guid buyerMemberID;
        protected Guid buyerID;
        protected Guid storeID1;
        protected Guid storeID2;
        protected Guid itemID1;
        protected Guid itemID2;
        protected bool testMood = true;

        public virtual void Setup()
        {
            DatabaseContextFactory.TestMode = true;
            DBHandler.Instance.TestMood = testMood;
            DBHandler.Instance.CleanDB();
            NotificationNotifier.GetInstance().TestMood = true;
            trading = new TradingSystem();
            trading.SetIsSystemInitialize(true);
            trading.TestMode = true;
            // create member
            userID = trading.Enter().Value;
            trading.Register(userID, "RotemSela@gmail.com", "noga", "schwartz", "asASD876!@");
            userID = trading.Login(userID, "RotemSela@gmail.com", "asASD876!@").Value;

            // create buyer member
            buyerMemberID = trading.Enter().Value;
            trading.Register(buyerMemberID, "dor@gmail.com", "dor", "biton", "asASD876!@");
            buyerMemberID = trading.Login(buyerMemberID, "dor@gmail.com", "asASD876!@").Value;
            trading.CreateSystemManager(buyerMemberID);

            // open stores
            storeID1 = trading.OpenNewStore(userID, "hello").Value;
            storeID2 = trading.OpenNewStore(userID, "hello2").Value;
            // add items to stores
            
            itemID1 = trading.AddItemToStore(userID, storeID1, "ipad 32", "electronic", 4000, 3).Value;
            itemID2 = trading.AddItemToStore(userID, storeID2, "ipad 32", "electronic", 3000, 1).Value;
            // create guest
            buyerID = trading.Enter().Value;
            // add items to cart for buyer
            trading.AddItemToCart(buyerID, storeID1, itemID1, 2);
            trading.AddItemToCart(buyerID, storeID2, itemID2, 1);

            // add items to cart buyer member
            trading.AddItemToCart(buyerMemberID, storeID1, itemID1, 2);
            trading.AddItemToCart(buyerMemberID, storeID2, itemID2, 1);
        }

        public void setTestMood()
        {
            testMood = false;
        }

        public virtual void CleanUp()
        {
            trading.CleanUp();
        }
    }
}
