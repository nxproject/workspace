# Based on https://github.com/occrp-attic/convert-document

FROM {repo_project}/libreofficebase:{tier}
LABEL {proj_label}

COPY --from={repo_project}/processor:{tier} /etc/wd /etc/wdjj

WORKDIR /etc/wd
