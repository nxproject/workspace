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

	@require(qx.core.Object)

************************************************************************ */

qx.Class.define('c._component', {

    extend: qx.core.Object,

    construct: function () {

        var self = this;

        // Call base
        self.base(arguments);

        // Init local stores
        nx.bucket.setWidgets(self, []);
        nx.bucket.setTools(self, []);
        nx.bucket.setFormatters(self, []);

        self.include(arguments);

    },

    members: {

        include: function () {

            var self = this;

            // Loop thru
            for (var i = 0, j = arguments.length; i < j; i++) {

                // Get
                var wkg = arguments[i];

                // Is it an array?
                if (wkg.callee) {
                    // Loop thru
                    for (var k = 0; k < wkg.length; k++) {
                        // Process wrapper
                        self.includeWrapper(wkg[k]);
                    }
                } else {
                    // Process wrapper
                    self.includeWrapper(wkg);
                }
            }

        },

        includeWrapper: function (wrapper) {

            var self = this;

            // Class?
            if (wrapper.classname) {

                // Get type
                var type = wrapper.classname.substr(0, wrapper.classname.indexOf('.'));
                // Accordingly
                switch (type) {
                    case 'qx':
                    case 'qxl':
                    case 'c':
                        // Add
                        nx.bucket.getWidgets(self).push(wrapper);
                        break;
                    case 'f':
                        // Add
                        nx.bucket.getFormatters(self).push(wrapper);
                        break;
                    case 't':
                        // Add
                        nx.bucket.getTools(self).push(wrapper);
                        break;
                }

            }

        },


        // ---------------------------------------------------------
        //
        // VALUES
        // 
        // ---------------------------------------------------------

        /**
         * 
         * Gets the value(s) of the component
         * 
         * */
        getValue: function () {

            var self = this;

            // Holding area
            var ans = [];
            // Loop thru
            nx.bucket.getWidgets(self).forEach(function (widget) {

                // Add
                ans.push(widget.getValue());

            });

            // Adjust if not many
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

        /**
         * 
         * Sets the value(s) of the component
         * 
         * @param {any} value
         */
        setValue: function (value) {

            var self = this;

            // Assure array
            if (!Array.isArray(value)) value = [value];
            // Loop thru
            nx.bucket.getWidgets(self).forEach(function (widget, index) {

                // Assume none
                var ivalue = null;
                // Valid index?
                if (index < value.length) {
                    // Get
                    ivalue = value[index];
                }
                // Set
                widget.setValue(ivalue);

            });

        },

        /**
         * 
         * Resets value
         * 
         */
        resetValue: function () {
            // Not allowed
        },

    },

    events: {
        // Placeholder
        changeValue: 'qx.event.type.Data'
    },

    statics: {

        preSelf: function (req, cb) {

            cb(req);

        }
    }

});