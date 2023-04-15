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
            if (newOwner.hasPermissions(storeID, new List<string> {"founder permissions", "owner permissions"}))
                throw new Exception("The member is already store owner");
            PromotedMember owner = newOwner.promoteToMember();
            directSupervisor.addAppoint(storeID, owner);
            owner.createOwner(storeID, directSupervisor);
            return owner;
        }

        public PromotedMember AppointStoreManager(Guid storeID, PromotedMember directSupervisor, Member newManager)
        {
            if (newManager.hasPermissions(storeID, new List<string> {"founder permissions", "owner permissions", "get store history"}))
                throw new Exception("The member is already store manager");
            PromotedMember manager = newManager.promoteToMember();
            directSupervisor.addAppoint(storeID, manager);
            manager.createManager(storeID, directSupervisor);
            return manager;
        }

        public void AddStoreManagerPermissions(Guid storeID, Member manager, string permission)
        {
            if (manager.hasPermissions(storeID, new List<string>{"founder permissions","owner permissions", permission}))
                throw new Exception("The member has the permission");
            ((PromotedMember)manager).addPermission(storeID, permission);
        }

        public void RemoveStoreManagerPermissions(Guid storeID, Member manager, string permission)
        {
            if (!manager.hasPermissions(storeID, new List<string>{permission}))
                throw new Exception($"The member {manager.Email} dosen't have the permission");
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