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

nx.setup = {

    /**
     * Height/width for relative sizes/positions
     */
    rowHeight: 25,
    rowSpacing: 2,
    colWidth: 25,
    colSpacing: 2,
    labelWidth: 4,
    handleSize: 5,
    toolWidth: 30,

    tagNormal: '#E6E6FA', // Lavender
    tagOK: '#7FFFD4', // Aquamarine
    tagWarning: '#FFFACD', // LemonChiffon
    tagError: '#FFA07A', // LightSalmon
    tagTools: '#FFEFD5', 

    iconDefault: 'asterisk_yellow',
    iconActiveTask: 'asterisk_orange',

    templatesID: 'templates',

    viaWeb: 'NXCode - ',
    viaWebIcon: 'office',

    colorTable: [
        '#e6194b', '#3cb44b', '#ffe119', '#4363d8',
        '#f58231', '#911eb4', '#46f0f0', '#f032e6',
        '#bcf60c', '#fabebe', '#008080', '#e6beff',
        '#9a6324', '#fffac8', '#800000', '#aaffc3',
        '#808000', '#ffd8b1', '#000075', '#808080'
    ],

    colorPastel: [
        '#F1ECFF', '#FCFCE9', '#FFECEC', '#E3FBE9',
        '#DBF0F7', '#FFF1E6', '#FFECFF', '#FBFBE8',
        '#EAFFEF', '#F8F1F1', '#EEEEFF', '#F5F5E2'
    ],

    /**
     * 
     * Offset of new window
     * 
     */
    winOffset: 10,
    winOffsetDiff: 30,

    /**
     * 
     * The height of the caption
     * 
     */
    captionHeight: 35,

    /**
     * 
     * Initializes the system
     * 
     */
    __sys: function () {

        var self = this;

        Notiflix.Notify.Init({
            info: {
                background: '#6699ff',
                timeout: 6000
            },
            failure: {
                background: '#f1463a',
                timeout: 8000
            },
            warning: {
                background: '#d7cd13',
                timeout: 6000
            },
            success: {
                background: '#97d775',
                timeout: 6000
            }
        });

        nx.default.init();
    },

    /**
     * 
     * Checks to see if a key is in the requirements object and calls
     * a callback with the value if found
     * 
     * @param {widget} widget
     * @param {object} req
     * @param {string} key
     * @param {function} cb
     */
    __if: function (widget, req, key, cb, ncb) {

        // Do we have the key?
        if (req && typeof req[key] !== 'undefined') {
            // Do the callback
            if (cb) cb(widget, req[key]);
        } else {
            // Do the not callback
            if (ncb) ncb();
        }

    },

    /**
     * 
     * Sets up all options in the requirements object
     * 
     * @param {widget} widget
     * @param {object} req
     */
    __component: function (widget, req) {

        // If indirect
        req = req || nx.bucket.getParams(widget);

        // Valid?
        if (widget && req && typeof req === 'object') {

            // Loop thru the req keys
            Object.keys(req).forEach(function (key) {

                // Do we handle it?
                if (typeof nx.setup[key] === 'function') {

                    // Do
                    nx.setup[key](widget, req);
                }

            }, widget);

        }

    },

    top: function (widget, req) {

        //
        nx.setup.__if(widget, req, 'top', function (widget, sett) {

            // Can we do?
            if (widget.setDomTop) {
                // Protect
                try {
                    // Set
                    widget.setDomTop(nx.util.toRelative(sett, nx.setup.rowHeight));
                } catch (e) {
                    var a = e;
                }
            }

        });

    },

    left: function (widget, req) {

        //
        nx.setup.__if(widget, req, 'left', function (widget, sett) {

            // Can we do?
            if (widget.setDomLeft) {
                // Protect
                try {
                    // Set
                    widget.setDomLeft(nx.util.toRelative(sett, nx.setup.colWidth));
                } catch { }
            }

        });

    },

    width: function (widget, req) {

        //
        nx.setup.__if(widget, req, 'width', function (widget, sett) {

            // Can we do?
            if (widget.setWidth) {
                // Set
                widget.setWidth(nx.util.toRelative(sett, nx.setup.colWidth + nx.setup.colSpacing, 0));
            }

        });

    },

    height: function (widget, req) {

        //
        nx.setup.__if(widget, req, 'height', function (widget, sett) {

            // Can we do?
            if (widget.setHeight) {
                // Set
                widget.setHeight(nx.util.toRelative(sett, nx.setup.rowHeight + nx.setup.rowSpacing, 0));
            }

        });

    },

    value: function (widget, req) {

        //
        nx.setup.__if(widget, req, 'value', function (widget, sett) {

            // Is it an object?
            if (typeof sett === 'object') {
                sett = JSON.stringify(sett);
            } else {
                if (widget.setLabel) {
                    widget.setLabel(sett);
                } else if (widget.setValue) {
                    // Set
                    widget.setValue(sett);
                }
            }

        });

    },

    menu: function (widget, req) {

        //
        nx.setup.__if(widget, req, 'menu', function (widget, sett) {

            // Can we do?
            if (widget.setMenu) {

                // Make the menu
                var menu = new c._menu();
                // Make the entries
                nx.util.createMenu(menu, sett);
                // Add menu
                widget.setMenu(menu);

            }
        });

    },

    listeners: function (widget, req) {

        //
        nx.setup.__if(widget, req, 'listeners', function (widget, sett) {

            // Can we do?
            if (typeof sett === 'object') {

                // Loop thru
                Object.keys(sett).forEach(function (event) {

                    // Set
                    widget.addListener(event, sett[event], widget);

                });
            }


        });
    },

    fns: function (widget, req) {

        //
        nx.setup.__if(widget, req, 'fns', function (widget, sett) {

            // Can we do?
            if (typeof sett === 'object') {

                // Loop thru
                Object.keys(sett).forEach(function (fnname) {

                    // Get
                    var fn = sett[fnname];

                    // Valid?
                    if (typeof fn === 'function') {
                        // Set
                        widget[fnname] = fn;
                    }

                });
            }


        });
    },

    click: function (widget, req) {

        //
        nx.setup.__if(widget, req, 'click', function (widget, sett) {

            //
            var event = widget.classname === 'qx.ui.basic.Label' ? 'click' : 'execute';
            // Set
            widget.addListener(event, sett, widget);

        });

    },

    beforeOpen: function (widget, req) {

        //
        nx.setup.__if(widget, req, 'beforeOpen', function (widget, sett) {

            // Set
            widget.addListener('mousedown', sett, widget);

        });

    },

    selected: function (widget, req) {

        //
        nx.setup.__if(widget, req, 'selected', function (widget, sett) {

            // Set
            widget.addListener('changeValue', sett, widget);

        });

    },

    ro: function (widget, req) {

        //
        nx.setup.__if(widget, req, 'ro', function (widget, sett) {

            //
            if (widget.classname === 'c._grid') {

                // Can we do?
                if (widget.getSelectionModel) {
                    if (sett === 'y')
                        widget.getSelectionModel().setSelectionMode(1);
                }

            } else {

                // Can we do?
                if (widget.setEnabled) {
                    // Set
                    widget.setEnabled(sett !== 'y');
                }

            }

        });

    },

    tabsAt: function (widget, req) {

        //
        nx.setup.__if(widget, req, 'tabsAt', function (widget, sett) {

            // Can we do?
            if (widget.setBarPosition) {
                widget.setBarPosition(sett);
            }

        });

    },

    caption: function (widget, req) {

        //
        nx.setup.__if(widget, req, 'caption', function (widget, sett) {

            // Can we do?
            if (widget.setCaption) {
                // Set
                widget.setCaption(sett);
            }

        });

    },

    label: function (widget, req) {

        //
        nx.setup.__if(widget, req, 'label', function (widget, sett) {

            // Can we do?
            if (widget.setLabel && widget.classname !== 'qxl.upload.UploadButton') {
                // Set
                widget.setLabel(sett);
            }

        });

    },

    textAlign: function (widget, req) {

        //
        nx.setup.__if(widget, req, 'textAlign', function (widget, sett) {

            // Can we do?
            if (widget.setTextAlign) {
                // Set
                widget.setTextAlign(sett);
            }

        });

    },

    backgroundColor: function (widget, req) {

        ////
        //nx.setup.__if(widget, req, 'backgroundColor', function (widget, sett) {

        //    // Can we do?
        //    if (widget.setBackgroundColor) {
        //        // Set
        //        widget.setBackgroundColor(sett);
        //    }

        //});

    },

    icon: function (widget, req) {

        //
        nx.setup.__if(widget, req, 'icon', function (widget, sett) {

            // Can we do?
            if (widget.setIcon) {
                // Set
                widget.setIcon(nx.util.getIcon(sett));
            }

        });

    },

    _leafs: function (widget, req) {

        var self = this;

        //
        nx.setup.__if(widget, req, 'choices', function (widget, sett) {

            // Assure array
            if (!Array.isArray(sett)) sett = [sett];

            // Loop thru
            sett.forEach(function (entry) {

                //
                var node;
                // Does it have choices?
                if (entry.choices && entry.choices.length) {
                    node = new qx.ui.tree.TreeFolder(entry.label);
                    node.setOpen(true);
                    // Call setup
                    self._leafs(node, entry);
                } else {
                    node = new qx.ui.tree.TreeFile(entry.label);
                }
                // Icon
                if (entry.icon) node.setIcon(nx.util.getIcon(entry.icon));
                // Save click
                node.onClick = entry.click;
                nx.setup.drag(node, entry);
                // Context menu
                if (entry.contextMenu && entry.contextMenu.items) {
                    var menu = new c._menu();
                    nx.util.createMenu(menu, entry.contextMenu.items, node);
                    nx.util.setContextMenu(node, menu);
                }

                //
                nx.bucket.setParams(node, entry);
                // Add
                widget.add(node);

            });

        });

    },

    choices: function (widget, req) {

        var self = this;

        //
        nx.setup.__if(widget, req, 'choices', function (widget, sett) {

            // Assure array
            if (!Array.isArray(sett)) sett = [sett];

            //
            if (widget.setRoot) {
                // Make tree root
                var root = new qx.ui.tree.TreeFolder(req.treeRoot || req.caption || '');
                // Call leaf setup
                self._leafs(root, req);
                // Link
                widget.setRoot(root);
                //
                root.setOpen(true);

            } else if (widget.setMenu) {

                // Create the menu object
                var menu = new c._menu();
                // And now the menu
                nx.util.createMenu(menu, sett, widget);
                // Map
                widget.setMenu(menu);

            } else {

                // Can we remove?
                if (widget.removeAll) {
                    widget.removeAll();
                }

                // Must be able to add
                if (widget.add) {

                    // Add empty
                    if (nx.util.default(req.allowEmpty, true)) {
                        var tempItem = new qx.ui.form.ListItem('');
                        widget.add(tempItem);
                    }

                    // Loop thru
                    sett.forEach(function (entry) {
                        if (typeof entry === 'string') {
                            var tempItem = new qx.ui.form.ListItem(entry);
                            widget.add(tempItem);
                        } else {
                            var tempItem = new qx.ui.form.ListItem(entry.label, nx.util.getIcon(entry.icon));
                            tempItem.setSelectable(nx.util.default(entry.selectable, true));
                            if (entry.click) {
                                tempItem.addListener('click', entry.click);
                            }
                            widget.add(tempItem);
                        }
                    });

                    nx.setup.__if(widget, req, 'selectionMode', function (widget, sett) {
                        if (widget.setSelectionMode) widget.setSelectionMode(sett);
                    }, function () {
                        if (widget.setSelectionMode) widget.setSelectionMode('single');
                    });

                }

            }

        });

    },

    allowMany: function (widget, req) {

        nx.setup.__if(this, req, 'allowMany', function (widget, sett) {

            // Can we do?
            if (widget.setSelectionMode) {
                //
                widget.setSelectionMode(nx.util.toBoolean(sett) ? 'multi' : 'single');
            }

        });

    },

    contextMenu: function (widget, req) {

        nx.setup.__if(this, req, 'contextMenu', function (widget, sett) {

            // Can we do?
            if (widget.setContextMenu) {
                //
                var menu = new c._menu();
                nx.util.createMenu(menu, sett.items, widget);
                nx.util.setContextMenu(widget, menu);
            }

        });

    },

    drag: function (widget, req) {

        nx.setup.__if(this, req, 'drag', function (xwidget, sett) {

            // Sett must be an object
            if (typeof sett === 'object') {
                // Do we have a start?
                if (sett.onStart) {
                    if (widget.setDraggable) {
                        widget.setDraggable(true);
                        widget.addListener('dragstart', sett.onStart);
                    }
                }

                // Do we have a drop?
                if (sett.onDrop) {
                    if (widget.setDroppable) {
                        widget.setDroppable(true);
                        widget.addListener('drop', sett.onDrop);
                    }
                }
            }

        });

    }

};

nx.default = {

    _defaults: {
        fieldWidth: 20,
        pickWidth: 25,
        pickHeight: 20,
        signatureHeight: 10,
        inputWidth: 20,
        tabWidth: 22,
        parenttabWidth: 24,
        QRWidth: 6,
        QRHeight: 6
    },

    init: function () {

        var self = this;

        self._defaults.pickHeight = Math.floor((0.75 * nx.util.getAbsoluteBounds(nx.desktop).height) / nx.setup.rowHeight);
    },

    get: function (key) {

        var self = this;

        var ans;
        var mult = 1;

        if (key && typeof key === 'string') {
            if (nx.util.startsWith(key, '-')) {
                mult = -1;
                key = key.substr(1);
            }

            var factor = '';
            var pos = key.indexOf('@');
            if (pos !== -1) {
                factor = key.substr(pos + 1);
                key = key.substr(0, pos);
            }

            // Handle default
            if (nx.util.startsWith(key, 'default')) {
                // Remove 
                key = key.substr(7);
                // Remove extra
                var wkg = key.replace('.', '');
                // Get from user
                ans = nx.desktop.user.getSIField('default' + wkg);
                // Any?
                if (!ans) {
                    // Get from system
                    switch (wkg) {
                        case 'screenAbsoluteWidth':
                            ans = Math.floor(nx.util.getAbsoluteBounds(nx.desktop).width / nx.setup.colWidth);
                            break;

                        case 'screenAbsoluteHeight':
                            ans = Math.floor(nx.util.getAbsoluteBounds(nx.desktop).height / nx.setup.rowHeight);
                            break;

                        case 'screenWidth':
                            ans = Math.floor(nx.util.getAbsoluteBounds(nx.desktop).width * 0.95 / nx.setup.colWidth);
                            break;

                        case 'screenHeight':
                            ans = Math.floor(nx.util.getAbsoluteBounds(nx.desktop).height * 0.75 / nx.setup.rowHeight);
                            break;

                        case 'pickAdjust':
                            ans = 10 + (nx.setup.colWidth * self.get('default.pickWidth'));
                            break;

                        default:
                            ans = self._defaults[wkg];
                            break;
                    }
                }
            } else {
                ans = key;
            }
        } else {
            ans = key;
        }

        // Assure
        if (!ans) ans = 0;

        // Make number
        if (typeof ans !== 'number') {
            // Make it so
            ans = parseFloat(ans);
        }

        // Adjust
        if (mult !== 1) ans *= mult;

        // Add factor
        if (factor) {
            ans = Math.floor(ans * parseFloat(factor));
        }

        return ans;
    }

};

nx.tt = {

    tagWidget: function (e, fn, cb) {

        var widget = nx.util.eventGetWidget(e);
        var form = nx.bucket.getWin(widget);
        var params = nx.bucket.getParams(form);
        var ds = params.ds;
        var id = params.id;

        //
        nx.util.serviceCall('AO.Tag', {
            user: nx.desktop.user.getName(),
            type: 'pin',
            ds: ds,
            id: id,
            action: fn
        }, function (result) {
            // Reload
            nx.desktop.user._loadStartMenu();
            //
            if (cb) {
                cb(result);
            } else {
                // Tell user
                nx.util.notifyInfo('Tracking ' + result.value);
            }
        });
    },

    tagEntry: function (entry, fn, cb) {
        //
        nx.util.serviceCall('AO.Tag', {
            user: nx.desktop.user.getName(),
            type: 'pin',
            ds: entry.ds,
            id: entry.id,
            action: 'continue'
        }, function (result) {
            // Reload
            nx.desktop.user._loadStartMenu();
            //
            if (cb) {
                cb(result);
            } else {
                // Tell user
                nx.util.notifyInfo('Tracking ' + result.value);
            }
        });
    }

};