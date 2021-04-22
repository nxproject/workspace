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

qx.Class.define('tools.UserSettings', {

    type: 'static',

    statics: {

        startindex: 'hidden',

        // This is what you override
        do: function (req) {

            var isacct = nx.desktop.user.getIsSelector('ACCT');
            var user = nx.desktop.user.getName();

            nx.desktop.addWindowDS({
                ds: (isacct ? '_billaccess' : '_user'),
                id: user,
                view: '_usersettings',
                caption: 'Personal Settings',
                atSave: function (data) {

                    // Acct?
                    if (isacct) {
                        // Update
                        nx.util.serviceCall('AO.AccessSet', {
                            ds: '_billaccess',
                            id: user,
                            data: data
                        }, function () {
                            nx.util.notifyOK('Password reset');
                        });
                    } else {
                        //
                        nx.desktop.user.setValues(data);
                    }
                }

            });

        }

    }

});