using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.DomainLayer.User;
using SadnaExpress.ServiceLayer;
using SadnaExpress.Services;

namespace SadnaExpress
{
    internal class Program
    {
        private static Server _server;
        public static void Main(string[] args)
        {

            //new IntegrationTests().Run();


            // first test success throwed exception cant add item and stock to cart with more amount of 

            //TradingSystem domain = TradingSystem.Instance;
            //TradingSystem.Instance.Register(0, "asdqwe@gmail.com", "First", "Userqwe", "Asdqwe123");
            //TradingSystem.Instance.Register(1, "zxcasd@gmail.com", "Second", "Userxzc", "Zxcqwe123");

            //TradingSystem.Instance.Login(0, "asdqwe@gmail.com", "Asdqwe123");
            //Guid storeId = TradingSystem.Instance.OpenNewStore(0, "FirstStore").Value;
            //TradingSystem.Instance.AddItemToStore(0, storeId, "apple", "fruits", 5.99, 3);
            //TradingSystem.Instance.AddItemToCart(1, storeId, 0, 4);

            //Console.ReadLine();

            /////////////////////////////////////////////////////////////////
            // second test: successful purchase or failed purchase

            TradingSystem domain = TradingSystem.Instance;
            TradingSystem.Instance.Register(0, "asdqwe@gmail.com", "First", "Userqwe", "Asdqwe123");
            TradingSystem.Instance.Register(1, "zxcasd@gmail.com", "Second", "Userxzc", "Zxcqwe123");

            TradingSystem.Instance.Login(0, "asdqwe@gmail.com", "Asdqwe123");
            Guid storeId = TradingSystem.Instance.OpenNewStore(0, "FirstStore").Value;

            TradingSystem.Instance.AddItemToStore(0, storeId, "apple", "fruits", 5.99, 3);

            TradingSystem.Instance.AddItemToCart(1, storeId, 0, 2);

            TradingSystem.Instance.PurchaseCart(1, "Credit and Visa");
            Console.ReadLine();
        }

        public class IntegrationTests
        {


            private void Guest_get_info_stores_and_items_in()
            {
            }
            

            private void Member_get_info_purchase_history()
            {
                throw new NotImplementedException();
            }

            private void Member_rating_item()
            {
                throw new NotImplementedException();
            }

            private void Member_get_info_stores_and_items_in()
            {
                throw new NotImplementedException();
            }

            private void Member_show_shopping_cart()
            {
                throw new NotImplementedException();
            }

            private void Member_save_items_on_cart()
            {
                throw new NotImplementedException();
            }

            private void Member_search_items_fail()
            {
                throw new NotImplementedException();
            }

            private void Member_search_items_success()
            {
                throw new NotImplementedException();
            }

            private void Guest_purchase_cart_success()
            {
                throw new NotImplementedException();
            }

            private void Guest_show_shopping_cart()
            {
                throw new NotImplementedException();
            }

            private void Guest_save_items_on_cart()
            {
                throw new NotImplementedException();
            }

            private void Guest_search_items_fail()
            {
                throw new NotImplementedException();
            }

            private void Guest_search_items_success()
            {
                throw new NotImplementedException();
            }
            private void Get_purchases_history_info()
            {
                throw new NotImplementedException();
            }

            private void Get_employees_info()
            {
                throw new NotImplementedException();
            }

            private void Member_send_msg_store()
            {
                throw new NotImplementedException();
            }

            private void Member_rating_store()
            {
                throw new NotImplementedException();
            }

            private void Member_review_store()
            {
                throw new NotImplementedException();
            }
            private void Member_purchase_same_items_same_time()
            {
                throw new NotImplementedException();
            }

            private void Member_purchase_cart_fail()
            {
                throw new NotImplementedException();
            }

            private void Member_purchase_cart_success()
            {
                throw new NotImplementedException();
            }

            private void Guest_purchase_same_items_same_time()
            {
                throw new NotImplementedException();
            }
                        private void Manager_change_premission_bad()
            {
                throw new NotImplementedException();
            }


        }
        
    }
}