using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using Microsoft.Owin.Hosting;
using SadnaExpress.API;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.DomainLayer.User;
using SadnaExpress.ServiceLayer;
using SadnaExpress.Services;
using SadnaExpress.API.SignalR;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using System.Configuration;
using ConfigurationManager = System.Configuration.ConfigurationManager;
using System.Data.SqlTypes;
using SadnaExpress.DataLayer;using System.Collections.Concurrent;


namespace SadnaExpress
{
    class State
    {
        private static State instance = null;
        public static State Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new State();
                }
                return instance;
                
            }
        }

        public class Params
        {
            public string email { get; set; }
            public string MemberEmail { get; set; }

            public string MemberId { get; set; }

            public string firstName { get; set; }
            public string lastName { get; set; }
            public string password { get; set; }

            public string StoreName { get; set; }
            public string StoreID { get; set; }

            public string itemName { get; set; }
            public string itemCategory { get; set; }
            public double itemPrice { get; set; }
            public int itemQuantity { get; set; }

            public string Permission { get; set; }

        }

        public class FunctionParams
        {
            public string function { get; set; }
            public Params @params { get; set; }
        }

        public void stateFile(String fileName)
        {
            DBHandler.Instance.CleanDB();
            Dictionary<String, Member> members = new Dictionary<string, Member>();
            Dictionary<String, Store> stores = new Dictionary<string, Store>();
            Guid userId;
            TradingSystem trading = new TradingSystem();
            trading.SetIsSystemInitialize(true);
            trading.TestMode = true;
            string email = "";

            string path = Path.Combine(Environment.CurrentDirectory, Path.Combine("..\\..",fileName));
            string json = File.ReadAllText(path);
            List<FunctionParams> functionParamsList = JsonConvert.DeserializeObject<List<FunctionParams>>(json);
            foreach (FunctionParams functionParams in functionParamsList)
            {
                string function = functionParams.function;
                Params parameters = functionParams.@params;
                switch (function)
                {
                    case "Register":
                        userId = trading.Enter().Value;
                        email = parameters.email;
                        string firstName = parameters.firstName;
                        string lastName = parameters.lastName;
                        string password = parameters.password;
                        trading.Register(userId, email, firstName, lastName, password);
                        Member member = trading.GetMember(email).Value;
                        members.Add(email, member);
                        break;

                    case "CreateSystemManager":
                        email = parameters.email;
                        userId = members[email].UserId;
                        PromotedMember promotedMember = trading.GetMember(email).Value.promoteToMember();
                        promotedMember.createSystemManager();
                        break;

                    case "Login":
                        userId = trading.Enter().Value;
                        email = parameters.email;
                        password = parameters.password;
                        trading.Login(userId, email, password);

                        break;

                    case "Logout":
                        email = parameters.email;
                        userId = members[email].UserId;
                        trading.Logout(userId);
                        break;

                    case "OpenNewStore":
                        String StoreName = parameters.StoreName;
                        email = parameters.email;
                        userId = members[email].UserId;
                        trading.OpenNewStore(userId, StoreName);
                        Store store = trading.GetStore(StoreName).Value;
                        stores.Add(StoreName, store);
                        break;

                    case "AppointStoreOwner":
                        email = parameters.email;
                        String anotherEmail = parameters.MemberEmail;
                        StoreName = parameters.StoreName;
                        userId = members[email].UserId;
                        anotherEmail = members[anotherEmail].Email;
                        Guid storeID = stores[StoreName].StoreID;
                        trading.AppointStoreOwner(userId, storeID, anotherEmail);
                        break;

                    case "AppointStoreManager":
                        email = parameters.email;
                        anotherEmail = parameters.MemberEmail;
                        StoreName = parameters.StoreName;
                        userId = members[email].UserId;
                        anotherEmail = members[anotherEmail].Email;
                        storeID = stores[StoreName].StoreID;
                        trading.AppointStoreManager(userId, storeID, anotherEmail);
                        trading.AddStoreManagerPermissions(userId, storeID, anotherEmail, parameters.Permission);
                        break;

                    case "AddItemToStore":
                        email = parameters.email;
                        StoreName = parameters.StoreName;
                        storeID = stores[StoreName].StoreID;
                        userId = members[email].UserId;
                        String itemName = parameters.itemName;
                        String itemCategory = parameters.itemCategory;
                        double itemPrice = parameters.itemPrice;
                        int quantity = parameters.itemQuantity;
                        trading.AddItemToStore(userId, storeID, itemName, itemCategory, itemPrice,
                            quantity);
                        break;

                }
            }
        }

        public void checkFile0()
        {
            TradingSystem trading = TradingSystem.Instance;
            trading.SetIsSystemInitialize(true);
            trading.TestMode = true;
            ConcurrentDictionary<Guid, Member> members = DBHandler.Instance.GetAllMembers();

            // 3 members register
            if (members.Count != 3)
            {
                throw new Exception("There is a user that was not initilized");
            }
            List<Member> list = new List<Member>(members.Values);


            // open store1
            ConcurrentDictionary<Guid, Store> stores = DBHandler.Instance.GetAllStores();
            List<Store> storesList = new List<Store>(stores.Values);
            if (storesList[0] == null)
            {
                throw new Exception("The store was not created");

            }
            Guid storeID = storesList[0].StoreID;

            List<String> permissions = new List<string>();
            permissions.Add("owner permissions");
            foreach (Member member in list)
            {
                // user2 has owner permissions
                if (member.Email.Equals("user2@gmail.com"))
                {
                    if (!((PromotedMember)member).hasPermissions(storeID, permissions))
                    {
                        throw new Exception("The user does not have the correct permissions");
                    }
                }
                // user3 has owner permissions
                if (member.Email.Equals("user3@gmail.com"))
                {
                    if (!((PromotedMember)member).hasPermissions(storeID, permissions))
                    {
                        throw new Exception("The user does not have the correct permissions");
                    }
                }


            }
        }

        public void checkFile1()
        {
            TradingSystem trading = TradingSystem.Instance;
            trading.SetIsSystemInitialize(true);
            trading.TestMode = true;
            ConcurrentDictionary<Guid, Member> members = DBHandler.Instance.GetAllMembers();

            // 6 members register
            if (members.Count != 6)
            {
                throw new Exception("There is a user that was not initilized");
            }
            List<Member> list = new List<Member>(members.Values);

            // open store1
            ConcurrentDictionary<Guid, Store> stores = DBHandler.Instance.GetAllStores();
            List<Store> storesList = new List<Store>(stores.Values);
            if (storesList[0] == null)
            {
                throw new Exception("The store was not created");

            }
            Guid storeID = storesList[0].StoreID;
            List<Item> items = trading.GetItemsInStore(list[1].UserId, storeID).Value;

            // item in store1
            if (items[0] == null)
            {
                throw new Exception("The store does not have items");
            }
            List<String> permissionsOwner = new List<string>();
            List<String> permissionsManager = new List<string>();

            permissionsOwner.Add("owner permissions");
            permissionsManager.Add("product management permissions");
            foreach (Member member in list)
            {
                // user3 has managment permissions
                if (member.Email.Equals("user3@gmail.com"))
                {
                    if (!((PromotedMember)member).hasPermissions(storeID, permissionsManager))
                    {
                        throw new Exception("The user does not have the correct permissions");
                    }
                }
                // user4 or user5 has owner permissions

                if (member.Email.Equals("user4@gmail.com") || member.Email.Equals("user5@gmail.com"))
                {
                    if (!((PromotedMember)member).hasPermissions(storeID, permissionsOwner))
                    {
                        throw new Exception("The user does not have the correct permissions");
                    }
                }
              
            }
           
        }

    }
}
