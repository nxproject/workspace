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

qx.Class.define('t.email', {

    extend: qx.core.Object,

    implement: i.iTool,

    members: {

        caption: 'Send email',

        icon: 'email',

        allowed: function (widget, cb) {
            cb(nx.desktop.user.getIsSelector('EMAIL'));
        },

        setup: function (widget, button) { },

        click: function (widget) {

            var value = widget.getValue();
            if (nx.util.hasValue(value) && nx.util.isEMail(value)) {

                var win = nx.bucket.getWin(widget);
                var params = nx.bucket.getParams(win);
                nx.util.runTool('Documents', {
                    fsfn: 'email',
                    fslabel: 'EMail',
                    fsicon: 'email',
                    ds: params.ds,
                    id: params.id,
                    caller: win,
                    fullcaption: 'Select documents to EMail',
                    value: value
                });

            }

        }
    }

});