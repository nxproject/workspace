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

qx.Class.define('c.tabs', {

    extend: c._component,

    implement: i.iComponent,

    construct: function (req, orig, win) {

        var self = this;

        //
        var tree = new c._tabs();

        // Call base
        self.base(arguments, tree);

    },

    statics: {

        makeSelf: function (req, orig, win) {

            return new c.tabs(req, orig, win);

        }
    }

});