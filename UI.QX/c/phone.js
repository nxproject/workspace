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

qx.Class.define('c.phone', {

    extend: c._component,

    implement: i.iComponent,

    construct: function () {

        // Call base
        this.base(arguments, new f.phone(), new t.quickmessage(),
            new t.phonesms(), new t.smsweb(), new t.phonecall(), new t.phoneweb(),
            new c._textfield());

    },

    statics: {

        makeSelf: function (req) {

            return new c.phone(req);

        }
    }

});