'use strict';

Object.defineProperty(exports, '__esModule', { value: true });

const __chunk_1 = require('./chunk-39c49cc2.js');

class ListField {
    constructor(hostRef) {
        __chunk_1.registerInstance(this, hostRef);
    }
    render() {
        const name = this.name;
        const label = this.label;
        const items = this.items;
        return (__chunk_1.h(__chunk_1.Host, null, __chunk_1.h("label", { htmlFor: name }, label), __chunk_1.h("input", { id: name, name: name, type: "text", class: "form-control", value: items }), __chunk_1.h("small", { class: "form-text text-muted" }, this.hint)));
    }
    static get style() { return ""; }
}

exports.wf_list_field = ListField;
