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

nx.cm = {

    /**
      * 
      * Gets the widget associated with the content menu button
      * 
      * @param {any} ele
      */
    get: function (ele) {

        var self = this;

        return $(ele).parent().parent().prev().children().children().children('input');
    },

    /**
     * 
     * Maps the relative fields
     * 
     * @param {any} widget
     * @param {any} relname
     * @param {any} cb
     */
    map: function (widget, relname, cb) {

        var self = this;

        // Get the widgets name
        var wname = widget.attr('name');

        // Make an empty map
        var map = {
            _fields: {}
        };

        // Map self
        map[relname] = widget.val();
        map._fields[relname] = wname;

        // Get the window
        var data = nx.env.getBucketItem('_obj');

        // Make a list
        var tbd = [wname];
        var done = [];

        // Get dataset
        nx.db._loadDataset(data._ds, function (dsdef) {

            //
            var wname = tbd.pop();

            // Loop until no more
            while (wname) {

                // Add to done
                done.push(wname);

                // Get the field
                var fdef = dsdef.fields[wname];
                if (fdef) {

                    // Loop thru
                    Object.keys(fdef).forEach(function (fld) {

                        // A relative?
                        if (nx.util.startsWith(fld, 'rel')) {
                            // Get the field
                            var aofld = fdef[fld];
                            // remove brackets if any
                            if (nx.util.startsWith(aofld, '[') && nx.util.endsWith(aofld, ']')) {
                                aofld = aofld.substr(1, aofld.length - 2);
                            }
                            // Map
                            map._fields[fld] = aofld;
                            // Map value
                            map[fld] = data[aofld];

                            // Add if new
                            if (done.indexOf(aofld) === -1) {
                                tbd.push(aofld);
                            }
                        }

                    });

                }

                // Next
                wname = tbd.pop();
            }

            //
            if (cb) cb(map);
        });

    },

    /**
     * 
     * Sets a field using the map
     * 
     * @param {any} map
     * @param {any} fld
     * @param {any} value
     */
    set: function (map, fld, value) {

        var self = this;

        // Get the ao field
        var aofld = map._fields[fld];

        //
        if (aofld) {

            // Page
            $('[name=' + aofld + ']').val(value);
            // Database
            nx.db.objSetField(nx.env.getBucketItem('_obj'), aofld, value);

        }

    }

};