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

            nx.desktop.addWindowDS({
                ds: '_user',
                id: nx.desktop.user.getName(),
                view: '_usersettings',
                caption: 'Personal Settings',
                atSave: function (data) {
                    //
                    nx.desktop.user.setValues(data);
                }

            });

        }

    }

});