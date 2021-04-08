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

qx.Class.define('t.addressweb', {

    extend: qx.core.Object,

    implement: i.iTool,

    members: {

        caption: nx.setup.viaWeb + 'Map',

        icon: nx.setup.viaWebIcon,

        allowed: function (widget, cb) {
            cb(nx.desktop.user.getSIField('psapi'));
        },

        setup: function (widget, button) { },

        click: function (widget) {

            var start = nx.util.qrFill(widget, 'reladdr');
            if (start) {

                nx.util.qrBuild(widget, start, function (result) {

                    // Geocode
                    nx.util.geocode(result.reladdr, result.relcity, result.relstate, result.relzip, function (lat, long) {

                        var data = 'geo:' + lat + ',' + long;

                        nx.util.runTool('QR', {
                            data: data,
                            caption: nx.setup.viaWeb + 'Map'
                        });
                    });

                });
            }

        }
    }

});