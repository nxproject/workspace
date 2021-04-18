# Accounts

An account is a login that uses an email address or phone number as the identifier.  While [users](README_USERS.md)
are ***inhouse personnel*** , accounts are ***external entities***.

Accounts are the central dataset to [billing](README_BILLING.md) and [telemetry](README.TELEMETRY.md).  Accounts
are also ***virtual users*** when you choose and carry the same dataset rules.

![image](images/Acct1.png)

---

|Field|Meaning|
|-|-|
|Name|EMail or phone number that is the name used at log in|
|Password|Password used at login (case sensitive)|
|Allowed|Dtasets allowed for the user (see below)|
|Last login|Date and time that the account logged in|
|Subscribed On|Date and time that the account subscribed|
|Opt Out On|Date and time that the account opted out|
|Child of|User that is related to the account|

---

![image](images/Acct2.png)

|Field|Meaning|
|-|-|
|Last ctc out|Date and time that the account was contacted|
|Ctc out source|Source in which the account was contacted|
|Ctc out via|Method in which the account was contacted|
|Ctc out cmp|Cmpaign in which the account was contacted|

---

![image](images/Acct3.png)

|Field|Meaning|
|-|-|
|Last ctc in|Date and time that the account was contacted|
|Ctc in source|Source in which the account was contacted|
|Ctc in via|Method in which the account was contacted|
|Ctc in cmp|Cmpaign in which the account was contacted|

## Enabling accounts

You enable telemetry ubsing the [Site Settings](README_SITE.md) tool.  In it you will find:

![image](images/Site3.png)

Setting the ***Accounts*** entry to ***y*** enables the account options.

Note that ypu can also enter the default allowed value int the ***Def.Alwd.Def*** field, which will be used
for any account that does not have it's own definition.

## Automatic account creation

Accounts are automatically created and maintained when an ***account*** field is included in a dataset.  
The field labeled ***Access*** in this dataset is an example:

![image](images/Acct10.png)

Account fields allow for the display of the account dataset by using the ***Display*** entry in the options menu, accessible by
right-mouse click on the field.

## Uses

An account has multiple uses:

1) A login into the system
2) A billable entity
3) A contactable entity

### As a login

An account can be used to login into the system just like any user would.  For this case, the ***password*** and ***allowed*** fields are used.  The ***allowed*** field should have ***?ACCT*** to identify the account as an external account which limits access to certain functionality.

### As a billable entity

An account can be used to create invoices.  You need to enable billing in the [Site Settings](README_SITE.md) tool, ***System*** tab.

### As a contactable entity

You can EMail, SMS or call an account, which triggers ***outbound telemetry*** information.  When an account SMS, calls or clicks on any ***inbound telemetry*** link, the contact is added into the ***inbound telemetry*** fields.  You need to enable telemetry in the [Site Settings](README_SITE.md) tool, ***System*** tab.

## Allowed

The allowed field is the same as in the [Users](README_USERS.md) tool.

You can right mouse click on an ***allowed*** field and select ***Explain*** to get a breakdown on what the
rules given will provide.

[Home](../README.md)
