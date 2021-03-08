import { WorkflowPlugin } from "../models";
import { ActivityDefinition } from "../models";
export declare class WorkflowActivities implements WorkflowPlugin {
    private static readonly Category;
    getName: () => string;
    getActivityDefinitions: () => ActivityDefinition[];
    private do;
    private wait;
}
