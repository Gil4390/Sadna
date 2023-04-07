using System;
using System.Collections.Generic;

namespace SadnaExpress.DomainLayer.User
{
    public class PromotedMember : Member
    {
        private Dictionary<int, Member> directSupervisor;
        private Dictionary<int, LinkedList<Member>> appoint;
        private readonly Dictionary<int, LinkedList<string>> permissions;
        private readonly Permissions permissionsHolder;
        
        public PromotedMember(int id, string email, string firstName, string lastName, string password):base(id, email, firstName, lastName, password) {
            directSupervisor = new Dictionary<int,Member>();
            appoint = new Dictionary<int, LinkedList<Member>>();
            permissions = new Dictionary<int, LinkedList<string>>();
            permissionsHolder = Permissions.Instance;
        }

        public void createOwner(int storeID, Member directSupervisor)
        {
            this.directSupervisor.Add(storeID, directSupervisor);
            LinkedList<string> permissionsList = new LinkedList<string>();
            permissionsList.AddLast("owner permissions");
            permissions.Add(storeID, permissionsList);
        }

        public void createManager(int storeID, Member directSupervisor)
        {
            this.directSupervisor.Add(storeID, directSupervisor);
            LinkedList<string> permissionsList = new LinkedList<string>();
            permissionsList.AddLast("get store history");
            permissions.Add(storeID, permissionsList);
        }

        public void createFounder(int storeID)
        {
            LinkedList<string> permissionsList = new LinkedList<string>();
            permissionsList.AddLast("founder permissions");
            permissions.Add(storeID, permissionsList);
        }
        
        public void createSystemManager()
        {
            LinkedList<string> permissionsList = new LinkedList<string>();
            permissionsList.AddLast("system manager permissions");
            permissions.Add(-1, permissionsList); //-1 represent all stores
        }

        public override bool hasPermissions(int storeID, LinkedList<string> listOfPermissions)
        {
            if (permissions.ContainsKey(storeID))
            {
                foreach (string permission in listOfPermissions)
                {
                    if (!permissions[storeID].Contains(permission))
                        return false;
                }

                return true;
            }
            return false;
        }
        
        
        public override PromotedMember addNewOwner(int storeID, Member newOwner)
        {
            if (permissions.ContainsKey(storeID))
                if (permissions[storeID].Contains("owner permissions") ||
                    permissions[storeID].Contains("founder permissions"))
                    return permissionsHolder.addNewOwner(storeID, this, newOwner);
            throw new Exception("The member doesn’t have permissions to add new owner");
        } 
    }
}