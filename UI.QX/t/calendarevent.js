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

qx.Class.define('t.calendarevent', {

    extend: qx.core.Object,

    implement: i.iTool,

    members: {

        caption: nx.setup.viaWeb + 'Calendar',

        icon: nx.setup.viaWebIcon,

        allowed: function (widget, cb) {
            cb(true);
        },

        setup: function (widget, button) { },

        click: function (widget) {

            var self = this;

            var start = nx.util.qrFill(widget, 'relstarton');
            if (start) {

                nx.util.qrBuild(widget, start, function (result) {

                    var data = 'BEGIN:VEVENT\r\n';

                    data += 'SUMMARY:' + (result.relsubj || '') + '\r\n';
                    data += 'DESCRIPTION:' + (result.reldesc || '') + '\r\n';
                    data += 'LOCATION:' + (result.relloc || '') + '\r\n';
                    data += 'DTSTART:' + self.formatDate(result.relstarton) + '\r\n';
                    if (nx.util.hasValue(result.relendon)) {
                        data += 'DTEND:' + self.formatDate(result.relendon) + '\r\n';
                    } else {
                        data += 'DTEND:' + self.formatDate(result.relstarton) + '\r\n';
                    }

                    data += 'END:VEVENT';

                    nx.util.runTool('QR', {
                        data: data,
                        caption: nx.setup.viaWeb + 'Calendar'
                    });

                });

            }

        },

        formatDate: function (date) {

            var ans = new Date(date).toISOString();

            //return ans.replace(/-/g, '').replace(/:/g, '').substr(0, 13) + 'Z';
            //return ans.replace(/-/g, '').replace(/:/g, '').substr(0, 8);
            return nx.util.alphaNum(ans).substr(0, 15) + 'Z';
        }
    }

});