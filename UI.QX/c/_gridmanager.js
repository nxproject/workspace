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

qx.Class.define('c._gridmanager', {

    extend: qx.ui.table.model.Remote,

    construct: function () {

        // Save self
        var self = this;

        // Remove the params
        var table = arguments[0];

        // Call base
        self.base(arguments);

        // Link
        nx.bucket.setTable(self, table);
    },

    members:
    {
		/**
		 * 
		 * Initializes the manager
		 * 
		 */
        _initRequest: function (req) {

            var self = this;

            // Get
            var params = self.getParams(req);

            // Make room
            var coltitles = [];
            var colfields = [];

            // Get the definitions
            var dsdef = nx.bucket.getDataset(params);
            var viewdef = nx.bucket.getView(params);
            var table = nx.bucket.getTable(self);

            // Get the columns
            var cols = params.columns;
            // Handle array
            if (cols && Array.isArray(cols)) {
                // Loop thru
                cols.forEach(function (col, index) {
                    // Simple case
                    if (typeof col === 'string') {
                        coltitles.push(col);
                        colfields.push(col);
                    } else {
                        // Fill in
                        self.defineColumn(col, index, coltitles, colfields, dsdef, viewdef, req, table);
                    }
                });
            } else {
                if (!cols && viewdef) {
                    // From view
                    cols = viewdef.fields;
                }
                // Assure
                cols = cols || {};

                // Loop thru
                Object.keys(cols).forEach(function (field, index) {
                    // Get the definition
                    var def = cols[field];
                    // Fill in
                    self.defineColumn(def, index, coltitles, colfields, dsdef, viewdef, req, table);
                });
            }

            // Set
            self.setColumns(coltitles, colfields);
            // And save the field list
            params.fields = colfields;

            //
            //table.setDataRowRenderer(new c._gridrowrenderer(table));

        },

        defineColumn: function (def, index, coltitles, colfields, dsdef, viewdef, req, table) {

            var self = this;

            // 
            colfields.push(def.aoFld);
            coltitles.push(def.label || def.aoFld);

            // Do we have a dataset?
            if (dsdef && (!def.nxtype || def.nxtype === 'usedataset')) {
                // Get the field type
                var info = dsdef.fields[def.aoFld];
                // Any?
                if (info) {
                    def.nxtype = info.nxtype;
                }
            }

            // Assure
            def.nxtype = def.nxtype || 'string';

            // Pick grid?
            if (req.isPick) {
                // Nothing
            } else if (def.editable === 'y') {
                // Editable
                self.setColumnEditable(index, true);
            }

            // Sortable?
            if (def.sortable === 'y') {
                self.setColumnSortable(index, true);
            }
        },

        // ---------------------------------------------------------
        //
        // ACCESS
        // 
        // ---------------------------------------------------------

        getParams: function (req) {

            var self = this;

            // 
            if (req) {
                nx.bucket.setParams(self, req);
            }

            return nx.bucket.getParams(self);

        },

        // ---------------------------------------------------------
        //
        // REMOTE SUPPORT
        // 
        // ---------------------------------------------------------

        // overridden - called whenever the table requests the row count
        _loadRowCount: function () {

            // Save self
            var self = this;

            // Map
            var source = self.getParams();
            var table = nx.bucket.getTable(self);
            if (table.getDataRowRenderer().classname !== 'c._gridrowrenderer') {
                table.setDataRowRenderer(new c._gridrowrenderer(table));
            }

            // Local?
            if (source.data) {

                // Apply it to the model - the method "_onRowCountLoaded" has to be called
                self._onRowCountLoaded(source.data.length);

            } else {

                // Get the filters
                var filters = nx.bucket.getTable(self).getFilters();
                // Get the dataset definition
                nx.desktop._loadDataset(source.ds, function (dsdef) {

                    // Per user?
                    if (nx.util.hasValue(dsdef.privField) && dsdef.privAllow === 'y' && !nx.desktop.user.getIsSelector('MGR')) {
                        // Assure
                        filters = filters || [];
                        filters.push({
                            field: dsdef.privField,
                            op: 'Eq',
                            value: nx.desktop.user.getName()
                        });
                    }
                    // Call
                    nx.util.serviceCall('AO.QueryCount', {
                        ds: source.ds,
                        query: filters,
                        idprefix: source.idprefix
                    }, function (result) {
                        // Validate
                        if (result) {
                            // Apply it to the model - the method "_onRowCountLoaded" has to be called
                            self._onRowCountLoaded(result.count);
                        }
                    });
                });

            }

        },

        // overridden - called whenever the table requests new data
        _loadRowData: function (firstRow, lastRow) {

            // Save self
            var self = this;

            // Map
            var source = self.getParams();

            // Local?
            if (source.data) {

                var result = [];
                var data = source.data;

                // Get the sort column
                var sorton;
                if (self.getSortColumnIndex() > -1) {
                    sorton = source.fields[self.getSortColumnIndex()];
                }

                // Loop thru
                for (var i = firstRow; i <= lastRow; i++) {
                    // Get
                    result.push(data[i]);
                }

                // Sort?
                if (sorton) {
                    var sortdesc = !self.isSortAscending();
                    result.sort(function (a, b) {
                        if (sortdesc) {
                            return b[sorton].localeCompare(a[sorton]);
                        } else {
                            return a[sorton].localeCompare(b[sorton]);
                        }
                    });
                }

                // Apply it to the model - the method "_onRowDataLoaded" has to be called
                self._onRowDataLoaded(result);

            } else {

                // Get the sort colum
                var sortindex = self.getSortColumnIndex();
                if (sortindex < 0) sortindex = 0;
                var sortcol = '';
                if (sortindex >= 0) sortcol = source.fields[sortindex];

                // Get the filters
                var filters = nx.bucket.getTable(self).getFilters();
                // Get the dataset definition
                nx.desktop._loadDataset(source.ds, function (dsdef) {

                    // Per user?
                    if (nx.util.hasValue(dsdef.privField) && dsdef.privAllow === 'y' && !nx.desktop.user.getIsSelector('MGR')) {
                        // Assure
                        filters = filters || [];
                        filters.push({
                            field: dsdef.privField,
                            op: 'Eq',
                            value: nx.desktop.user.getName()
                        });
                    }

                    // Call
                    nx.util.serviceCall('AO.QueryGet', {
                        ds: source.ds,
                        query: filters,
                        firstRow: firstRow,
                        lastRow: lastRow,
                        sortCol: sortcol,
                        sortIndex: sortindex,
                        sortOrder: dsdef.sortOrder || (self.isSortAscending() ? 'asc' : 'desc'),
                        idprefix: source.idprefix
                    }, function (result) {
                        // Validate
                        if (result) {
                            // Modify 
                            result.data.forEach(function (row) {
                                // Does it have a description?
                                if (row._desc) {
                                    row._desc = nx.util.localizeDesc(row._desc);
                                }
                            });
                            // Apply it to the model - the method "_onRowDataLoaded" has to be called
                            self._onRowDataLoaded(result.data);
                        }
                    });

                });

            }

        }
    }
});