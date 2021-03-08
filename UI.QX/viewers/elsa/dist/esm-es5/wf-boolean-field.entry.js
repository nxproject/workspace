import { r as registerInstance, h } from './chunk-b380d234.js';
var BooleanField = /** @class */ (function () {
    function BooleanField(hostRef) {
        registerInstance(this, hostRef);
    }
    BooleanField.prototype.render = function () {
        var name = this.name;
        return (h("div", { class: "form-group" }, h("div", { class: "form-check" }, h("input", { id: name, name: name, class: "form-check-input", type: "checkbox", value: 'true', checked: this.checked }), h("label", { class: "form-check-label", htmlFor: name }, this.label)), h("small", { class: "form-text text-muted" }, this.hint)));
    };
    Object.defineProperty(BooleanField, "style", {
        get: function () { return ""; },
        enumerable: true,
        configurable: true
    });
    return BooleanField;
}());
export { BooleanField as wf_boolean_field };
