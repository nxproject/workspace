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

qx.Class.define('c.signature', {

    extend: c._component,

    implement: i.iComponent,

    construct: function () {

        var self = this;

        // Make canvas
        self._canvas = new c._signature();

        // Call base
        this.base(arguments, self._canvas);

    },

    statics: {

        makeSelf: function (req) {

            if (!req.height) {
                req.height = nx.setup._defaults.signatureHeight;
            }

            return new c.signature(req);

        }
    }

});