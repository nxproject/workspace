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

qx.Class.define('t.phonecall', {

    extend: qx.core.Object,

    implement: i.iTool,

    members: {

        caption: 'Make call',

        icon: 'user_comment',

        allowed: function (widget, cb) {
            cb(nx.desktop.user.getIsSelector('TWILIO'));
        },

        when: function (widget, button) {
            //
            nx.contextMenu.isPhone(widget, button);
        },

        setup: function (widget, button) { },

        click: function (widget) {

            var value = widget.getValue();
            if (nx.util.hasValue(value) && nx.util.isPhone(value)) {

                var win = nx.bucket.getWin(widget);
                var params = nx.bucket.getParams(win);

                // Save
                win.save(function () {
                    // Call

                    nx.util.serviceCall('Communication.Process', {
                        cmd: 'voice',
                        to: value
                    });
                });

            }
        }
    }

});