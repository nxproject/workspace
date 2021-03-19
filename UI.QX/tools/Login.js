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

qx.Class.define('tools.Login', {

    type: 'static',

    statics: {

        startindex: 'hidden',

        // This is what you override
        do: function (req) {

            nx.desktop.addWindow({

                nxid: 'login',
                caption: 'Login',
                center: 'both',
                defaultCommand: 'Ok',

                items: [

                    {
                        nxtype: 'lower',
                        top: 1,
                        left: 1,
                        width: 10,
                        label: 'Name'
                    }, {
                        nxtype: 'password',
                        top: 2,
                        left: 1,
                        width: 10,
                        label: 'Password'
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
                                var name = win.getValue('Name');
                                var pwd = win.getValue('Password');

                                // Login
                                nx.desktop.user.login(name, pwd);

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