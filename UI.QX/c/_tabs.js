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

qx.Class.define('c._tabs', {

    extend: qx.ui.tabview.TabView,

    members: {

        initGridViews: function (req, form, win) {

            var self = this;

            nx.bucket.setItem(self, '_greq', req);
            nx.bucket.setItem(self, '_gform', form);
            nx.bucket.setItem(self, '_gwin', win);

            var holder = self;

            nx.setup.listeners(self, {

                listeners: {

                    appear: function () {

                        // Save self
                        var self = this;

                        // Get
                        var req = nx.bucket.getItem(self, '_greq');
                        nx.bucket.deleteItem(self, '_greq');
                        var form = nx.bucket.getItem(self, '_gform');
                        nx.bucket.deleteItem(self, '_gform');
                        var win = nx.bucket.getItem(self, '_gwin');
                        nx.bucket.deleteItem(self, '_gwin');

                        //
                        if (req) {
                            // Get  the params
                            var params = nx.bucket.getParams(nx.bucket.getBucketHolder(form));
                            // Local?
                            if (!req.gridview) {
                                // Assure array
                                var items = req.items;
                                if (!Array.isArray(items)) items = [items];
                                // Loop thru
                                items.forEach(function (vdef) {
                                    // Make the page
                                    var page = new qx.ui.tabview.Page(vdef.caption);
                                    // Same as a desktop
                                    page.setLayout(new qx.ui.layout.Canvas());
                                    // A bit of a margin
                                    page.setPadding(10);
                                    //
                                    var xparams = nx.util.merge({}, params);
                                    // Add the fields
                                    var hw = nx.util.addFields(win, vdef.items, page, xparams, nx.bucket.getWin(self));
                                    // Add
                                    self.add(page);
                                    // Add the renderer
                                    page.add(new nx.renderer(page));
                                });
                            } else {
                                // Get the dataset
                                var ds = nx.bucket.getDataset(win)._ds;
                                // 
                                var sysmode = nx.bucket.getParams(win).sysmode;
                                // Split tabs
                                var tabs = nx.util.splitSpace(req.gridview);
                                // Process each
                                tabs.forEach(function (pagename) {
                                    // Load the view
                                    nx.desktop._loadView(ds, pagename, function (rdef) {
                                        // Explode
                                        nx.desktop._viewExplode(ds, rdef, function (vdef) {
                                        // Flag as viewable
                                            var showpage = true;
                                            // Selector?
                                            if (vdef.selector) {
                                            // Is it on?
                                                showpage = nx.desktop.user.getIsSelector(vdef.selector);
                                            }
                                            // Do we show?
                                            if (showpage) {
                                                // Make the page
                                                var page = new qx.ui.tabview.Page(vdef.caption);
                                                // 
                                                if (sysmode) {
                                                    nx.bucket.setWin(page, self);
                                                    nx.bucket.setView(page, pagename);
                                                    var menu = new c._menu();
                                                    nx.util.createMenu(menu, [
                                                        {
                                                            label: 'Add fields',
                                                            icon: 'application_add',
                                                            click: function (e) {
                                                                alert('TBD');
                                                            }
                                                        }, {
                                                            label: 'Edit',
                                                            icon: 'application_add',
                                                            click: function (e) {
                                                                var widget = nx.util.eventGetWidget(e);
                                                                var view = nx.bucket.getView(widget);
                                                                // Call tool
                                                                nx.util.runTool('Object', {
                                                                    ds: ds,
                                                                    view: view,
                                                                    sysmode: true,
                                                                    caller: win
                                                                });
                                                            }
                                                        }
                                                    ], page);
                                                    nx.util.setContextMenu(page, menu);
                                                }
                                                // Same as a desktop
                                                page.setLayout(new qx.ui.layout.Canvas());
                                                // A bit of a margin
                                                page.setPadding(10);
                                                // Add
                                                holder.add(page);
                                                // Fields only if not in sysmode
                                                //if (!params.sysmode) {
                                                // Build fields
                                                var flds = [];
                                                Object.keys(vdef.fields).forEach(function (fld) {
                                                    flds.push(vdef.fields[fld]);
                                                });
                                                //
                                                var xparams = nx.util.merge({}, params);
                                                xparams.inview = pagename;
                                                // Get base form
                                                var bwin = nx.bucket.getWin(self);
                                                // Add to list
                                                nx.bucket.setUsedViews(bwin, pagename);
                                                // Add the fields
                                                var hw = nx.util.addFields(win, flds, page, xparams, nx.bucket.getWin(self));
                                                //
                                                // Add the renderer
                                                page.add(new nx.renderer(page));
                                            }

                                        });
                                    });
                                });
                            }

                            // Save settings
                            nx.bucket.setParams(self, req);
                            nx.bucket.setDataset(self, req.ds);
                        }

                    }
                }
            });
        },

        getValue: function () {

            var self = this;

            return nx.util.getValue(self);

        },

        setValue: function (value) {

            var self = this;

            return nx.util.setValue(self, value);
        }

    }

});