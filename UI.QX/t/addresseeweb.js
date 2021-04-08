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

qx.Class.define('t.addresseeweb', {

    extend: qx.core.Object,

    implement: i.iTool,

    members: {

        caption: nx.setup.viaWeb + 'Contact',

        icon: nx.setup.viaWebIcon,

        allowed: function (widget, cb) {
            cb(true);
        },

        setup: function (widget, button) { },

        click: function (widget) {

            var start = nx.util.qrFill(widget, 'relname');
            if (start) {

                nx.util.qrBuild(widget, start, function (result) {

                    var data = 'MECARD:';

                    data += 'N:' + (result.relname || '') + ';';
                    data += 'ADR:,,' + (result.reladdr || '') + ',' + (result.relcity || '') + ',' + (result.relstate || '') + ',' + (result.relzip || '') + ';';
                    data += 'EMAIL:' + (result.relemail || '') + ';';
                    data += 'TEL:' + (result.relphone || '') + ';';

                    data += ';';

                    nx.util.runTool('QR', {
                        data: data,
                        caption: nx.setup.viaWeb + 'Contact'
                    });

                });

            }

        }
    }

});