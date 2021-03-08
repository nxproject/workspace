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

	@require(qx.core.Object)

************************************************************************ */

qx.Class.define('tools.TPTool', {

    type: 'static',

    statics: {

        caption: 'TPTool',
        icon: 'cancel',
        startindex: 'hidden',

        // This is what you override
        do: function (req) {

            // Do we have an id?
            if (!req.id) {
                nx.util.runTool('Input', {
                    label: 'Name',
                    atOk: function (name) {
                        if (name) {
                            req.id = req.idprefix + name;
                            req.tptool(req);
                        }
                    }
                });
            } else {
                req.tptool(req);
            }

        }

    }

});