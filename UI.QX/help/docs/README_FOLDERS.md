# NX.Node - Where do I put my files

The folder structure look like this:
```
   X				Root folder
   
   X				Shared folder
   |
   +-------X		Dynamic folder
   |
   +-------X		Document folder 
```
These are the environment settings used by the folder mapping:

Setting|Meaning|Default
-------|-------|-------
root_folder|Starting point of the folder structure|Working directory
shared_folder|Folder where the shared items are kept|/etc/shared
dyn_folder|Folder where loaded DLLs are kept|#shared_folder#/dyn
doc_folder|Folder where documents are kept|#shared_folder#/files

You can change the subfolder by setting the environment setting as follows:
```
--doc_folder web
```
which would sey the folder to be the value of **shared_folder** with **web** appended
as a sub folder.

If you want to set the full path, simply enter the path as follows:
```
--doc_folder @/etc/web
```

## Linux v. MS Windows

The bees themselves all live in Linux, and you must think in Linux.  Folder paths
are in the format of **/folder/subfolder1/subfolder2...**.  It applies to the following:

* config
* code_folder
* xxx_folder

The config and code_folder are not stored in the shared environment settings.

[Back to top](/help/docs/README.md)
