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
        
        public void register(int id , string email , string firstName ,  string lastName , string password)
        {
            try
            {
                userFacade.Register(id ,email, firstName, lastName, password);

            }
            catch (Exception e)
            {

            }
			
        }
        public void enter(int id)
        {
            try
            {
                userFacade.Enter(id);

            }
            catch (Exception e)
            {

            }

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
    }
}