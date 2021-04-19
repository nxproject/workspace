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

qx.Class.define('c._gridheadercell', {

    extend: qx.ui.table.headerrenderer.HeaderCell,

    members:
    {
        // overridden
        _createChildControlImpl: function _createChildControlImpl(id, hash) {

            var self = this;
            var control;

            switch (id) {
                case 'label':
                    var ph = self.getLabel();

                    self._label = new qx.ui.basic.Label(ph).set({
                        anonymous: true,
                        allowShrinkX: true
                    });

                    self._add(self._label, {
                        row: 0,
                        column: 1
                    });

                    self._icon = new qx.ui.basic.Image(nx.util.getIcon('magnifier'));
                    self._add(self._icon, {
                        row: 1,
                        column: 0
                    });

                    self._filter = new c._textfield();
                    self._filter.setPlaceholder(ph);
                    self._add(self._filter, {
                        row: 1,
                        column: 1
                    });

                    self._filter.addListener('keyup', function (e) {
                        switch (e.getKeyIdentifier()) {
                            case 'Tab':
                            case 'Enter':
                                // Hide the filter
                                //self._filter.hide();
                                // 
                                self.lookup();
                                break;
                            case 'Space':
                                nx.util.insertAtCursor(filter, ' ');
                                break;
                        }
                    });

                    self._filter.addListener('blur', function (e) {
                        // Hide filter
                        //self._filter.hide();

                        //
                        self.lookup();
                    });
                    self._filter.addListener('focus', function (e) {
                        // Show the filter on focus
                        //self._filter.show();

                        // Map
                        var cellinfo = nx.bucket.getCellInfo(self._filter);
                        var table = cellinfo.table;
                        nx.bucket.setInFilter(table, true);
                    });

                    // Start hidden
                    //self._filter.hide();
                    // Link
                    nx.bucket.setCellInfo(self._filter, nx.bucket.getCellInfo(self));
                    control = self._label;
                    break;

                case 'sort-icon':
                    self._sorticon = new qx.ui.basic.Image(self.getSortIcon());
                    self._sorticon.setAnonymous(true);

                    self._add(self._sorticon, {
                        row: 0,
                        column: 2
                    });
                    control = self._sorticon;

                    break;

                case 'icon':
                    self._icon = new qx.ui.basic.Image(self.getIcon()).set({
                        allowShrinkX: true
                    });

                    self._add(self._icon, {
                        row: 0,
                        column: 0
                    });

                    self._icon.addListener('click', function (e) {
                        //
                        if (!self._filter.isVisible()) {
                            // Show the fiter
                            //self._filter.show();
                            // And give it focus
                            self._filter.focus();
                        } else {
                            // Hide the filter
                            //self._filter.hide();
                            // Lookup
                            self.lookup();
                        }
                    });
                    control = self._icon;

                    break;
            }

            return control || qx.ui.table.headerrenderer.HeaderCell.prototype._createChildControlImpl.base.call(self, id);
        },

        lookup: function () {

            var self = this;

            // Map
            var widget = self._filter;
            var cellinfo = nx.bucket.getCellInfo(widget);
            var table = cellinfo.table;
            nx.bucket.setInFilter(table, false);
            var value = widget.getValue();

            // Set the icon
            self._icon.setSource(nx.util.getIcon(nx.util.hasValue(value) ? 'magnifier_zoom_in' : 'magnifier'));

            // Set the filter
            table.setFilter(cellinfo.col, value);

        }
    }
});