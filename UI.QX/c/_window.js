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

qx.Class.define('c._window', {

    extend: qx.ui.window.Window,

    construct: function () {

        // Save self
        var self = this;

        // Remove the req
        var req = Array.prototype.shift.call(arguments);

        // Call base
        this.base(arguments);

        // Do we have values?
        if (req.obj) {
            // Save
            self._aoobject = req.obj;
        }

        // Assure canvas
        self.setLayout(new qx.ui.layout.VBox());

        // Setup fields
        nx.bucket.setFields(self, {});
        nx.bucket.setChanges(self, []);
        nx.bucket.setDataset(self, nx.bucket.getDataset(req));
        nx.bucket.setView(self, nx.bucket.getView(req));

        // Handle extended entries
        if (req.defaultCommand) {
            req.keyupCB = function (widget, keycode) {
                if (keycode === 13) {
                    nx.bucket.getForm(widget).doCommand(req.defaultCommand);
                }
            };
        }

        // Assure 
        req.nxid = req.nxid || nx.util.uuid();
        req.caption = req.caption || 'Untitled';

        // Phony 
        self.nxtype = 'window';

        // Set buttons
        self.setShowClose(false);
        self.setShowMinimize(nx.util.default(req.allowMinimize, false));
        self.setShowMaximize(nx.util.default(req.allowMaximize, false));

        // Handle close
        if (nx.util.default(req.allowClose, true)) {

            // Assure commands
            req.commands = req.commands || {};
            req.commands.items = req.commands.items || [];
            // Assume none
            var cancelfound = false;
            var sepfound = false;
            // Loop thru
            req.commands.items.forEach(function (cmd) {
                // Cancel?
                if (cmd.label === 'Close') {
                    cancelfound = true;
                }
                // Separator?
                if (typeof cmd === 'string' && cmd === '>') {
                    sepfound = true;
                }
            });
            // Do we need to add?
            if (!cancelfound) {
                // Do we have a separator
                if (!sepfound) {
                    // Add
                    req.commands.items.splice(0, 0, '>');
                }
                // Add
                req.commands.items.splice(0, 0, {
                    label: 'Close',
                    icon: 'cancel',
                    click: function (e) {

                        var self = this;

                        // Map window
                        var win = nx.bucket.getWin(self);

                        // Callback
                        if (req.atCancel) req.atCancel();

                        // Close
                        win.safeClose();

                    }

                });
            }
        }

        // Configure
        nx.setup.__component(self, req);

        // Internal listeners
        var iListeners = {

            changeActive: function () {

                var self = this;

                // Get the button
                var button = self.taskbarButton;

                // Any?
                if (button) {

                    // Active?
                    if (self.getActive()) {

                        // Set the icon
                        button.setIcon(nx.util.getIcon(nx.setup.iconActiveTask));

                        // Set
                        nx.desktop.setActiveWindow(self);

                        //
                        nx.desktop.getDesktop().scrollChildIntoView(self);

                    } else {

                        // Reset
                        button.setIcon(self.getIcon());

                    }

                }

            },

            changeCaption: function () {

                var self = this;

                // Get the button
                var button = self.taskbarButton;
                // Any?
                if (button) {

                    // Change it
                    button.setLabel(self.getCaption());

                }

            },

            beforeClose: function () {

                var self = this;

                // Do we clear children?
                if (!nx.desktop.isCtrl()) {
                    // Get children
                    var list = self.getChildWindows();
                    // Loop thru
                    list.forEach(function (child) {
                        nx.bucket.setCaller(nx.bucket.getForm(child));
                        child.safeClose();
                    });
                } else {
                    // Get position
                    var bounds = nx.util.getAbsoluteBounds(self);
                    // Get the caller
                    var caller = nx.bucket.getCaller(self);
                    // Reposition
                    self.getChildWindows().forEach(function (win) {
                        // Change the parent
                        nx.bucket.setCaller(win, caller);
                        // Get the bounds
                        var cbounds = nx.util.getAbsoluteBounds(win);
                        // Actual bounds
                        var abounds = {
                            left: bounds.left,
                            top: bounds.top,
                            width: cbounds.width,
                            height: cbounds.height
                        };
                        // Move
                        win.locateWindow(abounds);
                    });
                }

                // Get the caller
                var caller = nx.bucket.getCaller(nx.bucket.getForm(self));
                // Any?
                if (caller && caller.removeChildWindow) {
                    // Remove link
                    caller.removeChildWindow(self);
                    // Make sure caler is visible
                    if (caller.isMinimized()) {
                        caller.restore();
                    }
                    //caller.showRestore();
                }

                // Delete taskbar button
                if (self.taskbarButton) nx.desktop.windowButtons.remove(self.taskbarButton);

                // Get params
                var params = nx.bucket.getParams(self);
                if (params && params.nxid) {
                    // Remove SIO
                    nx.desktop.removeSIO(params.nxid);
                    // And external
                    nx.desktop.removeTP(params.nxid);
                }

                nx.desktop.clearActiveWindow(self);
            },

            beforeMinimize: function (e) {

                var self = this;

                // Save
                self.setStoredBounds();

                // Only after all done
                if (self._rendered) {

                    // Get the bounds
                    var bounds = nx.util.getAbsoluteBounds(self);

                    // Move children
                    self.locateWindow(bounds, 'm');

                }
            },

            beforeRestore: function (e) {

                var self = this;

                // Get the bounds
                var bounds = self.getStoredBounds();
                var mode = 'r';

                // Only if we do not stack
                if (nx.desktop.user.getOpenModeChild() != 'stack') {
                    // Do we have a caller?
                    var caller = nx.bucket.getCaller(self);
                    if (caller) {
                        // Loop
                        while (caller) {

                            // Is caller visible?
                            if (!caller.isMinimized()) {

                                // Get its bound
                                var cbounds = nx.util.getAbsoluteBounds(caller);
                                // Recalc
                                bounds = {
                                    top: cbounds.top,
                                    left: cbounds.left + cbounds.width,
                                    width: bounds.width,
                                    height: bounds.height
                                };
                                mode = 'a';
                                // And no more
                                caller = null;

                            } else {

                                // Get its caller
                                caller = nx.bucket.getCaller(caller);

                            }
                        }

                    }
                }

                // 
                self._restorebounds = bounds;
                self._restoremode = mode;
                self.locateWindow(bounds, mode);

            },

            move: function (e) {

                var self = this;

                // Do we skip?
                if (!self._skipmove) {

                    // Are we visible?
                    if (!self.isMinimized()) {
                        // Get bounds
                        var bounds = e.getData();

                        //self.dumpTrace('move', bounds);

                        // Move
                        self.locateWindow(bounds);
                    }

                }

            },

            appear: function () {

                var self = this;

                // Map the form
                var form = nx.bucket.getForm(self);
                var params = nx.bucket.getParams(form);
                // Set the id 
                if (params.nxid) {
                    self.getContentElement().getDomElement().id = params.nxid;
                }

                // Are we rendered?
                if (self._rendered) {

                    // Get bounds
                    var bounds = nx.util.getAbsoluteBounds(self);
                    var mode;

                    if (self._restorebounds) {

                        bounds = self._restorebounds;
                        delete self._restorebounds;
                        mode = self._restoremode;
                        delete self._restoremode;

                    }

                    // Redraw
                    self.locateWindow(bounds, mode);

                } else {

                    // Add adjustable
                    if (params.sysmode) {

                        var spacing = [nx.setup.rowHeight + nx.setup.rowSpacing, nx.setup.colWidth + nx.setup.colSpacing];

                        // Get list
                        var widgets = $("[qxclass='nx.overlay']");
                        // Link
                        widgets.draggable({ grid: spacing }, {
                            stop: function (e) {
                                // Assume none
                                var widget
                                // Get children of container
                                var list = e.target.$$widgetObject.getChildren();
                                // Loop thru
                                list.some(function (poss) {
                                    // Does it have a container?
                                    if (nx.bucket.getContainer(poss)) {
                                        //Save
                                        widget = poss;
                                    }
                                    return widget;
                                });
                                // Widget?
                                if (widget) {

                                    // Spacing
                                    var rowSpacing = nx.setup.rowHeight + nx.setup.rowSpacing;
                                    var colSpacing = nx.setup.colWidth + nx.setup.colSpacing;
                                    // Get bounds
                                    var bounds = nx.util.getRelativeBounds(nx.bucket.getContainer(widget));
                                    bounds.top = nx.util.fromRelative(bounds.top, rowSpacing) - 1;
                                    bounds.left = nx.util.fromRelative(bounds.left, colSpacing);
                                    // Map
                                    var form = nx.bucket.getForm(widget);
                                    var ds = nx.bucket.getDataset(form);
                                    if (ds) {
                                        var view = nx.bucket.getView(form);
                                        var params = nx.bucket.getParams(widget);
                                        var fieldname = params.aoFld;
                                        nx.desktop._alterView(ds._ds, view._id, fieldname, {
                                            top: bounds.top.toString(),
                                            left: bounds.left.toString()
                                        }, nx.util.noOp);
                                    }
                                }
                            }
                        });
                    } else {
                        self.setResizable(false);
                    }

                    // Add context menu
                    nx.util.makeWindowContextMenu(self, req.sysmode);

                    // Get children
                    Object.keys(nx.bucket.getFields(self)).some(function (field) {
                        // Get the widget
                        var widget = nx.bucket.getFields(self)[field];
                        // Can we focus?
                        if (widget.getFocusable && widget.getFocusable()) {
                            // Focus
                            widget.focus();
                            // Only once
                            return true;
                        } else {
                            return false;
                        }
                    });

                    // Get button
                    if (self.topToolbar) {
                        var btn = self.getButtonWithLabel(self.topToolbar, 'Parent');
                        if (btn) {
                            // Get parent
                            var parent = self.getParent();
                            if (nx.util.hasValue(parent)) {
                                // Parse
                                var info = nx.util.uuidToObject(parent);
                                // Get the dataset
                                nx.desktop._loadDataset(info.ds, function (ds) {
                                    // Get data
                                    nx.desktop._loadData(info.ds, info.id, function (data) {
                                        // If none
                                        var desc = 'Missing ' + ds.caption;
                                        // Adjust desc
                                        if (data && data.values) {
                                            desc = nx.util.localizeDesc(data.values._desc);
                                        }
                                        // Fill button
                                        btn.setLabel(desc);
                                        btn.setIcon(nx.util.getIcon('database_table')); // ds.icon
                                        nx.bucket.setParent(btn, parent);
                                    });
                                });
                            } else {
                                self.topToolbar.remove(btn);
                            }
                        }
                    }

                    //
                    nx.setup.__if(self, req, 'center', function (widget, sett) {

                        var top = 0;
                        var left = 0;

                        // Adjust horizontal
                        if (widget.setDomLeft && (sett === 'horz' || sett == 'both')) {

                            // Get extra space
                            left = (nx.util.getAbsoluteBounds(nx.desktop).width - nx.util.getAbsoluteBounds(widget).width) / 2;
                            // Set left
                            widget.setDomLeft(left);

                        }

                        // Adjust vertical
                        if (widget.setDomTop && (sett === 'vert' || sett == 'both')) {

                            // Get extra space
                            top = (nx.util.getAbsoluteBounds(nx.desktop).height - nx.util.getAbsoluteBounds(widget).height) / 2;
                            // Set left
                            widget.setDomTop(top);

                        }

                        //
                        self._rendered = true;

                    }, function () {

                        // Start top left
                        var top = 0;
                        var left = 0;
                        var offset = true;

                        // The windows to check
                        var windows;
                        var omfn = nx.desktop.user.getOpenMode();
                        // Get the caller
                        var caller = nx.bucket.getCaller(nx.bucket.getForm(self));
                        // Any?
                        if (caller && caller.getChildWindows) {
                            // Get
                            windows = caller.getChildWindows();
                            // Adjust
                            var cbounds = nx.util.getAbsoluteBounds(caller);
                            //
                            switch (nx.desktop.user.getOpenModeChild()) {
                                case 'right':
                                    top = cbounds.top;
                                    left = cbounds.left + cbounds.width;
                                    offset = false;
                                    break;
                                case 'bottom':
                                    top = cbounds.top + cbounds.height;
                                    left = cbounds.left;
                                    offset = false;
                                    break;
                            }
                        } else {
                            // All
                            windows = nx.desktop.getChildWindows();
                        }

                        // Assure
                        windows = windows || [];
                        if (!windows.length && caller) {
                            windows = [caller];
                        }

                        // Any?
                        if (windows && windows.length) {

                            // Get top/left
                            var rootwin = null;
                            var rootoffset = 0;
                            var rootbounds = null;
                            windows.forEach(function (win) {

                                // Get the bounds
                                var bounds = nx.util.getAbsoluteBounds(win);
                                // Get offset
                                var coffset = nx.util.getDistance(bounds.top, bounds.left, 0, 0);

                                // If none, use it
                                if (!rootwin || coffset < rootoffset) {
                                    rootwin = win;
                                    rootoffset = coffset;
                                    rootbounds = bounds;
                                }

                            });

                            // Loop thru
                            windows.forEach(function (win) {

                                // Skip the root
                                if (win !== rootwin) {
                                    // Get the bounds
                                    var bounds = nx.util.getAbsoluteBounds(win);
                                    // Must be below or to the right
                                    if (bounds.top >= rootbounds.top && bounds.left >= rootbounds.left) {
                                        // But within reach
                                        if ((bounds.top - rootbounds.top) <= nx.setup.winOffsetDiff &&
                                            (bounds.left - rootbounds.left) <= nx.setup.winOffsetDiff) {
                                            // Switch
                                            rootwin = win;
                                            rootbounds = bounds;
                                        }
                                    }
                                }

                            });

                            // Use the root window
                            top = rootbounds.top;
                            left = rootbounds.left;

                            var poss;

                            // According to open mode
                            switch (omfn) {
                                case 'right':
                                    do {
                                        // Move
                                        left += rootbounds.width;
                                        // Get window to the right
                                        poss = nx.desktop.getWindowAt(top, left);
                                        //
                                        if (poss) {
                                            // Get bounds
                                            rootbounds = nx.util.getAbsoluteBounds(poss);
                                        }
                                    } while (poss);
                                    break;
                                case 'bottom':
                                    do {
                                        // Move
                                        top += rootbounds.height;
                                        // Get window below
                                        poss = nx.desktop.getWindowAt(top, left);
                                        //
                                        if (poss) {
                                            // Get bounds
                                            rootbounds = nx.util.getAbsoluteBounds(poss);
                                        }
                                    } while (poss);
                                    break;

                                default:
                                    // Adjust
                                    top += nx.setup.winOffset;
                                    left += nx.setup.winOffset;
                                    break;
                            }
                        } else {

                            // Adjust
                            if (offset) {
                                top += nx.setup.winOffset;
                                left += nx.setup.winOffset;
                            }
                        }

                        // Link
                        nx.bucket.setCaller(self, req.caller);

                        // Get the width
                        var width = nx.util.getAbsoluteBounds(self).width;
                        var dwidth = nx.util.getAbsoluteBounds(nx.desktop).width;

                        var callers = [];
                        var caller = nx.bucket.getCaller(self);

                        while (caller) {
                            callers.unshift(caller);
                            caller = nx.bucket.getCaller(caller);
                        }

                        var wleft = left;

                        // Does it put ourselves outside physical?
                        while ((wleft + width) > dwidth && callers.length) {
                            // Get the caller 
                            var caller = callers.shift();
                            // If not minimized
                            if (!caller.isMinimized()) {
                                // get width
                                var cwidth = nx.util.getAbsoluteBounds(caller).width;
                                // Minimize
                                caller.minimize();
                                // Move back
                                wleft -= cwidth;
                            }
                        }

                        // Position
                        self.setDomTop(top);
                        self.setDomLeft(left);

                        //
                        self._rendered = true;

                    });

                    // Handle resizing
                    self.addListener('resize', function (e) {
                        // Get the window
                        var win = e.getTarget()
                        // Was it just resized?
                        if (win._skipResize) {
                            // Reset
                            delete win._skipResize;
                        } else {
                            // Get the bounds
                            var bounds = nx.util.getAbsoluteBounds(win);
                            // A difference?
                            if (Math.abs(bounds.height - self.getStoredBounds().height) > 2) {
                                // Only once
                                win._skipResize = true;
                                // Make room for rendered
                                var renderer;
                                // Loop thru
                                win.getChildren().forEach(function (widget) {
                                    // Is it the renderer?
                                    if (widget.classname === 'nx.renderer') {
                                        // Save
                                        renderer = widget;
                                    } else {
                                        // Remove from available
                                        bounds.height -= nx.util.getAbsoluteBounds(widget).height;
                                    }
                                });
                                // Do we have a rendered?
                                if (renderer) {
                                    // Adjust height
                                    renderer.setHeight(bounds.height);
                                    renderer.setWidth(bounds.width);
                                }
                            }
                        }
                    });

                    // Getthe caller
                    var caller = nx.bucket.getCaller(nx.bucket.getForm(self));
                    // Any?
                    if (caller && caller.addChildWindow) {
                        // Link
                        caller.addChildWindow(self);
                    }

                    // Save size
                    self.setStoredBounds();

                    // Fill
                    //if (self._aoobject) {
                    //    // Fill
                    //    self.setFormData(self._aoobject.values);
                    //}

                }
            }

        };
        // Setup internal
        nx.setup.listeners(self, {
            listeners: iListeners
        });

        // Create taskbar button
        self.taskbarButton = new qx.ui.toolbar.Button(self.getCaption());
        nx.setup.listeners(self.taskbarButton, {

            listeners: {

                click: function (e) {

                    // Get the widget
                    var widget = nx.util.eventGetWidget(e);
                    // Get the window
                    var win = widget.window;
                    // Any?
                    if (win) {

                        // Minimized?
                        if (win.isMinimized()) {

                            // Restore
                            win.restore();

                            // Activate
                            win.setActive(true);

                        } else if (win.isActive()) {

                            // Minimize
                            if (!win.isModal()) {
                                win.minimize();
                            }

                        } else {

                            // Activate
                            win.setActive(true);

                        }

                    }

                }

            }
        });
        // Link back to window
        self.taskbarButton.window = self;
        // Add to taskbar
        nx.desktop.windowButtons.add(self.taskbarButton);

        // Top toolbar
        nx.setup.__if(self, req, 'topToolbar', function (widget, sett) {

            // Make
            widget.topToolbar = nx.util.createToolbar(sett, widget);
            // Add
            widget.add(widget.topToolbar);

        });

        // Make the form
        // Create a container for the main layout and set the main layout
        var form;
        if (req.sysmode) {
            form = new qx.ui.container.Resizer(new qx.ui.layout.Canvas());
        } else {
            form = new qx.ui.container.Composite(new qx.ui.layout.Canvas());
        }
        form.setPadding(0);
        nx.bucket.setForm(self, form);

        // Save settings
        nx.bucket.setParams(form, req);
        nx.bucket.setParent(form, req.caller);
        nx.bucket.setChildren(form, []);
        nx.bucket.setDataset(form, nx.bucket.getDataset(req));
        nx.bucket.setView(form, nx.bucket.getView(req));
        // Caller chain
        nx.bucket.setChain(form, req.chain || Object.assign({}, nx.bucket.getChain(form) || {}));
        // Do we have an object?
        if (req.ds && req.id) {
            // Replace
            nx.bucket.getChain(form)[req.ds] = req.id;
        }

        self.setUseMoveFrame(true);

        // Body
        nx.setup.__if(self, req, 'items', function (widget, sett) {

            // Add
            self.addFields(sett);

        });

        // Make the renderer
        self.add(new nx.renderer(nx.bucket.getForm(self)));

        // Bottom toolbar
        nx.setup.__if(self, req, 'bottomToolbar', function (widget, sett) {

            // Make
            widget.bottomToolbar = nx.util.createToolbar(sett, widget);
            // Add
            self.add(widget.bottomToolbar);

        });

        // Comamnd line
        nx.setup.__if(self, req, 'commands', function (widget, sett) {

            // Make
            widget.commandToolbar = nx.util.createToolbar(sett, widget, req.defaultCommand);
            // Add
            self.add(widget.commandToolbar);

        });

        // ExtraComamnd line
        nx.setup.__if(self, req, 'syscommands', function (widget, sett) {

            // Make
            widget.sysToolbar = nx.util.createToolbar(sett, widget, req.defaultCommand);
            // Add
            self.add(widget.sysToolbar);

        });

        // Values
        nx.setup.__if(self, req, 'value', function (widget, sett) {

            // Load
            widget.setFormData(sett);

        });

        // SIO 
        nx.setup.__if(self, req, 'processSIO', function (widget, sett) {

            // Set
            widget.processSIO = sett;

        });

    },

    members: {

        addFields: function (sett, cb) {

            var self = this;

            //
            var form = nx.bucket.getForm(self);
            var req = nx.bucket.getParams(form);

            // Make
            nx.util.addFields(self, sett, form, req, self, cb);

        },

        getFields: function () {

            var self = this;

            var ans = [];
            // Loop thru
            self.getChildren().forEach(function (entry) {
                if (nx.bucket.getWidgets(entry) && nx.bucket.getWidgets(entry).length) {
                    ans.push(nx.bucket.getWidgets(entry)[0]);
                }
            });

            return ans;
        },

        getFieldsOfClass: function (cname) {

            var self = this;

            var ans = [];
            // Loop thru
            self.getChildren().forEach(function (entry) {
                // Does it match?
                if (entry.classname === cname) {
                    ans.push(entry);
                }
                // Get shown
                var items = nx.bucket.getChildren(entry);
                // Any?
                if (items && items.length) {
                    // Loop thru
                    items.forEach(function (item) {
                        if (item.classname === cname) {
                            ans.push(item);
                        }
                    });
                }
            });

            return ans;
        },

        showFieldDefinition: function (flds, ds, dsdef, view, viewdef) {

            var self = this;

            // Set the position
            var row = 1;
            // Loop thru
            Object.keys(viewdef.fields).forEach(function (fname) {
                // Get the definition
                var def = viewdef.fields[fname];
                //  Get the top
                var top = nx.util.toInt(def.top) + 1;
                // Better one?
                if (row < top) row = top;
            });

            // Assure array
            if (!Array.isArray(flds)) flds = [flds];
            //
            var widgets = [];
            var source = nx.bucket.getFields(self);
            // Loop thru
            flds.forEach(function (fieldname) {
                // Get the field in the form
                var field = source[fieldname];
                if (field) {
                    widgets.push(field);
                }
            });

            // Call
            nx.util.runTool('PropertiesField', {
                widget: widgets,
                caller: self
            });
        },

        getButtonWithLabel: function (list, label) {

            var self = this;

            // Assure list
            if (!Array.isArray(list)) {
                list = list.getChildren();
            }

            //
            var ans;
            list.some(function (widget) {
                if (widget.getLabel && widget.getLabel() === label) {
                    ans = widget;
                }
                return ans;
            });

            return ans;
        },

        setStoredBounds: function () {

            var self = this;

            // Only after setup
            if (self._rendered) {
                // Get the bounds
                var bounds = nx.util.getAbsoluteBounds(self);
                // Not if default
                if (bounds.top || bounds.left || bounds.width || bounds.height) {
                    // Save
                    nx.bucket.setBounds(self, bounds);
                }
            }
        },

        getStoredBounds: function () {

            var self = this;

            return nx.bucket.getBounds(self);
        },

        isMinimized: function () {

            var self = this;

            return self.getMode() === 'minimized';
        },

        //dumpTrace: function (fn, bounds, mode) {

        //    var self = this;

        //    var info = fn + ' - ' + self.getCaption();
        //    info += ', b : ' + JSON.stringify(bounds);
        //    if (mode) info += ' - m: ' + mode;
        //    console.log(info);

        //},

        locateWindow: function (bounds, mode) {

            var self = this;

            // Assure counter
            self._skipmove = self._skipmove || 0;
            // Assure count
            self._skipmove++;

            //self.dumpTrace('locateWindow', bounds, mode);

            // Move
            self.moveTo(bounds.left, bounds.top);

            // Do children
            self.moveChildren(bounds, mode);

            // Reset
            self._skipmove--;

        },

        moveChildren: function (bounds, mode) {

            var self = this;

            //self.dumpTrace('moveChildren', bounds, mode);

            // Get the children
            var children = self.getChildWindows();
            // Must have some
            if (children.length) {

                // Assure counter
                self._skipmove = self._skipmove || 0;
                // Assure count
                self._skipmove++;

                // Get the bounds
                //var bounds = nx.util.getAbsoluteBounds(self);

                // Reset bounds if minimized
                if (mode !== 'a') {
                    if (mode !== 'r' && ((mode === 'm') || self.isMinimized())) {

                        // Do we have a caller?
                        var caller = nx.bucket.getCaller(self);
                        if (caller) {
                            // Loop
                            while (caller) {

                                // Is caller visible?
                                if (!caller.isMinimized()) {

                                    // Get its bound
                                    bounds = nx.util.getAbsoluteBounds(caller);

                                    // And no more
                                    caller = null;

                                } else {

                                    // Get its caller
                                    caller = nx.bucket.getCaller(caller);

                                    if (!caller) {

                                        // Default
                                        bounds = {
                                            top: nx.setup.winOffset,
                                            left: nx.setup.winOffset,
                                            height: 0,
                                            width: 0
                                        };

                                    }

                                }
                            }

                        } else {

                            // Default
                            bounds = {
                                top: nx.setup.winOffset,
                                left: nx.setup.winOffset,
                                height: 0,
                                width: 0
                            };

                        }

                    }
                }

                //self.dumpTrace('moveChildren (after)', bounds, mode);

                // Get the upper bounds
                var ubounds;
                // Loop thru
                children.forEach(function (child) {
                    // Must be visible
                    if (!child.isMinimized()) {
                        // Get the bounds
                        var cubounds = nx.util.getAbsoluteBounds(child);
                        // First?
                        if (!ubounds) {
                            ubounds = cubounds;
                        } else {
                            // Lower?
                            if (cubounds.top < ubounds.top && cubounds.left < ubounds.left) {
                                ubounds = cubounds;
                            }
                        }
                    }
                });

                // Adjust all
                children.forEach(function (child) {
                    // Get the location
                    var cbounds = nx.util.getAbsoluteBounds(child);
                    // Delta
                    var dtop = 0;
                    var dleft = 0;
                    if (ubounds) {
                        dtop = cbounds.top - ubounds.top;
                        dleft = cbounds.left - ubounds.left;
                    }
                    // Actual bounds
                    var abounds = {
                        left: bounds.left + bounds.width + dleft,
                        top: bounds.top + dtop,
                        width: cbounds.width,
                        height: cbounds.height
                    };
                    // Move
                    child.locateWindow(abounds, (child.isMinimized() ? 'a' : null));
                });

                // Reset
                self._skipmove--;

            }

        },

        getRenderer: function () {

            var self = this;

            var ans;

            // Get possible
            var list = self.getFieldsOfClass('nx.renderer');
            // Any?
            if (list && list.length) {
                ans = list[0];
            }

            return ans;
        },

        showAllow: function () {

            var self = this;

            // Resize screen
            var bounds = nx.util.getAbsoluteBounds(self);
            var allowed = 0.66 * nx.setup.colWidth * nx.default.get('default.screenWidth');
            if (bounds.width >= allowed) {
                var shrink = nx.default.get('default.pickAdjust');
                self.adjustWidth(-shrink);
                nx.bucket.setItem(self, 'adjust', shrink)
            }

        },

        showRestore: function () {

            var self = this;

            var adjust = nx.bucket.getItem(self, 'adjust');
            if (adjust) {
                nx.bucket.setItem(self, 'adjust');
                self.adjustWidth(adjust);
            }

        },

        showMax: function () {

            var self = this;

            // Get the bounds 
            var bounds = nx.util.getAbsoluteBounds(nx.desktop);
            // Horizontal
            var width = nx.default.get('default.screenWidth') * nx.setup.colWidth;
            var left = (bounds.width - width) / 2;
            // Set left
            self.setDomLeft(left);
            self.setWidth(width);
            // Vertical
            var height = nx.default.get('default.screenHeight') * nx.setup.rowHeight;
            var top = (bounds.height - height) / 4;
            // Set left
            self.setDomTop(top);
            self.setHeight(height);

        },

        safeClose: function () {

            var self = this;

            try {
                self.close();
            } catch (e) {
                self.close();
            }

        },

        // ---------------------------------------------------------
        //
        // AO
        // 
        // ---------------------------------------------------------

        /**
         * 
         * The underlying AO Object
         * 
         */
        _aoobject: null,

        _call: function (fn) {

            var self = this;

            // Get the children
            var children = nx.bucket.getChildren(self);
            // Any?
            if (children) {

                // Loop thru
                children.forEach(function (winid) {

                    // Get window
                    var win = nx.desktop.findWindow(winid);
                    // Any?
                    if (win) {
                        // Call
                        var wfn = win[fn];
                        if (wfn) wfn();
                    }

                });

            }

        },

        close: function () {

            var self = this;

            // Call children handler
            self._call('close');

            arguments.callee.base.apply(this);
        },

        /**
         * 
         * Saves AO Object and closes window
         * 
         */
        save: function (cb) {

            var self = this;

            // Do we have an object?
            if (self._aoobject) {
                // Get the data
                var data = self.getFormData();

                // Get the form
                var form = nx.bucket.getForm(self);
                // Any?
                if (form) {
                    // Get params
                    var params = nx.bucket.getParams(form);
                    // Any?
                    if (params) {
                        // Get the callback
                        var lcb = params.atSave;
                        // Any?
                        if (lcb) {
                            //
                            lcb(data);
                        }
                    }
                }

                // Loop thru
                Object.keys(data).forEach(function (field) {
                    // Changed?
                    var changed = self._aoobject.hasChanged(field, data[field]) || field === '_parent';
                    // Update if changed
                    if (changed) {
                        self._aoobject.setField(field, data[field]);
                    }
                });

                // Write out
                nx.desktop.aomanager.set(self._aoobject, function (ok) {

                    // Call children handler
                    self._call('save');

                    if (cb) {

                        cb(ok);

                    } else {

                        // Close
                        self.safeClose();

                    }
                });

            } else {

                // Close
                self.safeClose();

            }

        },

        // ---------------------------------------------------------
        //
        // FIELDS
        // 
        // ---------------------------------------------------------

        /**
         * 
         * Gets a field
         * 
         * @param {string} field
         */
        getField: function (field) {

            var self = this;

            return nx.bucket.getFields(self)[field];

        },

        /**
         * 
         * Geta a field value
         * 
         * @param {string} field
         */
        getValue: function (field) {

            var self = this;

            // Assume none
            var ans = null;

            // Get the field
            var phy = self.getField(field);
            // Valid?
            if (phy && phy.getValue) {
                // Get
                ans = phy.getValue();
            }

            return ans;

        },

        /**
         * 
         * Sets a field value
         * 
         * @param {string} field
         * @param {any} value
         */
        setValue: function (field, value) {

            var self = this;

            // Get the field
            var phy = self.getField(field);
            // Valid?
            if (phy && phy.setValue) {
                // Assure value
                if (typeof value === 'undefined') value = null;
                nx.util.processFormatters(phy, value, 0, function (value) {
                    // Did it change?
                    if (!nx.util.isSameValue(value, phy.getValue())) {
                        // Set
                        phy.setValue(value);
                    }
                });
            }
        },

        getParent: function () {

            var self = this;

            var ans;

            if (self._aoobject) {
                ans = self._aoobject.values._parent;
            }

            return ans;

        },

        /**
         * 
         * Getsthe value from the object
         * 
         * @param {any} field
         */
        getObjValue: function (field) {

            var self = this;

            var ans;

            if (self._aoobject) {
                ans = self._aoobject.values[field];
            }

            return ans;
        },

        // ---------------------------------------------------------
        //
        // FORMS
        // 
        // ---------------------------------------------------------

        /**
         * 
         * Gets all of the data in the form.
         * 
         */
        getFormData: function () {

            var self = this;

            // Response
            var ans = {};

            // Get the list
            var list = self.getFormFields()
                ;

            // Loop thru
            list.forEach(function (field) {
                // Save
                ans[field] = self.getValue(field) || '';
            });

            // Parent
            var parent = self.getParent();
            if (parent) {
                ans._parent = parent;
            }

            return ans;

        },

        /**
         * 
         * Gets all of the field names in the form.
         *If chgsonly is set, only the fields that have changed are returned
         * 
         */
        getFormFields: function () {

            var self = this;

            // Get the list
            return Object.keys(nx.bucket.getFields(self));

        },

        /**
         * 
         * Sets the data for the entire form.
         * If chgsonly is set, only the fields in values
         * are updated, otherwise the entire form is updated
         * 
         * @param {object} values
         */
        setFormData: function (values) {

            var self = this;

            // Must have a value
            if (values) {
                // Loop thru
                Object.keys(values).forEach(function (field) {
                    // Get the value
                    var value = values[field];
                    // Parent link?
                    if (field !== '_parent') {
                        // Filter known fields only
                        self.setValue(field, value);
                    }
                });
            }

        },

        /**
         * 
         * Returns the changes in the form
         * 
         */
        getFormDataChanges: function () {

            var self = this;

            var ans = {};// Do we have an object?
            if (self._aoobject) {
                // Get the data
                var data = self.getFormData();

                // Loop thru
                Object.keys(data).forEach(function (field) {
                    // Changed?
                    var changed = self._aoobject.hasChanged(field, data[field]) || field === '_parent';
                    // Update if changed
                    if (changed) {
                        ans[field] = data[field];
                    }
                });
            }

            return ans;
        },

        /**
         * 
         * Returns the changes in the form
         * 
         */
        setFormDataChange: function (field) {

            var self = this;

            if (self._aoobject) {
                // Fla
                self._aoobject.markField(field);
            }
        },

        doCommand: function (label) {

            var self = this;

            // Loop thry
            self.commandToolbar.getChildren().forEach(function (cmd) {
                // Is this it?
                if (label === cmd.$$user_label) {
                    cmd.fireEvent('execute');
                }
            });
        },

        // ---------------------------------------------------------
        //
        // CHILDREN
        // 
        // ---------------------------------------------------------

        getChildWindows: function () {

            var self = this;

            // Assure
            self._childWindows = self._childWindows || [];

            return self._childWindows;

        },

        addChildWindow: function (child) {

            var self = this;

            // Assure
            self._childWindows = self._childWindows || [];

            // Add
            self._childWindows.push(child);

        },

        removeChildWindow: function (child) {

            var self = this;

            // Assure
            self._childWindows = self._childWindows || [];

            // Find
            var pos = self._childWindows.indexOf(child);
            // Any?
            if (pos !== -1) {
                self._childWindows.splice(pos, 1);
            }

        }
    }

});