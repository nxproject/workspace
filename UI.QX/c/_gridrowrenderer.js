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

qx.Class.define('c._gridrowrenderer', {

    extend: qx.ui.table.rowrenderer.Default,

    members:
    {
        // overridden
        updateDataRowElement: function (rowInfo, rowElem) {

            this.base(arguments, rowInfo, rowElem);

            var color = null;

            if (rowInfo.rowData) {
                color = rowInfo.rowData._color;
                if (typeof color === 'function') color = color(rowInfo);
            }

            // Do we have a color?
            if (color) {
                var style = rowElem.style;
                style.backgroundColor = color;
            }
        },

        // overridden
        createRowStyle: function (rowInfo) {

            var color = null;
            if (rowInfo.rowData) {
                color = rowInfo.rowData._color;
            }
            if (typeof color === 'function') color = color(rowInfo);
            var colors = this._colors;

            var rowStyle = [];
            rowStyle.push(";");
            rowStyle.push(this._fontStyleString);
            rowStyle.push("background-color:");

            if (color) {
                rowStyle.push(color);
                rowStyle.push(';color:');
                rowStyle.push(colors.colNormal);

                // Fix selection
                colors.bgcolFocusedSelected = color;
                colors.bgcolFocused = color;
                colors.bgcolOdd = color;
                colors.bgcolEven = color;
                colors.colSelected = colors.colNormal;
            } else {
                if (rowInfo.focusedRow && this.getHighlightFocusRow()) {
                    rowStyle.push(rowInfo.selected ? colors.bgcolFocusedSelected : colors.bgcolFocused);
                } else {
                    if (rowInfo.selected) {
                        rowStyle.push(colors.bgcolSelected);
                    }
                    else {
                        if (color) {
                            rowStyle.push(color);
                        } else {
                            rowStyle.push((rowInfo.row % 2) ? colors.bgcolOdd : colors.bgcolEven);
                        }
                    }
                }

                rowStyle.push(';color:');
                rowStyle.push(rowInfo.selected ? colors.colSelected : colors.colNormal);
            }

            rowStyle.push(';border-bottom: 1px solid ', colors.horLine);

            if (rowInfo.rowData) {
                // Split the font style
                var items = this._fontStyleString.split(';');
                // Make room
                var fonts = [];
                // And Size
                var fontsize = '12px';
                // Loop thru
                items.forEach(function (entry) {
                    // Split
                    var pieces = entry.split(':');
                    // Must have two
                    if (pieces.length === 2) {
                        // From first
                        switch (pieces[0]) {
                            case 'font-family':
                                // Split
                                fonts = pieces[1].split(',');
                                break;
                            case 'font-size':
                                fontsize = pieces[1];
                                break;
                        }
                    }
                });
                // Assume given is smallest
                var height = rowInfo.styleHeight;
                // Get field names
                var fields = Object.keys(rowInfo.rowData);
                // Get the model
                var model = rowInfo.table.getNewTableColumnModel()();
                // And the columns
                var cols = model.getVisibleColumns();
                var cdef = nx.bucket.getParams(rowInfo.table).columns;
                // Loop thru
                cols.forEach(function (col) {
                    // Is it aoutosize?
                    if (cdef[col].autoresize) {
                        // Check the renderer
                        if (model.getDataCellRenderer(col).classname !== 'c._gridcellrenderer') {
                            // Change
                            model.setDataCellRenderer(col, new c._gridcellrenderer());
                        }
                        // Get the width
                        var width = model.getColumnWidth(col);
                        // Must have some
                        if (width) {
                            // Get the text
                            var text = rowInfo.rowData[fields[col]];
                            // Loop thru fonts
                            fonts.forEach(function (font) {
                                // Compute width
                                var metrics = nx.util.getTextMetrics(text, fontsize + ' ' + font);
                                // Longer?
                                if (metrics.width > width) {
                                    // Compute rows
                                    var rows = 7 + (rowInfo.styleHeight * (1 + Math.ceil(metrics.width / width)));
                                    // Use largest
                                    if (height < rows) height = rows;
                                }
                            });
                        }
                    }
                });
                rowInfo.styleHeight = height;
            }

            return rowStyle.join("");
        }

    }

});