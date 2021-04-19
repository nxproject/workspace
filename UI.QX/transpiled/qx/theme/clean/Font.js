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
 * The ville.Clean font theme.
 * @asset(/transpiled//transpiled/qx/theme/nx/font/Lato/*.ttf)
 * 
 */
window.nxTheme.Font =
{

    fonts:
    {
        "default":
        {
            family: ["sans-serif"],
            color: "text",
            weight: "400",
            size: 14,
            sources: [
                {
                    family: "Lato",
                    source: [
                        "/transpiled/qx/theme/nx/font/Lato/Lato-Regular.ttf"
                    ]
                }
            ]
        },

        "bold":
        {
            include: "default",
            weight: "700"
        },

        "datechooser":
        {
            include: "default",
            size: 13
        },

        "datechooser-bold":
        {
            include: "bold",
            size: 13
        },

        "button":
        {
            include: "bold",
            size: window.nxTheme.Defaults.buttonFontSize
        },

        "groupbox-legend":
        {
            include: "bold"
        },

        "window-header":
        {
            include: "default",
            size: window.nxTheme.Defaults.winCaptionFontSize
        },

        "unicode-icons-sm":
        {
            size: 20,
            family: ["arial", "helvetica", "Segoe UI Symbol"]
        },

        "input":
        {
            include: "default",
            size: 14
        },


        "headline":
        {
            include: "default",
            size: 24
        },

        "small":
        {
            include: "default",
            size: 11
        },

        "monospace":
        {
            size: 11,
            family: ["DejaVu Sans Mono", "Courier New", "monospace"]
        },

        // Theme Browser Content Formatting
        "control-header":
        {
            include: "default",
            size: 24,
            bold: true
        },

        "control-header2":
        {
            include: "default",
            size: 20
        },

        // TreeVirtual Legacy
        "treevirtual":
        {
            include: "default",
            size: 8
        }
    }
};