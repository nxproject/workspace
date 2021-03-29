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

qx.Class.define('c._grid', {

    extend: qx.ui.table.Table,

    include: qx.ui.table.MTableContextMenu,

    construct: function () {

        // Save self
        var self = this;

        // Remove the params
        var req = arguments[0];

        // Save
        nx.bucket.setParams(self, req);

        //
        self.setDataRowRenderer(new c._gridrowrenderer(self));

        // Make column model
        var colmodel = new c._gridcolumnmodel();
        self.setNewTableColumnModel(function () {
            return colmodel;
        });

        // Make the manager
        self._manager = new c._gridmanager(self);

        // Do we have a dataset?
        if (req.ds) {

            // Load
            nx.desktop._loadDataset(req.ds, function (dsdef) {

                // Save
                nx.bucket.setDataset(req, dsdef);

                // Do we have a field?
                if (req.aoFld) {

                    // Get the field def
                    var fdef = dsdef.fields[req.aoFld];
                    // Do we have a view?
                    if (fdef && fdef.gridview) {

                        // Load the view
                        self._loadView(req.ds, fdef.gridview, function (viewdef) {

                            // Save
                            nx.bucket.setView(req, viewdef);

                            //
                            self._manager._initRequest(req);

                        });

                    } else {

                        //
                        self._manager._initRequest(req);

                    }

                } else {

                    //
                    self._manager._initRequest(req);

                }
            });
        } else {
            //
            self._manager._initRequest(req);
        }

        // Call base
        self.base(arguments, self._manager);

        // Get the selection model
        var selmodel = self.getSelectionModel();

        // Handle save/restore
        if (req.cookie) {
            // Save
            self.__cookie_name = req.cookie;

            this.addListener('appear', function (e) {
                this.load_column_state();
                var tcm = this.getTableColumnModel();

                tcm.addListener('widthChanged', function (e) {
                    this.save_column_state();
                }, this);
                tcm.addListener('orderChanged', function (e) {
                    this.save_column_state();
                }, this);
                tcm.addListener('visibilityChanged', function (e) {
                    this.save_column_state();
                }, this);
            }, self);
        }

        // According to type
        if (req.isPick) {

            // Many?
            if (req.allowMany) {
                selmodel.setSelectionMode(4);
            }
            // Setup selection
            selmodel.addListener('changeSelection', function (e) {
                // Only if no filtering
                if (!nx.bucket.getInFilter(self)) {
                    // Get the target
                    var selectionModel = e.getTarget();
                    // Make room
                    var selectedRows = [];
                    // Loop thru
                    selectionModel.iterateSelection(function (index) {
                        // Make the row
                        var row = self._manager.getRowData(index);
                        // Add the row index
                        row._row = index;
                        // Add to result
                        selectedRows.push(row);
                    });
                    // Do the event
                    self.fireDataEvent('selected', selectedRows);
                }
            });

            // Filtering?
            if (req.allowFilter) {
                // Loop thru
                for (var i = 0; i < self._manager.getColumnCount(); i++) {
                    colmodel.setHeaderCellRenderer(i, new c._gridheaderrenderer());
                }
            }

        } else {

            // Start with no options
            var options = [];

            // Can we add?
            if (req.allowAdd) {
                options.push({
                    label: 'Add',
                    icon: 'add',
                    click: function (e) {
                        var a = 1;
                        // TBD
                    }
                });
            }
            // Can we delete?
            if (req.allowDelete) {
                options.push({
                    label: 'Delete',
                    icon: 'delete',
                    click: function (e) {
                        var a = 1;
                        // TBD
                    }
                });
            }

            // Any options?
            if (options.length) {

                // Save
                nx.bucket.setOptions(self, options);

                self.addListener('appear', function (e) {

                    // Loop thru
                    for (var i = 0; i < self._manager.getColumnCount(); i++) {
                        self.setContextMenuHandler(i, self.getContextHandler);
                    }

                });

            }

            // Allow darg/drop?
            if (req.allowMove) {

                self.addListener("dragstart", function (e) {

                    var self = this;

                    var focusedRow = self._table.getFocusedRow();
                    self._startRow = { maxIndex: focusedRow, minIndex: focusedRow };
                    e.addAction("move");
                    e.addType("movetransfer");

                }, this);

                self.addListener("droprequest", function (e) {

                    var self = this;

                    var type = e.getCurrentType();
                    var sel = self._table.getSelectionModel().getSelectedRanges();

                    // use the focused row instead of the selection in nothing selected
                    if (sel.length == 0) {
                        sel = [self._startRow];
                    }

                    var selMap = [];

                    for (var i = 0; i < sel.length; i++) {
                        for (var s = sel[i].minIndex; s <= sel[i].maxIndex; s++) {
                            var rowdata = self._table.getTableModel().getRowData(s);
                            if (rowdata == null) {
                                continue;
                            }
                            rowdata.rowId = s;
                            selMap.push(rowdata);
                        }
                    }
                    e.addData(type, selMap);

                }, this);

                self.addListener("drop", function (e) {

                    var self = this;

                    if (e.supportsType("movetransfer")) {
                        var data = e.getData("movetransfer");
                        var dm = self._table.getTableModel();
                        dm.removeRows(data[0].rowId, data.length);
                        dm.addRows(data, self._table.getFocusedRow());
                    }

                }, this);

            }

            // Editable?
            if (req.allowEdit) {

                // Loop thru
                for (var i = 0; i < self._manager.getColumnCount(); i++) {
                    colmodel.setColumnEditable(i, true);
                }

            }

        }

    },

    members: {

        /**
         * Gets the values for the grid
         *
         */
        getValue: function () {

            var self = this;

            var ans = [];

            // Get the model
            var tm = self.getTableModel();
            // Get the row count
            var rows = tm.getRowCount();
            // Loop thru
            for (var i = 0; i < rows; i++) {
                // Add row
                ans.push(tm.getRowData(i));
            }

            return ans;

        },

        /**
         * 
         * Sets the values for a grid
         * 
         * @param {array} value
         */
        setValue: function (value) {

            var self = this;

            // Get the manager
            var mgr = self._manager;

            // Save
            var params = nx.bucket.getParams(mgr);
            params.data = value;

            // Reload
            self.refresh();

        },

        /**
         * 
         * Links a toolbar filter
         * 
         * @param {object} tb
         */
        setFilterToolbar: function (tb) {

            var self = this;

            nx.bucket.setFilterToolbar(self, tb);

            // Refresh
            self.refresh();
        },

        /**
         * 
         * Set the filter for a column
         * 
         * @param {number} index
         * @param {string} value
         */
        setFilter: function (index, value) {

            var self = this;

            // Map the column
            var col = nx.bucket.getParams(self).columns[index];
            if (typeof col === 'object') {
                col = col.aoFld;
            } else if (Array.isArray(col)) {
                col = col[0];
            }

            // Get the filters
            var filters = nx.bucket.getFilters(self);
            // Make if needed
            if (!filters) {
                nx.bucket.setFilters(self, filters = {});
            }
            // Do we have a value?
            if (nx.util.hasValue(value)) {
                // Save
                filters[col] = value;
            } else {
                // Remove
                delete filters[col];
            }

            // Refresh
            self.refresh();
        },

        getFilters: function () {

            var self = this;

            var ans = [];

            // Get the params
            var params = nx.bucket.getParams(self);
            if (params) {
                // Get the chain
                var chain = params.chain;
                if (chain) {
                    // Check to see if query
                    if (chain._cooked) {
                        ans.push(chain);
                    } else {
                        // Get the datasetvar ds = params.ds;
                        if (nx.util.hasValue(params.ds)) {
                            // Get the value
                            var value = chain[params.ds];
                            if (value) {
                                ans.push({
                                    field: '_desc',
                                    value: value
                                });
                            }
                        }
                    }
                }
            }


            // Get col filters
            var cols = nx.bucket.getFilters(self);
            if (cols) {
                Object.keys(cols).forEach(function (col) {
                    // Handle description differently
                    if (col === '_desc') {
                        ans.push({
                            field: col,
                            op: 'Any',
                            value: cols[col]
                        });
                    } else {
                        ans.push({
                            field: col,
                            op: 'Eq',
                            value: cols[col]
                        });
                    }
                });
            }

            // Do we have a toolbar?
            var holder = nx.util.processPickToolbar(nx.bucket.getFilterToolbar(self));
            if (holder && holder.length) {
                ans = ans.concat(holder);
            }

            return ans;

        },

        /**
         * 
         * Refreshes the grid
         * 
         */
        refresh: function () {

            var self = this;

            // Get the model
            var tablemodel = self.getTableModel();
            // Refresh
            tablemodel.reloadData();

        },

        /**
         * 
         * Context menu for grid 
         * 
         * @param {number} col
         * @param {number} row
         * @param {object} table
         * @param {object} dataModel
         * @param {object} contextMenu
         */
        getContextHandler: function (col, row, table, dataModel, contextMenu) {

            // Add the options
            var options = nx.bucket.getOptions(table);

            // Loop thru
            options.forEach(function (entry) {
                // Make the button
                var button = new qx.ui.menu.Button(entry.label, nx.util.getIcon(entry.icon));
                // Save the table
                nx.bucket.setGrid(button, table);
                nx.bucket.setRow(button, table);
                nx.bucket.setColumn(button, table);
                nx.bucket.setDataModel(button, table);
                // Setup
                nx.setup.__component(button, entry);
                // Save
                contextMenu.add(button);
            });

            return true;
        },

        // ---------------------------------------------------------
        //
        // From: https://www.smorgasbork.com/2013/09/30/saving-and-restoring-qooxdoo-table-column-sizes-visibility-and-order/
        // 
        // ---------------------------------------------------------

        __loading_column_state: false,

        __cookie_name: 'table_column_state',

        save_column_state: function () {
            if (this.__loading_column_state) {
                return;
            }

            var tcm = this.getTableColumnModel();

            var num_cols = tcm.getOverallColumnCount();

            var hidden_cols = [];

            var state = {};
            state.col_widths = [];
            state.col_visible = [];
            for (var i = 0; i < num_cols; i++) {
                state.col_visible.push(tcm.isColumnVisible(i) ? 1 : 0);
                state.col_widths.push(tcm.getColumnWidth(i));

                if (!tcm.isColumnVisible(i)) {
                    hidden_cols.push(i);
                }

            }
            var vis_cols = tcm.getVisibleColumns();

            state.col_order = [];

            for (var i = 0; i < vis_cols.length; i++) {
                state.col_order.push(vis_cols[i]);
            }

            for (var i = 0; i < hidden_cols.length; i++) {
                state.col_order.push(hidden_cols[i]);
            }

            var str_state = JSON.stringify(state);

            nx.util.cookieSet(this.__cookie_name, str_state);
        },

        load_column_state: function () {
            this.__loading_column_state = true;

            var str_state = nx.util.cookieGet(this.__cookie_name);

            if (str_state === null) {
                this.__loading_column_state = false;
                return;
            }

            var state = JSON.parse(str_state);

            var tcm = this.getTableColumnModel();
            var num_cols = tcm.getOverallColumnCount();

            // if the application code has changed and the number of columns is different since
            // the time we saved the cookie, just ignore the cookie
            if (state.col_order.length != num_cols) {
                return;
            }

            tcm.setColumnsOrder(state.col_order);

            for (var i = 0; i < num_cols; i++) {
                tcm.setColumnVisible(i, (state.col_visible[i] == 1));
                tcm.setColumnWidth(i, state.col_widths[i]);
            }

            this.__loading_column_state = false;
        },

    },

    events: {
        // Placeholder
        selected: 'qx.event.type.Data'
    }

});