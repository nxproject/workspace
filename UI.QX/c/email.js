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

qx.Class.define('c.email', {

    extend: c._component,

    implement: i.iComponent,

    construct: function () {

        // Call base
        if (nx.desktop.user.getIsSelector('EMAIL')) {
            this.base(arguments, new f.email(), new t.email(), new t.emailweb(), new c._textfield());
        } else {
            this.base(arguments, new f.email(), new t.emailweb(), new c._textfield());
        }

    },

    statics: {

        makeSelf: function (req) {

            return new c.email(req);

        }
    }

});