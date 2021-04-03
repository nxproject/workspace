/* ************************************************************************

   Framework7 - a dynamic web interface

   https://framework7.io/

   Copyright:
     2020-2021 Jose E. Gonzalez (nxoffice2021@gmail.com)

   License:
     MIT: https://opensource.org/licenses/MIT
     See the LICENSE file in the project's top-level directory for details.

   Authors:
     * Jose E. Gonzalez

************************************************************************ */

nx.aoobject = {

    /**
     * 
     * Initializes object
     *
     */
    prep: function (values) {

        var self = this;

        //
        var aoobject = {
            ds: null,
            id: null,
            values: null
        };

        // Save
        aoobject.values = values || {};

        // Assure changes
        aoobject.values._changes = aoobject.values._changes || [];
        if (typeof (aoobject.values._changes) === 'string') {
            aoobject.values._changes = JSON.parse(aoobject.values._changes);
        }

        return aoobject;
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
    getFields: function (aoobject) {

        var self = this;

        // Assume none
        var ans = [];
        // Do we have data?
        if (aoobject.values) {
            // Make list
            ans = Object.keys(aoobject.values);
        }

        return ans;
    },

    /**
     * 
     * Gets a field value
     * 
     * @param {obj} aoobject
     * @param {string} field
     */
    getField: function (aoobject, field) {

        var self = this;

        // Assume none
        var ans = null;

        // Valid record?
        if (aoobject.values) {

            // Get
            ans = aoobject.values[field];

        }

        return ans;
    },

    /**
     * 
     * 
     * Sets a field value
     * 
     * @param {obj} aoobject
     * @param {string} field
     * @param {any} value
     */
    setField(aoobject, field, value) {

        var self = this;

        // Valid record?
        if (aoobject.values) {

            // Changed?
            if (aoobject.values[field] !== value) {

                // Set
                aoobject.values[field] = value;
                // Add to changes
                if (aoobject.values._changes.indexOf(field) === -1) aoobject.values._changes.push(field);

            }
        }

    },

    /**
     * 
     * Assures the value in a field
     * 
     * @param {obj} aoobject
     * @param {string} field
     * @param {any} value
     */
    assureField: function (aoobject, field, value) {

        var self = this;

        self.setField(aoobject, field, self.getField(aoobject, field) || value);
    },

    /**
     * 
     * Forces a value into the field
     * 
     * @param {obj} aoobject
     * @param {string} field
     * @param {any} value
     */
    forceField: function (aoobject, field, value) {

        var self = this;

        // Save
        self.setField(aoobject, field, value);
        // Mark
        self.markField(aoobject, field);

    },

    /**
     * 
     * Marks a field as changed
     * 
     * @param {obj} aoobject
     * @param {string} field
     */
    markField: function (aoobject, field) {

        var self = this;

        // Reset
        if (aoobject.values) {
            aoobject.values._changes.push(field);
        }

    },

    /**
     * 
     * Clears marked fields
     * 
     * @param {obj} aoobject
     */
    clearMarked: function (aoobject) {

        var self = this;

        // Reset
        if (aoobject.values) {
            aoobject.values._changes = [];
        }

    },

    /**
     * 
     * Returns true if field has changed
     * 
     * @param {any} aoobject
     * @param {any} field
     */
    hasChanged: function (aoobject, field, value) {

        var self = this;

        var ans = false;

        // Check
        if (aoobject.values) {
            // Check to see if it is changed
            ans = aoobject.values._changes.indexOf(field) !== -1;
        }

        // If not marked, check value
        if (!ans) {
            // Same?
            ans = !nx.util.isSameValue(value, self.values[field]);
        }

        return ans;
    }

};