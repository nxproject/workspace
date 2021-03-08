import { r as registerInstance, h, H as Host } from './chunk-b380d234.js';
var ListField = /** @class */ (function () {
    function ListField(hostRef) {
        registerInstance(this, hostRef);
    }
    ListField.prototype.render = function () {
        var name = this.name;
        var label = this.label;
        var items = this.items;
        return (h(Host, null, h("label", { htmlFor: name }, label), h("input", { id: name, name: name, type: "text", class: "form-control", value: items }), h("small", { class: "form-text text-muted" }, this.hint)));
    };
    Object.defineProperty(ListField, "style", {
        get: function () { return ""; },
        enumerable: true,
        configurable: true
    });
    return ListField;
}());
export { ListField as wf_list_field };
