'use strict';

Object.defineProperty(exports, '__esModule', { value: true });

const __chunk_1 = require('./chunk-39c49cc2.js');

class ExpressionField {
    constructor(hostRef) {
        __chunk_1.registerInstance(this, hostRef);
        this.selectSyntax = (syntax) => this.syntax = syntax;
        this.onSyntaxOptionClick = (e, syntax) => {
            e.preventDefault();
            this.selectSyntax(syntax);
        };
        this.renderInputField = () => {
            const name = this.name;
            const value = this.value;
            if (this.multiline)
                return __chunk_1.h("textarea", { id: name, name: `${name}.expression`, class: "form-control", rows: 3 }, value);
            return __chunk_1.h("input", { id: name, name: `${name}.expression`, value: value, type: "text", class: "form-control" });
        };
    }
    render() {
        const name = this.name;
        const label = this.label;
        const hint = this.hint;
        const syntaxes = ['Eval'];
        const selectedSyntax = this.syntax || 'Eval';
        return (__chunk_1.h("host", null, __chunk_1.h("label", { htmlFor: name }, label), __chunk_1.h("div", { class: "input-group" }, __chunk_1.h("input", { name: `${name}.syntax`, value: selectedSyntax, type: "hidden" }), this.renderInputField(), __chunk_1.h("div", { class: "input-group-append" }, __chunk_1.h("button", { class: "btn btn-primary dropdown-toggle", type: "button", id: `${name}_dropdownMenuButton`, "data-toggle": "dropdown", "aria-haspopup": "true", "aria-expanded": "false" }, selectedSyntax), __chunk_1.h("div", { class: "dropdown-menu", "aria-labelledby": `${name}_dropdownMenuButton` }, syntaxes.map(x => __chunk_1.h("a", { onClick: e => this.onSyntaxOptionClick(e, x), class: "dropdown-item", href: "#" }, x))))), __chunk_1.h("small", { class: "form-text text-muted" }, hint)));
    }
    static get style() { return "wf-expression-field .input-group>.form-control:not(:last-child){border-top-left-radius:.25rem;border-bottom-left-radius:.25rem}"; }
}

exports.wf_expression_field = ExpressionField;
