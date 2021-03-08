import { WorkflowPlugin } from "../models";
import { ActivityDefinition } from "../models";
export declare class WorkflowActivities implements WorkflowPlugin {
    private static readonly Category;
    getName: () => string;
    getActivityDefinitions: () => ActivityDefinition[];
    private call;
    private delay;
    private end;
    private fork;
    private ifelse;
    private join;
    private switch;
    private task;
    private continueon;
    private onerror;
    private onoverdue;
    private do;
    private storeuse;
    private set;
}
