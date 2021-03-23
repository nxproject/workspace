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

qx.Class.define('f.lu', {

    extend: qx.core.Object,

    implement: i.iFormatter,

    members: {

        onlyAtBlur: true,

        format: function (widget, value, cb) {

            // Must have a dataset
            var ds = nx.bucket.getParams(widget).linkds;
            if (ds && nx.util.hasValue(value)) {

                // Phony a chain
                var chain = {};
                // Add the value
                chain[ds] = '%' + value;
                // Call view
                nx.util.runTool('View', {
                    caller: nx.bucket.getForm(widget, true),
                    ds: ds,
                    chain: chain,
                    onSelect: function (e, data) {
                        // Any?
                        if (data && data.length) {
                            var data = data[0];
                            // Map back
                            var params = nx.bucket.getParams(widget);
                            var win = nx.bucket.getWin(widget);
                            var form = nx.bucket.getForm(widget);
                            var dsdef = nx.bucket.getDataset(win);
                            // Get the field
                            var fld = params.aoFld;
                            // Get the field definition
                            var fdef = dsdef.fields[fld];
                            // Any?
                            if (fdef && fdef.lumap) {
                                // Get the map
                                var map = nx.util.splitSpace(fdef.lumap);
                                // Loop up the value
                                var value = data[map[0]];
                                //
                                var newvalue = {};
                                // Loop thtu rest
                                for (var i = 1; i < map.length; i += 2) {
                                    // Get the field name
                                    var fname = map[i];
                                    // And the value
                                    var fvalue = data[map[i + 1]];
                                    // Save
                                    newvalue[fname] = fvalue;
                                }
                                // Set
                                form.setFormData(newvalue);
                            }
                            cb(value);
                        } else {
                            cb(null);
                        }
                    }
                });

            } else {
                cb(value);
            }

        }
    }

});