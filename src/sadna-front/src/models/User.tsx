export interface Member {
  id: string, 
  email: string,
  firstName: string,
  lastName: string,
  loggedIn: boolean,
  permissions: string[]
};