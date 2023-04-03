using System;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.DomainLayer.User;

namespace SadnaExpress.ServiceLayer
{
    public class Tradingsystem
    {
        private IStoreFacade storeFacade;
        private IUserFacade userFacade;
        
        public Tradingsystem()
        {
            storeFacade = new StoreFacade();
            userFacade = new UserFacade();
        }
        public int enter()
        {
            return userFacade.Enter();
        }
        public void exit(int id)
        {
            try
            {
                userFacade.Exit(id);

            }
            catch (Exception e)
            {

            }

        }
        public void register(int id, string email, string firstName, string lastName, string password)
        {
            try
            {
                userFacade.Register(id, email, firstName, lastName, password);

            }
            catch (Exception e)
            {

            }
        }
        public int login(int id, string email, string password)
        {
            try
            {
                return userFacade.Login(id, email, password);

            }
            catch (Exception e)
            {
                return -1; //need to check this
            }

        }
        public int logout(int id)
        {
            try
            {
                return userFacade.Logout(id);

            }
            catch (Exception e)
            {
                return -1; //need to check this
            }

        }
    }
}