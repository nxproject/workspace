# Based on https://github.com/occrp-attic/convert-document

FROM {repo_project}/{hive}:libreofficebase
LABEL {proj_label}

COPY --from={repo_project}/{hive}:processor /etc/wd /etc/wd

WORKDIR /etc/wd
