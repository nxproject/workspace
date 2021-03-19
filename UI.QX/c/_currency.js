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

qx.Class.define('c._currency', {

    extend: c._textfield,

    members: {

        /**
         * Gets the values for the grid
         *
         */
        getValue: function () {

            var self = this;

            // Get the real value
            var ans = self.base(arguments);
            // Anything?
            if (ans) {
                ans = nx.util.numbersOnly(value);
            }

            return ans;

        },

        /**
         *
         * Sets the value(s) of the component
         *
         * @param {any} value
         */
        setValue: function (value) {

            var self = this;

            if (value) {
                value = nx.util.toNumber(value);
                if (isNaN(value)) value = 0;
                value = '$' + value.toFixed(2);
            }
            // Call base
            self.base(args, value);
        }

    }

});