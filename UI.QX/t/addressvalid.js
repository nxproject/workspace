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

qx.Class.define('t.addressvalid', {

    extend: qx.core.Object,

    implement: i.iTool,

    members: {

        caption: 'Validate',

        icon: 'pin',

        allowed: function (widget, cb) {
            cb(nx.desktop.user.getSIField('psapi'));
        },

        setup: function (widget, button) { },

        click: function (widget) {

            var start = nx.util.qrFill(widget, 'reladdr');
            if (start) {

                nx.util.qrBuild(widget, start, function (result) {

                    nx.util.addressLookup(result.reladdr, result.relcity, result.relstate, result.relzip, function (addr, city, state, zip, conf) {

                        // Must be a match
                        if (conf >= 0.9) {

                            // Get the window
                            var win = nx.bucket.getForm(widget);

                            // Fill
                            nx.util.qrSet(win, result, 'reladdr', addr);
                            nx.util.qrSet(win, result, 'relcity', city);
                            nx.util.qrSet(win, result, 'relstate', state);
                            nx.util.qrSet(win, result, 'relzip', zip);

                        } else {
                            nx.util.notifyWarning('Not enough information');
                        }
                    });

                });
            }

        }
    }

});