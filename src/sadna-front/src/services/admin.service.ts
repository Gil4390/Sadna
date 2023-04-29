
import { AxiosResponse } from 'axios';
// @ts-ignore
import BaseService from './base.service.ts';

const adminApi = 'admin';
const isInit = 'is-system-init';


export class AdminService extends BaseService {
    protected getUrl(relativeUrl: string): string {
        return super.getUrl(`${adminApi}/${relativeUrl}`);
    }

    public async IsSystemInitialize(): Promise<AxiosResponse<boolean>> {
        return super.get(this.getUrl(`${isInit}`));
    }

}