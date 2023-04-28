
import axios, { AxiosRequestConfig, AxiosResponse } from 'axios';

const axiosParams = { timeout: 30000 };

export default class BaseService {

    protected getUrl(relativeUrl: string): string {       
        return `http://localhost:8080/${relativeUrl}`;
    }


    protected async get(
        url: string,
        config?: AxiosRequestConfig
    ): Promise<AxiosResponse<any>> {
        return axios.get(url, this.getConfig(config));
    }

    protected async delete(
        url: string,
        config?: AxiosRequestConfig
    ): Promise<AxiosResponse<any>> {
        return axios.delete(url, this.getConfig(config));
    }

    protected async put(
        url: string,
        data?: any,
        config?: AxiosRequestConfig
    ): Promise<AxiosResponse<any>> {
        return axios.put(url, { ...data }, this.getConfig(config));
    }

    protected async post(
        url: string,
        data?: any,
        config?: AxiosRequestConfig,
        rawData = false
    ): Promise<AxiosResponse<any>> {
        return axios.post(
            url,
            rawData ? data : { ...data },
            this.getConfig(config)
        );
    }

    private getConfig = (
        c?: AxiosRequestConfig | undefined
    ): AxiosRequestConfig => {
        let config = { ...axiosParams };
        if (hasData(c)) {
            config = {
                ...config,
                ...c,
            };
        }
        return config;
    };
}

function hasData(data: any): boolean {
    if (data === null || data === undefined) {
        return false;
    }

    if (typeof data === 'string' && data === '') {
        return false;
    }

    if (Array.isArray(data)) {
        return data.length > 0;
    }

    return true;
}
