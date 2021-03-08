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

qx.Class.define('tools.Comm', {

    type: 'static',

    statics: {

        startindex: 'hidden',

        // This is what you override
        do: function (req) {

            var row = 1;
            var items = [];

            if (req.askto) {
                items.push({
                    nxtype: 'string',
                    top: row,
                    left: 1,
                    width: 'default.fieldWidth',
                    label: 'To'
                });
                row++;
            }
            items.push({
                nxtype: 'string',
                top: row,
                left: 1,
                width: 'default.fieldWidth',
                label: 'Subject'
            });
            row++;
            items.push({
                nxtype: 'string',
                top: row,
                left: 1,
                width: 'default.fieldWidth',
                label: 'Message'
            });

            nx.desktop.addWindow({

                caption: req.fslabel,
                defaultCommand: 'Ok',
                caller: req.caller,

                items: items,

                commands: {

                    items: [

                        '>', {

                            label: 'Ok',
                            icon: 'accept',
                            click: function (e) {

                                // Get the button
                                var widget = nx.util.eventGetWidget(e);

                                // Map window
                                var win = nx.bucket.getWin(widget);

                                // Get values
                                var subj = win.getValue('Subject') || req.to;
                                var msg = win.getValue('Message');
                                if (req.askto) {
                                    req.value = win.getValue('To');
                                }

                                // 
                                nx.util.serviceCall('Comm.Process', {
                                    fn: req.fsfn,
                                    to: req.value,
                                    subject: subj,
                                    message: msg,
                                    attachments: req.attachments
                                });

                                // Close
                                win.safeClose();
                            }

                        }

                    ]
                }

            });

        }

    }

});