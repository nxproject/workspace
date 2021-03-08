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

qx.Class.define('tools.MakeSite', {

    type: 'static',

    statics: {

        startindex: 'hidden',
        startgroup: 'System',
        startpriority: 'K',

        // This is what you override
        do: function (req) {

            var virt = nx.desktop.user.getIsVirtual();

            nx.desktop.addWindow({

                caption: 'Make Site',
                defaultCommand: 'Ok',

                items: [

                    {
                        nxtype: 'keyword',
                        top: 1,
                        left: 1,
                        width: 'default.fieldWidth',
                        label: 'ID'
                    }, {
                        nxtype: 'string',
                        top: 2,
                        left: 1,
                        width: 'default.fieldWidth',
                        label: 'IP'
                    }, {
                        nxtype: 'string',
                        top: 3,
                        left: 1,
                        width: 'default.fieldWidth',
                        label: 'Domain'
                    }, {
                        nxtype: 'email',
                        top: 4,
                        left: 1,
                        width: 'default.fieldWidth',
                        label: 'SSL Email'
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
                                var id = win.getValue('ID');
                                var ip = win.getValue('IP');
                                var domain = win.getValue('Domain');
                                var email = win.getValue('SSL Email');

                                // Login
                                nx.util.serviceCall('Office.MakeSite', {
                                    id: id,
                                    ip: ip,
                                    domain: domain,
                                    email: email
                                }, function (result) {
                                    if (result) {
                                        var msg = result.message || 'Unknown error!';
                                        if (msg.indexOf('!') == -1) {
                                            nx.util.notifyInfo(msg);
                                        } else {
                                            nx.util.notifyError(msg);
                                        }
                                    } else {
                                        nx.util.notifyError('Internal error, retry');
                                    }
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
}

});