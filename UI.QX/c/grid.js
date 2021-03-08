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

qx.Class.define('c.grid', {

    extend: c._component,

    implement: i.iComponent,

    construct: function () {

        // Remove the params
        var params = Array.prototype.shift.call(arguments) || {};

        // Call base
        this.base(arguments, new c._grid(params));

    },

    statics: {

        makeSelf: function (req) {

            return new c.grid(req);

        }
    }

});