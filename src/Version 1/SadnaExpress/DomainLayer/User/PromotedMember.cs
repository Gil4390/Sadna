using System;
using System.Collections.Generic;

namespace SadnaExpress.DomainLayer.User
{
    public class PromotedMember : Member
    {
        private Dictionary<Guid, Member> directSupervisor;
        private Dictionary<Guid, LinkedList<Member>> appoint;
        private readonly Dictionary<Guid, LinkedList<string>> permissions;
        private readonly Permissions permissionsHolder;
        
        /* permissions:
         * owner permissions
         * founder permissions
         * system manager permissions
         * edit manager permissions
         * get store history
         * add new owner
         * add new manager
         */
        public PromotedMember(int id, string email, string firstName, string lastName, string password):base(id, email, firstName, lastName, password) {
            directSupervisor = new Dictionary<Guid,Member>();
            appoint = new Dictionary<Guid, LinkedList<Member>>();
            permissions = new Dictionary<Guid, LinkedList<string>>();
            permissionsHolder = Permissions.Instance;
        }

        public void createOwner(Guid storeID, Member directSupervisor)
        {
            this.directSupervisor.Add(storeID, directSupervisor);
            LinkedList<string> permissionsList = new LinkedList<string>();
            permissionsList.AddLast("owner permissions");
            permissions.Add(storeID, permissionsList);
        }

        public void createManager(Guid storeID, Member directSupervisor)
        {
            this.directSupervisor.Add(storeID, directSupervisor);
            LinkedList<string> permissionsList = new LinkedList<string>();
            permissionsList.AddLast("get store history");
            permissions.Add(storeID, permissionsList);
        }

        public void createFounder(Guid storeID)
        {
            LinkedList<string> permissionsList = new LinkedList<string>();
            permissionsList.AddLast("founder permissions");
            permissions.Add(storeID, permissionsList);
        }
        
        public void createSystemManager()
        {
            LinkedList<string> permissionsList = new LinkedList<string>();
            permissionsList.AddLast("system manager permissions");
            permissions.Add(Guid.Empty, permissionsList); 
        }

        public void addAppoint(Guid storeID, Member member)
        {
            if (appoint.ContainsKey(storeID))
                appoint[storeID].AddLast(member);
            else
            {
                LinkedList<Member> appointList = new LinkedList<Member>();
                appointList.AddLast(member);
                appoint.Add(storeID, appointList);
            } 
        }

        public override bool hasPermissions(Guid storeID, LinkedList<string> listOfPermissions)
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

        public override PromotedMember AppointStoreOwner(Guid storeID, Member newOwner)
        {
            if (permissions.ContainsKey(storeID))
                if (permissions[storeID].Contains("owner permissions") ||
                    permissions[storeID].Contains("founder permissions")||
                    permissions[storeID].Contains("add new owner"))
                    return permissionsHolder.AppointStoreOwner(storeID, this, newOwner);
            throw new Exception("The member doesn’t have permissions to add new owner");
        } 
        public override PromotedMember AppointStoreManager(Guid storeID, Member newManager)
        {
            if (permissions.ContainsKey(storeID))
                if (permissions[storeID].Contains("owner permissions") ||
                    permissions[storeID].Contains("founder permissions") ||
                    permissions[storeID].Contains("add new manager"))
                    return permissionsHolder.AppointStoreManager(storeID, this, newManager);
            throw new Exception("The member doesn’t have permissions to add new manager");
        } 
        public override void AddStoreManagerPermissions(Guid storeID, Member manager, string permission)
        {
            if (permissions.ContainsKey(storeID))
                if (permissions[storeID].Contains("owner permissions") ||
                    permissions[storeID].Contains("founder permissions") ||
                    permissions[storeID].Contains("edit manager permissions"))
                {
                    permissionsHolder.AddStoreManagerPermissions(storeID, manager, permission);
                    permissions[storeID].AddLast(permission);
                }
            throw new Exception("The member doesn’t have permissions to edit manager's permissions");
        }
        public override void RemoveStoreManagerPermissions(Guid storeID, Member manager, string permission)
        {
            if (permissions.ContainsKey(storeID))
                if (permissions[storeID].Contains("owner permissions") ||
                    permissions[storeID].Contains("founder permissions") ||
                    permissions[storeID].Contains("edit manager permissions"))
                {
                    permissionsHolder.RemoveStoreManagerPermissions(storeID, manager, permission);
                    permissions[storeID].Remove(permission);
                }
            throw new Exception("The member doesn’t have permissions to edit manager's permissions");
        }
    }
}