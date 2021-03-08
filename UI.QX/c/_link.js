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

qx.Class.define('c._link', {

    extend: qx.ui.form.TextField,

    construct: function () {

        var self = this;

        // Call base
        self.base(arguments);

    },

    members: {

        // ---------------------------------------------------------
        //
        // VALUES
        // 
        // ---------------------------------------------------------

        _uuid: null,

        /**
         * 
         * Gets the value(s) of the component
         * 
         * */
        getValue: function (usebase) {

            var self = this;

            var ans = self._uuid;
            if (usebase) ans = this.base(arguments);

            // 
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
            var args = arguments;

            // Clear
            self._uuid = null;
            // Call base
            self.base(args, null);

            // Anything?
            if (value) {
                // Object?
                if (typeof value === 'object') {
                    // Make the uuid
                    self._uuid = nx.util.objectToUUID(value);
                    // Show
                    var value = nx.util.localizeDesc(value);
                    // Call base
                    self.base(args, value);
                } else if (nx.util.isUUID(value)) {
                    // Save
                    self._uuid = value;
                        // Parse
                    var info = nx.util.uuidToObject(value);
                    // Fetch
                    nx.desktop.aomanager.get(info.ds, info.id, function (data) {
                        // Show
                        var value = nx.util.localizeDesc(data.values);
                        // Call base
                        self.base(args, value);
                    });
                }
            }
        }

    }

});