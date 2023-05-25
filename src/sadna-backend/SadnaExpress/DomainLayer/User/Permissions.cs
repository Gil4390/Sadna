using SadnaExpress.DataLayer;
using SadnaExpress.DomainLayer.Store;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Exception = System.Exception;

namespace SadnaExpress.DomainLayer.User
{
    // todo
    [NotMapped]
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
        
        public PromotedMember AppointStoreOwner(Guid storeID, PromotedMember directSupervisor, Member newOwner)
        {
            if (newOwner.hasPermissions(storeID, new List<string> { "founder permissions", "owner permissions" }))
                throw new Exception("The member is already store owner");

            PromotedMember owner = newOwner.promoteToMember();
            owner.LoggedIn = newOwner.LoggedIn;
            directSupervisor.addAppoint(storeID, owner);
            owner.createOwner(storeID, directSupervisor);
            return owner;
        }

        public Tuple<List<Member>, List<Member>> RemoveStoreOwner(Guid storeID,PromotedMember directOwner, Member member)
        {
            if (!member.hasPermissions(storeID, new List<string> {"owner permissions"}))
                throw new Exception($"The member {member.Email} isn't owner");
            PromotedMember storeOwner = ((PromotedMember)member);
            
            if(!storeOwner.getDirectManager(storeID).Email.ToLower().Equals(directOwner.Email.ToLower()))
                throw new Exception($"{directOwner.Email} isn't the direct owner of {storeOwner.Email}");
            //if(storeOwner.getDirectManager(storeID) != directOwner) // check by email it is better with database
            //    throw new Exception($"{directOwner.Email} isn't the direct owner of {storeOwner.Email}");

            // remove the appoints
            Stack<PromotedMember> stack = new Stack<PromotedMember>();
            List<Member> regMembers = new List<Member>();
            List<Member> NotOwners = new List<Member>();
            stack.Push(storeOwner);

            while (stack.Count > 0)
            {
                PromotedMember current = stack.Pop();

                if (current.getAppoint(storeID) != null)
                {
                    foreach (PromotedMember child in current.getAppoint(storeID))
                        stack.Push(child);
                }

                NotOwners.Add(current);
                current.removeAllDictOfStore(storeID);
                if (current.Permission.Count == 0)
                {
                    var newMember = new Member(current);
                    regMembers.Add(newMember);
                    DBHandler.Instance.DowngradePromotedMemberToReg(newMember);
                }
            }
            //remove the owner from appoint
            directOwner.removeAppoint(storeID, storeOwner);
            Tuple<List<Member>, List<Member>> result = new Tuple<List<Member>, List<Member>>(regMembers, NotOwners);
            return result;
        }
        
        public PromotedMember AppointStoreManager(Guid storeID, PromotedMember directSupervisor, Member newManager)
        {
            if (newManager.hasPermissions(storeID, new List<string> {"founder permissions", "owner permissions", "get store history"}))
                throw new Exception("The member is already store manager");
            
            PromotedMember manager = newManager.promoteToMember();
            manager.LoggedIn = newManager.LoggedIn;
            directSupervisor.addAppoint(storeID, manager);
            manager.createManager(storeID, directSupervisor);
            return manager;
        }

        public void AddStoreManagerPermissions(PromotedMember appointer, Guid storeID, Member manager, string permission)
        {
            lock (manager)
            {
                PromotedMember pmember = (PromotedMember)manager;
                if (manager.hasPermissions(storeID,
                        new List<string> { "founder permissions", "owner permissions", permission }))
                    throw new Exception("The member already has the permission");
                Guid directManagerID = pmember.getDirectManager(storeID).UserId;
                if (!directManagerID.Equals(appointer.UserId))
                    throw new Exception("The caller is not the appointer of the manager");
                if (permission == "owner permissions")
                {
                    pmember.Permission[storeID] = new List<string>();
                    NotificationSystem.Instance.RegisterObserver(storeID, manager);
                }
                pmember.addPermission(storeID, permission);
            }
        }

        public Member RemoveStoreManagerPermissions(PromotedMember directManager, Guid storeID, Member manager, string permission)
        {
            lock (manager)
            {
                if (!manager.hasPermissions(storeID, new List<string> { permission }))
                    throw new Exception($"The member {manager.Email} dosen't have the permission");
            }
            PromotedMember promanager = (PromotedMember)manager;
            promanager.removePermission(storeID, permission);
            if (promanager.Permission.Count == 0)
            {
                directManager.removeAppoint(storeID, promanager);
                return new Member(promanager);
            }
            return null;
        }

        public List<PromotedMember> GetEmployeeInfoInStore(Guid storeID, PromotedMember member)
        {
            List<PromotedMember> employees = new List<PromotedMember>();
            Stack<PromotedMember> stack = new Stack<PromotedMember>();
            stack.Push(member);
            try
            {
                while (stack.Count > 0)
                {
                    PromotedMember current = stack.Pop();
                    employees.Add(current);

                    PromotedMember directManager = current.getDirectManager(storeID);
                    if (directManager != null && !employees.Contains(directManager))
                        stack.Push(directManager);

                    foreach (PromotedMember child in current.getAppoint(storeID))
                        if (!employees.Contains(child))
                            stack.Push(child);
                }
            }
            catch(Exception ex)
            {

            }
            return employees;
        }

        
    }
}