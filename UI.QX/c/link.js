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

qx.Class.define('c.link', {

    extend: c._component,

    implement: i.iComponent,

    construct: function (req) {

        var self = this;

        // Call base
        self.base(arguments, new f.link(), new c._link(), new t.link());

    },

    statics: {

        makeSelf: function (req) {

            return new c.link(req);

        }
    }

});