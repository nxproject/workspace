import { jsPlumbInstance } from "jsplumb";
export declare class JsPlumbUtils {
    static createInstance: (container: any, readonly: boolean) => jsPlumbInstance;
    static createEndpointUuid: (activityId: string, outcome: string) => string;
    static colorEndpoint: (label: string) => any;
    static getSourceEndpointOptions: (activityId: string, outcome: string, executed: boolean) => any;
}
