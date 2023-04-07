using System;
using System.Collections.Generic;
using Exception = System.Exception;

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
        
        public PromotedMember addNewOwner(Guid storeID, PromotedMember directSupervisor, Member newOwner)
        {
            LinkedList<string> relevantPermissions = new LinkedList<string>();
            relevantPermissions.AddLast("owner permissions");
            relevantPermissions.AddLast("founder permissions");
            relevantPermissions.AddLast("add new owner");
            if (newOwner.hasPermissions(storeID, relevantPermissions))
                throw new Exception("The member is already store owner");
            PromotedMember owner = newOwner.promoteToMember();
            owner.createOwner(storeID, directSupervisor);
            return owner;
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