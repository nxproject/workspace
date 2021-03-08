'use strict';

Object.defineProperty(exports, '__esModule', { value: true });

const __chunk_1 = require('./chunk-39c49cc2.js');

class BooleanField {
    constructor(hostRef) {
        __chunk_1.registerInstance(this, hostRef);
    }
    render() {
        const name = this.name;
        return (__chunk_1.h("div", { class: "form-group" }, __chunk_1.h("div", { class: "form-check" }, __chunk_1.h("input", { id: name, name: name, class: "form-check-input", type: "checkbox", value: 'true', checked: this.checked }), __chunk_1.h("label", { class: "form-check-label", htmlFor: name }, this.label)), __chunk_1.h("small", { class: "form-text text-muted" }, this.hint)));
    }
    static get style() { return ""; }
}

exports.wf_boolean_field = BooleanField;
