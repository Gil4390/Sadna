﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaExpress.ServiceLayer;
using System;

using SadnaExpressTests;
using static SadnaExpressTests.Mocks;
using SadnaExpress.DomainLayer.User;
using System.Collections.Concurrent;
using SadnaExpress.DomainLayer.Store;

namespace SadnaExpressTests.Acceptance_Tests
{
    [TestClass()]
    public class TradingSystemAT
    {
        protected ProxyBridge proxyBridge;
        protected Guid userid;
        protected Guid systemManagerid;
        protected IPasswordHash passwordHash;
        [TestInitialize]
        public virtual void SetUp()
        {
            proxyBridge = new ProxyBridge();
            passwordHash = new PasswordHash();
            IStoreFacade storeFacade = new StoreFacade();
            ConcurrentDictionary<Guid, User> current_users = new ConcurrentDictionary<Guid, User>();
            User entered_user = new User();
            userid = entered_user.UserId;
            current_users.TryAdd(userid, entered_user);
            ConcurrentDictionary<Guid, Member> members = new ConcurrentDictionary<Guid, Member>();
            systemManagerid = Guid.NewGuid();
            PromotedMember systemManager = new PromotedMember(systemManagerid, "RotemSela@gmail.com", "noga", "schwartz", passwordHash.Hash("AS87654askj"));
            systemManager.createSystemManager();
            members.TryAdd(systemManagerid, systemManager);
            IUserFacade _userFacade = new UserFacade(current_users, members, new PasswordHash(), new Mock_PaymentService(), new Mock_SupplierService());
            TradingSystem Ts=new TradingSystem(_userFacade, storeFacade);
            Ts.TestMode = true;

            proxyBridge.SetBridge(Ts);
            proxyBridge.SetPaymentService(new Mock_PaymentService());
            proxyBridge.SetSupplierService(new Mock_SupplierService());
            proxyBridge.SetIsSystemInitialize(true);
        }


        private class Mock_Bad_SupplierService : Mock_SupplierService
        {
            // bad connection
            public override bool Connect()
            {
                return false;
            }

        }

        private class Mock_Bad_PaymentService : Mock_PaymentService
        {
            // bad connection
            public override bool Connect()
            {
                return false;
            }

        }

        [TestCleanup]
        public virtual void CleanUp()
        {
            proxyBridge.CleanUp();
        }

    }
}
