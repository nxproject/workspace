import { r as registerInstance, h } from './chunk-b380d234.js';
var TextField = /** @class */ (function () {
    function TextField(hostRef) {
        registerInstance(this, hostRef);
    }
    TextField.prototype.render = function () {
        var name = this.name;
        return (h("host", null, h("label", { htmlFor: name }, this.label), h("input", { id: name, name: name, type: "text", class: "form-control", value: this.value }), h("small", { class: "form-text text-muted" }, this.hint)));
    };
    Object.defineProperty(TextField, "style", {
        get: function () { return ""; },
        enumerable: true,
        configurable: true
    });
    return TextField;
}());
export { TextField as wf_text_field };
