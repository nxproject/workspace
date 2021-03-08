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

qx.Class.define('t.vin', {

    extend: qx.core.Object,

    implement: i.iTool,

    members: {

        caption: 'Fetch',

        icon: 'car',

        setup: function (widget, button) { },

        click: function (widget) {

            var start = nx.util.qrFill(widget, 'relvinnum');
            if (start) {

                nx.util.qrMap(widget, start, function (result) {

                    if (start.relvinnum) {

                        var url = 'https://vpic.nhtsa.dot.gov/api/vehicles/decodevinvalues/' + start.relvinnum + '?format=json';

                        $.get(url, {}, function (nhtsa) {

                            if (nhtsa && nhtsa.Results && nhtsa.Results.length) {

                                var info = nhtsa.Results[0];

                                // Get the window
                                var win = nx.bucket.getForm(widget);

                                // Fill
                                nx.util.qrSet(win, result, 'relvinyear', info.ModelYear);
                                nx.util.qrSet(win, result, 'relvinmake', info.Make);
                                nx.util.qrSet(win, result, 'relvinmodel', info.Model);
                                nx.util.qrSet(win, result, 'relvinseries', info.Series);
                                nx.util.qrSet(win, result, 'relvintype', info.VehicleType);
                                nx.util.qrSet(win, result, 'relvintrim', info.Trim);
                                nx.util.qrSet(win, result, 'relvindoors', info.Doors);
                                nx.util.qrSet(win, result, 'relvincyl', info.EngineCylinders);
                                nx.util.qrSet(win, result, 'relvindisp', info.DisplacementL);
                                nx.util.qrSet(win, result, 'relvinfuel', info.FuelTypePrimary);
                                nx.util.qrSet(win, result, 'relvinspeed', info.TransmissionSpeed);
                                nx.util.qrSet(win, result, 'relvintrans', info.TransmissionStyle);


                            }

                        }, 'json');
                    }
                });
            }

        }
    }

});