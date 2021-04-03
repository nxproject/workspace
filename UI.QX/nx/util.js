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

nx.util = {

    /**
     * 
     * Gets the value, if undefined use default
     * 
     * @param {any} entry
     * @param {any} defaultvalue
     */
    default: function (entry, defaultvalue) {
        // If undefined
        if (typeof entry === 'undefined') entry = defaultvalue;

        return entry;
    },

    /**
     * 
     * Returns a universal unique ID
     * 
     */
    uuid: function () {
        // From: https://stackoverflow.com/questions/3231459/create-unique-id-with-javascript
        return Date.now().toString(36) + Math.random().toString(36).substr(2);
    },

    /**
     * 
     * Does nothing
     * 
     */
    noOp: function () { },

    // ---------------------------------------------------------
    //
    // COMPONENTS
    // 
    // ---------------------------------------------------------

    /**
     * 
     * Creates component
     * 
     * @param {any} req
     * @param {widget} parent
     */
    createComponent: function (req, parent, cb) {

        // Assume failure
        var ans = null;

        // Must be object
        if (typeof req === 'object' && req.jtype) {

            // Assure
            nx.office.load('c.' + req.jtype, function () {

                // Make
                var widget = eval('new c.' + req.jtype + '()');

                //
                if (cb) cb();

            });

        } else {
            //
            if (cb) cb();
        }
    },

    /**
     * 
     * Creates a menu broken by first letter
     * 
     * @param {any} value
     * @param {any} widget
     */
    createAlphabeticalMenu: function (value, widget) {

        var self = this;

        // Make the menu
        var menu = new c._menu();

        // List of letters used
        var alpha = [];
        // Loop thru
        value.forEach(function (entry) {
            //
            var letter;
            // String?
            if (typeof entry === 'string') {
                // Get first letter
                letter = entry.substr(0, 1);
            } else {
                // Get the label
                letter = entry.label.substr(0, 1);
            }
            // Skip separators
            if (letter !== '-') {
                // Add if new
                if (!alpha.indexOf(letter) === -1) {
                    alpha.push(letter);
                }
            }
            // Sort
            alpha.sort();
            // Loop thru
            alpha.forEach(function (letter) {
                // Make the holder
                var holder = new c._menuButton(letter);
                // Now the sub menu
                var submenu = new c._menu();
                // Make list
                var list = [];
                // Loop thru
                value.forEach(function (entry) {
                    // Assume string
                    var label = entry;
                    // Check
                    if (typeof entry !== string) {
                        label = entry.label;
                    }
                    // Matches?
                    if (letter === label.substr(0, 1)) {
                        // Add
                        list.push(entry);
                    }
                });
                // Make the menu
                self.createMenu(submenu, list, widget);
                // Save
                holder.setMenu(submenu);
                // Add to menu
                menu.add(holder);
            });
        });

        return menu;
    },

    makeMenu: function (sett, widget) {
        // Create the menu object
        var menu = new c._menu();
        // And now the menu
        nx.util.createMenu(menu, sett, widget);
        // Map
        widget.setMenu(menu);
    },

    /**
     * 
     * Creates a menu
     * 
     * @param {widget} menu
     * @param {any} value
     */
    createMenu: function (menu, value, widget) {

        var self = this;

        // Must have some
        if (value) {

            // Assure array
            if (!Array.isArray(value)) {
                value = [value];
            }

            // Loop thru
            value.forEach(function (entry) {

                if (!entry) {

                    // A null is a separator
                    menu.addSeparator();

                } else if (typeof entry === 'string') {

                    // Separator?
                    if (entry === '-') {

                        // Add
                        menu.addSeparator();

                    } else {

                        var button = new qx.ui.menu.Button(entry);
                        // Create
                        self.createMenuButton(menu, { label: entry, tool: entry }, widget);
                    }

                } else if (Array.isArray(entry)) {

                    // Make button
                    var button = new qx.ui.menu.Button('Select');
                    // Sub-menu
                    var subMenu = new c._menu();
                    // Link in
                    self.createMenu(subMenu, entry, widget);
                    // Map
                    button.setMenu(subMenu);
                    // And add
                    menu.add(button);

                } else {

                    // A delimiter?
                    if (entry.label === '-') {

                        // Add
                        menu.addSeparator();

                    } else {

                        // Flag
                        var ok = false;

                        // Sub menu?
                        if (entry.items) {

                            // Make button
                            var button = new qx.ui.menu.Button(entry.label, nx.util.getIcon(entry.icon));
                            // Sub-menu
                            var subMenu = new c._menu();
                            // Link in
                            self.createMenu(subMenu, entry.items, widget);
                            // Map
                            button.setMenu(subMenu);
                            // And add
                            menu.add(button);

                        } else if (entry.tool || !entry.ds) {

                            // Make a button
                            self.createMenuButton(menu, entry, widget);

                        } else if (entry.ds) {

                            //// Get privileges
                            //var ops = entry.fns;

                            // Start saying nothing added
                            var added;

                            // Can we view?
                            if (nx.desktop.user.opAllowed(entry.ds, 'v')) {
                                added = {
                                    label: 'View',
                                    fn: 'v',
                                    icon: 'application_edit',
                                    ds: entry.ds,
                                    view: entry.view
                                };
                            } else if (nx.desktop.user.opAllowed(entry.ds, 'a')) {
                                added = {
                                    label: 'Add',
                                    fn: 'a',
                                    icon: 'application_add',
                                    ds: entry.ds,
                                    view: entry.view
                                };
                            }

                            // Tools added?
                            if (!added) {

                                if (entry.tools && entry.tools.length) {

                                    // Make button
                                    var button = new qx.ui.menu.Button(entry.label, nx.util.getIcon(entry.icon));
                                    nx.util.setWidget(button, widget);
                                    // Sub-menu
                                    var subMenu = new c._menu();

                                    // Add any tools
                                    if (entry.tools && entry.tools.length) {
                                        // Add separator
                                        subMenu.addSeparator();
                                        // Loop
                                        entry.tools.forEach(function (tool) {
                                            // Sub menu?
                                            if (tool.items) {
                                                // Add the child button
                                                var fnbutton = new qx.ui.menu.Button(tool.label, nx.util.getIcon(tool.icon));
                                                // Sub-menu
                                                var subMenuX = new c._menu();
                                                // Link in
                                                self.createMenu(subMenuX, tool.items, widget);
                                                // Map
                                                fnbutton.setMenu(subMenuX);
                                                // And add
                                                subMenu.add(button);
                                            } else {
                                                // Must have a label
                                                if (nx.util.hasValue(tool.label)) {
                                                    // Add the button
                                                    self.createMenuButton(subMenu, {
                                                        label: tool.label,
                                                        icon: tool.icon,
                                                        click: function (e) {
                                                        }
                                                    }, widget);
                                                }
                                            }
                                        });
                                    }

                                    // Map
                                    button.setMenu(subMenu);

                                    // And add
                                    menu.add(button)
                                }
                            } else {
                                // Add the child button
                                var fnbutton = new qx.ui.menu.Button(entry.label, nx.util.getIcon(entry.icon));
                                // Save
                                nx.bucket.setParams(fnbutton, added);
                                // The click
                                nx.setup.listeners(fnbutton, {
                                    listeners: {
                                        click: function (e) {
                                            // Get the button
                                            var tbutton = e.getTarget();
                                            // Get the params
                                            var params = nx.bucket.getParams(tbutton);
                                            // Any?
                                            if (params) {
                                                var tool;
                                                switch (params.fn) {
                                                    case 'a':
                                                        tool = 'Object';
                                                        break;
                                                    case 'v':
                                                        tool = 'View';
                                                        break;
                                                }
                                                // Call tool
                                                if (tool) self.runTool(tool, params);
                                            }
                                        }
                                    }
                                });

                                // And add
                                menu.add(fnbutton)
                            };

                        }
                    }

                }
            }, menu);
        }
    },

    /**
     * 
     * Creates an entry in the menu
     * 
     * @param {Menu} menu
     * @param {object} def
     */
    createMenuButton: function (menu, def, widget) {

        var self = this;

        // Make button
        var button = new qx.ui.menu.Button(def.label, self.getIcon(def.icon));

        // Set the info
        nx.bucket.setParams(button, def);
        nx.bucket.setWidget(button, widget);
        // Setup
        if (def.click) {
            nx.setup.__component(button, {

                listeners: {

                    click: def.click
                }
            });
        } else {
            nx.setup.__component(button, {

                listeners: {

                    click: function (e) {

                        // Get the button
                        var widget = self.eventGetWidget(e);
                        // Any?
                        if (widget) {
                            // Get the value
                            var def = nx.bucket.getParams(widget);
                            // Any?
                            if (def) {

                                // Call tool
                                self.runTool(def.tool || def.label, def);

                            }
                        }
                    }
                }

            });
        }
        // Children?
        nx.setup.__if(button, def, 'items', function (btn, sett) {

            // Create
            var menu = new c._menu();

            // Make sub-menu
            self.createMenu(menu, sett, widget);

            // Link
            btn.setMenu(menu);

        });
        // Add to menu
        menu.add(button);

        return button;
    },

    getIcon: function (icon) {

        // Do we have an icon?
        if (icon) {
            // A folder?
            if (icon.indexOf('/') === -1) icon = 'icons/' + icon;
            // An extension
            if (icon.indexOf('.') === -1) icon += '.png';
        }

        return icon;
    },

    getChildOfClass: function (widget, cname) {

        var self = this;

        // Assume none
        var ans;

        // Loop thru
        widget.getChildren().some(function (child) {
            // Same class?
            if (child.classname === cname) {
                ans = child;
            }
            return child;
        });

        return ans;
    },

    addFields: function (self, sett, form, req, win, cb) {

        var ans = {
            height: 0,
            width: 0
        };

        // Assure array
        if (!Array.isArray(sett)) sett = [sett];

        // Get bounds
        var barea = {
            top: 0,
            left: 0,
            height: 0,
            width: 0
        };

        // Loop thru
        sett.forEach(function (raw, rawindex) {

            // Copy
            var entry = Object.assign({}, raw);

            // Defaults
            entry.nxtype = entry.nxtype || 'string';
            entry.inview = req.inview;
            //entry.height = entry.height || 1;
            //entry.width = entry.width || 10;

            // 
            entry.width = nx.util.toInt(nx.default.get(entry.width));
            if (entry.width < 1) entry.width = 1;

            entry.height = nx.util.toInt(nx.default.get(entry.height));
            if (entry.height < 1) entry.height = 1;

            var t, l, h, w;

            // Process top
            if (!entry.top) {
                t = barea.top + barea.height;
            } else if (typeof entry.top === 'string') {
                // Offset?
                if (entry.top.indexOf('+') !== -1) {
                    t = barea.top + barea.height + nx.util.toNumber(entry.top);
                } else {
                    t = nx.util.toNumber(entry.top);
                }
            } else {
                t = entry.top;
            }

            // Process left
            if (!entry.left) {
                l = barea.left + barea.width;
            } else if (typeof entry.left === 'string') {
                // Offset?
                if (entry.left.indexOf('+') !== -1) {
                    l = barea.left + barea.width + nx.util.toNumber(entry.left);
                } else {
                    l = nx.util.toNumber(entry.left);
                }
            } else {
                l = entry.left;
            }
            // Assure
            l = l || 1;

            // Handle height
            if (typeof entry.height === 'string') {
                h = nx.util.toNumber(entry.height);
            } else {
                h = entry.height;
            }
            // Assure
            h = h || 1;

            // Handle width
            if (typeof entry.width === 'string') {
                w = nx.util.toNumber(entry.width);
            } else {
                w = entry.width;
            }

            // Save to last
            barea = {
                top: t,
                left: l,
                height: h,
                width: w
            };

            // Adjust limits
            var swidth = (w + l + l) * nx.setup.colWidth;
            if (swidth > ans.width) ans.width = swidth;
            var sheight = (h + t + t) * nx.setup.rowHeight;
            if (sheight > ans.height) ans.height = sheight;

            // Loaded?
            if (!c[entry.nxtype]) {
                // Assume string
                entry.nxtype = 'string';
            }

            // Assure
            nx.office.load('c.' + entry.nxtype, function () {

                // Make passed
                var nxparams = nx.util.merge(entry, {
                    label: entry.label || '',
                    row: t,
                    column: l,
                    rowSpan: h,
                    colSpan: w,
                    aoFld: entry.aoFld || entry.label,
                    cb: {},
                    adjustRowSpan: nx.default.get(entry.adjustHeight),
                    adjustColSpan: nx.default.get(entry.adjustWidth)
                });

                // Setup the callbacks
                Object.keys(req).forEach(function (key) {
                    if (nx.util.endsWith(key, 'CB')) {
                        nxparams.cb[key.substr(0, key.length - 2)] = req[key];
                    }
                });
                Object.keys(entry).forEach(function (key) {
                    if (nx.util.endsWith(key, 'CB')) {
                        nxparams.cb[key.substr(0, key.length - 2)] = entry[key];
                        delete entry[key];
                    }
                });

                // Make
                var compgen = c[entry.nxtype];
                var dyn = compgen.makeSelf(entry, req, self);
                // Pass options
                nxparams.formatters = nx.bucket.getFormatters(dyn);
                nx.bucket.setFormatters(dyn);
                nxparams.tools = nx.bucket.getTools(dyn);
                nx.bucket.setTools(dyn);
                // Any?
                if (nx.bucket.getWidgets(dyn)) {
                    // Loop thru
                    nx.bucket.getWidgets(dyn).forEach(function (widget) {
                        // Save
                        nx.bucket.setParams(widget, nxparams);
                        nx.bucket.setContainer(widget, dyn);
                        nx.bucket.setForm(widget, self);
                        nx.bucket.setWin(widget, win);
                        //
                        form.add(widget);
                        // Get name
                        var fieldName = nxparams.aoFld || nxparams.label;
                        // Setup
                        nx.setup.__component(widget, entry);
                        // Link
                        if (fieldName) {
                            nx.bucket.getFields(win)[fieldName] = widget;

                            // Set the value if there is an object
                            if (win._aoobject) {
                                var value = nxparams.value;
                                if (typeof value === 'undefined') {
                                    value = win._aoobject.values[fieldName];
                                }
                                if (typeof value !== 'undefined') {
                                    nx.util.processFormatters(widget, value, 0, null, true);
                                }
                            }
                        }
                    });
                } else {
                    // Setup
                    nx.setup.__component(dyn, entry);
                }

                // 
                //var render = win.getRenderer();
                //if (render) {
                //    render.addItems([dyn]);
                //}

                if (cb && rawindex >= (sett.length - 1)) {
                    cb();
                }

            });

        });

        return ans;
    },

    reportableFields: function (ds, value, cb, nodates) {

        var self = this;

        if (value && ds) {
            nx.desktop._loadDataset(ds, function (dsdef) {
                value = value.toLowerCase().replace(/[^a-z0-9\x20]/g, '');

                var wkg = '';
                var raw = nx.util.splitSpace(value);
                raw.forEach(function (fld) {
                    var def = dsdef.fields[fld];
                    if (def) {
                        switch (def.nxtype) {
                            case 'date':
                            case 'time':
                            case 'datetime':
                                if (!nodates) {
                                    wkg += fld + ' ';
                                }
                                break;

                            case 'link':
                            case 'tabs':
                            case 'grid':
                            case 'button':
                            case 'image':
                            case 'label':
                            case 'password':
                            case 'protected':
                            case 'signature':
                            case 'upload':
                                break;

                            default:
                                wkg += fld + ' ';
                                break;
                        }
                    }
                });
                cb(wkg.trim());
            });
        } else {
            cb(value);
        }
    },

    /**
     * 
     * Calls the formatters in order
     * 
     * @param {widget} widget
     * @param {number} at
     */
    processFormatters: function (widget, value, at, cb, onlyatblur) {

        var self = this;

        // Get list
        var list = nx.bucket.getParams(widget).formatters;

        // Done?
        if (list && at < list.length) {
            // If no callback, skip only at blur
            if (onlyatblur && list[at].onlyAtBlur) {
                // Call next
                self.processFormatters(widget, value, at + 1, cb, onlyatblur);
            } else {
                // Do next
                list[at].format(widget, value, function (fmt) {
                    if (fmt && typeof fmt === 'number') {
                        fmt = fmt.toString();
                    }
                    // Call next
                    self.processFormatters(widget, fmt, at + 1, cb, onlyatblur);
                }, onlyatblur);
            }
        } else {
            // Save
            widget.setValue(value);
            // Do the callback
            if (cb) cb(value);
        }
    },

    processPreSelf: function (compgen, entry, cb) {
        if (compgen && compgen.preSelf) {
            var xentry = nx.util.merge({}, entry);
            compgen.preSelf(xentry, cb);
        } else {
            cb(entry);
        }
    },

    // ---------------------------------------------------------
    //
    // PICK
    // 
    // ---------------------------------------------------------

    createPickToolbar: function (ds, cb) {
        //
        var ans = null;

        // Get pick list
        var pick = nx.desktop._getPick(ds);
        // And key the list
        var picklist = Object.keys(pick);
        // Any?
        if (picklist.length) {
            // Make the toolbar
            var tb = [];
            //
            tb.push('>');
            // Loop thru
            picklist.forEach(function (pkey) {
                // Get the definition
                var def = pick[pkey];
                // Must have values
                if (nx.util.hasValue(def.label)) {
                    // Add
                    tb.push({
                        label: def.label,
                        icon: def.icon,
                        toggle: true,
                        passed: def,
                        click: function (e) {
                            cb(e);
                        }
                    });
                }
            });
            //
            tb.push('>');
            // Add
            if (tb.length > 2) {
                ans = {
                    items: tb
                };
            }
        }

        return ans;
    },

    processPickToolbar: function (tb, qry) {

        var self = this;

        var ans = qry || [];

        // Clear
        ans.splice(0, ans.length);

        if (tb) {

            var fp = tb._fp;
            tb._fp = true;

            // Loop thru
            tb.getChildren().forEach(function (button) {
                // Get passed values
                var passed = nx.bucket.getPassed(button);
                if (passed && !fp) {
                    if (passed.selected === 'y') button.setValue(true);
                }
                // Can it be checked?
                if (button.getValue && button.getValue()) {
                    // Get
                    var pq = self.processPickToolbarItem(passed);
                    if (pq) {
                        ans.push(pq);
                    }

                }
            });
        }

        return ans;
    },

    processPickToolbarItem: function (passed) {

        var self = this;

        var ans;
        var queries = [];
        var i = 1;

        // Loop thru
        while (nx.util.hasValue(passed['field' + i])) {
            queries.push({
                field: passed['field' + i],
                op: passed['op' + i],
                value: passed['value' + i]
            });
            i++;
        }

        // Any?
        if (queries.length) {
            // All or any?
            ans = {
                sop: passed.AllAny,
                queries: queries
            };
        }

        return ans;

    },

    // ---------------------------------------------------------
    //
    // CONTEXT MENUS
    // 
    // ---------------------------------------------------------

    setContextMenu: function (widget, menu) {
        // Set
        if (widget && widget.setContextMenu) {
            widget.setContextMenu(menu);
        }
    },

    makeWindowContextMenu: function (win, insysmode) {

        var self = this;

        // If not a valid window, no menu
        if (!nx.desktop.isWindowDS(win)) {
            return;
        }

        // Create
        var menu = new c._menu();
        var defs = [];

        //
        if (insysmode) {

            defs.push({
                label: 'Caption',
                icon: 'bell',
                click: function (e) {
                    var form = nx.util.eventGetWidget(e);
                    var params = nx.bucket.getParams(form);
                    var ds = params.ds;
                    var view = params.view;

                    // Get the view
                    nx.desktop._loadView(ds, view, function (viewdef) {
                        // Get the caption
                        nx.util.runTool('Input', {
                            label: 'Caption',
                            value: viewdef.caption,
                            atOk: function (value) {
                                viewdef.caption = value;
                            }
                        });
                    });

                    var a = 1;
                }
            });

        } else {

        }

        if (defs.length) {

            self.createMenu(menu, defs, win);
            nx.util.setContextMenu(win, menu);

        }

    },

    makeFieldContextMenu: function (widget, insysmode) {

        var self = this;

        // Create
        var menu = new c._menu();
        var defs = [];

        //
        if (insysmode) {

            //
            defs.push({
                label: 'Field Properties',
                icon: 'wrench',
                click: function (e) {
                    // Get the button
                    var widget = self.eventGetWidget(e);
                    if (widget) {
                        // Call
                        self.runTool('PropertiesField', {
                            widget: widget,
                            caller: nx.bucket.getForm(widget, true)
                        });
                    }
                }
            });

        } else {

        }

        if (defs.length) {

            self.createMenu(menu, defs, widget);
            nx.util.setContextMenu(widget, menu);

        }

    },

    makeLabelContextMenu: function (widget, label, prefix) {

        var self = this;

        // If not a valid window, no menu
        if (!nx.desktop.isWindowDS(widget)) {
            return;
        }

        // Create
        var menu = new c._menu();
        // And the definitions
        var defs = [];

        // Is it a normal data entry window?
        if (nx.desktop.isWindowDS(widget, true)) {

        } else {

            defs.push({
                label: 'Copy field def',
                icon: 'copy',
                click: function (e) {
                    var widget = nx.util.eventGetWidget(e);
                    var params = nx.bucket.getParams(widget);
                    clipboard.copy('[' + prefix + params.aoFld + ']');
                }
            });
        }

        self.createMenu(menu, defs, widget);
        nx.util.setContextMenu(label, menu);

    },

    getFieldsContextMenu: function (win) {

        var self = this;

        // Create
        var menu = new c._menu();

        // Get params
        var params = nx.bucket.getParams(nx.bucket.getForm(win));

        // Make alpha list
        var alpha = [];
        // Sort

        // Sort
        params._dsfields.sort();
        // Loop thru
        params._dsfields.forEach(function (field) {
            // Get the first character
            var letter = field.substr(0, 1);
            // Add if new
            if (alpha.indexOf(letter) === -1) {
                alpha.push(letter);
            }
        });
        // Sort
        alpha.sort();
        // Loop thru letters
        alpha.forEach(function (letter) {
            // Make button
            var holder = new qx.ui.menu.Button(letter);
            // Make sub menu
            var submenu = new c._menu();

            // Loop thru fields
            params._dsfields.forEach(function (field) {
                // Matches first letter?
                if (letter === field.substr(0, 1)) {
                    var button = self.createWindowMenuButton(field, null, win, function (e) {
                        // Get the field name
                        var fieldname = e.getTarget().getLabel();
                        // Get the button
                        var widget = nx.util.eventGetWidget(e);
                        // Get the form
                        var form = nx.bucket.getForm(widget);
                        // And the window
                        var win = nx.bucket.getWin(widget);
                        //
                        var ds = nx.util.toDatasetName(nx.bucket.getDataset(form)._ds);
                        var view = nx.util.toViewName(nx.bucket.getView(form)._id);

                        // Get the dataset definition
                        nx.desktop._loadDataset(ds, function (dsdef) {
                            // And the view
                            nx.desktop._loadView(ds, view, function (viewdef) {
                                // Show the field definition
                                win.showFieldDefinition(fieldname, ds, dsdef, view, viewdef);
                            });
                        });
                    });
                    submenu.add(button);
                }
            });

            // Fill
            holder.setMenu(submenu);
            // Add holder
            menu.add(holder);
        });
        //
        var button = self.createWindowMenuButton('Add Fields', 'application_add', win, function (e) {

            // Get the button
            var win = nx.bucket.getForm(e.getTarget());

            // Get list
            nx.util.runTool('Input', {
                label: 'Field names',
                atOk: function (list) {
                    // Get params
                    var params = nx.bucket.getParams(nx.bucket.getForm(win));
                    //
                    var ds = nx.util.toDatasetName(nx.bucket.getView(params)._ds);
                    var view = nx.util.toViewName(nx.bucket.getView(params)._id);
                    // Parse
                    list = nx.util.splitSpace(list);
                    // Add to dataset
                    nx.desktop._addDatasetFields(ds, list);
                    // And to view
                    var fdefs = nx.desktop._addViewFields(ds, view, list);
                    // Redraw window
                    win.addFields(fdefs, function () {
                        // Get the dataset definition
                        nx.desktop._loadDataset(ds, function (dsdef) {
                            // And the view
                            nx.desktop._loadView(ds, view, function (viewdef) {
                                // Show the field definition
                                win.showFieldDefinition(list, ds, dsdef, view, viewdef);
                            });
                        });
                    });
                }
            });

        });
        menu.add(button);

        return menu;
    },

    getPickListContextMenu: function (win, ds, list) {

        var self = this;

        // Create
        var menu = new c._menu();

        // Sort
        list.sort();
        // Loop thru
        list.forEach(function (field) {

            var button = self.createWindowMenuButton(field, null, win, function (e) {

                // Call
                self.runTool('PropertiesPick', {
                    ds: ds,
                    pick: nx.util.eventGetLabel(e),
                    caller: win
                });

            });
            menu.add(button);
        });

        //
        var button = self.createWindowMenuButton('Add Pick', 'magnifier_zoom_in', win, function (e) {

            // Get list
            nx.util.runTool('Input', {
                label: 'Pick names',
                atOk: function (list) {

                    // Call
                    nx.util.serviceCall('AO.PickListAddList', {
                        ds: nx.util.toDatasetName(ds),
                        list: list
                    });
                }
            });

        });
        menu.add(button);

        return menu;
    },

    makeToolsContextMenu: function (widget, tools) {

        var self = this;

        //
        var defs = [];

        // Create holder
        var menu = new c._menu();
        // Loop thru
        tools.forEach(function (def) {
            // Make the button
            var button = new qx.ui.menu.Button(def.caption);
            // Add it
            menu.add(button);
            // Link
            nx.bucket.setWidget(button, widget);
            // Setup
            def.setup(widget, button);
            // Configure
            nx.setup.__component(button, {
                icon: def.icon || '',
                listeners: {
                    click: function (e) {
                        def.click(nx.util.eventGetWidget(e));
                    }
                }
            });
        });
        // Set
        nx.util.setContextMenu(widget, menu);

    },

    createWindowMenuButton: function (caption, icon, win, cb) {

        var self = this;

        // Add propeties
        var button = new qx.ui.menu.Button(caption, nx.util.getIcon(icon));
        // Link
        nx.bucket.setForm(button, win)
        // Callback
        nx.setup.__component(button, {

            listeners: {

                click: cb
            }
        });
        return button;
    },

    createContextMenuButton: function (caption, widget, label, cb) {
        // Add propeties
        var button = new qx.ui.menu.Button(caption);
        // Link
        nx.bucket.setWidget(button, widget);
        nx.bucket.setLabel(button, label);
        // Callback
        nx.setup.__component(button, {

            listeners: {

                click: cb
            }
        });
        return button;
    },

    // ---------------------------------------------------------
    //
    // RUN
    // 
    // ---------------------------------------------------------

    runTool: function (tool, req) {

        // Save
        var name = tool;
        // Load
        nx.office.load('tools.' + name, function () {
            // Call
            return tools[name].do(req);
        });

    },

    // ---------------------------------------------------------
    //
    // VALUES
    // 
    // ---------------------------------------------------------

    /**
     * 
     * Gets the value from a widget
     * 
     * @param {any} widget
     */
    getValue: function (widget) {

        // Assume none
        var ans = null;
        // See if list
        if (widget.getSelection) {
            // Assume none
            ans = [];
            // Loop thru
            widget.getSelection().forEach(function (entry) {
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
        } else if (!ans && widget.getValue) {
            // Get
            ans = widget.getValue();
        }

        return ans;

    },

    /**
     * 
     * Sets a value for a widget
     * 
     * @param {any} widget
     * @param {any} value
     */
    setValue: function (widget, value) {

        // See if list
        if (widget.setSelection) {
            // Assure array
            if (!Array.isArray(value)) value = [value];
            // Create list
            var list = [];
            widget.getChildren().forEach(function (entry) {
                // Get the label
                var label = entry.getLabel();
                // In list?
                if (value.indexOf(label) !== -1) {
                    // Add
                    list.push(entry);
                }
            });
            // Set
            widget.setSelection(list);
        } else {
            // Does it have an interface?
            if (widget.setValue) {
                // Get
                widget.setValue(value);
            }
        }

    },

    /**
     * 
     * Copies widget to clipboard
     * 
     * @param {any} wdget
     */
    copy: function (widget) {

        var textArea = document.createElement("textarea");
        if (typeof widget === 'string') {
            textArea.value = widget;
        } else {
            textArea.value = widget.getValue();
        }

        // Avoid scrolling to bottom
        textArea.style.top = "0";
        textArea.style.left = "0";
        textArea.style.position = "fixed";

        document.body.appendChild(textArea);
        textArea.focus();
        textArea.select();

        try {
            var successful = document.execCommand('copy');
            var msg = successful ? 'successful' : 'unsuccessful';
            console.log('Fallback: Copying text command was ' + msg);
        } catch (err) {
            console.error('Fallback: Oops, unable to copy', err);
        }

        document.body.removeChild(textArea);
    },

    /**
     * Deep merge to objects
     * 
     * @param {object} obj1
     * @param {object} obj2
     */
    merge: function (obj1, obj2) {

        var self = this;

        // Make result
        var ans = new Object();
        // Assure input
        obj1 = obj1 || {};
        obj2 = obj2 || {};
        // List of to be done later
        var tbd = Object.keys(obj2);
        // Loop thru obj1
        Object.keys(obj1).forEach(function (key) {
            // Is it in obj2?
            if (!tbd.includes(key)) {
                // No simple copy
                ans[key] = obj1[key];
            } else {
                // Remove from tbd later
                tbd.splice(tbd.indexOf(key), 1);
                // Is it an object?
                if (typeof obj1[key] === 'object' && typeof obj2[key] === 'object') {
                    // Deep merge
                    ans[key] = self.merge(obj1[key], obj2[key]);
                } else {
                    // Use obj2
                    ans[key] = obj2[key];
                }
            }
        });
        // Loop thru
        tbd.forEach(function (key) {
            // Not in obj1, so obj2 is it
            ans[key] = obj2[key];
        });


        return ans;
    },

    clear: function (obj) {

        var self = this;

        Object.keys(obj).forEach(function (key) { delete obj[key]; });
    },

    // ---------------------------------------------------------
    //
    // RPC
    // 
    // ---------------------------------------------------------

    _token: null,

    loopbackURL: function () {
        return window.location.protocol + '//' + window.location.host;
    },

    makeQRCode: function (data, size) {

        var self = this;

        // Assure
        data = data || {};
        data.url = self.loopbackURL();
        if (nx.desktop.user.SIO) {
            data.uuid = nx.desktop.user.SIO.uuid;
        }

        // Make
        var qr = new QRious({
            size: size || 150,
            value: data
        });

        return qr.toDataURL();

    },

    serviceCall: function (fn, data, cb, win) {

        var self = this;

        // Assure rpc
        if (!self.rpc) {

            self.selfURL = self.loopbackURL() + '/service';

            // Create the rpc
            self.rpc = new qx.io.remote.Rpc(self.selfURL);
            self.rpc.setTimeout(15000);
        }

        //
        self.rpc.setUrl(self.selfURL + '/' + fn);

        // Add token to data
        data._token = self._token;
        data._user = nx.desktop.user.getName();
        if (nx.desktop.user.SIO) {
            data._uuid = nx.desktop.user.SIO.uuid;
        }
        if (win) {
            var winid;
            if (typeof win === 'object') {
                winid = win._nxid;
                if (!winid) {
                    var params = nx.bucket.getParams(win);
                    if (params) {
                        winid = params.nxid;
                    }
                }
            }
            if (typeof winid === 'string') {
                data._winid = winid;
            }
        }

        // And call
        self.rpc.callAsync(function (result, ex, id) {
            // Protect
            try {
                // An exception?
                if (ex == null) {
                    // Save token
                    self._token = result._token;
                    // No, clean data
                    if (cb) cb(result);
                } else {
                    // Tell user
                    if (cb) cb(null);
                }
            } catch (e) { }
        }, fn, data);

    },

    // ---------------------------------------------------------
    //
    // SUPPORT
    // 
    // ---------------------------------------------------------

    ifEmpty: function (value, defaultvalue) {
        return (!value) ? (defaultvalue || '') : value;
    },

    hasValue: function (value) {
        return value && !!value.length;
    },

    startsWith: function (value, prefix) {
        var ans = false;
        if (value && prefix && value.length >= prefix.length) {
            ans = prefix === value.substr(0, prefix.length);
        }
        return ans;
    },

    endsWith: function (value, suffix) {
        var ans = false;
        if (value && suffix && value.length >= suffix.length) {
            ans = suffix === value.substr(value.length - suffix.length, suffix.length);
        }
        return ans;
    },

    uuidCounter: 0,

    localUUID: function (prefix) {

        var self = this;

        return (prefix || '') + self.uuidCounter++;

    },

    // https://stackoverflow.com/questions/105034/how-to-create-guid-uuid#:~:text=UUIDs%20%28Universally%20Unique%20IDentifier%29%2C%20also%20known%20as%20GUIDs,%40broofa%27s%20answer%2C%20below%29%20there%20are%20several%20common%20pitfalls%3A
    // Public Domain/MIT
    uuid: function () {
        //Timestamp
        var d = new Date().getTime();
        //Time in microseconds since page-load or 0 if unsupported
        var d2 = (performance && performance.now && (performance.now() * 1000)) || 0;
        return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
            //random number between 0 and 16
            var r = Math.random() * 16;
            if (d > 0) {
                //Use timestamp until depleted
                r = (d + r) % 16 | 0;
                d = Math.floor(d / 16);
            } else {
                //Use microseconds since page-load if supported
                r = (d2 + r) % 16 | 0;
                d2 = Math.floor(d2 / 16);
            }
            return (c === 'x' ? r : (r & 0x3 | 0x8)).toString(16);
        });
    },

    isSameValue: function (v1, v2) {
        // Assume not
        var ans = false;
        // Arrays?
        if ((v1 && Array.isArray(v1)) || (v2 && Array.isArray(v2))) {
            // Adjust
            if (!v1) v1 = [];
            if (!Array.isArray(v1)) v1 = [v1];
            if (!v2) v2 = [];
            if (!Array.isArray(v2)) v1 = [v2];
            ans = v1.length === v2.length && v1.every((val, index) => val === v2[index]);
        } else {
            // Adjust
            v1 = v1 || '';
            v2 = v2 || '';
            ans = v1 === v2;
        }

        return ans;
    },

    joinSpace: function (values) {

        var self = this;

        var ans = '';
        values.forEach(function (value) {
            if (value.indexOf(' ') == -1) {
                ans += value + ' ';
            } else if (value.indexOf('\'') == -1) {
                ans += '\'' + value + '\' ';
            } else {
                ans += '"' + value + '" ';
            }
        });
        return ans.trim();
    },

    splitSpace: function (value, remdelim) {

        var self = this;

        var ans = [];
        if (!value || typeof value === 'object') value = '';
        var list = value.match(/[^\s\x23\x5B\x22\x27\x5D\x28\x29]+|\x23([^\x23]*)\x23|\x5B([^\x5D\x5B]*)\x5D|\x22([^\x22]*)\x22|\x27([^\x27]*)\x27|\x28([^\x28\x29]*)\x29/gi);
        if (list) {
            list.forEach(function (entry) {
                if (entry != '') {
                    if (remdelim) {
                        if (self.startsWith(entry, "'") && self.endsWith(entry, "'")) {
                            entry = entry.substr(1, entry.length - 2);
                        } else if (self.startsWith(entry, '"') && self.endsWith(entry, '"')) {
                            entry = entry.substr(1, entry.length - 2);
                        }
                    }

                    ans.push(entry);
                }
            });
        }
        return ans;
    },

    alphaNum: function (value) {
        return (value || '').replace(/[^a-z0-9]/gi, '');
    },

    elapsedTime: function (secs, extra) {

        var self = this;

        if (secs instanceof Date) secs = (new Date().getTime() - secs.getTime()) / 1000;
        if (typeof secs === 'string') {
            secs = parseFloat(secs);
        }
        // Add the extra
        if (extra) secs += extra;
        // Must have positive value
        if (secs < 0) secs = 0;

        // remove mili
        secs = Math.floor(secs);
        // Compute days
        var days = Math.floor(secs / 86400);
        secs -= days * 86400;
        // Compute hours
        var hours = Math.floor(secs / 3600);
        secs -= hours * 3600;
        // Minutes
        var minutes = Math.floor(secs / 60);
        // Seconds
        var seconds = secs - (minutes * 60);

        var ans = '';

        if (ans.length || days > 0) ans += ':' + days;
        if (ans.length || hours > 0) ans += ':' + self.paddy(hours, 2, '0');
        if (ans.length || minutes > 0) ans += ':' + self.paddy(minutes, 2, '0');
        if (ans.length || seconds > 0) ans += ':' + self.paddy(seconds, 2, '0');

        if (ans.length) ans = ans.substr(1);

        return ans;
    },

    // From https://stackoverflow.com/questions/1267283/how-can-i-pad-a-value-with-leading-zeros
    paddy: function (num, padlen, padchar) {
        var pad_char = typeof padchar !== 'undefined' ? padchar : '0';
        var pad = new Array(1 + padlen).join(pad_char);
        return (pad + num).slice(-pad.length);
    },

    getTextMetrics: function (text, font) {

        var self = this;

        // From: https://stackoverflow.com/questions/44926614/dynamically-adjust-textfield-width-to-content
        var canvas = self.canvas || (self.canvas = document.createElement("canvas"));
        var context = canvas.getContext("2d");
        context.font = font;
        var metrics = context.measureText(text);
        return metrics;
    },

    removeSystemKeys: function (values, sort) {

        var self = this;

        var ans = [];
        // Remove system
        values.forEach(function (value) {
            if (!self.startsWith(value, '_') || value === '_user') {
                ans.push(value);
            }
        });
        if (sort) ans.sort();
        return ans;
    },

    removeValue: function (list, value) {
        var pos = list.indexOf(value);
        if (pos !== -1) {
            list.splice(pos, 1);
        }
        return list;
    },

    insertAtCursor: function (field, value) {

        // Must have a field
        if (field && field.getContentElement) {
            // Convert to DOM
            field = field.getContentElement().getDomElement();
            //IE support
            if (document.selection) {
                field.focus();
                sel = document.selection.createRange();
                sel.text = value;
            }
            // Microsoft Edge
            else if (window.navigator.userAgent.indexOf("Edge") > -1) {
                var startPos = field.selectionStart;
                var endPos = field.selectionEnd;

                field.value = field.value.substring(0, startPos) + value
                    + field.value.substring(endPos, field.value.length);

                var pos = startPos + value.length;
                field.focus();
                field.setSelectionRange(pos, pos);
            }
            //MOZILLA and others
            else if (field.selectionStart || field.selectionStart == '0') {
                var startPos = field.selectionStart;
                var endPos = field.selectionEnd;
                field.value = field.value.substring(0, startPos)
                    + value
                    + field.value.substring(endPos, field.value.length);
            } else {
                field.value += value;
            }
        }

    },

    isEMail: function (value) {

        var self = this;

        var ans = false;

        if (self.hasValue(value)) {
            ans = value.match(/^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$/);
        }

        return ans;
    },

    isPhone: function (value) {

        var self = this;

        var ans = false;

        if (self.hasValue(value)) {
            ans = value.match(/\(\d{3}\)\s*\d{3}-\d{4}/);
        }

        return ans;
    },

    // ---------------------------------------------------------
    //
    // User Agent
    // 
    // ---------------------------------------------------------

    _ua: {},

    isSecure: function () {

        var self = this;

        return window.location.protocol === 'https:'
    },

    isMobile: function () {

        var self = this;

        return self.userAgentHas('mobile');

    },

    isTablet: function () {

        var self = this;

        return self.userAgentHas('tablet');

    },

    isAndroid: function () {

        var self = this;

        return self.userAgentHas('android');

    },

    /**
     *  User agent check
     *  I know that this is dumb, but is all I need
     *  
     * @param {any} text
     */
    userAgentHas: function (text) {

        var self = this;

        var ans;

        // See if we already done
        if (typeof self._ua[text] === 'undefined') {

            // Make regex
            var re = new RegExp(text);
            // Match
            ans = re.exec(navigator.userAgent.toLowerCase()) != null;
            // Save
            self._ua[text] = ans;

        } else {

            // Get
            ans = self._ua[text];

        }

        return ans;
    },

    /**
     * 
     * Checks for camera
     * Must be calle once before any real use
     * 
     */
    hasCamera: function () {

        var self = this;

        //
        if (typeof self._ua._camera === 'undefined') {

            // Secure?
            if (!self.isSecure()) {
                // Nope
                self._ua._camera = false;
            } else {
                if (navigator.getUserMedia && self.isSecure()) {
                    navigator.getUserMedia({ video: true, audio: false }, function () {
                        self._ua._camera = true;
                    }, function () {
                        self._ua._camera = false;
                    });
                }

                //
                return false;
            }
        }

        return self._ua._camera;
    },

    // ---------------------------------------------------------
    //
    // NOTIFICATIONS
    // 
    // ---------------------------------------------------------

    confirm: function (title, msg, cb) {

        // Call
        nx.util.runTool('Confirm', {
            caption: title,
            msg: msg,
            atOk: function () {
                if (cb) cb(true);
            },
            atCancel: function () {
                if (cb) cb(false);
            }
        });

        //Notiflix.Confirm.Init({
        //    borderRadius: '5px',
        //    titleFontSize: '12px',
        //    messageFontSize: '14px',
        //    buttonsFontSize: '12px'
        //});

        //Notiflix.Confirm.Show(title || 'Please confirm', msg, 'Yes', 'No', function () {
        //    if (cb) cb(true);
        //}, function () {
        //    if (cb) cb(false);
        //});

    },

    _notify: function (msg, options) {

        var self = nx.util;

        // Assure
        options = options || {};

        // Get the callback
        var callback = options.callback;
        // Remove
        delete options.callback;

        // Make the options
        options = self.merge({
            style: 'Success',
            messageMaxLength: 300
        }, options || {});

        // Do we have a callback?
        if (callback) {
            // Make the button
            Notiflix.Notify[options.style](msg, callback, options);
        } else {
            // Make the button
            Notiflix.Notify[options.style](msg);
        }
    },

    notifyInfo: function (msg, options) {

        var self = nx.util;

        // Make the options
        options = self.merge({
            style: 'Info'
        }, options || {});

        self._notify(msg, options);
    },

    notifyError: function (msg, options) {

        var self = nx.util;

        // Make the options
        options = self.merge({
            style: 'Failure'
        }, options || {});

        self._notify(msg, options);
    },

    notifyWarning: function (msg, options) {

        var self = nx.util;

        // Make the options
        options = self.merge({
            style: 'Warning'
        }, options || {});

        self._notify(msg, options);
    },

    notifyOK: function (msg, options) {

        var self = this;

        // Make the options
        options = self.merge({
            style: 'Success'
        }, options || {});

        self._notify(msg, options);
    },

    notifyQM: function (msg, options) {

        var self = nx.util;

        // TBD - Handle att as string of JARRAY

        // Make the options
        options = self.merge({
            style: 'Info',
            callback: function () {
                if (options.from) {
                    nx.util.runTool('QM', {
                        to: options.from
                    });
                }
            },
            callbackOnTextClick: !!options.from
        }, options || {});

        self._notify(msg + (options.from ? ' // Click here to respond' : ''), options);
    },

    sendNotify: function (to, msg, type) {
        // Send
        nx.desktop.user.SIOSend('$$noti', {
            to: to,
            type: type || 'QM',
            msg: msg
        }, {
            allow: true
        });
    },

    notifyLoadingStart: function (msg) {

        var self = this;

        Notiflix.Loading.Pulse(msg);

    },

    notifyLoadingEnd: function () {

        var self = this;

        Notiflix.Loading.Remove();

    },

    // ---------------------------------------------------------
    //
    // CONVERSION
    // 
    // ---------------------------------------------------------

    numbersOnly: function (value) {

        var self = this;

        //
        value = value || '';
        return nx.util.ifEmpty(value.toString(), '0').replace(/[^0-9\.\-]/, '');
    },

    toInt: function (value, radix, defaultvalue) {

        var self = this;

        //
        var ans = value || defaultvalue || '0';
        if (typeof value !== 'number') {
            ans = parseInt(self.numbersOnly(value), radix || 10);
        }
        return ans;
    },

    toNumber: function (value) {

        var self = this;

        //
        var ans = value || '0';
        if (typeof value !== 'number') {
            ans = Number(self.numbersOnly(value));
        }
        return ans;
    },

    toBoolean: function (value) {

        var self = this;

        //
        var ans = false;
        if (value && typeof value !== 'boolean') {
            value = value.toString();
            ans = value.indexOf('y') != -1 ||
                value.indexOf('Y') != -1 ||
                value.indexOf('t') != -1 ||
                value.indexOf('T') != -1 ||
                value.indexOf('1') != -1
        }
        return ans;
    },

    toRelative: function (value, factor, offset) {

        var self = this;

        //
        value = value || '';
        var ans = value.toString();
        // If not factored, reset
        var pos = ans.indexOf('@');
        if (pos !== -1) {
            // Reset factor
            factor = parseFloat(ans.substr(pos + 1));
            // Clear flag
            ans = ans.substr(0, pos);
        }
        // Handle missing offset
        if (typeof offset === 'undefined') offset = 1;
        // Convert
        ans = (parseInt(self.numbersOnly(ans), 10) - offset) * factor;

        return ans;
    },

    fromRelative: function (value, factor, offset) {

        var self = this;

        // Handle missing offset
        if (typeof offset === 'undefined') offset = 1;
        // 
        value /= factor;
        value += offset;

        return Math.floor(value);
    },

    toDatasetName: function (value) {

        var self = this;

        //
        if (self.hasValue(value)) {
            value = value.toLowerCase().replace(/[^a-z_]/g, '');
            var issys = self.startsWith(value, '_');
            //
            value = (issys ? "_" : "") + value.toLowerCase().replace(/[^a-z]/g, '');
        }

        return value;
    },

    toViewName: function (value) {

        var self = this;

        //
        if (self.hasValue(value)) {
            //
            var pos = value.indexOf('_');
            if (pos !== -1) {
                value = value.substr(pos + 1);
            }
            //
            value = self.toDatasetName(value);
        }

        return value;
    },

    // ---------------------------------------------------------
    //
    // WIDGETS
    // 
    // ---------------------------------------------------------

    createToolbar: function (req, win, defbutton) {

        var self = this;

        // Add toolbar
        var toolbar = new qx.ui.toolbar.ToolBar();
        toolbar.setSpacing(5);
        // Setup
        nx.setup.__component(toolbar, req);

        // Do items
        nx.setup.__if(toolbar, req, 'items', function (widget, sett) {

            // Assure array
            if (!Array.isArray(sett)) sett = [sett];

            // Loop thru
            sett.forEach(function (def) {

                // Delimiter?
                if (typeof def === 'string') {

                    // Separator?
                    if (def === '>') {

                        toolbar.addSpacer();

                        // add a widget which signals that something have been removed
                        var overflow = new qx.ui.toolbar.MenuButton('More...');
                        toolbar.add(overflow);
                        toolbar.setOverflowIndicator(overflow);

                    } else if (def === 'X') {

                        // Make the button
                        var button = new qx.ui.toolbar.Button();
                        // Link to window
                        nx.bucket.setWin(button, win);
                        // Setup
                        nx.setup.__component(button, {
                            label: 'Close',
                            icon: 'cancel',
                            click: function (e) {

                                var self = nx.util.eventGetWidget(e);

                                // Map window
                                var win = nx.bucket.getWin(self);

                                // Callback
                                if (req.atCancel) req.atCancel();

                                // Close
                                win.safeClose();

                            }

                        });
                        // Handle default
                        if (defbutton === button.getLabel()) {
                            button.setIcon(self.getIcon(nx.setup.iconDefault));
                        }
                        // Add to toolbar
                        toolbar.add(button);


                    } else {

                        // Make the button
                        var button = new new qx.ui.toolbar.Button();
                        // Link to window
                        nx.bucket.setWin(button, win);
                        // Setup
                        nx.setup.__component(button, { label: def });
                        // Handle default
                        if (defbutton === def) {
                            button.setIcon(self.getIcon(nx.setup.iconDefault));
                        }
                        // Add to toolbar
                        toolbar.add(button);
                    }

                } else {

                    // Get the type
                    var type = def.toggle ? 'CheckBox' : (def.choices ? 'MenuButton' : 'Button');
                    // Make the button
                    var button = new qx.ui.toolbar[type]();
                    // Link to window
                    nx.bucket.setWin(button, win);
                    // Save passed
                    nx.bucket.setPassed(button, def.passed);
                    // Setup
                    nx.setup.__component(button, def);
                    // Handle default
                    if (defbutton === def.label) {
                        button.setIcon(self.getIcon(nx.setup.iconDefault));
                    }
                    // Add to toolbar
                    toolbar.add(button);

                }

            });

        });

        return toolbar;
    },

    getAbsoluteBounds: function (widget) {

        var self = this;

        // Default
        var ans = {
            top: 0,
            left: 0,
            width: 0,
            height: 0
        };

        // Allowed?
        if (widget && (!widget.isVisible || widget.isVisible())) {
            // Can we get basics?
            if (widget.getContentLocation) {
                var wkg = widget.getContentLocation();
                if (wkg) {
                    ans.top = ans.top || wkg.top;
                    ans.left = ans.left || wkg.left;
                    ans.width = ans.width || (wkg.right - wkg.left);
                    ans.height = ans.height || (wkg.bottom - wkg.top);
                }
            }
            if (widget.getBounds) {
                var wkg = widget.getBounds();
                if (wkg) {
                    ans.top = ans.top || wkg.top;
                    ans.left = ans.left || wkg.left;
                    ans.width = wkg.width || ans.width;
                    ans.height = wkg.height || ans.height;
                }
            }
            // Do we need width?
            if (!ans.width && widget.getWidth) ans.width = widget.getWidth();
            // Do we need height?
            if (!ans.height && widget.getHeight) ans.height = widget.getHeight();
        }

        return ans;
    },

    getRelativeBounds: function (widget) {

        var self = this;

        // Default
        var ans = {
            top: 0,
            left: 0,
            width: 0,
            height: 0
        };

        // Allowed?
        if (widget && (!widget.isVisible || widget.isVisible())) {
            // Can we get basics?
            if (widget.getContentLocation) {
                var wkg = widget.getContentLocation();
                if (wkg) {
                    ans.top = wkg.top;
                    ans.left = wkg.left;
                    ans.width = wkg.right - wkg.left;
                    ans.height = wkg.bottom - wkg.top;
                }
            }
            // Do we have a label?
            if (nx.bucket.getLabel(widget)) {
                var wkg = nx.bucket.getLabel(widget).getContentLocation();
                if (wkg) {
                    ans.left = wkg.left;
                }
            }
            // Do we have a parent?
            if (widget.$$parent) {
                var wkg = widget.$$parent.getContentLocation();
                if (wkg) {
                    ans.width = wkg.right - wkg.left;
                    ans.height = wkg.bottom - wkg.top;
                    if (widget.$$parent.$$parent) {
                        var pwkg = widget.$$parent.$$parent.getContentLocation();
                        if (pwkg) {
                            ans.top -= pwkg.top;
                            ans.left -= pwkg.left;
                        }
                    }
                }
            }
        }

        return ans;
    },

    getDistance: function (top1, left1, top2, left2) {
        //
        var dt = top1 - top2;
        var dl = left1 - left2;
        //
        return Math.sqrt((dt * dt) + (dl * dl));
    },

    // ---------------------------------------------------------
    //
    // AO
    // 
    // ---------------------------------------------------------

    localizeDesc: function (desc) {

        var self = this;

        // 
        if (desc) {
            if (typeof desc === 'object') {
                desc = desc._desc;
            }
        }
        return desc;
    },

    objectToUUID: function (obj, ds) {

        var self = this;

        return '::' + self.ifEmpty(ds || obj._ds) + ':' + self.ifEmpty(obj._id) + '::';

    },

    uuidToObject: function (value, id) {

        var self = this;

        var ans = null;

        // Do we have an id?
        if (id) {
            // Make
            ans = {
                ds: value,
                id: id
            };
        } else {
            // Split
            var list = self.ifEmpty(value).split(':');
            // Make
            ans = {
                ds: list[2],
                id: list[3]
            };
        }

        return ans;
    },

    isUUID: function (value) {

        var self = this;

        var ans = false;

        if (value) {
            //
            ans = value.match(/\x3A\x3A[^\x3A]+\x3A[^\x3A]+\x3A\x3A/);
        }

        return ans;
    },

    /**
     * 
     * Creates an ID from the dataset and id
     * 
     * @param {any} ds
     * @param {any} id
     */
    makeID: function (ds, id) {

        // Fix ds name
        ds = nx.util.toDatasetName(ds);

        return '::' + ds + ':' + id + '::';
    },

    /**
     * 
     * Parses an ID into dateset and id
     * 
     * @param {any} id
     */
    parseID: function (id) {
        if (!id) {
            return null;
        } else {
            // Split
            var pieces = id.split(':');
            // Must have siz pieces
            if (pieces.length === 6) {
                // Return
                return {
                    ds: pieces[2],
                    id: pieces[3]
                }
            } else {
                return null;
            }
        }
    },


    // ---------------------------------------------------------
    //
    // EVENTS
    // 
    // ---------------------------------------------------------

    eventGetWidget: function (e) {

        var self = this;

        // Get the target
        var target = e.getTarget();
        // Get the widget at the target
        return nx.bucket.getWidget(target) || target;
    },

    eventGetWidgetOfClass: function (e, cname) {

        var sel = this;

        // Get the starting widget
        var widget = e.getTarget();
        // And loop until found
        while (widget && widget.classname !== cname) {
            // Get parent
            widget = widget.$$parent;
        }
        return widget;
    },

    eventGetData: function (e) {

        var self = this;

        // Get the data at the target
        return e.getData();
    },

    eventGetLabel: function (e) {

        var self = this;

        var ans;

        // Get the widget at the target
        var widget = e.getTarget();
        // Does it have alabel?
        if (widget.getLabel) {
            ans = widget.getLabel();
        }
        return ans;
    },

    eventGetContainer: function (e) {

        var self = this;

        // Get the widget
        var widget = self.eventGetWidget(e);
        // Do we have a container?
        if (widget) {
            // Replace
            widget = mx.bucket.eventGetContainer(widget);
        } else {
            // Force
            widget = null;
        }

        return widget;
    },

    eventGetWindow: function (e) {

        var self = this;

        return self.eventGetWidgetOfClass(e, 'c._window');

    },

    eventGetWidgetParent: function (e) {

        var self = this;

        var widget = this.eventGetWidget(e);
        if (widget) ans = widget.$$parent;

        return ans;

    },

    eventStop: function (e) {

        var self = this;

        // Can we stop?
        if (e.stopPropagation && !e.getBubbles()) {
            e.stopPropagation();
        } else {
            // Cancel bubbly
            e.setBubbles(false);
        }

    },

    eventOnVisible: function (e, cb) {

        var self = this;

        // 
        var ele = $(e.getTarget().getFocusElement());
        // ID?
        if (!self.hasValue(ele.attr('id'))) {
            ele.attr('id', self.localUUID('cmp'));
        }
        // Get the id
        var id = ele.attr('id');

        // Start loop
        self.eventOnVisibleSub(id, cb);
    },

    eventOnVisibleSub: function (id, cb) {

        var self = this;

        // Visible?
        if (document.getElementById(id)) {
            // Call
            cb(id);
        } else {
            setTimeout(function () {
                self.eventOnVisibleSub(id, cb);
            }, 400);
        }
    },

    // ---------------------------------------------------------
    //
    // COOKIES
    // 
    // ---------------------------------------------------------

    cookieSet: function (name, value) {
        qx.module.Cookie.set(nx.desktop.user.getName() + '_' + name, value);
    },

    cookieGet: function (name) {
        return qx.module.Cookie.get(nx.desktop.user.getName() + '_' + name);
    },

    // ---------------------------------------------------------
    //
    // QR
    // 
    // ---------------------------------------------------------

    qrFill: function (widget, fld, buffer) {

        var self = this;

        // Assume failue
        var ans = buffer;
        // Get the value
        var value = widget.getValue();
        // Anything?
        if (this.hasValue(value)) {
            // Assure
            ans = ans || {};
            ans._fields = ans._fields || {};
            // Add the value
            ans[fld] = value;
            // And the field
            ans._fields[fld] = nx.bucket.getParams(widget).aoFld;
        }
        return ans;
    },

    qrAdd: function (aofld, fld, value, buffer) {

        var self = this;

        // Assume failue
        var ans = buffer;
        // Assure
        ans = ans || {};
        ans._fields = ans._fields || {};
        // And the field
        ans._fields[fld] = aofld;
        // Anything?
        if (this.hasValue(value)) {
            // Add the value
            ans[fld] = value;
        }
        return ans;
    },

    qrBuild: function (widget, result, cb) {

        var self = this;

        // Get the window
        var win = nx.bucket.getForm(widget);
        // Get the field params
        var params = nx.bucket.getParams(widget);
        // Get the dataset
        var ds = nx.bucket.getDataset(win);
        // And the data
        var data = win.getFormData();

        // Do the first dataset
        self.qrLayer(params.aoFld, ds, data, result, cb);

    },

    qrLayer: function (fld, ds, data, result, cb, done, related, entries) {

        var self = this;

        // Get the field definition
        var def = ds.fields[fld];
        // Assure        
        related = related || [];
        entries = entries || [];
        done = done || [];
        if (!related.length) {
            // Loop thru
            Object.keys(def).forEach(function (entry) {
                // Related?
                if (nx.util.startsWith(entry, 'rel')) {
                    // Does it have a value?
                    var cfld = def[entry];
                    if (nx.util.hasValue(cfld)) {
                        related.push(cfld);
                        entries.push(entry);
                    }
                }
            });
        }

        // Flag as normal
        var docb = true;

        // Loop thru
        for (var i = 0; i < related.length; i++) {
            // Working area
            var buffer = '';
            var aofld;
            // Get first
            var cfld = related[i];
            // Split
            var flds = nx.util.splitSpace(cfld);
            // Loop
            flds.forEach(function (ifld) {
                if (nx.util.hasValue(ifld)) {
                    if (nx.util.startsWith(ifld, "'")) {
                        buffer += ifld.substr(1, ifld.length - 2) + ' ';
                    } else if (nx.util.startsWith(ifld, '[')) {
                        ifld = ifld.substr(1, ifld.length - 2);
                        // Only nce
                        if (done.indexOf(ifld) === -1) {
                            // Flag
                            done.push(ifld);
                            // Use it
                            self.qrLayer(ifld, ds, data, result, null, done);
                        }
                        // Get
                        var value = data[ifld] || '';
                        aofld = ifld;
                        // Split
                        var poss = value.split(':');
                        // ID?
                        if (poss.length === 6) {
                            // Taking a turn
                            docb = false;
                            // Get object
                            nx.util.serviceCall('AO.ObjectGet', {
                                ds: poss[2],
                                id: poss[3]
                            }, function (obj) {
                                data[ifld] = obj._desc;
                                self.qrLayer(fld, ds, data, result, cb, done, related, entries);
                            });
                        } else {
                            // And save in result
                            buffer += value + ' ';
                        }
                    }
                }
            });

            //
            if (!docb) break;
            //
            self.qrAdd(aofld, entries[i], buffer.trim(), result);
        }

        if (docb && cb) cb(result);
    },

    qrMap: function (widget, result, cb) {

        var self = this;

        // Get the window
        var win = nx.bucket.getForm(widget);
        // Get the field params
        var params = nx.bucket.getParams(widget);
        // Get the dataset
        var ds = nx.bucket.getDataset(win);
        // And the data
        var data = win.getFormData();

        // Do the first dataset
        self.qrMapLayer(params.aoFld, ds, data, result, cb);

    },

    qrMapLayer: function (fld, ds, data, result, cb, done, related, entries) {

        var self = this;

        // Get the field definition
        var def = ds.fields[fld];
        // Assure        
        related = related || [];
        entries = entries || [];
        done = done || [];
        if (!related.length) {
            // Loop thru
            Object.keys(def).forEach(function (entry) {
                // Related?
                if (nx.util.startsWith(entry, 'rel')) {
                    // Does it have a value?
                    var cfld = def[entry];
                    if (nx.util.hasValue(cfld)) {
                        related.push(cfld);
                        entries.push(entry);
                    }
                }
            });
        }

        // Flag as normal
        var docb = true;

        // Loop thru
        for (var i = 0; i < related.length; i++) {
            // Get first
            var cfld = related[i];
            // And ref
            var ref = entries[i];
            // Remove extra stuff
            if (nx.util.startsWith(cfld, '[')) {
                cfld = cfld.substr(1, cfld.length - 2);
            }
            // Save
            result[ref] = data[cfld];
            // Field
            result._fields[ref] = cfld;
        }

        if (docb && cb) cb(result);
    },

    qrSet: function (win, result, fld, value) {

        var self = this;

        //
        if (result._fields[fld]) win.setValue(result._fields[fld], value);

    },

    // ---------------------------------------------------------
    //
    // Expression evaluator
    // 
    // ---------------------------------------------------------

    eval: function (expr, data, cb, at, result) {

        var self = this;

        // 
        result = result || '';
        at = at || 0;

        var docb = true;

        // Split
        if (!Array.isArray(expr)) {
            expr = nx.util.splitSpace(expr);
        }
        // Loop
        for (var i = at; at < expr.length; i++) {
            var ifld = expr[at];
            // Remove extra spaces
            if (nx.util.hasValue(ifld)) {
                if (nx.util.startsWith(ifld, "'")) {
                    result += ifld.substr(1, ifld.length - 2) + ' ';
                } else if (nx.util.startsWith(ifld, '[')) {
                    ifld = ifld.substr(1, ifld.length - 2);
                    // Get
                    var value = data[ifld] || '';
                    // Split
                    var poss = value.split(':');
                    // ID?
                    if (poss.length === 6) {
                        // Taking a turn
                        docb = false;
                        // Get object
                        nx.util.serviceCall('AO.ObjectGet', {
                            ds: poss[2],
                            id: poss[3]
                        }, function (obj) {
                            data[ifld] = obj._desc;
                            self.eval(expr, data, cb, result, at);
                        });
                    } else {
                        // And save in result
                        result += value + ' ';
                    }
                } else {
                    result += ifld + ' ';
                }
            }

            if (docb && cb) cb(result);
        }

    },

    evalJS: function (expr, data, dsdef) {

        var self = this;

        var ans;

        // Assure
        expr = self.ifEmpty(expr, '0');
        data = data || {};
        // RegEx for fields
        var re = /\x5B[a-z][a-z0-9]*\x5D/gi;
        // Is this a field?
        if (dsdef[expr]) {
            // Get the compute field
            expr = self.ifEmpty(dsdef[expr].compute, '0');
        }
        // Get all the fields
        var fields = expr.matchAll(re);
        // Only do each field once
        var done = [];
        // Loop thru
        for (var entry of fields) {
            // Get the field
            var field = entry[0];
            // Get the value
            var value = self.evalGetField(field, data, dsdef, done);
            // Replace
            expr = expr.replaceAll(field, value);
        }

        try {
            ans = eval(expr);
        } catch (e) {
            ans = 'ERROR: ' + e.message + ' - "' + expr + '"';
        }

        return ans || expr;
    },

    evalGetField: function (field, data, dsdef, done) {

        var self = this;

        // Handle if has delims
        if (self.startsWith(field, '[') && self.endsWith(field, ']')) {
            field = field.substr(1, field.length - 2);
        }

        // Check
        if (done.indexOf(field) === -1) {
            // Add to done
            done.push(field);
            // Get the value
            var value = data[field];
            // Get from dataset
            var def = null;
            if (dsdef) def = dsdef.fields[field];
            // Is it in the dataset?
            if (def && def.compute) {
                // Get the value
                value = self.evalGetField(field, data, dsdef, done);
            }
            // Save
            data[field] = value;
        }

        return data[field];
    },

    // ---------------------------------------------------------
    //
    // Geocoding
    // 
    // ---------------------------------------------------------

    positionStack: function (query, cb) {

        var self = this;

        $.ajax({
            url: window.location.protocol + '//api.positionstack.com/v1/forward',
            crossDomain: true,
            data: {
                access_key: nx.desktop.user.getSIField('psapi'),
                query: query,
                limit: 1
            }
        }).done(function (data) {
            if (data.data && Array.isArray(data.data) && data.data.length) {
                var info = data.data[0];
                if (cb) cb(info);
            }
        }).fail(function (xhr, status) {
            nx.util.notifyError('Unable to connect to PositionStack, check your settings and/or account');
        });
    },

    geocode: function (addr, city, state, zip, cb) {

        var self = this;

        var passed = (addr || '') + ' ' + (city || '') + ' ' + (state || '') + ' ' + (zip || '');

        self.positionStack(passed, function (info) {
            if (cb) cb(info.latitude, info.longitude);
        });

    },

    addressLookup: function (addr, city, state, zip, cb) {

        var self = this;

        var passed = (addr || '') + ' ' + (city || '') + ' ' + (state || '') + ' ' + (zip || '');

        self.positionStack(passed, function (info) {
            if (cb) cb(info.name, info.locality, info.region_code, info.postal_code, info.confidence);
        });

    }
};

nx.ahk = {

    /**
     * 
     * Timestamp of last command executed
     * 
     */
    _lastts: null,

    /**
     * 
     * Process comamnds
     * 
     * @param {any} text
     */
    process: function (text) {

        var self = this;

        // Ours?
        if (nx.util.startsWith(text, '@@nxproject@@')) {
            // Parse
            var cmds = JSON.parse(text.substr(13));
            // Assure array
            if (!Array.isArray(cmds)) cmds = [cmds];
            // Loop thru
            cmds.forEach(function (cmd) {
                // Same as last?
                if (self._lastts !== cmd.ts) {
                    // Save
                    self._lastts = cmd.ts;
                    // defaults
                    var create = 'n';
                    // According to command
                    switch (cmd.command) {

                        case 'SearchOrCreate':
                            create = 'y';
                        case 'Search':
                            // Must be logged in
                            if (nx.desktop.user.getName()) {
                                // Get the value
                                var value = cmd.value;
                                // Must have one
                                if (value) {
                                    // Get the search list
                                    var tbs = nx.desktop.user.getSIField('ahksearch');
                                    // Any?
                                    if (nx.util.hasValue(tbs)) {
                                        // Get the id
                                        nx.util.serviceCall('AO.ObjectGeMatch', {
                                            value: value,
                                            create: create,
                                            matches: tbs,
                                            data: cmd.data || {},
                                            force: cmd.force || 'n'
                                        }, function (result) {
                                            //
                                            if (result && result.id) {
                                                // Parse
                                                var parsed = nx.util.parseID(result.id);
                                                if (parsed != null) {
                                                    // View
                                                    nx.fs.viewObject(parsed);
                                                    // Exit loop 
                                                    i = tbs.length;
                                                }
                                            }
                                        });
                                    }
                                }
                            }
                            break;
                    }
                }
            });
        }
    }
}