using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using SadnaExpress.DomainLayer.Store;

namespace SadnaExpress.DomainLayer.User
{
    public class PromotedMember : Member
    {
        private ConcurrentDictionary<Guid, PromotedMember> directSupervisor;
        private ConcurrentDictionary<Guid, List<PromotedMember>> appoint;
        private readonly ConcurrentDictionary<Guid, List<string>> permissions;
        public ConcurrentDictionary<Guid, List<string>> Permission{get=>permissions;}
        private ConcurrentDictionary<Guid, List<Bid>> bidsOffers;
        private readonly Permissions permissionsHolder;

        /* permissions:
         * owner permissions
         * founder permissions
         * system manager permissions
         * edit manager permissions
         * get store history
         * add new owner
         * remove owner
         * add new manager
         * get employees info
         * product management permissions
         * policies permission
         */
        public PromotedMember(Guid id, string email, string firstName, string lastName, string password, ShoppingCart shoppingCart=null, bool login=false) : base(id,
            email, firstName, lastName, password)
        {
            directSupervisor = new ConcurrentDictionary<Guid, PromotedMember>();
            appoint = new ConcurrentDictionary<Guid, List<PromotedMember>>();
            permissions = new ConcurrentDictionary<Guid, List<string>>();
            permissionsHolder = Permissions.Instance;
            bidsOffers = new ConcurrentDictionary<Guid, List<Bid>>();
            LoggedIn = login;
            if (shoppingCart == null)
                ShoppingCart = new ShoppingCart();
            else
                ShoppingCart = shoppingCart;
        }

        public void createOwner(Guid storeID, PromotedMember directSupervisor)
        {
            this.directSupervisor.TryAdd(storeID, directSupervisor);
            permissions.TryAdd(storeID, new List<string>{"owner permissions"});
            appoint.TryAdd(storeID, new List<PromotedMember>());
        }

        public void createManager(Guid storeID, PromotedMember directSupervisor)
        {
            this.directSupervisor.TryAdd(storeID, directSupervisor);
            permissions.TryAdd(storeID, new List<string> {"get store history"});
            appoint.TryAdd(storeID, new List<PromotedMember>());
        }

        public void createFounder(Guid storeID)
        {
            permissions.TryAdd(storeID, new List<string> {"founder permissions"});
            directSupervisor.TryAdd(storeID, null);
            appoint.TryAdd(storeID, new List<PromotedMember>());
        }
        public override PromotedMember openNewStore(Guid storeID)
        {
            createFounder(storeID);
            return null;
        }
        public void createSystemManager()
        {
            List<string> permissionsList = new List<string>();
            permissionsList.Add("system manager permissions");
            permissions.TryAdd(Guid.Empty, new List<string> {"system manager permissions"});
        }
        public void addAppoint(Guid storeID, PromotedMember member)
        {
            appoint[storeID].Add(member);
        }
        public void removeAppoint(Guid storeID, PromotedMember member)
        {
            if (appoint.ContainsKey(storeID))
                appoint[storeID].Remove(member);
        }
        public void removeAllDictOfStore(Guid storeID)
        {
            List<PromotedMember> removedValue1;
            List<string> removedValue2;
            PromotedMember removedValue3;
            appoint.TryRemove(storeID, out removedValue1);
            if (permissions.ContainsKey(storeID))
            {
                while (permissions[storeID].Count != 0 )
                {
                    removePermission(storeID, permissions[storeID].First());
                }
                List<string> output = new List<string>();
                permissions.TryRemove(storeID, out output);
            }
            directSupervisor.TryRemove(storeID, out removedValue3);
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
            if (appoint.ContainsKey(storeID))
                return appoint[storeID];
            return null;
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
            
            if (per.Equals("policies permission"))
                foreach (Bid bid in bidsOffers[storeID])
                    bid.RemoveEmployee(this);
        }
        public override PromotedMember AppointStoreOwner(Guid storeID, Member newOwner)
        {
            if (hasPermissions(storeID, new List<string>{"owner permissions","founder permissions", "add new owner"}))
                return permissionsHolder.AppointStoreOwner(storeID, this, newOwner);
            throw new Exception("The member doesn’t have permissions to add new owner");
        }

        public override Tuple<List<Member>, List<Member>> RemoveStoreOwner(Guid storeID, Member storeOwner)
        {
            if (hasPermissions(storeID, new List<string>{"owner permissions","founder permissions", "remove owner"}))
                return permissionsHolder.RemoveStoreOwner(storeID, this, storeOwner);
            else
                throw new Exception($"The member doesn’t have permissions to remove {storeOwner.Email}");
        }
        
        
        public override PromotedMember AppointStoreManager(Guid storeID, Member newManager)
        {
            if (hasPermissions(storeID, new List<string>{"owner permissions","founder permissions", "add new manager"}))
                return permissionsHolder.AppointStoreManager(storeID, this, newManager);
            throw new Exception("The member doesn’t have permissions to add new manager");
        }

        public override void AddStoreManagerPermissions(Guid storeID, Member manager, string permission)
        {
            if (!hasPermissions(storeID,
                    new List<string> { "owner permissions", "founder permissions", "edit manager permissions" }))
                throw new Exception("The member doesn’t have permissions to edit manager's permissions");
            permissionsHolder.AddStoreManagerPermissions(this, storeID, manager, permission);
        }

        public override Member RemoveStoreManagerPermissions(Guid storeID, Member manager, string permission)
        {
            if (!hasPermissions(storeID,
                    new List<string> { "owner permissions", "founder permissions", "edit manager permissions" }))
                throw new Exception("The member doesn’t have permissions to edit manager's permissions");
            return permissionsHolder.RemoveStoreManagerPermissions(this, storeID, manager, permission);
        }
        public override List<PromotedMember> GetEmployeeInfoInStore(Guid storeID)
        {
            if (hasPermissions(storeID,
                    new List<string> { "owner permissions", "founder permissions", "get employees info"}))
                return permissionsHolder.GetEmployeeInfoInStore(storeID, this);
            throw new Exception("The member doesn’t have permissions to get employees info");
        }
   
        public override void CloseStore(Guid storeID)
        {
            if (!hasPermissions(storeID,
                    new List<string> {"founder permissions"}))
            throw new Exception("The member doesn’t have permissions to close store");
        }

        public void AddBid(Guid storeID, Bid bid)
        {
            if (!bidsOffers.ContainsKey(storeID))
            {
                bidsOffers[storeID] = new List<Bid>();
            }
            bidsOffers[storeID].Add(bid);
        }
        
        public void RemoveBid(Guid storeID, Bid bid)
        {
            bidsOffers[storeID].Remove(bid);
        }
        
        public override void ReactToBid(Guid storeID, string itemName, string bidResponse)
        {
            Bid bidFounded = null;
            foreach (Bid bid in bidsOffers[storeID])
                if (bid.ItemName.Equals(itemName))
                    bidFounded = bid;
            if (bidFounded == null)
                throw new Exception($"bid on {itemName} not exist");
            
            bidFounded.ReactToBid(this, bidResponse);
        }
        
        public override List<Bid> GetBidsInStore(Guid storeID)
        {
            if (bidsOffers.ContainsKey(storeID))
            {
                return bidsOffers[storeID];
            }
            throw new Exception($"Store {storeID} not exist");
        }
    }
}