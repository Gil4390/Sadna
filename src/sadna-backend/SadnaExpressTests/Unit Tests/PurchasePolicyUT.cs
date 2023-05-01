using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.DomainLayer.Store.DiscountPolicy;

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
            store = new Store("Hello");
            item1 = store.AddItem("Bisli", "Food", 10.0, 2);
            item2 = store.AddItem("Bamba", "Food", 8.0, 2);
            item3 = store.AddItem("Ipad", "electronic", 4000, 2);
            basket = new Dictionary<Item, int> {{store.GetItemById(item1), 1}, {store.GetItemById(item2), 1},
                {store.GetItemById(item3), 1}};
            // basket = new Dictionary<Item, int>();
            // cond1 = store.AddCondition(item1, "min quantity", 2);
            // // cond2 = store.AddCondition(store, "min value", 0);
            // //store.AddSimplePurchaseCondition(cond1);
            // //store.AddSimplePurchaseCondition(cond2);
            // ConditioningResultSum s1 = new ConditioningResultSum(store.GetItemById(item1), 1);
            // ConditioningCondition cc1 = new ConditioningCondition(cond1, s1);
            // store.AddSimplePurchaseCondition(cc1);
        }
        #endregion
        
        #region Add new condition to purhcase policy
        [TestMethod]
        public void AddNewSimpleConditionSuccess()
        {
            cond1 = store.AddCondition(store.GetItemById(item1), "min quantity", 2);
            store.AddSimplePurchaseCondition(cond1);
            Assert.IsNotNull(store.PurchasePolicy.cond1);
            Assert.IsNull(store.PurchasePolicy.cond2);
        }
        
        [TestMethod]
        public void AddNewSimpleConditionFail_WrongNum()
        {
            try
            {
                cond1 = store.AddCondition(item1, "min quantity", -1);
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
                cond1 = store.AddCondition(item1, "min num", 2);
            }
            catch (Exception e)
            {
                Assert.IsTrue(true);
            }
        }
        
        [TestMethod]
        public void AddNewSimpleConditionFail_EmptyItem()
        {
            try
            {
                cond1 = store.AddCondition(new Item("a","b",1), "min num", 2);
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
            cond1 = store.AddCondition(store.GetItemById(item1), "min quantity", 0);
            store.AddSimplePurchaseCondition(cond1);
            bool res = store.EvaluatePurchasePolicy(store, basket);
            Assert.IsTrue(res);
        }
        [TestMethod]
        public void Simple1Condition_Fail_maxQuantity()
        {
            cond1 = store.AddCondition(store.GetItemById(item1), "max quantity", 0);
            store.AddSimplePurchaseCondition(cond1);
            bool res = store.EvaluatePurchasePolicy(store, basket);
            Assert.IsFalse(res);
        }
        [TestMethod]
        public void Simple1Condition_Success_value()
        {
            cond1 = store.AddCondition(store.GetItemById(item1), "min value", 0);
            store.AddSimplePurchaseCondition(cond1);
            bool res = store.EvaluatePurchasePolicy(store, basket);
            Assert.IsTrue(res);
        }
        [TestMethod]
        public void Simple1Condition_Fail_value()
        {
            cond1 = store.AddCondition(store.GetItemById(item1), "max value", 0);
            store.AddSimplePurchaseCondition(cond1);
            bool res = store.EvaluatePurchasePolicy(store, basket);
            Assert.IsFalse(res);
        }
        [TestMethod]
        public void Simple1Condition_Success_time()
        {
            cond1 = store.AddCondition(store.GetItemById(item1), "before time", 0 , DateTime.MaxValue);
            store.AddSimplePurchaseCondition(cond1);
            bool res = store.EvaluatePurchasePolicy(store, basket);
            Assert.IsTrue(res);
        }
        [TestMethod]
        public void Simple1Condition_Fail_time()
        {
            cond1 = store.AddCondition(store.GetItemById(item1), "after time", 0 , DateTime.MaxValue);
            store.AddSimplePurchaseCondition(cond1);
            bool res = store.EvaluatePurchasePolicy(store, basket);
            Assert.IsFalse(res);
        }
        #endregion
        
        #region Complex condition - 2 conditions
        [TestMethod]
        public void TwoConditions_Success_min_quantity_value()
        {
            cond1 = store.AddCondition(store.GetItemById(item1), "min quantity", 0);
            cond2 = store.AddCondition(store.GetItemById(item1), "min value", 0);
            store.AddSimplePurchaseCondition(cond1);
            store.AddSimplePurchaseCondition(cond2);
            bool res = store.EvaluatePurchasePolicy(store, basket);
            Assert.IsTrue(res);
        }
        [TestMethod]
        public void TwoConditions_Success_min_quantity_max_value()
        {
            cond1 = store.AddCondition(store.GetItemById(item1), "min quantity", 0);
            cond2 = store.AddCondition(store.GetItemById(item1), "max value", 1000000);
            store.AddSimplePurchaseCondition(cond1);
            store.AddSimplePurchaseCondition(cond2);
            bool res = store.EvaluatePurchasePolicy(store, basket);
            Assert.IsTrue(res);
        }
        [TestMethod]
        public void TwoConditions_Success_max_quantity_max_value()
        {
            cond1 = store.AddCondition(store.GetItemById(item1), "max quantity", 100);
            cond2 = store.AddCondition(store.GetItemById(item1), "max value", 1000000);
            store.AddSimplePurchaseCondition(cond1);
            store.AddSimplePurchaseCondition(cond2);
            bool res = store.EvaluatePurchasePolicy(store, basket);
            Assert.IsTrue(res);
        }
        [TestMethod]
        public void TwoConditions_Success_max_quantity_min_value()
        {
            cond1 = store.AddCondition(store.GetItemById(item1), "max quantity", 100);
            cond2 = store.AddCondition(store.GetItemById(item1), "min value", 0);
            store.AddSimplePurchaseCondition(cond1);
            store.AddSimplePurchaseCondition(cond2);
            bool res = store.EvaluatePurchasePolicy(store, basket);
            Assert.IsTrue(res);
        }
        
        [TestMethod]
        public void TwoConditions_fail_contradictory_quantity()
        {
            cond1 = store.AddCondition(store.GetItemById(item1), "max quantity", 0);
            cond2 = store.AddCondition(store.GetItemById(item1), "min quantity", 0);
            store.AddSimplePurchaseCondition(cond1);
            store.AddSimplePurchaseCondition(cond2);
            bool res = store.EvaluatePurchasePolicy(store, basket);
            Assert.IsFalse(res);
        }
        [TestMethod]
        public void TwoConditions_fail_contradictory_value()
        {
            cond1 = store.AddCondition(store.GetItemById(item1), "max value", 0);
            cond2 = store.AddCondition(store.GetItemById(item1), "min value", 0);
            store.AddSimplePurchaseCondition(cond1);
            store.AddSimplePurchaseCondition(cond2);
            bool res = store.EvaluatePurchasePolicy(store, basket);
            Assert.IsFalse(res);
        }
        #endregion
        
        #region Complex condition - 2 conditions
        [TestMethod]
        public void Complex_Success()
        {
            cond1 = store.AddCondition(store.GetItemById(item1), "min quantity", 1);
            cond2 = store.AddCondition(store.GetItemById(item2), "min value", 2);
            cond3 = store.AddCondition(store.GetItemById(item3), "min value", 3);

            store.AddSimplePurchaseCondition(cond1);
            store.AddSimplePurchaseCondition(cond2);
            store.AddSimplePurchaseCondition(cond3);
            bool res = store.EvaluatePurchasePolicy(store, basket);
            Assert.IsTrue(res);
        }
        [TestMethod]
        public void Complex_Fail()
        {
            cond1 = store.AddCondition(store.GetItemById(item1), "min quantity", 1);
            cond2 = store.AddCondition(store.GetItemById(item2), "min value", 0);
            cond3 = store.AddCondition(store.GetItemById(item2), "max value", 0);

            store.AddSimplePurchaseCondition(cond1);
            store.AddSimplePurchaseCondition(cond2);
            store.AddSimplePurchaseCondition(cond3);
            bool res = store.EvaluatePurchasePolicy(store, basket);
            Assert.IsFalse(res);
        }
        [TestMethod]
        public void Complex_Or_Success()
        {
            cond1 = store.AddCondition(store.GetItemById(item1), "min quantity", 1);
            cond2 = store.AddCondition(store.GetItemById(item2), "min quantity", 0);

            store.AddSimplePurchaseCondition(cond1);
            store.AddSimplePurchaseCondition(cond2 , null , new OrOperator());
            bool res = store.EvaluatePurchasePolicy(store, basket);
            Assert.IsTrue(res);
        }
        [TestMethod]
        public void Complex_Or_Fail()
        {
            cond1 = store.AddCondition(store.GetItemById(item1), "max quantity", 0);
            cond2 = store.AddCondition(store.GetItemById(item2), "max quantity", 0);

            store.AddSimplePurchaseCondition(cond1);
            store.AddSimplePurchaseCondition(cond2 , null , new OrOperator());
            bool res = store.EvaluatePurchasePolicy(store, basket);
            Assert.IsFalse(res);
        }
        #endregion
        
        #region Conditional condition
        [TestMethod]
        public void ConditionalSimple_Success()
        {
            cond1 = store.AddCondition(store.GetItemById(item1), "min quantity", 0);
            ConditioningCondition cc1 = store.AddConditioning(cond1, store.GetItemById(item1),"sum", 3);
            store.AddSimplePurchaseCondition(cc1);
            bool res = store.EvaluatePurchasePolicy(store, basket);
            Assert.IsTrue(res);
        }
        [TestMethod]
        public void ConditionalSimple_Fail_condition()
        {
            cond1 = store.AddCondition(store.GetItemById(item1), "min quantity", 100);
            ConditioningCondition cc1 = store.AddConditioning(cond1, store.GetItemById(item1),"sum", 3);
            store.AddSimplePurchaseCondition(cc1);
            bool res = store.EvaluatePurchasePolicy(store, basket);
            Assert.IsFalse(res);
        }
        [TestMethod]
        public void ConditionalSimple_Fail_basket()
        {
            cond1 = store.AddCondition(store.GetItemById(item1), "min quantity", 0);
            ConditioningCondition cc1 = store.AddConditioning(cond1, store.GetItemById(item1),"sum", 0);
            store.AddSimplePurchaseCondition(cc1);
            bool res = store.EvaluatePurchasePolicy(store, basket);
            Assert.IsFalse(res);
        }
        #endregion
        
        #region Conditional condition - complex condition
        [TestMethod]
        public void ConditionalComplex_Success()
        {
            cond1 = store.AddCondition(store.GetItemById(item1), "min quantity", 1);
            cond2 = store.AddCondition(store.GetItemById(item1), "min value", 2);
            cond3 = store.BuildCondition(cond1, cond2, new AndOperator());
            ConditioningCondition cc1 = store.AddConditioning(cond1, store.GetItemById(item1),"sum", 3);
            store.AddSimplePurchaseCondition(cc1);
            bool res = store.EvaluatePurchasePolicy(store, basket);
            Assert.IsTrue(res);
        }
        [TestMethod]
        public void ConditionalComplex_Fail()
        {
            cond1 = store.AddCondition(store.GetItemById(item1), "max quantity", 1);
            cond2 = store.AddCondition(store.GetItemById(item1), "max value", 2);
            cond3 = store.BuildCondition(cond1, cond2, new AndOperator());
            ConditioningCondition cc1 = store.AddConditioning(cond1, store.GetItemById(item1),"sum", 1);
            store.AddSimplePurchaseCondition(cc1);
            bool res = store.EvaluatePurchasePolicy(store, basket);
            Assert.IsFalse(res);
        }
        #endregion
        
        #region Get condition
        [TestMethod]
        public void GetConditionSuccess()
        {
            cond1 = store.AddCondition(store.GetItemById(item1), "min quantity", 0);
            store.AddSimplePurchaseCondition(cond1);
            Assert.IsNotNull(store.GetCondition(cond1));
        }
        [TestMethod]
        public void GetConditionFail()
        {
            cond1 = store.AddCondition(store.GetItemById(item1), "min quantity", 1);
            store.AddSimplePurchaseCondition(cond1);
            cond2 = store.AddCondition(store.GetItemById(item1), "min quantity", 2);
            Assert.IsNull(store.GetCondition(cond2));
        }
        #endregion
        
        #region Remove condition
        [TestMethod]
        public void RemoveConditionSuccess()
        {
            cond1 = store.AddCondition(store.GetItemById(item1), "min quantity", 0);
            store.AddSimplePurchaseCondition(cond1);
            store.RemoveCondition(cond1);
            Assert.IsNull(store.GetCondition(cond1));
            Assert.IsNull(store.PurchasePolicy);
        }
        [TestMethod]
        public void RemoveConditionFail()
        {
            cond1 = store.AddCondition(store.GetItemById(item1), "min quantity", 0);
            cond2 = store.AddCondition(store.GetItemById(item1), "min quantity", 2);
            store.AddSimplePurchaseCondition(cond1);
            store.RemoveCondition(cond2);
            Assert.IsNotNull(store.GetCondition(cond1));
            Assert.IsNull(store.GetCondition(cond2));
            Assert.IsNotNull(store.PurchasePolicy);
        }
        
        [TestMethod]
        public void RemoveSimpleConditionSuccess()
        {
            cond1 = store.AddCondition(store.GetItemById(item1), "min quantity", 1);
            cond2 = store.AddCondition(store.GetItemById(item1), "min quantity", 2);
            store.AddSimplePurchaseCondition(cond1);
            store.AddSimplePurchaseCondition(cond2);
            Assert.IsNotNull(store.GetCondition(cond1));
            Assert.IsNotNull(store.GetCondition(cond2));
            store.RemoveCondition(cond2);
            Assert.IsNotNull(store.GetCondition(cond1));
            Assert.IsNull(store.GetCondition(cond2));
        }
        [TestMethod]
        public void RemoveSimpleConditionFail()
        {
            cond1 = store.AddCondition(store.GetItemById(item1), "min quantity", 1);
            cond2 = store.AddCondition(store.GetItemById(item1), "min quantity", 2);
            cond3 = store.AddCondition(store.GetItemById(item1), "min quantity", 3);
            store.AddSimplePurchaseCondition(cond1);
            store.AddSimplePurchaseCondition(cond2);
            Assert.IsNotNull(store.GetCondition(cond1));
            Assert.IsNotNull(store.GetCondition(cond2));
            store.RemoveCondition(cond3);
            Assert.IsNotNull(store.GetCondition(cond1));
            Assert.IsNotNull(store.GetCondition(cond2));
        }
        [TestMethod]
        public void RemoveSimpleConditionFailNull()
        {
            cond1 = store.AddCondition(store.GetItemById(item1), "min quantity", 1);
            cond2 = store.AddCondition(store.GetItemById(item1), "min quantity", 2);
            store.AddSimplePurchaseCondition(cond1);
            store.AddSimplePurchaseCondition(cond2);
            Assert.IsNotNull(store.GetCondition(cond1));
            Assert.IsNotNull(store.GetCondition(cond2));
            store.RemoveCondition(cond4);
            Assert.IsNotNull(store.GetCondition(cond1));
            Assert.IsNotNull(store.GetCondition(cond2));
        }
        [TestMethod]
        public void RemoveConditionalConditionSuccess()
        {
            cond1 = store.AddCondition(store.GetItemById(item1), "min quantity", 0);
            ConditioningCondition cc1 = store.AddConditioning(cond1, store.GetItemById(item1),"sum", 3);
            store.AddSimplePurchaseCondition(cc1);
            store.RemoveCondition(cc1);
            Assert.IsNull(store.GetCondition(cc1));
        }
        #endregion
    }
}