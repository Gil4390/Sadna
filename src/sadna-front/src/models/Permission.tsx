export interface PermissionPerStore { 
  [key: string]:Permission[]
};

export interface Permission { 
  owner: boolean,
  founder: boolean,
  system_manager: boolean,
  edit_manager: boolean,
  get_store_history: boolean,
  add_new_owner : boolean,
  remove_owner : boolean,
  add_new_manager : boolean,
  get_employees_info : boolean,
  product_management : boolean,
  policies_permission : boolean,
};

