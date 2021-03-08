import { EventEmitter } from '../../../stencil.core';
import { jsPlumbInstance } from "jsplumb";
import { Activity, ActivityDefinition, Point, Workflow } from "../../../models";
export declare class Designer {
    canvas: HTMLElement;
    constructor();
    private el;
    canvasHeight: string;
    activityDefinitions: Array<ActivityDefinition>;
    readonly: boolean;
    workflow: Workflow;
    onWorkflowChanged(value: Workflow): void;
    newWorkflow(): Promise<void>;
    getWorkflow(): Promise<any>;
    addActivity(activityDefinition: ActivityDefinition): Promise<void>;
    updateActivity(activity: Activity): Promise<void>;
    private editActivityEvent;
    private addActivityEvent;
    workflowChanged: EventEmitter;
    jsPlumb: jsPlumbInstance;
    lastClickedLocation: Point;
    activityContextMenu: HTMLWfContextMenuElement;
    selectedActivity: Activity;
    private elem;
    componentWillLoad(): void;
    componentDidRender(): void;
    render(): any;
    private renderContextMenu;
    private deleteActivity;
    private createActivityModels;
    private setupJsPlumb;
    private setupPopovers;
    private setupActivityElement;
    private setupDragDrop;
    private setupTargets;
    private setupOutcomes;
    private setupConnections;
    private getActivityElements;
    private static getActivityId;
    private findActivityByElement;
    private findActivityById;
    private updateActivityInternal;
    private setupJsPlumbEventHandlers;
    private connectionCreated;
    private connectionDetached;
    private onEditActivity;
    private onAddActivityClick;
    private onDeleteActivityClick;
    private onEditActivityClick;
    private onActivityContextMenu;
}
