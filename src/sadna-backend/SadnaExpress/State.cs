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
                           Logger.Instance.Error("The email field is null in function " + (function));
                           throw new Exception("The email field is null in function " + (function));

                        }
                        string firstName = parameters.firstName;
                        if (string.IsNullOrEmpty(firstName))
                        {
                            Logger.Instance.Error("The firstName field is null in function " + (function));
                            throw new Exception("The firstName field is null in function " + (function));
                        }
                        string lastName = parameters.lastName;
                        if (string.IsNullOrEmpty(lastName))
                        {
                            Logger.Instance.Error("The lastName field is null in function " + (function));
                            throw new Exception("The lastName field is null in function " + (function));
                        }
                        string password = parameters.password;
                        if (string.IsNullOrEmpty(password))
                        {
                            Logger.Instance.Error("The password field is null in function " + (function));
                            throw new Exception("The password field is null in function " + (function));
                        }
                        Response res = trading.Register(userId, email, firstName, lastName, password);
                        if (res.ErrorOccured)
                            throw new Exception(res.ErrorMessage);
                        Member member = trading.GetMember(email).Value;
                        members.Add(email, member);
                        expectedParamsCount = 4;
                        checkExpectedParams(expectedParamsCount, parameters, function);

                        Logger.Instance.Info("The parser for " + (function) + " function worked successfully.");

                        break;

                    case "CreateSystemManager":
                        expectedParamsCount = 1;
                        checkExpectedParams(expectedParamsCount, parameters, function);
                        email = parameters.email;
                        if (string.IsNullOrEmpty(email))
                        {
                            Logger.Instance.Error("The email field is null in function " + (function));
                            throw new Exception("The email field is null in function " + (function));

                        }
                        userId = members[email].UserId;
                        PromotedMember promotedMember = trading.GetMember(email).Value.promoteToMember();
                        if (promotedMember == null) { 
                            Logger.Instance.Error("unable to promoted this member");
                            throw new Exception("unable to promoted this member");

                        }
                        promotedMember.createSystemManager();
                        Logger.Instance.Info("The parser for " + (function) + " function worked successfully.");

                        break;

                    case "Login":
                        expectedParamsCount = 2;
                        checkExpectedParams(expectedParamsCount, parameters, function);
                        userId = trading.Enter().Value;
                        email = parameters.email;
                        if (string.IsNullOrEmpty(email))
                        {
                            Logger.Instance.Error("The email field is null in function " + (function));
                            throw new Exception("The email field is null in function " + (function));
                        }
                        password = parameters.password;
                        if (string.IsNullOrEmpty(password))
                        {
                            Logger.Instance.Error("The password field is null in function " + (function));
                            throw new Exception("The password field is null in function " + (function));
                        }
                        ResponseT<Guid> response = trading.Login(userId, email, password);
                        if (response.ErrorOccured)
                            throw new Exception(response.ErrorMessage);
                        Logger.Instance.Info("The parser for " + (function) + " function worked successfully.");

                        break;

                    case "Logout":
                        expectedParamsCount = 1;
                        checkExpectedParams(expectedParamsCount, parameters, function);
                        email = parameters.email;
                        if (string.IsNullOrEmpty(email))
                        {
                            Logger.Instance.Error("The email field is null in function " + (function));
                            throw new Exception("The email field is null in function " + (function));

                        }
                        userId = members[email].UserId;
                        response = trading.Logout(userId);
                        if (response.ErrorOccured)
                            throw new Exception(response.ErrorMessage);
                        Logger.Instance.Info("The parser for " + (function) + " function worked successfully.");

                        break;

                    case "OpenNewStore":
                        expectedParamsCount = 2;
                        checkExpectedParams(expectedParamsCount, parameters, function);
                        String StoreName = parameters.StoreName;
                        if (string.IsNullOrEmpty(StoreName))
                        {
                            Logger.Instance.Error("The email field is null in function " + (function));
                            throw new Exception("The StoreName field is null in function " + (function));

                        }
                        email = parameters.email;
                        if (string.IsNullOrEmpty(email))
                        {
                            Logger.Instance.Error("The email field is null in function " + (function));
                            throw new Exception("The email field is null in function " + (function));

                        }
                        userId = members[email].UserId;
                        response = trading.OpenNewStore(userId, StoreName);
                        if (response.ErrorOccured)
                            throw new Exception(response.ErrorMessage);
                        Store store = trading.GetStore(StoreName).Value;
                        stores.Add(StoreName, store);
                        Logger.Instance.Info("The parser for " + (function) + " function worked successfully.");

                        break;

                    case "AppointStoreOwner":
                        expectedParamsCount = 3;
                        checkExpectedParams(expectedParamsCount, parameters, function);
                        email = parameters.email;
                        if (string.IsNullOrEmpty(email))
                        {
                            Logger.Instance.Error("The email field is null in function " + (function));
                            throw new Exception("The email field is null in function " + (function));

                        }
                        String anotherEmail = parameters.MemberEmail;
                        if (string.IsNullOrEmpty(anotherEmail))
                        {
                            Logger.Instance.Error("The anotherEmail field is null in function " + (function));
                            throw new Exception("The anotherEmail field is null in function " + (function));

                        }
                        StoreName = parameters.StoreName;
                        if (string.IsNullOrEmpty(StoreName))
                        {
                            Logger.Instance.Error("The StoreName field is null in function " + (function));
                            throw new Exception("The StoreName field is null in function " + (function));

                        }
                        userId = members[email].UserId;
                        anotherEmail = members[anotherEmail].Email;
                        Guid storeID = stores[StoreName].StoreID;
                        res = trading.AppointStoreOwner(userId, storeID, anotherEmail);
                        if (res.ErrorOccured)
                            throw new Exception(res.ErrorMessage);

                        Logger.Instance.Info("The parser for " + (function) + " function worked successfully.");

                        break;

                    case "AppointStoreManager":
                        expectedParamsCount = 4;
                        checkExpectedParams(expectedParamsCount, parameters, function);
                        email = parameters.email;
                        if (string.IsNullOrEmpty(email))
                        {
                            Logger.Instance.Error("The email field is null in function " + (function));
                            throw new Exception("The email field is null in function " + (function));

                        }
                        anotherEmail = parameters.MemberEmail;
                        if (string.IsNullOrEmpty(anotherEmail))
                        {
                            Logger.Instance.Error("The anotherEmail field is null in function " + (function));
                            throw new Exception("The anotherEmail field is null in function " + (function));

                        }
                        StoreName = parameters.StoreName;
                        if (string.IsNullOrEmpty(StoreName))
                        {
                            Logger.Instance.Error("The StoreName field is null in function " + (function));
                            throw new Exception("The StoreName field is null in function " + (function));

                        }
                        if (string.IsNullOrEmpty(parameters.Permission))
                        {
                            Logger.Instance.Error("The permission field is null in function " + (function));
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
                        Logger.Instance.Info("The parser for " + (function) + " function worked successfully.");

                        break;

                    case "AddItemToStore":
                        expectedParamsCount = 6;
                        checkExpectedParams(expectedParamsCount, parameters, function);
                        email = parameters.email;
                        if (string.IsNullOrEmpty(email))
                        {
                            Logger.Instance.Error("The email field is null in function " + (function));
                            throw new Exception("The email field is null in function " + (function));

                        }
                        StoreName = parameters.StoreName;
                        if (string.IsNullOrEmpty(StoreName))
                        {
                            Logger.Instance.Error("The StoreName field is null in function " + (function));
                            throw new Exception("The StoreName field is null in function " + (function));

                        }
                        storeID = stores[StoreName].StoreID;
                        userId = members[email].UserId;
                        String itemName = parameters.ItemName;
                        if (string.IsNullOrEmpty(itemName))
                        {
                            Logger.Instance.Error("The itemName field is null in function " + (function));
                            throw new Exception("The itemName field is null in function " + (function));

                        }
                        String itemCategory = parameters.ItemCategory;
                        if (string.IsNullOrEmpty(itemCategory))
                        {
                            Logger.Instance.Error("The itemCategory field is null in function " + (function));
                            throw new Exception("The itemCategory field is null in function " + (function));

                        }

                        double itemPrice;
                        double.TryParse(parameters.ItemPrice, out itemPrice);
                         if (itemPrice == 0) { 
                            Logger.Instance.Error("The itemPrice field is 0 in function " + (function));
                            throw new Exception("The itemPrice field is 0 in function " + (function));

                            }


                        int quantity;
                        int.TryParse(parameters.ItemQuantity, out quantity);
                        if (quantity == 0) { 
                            Logger.Instance.Error("The quantity field is 0 in function " + (function));
                            throw new Exception("The quantity field is 0 in function " + (function));

                            }

                        res = trading.AddItemToStore(userId, storeID, itemName, itemCategory, itemPrice,
                            quantity);
                         if (res.ErrorOccured)
                            throw new Exception(res.ErrorMessage);
                        itemPrice = 0;
                        quantity = 0;
                        Logger.Instance.Info("The parser for " + (function) + " function worked successfully.");

                        break;

                    default:
                        Logger.Instance.Error("Invalid function name: " + (function));
                        throw new Exception("Invalid function name: " + (function));

                }
            }
        }
        public void checkExpectedParams(int expectedParamsCount, Params parameters, string function)
        {
            int actualParamsCount = parameters != null ? GetNonNullParameterCount(parameters) : 0;

            if (actualParamsCount != expectedParamsCount)
            {
                Logger.Instance.Error($"Invalid number of parameters for function '{function}'. Expected: {expectedParamsCount}, Received: {actualParamsCount}");
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
                Logger.Instance.Error("There is a user that was not initilized");
                throw new Exception("There is a user that was not initilized");

            }
            List<Member> list = new List<Member>(members.Values);


            // open store1
            ConcurrentDictionary<Guid, Store> stores = DBHandler.Instance.GetAllStores();
            List<Store> storesList = new List<Store>(stores.Values);
            if (storesList[0] == null)
            {
                Logger.Instance.Error("The store was not created");
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
                        Logger.Instance.Error("The user does not have the correct permissions");
                        throw new Exception("The user does not have the correct permissions");

                    }
                }
                // user3 has owner permissions
                if (member.Email.Equals("user3@gmail.com"))
                {
                    if (!((PromotedMember)member).hasPermissions(storeID, permissions))
                    {
                        Logger.Instance.Error("The user does not have the correct permissions");
                        throw new Exception("The user does not have the correct permissions");

                    }
                }
            }
           Logger.Instance.Info("The parser for data.json file worked successfully.");
        }

        public void checkFile1()
        {
            TradingSystem trading = TradingSystem.Instance;
            trading.SetIsSystemInitialize(true);
            trading.TestMode = true;
            ConcurrentDictionary<Guid, Member> members = DBHandler.Instance.GetAllMembers();

            // 5 members register
            if (members.Count != 5)
            {
                Logger.Instance.Error("There is a user that was not initilized");
                throw new Exception("There is a user that was not initilized");

            }
            List<Member> list = new List<Member>(members.Values);

            // open store1
            ConcurrentDictionary<Guid, Store> stores = DBHandler.Instance.GetAllStores();
            List<Store> storesList = new List<Store>(stores.Values);
            if (storesList[0] == null)
            {
                Logger.Instance.Error("The store was not created");
                throw new Exception("The store was not created");


            }
            Guid storeID = storesList[0].StoreID;
            List<Item> items = trading.GetItemsInStore(list[1].UserId, storeID).Value;

            // item in store1
            if (items[0] == null)
            {
                Logger.Instance.Error("The store does not have items");
                throw new Exception("The store does not have items");
            }
            List<String> permissionsSystemManager = new List<string>();

            permissionsSystemManager.Add("system manager permissions");
            foreach (Member member in list)
            {
                // user1 has system manager permissions
                if (member.Email.Equals("user1@gmail.com"))
                {
                    if (!((PromotedMember)member).hasPermissions(Guid.Empty, permissionsSystemManager))
                    {
                        Logger.Instance.Error("The user does not have the correct permissions");
                        throw new Exception("The user does not have the correct permissions");

                    }
                }
             

            }
          Logger.Instance.Info("The parser for data2.json file worked successfully.");

        }

    }
}
