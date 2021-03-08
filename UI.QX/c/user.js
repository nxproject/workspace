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

************************************************************************ */

qx.Class.define('c.user', {

    extend: c.combobox,

    statics: {

        makeSelf: function (req) {

            // New list
            var users = [];
            // Copy list
            for (i = 0; i < nx.desktop.user.SIOUsers.length; i++) {
                var user = nx.desktop.user.SIOUsers[i];
                if (users.indexOf(user) === -1) users.push(user);
            }
            // And add ourselves
            var user = nx.desktop.user.getField('name');
            if (users.indexOf(user) === -1) users.push(user);
            // Sort
            users.sort();
            // And add
            req.choices = users;

            return new c.user(req);

        }
    }

});