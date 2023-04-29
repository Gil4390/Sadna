import {trackPromise} from 'react-promise-tracker';
// @ts-ignore
import {GuestService} from './guest.service.ts';

export class GuestThunks{
    private static GuestService=new GuestService();

    public static Enter = ():any => {
        return async () => {
            try{
                const respone = await trackPromise(GuestThunks.GuestService.Enter()); 
                return respone?.data;
            }
            catch(error){
               console.log(error); 
            }
        };
    };

    public static Exit = (userid:string):any => {
        return async () => {
            try{
                await GuestThunks.GuestService.Exit(userid);
            }
            catch(error){
               console.log(error); 
            }
        };
    };
    
  }