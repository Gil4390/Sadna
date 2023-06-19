using SadnaExpress.DomainLayer.Store;
using SadnaExpress.DomainLayer.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaExpress.ServiceLayer.SModels
{

    /* permissions:
     * owner permissions
     * founder permissions
     * system manager permissions
     * edit manager permissions
     * get store history
     * 
     * add new owner
     * remove owner
     * add new manager
     * get employees info
     * product management permissions
     */

    public class SPermission
    {
        private bool owner = false;
        private bool founder = false;
        private bool system_manager = false;
        private bool edit_manager = false;
        private bool get_store_history = false;
        private bool add_new_manager = false;
        private bool get_employees_info = false;
        private bool product_management = false;
        private bool policies_permission = false;

        public SPermission(List<string> permissions)
        {
            foreach (string p in permissions)
            {
                switch(p) {
                    case "owner permissions":
                        owner = true;
                        break;
                    case "founder permissions":
                        founder = true;
                        break;
                    case "edit manager permissions":
                        edit_manager = true;
                        break;
                    case "system manager permissions":
                        system_manager = true;
                        break;
                    case "get store history":
                        get_store_history = true;
                        break;
                    case "add new manager":
                        add_new_manager = true;
                        break;
                    case "get employees info":
                        get_employees_info = true;
                        break;
                    case "product management permissions":
                        product_management = true;
                        break;
                    case "policies permission":
                        policies_permission = true;
                        break;
                }

                if (founder || owner)
                {
                    edit_manager = true;
                    get_store_history = true;
                    add_new_manager = true;
                    get_employees_info = true;
                    product_management = true;
                    policies_permission = true;
                }
            }
        }

        public bool Get_store_history { get => get_store_history; set => get_store_history = value; }
        public bool Owner { get => owner; set => owner = value; }
        public bool Founder { get => founder; set => founder = value; }
        public bool Edit_manager { get => edit_manager; set => edit_manager = value; }
        public bool Add_new_manager { get => add_new_manager; set => add_new_manager = value; }
        public bool Get_employees_info { get => get_employees_info; set => get_employees_info = value; }
        public bool Product_management { get => product_management; set => product_management = value; }
        public bool System_manager { get => system_manager; set => system_manager = value; }
        public bool Policies_permission { get => policies_permission; set => policies_permission = value; }

    }
}
