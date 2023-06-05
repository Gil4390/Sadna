using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaExpress.DataLayer;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.DomainLayer.Store.Policy;

namespace SadnaExpressTests.Unit_Tests
{
    [TestClass()]
    public class PurchasePolicyUT
    {
        private Store store;
        private Guid item1;
        private Guid item2;
        private Guid item3;
        private DiscountPolicy policy1;
        private DiscountPolicy policy2;
        private DiscountPolicy policy3;
        private Condition cond1;
        private Condition cond2;
        private Condition cond3;
        private Condition cond4;
        private Dictionary<Item, int> basket;
        
        #region SetUp
        [TestInitialize]
        public void SetUp()
        {
            DatabaseContextFactory.TestMode = true;
            DBHandler.Instance.TestMood = true;
            DBHandler.Instance.CleanDB();
            store = new Store("Hello");
            item1 = store.AddItem("Bisli", "Food", 10.0, 2);
            item2 = store.AddItem("Bamba", "Food", 8.0, 2);
            item3 = store.AddItem("Ipad", "electronic", 4000, 2);
            basket = new Dictionary<Item, int> {{store.GetItemById(item1), 1}, {store.GetItemById(item2), 1},
                {store.GetItemById(item3), 1}};
        }
        #endregion
        
        #region Add new condition to purhcase policy
        [TestMethod]
        public void AddNewSimpleConditionSuccess()
        {
            //Act
            cond1 = store.AddCondition("Item","Bisli", "min quantity", 2, DateTime.MaxValue);
            //Assert
            Assert.IsTrue(store.PurchasePolicyList.Contains(cond1));
            Assert.IsFalse(store.PurchasePolicyList.Contains(cond2));
        }
        
        [TestMethod]
        public void AddNewSimpleConditionFail_WrongNum()
        {
            try
            {
                cond1 = store.AddCondition("Item","Bisli", "min quantity", -1,DateTime.MaxValue);
            }
            catch (Exception e)
            {
                Assert.IsTrue(true);
            }
        }

        [TestMethod]
        public void AddNewSimpleConditionFail_WrongString()
        {
            try
            {
                cond1 = store.AddCondition("Item","Bisli", "min num", 2, DateTime.MaxValue);
            }
            catch (Exception e)
            {
                Assert.IsTrue(true);
            }
        }
        
        [TestMethod]
        public void AddNewSimpleConditionFail_ItemNotExist()
        {
            try
            {
                cond1 = store.AddCondition("Item","blabla", "min num", 2, DateTime.MaxValue);
            }
            catch (Exception e)
            {
                Assert.IsTrue(true);
            }
        }
        #endregion
        
        #region No conditions purchase
        [TestMethod]
        public void NoConditionsPurchaseSuccess()
        {
            bool res = store.EvaluatePurchasePolicy(store, basket);
            Assert.IsTrue(res);
        }
        #endregion
        
        #region Simple condition purchase
        [TestMethod]
        public void Simple1Condition_Success_quantity()
        {
            //Arrange
            cond1 = store.AddCondition("Item","Bisli", "min quantity", 0, DateTime.MaxValue);
            //Act
            bool res = store.EvaluatePurchasePolicy(store, basket);
            //Assert
            Assert.IsTrue(res);
        }
        [TestMethod]
        public void Simple1Condition_Fail_maxQuantity()
        {
            //Arrange
            cond1 = store.AddCondition("Item","Bisli", "max quantity", 0, DateTime.MaxValue);
            //Assert
            Assert.ThrowsException<Exception>(()=> store.EvaluatePurchasePolicy(store, basket));
        }
        [TestMethod]
        public void Simple1Condition_Success_value()
        {
            //Arrange
            cond1 = store.AddCondition("Item","Bisli", "min value", 0, DateTime.MaxValue);
            //Act
            bool res = store.EvaluatePurchasePolicy(store, basket);
            //Assert
            Assert.IsTrue(res);
        }
        [TestMethod]
        public void Simple1Condition_Fail_value()
        {
            //Arrange
            cond1 = store.AddCondition("Item","Bisli", "max value", 0, DateTime.MaxValue);  
            //Assert
            Assert.ThrowsException<Exception>(()=>store.EvaluatePurchasePolicy(store, basket));
        }
        [TestMethod]
        public void Simple1Condition_Success_time()
        {
            //Arrange
            cond1 = store.AddCondition("Item","Bisli", "before time", 0 , DateTime.MaxValue);
            //Act
            bool res = store.EvaluatePurchasePolicy(store, basket);
            //Assert
            Assert.IsTrue(res);
        }
        [TestMethod]
        public void Simple1Condition_Fail_time()
        {
            //Arrange
            cond1 = store.AddCondition("Item","Bisli", "before time", 0 , new DateTime(1998/10/30));
            //Assert
            bool res = store.EvaluatePurchasePolicy(store, basket);
            Assert.IsFalse(res);
        }
        [TestMethod]
        public void Simple1Condition_Fail_time2()
        {
            //Arrange
            cond1 = store.AddCondition("Item","Bisli", "before time", 0 , DateTime.Today);
            //Assert
            bool res = store.EvaluatePurchasePolicy(store, basket);
            Assert.IsFalse(res);
        }
        
        
        #endregion

        #region Complex condition - 2 conditions
        [TestMethod]
        public void TwoConditions_Success_min_quantity_value()
        {
            //Arrange
            cond1 = store.AddCondition("Item","Bisli", "min quantity", 0, DateTime.MaxValue);
            cond2 = store.AddCondition("Item","Bisli", "min value", 0, DateTime.MaxValue);
    
            //Act
            bool res = store.EvaluatePurchasePolicy(store, basket);
            
            //Assert
            Assert.IsTrue(res);
        }
        [TestMethod]
        public void TwoConditions_Success_min_quantity_max_value()
        {
            //Arrange
            cond1 = store.AddCondition("Item","Bisli", "min quantity", 0, DateTime.MaxValue);
            cond2 = store.AddCondition("Item","Bisli", "max value", 1000000, DateTime.MaxValue);

            //Act
            bool res = store.EvaluatePurchasePolicy(store, basket);
            
            //Assert
            Assert.IsTrue(res);
        }
        [TestMethod]
        public void TwoConditions_Success_max_quantity_max_value()
        {
            //Arrange
            cond1 = store.AddCondition("Item","Bisli", "max quantity", 100, DateTime.MaxValue);
            cond2 = store.AddCondition("Item","Bisli", "max value", 1000000, DateTime.MaxValue);

            //Act
            bool res = store.EvaluatePurchasePolicy(store, basket);
            
            //Assert
            Assert.IsTrue(res);
        }
        [TestMethod]
        public void TwoConditions_Success_max_quantity_min_value()
        {
            //Arrange
            cond1 = store.AddCondition("Item","Bisli", "max quantity", 100, DateTime.MaxValue);
            cond2 = store.AddCondition("Item","Bisli", "min value", 0, DateTime.MaxValue);

            //Act
            bool res = store.EvaluatePurchasePolicy(store, basket);
            
            //Assert
            Assert.IsTrue(res);
        }
        
        [TestMethod]
        public void TwoConditions_fail_contradictory_quantity()
        {
            //Arrange
            cond1 = store.AddCondition("Item","Bisli", "max quantity", 0, DateTime.MaxValue);
            cond2 = store.AddCondition("Item","Bisli", "min quantity", 0, DateTime.MaxValue);
            
            //Assert
            Assert.ThrowsException<Exception>(()=>store.EvaluatePurchasePolicy(store, basket));
        }
        
        [TestMethod]
        public void TwoConditions_fail_contradictory_value()
        {
            //Arrange
            cond1 = store.AddCondition("Item","Bisli", "max value", 0, DateTime.MaxValue);
            cond2 = store.AddCondition("Item","Bisli", "min value", 0, DateTime.MaxValue);

            //Assert
            Assert.ThrowsException<Exception>(()=>store.EvaluatePurchasePolicy(store, basket));
        }
        #endregion
        
        #region Complex condition - 2 conditions
        [TestMethod]
        public void Complex_Success()
        {
            //Arrange
            cond1 = store.AddCondition("Item","Bisli", "min quantity", 1, DateTime.MaxValue);
            cond2 = store.AddCondition("Item","Bamba", "min value", 2, DateTime.MaxValue);
            cond3 = store.AddCondition("Item","Ipad", "min value", 3, DateTime.MaxValue);

            //Act
            bool res = store.EvaluatePurchasePolicy(store, basket);
            
            //Assert
            Assert.IsTrue(res);
        }
        [TestMethod]
        public void Complex_Fail()
        {
            //Arrange
            cond1 = store.AddCondition("Item","Bisli", "min quantity", 1, DateTime.MaxValue);
            cond2 = store.AddCondition("Item","Bamba", "min value", 0, DateTime.MaxValue);
            cond3 = store.AddCondition("Item","Bamba", "max value", 0, DateTime.MaxValue);
            //Act
            Assert.ThrowsException<Exception>(()=>store.EvaluatePurchasePolicy(store, basket));
        }
        
        [TestMethod]
        public void Complex_Or_Success()
        {
            //Arrange
            cond1 = store.AddCondition("Item","Bisli", "min quantity", 1, DateTime.MaxValue);
            cond2 = store.AddCondition("Item","Bamba", "min quantity", 0, DateTime.MaxValue);

            //Act
            store.AddSimplePurchaseCondition(cond1 , cond2 , "or");
            
            //Assert
            bool res = store.EvaluatePurchasePolicy(store, basket);
            Assert.IsTrue(res);
        }
        [TestMethod]
        public void Complex_Or_Fail()
        {
            //Arrange
            cond1 = store.AddCondition("Item","Bisli", "max quantity", 0, DateTime.MaxValue );
            cond2 = store.AddCondition("Item","Bamba", "max quantity", 0, DateTime.MaxValue);
            
            //Assert
            Assert.ThrowsException<Exception>(()=>store.EvaluatePurchasePolicy(store, basket));
        }
        #endregion
        
        #region Conditional condition
        [TestMethod]
        public void ConditionalSimple_Success()
        {
            //Arrange
            cond1 = store.AddCondition("Item","Bisli", "min quantity", 0, DateTime.MaxValue);
            cond2 = store.AddCondition("Item","Bamba", "max quantity", 2, DateTime.MaxValue);
            Condition cc1 = store.AddSimplePurchaseCondition(cond1, cond2,"if");

            //Act
            bool res = store.EvaluatePurchasePolicy(store, basket);
            
            //Assert
            Assert.IsTrue(res);
        }
        [TestMethod]
        public void ConditionalSimple_Fail_condition()
        {
            //Arrange
            cond1 = store.AddCondition("Item","Bisli", "min quantity", 100,DateTime.MaxValue);
            Condition cc1 = store.AddCondition("Item", "Bisli","max quantity", 1000,DateTime.MaxValue,"if", cond1.ID);
            
            //Assert
            Assert.ThrowsException<Exception>(() => store.EvaluatePurchasePolicy(store, basket));
        }
        [TestMethod]
        public void ConditionalSimple_Fail_basket()
        {
            //Arrange
            cond1 = store.AddCondition("Item","Bisli", "min quantity", 30, DateTime.MaxValue);
            Condition cc1 = store.AddCondition("Item","Bisli","min value", 0, DateTime.MaxValue,"if", cond1.ID);
            
            //Act
            Assert.ThrowsException<Exception>(()=>store.EvaluatePurchasePolicy(store, basket));
        }
        #endregion
        
        #region Conditional condition - complex condition
        [TestMethod]
        public void ConditionalComplex_Success()
        {
            //Arrange
            cond1 = store.AddCondition("Item","Bisli", "min quantity", 1, DateTime.MaxValue);
            cond2 = store.AddCondition("Item","Bisli", "min value", 2, DateTime.MaxValue);
            cond3 = store.AddSimplePurchaseCondition(cond1, cond2, "and");
            
            //Act
            Condition cc1 = store.AddCondition( "Item","Bamba","min quantity", 0, DateTime.MaxValue,"if", cond3.ID);
            
            //Assert
            bool res = store.EvaluatePurchasePolicy(store, basket);
            Assert.IsTrue(res);
        }
        [TestMethod]
        public void ConditionalComplex_Fail()
        {
            //Arrange
            cond1 = store.AddCondition("Item","Bamba", "max quantity", 0, DateTime.MaxValue);
            Condition cc1 = store.AddCondition("Item","Bisli","max value", 15, DateTime.MaxValue,"if", cond1.ID);
            //Act
            Assert.ThrowsException<Exception>(()=>store.EvaluatePurchasePolicy(store, basket));
        }
        #endregion
        
        #region Get condition
        [TestMethod]
        public void GetConditionSuccess()
        {
            //Act
            cond1 = store.AddCondition("Item", "Bisli", "min quantity", 0, DateTime.MaxValue);
            //Assert
            Assert.IsTrue(store.PurchasePolicyList.Contains(cond1));
        }
        #endregion
        
        #region Remove condition
        [TestMethod]
        public void RemoveConditionSuccess()
        {
            //Arrange
            cond1 = store.AddCondition("Item", "Bisli", "min quantity", 0, DateTime.MaxValue);
            //Act
            store.RemoveCondition(cond1.ID);
            //Assert
            Assert.IsNull(store.GetPurchaseCond(cond1.ID));
            Assert.IsFalse(store.PurchasePolicyList.Contains(cond1));
        }
        
        [TestMethod]
        public void RemoveComplexConditionSuccess()
        {
            //Arrange
            cond1 = store.AddCondition("Item", "Bisli", "min quantity", 0, DateTime.MaxValue);
            cond2 = store.AddCondition("Item", "Bisli", "min quantity", 2, DateTime.MaxValue, "and", cond1.ID);
            //Act
            store.RemoveCondition(cond2.ID);
            //Assert
            Assert.IsNull(store.GetPurchaseCond(cond2.ID));
        }
        
        [TestMethod]
        public void RemoveSimpleConditionSuccess()
        {
            //Arrange
            cond1 = store.AddCondition("Item", "Bisli", "min quantity", 1,DateTime.MaxValue);
            cond2 = store.AddCondition("Item", "Bisli", "min quantity", 2, DateTime.MaxValue);

            Assert.IsTrue(store.PurchasePolicyList.Contains(cond1));
            Assert.IsTrue(store.PurchasePolicyList.Contains(cond2));
             //Act
            store.RemoveCondition(cond2.ID);
            
            //Assert
            Assert.IsTrue(store.PurchasePolicyList.Contains(cond1));
            Assert.IsFalse(store.PurchasePolicyList.Contains(cond2));
        }

        [TestMethod]
        public void RemoveSimpleConditionFailNull()
        {
            //Arrange
            cond1 = store.AddCondition("Item", "Bisli", "min quantity", 1, DateTime.MaxValue);
            cond2 = store.AddCondition("Item", "Bisli", "min quantity", 2, DateTime.MaxValue);
            Assert.IsTrue(store.PurchasePolicyList.Contains(cond1));
            Assert.IsTrue(store.PurchasePolicyList.Contains(cond2));

            //Assert
            Assert.ThrowsException<Exception>(() => store.RemoveCondition(7));
            Assert.IsTrue(store.PurchasePolicyList.Contains(cond1));
            Assert.IsTrue(store.PurchasePolicyList.Contains(cond2));
        }
        [TestMethod]
        public void RemoveConditionalConditionSuccess()
        {
            //Arrange
            cond1 = store.AddCondition("Item", "Bisli", "min quantity", 0, DateTime.MaxValue);
            Condition cc1 = store.AddCondition("Item", "Bisli" ,"max quantity", 3,DateTime.MaxValue,"if", cond1.ID);
            //Act
            store.RemoveCondition(cc1.ID);
            //Assert
            Assert.IsFalse(store.PurchasePolicyList.Contains(cc1));
        }
        #endregion
        
        #region Clean Up
        [TestCleanup]
        public void CleanUp()
        {
            store.PurchasePolicyList = new List<Condition>();
        }
        #endregion
    }
}