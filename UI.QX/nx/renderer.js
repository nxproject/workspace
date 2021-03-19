qx.Class.define('nx.renderer', {

    extend: qx.ui.form.renderer.AbstractRenderer,

    construct: function construct(form) {

        var self = this;

        // 
        nx.bucket.setChildren(self, []);

        this.base(arguments, form);

        var layout = new qx.ui.layout.Basic();
        self._setLayout(layout);

    },

    members: {

        _render: function (widgets) {

            var self = this;

            //
            widgets = widgets || self._form.getChildren();
            // Add
            self.addItems(widgets);
        },

        addItems: function (items) {

            var self = this;

            // Make room
            nx.bucket.setChildren(self, nx.bucket.getChildren(self).concat(items));

            // Get the first entry
            var startWidget;

            // add the items
            while (items.length) {

                // Get the widget
                var widget = items[0];

                // Get the setings
                var wparams = nx.bucket.getParams(widget);
                // Valid?
                if (wparams && !wparams._rendered) {

                    // Only once
                    wparams._rendered = true;

                    var isAO;

                    // Get the form
                    var win = nx.bucket.getWin(widget);
                    var form = nx.bucket.getForm(win);
                    // Is it a AO window?
                    var fparams = nx.bucket.getParams(form);
                    if (typeof isAO === 'undefined') {
                        isAO = fparams.nxid && nx.util.startsWith(fparams.nxid, 'ao_');
                    }
                    var sysmode = fparams.sysmode;

                    // Setup source prefix
                    var lprefix = '';
                    // Setup
                    if (fparams.ds === '_sys' && fparams.id === '_info') {
                        lprefix = '*sys:';
                    } else if (fparams.ds === '_user') {
                        lprefix = '*user:';
                    }

                    // Spacing
                    var rowSpacing = nx.setup.rowHeight + nx.setup.rowSpacing;
                    var colSpacing = nx.setup.colWidth + nx.setup.colSpacing;

                    // Label
                    var winp = nx.bucket.getParams(form) || {};
                    var labelSize = wparams.labelWidth || winp.labelWidth || nx.setup.labelWidth;

                    // Positioning
                    var top = nx.util.toRelative(wparams.row, rowSpacing);
                    var height = nx.util.toRelative(wparams.rowSpan, nx.setup.rowHeight, 0);
                    if (wparams.adjustRowSpan) {
                        height += nx.default.get(wparams.adjustRowSpan); // nx.util.toRelative(wparams.adjustRowSpan, nx.setup.rowHeight, 0);
                    }
                    var left = nx.util.toRelative(wparams.column, colSpacing);
                    var width = nx.util.toRelative(wparams.colSpan, nx.setup.colWidth, 0);
                    if (wparams.adjustColSpan) {
                        width += nx.default.get(wparams.adjustColSpan); // nx.util.toRelative(wparams.adjustColSpan, nx.setup.colWidth, 0);
                    }
                    var labelWidth = nx.util.toRelative(labelSize, nx.setup.colWidth, 0);

                    // Compute space left
                    var spaceLeft = wparams.colSpan - (wparams.label ? labelSize : 0);
                    // Compute start
                    var at = left;
                    // Compute the width
                    var entryWidth = nx.util.toRelative(spaceLeft, nx.setup.colWidth, 0);
                    if (wparams.adjustColSpan) {
                        entryWidth += nx.default.get(wparams.adjustColSpan); // nx.util.toRelative(wparams.adjustColSpan, nx.setup.colWidth, 0);
                    }

                    // Create a container for the main layout and set the main layout
                    var ctx;
                    if (sysmode) {
                        ctx = new nx.overlay(new qx.ui.layout.Canvas());
                        // Remove RO
                        nx.setup.ro(widget, {
                            ro: 'n'
                        });
                    } else {
                        ctx = new nx.field(new qx.ui.layout.Canvas());
                    }
                    ctx.setPadding(0);
                    // Add to form
                    self._add(ctx, {
                        top: top,
                        left: at,
                        height: height,
                        width: width
                    });
                    // Link
                    nx.bucket.setWidget(ctx, widget);
                    nx.bucket.setContainer(widget, ctx);

                    // All is relative
                    top = 0;
                    at = 0;

                    // Save area
                    var label;
                    if (nx.util.hasValue(wparams.label)) {
                        // Get the label
                        var ltext = wparams.label + ': ';
                        // Checkbox is special
                        if (widget.classname.indexOf('CheckBox') !== -1) {
                            ltext = '';
                        }
                        // Make the label
                        label = new qx.ui.basic.Label(ltext);
                        label.setRich(true);
                        label.setAppearance("form-renderer-label");
                        // And context menu
                        nx.util.makeLabelContextMenu(widget, label, lprefix);
                        ctx._add(label, {
                            top: top,
                            left: at
                        });
                        label.setWidth(labelWidth);
                        label.setTextAlign('right');
                        // Link
                        nx.bucket.setWidget(label, widget);

                        // Link
                        nx.bucket.setLabel(widget, label);
                        nx.bucket.setLabel(ctx, label);
                        // Reset color
                        nx.bucket.getContainer(widget).setTagNormal();
                        // Adjust
                        at += labelWidth;
                    }

                    // Hold to first
                    if (!startWidget) startWidget = widget;

                    // Add
                    ctx._add(widget, {
                        top: top,
                        left: at
                    });

                    // Set
                    if (widget.setWidth) widget.setWidth(entryWidth);
                    if (widget.setHeight) widget.setHeight(height);

                    // Adjust
                    at += entryWidth;

                    // Can we call?
                    if (label && label.setBuddy) {
                        label.setBuddy(widget);
                    }

                    // Placeholder
                    if (wparams.placeholder) {
                        widget.setPlaceholder(wparams.placeholder);
                    }

                    // Assume none
                    var link;
                    // Add RTC link
                    if (isAO) {
                        // Make link
                        link = new nx.rtc();
                        link.init(wparams.aoFld, fparams.nxid);
                        // Save
                        nx.bucket.setProcessSIO(widget, link);
                    }

                    // Formatter
                    if (link || (wparams.formatters && wparams.formatters.length)) {
                        nx.bucket.setFormatters(wparams.formatters);
                        widget.addListener('focus', function (e) {
                            // Get widget
                            var widget = nx.util.eventGetWidget(e);
                            // Save current
                            nx.bucket.setBeforeValue(widget, widget.getValue(true));
                        });
                        widget.addListener('blur', function (e) {
                            // Get widget
                            var widget = nx.util.eventGetWidget(e);
                            // Get the value
                            var value = widget.getValue(true);
                            // Changed?
                            if (nx.bucket.getBeforeValue(widget) !== value) {
                                // Format
                                nx.util.processFormatters(widget, value, 0, function (value) {
                                    // Get the SIO
                                    var link = nx.bucket.getProcessSIO(widget);
                                    if (link) {
                                        link.SIOSend(value);
                                    }

                                    // Computed fields
                                    var win = nx.bucket.getForm(widget);
                                    var form = nx.bucket.getForm(widget);
                                    if (form) {
                                        var ds = nx.bucket.getDataset(form);
                                        if (ds && ds._computed) {
                                            // Get data
                                            var data = win.getFormData();
                                            // And result
                                            var newdata = {};
                                            // Loop thru
                                            ds._computed.forEach(function (fld) {
                                                // Get expression
                                                var expr = ds.fields[fld].compute;
                                                if (expr) {
                                                    // Compute
                                                    var value = nx.util.evalJS(expr, data, ds);
                                                    // Save
                                                    newdata[fld] = value;
                                                }
                                            });
                                            // Save
                                            win.setFormData(newdata);
                                        }
                                    }

                                });
                            }
                        });
                    }

                    // Initialize the value
                    if (wparams.value) {
                        nx.setup.value(widget, wparams);
                    }

                    // Do we have a setup callback?
                    if (wparams.cb.setup) {
                        wparams.cb.setup(widget);
                    }

                    // Do we have a keypress callback?
                    if (wparams.cb.keyup) {
                        widget.addListener('keyup', function (e) {
                            wparams.cb.keyup(nx.util.eventGetWidget(e), e._keyCode);
                        });
                    }

                    // TOOLS
                    if (sysmode) {
                        nx.util.makeFieldContextMenu(widget, sysmode);
                        if (wparams.tools && wparams.tools.length) {
                            nx.bucket.getContainer(widget).setTagTools();
                        }
                    } else if (wparams.tools && wparams.tools.length) {
                        nx.util.makeToolsContextMenu(widget, wparams.tools);
                        nx.bucket.getContainer(widget).setTagTools();
                    } else if (wparams.contextMenu && widget.setContextMenu) {
                        // Make a menu
                        var menu = new c._menu();
                        // Fill it
                        nx.util.createMenu(menu, wparams.contextMenu.items, widget);
                        // Add
                        nx.util.setContextMenu(widget, menu);
                    }

                    // Do grids/tabs
                    if (widget.initGridViews) {
                        widget.initGridViews(wparams, self._form, win);
                    }

                }
            }

        },

        addButton: function (button) { },

        getChildren: function () {

            return nx.bucket.getChildren(this);

        },

        getChildrenOfClass: function (cname) {

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
        }
    }
});
