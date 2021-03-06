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

qx.Class.define('f.link', {

    extend: qx.core.Object,

    implement: i.iFormatter,

    members: {

        onlyAtBlur: true,

        format: function (widget, value, cb) {

            // Must have a dataset
            var ds = nx.bucket.getParams(widget).linkds;
            if (ds && !nx.util.isUUID(value)) {

                // Get the current chain
                var chain = Object.assign({}, nx.bucket.getChain(nx.bucket.getForm(widget)));
                // Add the value
                if (nx.util.hasValue(value)) chain[ds] = '%' + value;
                // Call view
                nx.util.runTool('View', {
                    caller: nx.bucket.getForm(widget, true),
                    ds: ds,
                    chain: chain,
                    onSelect: function (e, data) {
                        // Any?
                        if (data && data.length) {
                            var data = data[0];
                            data._ds = ds;
                            cb(data);
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