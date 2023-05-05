export interface User  {
  id: string, 
};

export interface Member extends User {
  firstName: string,
  lastName: string,
  email: string,
  loggedIn: boolean,
  permissions: string[]
};