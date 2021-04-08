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

qx.Class.define('t.link', {

    extend: qx.core.Object,

    implement: i.iTool,

    members: {

        caption: 'View',

        icon: 'cursor',

        allowed: function (widget, cb) {
            cb(true);
        },

        setup: function (widget, button) {

            var self = this;

            // Get the params
            var params = nx.bucket.getParams(widget);
            // And the dataset
            var ds = params.linkds;
            if (ds) {
                // Get the definition
                if (nx.util.hasValue(ds.linkds)) {
                    nx.desktop._loadDataset(ds.linkds, function (dsdef) {
                        // Set the icon
                        button.setIcon(nx.util.getIcon(dsdef.icon));
                    });
                }
            }
        },

        click: function (widget) {

            var self = this;

            // Get value
            var value = widget.getValue();
            // Any?
            if (nx.util.isUUID(value)) {
                // Parse
                var dsid = nx.util.uuidToObject(value);
                // Get the window
                var win = nx.bucket.getForm(widget);
                // View
                nx.desktop.addWindowDS({
                    ds: dsid.ds,
                    id: dsid.id,
                    view: nx.desktop.user.getDSInfo(dsid.ds).view,
                    caller: win,
                    chain: nx.bucket.getChain(win)
                });
            } else {
                // Get the container
                var ctx = nx.bucket.getContainer(widget);
                // And the renderer
                var render = ctx.$$parent;
                // Call the formatter
                nx.util.processFormatters(widget, '', 0);
            }

        }
    }

});