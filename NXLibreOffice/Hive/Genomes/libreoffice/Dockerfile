# Based on https://github.com/occrp-attic/convert-document

FROM {repo_project}/libreofficebase:{hive}
LABEL {proj_label}

COPY --from={repo_project}/processor:{hive} /etc/wd /etc/wd

WORKDIR /etc/wd
