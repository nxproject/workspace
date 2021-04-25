# EMail Templates

An EMail template is used to generate outbound emails.

![image](images/EMT1.png)

|Field|Meaning|
|-|-|
|Code|Name of template|
|Description|Desciption of template|
|Text|HTML that makes the template|

You can enter the HTML for the template or ruse the ***EMail Template Editor*** by right clicking on the ***text*** field and selecting the ***Edit*** option:

![image](images/EMT2.png)

The editor works by allowing you to drag/drop blocks from the left section of the editor into the body or center section.
These are the block definitions:

|Image|Name|Use|
|-|-|-|
|![image](../UI.QX/viewers/automizy/images/blocks/subject.gif)|Subject|Include the subject value|
|![image](../UI.QX/viewers/automizy/images/blocks/message.gif)|Message|Include the message value|
|![image](../UI.QX/viewers/automizy/images/blocks/attachments.gif)|Attachments|Include an attachments block|
|![image](../UI.QX/viewers/automizy/images/blocks/actions.gif)|Actions|Include an actions block|
|![image](../UI.QX/viewers/automizy/images/blocks/siteinfo.gif)|Site Information|Include a site information block|
|![image](../UI.QX/viewers/automizy/images/blocks/privacy.gif)|Privacy|Include a privacy text block|
|![image](../UI.QX/viewers/automizy/images/blocks/telemetry.gif)|Telemetry|Include a telemetry block|
|![image](../UI.QX/viewers/automizy/images/blocks/image.gif)|Image|Include an image block|
|![image](../UI.QX/viewers/automizy/images/blocks/text.gif)|Text|Include a text block|
|![image](../UI.QX/viewers/automizy/images/blocks/title.gif)|Title|Include a title text block|
|![image](../UI.QX/viewers/automizy/images/blocks/columns.gif)|Columns|Include a two-column block|
|![image](../UI.QX/viewers/automizy/images/blocks/share.gif)|Share|Include a ***Follow Us*** block|
|![image](../UI.QX/viewers/automizy/images/blocks/button.gif)|Button|Include a clickable button|
|![image](../UI.QX/viewers/automizy/images/blocks/gallery.gif)|Gallery|Include an image gallery block|
|![image](../UI.QX/viewers/automizy/images/blocks/html.gif)|HTML|Include a raw HTML text block|

## Merge fields

You can also insert merge field codes into any text by using the ***Merge Fields*** option in the edit menu:

![image](images/EMT3.png)

These generate the [directives](README_LE.md) for each type of entry:

|Entry|Directive|
|-|-|
|Subject|\{\_subject}}|
|Message|{{\_message}}|
|Data|{{replace_with_field}}|
|Attachments|```{{#if \_attachments}}<b>Attachments</b>{{#each \_attachments}}<br /><a class="aee-image-block-button" href="{{this.href}}" target="_blank" style="display: inline-block; color: #ffffff; background-color: {{this.color}}; border: solid 1px {{this.color}}; border-radius: 5px; box-sizing: border-box; cursor: pointer; text-decoration: none; font-size: 14px; font-weight: bold; margin: 2; padding: 12px 25px; text-transform: capitalize;">{{this.caption}}</a>{{/each}}{{/if}}<br />```|
|Actions|{{#if \_actions}}<br />{{#each \_actions}}<br /><a class="aee-image-block-button" href="{{this.href}}" target="_blank" style="display: inline-block; color: #ffffff; background-color: {{this.color}}; border: solid 1px {{this.color}}; border-radius: 5px; box-sizing: border-box; cursor: pointer; text-decoration: none; font-size: 14px; font-weight: bold; margin: 2; padding: 12px 25px; text-transform: capitalize; border-color: {{this.color}};">{{this.caption}}</a>{{/each}}{{/if}}<br />|
|Site Information|<span style="font-size: 12pt; "><b>{{\_sys.name}}</b></span><br>{{\_sys.addr1}}<br>{{\_sys.city}}, {{\_sys.state}}&nbsp;<br>{{\_sys.phone}}<br></span>|
|Payment Request|{{\_paymentrequest}}|

Note that you can use any field that is part of the [extended merge](EADME_T_MERGE.md) logic available for documents.


[Home](../README.md)
