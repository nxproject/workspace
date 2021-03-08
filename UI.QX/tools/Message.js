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

qx.Class.define('tools.Message', {

    type: 'static',

    statics: {

        startindex: 'hidden',

        // This is what you override
        do: function (req) {

            // Assure
            req = req || {};

            var cmds = [

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
            ];

            if (req.atOk) {
                cmds.push('>');
                cmds.push({

                    label: 'Ok',
                    icon: 'accept',
                    click: function (e) {

                        // Get the button
                        var widget = nx.util.eventGetWidget(e);

                        // Map window
                        var win = nx.bucket.getWin(widget);

                        // Get values
                        var value = win.getValue(req.label);

                        // Callback
                        if (req.atOk) req.atOk(value);

                        // Close
                        win.safeClose();
                    }

                });
            }

            var win = nx.desktop.addWindow({

                nxid: 'confirm',
                caption: req.caption,
                center: 'both',
                defaultCommand: 'Ok',
                allowClose: false,
                caller: req.caller,

                items: [

                    {
                        nxtype: 'textarea',
                        top: 1,
                        left: 1,
                        width: nx.default.get('default.inputWidth'),
                        height: req.height || 10,
                        label: req.label || '',
                        value: req.msg
                    }
                ],

                commands: {

                    items: cmds
                }

            });

            //
            win.setModal(true);
        }

    }

});