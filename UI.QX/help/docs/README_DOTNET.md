# NX.Node - DotNet

The processor genome file (Dockerfile) looks like this:

```
# NXProject Processor 3.1.0 Image

FROM {repo_project}/dotnet:{tier}
# FROM mcr.microsoft.com/dotnet/core/runtime:3.1

LABEL {proj_label}

COPY / /etc/wd/

WORKDIR /etc/wd

```

The default behavior is to use the dotnet genome file (Dockerfile) that looks like this:

```
# NXProject Processor 3.1.0 Image

FROM mcr.microsoft.com/dotnet/core/runtime:3.1

RUN apt install libc6
RUN apt-get update
RUN apt-get install -y libgif-dev autoconf libtool automake build-essential gettext libglib2.0-dev libcairo2-dev libtiff-dev libexif-dev
RUN apt-get install -y libgdiplus

```

This dotnet genome file includes support for ```libgdiplus```, which is required by calls to ```System.Drawing.Common``` in Linux. 
If you do not need this support, you can change the processor genome file to look like this:

```
# NXProject Processor 3.1.0 Image

FROM mcr.microsoft.com/dotnet/core/runtime:3.1

LABEL {proj_label}

COPY / /etc/wd/

WORKDIR /etc/wd

```

This will save you about 500MB of space in your containers.

[Back to top](/help/docs/README.md)
