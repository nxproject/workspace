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

	@require(qx.core.Object)

************************************************************************ */

qx.Class.define('tools.MergeMap', {

    type: 'static',

    statics: {

        startindex: 'hidden',

        // This is what you override
        do: function (req) {

            // Assure
            req = req || {};

            nx.util.serviceCall('Docs.MapGet', {
                path: req.path
            }, function (result) {

                var items = [];
                var row = 1;

                var map = result.map;
                if (map && typeof map === 'string') map = JSON.parse(map);
                map = map || {};

                Object.keys(map).forEach(function (entry) {
                    items.push({
                        nxtype: 'string',
                        top: row,
                        left: 1,
                        width: nx.default.get('defaultinputWidth'),
                        label: entry,
                        value: map[entry]
                    });
                    row++;
                });

                var win = nx.desktop.addWindow({

                    nxid: 'mergemap_' + req.path,
                    caption: 'Merge map for ' + req.path.substr(1 + req.path.lastIndexOf('/')),
                    allowClose: false,
                    caller: req.caller,

                    items: items,

                    commands: {

                        items: [

                            {
                                label: 'Close',
                                icon: 'cancel',
                                click: function (e) {

                                    var self = this;

                                    // Map window
                                    var win = nx.bucket.getWin(self);

                                    // Callback
                                    if (req.atCancel) req.atCancel();

                                    // Close
                                    win.safeClose();

                                }

                            }, '>', {

                                label: 'Ok',
                                icon: 'accept',
                                click: function (e) {

                                    // Get the button
                                    var widget = nx.util.eventGetWidget(e);

                                    // Map window
                                    var win = nx.bucket.getWin(widget);

                                    // Get data
                                    var data = win.getFormData();
                                    // Save
                                    nx.util.serviceCall('Docs.MapPut', {
                                        path: req.path,
                                        map: data
                                    });

                                    // Callback
                                    if (req.atOk) req.atOk();

                                    // Close
                                    win.safeClose();
                                }

                            }

                        ]
                    }

                });
            });
        }

    }

});