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

qx.Class.define('c._image', {

    extend: qx.ui.basic.Image,

    members: {

        // ---------------------------------------------------------
        //
        // VALUES
        // 
        // ---------------------------------------------------------

        _value: null,

        /**
         * 
         * Gets the value(s) of the component
         * 
         * */
        getValue: function () {

            var self = this;

            // Call redirect
            return self.getSource();

        },

        /**
         * 
         * Sets the value(s) of the component
         * 
         * @param {any} value
         */
        setValue: function (value) {

            var self = this;

            // Call redirect
            self.setSource(value);

        }

    }

});