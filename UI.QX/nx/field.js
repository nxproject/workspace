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

qx.Class.define('nx.field', {

    extend: qx.ui.container.Composite,

    members: {

        // ---------------------------------------------------------
        //
        // LABELS
        // 
        // ---------------------------------------------------------

        getLabelWidget: function () {

            var self = this;

            return nx.bucket.getLabel(self);
        },

        /**
         * 
         * Gets the label value
         * 
         */
        getLabel: function () {

            var self = this;

            // Holding area
            var ans = null;

            // Get the label widget
            var label = self.getLabelWidget();
            // Any?
            if (label) {
                // Get text
                ans = label.getValue();
            }

            return ans;
        },

        /**
         * 
         * Sets the label value
         * 
         * @param {string} text
         */
        setLabel: function (text) {

            var self = this;

            // Get the label widget
            var label = self.getLabelWidget();
            // Any?
            if (label) {
                // Set text
                label.setValue(text);
            }
        },

        // ---------------------------------------------------------
        //
        // TAGS
        // 
        // ---------------------------------------------------------

        setTag: function (color) {

            var self = this;

            // Get the label widget
            var label = nx.bucket.getLabel(self);
            // Any?
            if (label && color) {
                // Set color
                label.setBackgroundColor(color);
            }
        },

        setTagNormal: function () {
            this.setTag(nx.setup.tagNormal);
        },

        setTagOK: function () {
            this.setTag(nx.setup.tagOK);
        },

        setTagWarning: function () {
            this.setTag(nx.setup.tagWarning);
        },

        setTagError: function () {
            this.setTag(nx.setup.tagError);
        },

        setTagTools: function () {
            this.setTag(nx.setup.tagTools);
        }
    }

});