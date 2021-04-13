/* ************************************************************************

   Framework7 - a dynamic web interface

   https://framework7.io/

   Copyright:
     2020-2021 Jose E. Gonzalez (nxoffice2021@gmail.com)

   License:
     MIT: https://opensource.org/licenses/MIT
     See the LICENSE file in the project's top-level directory for details.

   Authors:
     * Jose E. Gonzalez

************************************************************************ */

nx.builder = {

    // ---------------------------------------------------------
    //
    // Page
    // 
    // ---------------------------------------------------------

    /**
     * 
     * Creates a page
     * 
     * @param {any} title
     * @param {function} menucb
     * @param {any} pagefn
     * @param {any} sidefn
     * @param {function} okcb
     * @param {function} cancelcb
     * @param {function} othercb
     * */
    page: function (title, showmenu, sidecb, pagefn, okcb, cancelcb, othercb) {

        var self = this;

        // Do the menu
        var sp = null;
        if (showmenu) {
            sp = self.link('', '+logo', 'nx.office.panelLeftOpen();', title, 'leftpanel', 'icon-large');
        }
        else {
            sp = self.icon('+office', 'icon-large');
        }

        // Build header
        var header = self.navbar(title, sp, sidecb);

        // Build footer
        var footer = [];
        if (cancelcb) {
            footer.push(self.link('Back', 'highlight_off', cancelcb, title, 'cancel'));
        } else if (!othercb) {
            footer.push(self.link('', '', function () { }, title, 'cancel'));
        }
        if (othercb) {
            // Assure array
            if (!Array.isArray(othercb)) othercb = [othercb];
            // Loop thru
            othercb.forEach(function (cb, index) {
                footer.push(self.link('', '', cb, title, index.toString()));
            });
        }
        if (okcb) {
            footer.push(self.link('Ok', 'done', okcb, title, 'ok'));
        }
        footer = self.toolbar(true, footer);

        // Build
        return self.tag('div', [
            header,
            self.div('page-content', pagefn),
            footer
        ], ['class', 'page navbar-fixed', 'data-name', 'home']);

    },

    /**
     * 
     * Generic error page
     * 
     */
    oops: function () {

        var self = this;

        //
        return self.page('Oops', false, null, self.scrollable(self.contentBlock('Internal error')), null, function () {
            nx.office.goBack();
        });

    },

    // ---------------------------------------------------------
    //
    // View
    // 
    // ---------------------------------------------------------

    form: function (defs, data) {

        var self = this;

        var ans = [];

        // Must have some
        if (defs) {
            // Assure
            if (!Array.isArray(defs)) defs = [defs];

            // Sort
            defs.sort(function (a, b) {
                var ans = a.top - b.top;
                if (!ans) {
                    ans = a.col - b.col;
                }
                return ans;
            });

            // Loop thru
            defs.forEach(function (def, index) {
                // Adjust indes
                var tidx = index + 1;
                // Make
                var fld = nx.fields._generate(def, null, tidx.toString(), data);
                // Any?
                if (fld) {
                    ans.push(fld);
                }
            });
        }

        // Make
        ans = self.tag('form',
            self.tag('div',
                self.tag('ul', ans),
                ['class', 'list'])
        );

        return ans;

    },

    /**
     * 
     * Generates a dataset view
     * 
     * @param {any} ds
     * @param {any} view
     * @param {object} data
     */
    view: function (ds, view, data, cb) {

        var self = this;

        //
        self.viewSection(ds, view, [], data, false, function (rows) {

            // Android issue
            if (nx.util.isAndroid()) {
                rows.push(self.menuDivider());
            }

            // Make
            cb(self.tag('form',
                self.tag('div',
                    self.tag('ul', rows),
                    ['class', 'list'])
            ));

        });

    },

    /**
     * 
     * Generates a dataset view section
     * 
     * @param {any} ds
     * @param {any} view
     * @param {array} ans
     * @param {object} data
     */
    viewSection: function (ds, view, ans, data, showcaption, cb) {

        var self = this;

        //
        var lcb = cb;

        // Get the dataset
        nx.db._loadDataset(ds, function (dsdef) {

            // Get the view
            var vdef = nx.db.__views[ds][view];

            // Show caption?
            if (showcaption) {
                ans.push(self.menuDivider(vdef.caption));
            }

            // Sort in proper order
            var flds = self.viewFields(vdef.fields);

            // Loop thru fields
            for (var i = 0; i < flds.length; i++) {

                //
                var fdef = flds[i];
                // From dataset
                var xdef = dsdef.fields[fdef.aoFld] || {};

                // Get the type
                var type = fdef.nxtype;
                if (type === 'usedataset') {
                    type = xdef.nxtype || 'tabs';
                }
                // Put back
                fdef.nxtype = type;

                // Is it a tab?
                if (type === 'tabs') {
                    // Get views
                    var views = nx.util.splitSpace(xdef.gridview || fdef.gridview, true);
                    var ix = i;

                    // Do each
                    for (var j = 0; j < views.length; j++) {

                        //
                        var vname = views[j];
                        //
                        self.viewSection(ds, vname, ans, data, true, nx.util.noOp);
                    }
                } else {
                    // Make
                    var fld = nx.fields._generate(fdef, xdef, ans.length.toString(), data);
                    // Any?
                    if (fld) {
                        ans.push(fld);
                    }
                }

            }

            if (lcb) lcb(ans);
        });

    },

    viewFields: function (fields) {

        var self = this;

        var result = [];

        // Loop thru
        Object.keys(fields).forEach(function (entry) {
            result.push(fields[entry]);
        });

        // Sort
        result.sort(function (a, b) {
            var ans = a.top - b.top;
            if (!ans) {
                ans = a.col - b.col;
            }
            return ans;
        });

        return result;
    },

    /**
     * 
     * Generates a menu
     * 
     * @param {any} items
     */
    menu: function (items) {

        var self = this;

        // Make
        return self.tag('div',
            self.tag('ul', items),
            ['class', 'list']);
    },

    /**
     * 
     * Generates a menu block
     * 
     * @param {any} items
     */
    menuBlock: function (items) {

        var self = this;

        var ans = [];

        //
        var divflag = true;

        // Assure
        if (!Array.isArray(items)) items = [items];

        // Do
        divflag = self.menuSection(items, ans, divflag);

        // Adjust
        if (nx.util.isAndroid() && !divflag) {
            ans.push(self.menuDivider());
        }

        // Make
        return ans;
    },

    /**
     * 
     * Creates a menu section
     * 
     * @param {any} items
     * @param {any} ans
     * @param {any} divflag
     * @param {any} refid
     */
    menuSection: function (items, ans, divflag, badges) {

        var self = this;

        // Loop thru
        items.forEach(function (entry) {
            //
            divflag = self.menuItem(entry, ans, divflag, badges);
        });

        return divflag;
    },

    /**
     * 
     * Creates a menu item that can be clicked
     * 
     * @param {any} label
     * @param {any} fn
     */
    menuItemCall: function (label, icon, cb, header, footer) {

        var self = this;

        var attr = ['class', 'item-inner'];
        if (cb) {
            // Make
            var cbr = cb;
            if (typeof cbr === 'function') {
                //
                cbr = self.callback(cbr);
            }
            // Add the event
            attr.push('onclick');
            attr.push(cbr);
        }


        //
        return self.tag('li',
            self.tag('div',
                self.tag('div', [
                    (header ? self.tag('div', header, ['class', 'item-header']) : null),
                    self.link(label, icon),
                    (footer ? self.tag('div', footer, ['class', 'item-footer']) : null)
                ], ['class', 'item-title']),
                attr),
            ['class', 'item-content']);
    },

    /**
     * 
     * Creates a menu item
     * 
     * @param {any} entry
     * @param {any} ans
     * @param {any} divflag
     */
    menuItem: function (entry, ans, divflag, badges) {

        var self = this;

        if (typeof entry === 'object') {
            // Divider?
            if (entry.label === '-') {
                if (!divflag) {
                    ans.push(self.menuDivider());
                }

                divflag = true;
            } else {
                // Is it a timer?
                if (entry.timer) {
                    // Get id
                    var id = nx.util.localUUID().toString();

                    ans.push(self.tag('div',
                        self.tag('div',
                            self.tag('div', [
                                self.icon('+' + entry.icon),
                                self.tag('span', '...', ['timerid', 'ttt_' + id]),
                                self.tag('div', entry.label, ['class', 'item-footer'])
                            ],
                                ['class', 'item-title']),
                            ['class', 'item-inner timer',
                                'onclick', "nx.calls.view('" + entry.obj + "');",
                                'refid', 'ttt_' + id]),
                        ['class', 'item-content']));

                    divflag = false;
                }
                else if (entry.html) {
                    ans.push(self.tag('li', entry.html));

                    divflag = true;
                } else {
                    // is it a tool?
                    var tool = entry.tool;
                    if (!tool || nx.calls[tool.toLowerCase()]) {

                        // Sub-items?
                        if (entry.items) {

                            // Copy badges
                            var nbadges = [];
                            if (badges) {
                                nbadges = nbadges.concat(badges);
                            }
                            // Add new one
                            nbadges.push(entry.label);
                            // Do
                            divflag = self.menuSection(entry.items, ans, divflag, nbadges);

                        } else {
                            //
                            var icon = entry.icon;
                            if (icon) {
                                icon = '+' + icon;
                            }

                            //
                            var def = [];
                            if (icon) {
                                def.push(self.tag('div', self.icon(icon), ['class', 'item=media']));
                            }

                            //
                            var label = entry.label;

                            // Apply any badges
                            if (badges) {
                                // Loop thru
                                badges.forEach(function (badge) {
                                    // Add
                                    label = label += ' ' + self.badge(badge, 'blue');
                                });
                            }

                            var attrc = ['class', 'item-content'];

                            // Is it a tool?
                            if (tool) {
                                label = self.link(label, '', nx.calls[tool.toLowerCase()], 'menu', label);
                                nx.user._toolIcons[tool.toLowerCase()] = entry.icon;
                            } else if (entry.items) {
                                label = self.chip(label, 'blue');
                            } else if (entry.cb) {
                                label = self.link(label);

                                attrc.push('onclick');
                                attrc.push(entry.cb);
                            } else if (entry.obj) {
                                // Map
                                label = self.link(label);

                                attrc.push('onclick');
                                attrc.push("nx.calls.view('" + entry.obj + "')");

                            } else if (entry.childds) {
                                // Map
                                label = self.link(label);

                                attrc.push('onclick');
                                attrc.push("nx.calls.pickchild('" + entry.childds + "')");

                            } else if (entry.ds) {
                                // Map
                                label = self.link(label);

                                attrc.push('onclick');
                                attrc.push("nx.calls.pick('" + entry.ds + "')");
                            }

                            //
                            def.push(self.tag('div',
                                self.tag('div', label, ['class', 'item-title']),
                                ['class', 'item-inner']));
                            //
                            ans.push(self.tag('li',
                                def,
                                attrc));

                            //
                            divflag = false;
                        }
                    }
                }
            }
        } else {
            // Divider?
            if (entry === '--') {
                ans.push(self.menuDivider());
            } else if (entry === '-') {
                if (!divflag) {
                    ans.push(self.menuDivider());
                }

                divflag = true;
            } else if (nx.util.startsWith(entry, '-')) {
                ans.push(self.menuDivider(entry.substr(1)));

                divflag = true;
            } else {
                ans.push(self.tag('div',
                    self.tag('div',
                        self.tag('div', entry, ['class', 'item-title']),
                        ['class', 'item-inner']),
                    ['class', 'item-content']));

                divflag = false;
            }
        }

        return divflag;
    },

    /**
     * 
     * The divider
     * 
     */
    menuDivider: function (msg) {

        var self = this;

        return self.tag('li', (msg || ''), ['class', 'item-divider']);
    },

    /**
     * 
     * Generates a menu for a side panel
     * 
     * @param {any} items
     */
    menuSide: function (items) {

        var self = this;

        // 
        var menu = [];
        // Build
        var wasDivider = self.menuSection(items, menu, false);
        //
        if (nx.util.isAndroid() && !wasDivider) {
            menu.push(nx.builder.menuDivider());
        }

        return self.menu(menu);
    },

    // ---------------------------------------------------------
    //
    // Callbacks
    // 
    // ---------------------------------------------------------

    /**
     * 
     * Creates a callback link
     * 
     * @param {string} name
     * @param {function} cb
     */
    callback: function (cb) {

        var self = this;

        var ans = cb;
        if (typeof ans === 'function') {

            // Make name
            var name = nx.util.localUUID('cb_');
            // Get holder
            var bucket = nx.env.getBucket();
            // 
            var holder = bucket._cbx;
            // None?
            if (!holder) {
                // Make
                holder = {};
                // Save
                bucket._cbx = holder;
            }
            // Save
            holder[name] = cb;
            // Get the reference
            ans = 'nx.env._buckets.' + bucket._bucket + '._cbx.' + name + '(this)';
        }

        return ans;
    },

    // ---------------------------------------------------------
    //
    // Specialty
    // 
    // ---------------------------------------------------------

    /**
     * 
     * Creates a link
     * 
     * @param {any} text
     * @param {any} icon
     * @param {function} cb
     */
    link: function (text, icon, cb, page, id, iattr, xattr, drefid) {

        var self = this;

        // Handle when cb is an object
        if (cb && typeof cb === 'object') {
            text = cb.label || text;
            icon = cb.icon || icon;
            cb = cb.cb;
        }

        //
        var ref = icon;

        // Handle QX icons
        if (ref && nx.util.startsWith(ref, '+')) {
            // Adjust
            icon = ref.replace(/\x2b/g, '');
        }

        //
        var items = [];
        if (ref) items.push(self.icon(ref, iattr));
        if (text) items.push(self.tag('span', text));

        if (xattr) {
            xattr = 'link ' + xattr;
        } else {
            xattr = 'link';
        }

        var attr = ['class', xattr];
        if (cb) {
            // Make
            var cbr = cb;
            if (typeof cbr === 'function') {
                //
                cbr = self.callback(cbr);
            }
            // Add the event
            attr.push('onclick');
            attr.push(cbr);
        }

        if (drefid) {
            attr.push('refid');
            attr.push(drefid);
        }

        //
        return self.tag('span', items, attr);

    },

    /**
     * 
     * Creates an icon
     * 
     * @param {any} icon
     */
    icon: function (icon, attr) {

        var self = this;

        var ans;

        //
        if (nx.util.startsWith(icon, '+')) {
            ans = self.tag('img', '', ['class', (attr || 'icon'), 'src', '../../../icons/' + icon.replace(/\x2b/g, '') + '.png']);
        } else {
            ans = self.tag('span',
                icon,
                ['class', 'material-icons']);
        }
        return ans;
    },

    /**
     * 
     * Creates a chip
     * 
     * @param {any} text
     * @param {any} color
     * @param {any} cb
     */
    chip: function (text, color, cb) {

        var self = this;

        var attr = ['class', 'chip color-' + color];
        if (cb) {
            attr.push('onclick');
            attr.push(self.callback(cb));
        }

        //
        return self.tag('div',
            self.div('chip-label', text),
            attr);

    },

    /**
     * 
     * Creates a chip
     * 
     * @param {any} text
     * @param {any} color
     * @param {any} cb
     */
    contextMenu: function (text, color, cb) {

        var self = this;

        var attr = ['class', 'chip color-' + color + ' context-menu'];
        if (cb) {
            attr.push('onclick');
            attr.push(self.callback(cb));
        }

        //
        return self.tag('div',
            self.div('chip-label', text),
            attr);

    },

    /**
     * 
     * Creates a badge
     * 
     * @param {any} text
     * @param {any} color
     */
    badge: function (text, color) {

        var self = this;

        return self.tag('div', text, ['class', 'badge bg-color-' + color]);

    },

    /**
     * 
     * Creates a badge
     * 
     * @param {any} text
     * @param {any} color
     */
    toggle: function (text, state, cb) {

        var self = this;

        var attrs = ['type', 'checkbox'];
        if (cb) {
            attrs.push('onclick');
            attrs.push(self.callback(cb));
        }
        if (state) attrs.push('checked');

        return self.tag('label', [
            self.tag('input', '', attrs),
            self.tag('span', '', ['class', 'toggle-icon'])
        ], ['class', 'toggle']) +
            self.tag('span', '  ' + text)

    },

    // ---------------------------------------------------------
    //
    // Navigation
    // 
    // ---------------------------------------------------------

    /**
     * 
     * Creates a navigation bar
     * 
     * @param {any} title
     * @param {any} leftfn
     * @param {any} rightfn
     */
    navbar: function (title, leftfn, rightfn) {

        var self = this;

        return this.div('navbar', [
            this.div('navbar-bg', null),
            this.div('navbar-inner sliding', [
                self.div('left', leftfn),
                self.tag('div', title, ['class', 'title']),
                self.div('right', rightfn)
            ]
            )
        ]
        );

    },

    /**
     *
     * Creates a tabs bar
     *
     * @param {any} fn
     */
    tabs: function () {

        var self = this;

        //
        var fns = [];
        for (var i = 0; i < arguments.length; i++) {
            fns.push(self.tag('div', arguments[i], ['class', 'page-content tab']));
        }


        return this.tag('div', fns, ['class', 'tabs']);
    },

    /**
     * 
     * Creates a toolbar bar
     * 
     * @param {any} bottom
     * @param {any} fn
     */
    toolbar: function () {

        var self = this;

        //
        var cls = 'toolbar ' + (arguments[0] ? 'toolbar-bottom' : 'toolbar-top');

        //
        var fns = [];
        for (var i = 1; i < arguments.length; i++) {
            fns.push(arguments[i]);
        }

        return this.tag('div',
            this.div('toolbar-inner', fns)
            , ['class', cls]);

    },

    // ---------------------------------------------------------
    //
    // Content Block
    // 
    // ---------------------------------------------------------

    /**
     * 
     * Creates a content block
     * 
     * @param {any} value
     */
    contentBlock: function (value) {

        var self = this;

        return self.div('content-block', value);
    },

    /**
     * 
     * Creates an inner content block
     * 
     * @param {any} value
     */
    innerContentBlock: function (value) {

        var self = this;

        return self.div('content-block-inner', value);
    },

    /**
     * 
     * Creates a content block title
     * 
     * @param {any} value
     */
    contentBlockTitle: function (value) {

        var self = this;

        return self.div('content-block-title', value);
    },

    /**
     * 
     * A scrollable area
     * 
     * @param {any} value
     */
    scrollable: function (value) {

        var self = this;

        return self.div('page', self.div('page-content', value));
    },

    // ---------------------------------------------------------
    //
    // Grid
    // 
    // ---------------------------------------------------------

    /**
     * 
     * Creates a grid
     * 
     * @param {any} headers
     * @param {any} rows
     * @param {any} widths
     */
    grid: function (headers, rows, widths) {

        var self = this;

        // Assure
        if (!Array.isArray(headers)) headers = [headers];
        if (!Array.isArray(rows)) rows = [rows];
        // Wudths are optional and default to auto;
        widths = widths || 'auto';
        if (!Array.isArray(widths)) widths = [widths];

        // Create the widths
        widths.forEach(function (width, index) {
            // Convert to string
            width = width.toString();
            // 
            if (width.includes('^')) {
                // Center options
                width = 'col-' + width.replace('^', '') + ' grid-col-center';
            } else if (width.includes('<')) {
                // Left options
                width = 'col-' + width.replace('<', '') + ' grid-col-left';
            } else if (width.includes('>')) {
                // Right options
                width = 'col-' + width.replace('>', '') + ' grid-col-right';
            } else {
                // Default is left
                width = 'col-' + width + ' grid-col-left';
            }
            // Put back
            widths[index] = width;
        });

        // Build header row
        var fmtHeader = [];
        // Loop thru
        header.forEach(function (piece, index) {
            // Get width
            var width = widths[width.length % index];
            // Add
            fmtHeader.push(self.div(width, piece));
        });
        // Finish
        fmtHeader = self.div('row', fmtHeader);

        // Now the rows
        var fmtRows = [];
        // Loop thru
        rows.forEach(function (row) {
            // Temp
            var fmtRow = [];
            // Assure
            if (!Array.isArray(row)) row = [row];
            // Loop for each column
            row.forEach(function (piece, index) {
                // Get width
                var width = widths[width.length % index];
                // Add
                fmtRow.push(self.div(width, piece));
            });
            // Add
            fmtRows.push(self.div('row', fmtRow));
        });
        // Finish

        return self.contentBlock([fmtHeader, fmtRows]);
    },

    // ---------------------------------------------------------
    //
    // Database related
    // 
    // ---------------------------------------------------------

    /**
     * 
     * Creates a pick list
     * 
     * @param {any} ds
     * @param {any} data
     * @param {any} bucketid
     */
    picklist: function (ds, data, bucketid, onselect) {

        var self = this;

        // Assure
        if (!Array.isArray(data)) data = [data];

        //
        if (onselect) {
            nx.env.setBucketItem('onSelect', onselect);
        }

        // Holder
        var holder = [];
        // Loop thru
        data.forEach(function (row, index) {
        // Click
            var onclick = null;
            if (onselect) {
                onclick = "nx.calls.pickselect('::" + ds + ":" + row._id + "::');";
            } else {
                onclick = "nx.calls.view('::" + ds + ":" + row._id + "::');";
            }
            // Make
            holder.push(self.tag('li', [
                self.tag('div', '', ['class', 'item=media']),
                self.tag('div',
                    self.tag('div',
                        self.link(row._desc),
                        ['class', 'item-title']),
                    ['class', 'item-inner'])],
                ['class', 'item-content', 'onclick', onclick]));
        });

        // Android
        if (nx.util.isAndroid()) {
            holder.push(self.menuDivider());
        }

        return self.tag('div',
            self.tag('ul', holder),
            ['class', 'list', 'id', self.pickListID(ds, bucketid)]);
    },

    pickListID: function (ds, bucketid) {
        //
        return 'picklist_' + ds + '_' + bucketid;
    },

    /**
     * 
     * Cteates a document tree
     * 
     * @param {any} ds
     * @param {any} data
     * @param {any} bucketid
     */
    documents: function (ds, data, bucketid, onselect) {

        var self = this;

        //
        var tree = [];

        // Loop thru
        data.forEach(function (entry) {
            //
            var node = self.documentNode(entry, 0, onselect);
            if (node) {
                tree.push(node);
            }
        });

        return self.tag('div', tree, ['class', 'treeview', 'id', self.pickListID(ds, bucketid)]);

    },

    documentNode: function (entry, level, onselect) {

        var self = this;

        //
        var list = [];
        level = (level || 0) + 1;

        // 
        var icon = '+download';
        var attr = ['class'];
        var attrc = ['class', 'treeview-item-content'];
        var children, cb;

        // Handle folders
        if (entry.items) {
            attr.push('treeview-item root treeview-item-toggle treeview-toggle treeview-item-opened');
            icon = '+folder';

            // Area
            children = [];
            entry.items.forEach(function (child) {
                // Add
                children.push(self.documentNode(child, level, onselect));
            });
            // Add list
            children = self.tag('div', children, ['class', 'treeview-item-children', 'style', 'margin-left:' + (level * 22) + 'px;']);

        } else {
            attr.push('treeview-item root');
            // Get extension
            attrc.push('onclick');
            var ext = '';
            var pos = entry.name.lastIndexOf('.');
            if (pos !== -1) {
                ext = entry.name.substr(pos + 1);
            }
            // According to extension
            switch (ext.toLowerCase()) {
                case 'pdf':
                    icon = '+pdf';
                    cb = onselect || 'nx.fs.viewpdf';
                    attrc.push(cb + "('" + entry.path + "');");
                    break;
                case 'odt':
                    icon = '+docx'
                    cb = onselect || 'nx.fs.viewaspdf';
                    attrc.push(cb + "('" + entry.path + "');");
                    break;
                case 'jpeg':
                case 'jpg':
                case 'png':
                case 'gif':
                case 'svg':
                    icon = '+photo';
                    cb = onselect || 'nx.fs.viewimage';
                    attrc.push(cb + "('" + entry.path + "');");
                    break;
                case 'mp4':
                case 'webm':
                case 'avi':
                case 'mov':
                case 'mpg':
                case 'wmv':
                case 'flv':
                case 'mkv':
                case 'ogv':
                case '3gp':
                case '3g2':
                    icon = '+drive_web';
                    cb = onselect || 'nx.fs.viewvideo';
                    attrc.push(cb + "('" + entry.path + "','" + ext + "');");
                    break;
                default:
                    cb = onselect || 'nx.fs.download';
                    attrc.push(cb + "('/f/" + entry.path + "');");

            }
        }

        // Label
        list.push(self.tag('div',
            self.tag('div', [
                self.icon(icon),
                self.tag('div', entry.name, ['class', 'treeview-item-label'])
            ], attrc),
            attr));

        return self.tag('div', [
            list,
            children
        ], ['class', 'treeview-item']);
    },

    upload: function () {

        var self = this;

        return self.tag('input',
            null,
            ['id', 'upload', 'type', 'file', 'accept', 'application/pdf, audio/*, video/*, image/', 'style', 'display: none;']);
    },

    /**
     * 
     * Creates a searchbar
     * 
     */
    searchbar: function (value) {

        var self = this;

        //
        var ans = [self.div('searchbar-backdrop')];

        ans.push(
            self.tag('form',
                self.tag('div',
                    self.tag('div', [
                        self.tag('input', '', ['type', 'search', 'placeholder', 'Search', 'value', (value || '')]),
                        self.tag('i', '', ['class', 'searchbar-icon']),
                        self.tag('span', '', ['class', 'input-clear-button'])
                    ], ['class', 'searchbar-input-wrap']),
                    ['class', 'searchbar-inner']),
                ['class', 'searchbar']));

        return ans;
    },

    // ---------------------------------------------------------
    //
    // Utilities
    // 
    // ---------------------------------------------------------

    /**
     * 
     * Creates a text block
     * 
     * @param {any} value
     * @param {any} prev
     */
    do: function (value, prev) {

        var self = this;

        // Assure 
        prev = prev || '';
        value = value || '';

        // Any?
        if (value) {
            // Do we have an array?
            if (Array.isArray(value)) {
                // Flatten
                value = self.flattenArray(value);
                // Temp
                var temp = '';
                // Loop thru
                value.forEach(function (piece) {
                    // Any?
                    if (piece) {
                        // Process
                        temp += self.do(piece);
                    }
                }, self);
                // Replace
                value = temp;
            } else {
                // Is the value a function?
                if (_.isFunction(value)) {
                    // Call
                    value = value();
                }
            }
        }

        // Append
        return prev + value;
    },

    /**
     * 
     * Creates a tag with content
     * 
     * @param {any} tag
     * @param {any} value
     */
    tag: function (tag, value, attributes) {

        var self = this;

        // Assure
        if (attributes) {
            // Did we get an array?
            if (Array.isArray(attributes)) {
                // Temp
                var temp = '';
                // Loop thru
                for (var i = 0; i < attributes.length; i += 2) {
                    // Odd one?
                    if ((i + 1) >= attributes.length) {
                        // Only the element
                        temp += ' ' + self.do(attributes[i]);
                    } else {
                        // Build
                        temp += ' ' + self.do(attributes[i]) + '="' + self.do(attributes[i + 1]) + '"';
                    }
                }
                // And replace
                attributes = temp;
            } else {
                // In case we got a function
                attributes = ' ' + self.do(attributes);
            }
        } else {
            // Make it an emoty string
            attributes = '';
        }

        return '<' + tag + attributes + '>' + self.do(value) + '</' + tag + '>';
    },

    /**
     * 
     * Creates a div with a given class
     * 
     * @param {any} cls
     * @param {any} value
     */
    div: function (cls, value) {

        var self = this;

        return self.tag('div', (value || '&nbsp;'), ['class', cls]);
    },

    flattenArray: function (value) {

        var self = this;

        // Assure
        if (value) {
            // Array?
            if (Array.isArray(value)) {
                // 
                var temp = [];
                // Loop thru
                value.forEach(function (piece) {
                    // Flatten 
                    piece = self.flattenArray(piece);
                    // Any?
                    if (piece) {
                        temp = temp.concat(piece);
                    }
                });
                // Result
                value = temp;
            } else {
                value = [value];
            }
        }

        return value;
    }

};