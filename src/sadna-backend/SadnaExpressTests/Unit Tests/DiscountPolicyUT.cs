using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.DomainLayer.Store.Policy;

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
        private DiscountPolicy policy4; //30% on ipad

        private Condition cond1; // min value 50 nis
        private Condition cond2; // min quantity of ipad 2
        private Condition cond3; // min  quantity of bisli 1
        private Condition cond4; // min value food 100 nis
        private Dictionary<Item, int> basket;

        #region SetUp
        [TestInitialize]
        public void SetUp()
        {
            store = new Store("Hello");
            item1 = store.AddItem("Bisli", "Food", 10.0, 2);
            item2 = store.AddItem("Bamba", "Food", 8.0, 2);
            item3 = store.AddItem("Ipad", "electronic", 4000, 2);
            policy1 = store.CreateSimplePolicy("Store", 20, new DateTime(2022, 4, 30), new DateTime(2024, 4, 30));
            policy2 = store.CreateSimplePolicy("ItemBisli", 50, new DateTime(2022, 4, 30),
                new DateTime(2024, 4, 30));
            policy3 = store.CreateSimplePolicy("CategoryFood", 50, new DateTime(2022, 4, 30),
                new DateTime(2024, 4, 30));
            policy4 = store.CreateSimplePolicy("ItemIpad", 30, new DateTime(2022, 4, 30),
                new DateTime(2024, 4, 30));
            basket = new Dictionary<Item, int> {{store.GetItemById(item1), 1}, {store.GetItemById(item2), 1},
                {store.GetItemById(item3), 1}};
            cond1 = store.AddCondition("Store","", "min value", 50);
            cond2 = store.AddCondition("Item", "Ipad", "min quantity", 2);
            cond3 = store.AddCondition("Item","Bisli", "min quantity", 1);
            cond4 = store.AddCondition("Category","Food", "min value", 100);

        }
        #endregion

        #region Simple policy calculate

        [TestMethod]
        public void CalculatePolicyOnItemSuccess()
        {
            //Arrange
            store.AddPolicy(policy2.ID);
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
            store.AddPolicy(policy1.ID);
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
            store.AddPolicy(policy3.ID);
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
            DiscountPolicy policyPass =
                store.CreateSimplePolicy("Store", 20, new DateTime(2022, 4, 30), new DateTime(2022, 5, 30));
            store.AddPolicy(policyPass.ID);
            //Act
            Dictionary<Item, KeyValuePair<double, DateTime>> items = store.DiscountPolicyTree.calculate(store, basket);
            //Assert
            Assert.AreEqual(0, items.Count);
        }

        #endregion

        #region If condition Caculate

        [TestMethod]
        public void CalculateConditionalPolicySuccess()
        {
            //Arrange
            DiscountPolicy
                complex = store.CreateComplexPolicy("if", cond1.ID,
                    policy1.ID); //if I buy more than 50 I get 20% on the items
            store.AddPolicy(complex.ID);
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
            DiscountPolicy
                complex = store.CreateComplexPolicy("if", cond2.ID,
                    policy1.ID); //if I buy more than 2 from Ipad I get 20% on the items
            store.AddPolicy(complex.ID);
            //Act
            Dictionary<Item, KeyValuePair<double, DateTime>> items = store.DiscountPolicyTree.calculate(store, basket);
            //Assert
            Assert.AreEqual(0, items.Count); //the cond not pass
        }

        #endregion

        #region And condition Caculate

        [TestMethod]
        public void CalculateAndPolicyTwoOkSuccess()
        {
            //Arrange
            DiscountPolicy
                and = store.CreateComplexPolicy("and", cond1.ID, cond3.ID,
                    policy1.ID); //if I buy more than 50 nis and more than one bisli I get 20% on the items
            store.AddPolicy(and.ID);
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
            DiscountPolicy
                and = store.CreateComplexPolicy("and", cond1.ID, cond2.ID,
                    policy1.ID); //if I buy more than 50 nis and more than two Ipad I get 20% on the items
            store.AddPolicy(and.ID);
            //Act
            Dictionary<Item, KeyValuePair<double, DateTime>> items = store.DiscountPolicyTree.calculate(store, basket);
            //Assert
            Assert.AreEqual(0, items.Count);
        }

        [TestMethod]
        public void CalculateAndPolicyTwoFail()
        {
            //Arrange
            DiscountPolicy
                and = store.CreateComplexPolicy("and", cond2.ID, cond4.ID,
                    policy1.ID); //if I buy more than 100 nis and more than two Ipad I get 20% on the items
            store.AddPolicy(and.ID);
            //Act
            Dictionary<Item, KeyValuePair<double, DateTime>> items = store.DiscountPolicyTree.calculate(store, basket);
            //Assert
            Assert.AreEqual(0, items.Count);
        }

        #endregion

        #region Or condition Caculate

        [TestMethod]
        public void CalculateOrPolicyOneOkCondSuccess()
        {
            //Arrange
            DiscountPolicy
                or = store.CreateComplexPolicy("or", cond4.ID, cond3.ID,
                    policy1.ID); //if I buy more than 100 nis food or more than one bisli I get 20% on the items
            store.AddPolicy(or.ID);
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
            DiscountPolicy
                or = store.CreateComplexPolicy("or", cond1.ID, cond3.ID,
                    policy1.ID); //if I buy more than 50 nis or more than one bisli I get 20% on the items
            store.AddPolicy(or.ID);
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
            DiscountPolicy
                or = store.CreateComplexPolicy("or", cond2.ID, cond4.ID,
                    policy1.ID); //if I buy more than 100 nis Food and more than one Ipad I get 20% on the items
            store.AddPolicy(or.ID);
            //Act
            Dictionary<Item, KeyValuePair<double, DateTime>> items = store.DiscountPolicyTree.calculate(store, basket);
            //Assert
            Assert.AreEqual(0, items.Count);
        }

        #endregion

        #region Xor condition Caculate

        [TestMethod]
        public void CalculateXorPolicyTwoOkFail()
        {
            //Arrange
            DiscountPolicy
                xor = store.CreateComplexPolicy("xor", cond1.ID, cond3.ID,
                    policy1.ID); //if I buy more than 50 nis xor more than one bisli I get 20% on the items
            store.AddPolicy(xor.ID);
            //Act
            Dictionary<Item, KeyValuePair<double, DateTime>> items = store.DiscountPolicyTree.calculate(store, basket);
            //Assert
            Assert.AreEqual(0, items.Count);
        }

        [TestMethod]
        public void CalculateXorPolicyOneSuccess()
        {
            //Arrange
            DiscountPolicy
                xor = store.CreateComplexPolicy("xor", cond1.ID, cond2.ID,
                    policy1.ID); //if I buy more than 50 nis xor more than two Ipad I get 20% on the items
            store.AddPolicy(xor.ID);
            //Act
            Dictionary<Item, KeyValuePair<double, DateTime>> items = store.DiscountPolicyTree.calculate(store, basket);
            //Assert
            Assert.AreEqual(8, items[store.GetItemById(item1)].Key); // changed
            Assert.AreEqual(6.4, items[store.GetItemById(item2)].Key); // changed
            Assert.AreEqual(3200, items[store.GetItemById(item3)].Key); // changed
        }

        [TestMethod]
        public void CalculateXorPolicyTwoFail()
        {
            //Arrange
            DiscountPolicy
                xor = store.CreateComplexPolicy("xor", cond2.ID, cond4.ID,
                    policy1.ID); //if I buy more than 100 nis xor more than two Ipad I get 20% on the items
            store.AddPolicy(xor.ID);
            //Act
            Dictionary<Item, KeyValuePair<double, DateTime>> items = store.DiscountPolicyTree.calculate(store, basket);
            //Assert
            Assert.AreEqual(0, items.Count);
        }

        #endregion

        #region Max condition Caculate

        [TestMethod]
        public void CalculateMaxPolicy1BetterSuccess()
        {
            //Arrange
            DiscountPolicy max = store.CreateComplexPolicy("max", policy1.ID, policy2.ID);
            store.AddPolicy(max.ID);
            //Act
            Dictionary<Item, KeyValuePair<double, DateTime>> items = store.DiscountPolicyTree.calculate(store, basket);
            //Assert
            //on all the items better
            Assert.AreEqual(8, items[store.GetItemById(item1)].Key); // changed
            Assert.AreEqual(6.4, items[store.GetItemById(item2)].Key); // changed
            Assert.AreEqual(3200, items[store.GetItemById(item3)].Key); // changed
        }

        [TestMethod]
        public void CalculateMaxPolicy3BetterSuccess()
        {
            //Arrange
            DiscountPolicy max = store.CreateComplexPolicy("max", policy1.ID, policy4.ID);
            store.AddPolicy(max.ID);
            //Act
            Dictionary<Item, KeyValuePair<double, DateTime>> items = store.DiscountPolicyTree.calculate(store, basket);
            //Assert
            Assert.AreEqual(2800, items[store.GetItemById(item3)].Key); // changed
            Assert.IsFalse(items.ContainsKey(store.GetItemById(item1))); //not return
            Assert.IsFalse(items.ContainsKey(store.GetItemById(item2))); //not return

        }

        #endregion

        #region Add condition Caculate

        [TestMethod]
        public void CalculateAddPolicySuccess()
        {
            //Arrange
            DiscountPolicy
                add = store.CreateComplexPolicy("add", policy1.ID, policy3.ID); //20% on store and 50% on food 
            store.AddPolicy(add.ID);
            //Act
            Dictionary<Item, KeyValuePair<double, DateTime>> items = store.DiscountPolicyTree.calculate(store, basket);
            //Assert
            Assert.AreEqual(3, items[store.GetItemById(item1)].Key); // changed
            Assert.AreEqual(2.4, items[store.GetItemById(item2)].Key); // changed
            Assert.AreEqual(3200, items[store.GetItemById(item3)].Key); // changed
        }

        #endregion

        #region All Kind condition Caculate

        [TestMethod]
        public void CalculateLongXorAndPolicy()
        {
            //Arrange
            DiscountPolicy
                xor = store.CreateComplexPolicy("xor", cond1.ID, cond2.ID,
                    policy1.ID); //if I buy more than 50 nis xor more than two ipad I get 20% on the items
            DiscountPolicy
                and = store.CreateComplexPolicy("and", cond1.ID, cond3.ID,
                    policy2.ID); //if I buy more than 50 nis and more than one bisli I get 50% on the bisli
            store.AddPolicy(xor.ID);
            store.AddPolicy(and.ID);
            //Act
            Dictionary<Item, KeyValuePair<double, DateTime>> items = store.DiscountPolicyTree.calculate(store, basket);
            //Assert
            Assert.AreEqual(5, items[store.GetItemById(item1)].Key); // changed
            Assert.AreEqual(6.4, items[store.GetItemById(item2)].Key); // changed
            Assert.AreEqual(3200, items[store.GetItemById(item3)].Key); // changed
        }

        [TestMethod]
        public void CalculateLongAddAndPolicy()
        {
            //Arrange
            DiscountPolicy
                add = store.CreateComplexPolicy("add", policy1.ID, policy2.ID); //I have 70% on bisli and the rest 20%
            DiscountPolicy
                xor = store.CreateComplexPolicy("xor", cond1.ID, cond2.ID,
                    policy3.ID); //if I buy more than 50 nis xor more than two ipad I get 50% on the "Food"
            DiscountPolicy
                and = store.CreateComplexPolicy("and", cond1.ID, cond3.ID,
                    xor.ID); //if I buy more than 50 nis and more than one bisli
            store.AddPolicy(add.ID);
            store.AddPolicy(and.ID);
            //Act
            Dictionary<Item, KeyValuePair<double, DateTime>> items = store.DiscountPolicyTree.calculate(store, basket);
            //Assert
            Assert.AreEqual(3, items[store.GetItemById(item1)].Key); // changed
            Assert.AreEqual(4, items[store.GetItemById(item2)].Key); // changed
            Assert.AreEqual(3200, items[store.GetItemById(item3)].Key); // changed
        }

        [TestMethod]
        public void CalculateTwoNotCommonPolicys()
        {
            //Arrange
            DiscountPolicy
                add = store.CreateComplexPolicy("add", policy1.ID, policy2.ID); //I have 70% on bisli and the rest 20%

            store.AddPolicy(policy4.ID);
            store.AddPolicy(add.ID);
            //Act
            Dictionary<Item, KeyValuePair<double, DateTime>> items = store.DiscountPolicyTree.calculate(store, basket);
            //Assert
            Assert.AreEqual(3, items[store.GetItemById(item1)].Key); // changed
            Assert.AreEqual(6.4, items[store.GetItemById(item2)].Key); // changed
            Assert.AreEqual(2800, items[store.GetItemById(item3)].Key); // changed
        }

        [TestMethod]
        public void CalculateOneCondNotPolicys()
        {
            //Arrange
            DiscountPolicy
                and = store.CreateComplexPolicy("and", cond1.ID, cond2.ID,
                    policy2.ID); //I have 70% on bisli and the rest 20%
            store.AddPolicy(policy4.ID);
            store.AddPolicy(and.ID);

            //Act
            Dictionary<Item, KeyValuePair<double, DateTime>> items = store.DiscountPolicyTree.calculate(store, basket);
            //Assert
            Assert.IsFalse(items.ContainsKey(store.GetItemById(item1))); //not return
            Assert.IsFalse(items.ContainsKey(store.GetItemById(item2))); //not return
            Assert.AreEqual(2800, items[store.GetItemById(item3)].Key); // changed
        }

        #endregion

        #region Add the same twice

        [TestMethod]
        public void AddTheSameSimplePolicyFail()
        {
            //Arrange
            store.CreateSimplePolicy("ItemIpad", 30, new DateTime(2022, 4, 30), new DateTime(2025, 4, 30));
            Assert.ThrowsException<Exception>(() =>
                store.CreateSimplePolicy("ItemIpad", 30, new DateTime(2022, 4, 30), new DateTime(2025, 4, 30)));
        }

        [TestMethod]
        public void AddTheAndPolicyFail()
        {
            //Arrange
            DiscountPolicy
                and = store.CreateComplexPolicy("and", cond1.ID, cond2.ID,
                    policy2.ID); //I have 70% on bisli and the rest 20%
            Assert.ThrowsException<Exception>(() =>
                store.CreateComplexPolicy("and", cond1.ID, cond2.ID, policy2.ID));
        }
        #endregion
        
        #region Remove
        [TestMethod]
        public void RemoveSimplePolicy()
        {
            //Arrange
            store.AddPolicy(policy1.ID);
            Assert.IsTrue(store.AllDiscountPolicies.ContainsKey(policy1));
            int prePolicy = store.AllDiscountPolicies.Count;
            //Act
            store.RemovePolicy(policy1.ID, "Policy");
            //Assert
            Assert.AreEqual(prePolicy - 1, store.AllDiscountPolicies.Count);
            Assert.IsFalse(store.AllDiscountPolicies.ContainsKey(policy1));
        }

        [TestMethod]
        public void RemoveComplexPolicy()
        {
            //Arrange
            DiscountPolicy
                xor = store.CreateComplexPolicy("xor", cond1.ID, cond2.ID,
                    policy1.ID); //if I buy more than 50 nis xor more than two ipad I get 20% on the items
            store.AddPolicy(xor.ID);
            Assert.IsTrue(store.AllDiscountPolicies.ContainsKey(xor));
            int prePolicy = store.AllDiscountPolicies.Count;
            //Act
            store.RemovePolicy(xor.ID, "Policy");
            //Assert
            Assert.AreEqual(prePolicy - 1, store.AllDiscountPolicies.Count);
            Assert.IsFalse(store.AllDiscountPolicies.ContainsKey(xor));
        }
        [TestMethod]
        public void RemoveConditionInComplexPolicy()
        {
            //Arrange
            DiscountPolicy xor = store.CreateComplexPolicy("xor", cond1.ID, cond2.ID,
                    policy1.ID); //if I buy more than 50 nis xor more than two ipad I get 20% on the items
            store.AddPolicy(xor.ID);
            Assert.IsTrue(store.AllDiscountPolicies.ContainsKey(xor));
            int prePolicy = store.AllDiscountPolicies.Count;
            Assert.IsTrue(store.CondDiscountPolicies.ContainsKey(cond1));
            int preCond = store.CondDiscountPolicies.Count;
            //Act
            store.RemovePolicy(cond1.ID, "Condition");
            //Assert
            Assert.AreEqual(prePolicy, store.AllDiscountPolicies.Count);
            Assert.IsTrue(store.AllDiscountPolicies.ContainsKey(xor));
            Assert.AreEqual(preCond-1, store.CondDiscountPolicies.Count);
            Assert.IsFalse(store.CondDiscountPolicies.ContainsKey(cond1));
        }
        #endregion

        #region Clean Up

        [TestCleanup]
        public void CleanUp()
        {
            store.CondDiscountPolicies = null;
            store.AllDiscountPolicies = null;
            store.DiscountPolicyTree = null;
        }

        #endregion
    }

}