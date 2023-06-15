using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using System.Linq;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.DataLayer;

namespace SadnaExpress.DomainLayer.User
{
    public class PromotedMember : Member
    {
        #region properties

        #region directSupervisor
        private ConcurrentDictionary<Guid, PromotedMember> directSupervisor;
        [NotMapped]
        public ConcurrentDictionary<Guid, PromotedMember> DirectSupervisor { get => directSupervisor; set => directSupervisor = value; }

        [NotMapped]
        public ConcurrentDictionary<Guid, string> directSupervisorHelper
        {
            get
            {
                ConcurrentDictionary<Guid, string> directSupervisorH = new ConcurrentDictionary<Guid, string>();
                if (directSupervisor != null)
                    foreach (Guid id in directSupervisor.Keys)
                    {
                        if (directSupervisor[id] == null)
                        {
                            directSupervisorH.TryAdd(id, "");
                        }
                        else
                            directSupervisorH.TryAdd(id, directSupervisor[id].Email);
                    }
                return directSupervisorH;
            }
            set
            {

            }
        }

        [NotMapped]
        public string DirectSupervisorJson
        {
            get => JsonConvert.SerializeObject(directSupervisorHelper);
            set => directSupervisorHelper = JsonConvert.DeserializeObject<ConcurrentDictionary<Guid, string>>(value);
        }

        public string DirectSupervisorDB
        {
            get;
            set;
        }
        #endregion

        #region appoint
        private ConcurrentDictionary<Guid, List<PromotedMember>> appoint;
        [NotMapped]
        public ConcurrentDictionary<Guid, List<PromotedMember>> Appoint { get => appoint; set => appoint = value; }

        [NotMapped]
        public ConcurrentDictionary<Guid, List<string>> appointHelper
        {
            get
            {
                ConcurrentDictionary<Guid, List<string>> appointH = new ConcurrentDictionary<Guid, List<string>>();
                if (appoint != null)
                    foreach (Guid id in appoint.Keys)
                    {
                        List<string> listTostring = new List<string>();
                        foreach (PromotedMember pm in appoint[id])
                            listTostring.Add(pm.Email);
                        appointH.TryAdd(id, listTostring);
                    }
                return appointH;
            }
            set
            {

            }
        }
        [NotMapped]
        public string AppointJson
        {
            get => JsonConvert.SerializeObject(appointHelper);
            set => appointHelper = JsonConvert.DeserializeObject<ConcurrentDictionary<Guid, List<string>>>(value);
        }

        public string AppointDB
        {
            get; set;
        }
        #endregion

        #region permissions
        private ConcurrentDictionary<Guid, List<string>> permissions;
        [NotMapped]
        public ConcurrentDictionary<Guid, List<string>> Permission{ get => permissions; set => Permission = value; }

        public string PermissionDB
        {
            get => JsonConvert.SerializeObject(permissions);
            set => permissions = JsonConvert.DeserializeObject<ConcurrentDictionary<Guid, List<string>>>(value);
        }
        #endregion

        #region bidsOffers
        private ConcurrentDictionary<Guid, List<Bid>> bidsOffers;
        [NotMapped]
        public ConcurrentDictionary<Guid, List<Bid>> BidsOffers { get => bidsOffers; set => bidsOffers = value; }

        [NotMapped]
        public string BidsOffersJson
        {
            get
            {
                ConcurrentDictionary<Guid, List<Guid>> helper = new ConcurrentDictionary<Guid, List<Guid>>();
                if (BidsOffers != null)
                    foreach (Guid id in BidsOffers.Keys)
                    {
                        List<Guid> bidsList = new List<Guid>();
                        foreach (Bid bid in BidsOffers[id])
                            bidsList.Add(bid.BidId);
                        helper.TryAdd(id, bidsList);
                    }
                return JsonConvert.SerializeObject(helper);
            }
            set
            {

            }
        }

        public string BidsOffersDB
        {
            get; set;
        }

        #endregion

        #region permissionsOffers
        private ConcurrentDictionary<Guid, List<Guid>> permissionsOffers;

        [NotMapped]
        public ConcurrentDictionary<Guid, List<Guid>> PermissionsOffers { get => permissionsOffers; set => permissionsOffers = value; }

        [NotMapped]
        public string PermissionsOffersJson
        {
            get
            {
                return JsonConvert.SerializeObject(PermissionsOffers);
            }
            set
            {

            }
        }

        public string PermissionsOffersDB
        {
            get; set;
        }
        #endregion

        private readonly Permissions permissionsHolder;

        #endregion

        public PromotedMember()
        {
            permissionsHolder = Permissions.Instance;
        }

        #region permissions kind
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
        #endregion

        #region constructor
        public PromotedMember(Guid id, string email, string firstName, string lastName, string password, ShoppingCart shoppingCart=null, bool login=false) : base(id,
            email, firstName, lastName, password)
        {
            directSupervisor = new ConcurrentDictionary<Guid, PromotedMember>();
            appoint = new ConcurrentDictionary<Guid, List<PromotedMember>>();
            permissions = new ConcurrentDictionary<Guid, List<string>>();
            permissionsHolder = Permissions.Instance;
            bidsOffers = new ConcurrentDictionary<Guid, List<Bid>>();
            permissionsOffers = new ConcurrentDictionary<Guid, List<Guid>>();
            LoggedIn = login;
            if (shoppingCart == null)
                ShoppingCart = new ShoppingCart();
            else
                ShoppingCart = shoppingCart;
        }
        #endregion

        #region PromotedMember actions
        public void createOwner(Guid storeID, PromotedMember directSupervisor)
        {
            this.directSupervisor.TryAdd(storeID, directSupervisor);
            permissions.TryAdd(storeID, new List<string>{"owner permissions"});
            appoint.TryAdd(storeID, new List<PromotedMember>());
            //here
            DBHandler.Instance.UpdatePromotedMember(this);
        }

        public void createManager(Guid storeID, PromotedMember directSupervisor)
        {
            this.directSupervisor.TryAdd(storeID, directSupervisor);
            permissions.TryAdd(storeID, new List<string> {"get store history"});
            appoint.TryAdd(storeID, new List<PromotedMember>());
            //here
            DBHandler.Instance.UpgradeMemberToPromotedMember(this);
        }

        public void createFounder(Guid storeID)
        {
            permissions.TryAdd(storeID, new List<string> {"founder permissions"});
            directSupervisor.TryAdd(storeID, null);
            appoint.TryAdd(storeID, new List<PromotedMember>());
            //here
            DBHandler.Instance.UpgradeMemberToPromotedMember(this);

        }
        
        public override PromotedMember openNewStore(Guid storeID)
        {
            createFounder(storeID);
            return this;
        }
        
        public void createSystemManager()
        {
            List<string> permissionsList = new List<string>();
            permissionsList.Add("system manager permissions");
            permissions.TryAdd(Guid.Empty, new List<string> {"system manager permissions"});
            //here
            DBHandler.Instance.UpgradeMemberToPromotedMember(this);
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

        public override PromotedMember AppointStoreOwner(Guid storeID, Member newOwner)
        {
            if (hasPermissions(storeID, new List<string>{"owner permissions","founder permissions", "add new owner"}))
                return permissionsHolder.AppointStoreOwner(storeID, this, newOwner);
            throw new Exception("The member doesn’t have permissions to add new owner");
        }

        public override PromotedMember ReactToJobOffer(Guid storeID, Member newEmp, bool offerResponse)
        {
            if (hasPermissions(storeID, new List<string> { "owner permissions", "founder permissions", "add new owner" }))
                return permissionsHolder.ReactToJobOffer(storeID, this, newEmp, offerResponse);
            throw new Exception("The member doesn’t have permissions to add new owner");
        }

        public override Tuple<List<Member>, List<Member>, HashSet<Guid>> RemoveStoreOwner(Guid storeID, Member storeOwner)
        {
            if (hasPermissions(storeID, new List<string>{"owner permissions","founder permissions", "remove owner"}))
                return permissionsHolder.RemoveStoreOwner(storeID, this, storeOwner);
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
                    new List<string> { "owner permissions", "founder permissions", "get employees info" }))
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

            if (bid.User.GetType() != typeof(User)) 
                DBHandler.Instance.UpdatePromotedMember(this);
        }

        public void RemoveBid(Guid storeID, Bid bid)
        {
            bidsOffers[storeID].Remove(bid);
            if (bid.User.GetType() != typeof(User)) 
                DBHandler.Instance.UpdatePromotedMember(this);
        }
        
        public override void ReactToBid(Guid storeID,  Guid bid, string bidResponse)
        {
            Bid bidFounded = null;
            foreach (Bid bidd in bidsOffers[storeID])
                if (bidd.BidId.Equals(bid))
                    bidFounded = bidd;
            if (bidFounded == null)
                throw new Exception($"bid not exist");
            
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

        public override PromotedMember promoteToMember()
        {
            return this;
        }

        #endregion

        
        #region HelpFunc

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
            // here
            DBHandler.Instance.UpdatePromotedMember(this);
        }
        
        public void addAppoint(Guid storeID, PromotedMember member)
        {
            if (appoint.ContainsKey(storeID))
                appoint.TryAdd(storeID, new List<PromotedMember>());
            appoint[storeID].Add(member);
            //here
            DBHandler.Instance.UpdatePromotedMember(this);
        }
        
        public void removeAppoint(Guid storeID, PromotedMember member)
        {
            if (appoint.ContainsKey(storeID))
                appoint[storeID].Remove(member);
            //here
            DBHandler.Instance.UpdatePromotedMember(this);
        }
        
        public void removePermission(Guid storeID, string per)
        {
            permissions[storeID].Remove(per);
            
            if (per.Equals("policies permission")||per.Equals("owner permissions"))
                if (bidsOffers.ContainsKey(storeID))
                    foreach (Bid bid in bidsOffers[storeID])
                        bid.RemoveEmployee(this);
        }
        
        public void removeAllDictOfStore(Guid storeID)
        {
            List<PromotedMember> removedValue1;
            List<string> removedValue2;
            PromotedMember removedValue3;
            List<Bid> removedValue4;
            List<Guid> removedValue5;
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
            bidsOffers.TryRemove(storeID, out removedValue4);
            permissionsOffers.TryRemove(storeID, out removedValue5);
        }

        #endregion

        public override bool Equals(object obj) 
        {
            PromotedMember newPm = (PromotedMember)obj;

            if (this.userId == newPm.userId && this.Email == newPm.Email)
                return true;

            return false;
        }


        public override string GetRole()
        {
            //here we look for the "highst" permissions 

            string role = "";
            if (PermissionDB.Contains("system manager permissions"))
            {
                role = "system manager";
            }
            else if (PermissionDB.Contains("founder permissions") || PermissionDB.Contains("owner permissions"))
            {
                role += "founder or owner";
            }
            else if (PermissionDB.Contains("edit manager permissions") || PermissionDB.Contains("get store history") ||
                    PermissionDB.Contains("add new owner") || PermissionDB.Contains("remove owner") ||
                    PermissionDB.Contains("add new manager") || PermissionDB.Contains("get employees info") ||
                    PermissionDB.Contains("product management permissions") || PermissionDB.Contains("policies permission"))
            {
                role += "store manager";
            }

            return role;
        }

    }
}