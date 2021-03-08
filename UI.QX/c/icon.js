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

qx.Class.define('c.icon', {

    extend: c.combobox,

    statics: {

        makeSelf: function (req) {

            var widget = new c.icon(req);

            // Setup
            nx.setup.choices(nx.bucket.getWidgets(widget)[0], {
                choices: nx.desktop.user.getIcons()
            });

            return widget;

        }
    }

});