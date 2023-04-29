using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.DomainLayer.Store.DiscountPolicy;

namespace SadnaExpressTests.Unit_Tests
{
    [TestClass()]
    public class DiscountPolicyUT
    {
        private Store store;
        private Guid item1;
        private Guid item2;
        private Guid item3;
        private DiscountPolicy policy1; //20% on all the store
        private DiscountPolicy policy2; //50% on bisli
        private DiscountPolicy policy3; //50% on Food
        private DiscountPolicy policy4; //10% on ipad

        private Condition cond1; //
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
            policy1 = store.AddSimplePolicy(store, 20, new DateTime(2022, 4, 30), new DateTime(2024, 4, 30));
            policy2 = store.AddSimplePolicy(store.GetItemById(item1), 50, new DateTime(2022, 4, 30),
                new DateTime(2024, 4, 30));
            policy3 = store.AddSimplePolicy("Food", 50, new DateTime(2022, 4, 30),
                new DateTime(2024, 4, 30));
            policy4 = store.AddSimplePolicy(store.GetItemById(item3), 10, new DateTime(2022, 4, 30),
                new DateTime(2024, 4, 30));
            basket = new Dictionary<Item, int> {{store.GetItemById(item1), 1}, {store.GetItemById(item2), 1},
                {store.GetItemById(item3), 1}};
            cond1 = store.AddCondition(store, "min value", 50);
            cond2 = store.AddCondition(store.GetItemById(item3), "min quantity", 2);
            cond3 = store.AddCondition(store.GetItemById(item1), "min quantity", 1);
            cond4 = store.AddCondition("Food", "min value", 100);

        }
        #endregion
        
        #region Simple policy calculate
        [TestMethod]
        public void CalculatePolicyOnItemSuccess()
        {
            //Arrange
            store.AddToTree(policy2);
            //Act
            Dictionary<Item, KeyValuePair<double, DateTime>> items = store.DiscountPolicyTree.calculate(store, basket);
            //Assert
            Assert.AreEqual(5, items[store.GetItemById(item1)].Key); // changed
            Assert.IsFalse(items.ContainsKey(store.GetItemById(item2))); // not return 
            Assert.IsFalse(items.ContainsKey(store.GetItemById(item3))); //not return
        }
        
        [TestMethod]
        public void CalculatePolicyOnStoreSuccess()
        {
            //Arrange
            store.AddToTree(policy1);
            //Act
            Dictionary<Item, KeyValuePair<double, DateTime>> items = store.DiscountPolicyTree.calculate(store, basket);
            //Assert
            Assert.AreEqual(8, items[store.GetItemById(item1)].Key); // changed
            Assert.AreEqual(6.4, items[store.GetItemById(item2)].Key); // changed
            Assert.AreEqual(3200, items[store.GetItemById(item3)].Key); // changed
        }
        [TestMethod]
        public void CalculatePolicyOnCategorySuccess()
        {
            //Arrange
            store.AddToTree(policy3);
            //Act
            Dictionary<Item, KeyValuePair<double, DateTime>> items = store.DiscountPolicyTree.calculate(store, basket);
            //Assert
            Assert.AreEqual(5, items[store.GetItemById(item1)].Key); // changed
            Assert.AreEqual(4, items[store.GetItemById(item2)].Key); // changed
            Assert.IsFalse(items.ContainsKey(store.GetItemById(item3))); //not return
        }

        [TestMethod]
        public void EndDatePassNoDiscount()
        {
            //Arrange
            DiscountPolicy policyPass = store.AddSimplePolicy(store, 20, new DateTime(2022, 4, 30), new DateTime(2022, 5, 30));
            store.AddToTree(policyPass);
            //Act
            Dictionary<Item, KeyValuePair<double, DateTime>> items = store.DiscountPolicyTree.calculate(store, basket);
            //Assert
            Assert.IsNull(items); 
        }
        #endregion
        
        #region If condition Caculate

        [TestMethod]
        public void CalculateConditionalPolicySuccess()
        {
            //Arrange
            DiscountPolicy complex = store.AddComplexPolicy("if", cond1, policy1); //if I buy more than 50 I get 20% on the items
            store.AddToTree(complex);
            //Act
            Dictionary<Item, KeyValuePair<double, DateTime>> items = store.DiscountPolicyTree.calculate(store, basket);
            //Assert
            Assert.AreEqual(8, items[store.GetItemById(item1)].Key); // changed
            Assert.AreEqual(6.4, items[store.GetItemById(item2)].Key); // changed
            Assert.AreEqual(3200, items[store.GetItemById(item3)].Key); // changed
        }
        [TestMethod]
        public void CalculateConditionalPolicyFail()
        {
            //Arrange
            DiscountPolicy complex = store.AddComplexPolicy("if", cond2, policy1); //if I buy more than 2 from Ipad I get 20% on the items
            store.AddToTree(complex);
            //Act
            Dictionary<Item, KeyValuePair<double, DateTime>> items = store.DiscountPolicyTree.calculate(store, basket);
            //Assert
            Assert.IsNull(items); //the cond not pass
        }
        #endregion
        
        #region And condition Caculate
        [TestMethod]
        public void CalculateAndPolicyTwoOkSuccess()
        {
            //Arrange
            DiscountPolicy and = store.AddComplexPolicy("and", cond1, cond3, policy1); //if I buy more than 50 nis and more than one bisli I get 20% on the items
            store.AddToTree(and);
            //Act
            Dictionary<Item, KeyValuePair<double, DateTime>> items = store.DiscountPolicyTree.calculate(store, basket);
            //Assert
            Assert.AreEqual(8, items[store.GetItemById(item1)].Key); // changed
            Assert.AreEqual(6.4, items[store.GetItemById(item2)].Key); // changed
            Assert.AreEqual(3200, items[store.GetItemById(item3)].Key); // changed
        }
        [TestMethod]
        public void CalculateAndPolicyOneFail()
        {
            //Arrange
            DiscountPolicy and = store.AddComplexPolicy("and", cond1, cond2, policy1); //if I buy more than 50 nis and more than two Ipad I get 20% on the items
            store.AddToTree(and);
            //Act
            Dictionary<Item, KeyValuePair<double, DateTime>> items = store.DiscountPolicyTree.calculate(store, basket);
            //Assert
            Assert.IsNull(items);
        }
        [TestMethod]
        public void CalculateAndPolicyTwoFail()
        {
            //Arrange
            DiscountPolicy and = store.AddComplexPolicy("and", cond2, cond4, policy1);  //if I buy more than 100 nis and more than two Ipad I get 20% on the items
            store.AddToTree(and);
            //Act
            Dictionary<Item, KeyValuePair<double, DateTime>> items = store.DiscountPolicyTree.calculate(store, basket);
            //Assert
            Assert.IsNull(items);
        }
        #endregion
        
        #region Or condition Caculate
        [TestMethod]
        public void CalculateOrPolicyOneOkCondSuccess()
        {
            //Arrange
            DiscountPolicy or = store.AddComplexPolicy("or", cond4, cond3, policy1); //if I buy more than 100 nis food or more than one bisli I get 20% on the items
            store.AddToTree(or);
            //Act
            Dictionary<Item, KeyValuePair<double, DateTime>> items = store.DiscountPolicyTree.calculate(store, basket);
            //Assert
            Assert.AreEqual(8, items[store.GetItemById(item1)].Key); // changed
            Assert.AreEqual(6.4, items[store.GetItemById(item2)].Key); // changed
            Assert.AreEqual(3200, items[store.GetItemById(item3)].Key); // changed
        }
        [TestMethod]
        public void CalculateOrPolicyTwoOkCondSuccess()
        {
            //Arrange
            DiscountPolicy or = store.AddComplexPolicy("or", cond1, cond3, policy1); //if I buy more than 50 nis or more than one bisli I get 20% on the items
            store.AddToTree(or);
            //Act
            Dictionary<Item, KeyValuePair<double, DateTime>> items = store.DiscountPolicyTree.calculate(store, basket);
            //Assert
            Assert.AreEqual(8, items[store.GetItemById(item1)].Key); // changed
            Assert.AreEqual(6.4, items[store.GetItemById(item2)].Key); // changed
            Assert.AreEqual(3200, items[store.GetItemById(item3)].Key); // changed
        }
        [TestMethod]
        public void CalculateOrPolicyBothFail()
        {
            //Arrange
            DiscountPolicy or = store.AddComplexPolicy("or", cond2, cond4, policy1); //if I buy more than 100 nis Food and more than one Ipad I get 20% on the items
            store.AddToTree(or);
            //Act
            Dictionary<Item, KeyValuePair<double, DateTime>> items = store.DiscountPolicyTree.calculate(store, basket);
            //Assert
            Assert.IsNull(items);
        }
        #endregion
        
        #region Xor condition Caculate
        [TestMethod]
        public void CalculateXorPolicyOneOkCondSuccess()
        {
            //Arrange
            DiscountPolicy xor = store.AddComplexPolicy("xor",policy1, policy2); //get 20% on all item or 50% on bisli
            store.AddToTree(xor);
            //Act
            Dictionary<Item, KeyValuePair<double, DateTime>> items = store.DiscountPolicyTree.calculate(store, basket);
            //Assert
            //on all the items better
            Assert.AreEqual(8, items[store.GetItemById(item1)].Key); // changed
            Assert.AreEqual(6.4, items[store.GetItemById(item2)].Key); // changed
            Assert.AreEqual(3200, items[store.GetItemById(item3)].Key); // changed
        }
        [TestMethod]
        public void CalculateXorPolicyOnItemFail()
        {
            //Arrange
            DiscountPolicy or = store.AddComplexPolicy("xor", cond2, cond4, policy1); //if I buy more than 100 nis Food and more than one Ipad I get 20% on the items
            store.AddToTree(or);
            //Act
            Dictionary<Item, KeyValuePair<double, DateTime>> items = store.DiscountPolicyTree.calculate(store, basket);
            //Assert
            Assert.IsNull(items);
        }
        #endregion
        
        #region Clean Up
        [TestCleanup]
        public void CleanUp()
        {
            store.DiscountPolicyTree = null;
        }
        #endregion
    }
}