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

qx.Class.define('c._selectbox', {

    extend: qx.ui.form.SelectBox,

    construct: function () {

        var self = this;

        // Call base
        self.base(arguments);

    },

    members: {

        getValue: function () {

            var self = this;

            // Assume none
            ans = [];
            // Loop thru
            self.getSelection().forEach(function (entry) {
                // Add text
                ans.push(entry.getLabel());
            });
            // By count
            switch (ans.length) {
                case 0:
                    ans = null;
                    break;
                case 1:
                    ans = ans[0];
                    break;
            }

            return ans;

        },

        setValue: function (value) {

            var self = this;

            // Assure array
            if (!Array.isArray(value)) value = [value];
            // Create list
            var list = [];
            self.getChildren().forEach(function (entry) {
                // Get the label
                var label = entry.getLabel();
                // In list?
                if (value.indexOf(label) !== -1) {
                    // Add
                    list.push(entry);
                }
            });
            // Set
            self.setSelection(list);
        }

    }

});