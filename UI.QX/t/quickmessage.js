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

qx.Class.define('t.quickmessage', {

    extend: qx.core.Object,

    implement: i.iTool,

    members: {

        caption: 'Quick Message',

        icon: 'lightning',

        allowed: function (widget, cb) {
            cb(nx.desktop.user.getIsSelector('QUICK'));
        },

        when: function (widget, button) {
        },

        setup: function (widget, button) { },

        click: function (widget) {

            var value = widget.getValue();
            if (nx.util.hasValue(value)) {
                // Select
                nx.util.runTool('View', {
                    ds: '_quickmessages',
                    onSelect: function (e, data) {
                        var fn = (nx.util.isPhone(value) ? 'sms' : 'email');
                        data.forEach(function (row) {
                            // 
                            nx.util.serviceCall('Communication.Process', {
                                cmd: fn,
                                to: value,
                                subject: row.subj,
                                message: row.msg,
                                att: [],
                                template: row.temp || '',
                                campaign: '',
                                telemetry: 'n',
                                mlink: ''
                            });
                        });
                    }
                });
            }

        }
    }

});