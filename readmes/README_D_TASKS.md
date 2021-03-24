# Tasks

## What is it?

## Creating a new task

## Commands

### Address
|Command|Description|Parameter|Use| |
|-|-|-|-|-|
|address.geocode|Gets the latitude and longitude of an address|addr|The address field|Required|
| | |city|The city field|Required|
| | |state|The state field|Required|
| | |zip|The zip field|Required|
| | |lat|The latitude field|Required|
| | |long|The longitude field|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||
|address.validate|Validates an address|addr|The address field|Required|
| | |city|The city field|Required|
| | |state|The state field|Required|
| | |zip|The zip field|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||

### Array
|Command|Description|Parameter|Use| |
|-|-|-|-|-|
|array.append|Appends word or words from a value|array|The name of the array||
| | |value|The value|Required|
| | |split|If true, the value is broken by spaces||
| | |if|Execute if||
| | |comment|What does this step accomplish||
|array.each|Calls a task for each object in the list.  The working object is found at [*l:listobj] and the count is at [*l:listcount]|array|The name of the array||
| | |field|The field to use for the current value|Required|
| | |store|The store to use for the current value|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||
|array.pop|Pops a store from an array (remove last)|array|The name of the array||
| | |store|The store name|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||
|array.push|Pushes a store into an array (add last)|array|The name of the array||
| | |store|The store name|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||
|array.shift|Shift a store from an array (remove first)|array|The name of the array||
| | |store|The store name|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||
|array.unshift|Unshift a store from an array (add first)|array|The name of the array||
| | |store|The store name|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||
|array.use|Sets the default array|array|The name of the array|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||

### Comm
|Command|Description|Parameter|Use| |
|-|-|-|-|-|
|msg.action.add|Adds an action to the message|msg|The message to use||
| | |task|The task|Required|
| | |name|The name to display||
| | |if|Execute if||
| | |comment|What does this step accomplish||
|msg.attachment.add|Adds an attachment to the message|msg|The message to use||
| | |doc|The document|Required|
| | |name|The name to display||
| | |if|Execute if||
| | |comment|What does this step accomplish||
|msg.to.add|Adds an addressee to the message|msg|The message to use||
| | |to|The to|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||
|msg.actions.clear|Removes all actions from the message|msg|The message to use||
| | |if|Execute if||
| | |comment|What does this step accomplish||
|msg.attachments.clear|Removes all attachments from the message|msg|The message to use||
| | |if|Execute if||
| | |comment|What does this step accomplish||
|msg.tos.clear|Removes all to's from the message|msg|The message to use||
| | |if|Execute if||
| | |comment|What does this step accomplish||
|msg.remove|Removes the message from the stack|msg|The message to remove||
| | |if|Execute if||
| | |comment|What does this step accomplish||
|msg.set.template.email|Sets the message EMail template|msg|The message to use||
| | |value|The template|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||
|msg.set.footer|Sets the message footer text|msg|The message to use||
| | |value|The footer text|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||
|msg.set.item|Sets the message item text|msg|The message to use||
| | |item|The  item|Required|
| | |value|The  text|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||
|msg.set.message|Sets the message text|msg|The message to use||
| | |value|The body text|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||
|msg.set.post|Sets the message post text|msg|The message to use||
| | |value|The post text|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||
|msg.set.template.sms|Sets the message SMS template|msg|The message to use||
| | |value|The template|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||
|msg.set.subject|Sets the message subject|msg|The message to use||
| | |value|The subject|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||
|msg.use|Sets the default message|msg|The message to use||
| | |if|Execute if||
| | |comment|What does this step accomplish||
|send|Sends a message|msg|The message to be sent||
| | |if|Execute if||
| | |comment|What does this step accomplish||

### Document
|Command|Description|Parameter|Use| |
|-|-|-|-|-|
|doc.add|Adds a document to the working set|name|The name to be used|Required|
| | |obj|The name of the object which is the parent||
| | |path|The path of the document|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||
|doc.clear|Clears all documents|if|Execute if||
| | |comment|What does this step accomplish||
|doc.copy|Copies a document|from|The source document name|Required|
| | |to|The target document name|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||
|doc.delete|Removes a document from the list and deletes it from storage|doc|The name of the document to be deleted|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||
|doc.pdf|Converts to PDF|doc|The document to be merged|Required|
| | |to|The PDF document|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||
|doc.remove|Removes a document from the list|doc|The document to be removed|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||
|doc.rename|Renames a document|from|The source document name|Required|
| | |as|The target document physical name|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||
|doc.save|Saves a document|doc|The name of the document to be saved|Required|
| | |text|The text||
| | |if|Execute if||
| | |comment|What does this step accomplish||
|merge.doc|Merges a .DOCX or .PDF file with values from the context|doc|The document to be merged|Required|
| | |to|The merged PDF document|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||

### Document List
|Command|Description|Parameter|Use| |
|-|-|-|-|-|
|doc.list.add|Adds a document to the list|doc|The document name to add|Required|
| | |list|The list|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||
|doc.list.clear|Empties a list|list|The list to be emptied|Required|
| | |remove|Remove documents||
| | |if|Execute if||
| | |comment|What does this step accomplish||
|doc.list.delete|Deletes all the documents in list|list|The list|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||
|doc.list.each|Calls code for each document in the list|list|The list||
| | |doc|The field to put the document name|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||
|doc.list.obj|Creates a document list from an object's folder|list|The list|Required|
| | |obj|The source object||
| | |folder|The folder||
| | |if|Execute if||
| | |comment|What does this step accomplish||
|doc.list.remove|Removes a document from the list|doc|The document name to remove|Required|
| | |list|The list|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||

### External
|Command|Description|Parameter|Use| |
|-|-|-|-|-|
|python|Runs a python script|code|Document that holds the python script.  Entry point should have two args, env - the workign environment and args - the parameters passed|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||
|send.tweet|Sends a tweet|consumerKey|Your consumer key from Twitter|Required|
| | |consumerSecret|Your secret key from Twitter|Required|
| | |accessToken|Your access token from Twitter|Required|
| | |accessTokenSecret|Your access token secret from Twitter|Required|
| | |msg|The message to be tweeted|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||

### Flow
|Command|Description|Parameter|Use| |
|-|-|-|-|-|
|call|Calls a task, returning to calling task on exit|task|The task to be called|Required|
| | |store|Store to pass as passed||
| | |if|Execute if||
| | |comment|What does this step accomplish||
|continue.on|Continues an an on.xxx call|comment|What does this step accomplish||
|noop|Does nothing| |
|end|Ends a task|store|Store to return||
| | |if|Execute if||
| | |comment|What does this step accomplish||
|on.error|Sets the default process to handle errors|comment|What does this step accomplish||
|switch|Switch execution based on a given expression|expression|The expression to evaluate. The evaluated value will be used to switch on|Required|
| | |cases|A comma-separated list of possible outcomes of the expression|Required|

### FTP
|Command|Description|Parameter|Use| |
|-|-|-|-|-|
|ftp.changedir|Changes the current directory|conn|The connection to use||
| | |dir|The directory name|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||
|ftp.close|Closes the sftp connection|conn|The connection to use||
| | |if|Execute if||
| | |comment|What does this step accomplish||
|ftp.delete|Deletes a file from the server|conn|The connection to use||
| | |file|The file name at the server|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||
|ftp.download|Downloads a file from the server|conn|The connection to use||
| | |file|The file name at the server|Required|
| | |doc|The document name|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||
|ftp.download.list|Downloads a set of files from the server|conn|The connection to use||
| | |list|The list to store the files|Required|
| | |obj|The object to store the files under||
| | |folder|The folder||
| | |del|If true, delete when downloaded||
| | |if|Execute if||
| | |comment|What does this step accomplish||
|ftp.open|Opens an sftp connection|conn|The connection to use||
| | |url|The URl to connect to|Required|
| | |port|The port||
| | |user|The user name||
| | |pwd|The user password||
| | |sftp|True if SFTP||
| | |cert|The SSH certificate if SFTP (from system wallet)||
| | |if|Execute if||
| | |comment|What does this step accomplish||
|ftp.upload|Uploads a file to the server|conn|The connection to use||
| | |file|The file name at the server|Required|
| | |doc|The document name|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||
|ftp.use|Sets the default sftp connection|conn|The connection to use|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||

### HTTP
|Command|Description|Parameter|Use| |
|-|-|-|-|-|
|http.cookie.get|Closes the sftp connection|conn|The connection to use||
| | |domain|The domain|Required|
| | |key|The key|Required|
| | |to|The location of the value|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||
|http.cookie.set|Sets a cookie value in the connection|conn|The connection to use||
| | |domain|The domain|Required|
| | |key|The key|Required|
| | |value|The value|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||
|http.get|Does an HTTP Get|conn|The connection to use||
| | |url|The URL|Required|
| | |storeout|The store to use as parameters||
| | |storein|The store to use as return||
| | |if|Execute if||
| | |comment|What does this step accomplish||
|http.use|Sets the default http connection|conn|The connection to use||
| | |if|Execute if||
| | |comment|What does this step accomplish||

### Object
|Command|Description|Parameter|Use| |
|-|-|-|-|-|
|obj.add|Adds an object to the working stack|obj|The name of the object|Required|
| | |ds|The dataset of the object|Required|
| | |id|The id of the object|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||
|obj.add.link|Adds a link of an object to the working stack|obj|The name of the object|Required|
| | |field|The link field|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||
|obj.add.parent|Adds the parent of an object to the working stack|obj|The name of the object|Required|
| | |from|The name of the source object||
| | |if|Execute if||
| | |comment|What does this step accomplish||
|obj.alias|Aliases an object to the working stack|obj|The name of the object|Required|
| | |from|The curent object name|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||
|obj.apply|Applies a store to an object|obj|The name of the object|Required|
| | |store|The source object||
| | |if|Execute if||
| | |comment|What does this step accomplish||
|obj.copy|Copies one object to another|obj|The name of the source object||
| | |to|The name of the target object|Required|
| | |flds|Fields to include||
| | |excl|Fields to exclude||
| | |ext|Copy extensions||
| | |if|Execute if||
| | |comment|What does this step accomplish||
|obj.create|Creates a new working object|ds|The dataset of the object|Required|
| | |store|The store holding the values||
| | |obj|The name of the object||
| | |parent|The name of the parent object||
| | |if|Execute if||
| | |comment|What does this step accomplish||
|obj.delete|Deletes the object from the database and from the working set|obj|The name of the object|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||
|obj.doc.copy|Copies all documents from an object|obj|The object to use||
| | |to|The target object|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||
|obj.doc.list|Creates a document list from an object|obj|The object to use||
| | |list|The list|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||
|obj.each|Call code for each object in a query|list|The list|Required|
| | |ds|The dataset of the object|Required|
| | |query|The store that is the query|Required|
| | |sortby|Sort by||
| | |limit|Max number of objects to return||
| | |skip|Number of objects to skip||
| | |dir|Direction of sort (asc/desc)||
| | |as|The name of the passed object|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||
|obj.lu|Carries out lookup on a field|obj|The name of the object||
| | |field|The field|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||
|obj.organizer|Creates an organizer for the object|obj|The object to use||
| | |options|The dataset options||
| | |doc|The document||
| | |folder|The folder||
| | |if|Execute if||
| | |comment|What does this step accomplish||
|obj.remove|Removes the object from working set|obj|The name of the object|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||
|obj.save|Saves the working object|obj|The name of the object||
| | |tasks|If true (non zero) allow tasks, otherwise disallow||
| | |if|Execute if||
| | |comment|What does this step accomplish||
|obj.set|Sets a value in a field of an object|obj|The name of the object||
| | |field|The field to set|Required|
| | |value|The value|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||
|obj.touch|Flags the field as changed|obj|The object to use||
| | |field|The field|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||
|obj.use|Sets the default object|obj|The object to use|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||
|obj.load.store|Copies a store into an object|obj|The name of the object||
| | |store|The store to use||
| | |if|Execute if||
| | |comment|What does this step accomplish||
|set.link|Sets a link to an object|obj|The name of the object||
| | |field|The field|Required|
| | |link|The linked object|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||
|set.parent|Sets the parent of an object|obj|The name of the object||
| | |link|The linked object|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||

### Object List
|Command|Description|Parameter|Use| |
|-|-|-|-|-|
|obj.list.add|Adds an object to the list|obj|The object name to add|Required|
| | |list|The list|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||
|obj.list.clear|Empties a list|list|The list to be emptied|Required|
| | |remove|Remove objects||
| | |if|Execute if||
| | |comment|What does this step accomplish||
|obj.list|Creates a list of objects using a query|list|The list|Required|
| | |ds|The dataset of the object|Required|
| | |query|The store that is the query|Required|
| | |sortby|Sort by||
| | |limit|Max number of objects to return||
| | |skip|Number of objects to skip||
| | |dir|Direction of sort (asc/desc)||
| | |if|Execute if||
| | |comment|What does this step accomplish||
|obj.list.delete|Deletes all objects in the list|list|The list|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||
|obj.list.each|Calls a chore for each object in the list.  The working object is found at [*l:listobj] and the count is at [*l:listcount]|list|The list|Required|
| | |obj|The object name to use inside the chore|Required|
| | |store|Store to pass as passed||
| | |if|Execute if||
| | |comment|What does this step accomplish||
|obj.list.pick|Creates an object list using a query|list|The name of the list|Required|
| | |ds|The dataset of the object|Required|
| | |query|The store that is the query|Required|
| | |sortby|Sort by||
| | |limit|Max number of objects to return||
| | |skip|Number of objects to skip||
| | |dir|Direction of sort (asc/desc)||
| | |merge|True if lists are merged||
| | |if|Execute if||
| | |comment|What does this step accomplish||
|obj.pick|Gets or creates an object using a query|name|The name of the object|Required|
| | |ds|The dataset of the object|Required|
| | |query|The store that is the query|Required|
| | |sortby|Sort by||
| | |limit|Max number of objects to return||
| | |skip|Number of objects to skip||
| | |dir|Direction of sort (asc/desc)||
| | |store|The store holding the values||
| | |parent|The name of the parent object||
| | |if|Execute if||
| | |comment|What does this step accomplish||
|obj.list.remove|Removes an object from the list|obj|The object name to remove|Required|
| | |list|The list|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||

### Ops
|Command|Description|Parameter|Use| |
|-|-|-|-|-|
|decr|Decrements a field|field|The field|Required|
| | |value|The value to decrement by||
| | |if|Execute if||
| | |comment|What does this step accomplish||
|incr|Increments a field|field|The field|Required|
| | |value|The value to increment by||
| | |if|Execute if||
| | |comment|What does this step accomplish||
|set|Sets a value|field|The field|Required|
| | |value|The value|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||

### PDF
|Command|Description|Parameter|Use| |
|-|-|-|-|-|
|pdf.one|Merges multiple .PDF files into one|doclist|The document list to be merged|Required|
| | |to|The merged document|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||
|pdf.split|Splits one .PDF file into multiple by page count|doc|The document to be split|Required|
| | |pages|The page count (<0 if skip, * if pages left over)|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||

### Query
|Command|Description|Parameter|Use| |
|-|-|-|-|-|
|query.use|Sets the default query|query|The query to use||
| | |if|Execute if||
| | |comment|What does this step accomplish||
|query.add|Sets a query expression|query|The query||
| | |field|The field|Required|
| | |op|The operation|Required|
| | |value|The value|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||

### Store
|Command|Description|Parameter|Use| |
|-|-|-|-|-|
|store.clear|Clears all values in the store|store|The store to clear||
| | |if|Execute if||
| | |comment|What does this step accomplish||
|store.each|Calls code, passing an element of the array|store|Store to use||
| | |field|Field in to use to pass name|Required|
| | |value|Field in to use to pass value|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||
|store.load.changes|Loads a store from changes in object|store|The store to load||
| | |obj|The object||
| | |if|Execute if||
| | |comment|What does this step accomplish||
|store.load.object|Loads a store from object|store|The store to load||
| | |obj|The object||
| | |if|Execute if||
| | |comment|What does this step accomplish||
|store.load.string|Loads a store from a string|store|The store to load||
| | |value|The value|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||
|store.remove|Removes the store from the stack|store|The store to remove||
| | |if|Execute if||
| | |comment|What does this step accomplish||
|store.use|Sets the default store|store|The store to use||
| | |if|Execute if||
| | |comment|What does this step accomplish||

### Text
|Command|Description|Parameter|Use| |
|-|-|-|-|-|
|text.add.text|Adds text to a text line|text|The text name||
| | |value|The text||
| | |line|The row||
| | |if|Execute if||
| | |comment|What does this step accomplish||
|text.add.word|Adds text to a text line|text|The text name||
| | |value|The text||
| | |line|The row||
| | |if|Execute if||
| | |comment|What does this step accomplish||
|text.append.paragraph|Appends a new word to a text|text|The text name||
| | |value|The text|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||
|text.add.doclist|Appends a block of doc names to a memo|text|The text name||
| | |list|The list||
| | |line|The row||
| | |delim|The delimiter||
| | |if|Execute if||
| | |comment|What does this step accomplish||
|text.insert.paragraph|Inserts a new paragraph to a text|text|The text name||
| | |value|The text|Required|
| | |line|The row to insert before||
| | |if|Execute if||
| | |comment|What does this step accomplish||
|text.load|Copies a value into a text area|text|The text name||
| | |value|The value||
| | |if|Execute if||
| | |comment|What does this step accomplish||
|text.text|Copies text into a field|text|The text name||
| | |field|The field|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||
|text.use|Sets the default text|text|The text name||
| | |if|Execute if||
| | |comment|What does this step accomplish||

### Time Track
|Command|Description|Parameter|Use| |
|-|-|-|-|-|
|tt.continue|Continues time tracking|obj|The name of the object||
| | |type|The type of tag||
| | |user|The user||
| | |reason|The reason of the freeze||
| | |if|Execute if||
| | |comment|What does this step accomplish||
|tt.freeze|Freezes time tracking|obj|The name of the object||
| | |type|The type of tag||
| | |user|The user||
| | |reason|The reason of the freeze||
| | |if|Execute if||
| | |comment|What does this step accomplish||
|tt.start|Starts time tracking|obj|The name of the object||
| | |type|The type of tag||
| | |user|The user||
| | |if|Execute if||
| | |comment|What does this step accomplish||
|tt.stop|Stops time tracking|obj|The name of the object||
| | |type|The type of tag||
| | |user|The user||
| | |if|Execute if||
| | |comment|What does this step accomplish||

### Trace
|Command|Description|Parameter|Use| |
|-|-|-|-|-|
|trace|Starts the trace mode|if|Execute if||
| | |comment|What does this step accomplish||
|trace.end|Ends the trace mode|if|Execute if||
| | |comment|What does this step accomplish||
|trace.send|Sends the trace|if|Execute if||
| | |comment|What does this step accomplish||

### USPS
|Command|Description|Parameter|Use| |
|-|-|-|-|-|
|usps.citystate|Returns the city and state for an address|address|The address|Required|
| | |city|The city|Required|
| | |state|The state|Required|
| | |zip|The zip|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||
|usps.zip|Returns the ZIP for an address|city|The city|Required|
| | |state|The state|Required|
| | |zip|The zip|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||
|usps.validate|Validates an address|address|The address|Required|
| | |city|The city|Required|
| | |state|The state|Required|
| | |zip|The zip|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||

### Word
|Command|Description|Parameter|Use| |
|-|-|-|-|-|
|word.pop|Remove word from end of string|string|The string||
| | |value|The text||
| | |delim|The delimiter||
| | |if|Execute if||
| | |comment|What does this step accomplish||
|word.push|Adds text to end of string|string|The string||
| | |value|The text||
| | |delim|The delimiter||
| | |if|Execute if||
| | |comment|What does this step accomplish||
|word.shift|Remove text from front of string|string|The string||
| | |value|The text||
| | |delim|The delimiter||
| | |if|Execute if||
| | |comment|What does this step accomplish||
|word.unshift|Adds text to front of string|string|The string||
| | |value|The text||
| | |delim|The delimiter||
| | |if|Execute if||
| | |comment|What does this step accomplish||

### Workflow
|Command|Description|Parameter|Use| |
|-|-|-|-|-|
|workflow.end|Ends a workflow on a given object|obj|The name of the object||
| | |wf|The workflow to start|Required|
| | |name|The user defined name|Required|
| | |if|Execute if||
| | |comment|What does this step accomplish||
|workflow.start|Starts a workflow on a given object|obj|The name of the object||
| | |wf|The workflow to start|Required|
| | |name|The user defined name|Required|
| | |store|The store to pass||
| | |if|Execute if||
| | |comment|What does this step accomplish||



[Home](../README.md)
