// @ts-ignore
import {AdminService} from './admin.service.ts';
import {trackPromise} from 'react-promise-tracker';

export class AdminThunks{
    private static adminService=new AdminService();

    public static IsSystemInitialize = ():any => {
        return async () => {
            try{
                const respone = await trackPromise(AdminThunks.adminService.IsSystemInitialize());
                return respone?.data;
            }
            catch(error){
               console.log(error); 
            }
        };
    };
    
  }