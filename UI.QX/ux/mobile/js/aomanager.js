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


nx.aomanager = {

	/**
	 *   
	 * Gets an AO Object from the database
	 * 
	 * @param {string} ds
	 * @param {string} id
	 * @param {function} cb
	 */
    get: function (ds, id, cb) {

        var self = this;

        // Result
        var ans = null;

        // Assume normal
        var data = null;
        // Is it?
        if (typeof ds === 'object') {
            // Nope
            data = ds;
            // From data
            ds = data._ds;
            id = data._id;
        }

        // Make into UUID
        var uuid = nx.util.uuidToObject(ds, id);

        // Valid?
        if (uuid.ds && uuid.id) {

            // Did we get the data?
            if (data) {

                // Assure changes
                data._changes = data._changes || [];
                if (typeof (data._changes) === 'string') {
                    data._changes = JSON.parse(data._changes);
                }

                // Make the object
                ans = nx.aoobject.prep(data);

                if (cb) cb(ans);

            } else {

                // Read
                nx.util.serviceCall('AO.ObjectGet', {
                    ds: uuid.ds,
                    id: uuid.id
                }, function (result) {

                    // Do we have an id?
                    if (result._id) {
                        // Make the object
                        ans = nx.aoobject.prep(data);
                    }

                    if (cb) cb(ans);

                });

            }
        } else {
            if (cb) cb(ans);
        }
    },

	/**
	 * 
	 * Creates an AO Object from the database
	 * 
	 * @param {string} ds
	 * @param {object} data
	 */
    create: function (ds, id, cb) {

        var self = this;

        // Result
        var ans = null;

        // Read
        nx.util.serviceCall('AO.ObjectCreate', {
            ds: ds
        }, function (result) {

            // Do we have an id?
            if (result._id) {

                // Make into UUID
                var uuid = nx.util.uuidToObject(ds, result._id);

                // Make the object
                ans = nx.aoobject.prep(data);

                // Save the location
                nx.util.getLocation(function (loc) {
                    nx.db.obj.setField(ans, loc.latitude + ',' + loc.longitude);

                    if (cb) cb(ans);
                });
            } else {
                if (cb) cb(ans);
            }

        });

        return ans;

    },

	/**
	 * 
	 * Saves an AO Object into the database
	 * 
	 * @param {o.aoobject} obj
	 */
    set: function (obj) {

        //
        var self = this;

        // Check
        if (obj && obj.values) {

            // Get the values
            var values = obj.values;

            // Valid?
            if (values) {

                // Build the change object
                var changed = {};
                var any = false;
                // Loop thru
                values._changes.forEach(function (field) {

                    // Save
                    changed[field] = values[field];
                    any = true;

                });

                // Write out
                if (any) {
                    nx.util.serviceCall('AO.ObjectSet', {
                        ds: values._ds,
                        id: values._id,
                        data: changed
                    }, nx.util.noOp);
                }
            }
        }
    }
};