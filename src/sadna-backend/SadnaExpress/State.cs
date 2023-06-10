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

            public string ItemName { get; set; }
            public string ItemCategory { get; set; }
            public string ItemPrice { get; set; }
            public string ItemQuantity { get; set; }

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
            int expectedParamsCount = 0;
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
                        if (string.IsNullOrEmpty(email))
                        {
                           throw new Exception("The email field is null in function " + (function));
                        }
                        string firstName = parameters.firstName;
                        if (string.IsNullOrEmpty(firstName))
                        {
                            throw new Exception("The firstName field is null in function " + (function));
                        }
                        string lastName = parameters.lastName;
                        if (string.IsNullOrEmpty(lastName))
                        {
                            throw new Exception("The lastName field is null in function " + (function));
                        }
                        string password = parameters.password;
                        if (string.IsNullOrEmpty(password))
                        {
                            throw new Exception("The password field is null in function " + (function));
                        }
                        Response res = trading.Register(userId, email, firstName, lastName, password);
                        if (res.ErrorOccured)
                            throw new Exception(res.ErrorMessage);
                        Member member = trading.GetMember(email).Value;
                        members.Add(email, member);
                        expectedParamsCount = 4;
                        checkExpectedParams(expectedParamsCount, parameters, function);
                        break;

                    case "CreateSystemManager":
                        expectedParamsCount = 1;
                        checkExpectedParams(expectedParamsCount, parameters, function);
                        email = parameters.email;
                        if (string.IsNullOrEmpty(email))
                        {
                            throw new Exception("The email field is null in function " + (function));
                        }
                        userId = members[email].UserId;
                        PromotedMember promotedMember = trading.GetMember(email).Value.promoteToMember();
                        if (promotedMember == null)
                            throw new Exception("unable to promoted this member");
                        promotedMember.createSystemManager();
                        break;

                    case "Login":
                        expectedParamsCount = 2;
                        checkExpectedParams(expectedParamsCount, parameters, function);
                        userId = trading.Enter().Value;
                        email = parameters.email;
                        if (string.IsNullOrEmpty(email))
                        {
                            throw new Exception("The email field is null in function " + (function));
                        }
                        password = parameters.password;
                        if (string.IsNullOrEmpty(password))
                        {
                            throw new Exception("The password field is null in function " + (function));
                        }
                        ResponseT<Guid> response = trading.Login(userId, email, password);
                        if (response.ErrorOccured)
                            throw new Exception(response.ErrorMessage);

                        break;

                    case "Logout":
                        expectedParamsCount = 1;
                        checkExpectedParams(expectedParamsCount, parameters, function);
                        email = parameters.email;
                        if (string.IsNullOrEmpty(email))
                        {
                            throw new Exception("The email field is null in function " + (function));
                        }
                        userId = members[email].UserId;
                        response = trading.Logout(userId);
                        if (response.ErrorOccured)
                            throw new Exception(response.ErrorMessage);
                        break;

                    case "OpenNewStore":
                        expectedParamsCount = 2;
                        checkExpectedParams(expectedParamsCount, parameters, function);
                        String StoreName = parameters.StoreName;
                        if (string.IsNullOrEmpty(StoreName))
                        {
                            throw new Exception("The StoreName field is null in function " + (function));
                        }
                        email = parameters.email;
                        if (string.IsNullOrEmpty(email))
                        {
                            throw new Exception("The email field is null in function " + (function));
                        }
                        userId = members[email].UserId;
                        response = trading.OpenNewStore(userId, StoreName);
                        if (response.ErrorOccured)
                            throw new Exception(response.ErrorMessage);
                        Store store = trading.GetStore(StoreName).Value;
                        stores.Add(StoreName, store);
                        break;

                    case "AppointStoreOwner":
                        expectedParamsCount = 3;
                        checkExpectedParams(expectedParamsCount, parameters, function);
                        email = parameters.email;
                        if (string.IsNullOrEmpty(email))
                        {
                            throw new Exception("The email field is null in function " + (function));
                        }
                        String anotherEmail = parameters.MemberEmail;
                        if (string.IsNullOrEmpty(anotherEmail))
                        {
                            throw new Exception("The anotherEmail field is null in function " + (function));
                        }
                        StoreName = parameters.StoreName;
                        if (string.IsNullOrEmpty(StoreName))
                        {
                            throw new Exception("The StoreName field is null in function " + (function));
                        }
                        userId = members[email].UserId;
                        anotherEmail = members[anotherEmail].Email;
                        Guid storeID = stores[StoreName].StoreID;
                        res = trading.AppointStoreOwner(userId, storeID, anotherEmail);
                        if (res.ErrorOccured)
                            throw new Exception(res.ErrorMessage);
                        break;

                    case "AppointStoreManager":
                        expectedParamsCount = 4;
                        checkExpectedParams(expectedParamsCount, parameters, function);
                        email = parameters.email;
                        if (string.IsNullOrEmpty(email))
                        {
                            throw new Exception("The email field is null in function " + (function));
                        }
                        anotherEmail = parameters.MemberEmail;
                        if (string.IsNullOrEmpty(anotherEmail))
                        {
                            throw new Exception("The anotherEmail field is null in function " + (function));
                        }
                        StoreName = parameters.StoreName;
                        if (string.IsNullOrEmpty(StoreName))
                        {
                            throw new Exception("The StoreName field is null in function " + (function));
                        }
                        if (string.IsNullOrEmpty(parameters.Permission))
                        {
                            throw new Exception("The permission field is null in function " + (function));
                        }
                        userId = members[email].UserId;
                        anotherEmail = members[anotherEmail].Email;
                        storeID = stores[StoreName].StoreID;
                        res = trading.AppointStoreManager(userId, storeID, anotherEmail);
                        if (res.ErrorOccured)
                            throw new Exception(res.ErrorMessage);
                        res = trading.AddStoreManagerPermissions(userId, storeID, anotherEmail, parameters.Permission);
                        if (res.ErrorOccured)
                            throw new Exception(res.ErrorMessage);
                        break;

                    case "AddItemToStore":
                        expectedParamsCount = 6;
                        checkExpectedParams(expectedParamsCount, parameters, function);
                        email = parameters.email;
                        if (string.IsNullOrEmpty(email))
                        {
                            throw new Exception("The email field is null in function " + (function));
                        }
                        StoreName = parameters.StoreName;
                        if (string.IsNullOrEmpty(StoreName))
                        {
                            throw new Exception("The StoreName field is null in function " + (function));
                        }
                        storeID = stores[StoreName].StoreID;
                        userId = members[email].UserId;
                        String itemName = parameters.ItemName;
                        if (string.IsNullOrEmpty(itemName))
                        {
                            throw new Exception("The itemName field is null in function " + (function));
                        }
                        String itemCategory = parameters.ItemCategory;
                        if (string.IsNullOrEmpty(itemCategory))
                        {
                            throw new Exception("The itemCategory field is null in function " + (function));
                        }

                        double itemPrice;
                        double.TryParse(parameters.ItemPrice, out itemPrice);
                         if (itemPrice == 0)
                            throw new Exception("The itemPrice field is 0 in function " + (function));


                        int quantity;
                        int.TryParse(parameters.ItemQuantity, out quantity);
                        if (quantity == 0)
                            throw new Exception("The quantity field is 0 in function " + (function));

                        res = trading.AddItemToStore(userId, storeID, itemName, itemCategory, itemPrice,
                            quantity);
                         if (res.ErrorOccured)
                            throw new Exception(res.ErrorMessage);
                        itemPrice = 0;
                        quantity = 0;
                        break;

                }
            }
        }
        public void checkExpectedParams(int expectedParamsCount, Params parameters, string function)
        {
            int actualParamsCount = parameters != null ? GetNonNullParameterCount(parameters) : 0;

            if (actualParamsCount != expectedParamsCount)
            {
                throw new Exception($"Invalid number of parameters for function '{function}'. Expected: {expectedParamsCount}, Received: {actualParamsCount}");
            }
        }

        private int GetNonNullParameterCount(Params parameters)
        {
            int count = 0;
            foreach (var property in parameters.GetType().GetProperties())
            {
                var value = property.GetValue(parameters);
                
                    if (value != null && (!property.PropertyType.IsValueType || Nullable.GetUnderlyingType(property.PropertyType) != null))
                    {
                        count++;
                    }
                
            }
            return count;
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
