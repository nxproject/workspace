# NX.Node - Genome

A genome is simply a Dockerfile, with a few additions.  This is the definition for the
processor genome:

```
\# NXProject Processor 3.1.0 Image

FROM mcr.microsoft.com/dotnet/core/runtime:3.1
LABEL {proj_label}

COPY / /etc/wd/

WORKDIR /etc/wd
```
When the system uses a genome, it will do replacement of all **{xxx}** entries with
values found in the [environment settings](README_ENv.md).

## Adding your own genomes

You can add your own genomes by adding your files to the Hive/Genomes folders. 

You can use the same version of the OS by using in your Dckerfile:
```
FROM {repo_project}/base:{tier}
```
Note that you MUST include the following:
```
LABEL {proj_label}
```
which is used to track to which hive the bumble bee containers belong.  Having this allows
the Docker repository to have other entries  that are not associated with NX.Project.

Note:  Please use the **Docker Build CLI** to test that any Genome that you
introduce can be both **built** and **started** before integrating them into the
system.  A genome and/or DNA that fails will most likely cause the system to go into
and endless loop.  You have been warned.

Note:  Make sure that any script files have Linux line endings.  How to set this
up in Visual Studio can be found [here](https://stackoverflow.com/questions/3802406/configure-visual-studio-to-use-unix-line-endings/63109256#63109256)

## Making the processor genome

The processor genome is used to create all the bees with the exception for bumble bees.
The genome is not part of the system as first downloaded, as it has to be compiled
and using the base code plust any additions tht you may want to include.

There are two basic rules you need to follow to include your own code:

* Your code must be .NET Core or .NET Standard

* You need to copy the DLLs you want included into a single folder structure

	OR

* You need to make your projects part of the Node folder

Once those are done, execute the following in the command line:
```
NXNode.exe --make_genome y --code_folder foldername --field name=iptodocker
```
The **code_folder** is optional but the folder should contain:

* Multipe project folders
* A single project
* One or more DLL's

The system will copy the appropriate files into the genome.  It may me used multiple times 
to include multiple sources.

If you leave out the field, the local Docker instance is assumed.

Note that the process uses folder **/build/container**, so any contents in that folder
will be automatically deleted.

[Back to top](/help/docs/README.md)
