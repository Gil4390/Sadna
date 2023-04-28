
import { AxiosResponse } from 'axios';
// @ts-ignore
import BaseService from './base.service.ts';

const guestApi = 'guest';
const enter = 'enter';
const exit = 'exit';
const register = 'register';
const storeInfo='store-info';
const itemByName = "item-by-name";
const itemByCategory = "item-by-category";
const itemByKeysWord = "item-by-keys-word";
const addItemCart = "add-item-cart";
const removeItemCart = "rm-item-cart";
const editItemCart = "edit-item-cart";
const shoppingCart = "shopping-cart";
const purchaseCart = "purchase-cart";

export class GuestService extends BaseService {
    protected getUrl(relativeUrl: string): string {
        return super.getUrl(`${guestApi}/${relativeUrl}`);
    }

    public async Enter(): Promise<AxiosResponse<string>> {
        return super.get(this.getUrl(`${enter}`));
    }

    public async Exit(userid : string): Promise<AxiosResponse> {
        return super.post(this.getUrl(`${exit}`),userid);
    }

    public async Register(userid : string, email:string,firstName:string,
        lastName:string, pass:string): Promise<AxiosResponse>
     {
        return super.get(this.getUrl(`${register}`));
    }

}