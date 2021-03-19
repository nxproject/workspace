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

qx.Class.define('t.account', {

    extend: qx.core.Object,

    implement: i.iTool,

    members: {

        caption: 'Set Password',

        icon: 'key',

        setup: function (widget, button) { },

        click: function (widget) {

            var self = this;

            // Get the value
            var value = widget.getValue();
            // Any?
            if (value) {
                //
                var win = nx.bucket.getWin(widget);
                // Save
                win.save();

                // Get the params
                var params = nx.bucket.getParams(widget);

                var a = 1;
            }

        }
    }

});