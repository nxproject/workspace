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

qx.Class.define('tools.QM', {

    type: 'static',

    statics: {

        startindex: 'hidden',
        startprivilege: 'QMXX',

        // This is what you override
        do: function (req) {

            var virt = nx.desktop.user.getIsAccount();

            nx.desktop.addWindow({

                caption: 'Messaging',
                icon: 'user_comment',
                defaultCommand: 'Ok',

                items: [

                    {
                        nxtype: virt ? 'string' : 'user',
                        top: 1,
                        left: 1,
                        width: 'default.fieldWidth',
                        label: 'To',
                        value: req.to,
                        ro: virt ? 'y' : 'n'
                    }, {
                        nxtype: 'textarea',
                        top: 2,
                        left: 1,
                        width: 'default.fieldWidth',
                        height: 'default.textareaHeight',
                        label: 'Message'
                    }
                ],

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
                                var name = win.getValue('To') || req.to;
                                var msg = win.getValue('Message');

                                // Login
                                nx.util.sendNotify(name, msg);

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