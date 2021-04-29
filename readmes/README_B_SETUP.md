# Setting up billing

To setup billing, multiple screens need to be filled out.  We start with the ***Site settings***, ***System Billing*** page:

![image](images/Billz2.png)

|Field|Meaning|
|-|-|
|Billing|Set to y to enable billing|
|Pay Req Tmplt|[EMail template](README_EMT.md) to be used.  If blank, the default template is used|
|Pay Req. Subj.|Text to use as subject in the pay request.  Default: Payment Request|
|Pay Req. Msg.|Text to use as the message in the pay request.  Default: Click on the button to complete the transaction|
|Inv. DOM|Day of the month to auto generate invoices.  Can be a negative number to compute from end of month.  If blank, no invoices are auto-generated|

## Pay button

The pay button is geneated when a [Stripe account](README_SVCS.md) is setup:

![image](images/Billz1.png)

As shown, yu can use the ***test*** keys for testing but make sure that you are using the ***live*** keys to properly credit your account.

## Templates

The [EMail template](README_EMT.md) tool allows you to create invoice templates.  Two sections are explicitly used:

![image](images/Billz3.png)

Note that all other sections are avaliable to fully customizze the invoice.

[Home](../README.md)
