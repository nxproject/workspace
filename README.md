# NX.Workspace

The NX.Workspace system is designed to create data and document storage platforms
for any office in an efficient manner.  It uses containers to allow for deployment
across a varied landscape.

## Who is this for

There are two groups that are the main target of the system:

* The DIY person that wants to create a simple system for their own use.  This group may take advantage of the data input creation tools and data linking options to crate and maintain a data and document storage system.
* A tech-savy consultant.  This group may take advantage of the advanced features of the system to create systems that are tailored for a specific industry.  He/She can then package the system and distribute his/her work by having his/her clients install the NX.Workspace and then adding the consultant's work. 

## Table of contents

* [Basics](readmes/README_TERMS.md)
	* [Visuals](readmes/README_VISUALS.md)
* [Site](readmes/README_SITE.md)
* Features
	* [Basics](readmes/README_D_BASICS.md)
		* [Lists and Expressions](readmes/README_LE.md)
	* [Objects](readmes/README_D_OBJ.md)
	* [Datasets](readmes/README_D_DATASETS.md)
		* [Pick Fields](readmes/README_D_PICK.md)
		* [System Datasets](README_D_SYSDS.md)
			* [Users](readmes/README_USERS.md)
			* [Accounts](readmes/README_ACCOUNTS.md)			
			* [EMail Templates](readmes/README_EMT.md)
			* [Campaigns](readmes/README_CAMPAIGN.md)
			* [Quick Messages](readmes/README_QM.md)
			* [Telemetry](readmes/README_TELEMETRY.md)
			* [Billing](readmes/README_BILLING.md)
				* [Rate Table](readmes/README_B_RATE.md)
				* [Charges](readmes/README_B_CHARGE.md)
				* [Subscriptions](readmes/README_B_SUBS.md)
				* [Invoices](readmes/README_B_INV.md)
			* [Time Tracking](readmes/README_D_TT.md)
			* [Quorums](readmes/README_QUORUMS.md)
			* [IOT](readmes/README_IOT.md)
			* [Help](readmes/README_HELP.md)
	* [Views](readmes/README_D_VIEWS.md)
	* [Fields](readmes/README_D_FIELDS.md)
	* Support
		* [Documents](readmes/README_D_DOCS.md)
			* [Merging](readmes/README_T_MERGE.md)
			* [Editor](readmes/README_T_EDITOR.md)
			* [PDF Viewer](readmes/README_T_PDF.md)
			* [Image Viewer](readmes/README_T_IMAGE.md)
			* [Video Viewer](readmes/README_T_VIDEO.md)
		* [Calendar](readmes/README_T_CAL.md)
		* [Report](readmes/README_T_REPORT.md)
		* [Analyze](readmes/README_T_ANALYZE.md)
		* [Tasks](readmes/README_D_TASKS.md)
		* [Workflows](readmes/README_D_WF.md)		
		* [NXCodes](readmes/README_D_NXCODE.md)
* [Mobile Support](readmes/README_MOBILE.md)
* [Collaboration](readmes/README_COLL.md)
* [Services](readmes/README_SVCS.md)
* [Packages](readmes/README_PKG.md)
* [AutoHotKeys - Linking your existing systenm](readmes/README_AHK.md)
* [Infrastructure](readmes/README_INFRA.md)

## Docker

A ready to run Docker image is kept at:

```
docker pull nxproject/site:processor
```

The code will automatically build and launch all other needed containers.

### Footnote

[The author](WHO.md)
