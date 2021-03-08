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

qx.Class.define('o.dataset', {

    extend: o.object,

    construct: function () {

        // Remove the req
        var id = Array.prototype.shift.call(arguments);

        // Call base
        this.base(arguments);

        // Map
        nx.desktop.aomanager.get('_dataset', id, function (data) {
            this.assure(data);
        });

    },

    members: {

        /**
         * The underlying object
         * 
         */
        aoobject: null,

        /**
         *
         * Assures correct format
         *
            */
        assure: function (aoobj) {

            var self = this;

            //
            self.aoobject = aoobj;

            // 
            self.aoobject.assureField('name', '');
            self.aoobject.assureField('fields', {});
            self.aoobject.assureField('views', {});

        },

        save: function () {

            var self = this;

            // Any?
            if (self.aoobject) {
                // Save
                nx.desktop.aomanager.set(self.aoobject);
            }
        },

        /**
         * 
         * Returns the displayable name
         * 
         */
        getName: function () {

            var self = this;

            var ans = null;

            // Any?
            if (self.aoobject) {
                // Get
                ans = self.aoobject.getField('name');
            }

            return ans;

        },

        // ---------------------------------------------------------
        //
        // FIELDS
        // 
        // ---------------------------------------------------------

        getFields: function () {

            var self = this;

            var ans = {};

            // Any?
            if (self.aoobject) {
                // Get
                ans = Object.keys(self.aoobject.getField('fields'));
            }

            return ans;

        },

        getField: function (field) {

            var self = this;

            return new o.datasetField(field, self.getFields()[field]);

        },

        // ---------------------------------------------------------
        //
        // VIEWS
        // 
        // ---------------------------------------------------------

        getViews: function () {

            var self = this;

            var ans = [];

            // Any?
            if (self.aoobject) {
                // Get
                ans = Object.keys(self.aoobject.getField('views'));
            }

            return ans;

        },

        getView: function (view) {

            var self = this;

            return new o.datasetView(view, self.getViews()[view]);

        },

        generateView: function (view, req) {

            var self = this;

            // Start with empty
            var ans = { };

            // Any?
            if (self.aoobject) {

                // Unique?
                if (self.aoobject.isUnique === 'y') {
                    ans.nxid = self.getName();
                }

                // Caption
                ans.caption = self.aoobject.caption;

                // Top toolbar
                if (req.topToolbar) {
                    ans.topToolbar = req.topToolbar;
                }

                // View
                var items = [];
                // Get the view
                var viewDef = self.getView(view);
                // And the list of fields
                var fieldDef = viewDef.fields || {};
                // Loop thru
                Object.keys(fieldDef).forEach(function (entry) {
                    items.push(entry);
                });
                // Save
                ans.items = items;

                // Bottom toolbar
                if (req.bottomToolbar) {
                    ans.bottomToolbar = req.bottomToolbar;
                }

                // Commands
                var list = [];
                // Add cancel
                list.push({
                    label: 'Close',
                    icon: 'cancel',
                    click: function (e) {

                        var self = this;

                        // Map window
                        var win = nx.bucket.getWin(self);

                        // Close
                        win.safeClose();

                    }

                });
                // Do left
                if (req.leftCommands) {
                // Assure array
                    if (!Array.isArray(req.leftCommands)) req.leftCommands = [req.leftCommands];
                    // Add
                    list.concat(req.leftCommands);
                }
                // Split
                list.push('>');
                // Do right
                if (req.leftCommands) {
                    // Assure array
                    if (!Array.isArray(req.rightCommands)) req.rightCommands = [req.rightCommands];
                    // Add
                    list.concat(req.rightCommands);
                }
                // Save
                list.push({

                    label: 'Save',
                    icon: 'database_save',
                    click: function (e) {

                        var self = this;

                        // Map window
                        var win = nx.bucket.getWin(self);

                        // Save
                        win.save();
                    }

                });
                // Set the list
                ans.commands = {
                    items: list
                };
                // Default
                if (self.aoobject.defaultCommand) {
                    ans.defaultCommand = self.aoobject.defaultCommand;
                }

            }

            return ans;
        }
    }

});

qx.Class.define('o.datasetField', {

    extend: o.object,

    construct: function () {

        // Save args
        this.name = Array.prototype.shift.call(arguments);
        this.values = Array.prototype.shift.call(arguments) || {};

        // Call base
        this.base(arguments);

    },

    members: {

        /** 
         *  
         * The field name
         * 
         */
        name: null,

        /**
         * 
         * The underlying values
         * 
         */
        values: null

    }

});

qx.Class.define('o.datasetView', {

    extend: o.object,

    construct: function () {

        // Save args
        this.name = Array.prototype.shift.call(arguments);
        this.values = Array.prototype.shift.call(arguments) || {};

        // Call base
        this.base(arguments);

    },

    members: {

        /** 
         *  
         * The field name
         * 
         */
        name: null,

        /**
         * 
         * The underlying values
         * 
         */
        values: null

    }

});