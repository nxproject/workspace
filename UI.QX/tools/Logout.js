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

qx.Class.define('tools.Logout', {

    type: 'static',

    statics: {

        caption: 'Logout',
        icon: 'cancel',
        startindex: '99999',

        // This is what you override
        do: function (req) {

            if (nx.desktop.getChildWindows().length) {

                // Confirm
                nx.util.confirm('Before you logout...', 'Ok to lose any work in progress?', function (ok) {

                    if (ok) {
                        // Create null user
                        nx.desktop.resetUser();
                        //
                        nx.util.notifyInfo('Windows closed, See you soon...');
                    }

                });

            } else {

                // Create null user
                nx.desktop.resetUser();
                //
                nx.util.notifyInfo('See you soon...');

            }

        }

    }

});