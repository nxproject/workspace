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

qx.Class.define('tools.QR', {

    type: 'static',

    statics: {

        startindex: 'hidden',

        // This is what you override
        do: function (req) {

            // Assure
            req = req || {};

            var value = nx.util.makeQRCode(req.data);

            var win = nx.desktop.addWindow({

                caption: req.caption,
                center: 'both',
                allowClose: false,
                caller: req.caller,
                icon: 'office',

                items: [

                    {
                        nxtype: 'image',
                        top: 1,
                        left: 1,
                        width: nx.default.get('default.QRWidth'),
                        height: nx.default.get('default.QRHeight'),
                        label: '',
                        value: value,
                        contextMenu: {
                            items: [
                                {
                                    label: 'Print',
                                    icon: 'printer',
                                    click: function (e) {
                                        printJS({
                                            printable: value,
                                            type: 'image'
                                        });
                                    }
                                }
                            ]
                        }

                    }
                ],

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

                        }

                    ]
                }

            });

            //
            win.setModal(true);
        }

    }

});