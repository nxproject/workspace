/* ************************************************************************

   SQville Software

   http://sqville.com

   Copyright:
     None

   License:
     MIT

   Authors:
     * Chris Eskew (chris.eskew@sqville.com)

************************************************************************ */

/**
 * Clean color theme, based off of Simple color theme (qooxdoo) and Semantic default
 */
window.nxTheme.Color =
{
    aliases: {
        primarycolor: "#3b83c0"
    },



    colors:
    {
        //************************
        //*** Color Pallette 1 ***
        //************************
        "primary": [33, 133, 208], //#2185D0
        "secondary": "#1b1c1d",
        "tertiary": "#5bbd72",

        // main
        "background": "#ffffff",
        "light-background": "#E0ECFF",

        // backgrounds
        "background-selected": "#6694E3",
        "background-selected-disabled": "#CDCDCD",
        "background-selected-dark": "#5685D6",
        "background-disabled": "#F7F7F7",
        "background-disabled-checked": "#BBBBBB",
        "background-pane": "#ffffff",
        "background-invalid": "#fff0f0",
        "background-group-item": "#BABABA",

        // tabview
        "tabview-unselected": "#1866B5",
        "tabview-button-border": "#134983",
        "tabview-label-active-disabled": "#D9D9D9",
        "tabviewspacebar-bar-selected": "#888888",
        "tabview-text-normal": "#444444",
        "tabviewspot-button-checked": "#F2F2F2",
        "tabviewspot-button-hovered": "#F7F7F7",

        // combobox
        "combobox-hovered": "#F2F2F2",
        "combobox-item-selected": "#F7F7F7",

        // list
        "group-item-": "#ffffff",

        // text colors
        "link": "#24B",
        "group-item": "#ffffff",

        // scrollbar
        "scrollbar-bright": "#F1F1F1",
        "scrollbar-dark": "#EBEBEB",

        // form
        "button": "rgba(0, 0, 0, 0.6)",
        "button-border": "#BBB",
        "button-border-hovered": "#939393",
        "invalid": "#FF0000",

        "button-box-bright": "#e0e0e0",
        "button-box-bright-hovered": "#e8e8e8",
        "button-box-dark": "#E3E3E3",
        "button-box-bright-pressed": "#cccccc",
        "button-box-dark-pressed": "#F5F5F5",
        "border-lead": "#888888",
        "button-text": "rgba(0, 0, 0, 0.6)",
        "button-text-hovered": "rgba(0, 0, 0, 0.8)",
        "button-focus-shadow": qx.core.Environment.get("css.rgba") ? "rgba(81, 167, 232, 0.8)" : "#51A7E8",   //SQv New

        //SQ New
        "textfield-selected": qx.core.Environment.get("css.rgba") ? "rgba(133, 183, 217, 1)" : "#85b7d9",
        "textfield-selected-darker": qx.core.Environment.get("css.rgba") ? "rgba(0, 0, 0, 0.5)" : "#BDBEBE",

        //SQ New
        "progressbar-base": qx.core.Environment.get("css.rgba") ? "rgba(229, 229, 229, 1)" : "#E5E5E5",
        "progressbar-gray": qx.core.Environment.get("css.rgba") ? "rgba(136, 136, 136, 1)" : "#888888",
        "progressbar-complete": qx.core.Environment.get("css.rgba") ? "rgba(33, 186, 69, 1)" : "#21BA45",
        "progressbar-warning": qx.core.Environment.get("css.rgba") ? "rgba(242, 192, 55, 1)" : "#F2C037",
        "progressbar-error": qx.core.Environment.get("css.rgba") ? "rgba(219, 40, 40, 1)" : "#DB2828",

        //*** Primary Button colors
        "primary-button-box": "#3b83c0",
        "primary-button-box-hovered": "#458ac6",
        "primary-button-box-pressed": "#3576ac",
        "primary-button-text": "#ffffff",

        //*** Secondary Button colors
        "secondary-button-box": "#1b1c1d",
        "secondary-button-box-hovered": "#222425",
        "secondary-button-box-pressed": "#0a0a0b",
        "secondary-button-inset-shadow": "rgba(39, 41, 43, 0.15)",
        "secondary-button-text": "primary-button-text",

        //*** Tertiary Button colors
        "tertiary-button-box": "#5bbd72",
        "tertiary-button-box-hovered": "#66C17B",
        "tertiary-button-box-pressed": "#46AE5F",
        "tertiary-button-inset-shadow": "rgba(39, 41, 43, 0.15)",
        "tertiary-button-text": "primary-button-text",

        "ville-black": qx.core.Environment.get("css.rgba") ? "rgba(0, 0, 0, 1)" : "#000000",
        "ville-arrow-gray": qx.core.Environment.get("css.rgba") ? "rgba(0, 0, 0, 0.6)" : "#444444",
        "ville-arrow-med-gray": qx.core.Environment.get("css.rgba") ? "rgba(0, 0, 0, 0.4)" : "#444444",

        // window
        "window-border": window.nxTheme.Defaults.winBorderColor,
        "window-border-inner": window.nxTheme.Defaults.winBackgroundColor,

        // group box
        "white-box-border": "#D8D8D8",
        "box-border-blue": "#3b83c0",
        "box-border-orange": "#e07b53",
        "box-border-green": "#5bbd72",

        // shadows
        "shadow": qx.core.Environment.get("css.rgba") ? "rgba(0, 0, 0, 0.4)" : "#999999",
        "shadow-light": qx.core.Environment.get("css.rgba") ? "rgba(0, 0, 0, 0.2)" : "#CCCCCC",

        // borders
        // 'border-main' is an alias of 'background-selected' (compatibility reasons)
        "border-main": "#6694E3",
        "border-light": qx.core.Environment.get("css.rgba") ? "rgba(0, 0, 0, 0.15)" : "#D8D8D8",
        "border-light-darker": qx.core.Environment.get("css.rgba") ? "rgba(0, 0, 0, 0.2)" : "#D8D8D8",
        "border-light-shadow": qx.core.Environment.get("css.rgba") ? "rgba(0, 0, 0, 0.15)" : "#D8D8D8",
        "border-super-light": qx.core.Environment.get("css.rgba") ? "rgba(0, 0, 0, 0.06)" : "#EEEEEE",

        // separator
        "border-separator": "#808080",

        // text
        "text-combobox-listitem": "rgba(0, 0, 0, 0.6)", //NOT USED
        "text": "rgba(0, 0, 0, 0.87)",
        "text-darker": "rgba(0, 0, 0, 0.9)",
        "text-disabled": "rgba(0, 0, 0, 0.4)",
        "text-selected": "#000000",
        "text-placeholder": "rgba(0, 0, 0, 0.4)",

        // tooltip
        "tooltip": "#FFFFE1",
        "tooltip-text": "#000000",

        // table
        "table-border": "#DEDEDE",
        "table-header": "#F9FAFB",
        "table-focus-indicator": [179, 217, 255],

        // used in table code
        "table-header-cell": [235, 234, 219],
        "table-row-background-focused-selected": [90, 138, 211],
        "table-row-background-focused": [221, 238, 255],
        "table-row-background-selected": [51, 94, 168],
        "table-row-background-even": "#ffffff",
        "table-row-background-odd": "#ffffff",
        "table-row-selected": [255, 255, 255],
        "table-row": [0, 0, 0],
        "table-row-line": "#EEE",
        "table-column-line": "#EEE",

        // used in progressive code
        "progressive-table-header": "#AAAAAA",
        "progressive-table-row-background-even": [250, 248, 243],
        "progressive-table-row-background-odd": [255, 255, 255],
        "progressive-progressbar-background": "gray",
        "progressive-progressbar-indicator-done": "#CCCCCC",
        "progressive-progressbar-indicator-undone": "#ffffff",
        "progressive-progressbar-percent-background": "gray",
        "progressive-progressbar-percent-text": "#ffffff",

        "desktop": window.nxTheme.Defaults.desktopColor
    }
}
