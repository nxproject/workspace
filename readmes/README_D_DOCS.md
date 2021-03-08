# Documents

Each object has a document store that is viewed via the ***Documents*** tool:

![image](/images/Doc1.png)

## Uploading documents

You can upload any type of documents to the store by clicking on the ***Upload*** button at the bottom of the tool.
You will be asked to select a file from your computer, which gets uploaded to the store:

![image](/images/Doc2.png)

Note that uploaded documents are brought in in the **Upload** folder.

## Top toolbar

|Cmd|Meaning|
|-|-|
|Select|Toggle between single entry and multiple entry selection modes|

## Context menu

The documents tool displays a context menu when the cursor is positioned over any entry and the user performs a
right-mouse click:

![image](/images/Doc3.png)

|Option|Meaning|
|-|-|
|Add Folder|Adds a folder or sub-folder.  Displayed when the entry is a folder|
|Add Document|Adds a document from the pre-defined system store|
|View|Views a document. Available for ***docx*** and ***pdf*** documents only|
|Download|Downloads the document|
|As PDF|Convert a document and/or folder into a ***pdf*** equivalent.  Only ***docx*** and ***pdf*** documents are included|
|Rename|Renames fiels or folders|
|Restore|Restore a document or folder from the internal backup|
|EMail|TBD|
|SMS|TBD|
|Delete|Deletes the document or folder|

### Add Folder

When you click on this option, you will be asked for a folder name:

![image](/images/Doc4.png)

And when confirmed, the folder will be created as a child of the selected folder:

![image](/images/Doc5.png)

This option is available only for folder entries.

### Add Document

When you click on this option, you will be asked for a file an template names:

![image](/images/Doc8.png)

The document will use the template and the data for the object and create a merged document:

![image](/images/Doc9.png)

Which is also added to the doocument list:

![image](/images/Doc10.png)

### View

Displays the document or folder as a ***pdf***.

### As PDF

When you click on this option, you will be asked for a file name:

![image](/images/Doc6.png)

And when confirmed, the document and/or folder will be converted to a ***pdf*** document and stored under the name given.

![image](/images/Doc7.png)

## Moving files

You can change the file folder by dragging a file into a different folder.

## Upload

The ***Upload*** button will dipslay the upload progress and display when the action is completed.

## Via web

The ***Via web*** button creates a link to the ***Documents*** window.  When clicked, a notification is displayed
with the link ID:

![image](/images/Doc11.png)

The link is copies to the clipboard and can be used in an email or SMS text.  The link has a format of:

```
http://10.192.180.160/capture?id=<linkid>
```

Where ***linkid*** is the ID shown in the notification.

### When used in a desktop browser

The link displays the following page:

![image](/images/Doc12.png)

Clicking on the ***file icon*** dsplays a ***file explorer*** allowing for the selection of a file.  Once a file is
selected, the display is updated:

![image](/images/Doc13.png)

Clicking on the ***check icon*** uploads the file to the ***Via web*** document folder.  

![image](/images/Doc18.png)

When compleyed, the display is updated:

![image](/images/Doc14.png)

### When used in a mobile device

The page displayed is similar to a desktop browser, except that the user is allowed to select a file or use the camera
to obtain a picture or video:

![image](/images/Doc15.png)

---

![image](/images/Doc16.png)

---

![image](/images/Doc17.png)

### Restrictions

Each link generated is alllowed to be used for a period of ***three hours*** and a ***maximum of 25 files***.

[Home](../README.md)
