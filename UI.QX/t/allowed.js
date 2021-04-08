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

qx.Class.define('t.allowed', {

    extend: qx.core.Object,

    implement: i.iTool,

    members: {

        caption: 'Explain',

        icon: 'cursor',

        allowed: function (widget, cb) {
            cb(true);
        },

        setup: function (widget, button) { },

        click: function (widget) {

            var self = this;

            // Get value
            var value = widget.getValue();
            // Any?
            if (nx.util.hasValue(value)) {
                // Parse
                nx.util.serviceCall('Office.ExplainAllowed', {
                    allowed: value
                }, function (result) {
                    nx.util.runTool('Message', {
                        caption: 'Explanation for ' + value,
                        msg: result.explanation,
                        caller: nx.bucket.getWin(widget)
                    });
                });
            }

        }
    }

});