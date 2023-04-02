using System;
using ConsoleApp1.DomainLayer;
using SadnaExpress.DomainLayer.User;

namespace SadnaExpress.ServiceLayer
{
	public class Service
	{
		private IStoreFacade storeFacade;
		private IUserFacade userFacade;
        public Service()
		{
			storeFacade = new StoreFacade();
			userFacade = new UserFacade();

        }

		public void register(int id , string email , string firstName ,  string lastName , string password)
		{
			try
			{
				userFacade.register(id ,email, firstName, lastName, password);

            }
			catch (Exception e)
			{

			}
			
		}
        public void enter(int id)
        {
            try
            {
                userFacade.enter(id);

            }
            catch (Exception e)
            {

            }

        }
        public void exit(int id)
        {
            try
            {
                userFacade.exit(id);

            }
            catch (Exception e)
            {

            }

        }
    }
}

