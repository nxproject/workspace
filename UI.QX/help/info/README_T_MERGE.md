# Merging

Merging is accomplished by creating ***docx*** documents with embedded merge codes.  This is an example:

![image](/help/info/images/Merge2.png)

Merge codes are field names enclosed in square brackets, as explained un the [Lists and Expressions](/help/info/README_LE.md)
***Fields*** section.

When merged, the merge fields are converted to their stored equivalents:

![image](/help/info/images/Merge3.png)

## Types of merge fields

There are two types types of merge fields.

### Source is object

The most common type of merge field is one where the data source is the object being used for the merge.  It is the field
name enclosed in brackets.  It the exmaple above:

```
[name]
```

Is an example of this type of field.

#### Getting the field definition code

You can get the field definition code by right-mouse clicking on any field and selecting the ***Copy field def*** entry:

![image](/help/info/images/Merge1.png)

### Source is a setting

When the source is a setting, the ***store field*** format is used.  These are the available stores:

#### Site information

Site information can be merged by using ***[\*sys:settingname]*** as the merge code.  You can get the field definition
code by right-mouse clicking on any field in the [Site Settings](/help/info/README_SITE.md) window and selecting the 
***Copy field def*** entry: 

![image](/help/info/images/Merge4.png)

Since not all users have access to the settings, here are the codes for the common ones:

|Field|Code|
|-|-|
|Name|[*sys:name]|
|Addr1|[*sys:addr1]|
|Addr2|[*sys:addr2]|
|City|[*sys:city]|
|State|[*sys:state]|
|Zip|[*sys:zip]|
|Phone|[*sys:phone]|
|Fax|[*sys:fax]|
|Tax ID|[*sys:taxid]|
|User Defined #1|[*sys:udf1]|
|User Defined #2|[*sys:udf2]|
|User Defined #3|[*sys:udf3]|

#### User information

User information can be merged by using ***[\*user:settingname]*** as the merge code.  You can get the field definition
code by right-mouse clicking on any field in the ***user*** window and selecting the 
***Copy field def*** entry: 

![image](/help/info/images/Merge5.png)

Since not all users have access to the settings, here are the codes for the common ones:

|Field|Code|
|-|-|
|Name|[*user:name]|
|Full name|[*user:dispname]|
|Title|[*user:title]|
|EMail Name|[*user:emailname]|
|Twilio Phone number|[*user:twiliophone]|

## Templates

Templates are accessed via the ***Templates*** button in the ***command bar*** in the ***pick*** window for the dataset.
Clicking on the button displays the window:

![image](/help/info/images/Merge6.png)

Ypu can upload a template or build from a ***built-in template*** as shown here:

![image](/help/info/images/Merge7.png)

![image](/help/info/images/Merge8.png)

The template is displayed and you can modify it to your needs:

![image](/help/info/images/Merge2.png)

The template is now available for use.

## Merge from object

You can merge from the object by clicking on the ***Merge*** button in the ***command bar*** for the object:

![image](/help/info/images/Merge9.png)

A list of templates is displayed, and by selecting one, it is used to cerate the merged document:

![image](/help/info/images/Merge3.png)

You can further edit the document to fit your needs.  The document is saved as part of the object documents:

![image](/help/info/images/Merge10.png)

## Merge from documents

You can also merge from the ***Documents*** window of the object by using the ***Add Document*** option from the 
context menu for any ***folder***:

![image](/help/info/images/Merge11.png)

This will prompt for a document name and which template to use:

![image](/help/info/images/Merge12.png)

And create the document as before.  The document is left in the ***documents*** area for the object:

![image](/help/info/images/Merge13.png)

The diffrerences in the two methods above are that while the ***from object*** allows you to create ***document sets***,
the ***from documents*** allow you to name the merged document and place it in a folder.

### Built-in templates

These are the built-in templates:

|Template|Use|
|-|-|
|Empty|An empty document|
|Letterhead|An empty document with letterhead|

## Document sets

Document stes are groups of mergeable documents.  They are created by creating ***folders*** in the ***templates***
***Documents*** window:

![image](/help/info/images/merge14.png)

Which prompts for a folder name:

![image](/help/info/images/Merge15.png)

And documents can be added to the folder:

![image](/help/info/images/Merge16.png)

[Home)(../README.md)
