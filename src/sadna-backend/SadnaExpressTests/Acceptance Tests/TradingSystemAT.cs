using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaExpress.ServiceLayer;
using System;

using SadnaExpressTests;
using static SadnaExpressTests.Mocks;
using SadnaExpress.DomainLayer.User;
using System.Collections.Concurrent;
using SadnaExpress.API.SignalR;
using SadnaExpress.DomainLayer;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.DomainLayer.Store.Policy;

namespace SadnaExpressTests.Acceptance_Tests
{
    [TestClass()]
    public class TradingSystemAT
    {
        protected ProxyBridge proxyBridge;
        protected Guid userid;
        protected Guid memberId;
        protected Guid memberId2;
        protected Guid memberId3;
        protected Guid memberId4;

        protected Guid systemManagerid;
        protected Guid storeOwnerid;
        protected Guid storeid1;
        protected Guid itemid1;
        protected Guid itemid11;
        protected Guid itemid22;
        protected Guid itemid23;
        protected Guid storeid2;
        protected Guid itemid2;
        protected Guid itemNoStock;
        protected ConcurrentDictionary<Guid, Store> stores;
        protected IPasswordHash passwordHash;
        protected PromotedMember storeOwner;
        protected DiscountPolicy policy3;
        [TestInitialize]
        public virtual void SetUp()
        {
            proxyBridge = new ProxyBridge();
            passwordHash = new PasswordHash();
            NotificationNotifier.GetInstance().TestMood = true;
            stores = new ConcurrentDictionary<Guid, Store>();
            Store store1 = new Store("Zara");
            storeid1 = store1.StoreID;
            itemid1 = store1.AddItem("Tshirt", "clothes", 99.8, 40);
            itemid22 = store1.AddItem("Ipad", "electronic", 99.8, 2);
            itemid23 = store1.AddItem("Cup", "Kitchen", 19.8, 1);

            itemid11 = store1.AddItem("Dress", "clothes", 70, 45);
            Store store2 = new Store("Fox");
            storeid2 = store2.StoreID;
            itemid2 = store2.AddItem("Pants", "clothes", 150, 200);
            
            store2.AddItem("Towel", "Home", 40, 450);
            store2.AddItem("Teddy bear toy", "children toys", 65, 120);
            itemNoStock = store2.AddItem("mouse", "animals", 65, 0);
            
            stores.TryAdd(store1.StoreID, store1);
            stores.TryAdd(store2.StoreID, store2);

            IStoreFacade storeFacade = new StoreFacade(stores);
            ConcurrentDictionary<Guid, User> current_users = new ConcurrentDictionary<Guid, User>();
            User entered_user = new User();
            userid = entered_user.UserId;
            current_users.TryAdd(userid, entered_user);

            ConcurrentDictionary<Guid, Member> members = new ConcurrentDictionary<Guid, Member>();
            ConcurrentDictionary<Guid, string> macs = new ConcurrentDictionary<Guid, string>();
            systemManagerid = Guid.NewGuid();
            memberId = Guid.NewGuid();
            memberId2 = Guid.NewGuid();
            memberId3 = Guid.NewGuid();
            memberId4 = Guid.NewGuid();

            storeOwnerid = Guid.NewGuid();
            string newMac = passwordHash.Mac();
            macs.TryAdd(memberId, newMac);
            Member member = new Member(memberId, "gil@gmail.com", "Gil", "Gil", passwordHash.Hash("asASD876!@"+newMac));
            newMac = passwordHash.Mac();
            macs.TryAdd(memberId2, newMac);
            Member member2 = new Member(memberId2, "sebatian@gmail.com", "Sebatian", "Sebatian", passwordHash.Hash("asASD123!@"+newMac));
            newMac = passwordHash.Mac();
            macs.TryAdd(memberId3, newMac);
            Member member3 = new Member(memberId3, "amihai@gmail.com", "Amihai", "Amihai", passwordHash.Hash("asASD753!@"+newMac));
            newMac = passwordHash.Mac();
            macs.TryAdd(memberId4, newMac);
            Member member4 = new Member(memberId4, "bar@gmail.com", "Bar", "Bar", passwordHash.Hash("asASD159!@"+newMac));


            newMac = passwordHash.Mac();
            macs.TryAdd(systemManagerid, newMac);
            PromotedMember systemManager = new PromotedMember(systemManagerid, "RotemSela@gmail.com", "noga", "schwartz", passwordHash.Hash("AS87654askj"+newMac));
            newMac = passwordHash.Mac();
            macs.TryAdd(storeOwnerid, newMac);
            storeOwner = new PromotedMember(storeOwnerid, "AsiAzar@gmail.com", "shay", "kres", passwordHash.Hash("A#!a12345678"+newMac));
            systemManager.createSystemManager();
            storeOwner.createFounder(storeid1);
            members.TryAdd(systemManagerid, systemManager);
            members.TryAdd(memberId, member);
            members.TryAdd(memberId2, member2);
            members.TryAdd(memberId3, member3);
            members.TryAdd(memberId4, member4);

            members.TryAdd(storeOwnerid, storeOwner);
            IUserFacade _userFacade = new UserFacade(current_users, members, macs,new PasswordHash(), new Mock_PaymentService(), new Mock_SupplierService());
            TradingSystem Ts = new TradingSystem(_userFacade, storeFacade);
            
            Ts.TestMode = true;

            proxyBridge.SetBridge(Ts);
            proxyBridge.SetPaymentService(new Mock_PaymentService());
            proxyBridge.SetSupplierService(new Mock_SupplierService());
            proxyBridge.SetIsSystemInitialize(true);
            
            DiscountPolicy policy1 = store1.CreateSimplePolicy("Store", 50, DateTime.Now, new DateTime(2024, 5, 20));
            Condition cond3 = store1.AddCondition("Item","Tshirt", "min quantity", 1);
            DiscountPolicy policy2 = store1.CreateComplexPolicy("if", cond3.ID, policy1.ID);

            policy3 = store1.CreateSimplePolicy("Store", 10, DateTime.Now, new DateTime(2024, 5, 20));

            store1.AddPolicy(policy2.ID);
            store1.AddPolicy(policy3.ID);
        }


        protected class Mock_Bad_SupplierService : Mock_SupplierService
        {
            // bad connection
            public override bool Handshake()
            {
                return false;
            }

        }

        protected class Mock_Bad_PaymentService : Mock_PaymentService
        {
            // bad connection
            public override string Handshake()
            {
                return "Not OK";
            }

        }

        [TestCleanup]
        public virtual void CleanUp()
        {
            proxyBridge.CleanUp();
        }

    }
}
