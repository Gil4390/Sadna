using SadnaExpress.DomainLayer.Store;
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

        public void RemoveStoreOwner(Guid storeID,PromotedMember directOwner, Member member)
        {
            if (member.hasPermissions(storeID, new List<string> {"owner permissions" }))
                throw new Exception($"The member {member.Email} isn't owner");
            PromotedMember storeOwner = ((PromotedMember)member);
            if(storeOwner.getDirectManager(storeID) != directOwner)
                throw new Exception($"{directOwner.Email} isn't the direct owner of {storeOwner.Email}");
            // remove the appoints
            Stack<PromotedMember> stack = new Stack<PromotedMember>();
            stack.Push(storeOwner);

            while (stack.Count > 0)
            {
                PromotedMember current = stack.Pop();
                current.removeAllDictOfStore(storeID);
                
                foreach (PromotedMember child in current.getAppoint(storeID))
                    stack.Push(child);
            }
            //remove the owner from appoint
            directOwner.removeAppoint(storeID, storeOwner);
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

                pmember.addPermission(storeID, permission);
            }
        }

        public void RemoveStoreManagerPermissions(Guid storeID, Member manager, string permission)
        {
            lock (manager)
            {
                if (!manager.hasPermissions(storeID, new List<string> { permission }))
                    throw new Exception($"The member {manager.Email} dosen't have the permission");
            }
            ((PromotedMember)manager).removePermission(storeID, permission);
        }

        public void closeStore(int storeID)
        {
            throw new NotImplementedException();
            //in the next version should send a message to all the employees
        }

        public List<PromotedMember> GetEmployeeInfoInStore(Guid storeID, PromotedMember member)
        {
            List<PromotedMember> employees = new List<PromotedMember>();
            Stack<PromotedMember> stack = new Stack<PromotedMember>();
            stack.Push(member);

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

            foreach (PromotedMember employee in employees)
                Console.WriteLine($"{employee.Email}: \n + {employee.FirstName} {employee.LastName} \n");
            return employees;
        }
    }
}