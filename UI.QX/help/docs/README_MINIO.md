# NX.Node - Minio

Minio is the document cloud storage system in the NXProject.  Its use is automatic when you use
either the **Proc.File** or **Route.File** modules.  As all AWS S3-compliant system use
a bucket structure and not a folder structure, the NX Project file manager maps a pseudo-file
tree into buckets.  Each folder is a bucket which holds all files in the folder as well
as pointers to sub-folders.

## Document manager

You can instatiate a Minio bumble bee, which is the interface to Minio by calling:
```JavaScript
uses Proc.Minio
```

## At startup

You can write to the folder structure while the minio bumble bee is being launched, and any
writes will be integrated into Minio as soon as the bumble bee is available.  Reads cannot
be queued, so they return empty strings for any file.

## Making the bumble bee an external and AWS

You can make the minio bumble be be external by calling:
```
--external minio=url
```
where the url is the locaton where your Minio instance is located.  If you wish to use AWS
call:
```
--external minio=s3.amazonaws.com --minio_acces youraccesskey --minio_secret yoursecretkey
```
Note that to run the minio bumble bee locally, the access and secret keys are optional,
except that if you destroy the environment store in the redis bumble bee, you will lose
access to the minio bumble bee documents, so use of your own keys is strongly suggested.

## Bucket usage

The minio bumble bee uses one bucket, which can be set by using:
```
--minio_bucket thebuckettobeused
```
The default name is **nxproject**.

Note that the object names used in the bucket are encoded to support the naming conventions
used in a disk based directory structure.

[Back to top](/help/docs/README.md)
