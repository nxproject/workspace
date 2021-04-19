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
/* ************************************************************************


************************************************************************* */
/**
 * Mapping class for all images used in the clean theme.
 *
 * @asset(qx/static/blank.png)
 * @asset(decoration/checkbox/*)
 * @asset(decoration/arrows/*)
 * @asset(decoration/slider/*)
 * @asset(decoration/tree/*)
 * @asset(decoration/table/*)
 * @asset(decoration/tabview/*)
 * @asset(decoration/colorselector/*)
 * @asset(decoration/treevirtual/*)
 * @asset(decoration/cursor/*)
 */
nxTheme.Image =
{
    extend: qx.core.Object,

    statics:
    {
        /**
         * Holds a map containig all the URL to the images.
         * @internal
         */
        URLS:
        {
            "blank": "qx/static/blank.png",

            // checkbox
            "checkbox-checked": "/transpiled/qx/theme/nx/decoration/checkbox/checkbox-check.svg",
            "checkbox-undetermined": "/transpiled/qx/theme/nx/decoration/checkbox/undetermined.png",  //Replaced with -- pure Qx -- Decoration entry:: checkbox-undetermined
            "checkbox-checked-disabled": "/transpiled/qx/theme/nx/decoration/checkbox/checkbox-check-disabled.svg",

            // window
            //"window-minimize" : "/transpiled/qx/theme/nx/decoration/window/minimize.gif", //Replaced with -- pure Qx -- Decoration entry:: window-button-minimize-icon
            //"window-maximize" : "/transpiled/qx/theme/nx/decoration/window/maximize.gif", //Replaced with -- pure Qx -- Decoration entry:: window-button-maximize-icon
            //"window-restore" : "/transpiled/qx/theme/nx/decoration/window/restore.gif", //Replaced with -- pure Qx -- Decoration entry:: window-button-restore
            //"window-close" : "/transpiled/qx/theme/nx/decoration/window/close.gif",

            // cursor
            "cursor-copy": "/transpiled/qx/theme/nx/decoration/cursor/circle-add.svg",
            //"cursor-move" : "/transpiled/qx/theme/nx/decoration/cursors/move.gif", //Replaced with -- pure Qx --
            "cursor-alias": "/transpiled/qx/theme/nx/decoration/cursor/alias.svg",
            "cursor-nodrop": "/transpiled/qx/theme/nx/decoration/cursor/nodrop.svg",

            // arrows
            "arrow-right": "/transpiled/qx/theme/nx/decoration/arrows/right.gif", //Replaced with -- pure Qx -- Decoration entry:: sqv-css-icon-arrow-right
            "arrow-left": "/transpiled/qx/theme/nx/decoration/arrows/left.gif", //Replaced with -- pure Qx -- Decoration entry:: sqv-css-icon-arrow-left
            "arrow-up": "/transpiled/qx/theme/nx/decoration/arrows/up.gif",
            "arrow-down": "/transpiled/qx/theme/nx/decoration/arrows/down.gif",
            //"arrow-forward" : "/transpiled/qx/theme/nx/decoration/arrows/forward.gif", //Replaced by Qx code
            //"arrow-rewind" : "/transpiled/qx/theme/nx/decoration/arrows/rewind.gif", //Replaced by Qx code
            "arrow-down-small": "/transpiled/qx/theme/nx/decoration/arrows/down-small.gif", //Embed option - Decoration entry:: sqv-css-icon-arrow-down-small
            "arrow-up-small": "/transpiled/qx/theme/nx/decoration/arrows/up-small.gif",  //Replaced by Decoration entry:: sqv-css-icon-arrow-up-small
            "arrow-up-invert": "/transpiled/qx/theme/nx/decoration/arrows/up-invert.gif", //Replaced by Decoration entry:: sqv-css-icon-arrow-up-invert
            "arrow-down-invert": "/transpiled/qx/theme/nx/decoration/arrows/down-invert.gif", //Replaced by Decoration entry:: sqv-css-icon-arrow-down-invert
            "arrow-right-invert": "/transpiled/qx/theme/nx/decoration/arrows/right-invert.gif", //Replaced by Decoration entry:: sqv-css-icon-arrow-right-invert

            // slider
            "line": "/transpiled/qx/theme/nx/decoration/slider/line.png",
            "line-selected": "/transpiled/qx/theme/nx/decoration/slider/line-selected.png",
            "line-invalid": "/transpiled/qx/theme/nx/decoration/slider/line-invalid.png",

            // split pane
            //"knob-horizontal" : "/transpiled/qx/theme/nx/decoration/splitpane/knob-horizontal.png", //Replaced by pure Qx
            //"knob-vertical" : "/transpiled/qx/theme/nx/decoration/splitpane/knob-vertical.png", // Replaced by pure Qx

            // tree
            "tree-folder": "/transpiled/qx/theme/nx/decoration/tree/folder.svg",
            "tree-folder-open": "/transpiled/qx/theme/nx/decoration/tree/folder-open.svg",
            "tree-file": "/transpiled/qx/theme/nx/decoration/tree/file.svg",
            //"tree-minus" : "/transpiled/qx/theme/nx/decoration/tree/minus.gif", //Replaced
            //"tree-plus" : "/transpiled/qx/theme/nx/decoration/tree/plus.gif", //Replaced

            // table
            //"select-column-order" : "/transpiled/qx/theme/nx/decoration/table/select-column-order.png", //Replaced by pure Qx
            //"table-ascending" : "/transpiled/qx/theme/nx/decoration/table/ascending.png",  //Not used
            //"table-descending" : "/transpiled/qx/theme/nx/decoration/table/descending.png", //Not used

            // tree virtual
            "tree-minus": "/transpiled/qx/theme/nx/decoration/treevirtual/minus.gif",
            "tree-plus": "/transpiled/qx/theme/nx/decoration/treevirtual/plus.gif",
            "treevirtual-line": "/transpiled/qx/theme/nx/decoration/treevirtual/line.gif",
            "treevirtual-minus-only": "/transpiled/qx/theme/nx/decoration/treevirtual/only_minus.gif",
            "treevirtual-plus-only": "/transpiled/qx/theme/nx/decoration/treevirtual/only_plus.gif",
            "treevirtual-minus-start": "/transpiled/qx/theme/nx/decoration/treevirtual/start_minus.gif",
            "treevirtual-plus-start": "/transpiled/qx/theme/nx/decoration/treevirtual/start_plus.gif",
            "treevirtual-minus-end": "/transpiled/qx/theme/nx/decoration/treevirtual/end_minus.gif",
            "treevirtual-plus-end": "/transpiled/qx/theme/nx/decoration/treevirtual/end_plus.gif",
            "treevirtual-minus-cross": "/transpiled/qx/theme/nx/decoration/treevirtual/cross_minus.gif",
            "treevirtual-plus-cross": "/transpiled/qx/theme/nx/decoration/treevirtual/cross_plus.gif",
            "treevirtual-end": "/transpiled/qx/theme/nx/decoration/treevirtual/end.gif",
            "treevirtual-cross": "/transpiled/qx/theme/nx/decoration/treevirtual/cross.gif",

            // menu
            //"menu-checkbox" : "/transpiled/qx/theme/nx/decoration/menu/checkbox.gif", //Replaced with Qx + CSS
            //"menu-checkbox-invert" : "/transpiled/qx/theme/nx/decoration/menu/checkbox-invert.gif", //Replaced with Qx + CSS
            //"menu-radiobutton-invert" : "/transpiled/qx/theme/nx/decoration/menu/radiobutton-invert.gif", //Replaced with Qx + CSS
            //"menu-radiobutton" : "/transpiled/qx/theme/nx/decoration/menu/radiobutton.gif", //Replaced with pure Qx

            // tabview
            "tabview-close": "/transpiled/qx/theme/nx/decoration/tabview/close.svg",
            "tabview-close-hovered": "/transpiled/qx/theme/nx/decoration/tabview/close-hovered.svg"
        }
    }
};