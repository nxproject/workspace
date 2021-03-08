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

	@require(qx.ui.form.Button)

************************************************************************ */

qx.Class.define('c.button', {

    extend: c._component,

    construct: function () {

        // Call base
        this.base(arguments, new qx.ui.form.Button());

    },

    statics: {

        makeSelf: function (req) {

            return new c.button(req);

        }
    }

});