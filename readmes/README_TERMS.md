# Basics

* ```Keyword``` - A word that starts with a letter and may contain letters and numbers only.  All items in NX.Workspace are named witha a keyword.
* ```JSON``` - The format that all the objects are stored in.
* ```List``` - A list of words separated by spaces.
* ```Store``` - A JSON object that is in working memory.

---

* ```Site``` - The set of datasets and users that are related to each other.  A site has a name that is a keyword.

---

* ```Field``` - A piece of information.  All fields have a name that is a keyword and stored in an object.
* ```Object``` - A set of fields.  Similar objects are stored in a dataset.  Objects have names which must be either a keyword or MongoDb Object IDs.
* ```Dataset``` - A set of objects.  Datasets have a definition and a name which is a keyword.
* ```View``` - A visual grouping of fields.  Views belong to a dataset and have names which are keywords.
* ```Context Menu``` - The menu displayed when a user right-clicks anywhere in the desktop.  The menu displayed, if any, depends on where the click took place. The context menu is used to display actions that are location dependent.
* ```Eval``` - A string that is mant to be evaluated in order to produce a value

---

* ```User``` - A person/entity that uses a site.  Normally a person in the organization that owns the system.  Users have a name which is a keyword.
* ```Account``` - A person/entity that uses the system.  Differs from an user in that the person/entity is not part of the organization that owns the system.  Accounts have names which are user defined, however phone numbers and email addresses are most common.
* ```Allowed``` - The rules that govern users and accounts.  It tells the system which datasets can be accesed and how.

---

* ```Document``` - A .DOCX or .PDF file.  All documents belong to either an object or a dataset.  Documents have names that are user defined.
* ```Activity``` - A task that has a duration and adjusted start/end times. Each activity belongs to a workflow and have a name which is a keyword.
* ```Workflow``` - A group of activities.  Workflows belong to a dataset and have a name which is a kwyword.
* ```Step``` - A executable command.  Steps belong to a task.
* ```Task``` - A set of steps.  Tasks belong to a dataset and have a name which is a keyword.
* ```Report``` - TBD

---

* ```Webtop``` - The visual displayed in a browser window when a user connects to the site.
* ```Taskbar``` - The bar at the bottom of the desktop.  It has a ***Start*** button in the leftmost position and a date/time display on the right, which also doubles as the ***Pinned*** menu.  The center holds task bar buttons, one for each window in the desktop.
* ```Taskbar button``` - The button that represents the window in the desktop.  Displays the caption text for the window.  When clicked, the window will cycle through the ***selected***, ***hidden*** and ***shown*** modes.
* ```Window ``` - The display of a view or a tool on the desktop.
* ```Top toolbar``` - The set of command or menu buttons displayed above the data area of a window.
* ```Bottom toolbar``` - The set of command or menu buttons displayed below the data area of a window.
* ```Command bar``` - The set of command or menu buttons displayed below any bottom toolbar or the data are of a window.
* ```System command bar``` - The set of command or menu buttons displayed below the command bar of a window.
* ```Notifications``` - A visual pop-up note that are displayed in the upper right corner of the desktop.  Notifications are designed to live for a short amount of time unless user interaction is required.

---

* ```Mobile``` - The visual displayed in a phone or tablet when a user connects to the site.
* ```Main menu``` - The menu displayed in the left side when the main menu icon is clicked.
* ```Support menu``` - The menu displayed in the right side when the support menu icon is clicked.

---

## Browsers

NX.Workspace has been tested against the following browsers:

* ```Google Chrome``` 87.0.4280.88 (Official Build) (64-bit)
* ```Mocrosoft Edge``` 87.0.664.66 (Official build) (64-bit)
* ```Mozilla Firefox``` 84.0.1 (64 bit)
* ```Opera``` 73.0.3856.284
* ```Brave``` 1.18.75 Chromium: 87.0.4280.101 (Official Build) (64-bit)
* ```Vivaldi``` 2.8.1664.44 (Stable channel) (64-bit)

---

## Dates and Times

When working in NX.Workspace, parts of which run in a browser and parts in the cloud, you need to understand dates and times and how they are handled
in each portion.

In the browser, dates and times are rendered as to the timezone specified in the browser, so the local machine timezone is used.

In the cloud, servers run in the UTC (Universal Time Coordinates) timezone so they can coordinate their work.  UTc is also known as Greewich 
Time or London Time.  In order to adjust any generated date and time to the  ***best*** date and time, the system setting ***timezone*** is
used.  For example:

```
#today([*sys:timezone])#
```

Generates today's date adjusted to the timezone set for the office.  If you left the timezone parameter out, and the office were in US Pacific
Time, a difference of either 7 or 8 hours would ocurr.  



[Home](../README.md)
