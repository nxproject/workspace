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

qx.Class.define('t.xemaileditor', {

    extend: qx.core.Object,

    implement: i.iTool,

    members: {

        caption: 'Edit',

        icon: 'pencil',

        allowed: function (widget, cb) {
            cb(true);
        },

        setup: function (widget, button) { },

        click: function (widget) {

            var self = this;

            // Get stuff
            var win = nx.bucket.getWin(widget);
            var params = nx.bucket.getParams(win);

            // Get the data
            var data = win.getFormData();
            // Get the id
            var id = data.code || params.id;
            // And the captiob
            var caption = data.code || win.getCaption();

            // Save
            win.save(function () {

                // Call editor
                nx.fs.editEMAIL({
                    ds: params.ds,
                    id: id,
                    caption: caption
                });

                // Close
                win.safeClose();

            });

        }
    }

});