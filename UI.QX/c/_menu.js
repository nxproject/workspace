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

qx.Class.define('c._menu', {

    extend: qx.ui.menu.Menu,

    members: {

        // ---------------------------------------------------------
        //
        // VALUES
        // 
        // ---------------------------------------------------------

        addEntry: function (label, cb) {

            var self = this;

            var btn = new qx.ui.menu.Button(label);
            if (cb) {
                nx.setup.click(btn, {
                    click: cb
                });
            }
            self.add(btn);

        },

        removeEntry: function (label) {

            var self = this;

            // Find
            var entry = self.findEntry(label);
            // Any?
            if (entry) {
                self.remove(entry);
            }

        },

        findEntry: function (label) {

            var self = this;

            var ans;

            self.getChildren().forEach(function (entry) {
                if (entry.getLabel() === label) {
                    ans = entry;
                }
            });

            return ans;
        }

    }

});