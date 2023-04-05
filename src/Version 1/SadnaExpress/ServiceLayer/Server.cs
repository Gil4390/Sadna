using SadnaExpress.DomainLayer.User;
using SadnaExpress.ServiceLayer.ServiceObjects;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SadnaExpress.ServiceLayer
{
    public class Server
    {
        TcpListener server = null;

        private bool tradingSystemOpen = false;

        public TradingSystem service;

        public Dictionary<string, string> Cache = new Dictionary<string, string>();

        public Server(string ip, int port)
        {
            service = new TradingSystem();
            IPAddress localAddr = IPAddress.Parse(ip);
            server = new TcpListener(localAddr, port);
            server.Start();
            
            Serve();
        }

        public void Serve()
        {
            try
            {
                while (!tradingSystemOpen)
                {
                    if (Console.ReadLine() == "Admin Admin")
                        tradingSystemOpen = true;
                    else
                    {
                        Console.WriteLine("Trading system will run when entering valid password");
                    }
                }
                while (tradingSystemOpen)
                {
                    TcpClient client = server.AcceptTcpClient();
                    Thread t = new Thread(HandleClient);
                    t.Start(client);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: " + e);
                server.Stop();
            }
        }
        public void HandleClient(Object obj)
        {

            TcpClient client = (TcpClient)obj;
            Console.WriteLine("Client connected!");
            ResponseT<int> responseID = service.enter(); // each handler gets a new id, if user loggs in the id will change
            int id = responseID.Value;
            var stream = client.GetStream();


            string messageFromClient = "";
            string responseToClient = "";
            Byte[] bytes = new Byte[256];
            int i;
            try
            {
                while (true)
                {
                    Byte[] reply;
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        messageFromClient = Encoding.ASCII.GetString(bytes, 0, i);
                        Console.WriteLine(messageFromClient);

                        //responseToClient = parse(messageFromClient);



                        string[] split = messageFromClient.Split(' ');
                        string command_type = split[0];

                        if (command_type == "EXIT")
                        {
                            //EXIT
                            Response response = service.exit(id);
                            client.Close();
                            Console.WriteLine("Client exited");
                        }
                        else if (command_type == "REGISTER")
                        {
                            //REGISTER <email> <firstName> <lastName> <password>
                            if (split.Length != 5) { throw new Exception("invalid register args"); }
                            string email = split[1];
                            string firstName = split[2];
                            string lastName = split[3];
                            string password = split[4];
                            Response response = service.register(id, email, firstName, lastName, password);

                            if (response.ErrorOccured)
                            {
                                responseToClient = response.ErrorMessage;
                            }
                            else
                            {
                                responseToClient = "OK";
                            }

                        }
                        else if (command_type == "LOGIN")
                        {
                            //LOGIN <email> <password>
                            if (split.Length != 3) { throw new Exception("invalid login args"); }
                            string email = split[1];
                            string password = split[2];
                            ResponseT<int> response = service.login(id, email, password);

                            if (response.ErrorOccured)
                            {
                                responseToClient = response.ErrorMessage;
                            }
                            else
                            {
                                id = response.Value;
                                responseToClient = "OK";
                            }
                        }
                        else if (command_type == "LOGOUT")
                        {
                            //LOGOUT
                            ResponseT<int> response = service.logout(id);
                            if (response.ErrorOccured)
                            {
                                responseToClient = response.ErrorMessage;
                            }
                            else
                            {
                                id = response.Value;
                                responseToClient = "OK";
                            }
                        }
                        else if (command_type == "INFO")
                        {
                            //INFO
                            ResponseT<List<S_Store>> respone = service.getAllStoreInfo(id);
                            List<S_Store> stores = respone.Value;
                            //todo generate message to client with all the info and send it to him
                        }
                        else if (command_type == "ADD-ITEM-TO-CART")
                        {
                            //ADD-ITEM-TO-CART <itemID> <amount>
                            if (split.Length != 3) { throw new Exception("invalid add item to cart args"); }
                            int itemID = int.Parse(split[1]);
                            int itemAmount = int.Parse(split[2]);
                            service.addItemToCart(id, itemID, itemAmount);
                        }
                        else if (command_type == "PURCHASE-CART")
                        {
                            //PURCHASE-CART <payment details>
                            if (split.Length != 2) { throw new Exception("invalid purchase cart args"); }
                            string paymentDetails = split[1];
                            service.purchaseCart(id, paymentDetails);
                            //todo send confirmation to client
                        }
                        else if (command_type == "CREATE-STORE")
                        {
                            //CREATE-STORE <storeName>
                            if (split.Length != 2) { throw new Exception("invalid store creation args"); }
                            string storeName = split[1];
                            service.createStore(id, storeName);
                        }
                        else if (command_type == "WRITE-REVIEW")
                        {
                            //WRITE-REVIEW <itemID> <review-text>
                            if (split.Length != 3) { throw new Exception("invalid write review args"); }
                            int itemID = int.Parse(split[1]);
                            string review = split[2];
                            service.writeReview(id, itemID, review);
                        }
                        else if (command_type == "RATE-ITEM")
                        {
                            //RATE-ITEM <itemID> <score>
                            if (split.Length != 3) { throw new Exception("invalid rate item args"); }
                            int itemID = int.Parse(split[1]);
                            int score = int.Parse(split[2]);
                            service.rateItem(id, itemID, score);
                        }
                        else if (command_type == "WRITE-MESSAGE-TO-STORE")
                        {
                            //WRITE-MESSAGE-TO-STORE <storeID> <message>
                            if (split.Length != 3) { throw new Exception("invalid write message to store args"); }
                            int storeID = int.Parse(split[1]);
                            string message = split[2];
                            service.writeMessageToStore(id, storeID, message);
                        }
                        else if (command_type == "COMPLAIN-TO-ADMIN")
                        {
                            //COMPLAIN-TO-ADMIN <message>
                            if (split.Length != 2) { throw new Exception("invalid complain to admin args"); }
                            string message = split[1];
                            service.complainToAdmin(id, message);
                        }
                        else if (command_type == "PURCHASES-INFO")
                        {
                            //PURCHASES-INFO
                            service.getPurchasesInfo(id);
                            //todo need to put the info in some service object and send the info to client
                        }
                        else if (command_type == "ADD-ITEM")
                        {
                            //ADD-ITEM <storeID> <itemName> <itemCat> <itemPrice>
                            if (split.Length != 5) { throw new Exception("invalid add item args"); }
                            int storeID = int.Parse(split[1]);
                            string itemName = split[2];
                            string itemCategory = split[3];
                            float itemPrice = float.Parse(split[4]);
                            service.addItemToStore(id, storeID, itemName, itemCategory, itemPrice);
                        }
                        else if (command_type == "REMOVE-ITEM")
                        {
                            //REMOVE-ITEM <storeID> <itemID>
                            if (split.Length != 3) { throw new Exception("invalid remove item args"); }
                            int storeID = int.Parse(split[1]);
                            int itemID = int.Parse(split[2]); 
                            service.removeItemFromStore(id, storeID, itemID);
                        }
                        else if (command_type == "EDIT-ITEM")
                        {
                            //todo name, price, category....
                        }
                        else if (command_type == "POLICY")
                        {
                            //todo
                        }
                        else if (command_type == "APOINT-STORE-OWNER")
                        {
                            //APOINT-STORE-OWNER <storeID> <newUserID>
                            if (split.Length != 3) { throw new Exception("invalid APOINT-STORE-OWNER args"); }
                            int storeID = int.Parse(split[1]);
                            int newUserID = int.Parse(split[2]);
                            service.appointStoreOwner(id, storeID, newUserID);
                        }
                        else if (command_type == "REMOVE-STORE-OWNER")
                        {
                            //REMOVE-STORE-OWNER <storeID> <UserID>
                            if (split.Length != 3) { throw new Exception("invalid REMOVE-STORE-OWNER args"); }
                            int storeID = int.Parse(split[1]);
                            int UserID = int.Parse(split[2]);
                            service.removeStoreOwner(id, storeID, UserID);
                        }
                        else if (command_type == "APOINT-STORE-MANAGER")
                        {
                            //APOINT-STORE-MANAGER <storeID> <newUserID>
                            if (split.Length != 3) { throw new Exception("invalid APOINT-STORE-MANAGER args"); }
                            int storeID = int.Parse(split[1]);
                            int newUserID = int.Parse(split[2]);
                            service.appointStoreManager(id, storeID, newUserID);
                        }
                        else if (command_type == "REMOVE-STORE-MANAGER")
                        {
                            //REMOVE-STORE-MANAGER <storeID> <UserID>
                            if (split.Length != 3) { throw new Exception("invalid REMOVE-STORE-MANAGER args"); }
                            int storeID = int.Parse(split[1]);
                            int UserID = int.Parse(split[2]);
                            service.removetStoreManager(id, storeID, UserID);
                        }
                        else if (command_type == "CHANGE-PERMMISION")
                        {
                            //todo
                        }
                        else if (command_type == "CLOSE-STORE")
                        {
                            //CLOSE-STORE <storeID>
                            if (split.Length != 2) { throw new Exception("invalid CLOSE-STORE args"); }
                            int storeID = int.Parse(split[1]);
                            service.closeStore(id, storeID);
                        }
                        else if (command_type == "REOPEN-STORE")
                        {
                            //REOPEN-STORE <storeID>
                            if (split.Length != 2) { throw new Exception("invalid REOPEN-STORE args"); }
                            int storeID = int.Parse(split[1]);
                            service.reopenStore(id, storeID);
                        }
                        else if (command_type == "EMPLOYEE-INFO")
                        {
                            //EMPLOYEE-INFO <storeID>
                            if (split.Length != 2) { throw new Exception("invalid EMPLOYEE-INFO args"); }
                            int storeID = int.Parse(split[1]);
                            ResponseT<List<S_Member>> response = service.getEmployeeInfoInStore(id, storeID);
                            List<S_Member> employees = response.Value;
                            //todo send the info to client
                        }
                        else if (command_type == "STORE-PURCHASES-INFO")
                        {
                            //STORE-PURCHASES-INFO <storeID>
                            if (split.Length != 2) { throw new Exception("invalid STORE-PURCHASES-INFO args"); }
                            int storeID = int.Parse(split[1]);
                            service.getPurchasesInfo(id, storeID);
                            //todo send the info to client
                        }
                        else if (command_type == "REPLY-COMPLAINT")
                        {
                            //todo
                            //manager and owner?
                        }
                        else if (command_type == "DELETE-STORE")
                        {
                            //PERMENATLY-CLOSE-STORE <storeID>
                            if (split.Length != 2) { throw new Exception("invalid DELETE-STORE args"); }
                            int storeID = int.Parse(split[1]);
                            service.deleteStore(id, storeID);
                        }
                        else if (command_type == "DELETE-MEMBER")
                        {
                            //PERMENATLY-CLOSE-STORE <userID>
                            if (split.Length != 2) { throw new Exception("invalid DELETE-MEMBER args"); }
                            int userID = int.Parse(split[1]);
                            service.deleteMember(id, userID);
                        }
                        else if (command_type == "SYSTEM-INFO")
                        {
                            //SYSTEM-INFO
                            //todo
                        }


                        reply = System.Text.Encoding.ASCII.GetBytes(responseToClient);
                        stream.Write(reply, 0, reply.Length);
                    }
                    responseToClient = "Check-For-Connection";
                    reply = System.Text.Encoding.ASCII.GetBytes(responseToClient);
                    stream.Write(reply, 0, reply.Length);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Connection lost");
                service.exit(id);
                client.Close();
            }
        }

        public string parse(string msg)
        {
            //Register (username) (password)
            //Login (username) (password)
            //Logout
            //Exit system
            //Open store (name) ...
            //Close store (name) ...
            //Purchase
            //Show history (storeName)
            //Add (itemName) (StoreName)
            //Remove (itemName) (StoreName)
            //Edit (itemName) (category)
            //Edit (itemName) (price)
            //Get (temName)
            //Get (category)
            //Get (keyWords)
            //Get (minPrice) (maxPrice)
            //Get (storeName) (rating)
            //Get (itemName) (rating)
            //Review (itemName) (Store)


            return "";
        }

    }
}