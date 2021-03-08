import { r as registerInstance, h, H as Host, g as getElement } from './chunk-b380d234.js';
var SelectField = /** @class */ (function () {
    function SelectField(hostRef) {
        var _this = this;
        registerInstance(this, hostRef);
        this.renderItem = function (item) {
            var isGroup = !!item.options;
            return isGroup ? _this.renderGroup(item) : _this.renderOption(item);
        };
        this.renderOption = function (option) {
            var type = typeof (option);
            var label = null;
            var value = null;
            switch (type) {
                case 'string':
                    label = option;
                    value = option;
                    break;
                case 'number':
                    label = option.toString();
                    value = option.toString();
                    break;
                case 'object':
                    var pair = option;
                    label = pair.label;
                    value = pair.value;
                    break;
                default:
                    throw Error("Unsupported option type: " + type + ".");
            }
            var isSelected = value === _this.value;
            return h("option", { value: value, selected: isSelected }, label);
        };
        this.renderGroup = function (group) {
            return (h("optgroup", { label: group.label }, group.options.map(_this.renderOption)));
        };
    }
    SelectField.prototype.componentWillLoad = function () {
        var encodedJson = this.element.getAttribute('data-items');
        if (!encodedJson)
            return;
        var json = decodeURI(encodedJson);
        this.items = JSON.parse(json);
    };
    SelectField.prototype.render = function () {
        var name = this.name;
        var label = this.label;
        var items = this.items || [];
        return (h(Host, null, h("label", { htmlFor: name }, label), h("select", { id: name, name: name, class: "custom-select" }, items.map(this.renderItem)), h("small", { class: "form-text text-muted" }, this.hint)));
    };
    Object.defineProperty(SelectField.prototype, "element", {
        get: function () { return getElement(this); },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(SelectField, "style", {
        get: function () { return ""; },
        enumerable: true,
        configurable: true
    });
    return SelectField;
}());
export { SelectField as wf_select_field };
