using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SadnaExpress.DomainLayer.User
{
    public class PromotedMember : Member
    {
        private ConcurrentDictionary<Guid, PromotedMember> directSupervisor;
        private ConcurrentDictionary<Guid, List<PromotedMember>> appoint;
        private readonly ConcurrentDictionary<Guid, List<string>> permissions;
        private readonly Permissions permissionsHolder;

        /* permissions:
         * owner permissions
         * founder permissions
         * system manager permissions
         * edit manager permissions
         * get store history
         * add new owner
         * add new manager
         * get employees info
         */
        public PromotedMember(Guid id, string email, string firstName, string lastName, string password) : base(id,
            email, firstName, lastName, password)
        {
            directSupervisor = new ConcurrentDictionary<Guid, PromotedMember>();
            appoint = new ConcurrentDictionary<Guid, List<PromotedMember>>();
            permissions = new ConcurrentDictionary<Guid, List<string>>();
            permissionsHolder = Permissions.Instance;
        }

        public void createOwner(Guid storeID, PromotedMember directSupervisor)
        {
            this.directSupervisor.TryAdd(storeID, directSupervisor);
            List<string> permissionsList = new List<string>();
            List<PromotedMember> appointList = new List<PromotedMember>();
            permissionsList.Add("owner permissions");
            permissions.TryAdd(storeID, permissionsList);
            appoint.TryAdd(storeID, appointList);
        }

        public void createManager(Guid storeID, PromotedMember directSupervisor)
        {
            this.directSupervisor.TryAdd(storeID, directSupervisor);
            List<string> permissionsList = new List<string>();
            List<PromotedMember> appointList = new List<PromotedMember>();
            permissionsList.Add("get store history");
            permissions.TryAdd(storeID, permissionsList);
            appoint.TryAdd(storeID, appointList);
        }

        public void createFounder(Guid storeID)
        {
            List<string> permissionsList = new List<string>();
            List<PromotedMember> appointList = new List<PromotedMember>();
            permissionsList.Add("founder permissions");
            permissions.TryAdd(storeID, permissionsList);
            directSupervisor.TryAdd(storeID, null);
            appoint.TryAdd(storeID, appointList);
        }

        public void createSystemManager()
        {
            List<string> permissionsList = new List<string>();
            permissionsList.Add("system manager permissions");
            permissions.TryAdd(Guid.Empty, permissionsList);
        }

        public void addAppoint(Guid storeID, PromotedMember member)
        {
            appoint[storeID].Add(member);
        }

        public override bool hasPermissions(Guid storeID, List<string> listOfPermissions)
        {
            if (permissions.ContainsKey(storeID))
            {
                foreach (string permission in listOfPermissions)
                {
                    if (permissions[storeID].Contains(permission))
                        return true;
                }
            }
            return false;
        }
        public List<PromotedMember> getAppoint(Guid storeID)
        {
            return appoint[storeID];
        }
        public PromotedMember getDirectManager(Guid storeID)
        {
            return directSupervisor[storeID];
        }
        public void addPermission(Guid storeID, string per)
        {
            permissions[storeID].Add(per);
        }
        public void removePermission(Guid storeID, string per)
        {
            permissions[storeID].Remove(per);
        }
        public override PromotedMember AppointStoreOwner(Guid storeID, Member newOwner)
        {
            if (permissions.ContainsKey(storeID))
                if (permissions[storeID].Contains("owner permissions") ||
                    permissions[storeID].Contains("founder permissions") ||
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
                    return;
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
                    return;
                }
            throw new Exception("The member doesn’t have permissions to edit manager's permissions");
        }
        public override List<PromotedMember> GetEmployeeInfoInStore(Guid storeID)
        {
            if (permissions[storeID].Contains("owner permissions") ||
                permissions[storeID].Contains("founder permissions") ||
                permissions[storeID].Contains("get employees info"))
                return permissionsHolder.GetEmployeeInfoInStore(storeID, this);
            throw new Exception("The member doesn’t have permissions to get employees info");
        }
    }
}