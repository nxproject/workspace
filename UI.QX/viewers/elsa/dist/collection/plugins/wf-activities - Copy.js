import { OutcomeNames } from "../models/outcome-names";
import pluginStore from '../services/workflow-plugin-store';
export class WorkflowActivities {
    constructor() {
        this.getName = () => "WorkflowActivities";
        this.getActivityDefinitions = () => ([
            this.do(),
            this.wait()
        ]);
        this.do = () => ({
            type: "Do",
            displayName: "Do",
            description: "Does activity.",
            runtimeDescription: `x => !!x.state.subj ? \`<strong>\${ x.state.subj }</strong>\` : x.definition.description`,
            //runtimeDescription: 'x => !!x.state.subj ? x.state.subj : x.definition.description ',
            category: WorkflowActivities.Category,
            outcomes: [OutcomeNames.Done, OutcomeNames.Fail, OutcomeNames.Other],
            properties: [{
                    name: 'subj',
                    type: 'text',
                    label: 'Description',
                    hint: 'Enter the description for the activity'
                }, {
                    name: 'msg',
                    type: 'text',
                    label: 'Message',
                    hint: 'Enter the message for the activity'
                }, {
                    name: 'duration',
                    type: 'text',
                    label: 'Duration',
                    hint: 'Enter the duration for the activity'
                }]
        });
        this.wait = () => ({
            type: "Wait",
            displayName: "Wait",
            description: "Waits for a given interval.",
            runtimeDescription: `x => !!x.state.subj ? \`<strong>\${ x.state.subj }</strong>\` : x.definition.description`,
            category: WorkflowActivities.Category,
            icon: 'fas fa-clock',
            outcomes: [OutcomeNames.Done],
            properties: [{
                    name: 'subj',
                    type: 'text',
                    label: 'Description',
                    hint: 'Enter the description for the wait'
                }, {
                    name: 'duration',
                    type: 'text',
                    label: 'Duration',
                    hint: 'Enter the duration to wait for'
                }]
        });
    }
}
WorkflowActivities.Category = "Work Flow";
pluginStore.add(new WorkflowActivities());
