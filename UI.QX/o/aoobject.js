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

qx.Class.define('o.aoobject', {

    extend: o.object,

    construct: function () {

        // Call base
        this.base(arguments);

    },

    members: {

        /**
         * 
         * The dataset that the object lives in
         * 
         */
        ds: null,

        /**
         * 
         * The ID of the object
         * 
         */
        id: null,

        /**
         * 
         * The values 
         * 
         */
        values: null,

        /**
         * 
         * Initializes object
         *
         */
        prep: function (values) {

            var self = this;

            // Save
            self.values = values;

            // Assure changes
            self.values._changes = self.values._changes || [];
            if (typeof (self.values._changes) === 'string') {
                self.values._changes = JSON.parse(self.values._changes);
            }
        },

        /**
         * 
         * Sets the changed field into the database
         * 
         * */
        set: function (cb) {

            var self = this;

            // Via desktop
            nx.desktop.aomanager.set(self, cb);

        },

        // ---------------------------------------------------------
        //
        // FIELDS
        // 
        // ---------------------------------------------------------

        /**
         * 
         * Returns a list of field names
         * 
         */
        getFields: function () {

            var self = this;

            // Assume none
            var ans = [];
            // Do we have data?
            if (self.values) {
                // Make list
                ans = Object.keys(self.values);
            }

            return ans;
        },

        /**
         * 
         * Gets a field value
         * 
         * @param {string} field
         */
        getField: function (field) {

            var self = this;

            // Assume none
            var ans = null;

            // Valid record?
            if (self.values) {

                // Get
                ans = self.values[field];

            }

            return ans;
        },

        /**
         * 
         * 
         * Sets a field value
         * 
         * @param {string} field
         * @param {any} value
         */
        setField(field, value) {

            var self = this;

            // Valid record?
            if (self.values) {

                // Changed?
                if (self.values[field] !== value) {

                    // Set
                    self.values[field] = value;
                    // Add to changes
                    if (self.values._changes.indexOf(field) === -1) self.values._changes.push(field);

                }
            }

        },

        /**
         * 
         * Assures the value in a field
         * 
         * @param {string} field
         * @param {any} value
         */
        assureField: function (field, value) {

            var self = this;

            self.setField(field, self.getField(field) || value);
        },

        /**
         * 
         * Forces a value into the field
         * 
         * @param {string} field
         * @param {any} value
         */
        forceField: function (field, value) {

            var self = this;

            // Save
            self.setField(field, value);
            // Mark
            self.markField(field);

        },

        /**
         * 
         * Marks a field as changed
         * 
         * @param {string} field
         */
        markField: function (field) {

            var self = this;

            // Reset
            if (self.values) {
                self.values._changes.push(field);
            }

        },

        /**
         * 
         * Clears marked fields
         * 
         */
        clearMarked: function () {

            var self = this;

            // Reset
            if (self.values) {
                self.values._changes = [];
            }

        },

        /**
         * 
         * Returns true if field has changed
         * 
         * @param {any} field
         */
        hasChanged: function (field, value) {

            var self = this;

            var ans = false;

            // Check
            if (self.values) {
                // Check to see if it is changed
                ans = self.values._changes.indexOf(field) !== -1;
            }

            // If not marked, check value
            if (!ans) {
                // Same?
                ans = !nx.util.isSameValue(value, self.values[field]);
            }

            return ans;
        }
    }

});
