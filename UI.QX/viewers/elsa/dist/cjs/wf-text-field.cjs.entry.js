'use strict';

Object.defineProperty(exports, '__esModule', { value: true });

const __chunk_1 = require('./chunk-39c49cc2.js');

class TextField {
    constructor(hostRef) {
        __chunk_1.registerInstance(this, hostRef);
    }
    render() {
        const name = this.name;
        return (__chunk_1.h("host", null, __chunk_1.h("label", { htmlFor: name }, this.label), __chunk_1.h("input", { id: name, name: name, type: "text", class: "form-control", value: this.value }), __chunk_1.h("small", { class: "form-text text-muted" }, this.hint)));
    }
    static get style() { return ""; }
}

exports.wf_text_field = TextField;
