# Workflows

![image](/images/Wf1.png)

## What is it?

## Creating a new workflow

## Activities

### Action
|Command|Description|Parameter|Use| |
|-|-|-|-|-|
|call|Starts a workflow|workflow|The workflow to start|Required|
| | |name|The user defined name|Required|
| | |store|The store to pass||
| | |if|Execute if||
| | |comment|What does this step accomplish||
|delay|Delays for a specified duration|duration|The duration length|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||
|do|Creates activity|subject|The subject line|Required|
| | |message|The message line|Required|
| | |assignedTo|The group to whom the activity is assigned to|Required|
| | |duration|The duration length|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||
|task|Calls a task|task|The task to call|Required|
| | |store|The store to pass||
| | |return|The store to use for the return||
| | |if|Execute if||
| | |comment|What does this step accomplish||

### Flow
|Command|Description|Parameter|Use| |
|-|-|-|-|-|
|continue.on|Continues an an on.xxx call|comment|What does this step accomplish||
|end|Ends the workflow|comment|What does this step accomplish||
|fork|Fork workflow execution into multiple branches|branches|A comma-separated list of names representing branches|Required|
| | |comment|What does this step accomplish||
|ifelse|Evaluate a Boolean expression and continue execution depending on the result|expression|The expression to evaluate. The evaluated value will be used to switch on|Required|
|join|Merge workflow execution back into a single branch|joinMode|Either wait for all or any|Required|
| | |comment|What does this step accomplish||
|on.error|Sets the default process to handle errors|comment|What does this step accomplish||
|on.overdue|Sets the default process to handle overdues|comment|What does this step accomplish||
|switch|Switch execution based on a given expression|expression|The expression to evaluate. The evaluated value will be used to switch on|Required|
| | |cases|A comma-separated list of possible outcomes of the expression|Required|

### Ops
|Command|Description|Parameter|Use| |
|-|-|-|-|-|
|set|Sets a value|field|The field|Required|
| | |value|The value|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||

### Store
|Command|Description|Parameter|Use| |
|-|-|-|-|-|
|store.use|Sets the default store|store|The store to use||
| | |if|Execute if||
| | |comment|What does this step accomplish||



[Home](../README.md)
