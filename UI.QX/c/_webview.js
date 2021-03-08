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

qx.Class.define('c._webview', {

    extend: qx.ui.embed.Iframe,

    construct: function () {

        // Save self
        var self = this;

        // Remove the params
        var req = arguments[0]; // Array.prototype.shift.call(arguments) || {};

        // Save
        nx.bucket.setParams(self, req);

        // Call base
        self.base(arguments);

    },

    members: {

        /**
         * Gets the values for the webview
         *
         */
        getValue: function () {

            var self = this       ;

            return self.getSource();

        },

        /**
         * 
         * Sets the values for the webview
         * 
         * @param {array} value
         */
        setValue: function (value) {

            var self = this;

            self.setSource(value);

        }

    }

});