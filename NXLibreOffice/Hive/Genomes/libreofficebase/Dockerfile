# Based on https://github.com/occrp-attic/convert-document

FROM mcr.microsoft.com/dotnet/core/runtime:3.1
LABEL {proj_label}

ENV DEBIAN_FRONTEND noninteractive

RUN apt-get -qq -y update \
    && apt-get -q -y dist-upgrade \
    && mkdir -p /usr/share/man/man1 \
    && apt-get -q -y install fonts-opensymbol hyphen-fr hyphen-de hyphen-en-us hyphen-it hyphen-ru fonts-dejavu fonts-dejavu-core fonts-dejavu-extra fonts-droid-fallback fonts-dustin fonts-f500 fonts-fanwood fonts-freefont-ttf fonts-liberation fonts-lmodern fonts-lyx fonts-sil-gentium fonts-texgyre fonts-tlwg-purisa \
    && apt-get -q -y install locales libreoffice libreoffice-writer psmisc curl libreoffice-impress libreoffice-common \
    && apt-get -qq -y autoremove \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/* /tmp/* /var/tmp/* \
    && localedef -i en_US -c -f UTF-8 -A /usr/share/locale/locale.alias en_US.UTF-8

ENV LANG='en_US.UTF-8'

RUN groupadd -g 1000 -r app \
    && useradd -m -u 1000 -d /tmp -s /bin/false -g app app

USER app

#HEALTHCHECK --interval=10s --timeout=10s --retries=100 \
#    CMD curl -f http://localhost:3000/health/live || exit 1
