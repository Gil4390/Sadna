using System;
using System.Collections.Generic;

namespace SadnaExpress.DomainLayer.User
{
    public class Permissions
    {
        private static readonly object syncRoot = new object();
        private static Permissions instance = null;
        private Permissions(){}

        public static Permissions Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new Permissions();
                        }
                    }
                }
                return instance;
            }
        }
        
        public PromotedMember addNewOwner(int storeID, PromotedMember directSupervisor, Member newOwner)
        {
            throw new NotImplementedException();
        }

        public void addNewManager(int storeID, Member directSupervisor, Member newManager)
        {
            throw new NotImplementedException();
        }

        public void addPermissions(int storeID, Member manager, string newPermission)
        {
            throw new NotImplementedException();
        }
        
        public void deletePermissions(int storeID, Member manager)
        {
            throw new NotImplementedException();
        }

        public void closeStore(int storeID)
        {
            throw new NotImplementedException();
        }

        public void getInfoOnStore(int storeID)
        {
            throw new NotImplementedException();
        }
    }
}