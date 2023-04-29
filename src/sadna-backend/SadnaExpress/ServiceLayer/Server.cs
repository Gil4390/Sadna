using SadnaExpress.DomainLayer.User;
using SadnaExpress.ServiceLayer.ServiceObjects;
using SadnaExpress.Services;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using SadnaExpress.DomainLayer.Store;

namespace SadnaExpress.ServiceLayer
{
    public class Server
    {
        public bool tradingSystemOpen = false;
        public TradingSystem service;

        public Server()
        {
            service = TradingSystem.Instance;
        }
        private static Guid ToGuid(int value)
        {
            byte[] bytes = new byte[16];
            BitConverter.GetBytes(value).CopyTo(bytes, 0);
            return new Guid(bytes);
        }

        //if will need in the next versions 

        //public void ServeClient(string name , Queue<string> commands)
        //{
        //    if (tradingSystemOpen)
        //    {
        //        ResponseT<Guid> responseID = service.Enter();
        //        Guid id = responseID.Value;
        //        string nexCommand;

        //        while (commands.Count > 0)
        //        {
        //            nexCommand = commands.Dequeue();
        //            string[] split = nexCommand.Split(' ');
        //            string command_type = split[0];
        //            if (command_type == "EXIT")
        //            {
        //                Response response = service.Exit(id);
        //                Console.WriteLine("Client exited");
        //                Thread.CurrentThread.Abort();
        //            }
        //            else if (command_type == "REGISTER")
        //            {
        //                //REGISTER <email> <firstName> <lastName> <password>
        //                if (split.Length != 5)
        //                {
        //                    throw new Exception("invalid register args");
        //                }

        //                string email = split[1];
        //                string firstName = split[2];
        //                string lastName = split[3];
        //                string password = split[4];
        //                Response response = service.Register(id, email, firstName, lastName, password);

        //                if (response.ErrorOccured)
        //                {
        //                    Console.WriteLine(id + " - " + response.ErrorMessage);
        //                }
        //                else
        //                {
        //                    Console.WriteLine(id + " - OK");
        //                }

        //            }
        //            else if (command_type == "LOGIN")
        //            {
        //                //LOGIN <email> <password>
        //                if (split.Length != 3)
        //                {
        //                    throw new Exception("invalid login args");
        //                }

        //                string email = split[1];
        //                string password = split[2];
        //                ResponseT<Guid> response = service.Login(id, email, password);

        //                if (response.ErrorOccured)
        //                {
        //                    Console.WriteLine(id + " - " + response.ErrorMessage);
        //                }
        //                else
        //                {
        //                    id = response.Value;
        //                    Console.WriteLine(id + " - OK");
        //                }
        //            }
        //            else if (command_type == "LOGOUT")
        //            {
        //                //LOGOUT
        //                ResponseT<Guid> response = service.Logout(id);
        //                if (response.ErrorOccured)
        //                {
        //                    Console.WriteLine(id + " - " + response.ErrorMessage);
        //                }
        //                else
        //                {
        //                    id = response.Value;
        //                    Console.WriteLine(id + " - OK");
        //                }
        //            }
        //            else if (command_type == "INFO")
        //            {
        //                //INFO
        //                ResponseT<List<S_Store>> respone = service.GetAllStoreInfo(id);
        //                List<S_Store> stores = respone.Value;
        //                //todo generate message to client with all the info and send it to him
        //            }
        //            else if (command_type == "ADD-ITEM-TO-CART")
        //            {
        //                //ADD-ITEM-TO-CART <itemID> <amount>
        //                if (split.Length != 4)
        //                {
        //                    throw new Exception("invalid add item to cart args");
        //                }
        //                int storeID = int.Parse(split[1]);
        //                int itemID = int.Parse(split[2]);
        //                int itemAmount = int.Parse(split[3]);
        //                service.AddItemToCart(id, ToGuid(storeID),itemID, itemAmount);
        //            }
        //            else if (command_type == "PURCHASE-CART")
        //            {
        //                //PURCHASE-CART <payment details>
        //                if (split.Length != 2)
        //                {
        //                    throw new Exception("invalid purchase cart args");
        //                }

        //                string paymentDetails = split[1];
        //                service.PurchaseCart(id, paymentDetails);
        //                //todo send confirmation to client
        //            }
        //            else if (command_type == "CREATE-STORE")
        //            {
        //                //CREATE-STORE <storeName>
        //                if (split.Length != 2)
        //                {
        //                    throw new Exception("invalid store creation args");
        //                }

        //                string storeName = split[1];
        //                service.OpenNewStore(id, storeName);
        //            }
        //            else if (command_type == "WRITE-REVIEW")
        //            {
        //                //WRITE-REVIEW <itemID> <review-text>
        //                if (split.Length != 3)
        //                {
        //                    throw new Exception("invalid write review args");
        //                }

        //                int itemID = int.Parse(split[1]);
        //                string review = split[2];
        //                service.WriteReview(id, itemID, review);
        //            }
        //            else if (command_type == "RATE-ITEM")
        //            {
        //                //RATE-ITEM <itemID> <score>
        //                if (split.Length != 3)
        //                {
        //                    throw new Exception("invalid rate item args");
        //                }

        //                int itemID = int.Parse(split[1]);
        //                int score = int.Parse(split[2]);
        //                service.RateItem(id, itemID, score);
        //            }
        //            else if (command_type == "WRITE-MESSAGE-TO-STORE")
        //            {
        //                //WRITE-MESSAGE-TO-STORE <storeID> <message>
        //                if (split.Length != 3)
        //                {
        //                    throw new Exception("invalid write message to store args");
        //                }

        //                int storeID = int.Parse(split[1]);
        //                string message = split[2];
        //                service.WriteMessageToStore(id, ToGuid(storeID), message);
        //            }
        //            else if (command_type == "COMPLAIN-TO-ADMIN")
        //            {
        //                //COMPLAIN-TO-ADMIN <message>
        //                if (split.Length != 2)
        //                {
        //                    throw new Exception("invalid complain to admin args");
        //                }

        //                string message = split[1];
        //                service.ComplainToAdmin(id, message);
        //            }
        //            else if (command_type == "PURCHASES-INFO")
        //            {
        //                //PURCHASES-INFO
        //                service.GetPurchasesInfo(id);
        //                //todo need to put the info in some service object and send the info to client
        //            }
        //            else if (command_type == "ADD-ITEM")
        //            {
        //                //ADD-ITEM <storeID> <itemName> <itemCat> <itemPrice>
        //                if (split.Length != 6)
        //                {
        //                    throw new Exception("invalid add item args");
        //                }

        //                int storeID = int.Parse(split[1]);
        //                string itemName = split[2];
        //                string itemCategory = split[3];
        //                double itemPrice = double.Parse(split[4]);
        //                int quantity = int.Parse(split[5]);
        //                service.AddItemToStore(id, ToGuid(storeID), itemName, itemCategory, itemPrice, quantity);
        //            }
        //            else if (command_type == "REMOVE-ITEM")
        //            {
        //                //REMOVE-ITEM <storeID> <itemID>
        //                if (split.Length != 3)
        //                {
        //                    throw new Exception("invalid remove item args");
        //                }

        //                int storeID = int.Parse(split[1]);
        //                int itemID = int.Parse(split[2]);
        //                service.RemoveItemFromStore(id, ToGuid(storeID), itemID);
        //            }
        //            else if (command_type == "EDIT-ITEM")
        //            {
        //                //todo name, price, category....
        //            }
        //            else if (command_type == "POLICY")
        //            {
        //                //todo
        //            }
        //            else if (command_type == "APOINT-STORE-OWNER")
        //            {
        //                //APOINT-STORE-OWNER <storeID> <newUserID>
        //                if (split.Length != 3)
        //                {
        //                    throw new Exception("invalid APOINT-STORE-OWNER args");
        //                }

        //                int storeID = int.Parse(split[1]);
        //                string userEmail = split[2];
        //                service.AppointStoreOwner(id, ToGuid(storeID), userEmail);
        //            }
        //            else if (command_type == "REMOVE-STORE-OWNER")
        //            {
        //                //REMOVE-STORE-OWNER <storeID> <UserID>
        //                if (split.Length != 3)
        //                {
        //                    throw new Exception("invalid REMOVE-STORE-OWNER args");
        //                }

        //                int storeID = int.Parse(split[1]);
        //                Guid UserID = Guid.Parse(split[2]);
        //                service.RemoveStoreOwner(id, ToGuid(storeID), UserID);
        //            }
        //            else if (command_type == "APOINT-STORE-MANAGER")
        //            {
        //                //APOINT-STORE-MANAGER <storeID> <newUserID>
        //                if (split.Length != 3)
        //                {
        //                    throw new Exception("invalid APOINT-STORE-MANAGER args");
        //                }

        //                int storeID = int.Parse(split[1]);
        //                string newUserEmail = split[2];
        //                service.AppointStoreManager(id, ToGuid(storeID), newUserEmail);
        //            }
        //            else if (command_type == "REMOVE-STORE-MANAGER")
        //            {
        //                //REMOVE-STORE-MANAGER <storeID> <UserID>
        //                if (split.Length != 3)
        //                {
        //                    throw new Exception("invalid REMOVE-STORE-MANAGER args");
        //                }

        //                int storeID = int.Parse(split[1]);
        //                Guid UserID = Guid.Parse(split[2]);
        //                service.RemovetStoreManager(id, ToGuid(storeID), UserID);
        //            }
        //            else if (command_type == "CHANGE-PERMMISION")
        //            {
        //                //todo
        //            }
        //            else if (command_type == "CLOSE-STORE")
        //            {
        //                //CLOSE-STORE <storeID>
        //                if (split.Length != 2)
        //                {
        //                    throw new Exception("invalid CLOSE-STORE args");
        //                }

        //                int storeID = int.Parse(split[1]);
        //                service.CloseStore(id, ToGuid(storeID));
        //            }
        //            else if (command_type == "REOPEN-STORE")
        //            {
        //                //REOPEN-STORE <storeID>
        //                if (split.Length != 2)
        //                {
        //                    throw new Exception("invalid REOPEN-STORE args");
        //                }

        //                int storeID = int.Parse(split[1]);
        //                service.ReopenStore(id, ToGuid(storeID));
        //            }
        //            else if (command_type == "EMPLOYEE-INFO")
        //            {
        //                //EMPLOYEE-INFO <storeID>
        //                if (split.Length != 2)
        //                {
        //                    throw new Exception("invalid EMPLOYEE-INFO args");
        //                }

        //                int storeID = int.Parse(split[1]);
        //                ResponseT<List<S_Member>> response = service.GetEmployeeInfoInStore(id, ToGuid(storeID));
        //                List<S_Member> employees = response.Value;
        //                //todo send the info to client
        //            }
        //            else if (command_type == "STORE-PURCHASES-INFO")
        //            {
        //                //STORE-PURCHASES-INFO <storeID>
        //                if (split.Length != 2)
        //                {
        //                    throw new Exception("invalid STORE-PURCHASES-INFO args");
        //                }

        //                int storeID = int.Parse(split[1]);
        //                service.GetPurchasesInfo(id, ToGuid(storeID));
        //                //todo send the info to client
        //            }
        //            else if (command_type == "REPLY-COMPLAINT")
        //            {
        //                //todo
        //                //manager and owner?
        //            }
        //            else if (command_type == "DELETE-STORE")
        //            {
        //                //DELETE-STORE <storeID>
        //                if (split.Length != 2)
        //                {
        //                    throw new Exception("invalid DELETE-STORE args");
        //                }
                        
        //                Guid storeID = Guid.Parse(split[1]);
        //                service.DeleteStore(id, storeID);
        //            }
        //            else if (command_type == "DELETE-MEMBER")
        //            {
        //                //DELETE-MEMBER <userID>
        //                if (split.Length != 2)
        //                {
        //                    throw new Exception("invalid DELETE-MEMBER args");
        //                }

        //                Guid userID = Guid.Parse(split[1]);
        //                service.DeleteMember(id, userID);
        //            }
        //            else if (command_type == "SYSTEM-INFO")
        //            {
        //                //SYSTEM-INFO
        //                //todo
        //            }
        //            else if (command_type == "SHOW SHOPPING CART")
        //            {
                        
        //                ResponseT<ShoppingCart> response = service.ShowShoppingCart(id);

        //                if (response.ErrorOccured)
        //                {
        //                    Console.WriteLine(id + " - " + response.ErrorMessage);
        //                }
        //                else
        //                { 
        //                    Console.WriteLine(id + " - " + response.Value);
        //                }
        //            }
        //            else if (command_type == "UPDATE")
        //            {
        //                int idx = 0;
        //                ResponseT<Guid> response = new ResponseT<Guid>();
        //                string updateCommand = "";
        //                foreach (string s in split)
        //                {
        //                    string command = "";
        //                    if (idx != 0 && idx % 2 != 0)
        //                    {
        //                        updateCommand = s;
        //                    }
        //                    else if (idx != 0 && idx % 2 == 0)
        //                    {
        //                        if (updateCommand == "FIRST")
        //                        {
        //                            response = service.UpdateFirst(id , s);
        //                        }
        //                        else if (updateCommand == "LAST")
        //                        {
        //                            response = service.UpdateLast(id , s);
        //                        }
        //                        else if (updateCommand == "PASSWORD")
        //                        {
        //                            response = service.UpdatePassword(id , s);
        //                        }
        //                    }
        //                }

        //                if (response.ErrorOccured)
        //                {
        //                    Console.WriteLine(id + " - " + response.ErrorMessage);
        //                }
        //                else
        //                { 
        //                    Console.WriteLine(id + " - " + response.Value);
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        Console.WriteLine("Trading system is not OPEN");
        //    }
        //}
    }
}