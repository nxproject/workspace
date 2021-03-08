import { h } from "@stencil/core";
export class ExportButton {
    constructor() {
        this.workflowFormats = {
            json: {
                format: 'json',
                fileExtension: '.json',
                mimeType: 'application/json',
                displayName: 'JSON'
            },
            yaml: {
                format: 'yaml',
                fileExtension: '.yaml',
                mimeType: 'application/x-yaml',
                displayName: 'YAML'
            },
            xml: {
                format: 'xml',
                fileExtension: '.xml',
                mimeType: 'application/xml',
                displayName: 'XML'
            },
            object: {
                format: 'object',
                fileExtension: '.bin',
                mimeType: 'application/binary',
                displayName: 'Binary'
            }
        };
        this.getWorkflowHost = () => {
            return !!this.designerHostId ? document.querySelector(`#${this.designerHostId}`) : null;
        };
        this.handleExportClick = async (e, descriptor) => {
            e.preventDefault();
            this.exportClickedEvent.emit(descriptor);
            const host = this.getWorkflowHost();
            if (!!host) {
                await host.export(descriptor);
            }
        };
    }
    render() {
        const descriptors = this.workflowFormats;
        return (h("div", { class: "dropdown" },
            h("button", { class: "btn btn-secondary dropdown-toggle", type: "button", id: "exportButton", "data-toggle": "dropdown", "aria-haspopup": "true", "aria-expanded": "false" }, "Export"),
            h("div", { class: "dropdown-menu", "aria-labelledby": "exportButton" }, Object.keys(descriptors).map(key => {
                const descriptor = descriptors[key];
                return (h("a", { class: "dropdown-item", href: "#", onClick: e => this.handleExportClick(e, descriptor) }, descriptor.displayName));
            }))));
    }
    static get is() { return "wf-export-button"; }
    static get properties() { return {
        "designerHostId": {
            "type": "string",
            "mutable": false,
            "complexType": {
                "original": "string",
                "resolved": "string",
                "references": {}
            },
            "required": false,
            "optional": false,
            "docs": {
                "tags": [],
                "text": ""
            },
            "attribute": "workflow-designer-host",
            "reflect": false
        },
        "workflowFormats": {
            "type": "unknown",
            "mutable": false,
            "complexType": {
                "original": "WorkflowFormatDescriptorDictionary",
                "resolved": "{ [key: string]: WorkflowFormatDescriptor; }",
                "references": {
                    "WorkflowFormatDescriptorDictionary": {
                        "location": "import",
                        "path": "../../../models"
                    }
                }
            },
            "required": false,
            "optional": false,
            "docs": {
                "tags": [],
                "text": ""
            },
            "defaultValue": "{\n    json: {\n      format: 'json',\n      fileExtension: '.json',\n      mimeType: 'application/json',\n      displayName: 'JSON'\n    },\n    yaml: {\n      format: 'yaml',\n      fileExtension: '.yaml',\n      mimeType: 'application/x-yaml',\n      displayName: 'YAML'\n    },\n    xml: {\n      format: 'xml',\n      fileExtension: '.xml',\n      mimeType: 'application/xml',\n      displayName: 'XML'\n    },\n    object: {\n      format: 'object',\n      fileExtension: '.bin',\n      mimeType: 'application/binary',\n      displayName: 'Binary'\n    }\n  }"
        }
    }; }
    static get events() { return [{
            "method": "exportClickedEvent",
            "name": "export",
            "bubbles": true,
            "cancelable": true,
            "composed": true,
            "docs": {
                "tags": [],
                "text": ""
            },
            "complexType": {
                "original": "any",
                "resolved": "any",
                "references": {}
            }
        }]; }
    static get elementRef() { return "el"; }
}
