/* ************************************************************************

   UI.QX - a dynamic web interface

   http://qooxdoo.org

   Copyright:
     2020-2021 Jose E. Gonzalez (nxoffice2021@gmail.com)

   License:
     MIT: https://opensource.org/licenses/MIT
     See the LICENSE file in the project's top-level directory for details.

   Authors:
     * Jose E. Gonzalez

************************************************************************ */

(function () {

    // Read shared
    var options = window.nxTheme.Options;

    if (options && typeof options === 'object') {
        // Loop thru
        Object.keys(options).forEach(function (key) {
            window.nxTheme.Defaults[key] = options[key];
        });
    }

    /**
     * Build all of the theme blocks
     */
    qx.Theme.define("qx.theme.nx.Appearance", window.nxTheme.Appearance);
    qx.Theme.define("qx.theme.nx.Color", window.nxTheme.Color);
    qx.Theme.define("qx.theme.nx.Decoration", window.nxTheme.Decoration);
    qx.Theme.define("qx.theme.nx.Font", window.nxTheme.Font);
    qx.Class.define("qx.theme.nx.Image", window.nxTheme.Image);

})();