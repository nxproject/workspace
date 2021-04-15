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

nx.desktop = {

    // Location where all windows reside
    root: null,

    init: function (app) {

        // Create the desktop
        nx.desktop.root = new qx.ui.window.Desktop();
        // Limits
        var width = window.innerWidth;
        nx.desktop.root.setWidth(width);
        nx.desktop.root.setMaxWidth(width * 5);
        var height = window.innerHeight;
        nx.desktop.root.setHeight(height);
        nx.desktop.root.setMaxHeight(height * 5);

        // Add the desktop to the root
        app.getRoot().add(nx.desktop.root);

        // Create a container for the main layout and set the main layout
        var mainContainer = new qx.ui.container.Composite(new qx.ui.layout.Grow());
        mainContainer.setPadding(0);

        // add the scroll container to the root
        app.getRoot().add(mainContainer, { bottom: 0, right: 0, left: 0 });

        // Add taskbar
        nx.desktop.taskbar = new qx.ui.toolbar.ToolBar();
        nx.desktop.taskbar.setSpacing(5);
        mainContainer.add(nx.desktop.taskbar);

        // Make the start button
        nx.desktop.startButton = new qx.ui.toolbar.MenuButton('Start', nx.util.getIcon('logo_32'));
        nx.desktop.taskbar.add(nx.desktop.startButton);

        nx.desktop.taskbar.addSeparator();

        // The window buttons
        nx.desktop.windowButtons = new qx.ui.toolbar.Part();
        nx.desktop.taskbar.add(nx.desktop.windowButtons);

        nx.desktop.taskbar.addSpacer();

        // add a widget which signals that something have been removed
        var overflow = new qx.ui.toolbar.MenuButton('More...');
        nx.desktop.taskbar.add(overflow);
        nx.desktop.taskbar.setOverflowIndicator(overflow);

        // create Help Button and add it to the toolbar
        nx.desktop.commandButton = new qx.ui.toolbar.MenuButton('', nx.util.getIcon('star'));
        nx.setup.beforeOpen(nx.desktop.commandButton, {
            beforeOpen: function () {
                // Make the menu
                nx.setup.menu(nx.desktop.commandButton, {

                    menu: nx.desktop.user.getCommandMenu()

                });
            }
        });
        nx.desktop.taskbar.add(nx.desktop.commandButton);

        nx.desktop.clockArea = new qx.ui.basic.Label('...');
        nx.desktop.taskbar.add(nx.desktop.clockArea);

        // Display the time
        nx.desktop.updateClock(nx.desktop);

        // Create the user
        nx.office.loadMany([

            'nx.renderer',
            'nx.field',
            'nx.overlay',
            'nx.bucket',
            'nx.fs',
            'nx.rtc',

            'o.object',

            'i.iComponent',
            'i.iFormatter',
            'i.iTool',

            'c._window',
            'c._component',
            'c.string',
            'c._textfield',

            'tools.Documents',

            'tp.socket-io',
            'tp.notiflix-aio',
            'tp.chrono-min',
            'tp.md5-min',
            'tp.keydrown-min',
            'tp.signaturepad-min',
            'tp.clipboard-min',

            'qx.bom.Cookie',
            'qx.module.Cookie',

            'tp.qrious-min',
            'tp.print'

        ], function () {

            // Load packages
            nx.office.loadPackage([

                '@o',
                '@f',
                '@c',
                '@t',

                '@qxl.upload'

            ], function () {

                // Resize
                window.addEventListener('resize', function () {

                    // Resize
                    nx.desktop.root.setHeight(window.innerHeight);
                    nx.desktop.root.setWidth(window.innerWidth);

                });

                window.addEventListener('beforeunload', function () {
                    nx.desktop.resetUser();
                });

                nx.util.hasCamera();

                // System setup
                nx.setup.__sys();

                // Get the desktop
                var dtop = nx.desktop;

                // Create AO Object manager
                dtop.aomanager = new o.aomanager();

                // Make the user
                dtop.user = new o.user({});

                // Now see if there was something passed
                var queryString = window.location.search;
                var urlParams = new URLSearchParams(queryString);
                //
                var id = urlParams.get('id');
                if (id) {
                    nx.util.serviceCall('Access.Connect', {
                        item: id,
                        location: 'w'
                    }, function (result) {

                        if (result.pwd) {
                            nx.util.runTool('XPassword', {
                                pwd: result.pwd,
                                result: result,
                                onOk: function (result) {
                                    nx.desktop.user.processLogin(result);
                                }
                            });
                        } else {
                            nx.desktop.user.processLogin(result);
                        }

                    });
                } else {

                    var u = urlParams.get('u');
                    var p = urlParams.get('p');
                    if (u && p) {
                        nx.desktop.user.login(u, p);
                        if (!nx.desktop.user.getName()) {
                            // Create null user
                            dtop.resetUser();
                        }
                    } else {
                        // Create null user
                        dtop.resetUser();
                    }
                }


            });

        })

        // Link in AutoHotKeys
        window.onfocus = function () {
            // Get the clipboard text
            if (navigator.clipboard) {
                navigator.clipboard.readText()
                    .then(text => {
                        // Handle
                        nx.ahk.process(text);
                    })
                    .catch(err => {
                        //console.error('Failed to read clipboard contents: ', err);
                    });
            }
        };
    },

    // The active window
    _activeWindow: null,

    // The taskbar
    taskbar: null,

    // Start button
    startButton: null,

    // Command button
    commandButton: null,

    // The window buttons in the taskbar
    windowButtons: null,

    getDesktop: function () {

        var self = this;

        return self.root.getChildren()[0];

    },

    getBrowserMode: function () {
        // Get the width
        var w = window.innerWidth;

        if (w < 768) {
            // Extra Small Device
            return "xs";
        } else if (w < 991) {
            // Small Device
            return "sm"
        } else if (w < 1199) {
            // Medium Device
            return "md"
        } else {
            // Large Device
            return "lg"
        }
    },

    // ---------------------------------------------------------
    //
    // WINDOWS
    // 
    // ---------------------------------------------------------

    /**
     * 
     * Adds a window to the desktop
     * 
     * @param {any} req
     */
    addWindow: function (req) {

        var self = this;

        // Placeholder
        var win = null;

        // Assure
        req = req || {};

        // Do we have an id?
        if (req.nxid) {

            // Look for it
            win = self.findWindow(req.nxid);

        }

        // New?
        if (!win) {

            // Set defaults
            self._viewLocalize(null, req);

            // Create
            win = new c._window(req);
            win.setAutoDestroy(true);
            // Set the id
            nx.bucket.setWindowID(win, req.nxid)

            // Add
            nx.desktop.root.add(win);

            // Do we have a caller?
            if (req.caller) {
                // Allow for space
                req.caller.showAllow();
            }

        }

        // Show
        win.show();

        // Activate
        win.setActive(true);

        return win;

    },

    /**
     * 
     * Returns a window from a given id 
     * 
     * @param {string} id
     */
    findWindow: function (id) {

        var self = this;

        // Assume none
        var ans = null;

        // Loop thru
        self.getChildWindows().some(function (win) {
            // Same?
            if (nx.bucket.getWindowID(win) === id) {
                // Found
                ans = win;
            }

            return ans;
        });

        return ans;

    },

    /**
     * 
     * Sets the active window
     * 
     * @param {any} win
     */
    setActiveWindow: function (win) {

        var self = this;

        // Save
        self._activeWindow = win;

    },

    /**
     * 
     * 
     * Clears the active window if it is a specified window
     * 
     * @param {any} win
     */
    clearActiveWindow: function (win) {

        var self = this;

        // Save
        if (self._activeWindow === win) {
            self._activeWindow = null;
        }

    },

    /**
     * 
     * Gets the active window
     * 
     * */
    getActiveWindow: function () {

        var self = this;

        // Save
        return self._activeWindow;

    },

    getWindowAt: function (top, left) {

        var self = this;

        var ans;

        var ansdist = 0;

        // Loop thru
        self.getChildWindows().forEach(function (win) {

            // Get the bounds
            var bounds = nx.util.getAbsoluteBounds(win);

            // Get distance
            var dist = nx.util.getDistance(bounds.top, bounds.left, top, left);
            // Must be close
            if (dist <= nx.setup.winOffsetDiff) {
                // New or smaller?
                if (!ans || dist < ansdist) {
                    // Save
                    ans = win;
                    ansdist = dist;
                }
            }

        });

        return ans;

    },

    /**
     * 
     * Is the window a dataset based window?
     * 
     * @param {any} widget
     * @param {bool} insysmode
     */
    isWindowDS: function (widget, insysmode) {
        // Assume not
        var ans = false;
        // Get the form
        var form = nx.bucket.getForm(widget);
        // 
        if (form) {
            // Get params
            var ds = nx.bucket.getDataset(form);
            // Do we have a dataset?
            ans = !!ds;
            // Check for sysmode
            if (insysmode && ans) {
                // Reset
                ans = false;
                // Get the form
                form = nx.bucket.getForm(form);
                if (form) {
                    var params = nx.bucket.getParams(form);
                    // Do we have params?
                    if (params) {
                        ans = !!params.sysmode;
                    }
                }
            }
        }
        return ans;
    },

    /**
     * 
     * Adds a a dataset view
     * 
     * @param {object} req
     */
    addWindowDS: function (req) {

        var self = this;

        // Assure
        req = req || {};
        // Parse
        var ds = req.ds;
        var id = req.id;
        var view = req.view || nx.desktop.user.getDSInfo(ds).view;
        var mode = (id ? 'edit' : 'add');
        var caption = req.caption;
        var caller = req.caller;

        // Make window id if needed
        if (nx.util.hasValue(ds) && nx.util.hasValue(id)) {
            req.nxid = 'ao_' + ds + '_' + id;
        }

        // must have a dataset
        if (ds) {
            self._loadDataset(ds, function (dsdef) {

                // Do we have a dataset?
                if (dsdef) {
                    // Load the view
                    self._loadView(ds, view, function (rviewdef) {
                        // Explode
                        self._viewExplode(ds, rviewdef, function (viewdef) {
                            // Get the data
                            self._loadData(ds, id, function (data) {

                                // Handle new
                                if (!id && data) {
                                    req.id = id = data.values._id;
                                }

                                //
                                if (req.data) {
                                    Object.keys(req.data).forEach(function (fld) {
                                        var value = req.data[fld];
                                        data.setField(fld, value);
                                    });
                                }

                                // Update the data
                                Object.keys(viewdef.fields).forEach(function (fldname) {
                                    var fdef = viewdef.fields[fldname];
                                    if (typeof fdef.value !== 'undefined') {
                                        data.forceField(fdef.aoFld, fdef.value);
                                    }
                                });

                                var win;

                                // Top toolbar
                                var tt = req.topToolbar || {};
                                tt.items = tt.items || [];
                                // Parent
                                tt.items.push({
                                    label: 'Parent',
                                    icon: 'accept',
                                    click: function (e) {
                                        // Get the widget
                                        var widget = nx.util.eventGetWidget(e);
                                        // And the parent
                                        var parent = nx.bucket.getParent(widget);
                                        if (parent) {
                                            // Parse
                                            var info = nx.util.uuidToObject(parent);
                                            // View
                                            nx.desktop.addWindowDS({
                                                ds: info.ds,
                                                id: info.id,
                                                view: nx.desktop.user.getDSInfo(info.ds).view,
                                                caller: nx.util.eventGetWindow(e),
                                                chain: req.chain
                                            });
                                        }
                                    }
                                });

                                // Do we have an account?
                                var acct = data.values._account;
                                if (acct) {

                                    // Billing?
                                    if (nx.desktop.user.getIsSelector('BILLING')) {
                                        // TBD
                                    }

                                    // Extended?
                                    if (nx.desktop.user.getIsSelector('TELE') || nx.desktop.user.getIsSelector('EMAIL')) {


                                        var items = [
                                            new t.quickmessage()
                                        ];

                                        if (nx.util.isEMail(acct)) {
                                            items.push(new t.emailTelemetry());
                                        }

                                        if (nx.util.isPhone(acct)) {
                                            items.push(new t.phonesmsTelemetry());
                                        }

                                        tt.items.push({
                                            label: acct,
                                            icon: 'lightning',
                                            choices: items
                                        });
                                    }

                                }

                                // Add documents
                                if (nx.desktop.user.opAllowed(ds, 'd')) {
                                    tt.items.push({
                                        label: 'Documents',
                                        icon: 'folder',
                                        click: function (e) {
                                            nx.util.runTool('Documents', {
                                                ds: ds,
                                                id: id,
                                                caption: nx.util.localizeDesc(data.getField('_desc') || 'New'),
                                                caller: win
                                            });
                                        }
                                    });
                                }

                                // Localize
                                var childdss = dsdef.childdss;
                                var relateddss = dsdef.relateddss;

                                // Make list
                                var list = nx.util.splitSpace(relateddss);
                                // Assure end
                                list.push(null);
                                // Loop thru
                                while (list.length) {
                                    // Get
                                    var cds = list.shift();
                                    var cfld = list.shift();
                                    // Add button
                                    if (cds && cfld) {

                                        //
                                        if (nx.desktop.user.opAllowed(cds, 'a') || nx.desktop.user.opAllowed(cds, 'v')) {
                                            // Get
                                            nx.desktop._loadDataset(cds, function (cdsdef) {

                                                tt.items.push({
                                                    label: cdsdef.caption,
                                                    icon: cdsdef.icon,
                                                    passed: {
                                                        ds: cds,
                                                        fld: cfld
                                                    },
                                                    click: function (e) {
                                                        // Get the widget
                                                        var widget = nx.util.eventGetWidget(e);
                                                        // And the form
                                                        var win = nx.bucket.getWin(widget);
                                                        // Get the passed
                                                        var passed = nx.bucket.getPassed(widget);
                                                        // And the target
                                                        var targetds = passed.ds;
                                                        // Get the data source
                                                        var data = win._aoobject.values;
                                                        // And make the id
                                                        var id = nx.util.objectToUUID(data);
                                                        // Make the chain
                                                        var chain = {
                                                            sop: 'Any',
                                                            queries: [{
                                                                field: passed.fld,
                                                                op: '=',
                                                                value: id
                                                            }],
                                                            _cooked: true
                                                        }
                                                        // Call picker
                                                        nx.util.runTool('View', {
                                                            ds: targetds,
                                                            caller: win,
                                                            chain: chain
                                                        });
                                                    }
                                                });
                                            });
                                        }

                                    } else {

                                        // Make list
                                        var list = nx.util.splitSpace(childdss);
                                        // Assure end
                                        list.push(null);
                                        // Loop thru
                                        while (list.length) {
                                            // Get
                                            var cds = list.shift();
                                            // Add button
                                            if (cds) {

                                                //
                                                if (nx.desktop.user.opAllowed(cds, 'a') || nx.desktop.user.opAllowed(cds, 'v')) {
                                                    // Get
                                                    nx.desktop._loadDataset(cds, function (cdsdef) {

                                                        tt.items.push({
                                                            label: cdsdef.caption,
                                                            icon: cdsdef.icon,
                                                            passed: {
                                                                ds: cds
                                                            },
                                                            click: function (e) {
                                                                // Get the widget
                                                                var widget = nx.util.eventGetWidget(e);
                                                                // And the form
                                                                var win = nx.bucket.getWin(widget);
                                                                // And the params
                                                                var params = nx.bucket.getParams(win);
                                                                // Get the dataset
                                                                var ds = params.ds;
                                                                // And the target
                                                                var targetds = nx.bucket.getPassed(widget).ds;
                                                                // Get the data source
                                                                var data = win._aoobject.values;
                                                                // And make the id
                                                                var id = nx.util.objectToUUID(data);
                                                                // Save the object
                                                                win.save(function () {
                                                                    // Make the chain
                                                                    var chain = {
                                                                        sop: 'Any',
                                                                        queries: [{
                                                                            field: '_parent',
                                                                            op: '=',
                                                                            value: id
                                                                        }],
                                                                        _cooked: true
                                                                    }
                                                                    // Call picker
                                                                    nx.util.runTool('View', {
                                                                        ds: targetds,
                                                                        caller: win,
                                                                        chain: chain
                                                                    });
                                                                });
                                                            }
                                                        });
                                                    });
                                                }

                                            } else {

                                                // Do we have a chain?
                                                if (req.chain && req.chain._cooked) {
                                                    // Get the queries
                                                    var xlist = req.chain.queries;
                                                    // Do we use just first
                                                    if (req.chain.sop != - 'All') {
                                                        xlist = [xlist[0]];
                                                    }
                                                    // Loop thru
                                                    xlist.forEach(function (entry) {
                                                        // Set the value
                                                        data.forceField(entry.field, entry.value);
                                                    });
                                                }

                                                // Add tools
                                                var tdefs = [];

                                                if (nx.desktop.user.opAllowed(req.ds, 'v', (dsdef.chatAllow || ''))) {
                                                    tdefs.push({
                                                        label: 'Chat',
                                                        icon: 'user_comment',
                                                        click: function (e) {
                                                            var widget = nx.util.eventGetWidget(e);
                                                            var form = nx.bucket.getWin(widget);
                                                            var win = nx.bucket.getWin(widget);

                                                            //
                                                            nx.util.runTool('Chat', {
                                                                desc: win.getCaption(),
                                                                win: nx.bucket.getParams(form).nxid,
                                                                caller: win
                                                            });
                                                        }
                                                    });
                                                }

                                                if (nx.desktop.user.opAllowed(req.ds, 'v', (dsdef.ttAllow || ''))) {

                                                    ttitems = [
                                                        {
                                                            label: 'Start',
                                                            icon: 'flag_green',
                                                            click: function (e) {
                                                                nx.tt.tagWidget(e, 'start');
                                                            }
                                                        }, {
                                                            label: 'Start Frozen',
                                                            icon: 'flag_yellow',
                                                            click: function (e) {
                                                                nx.tt.tagWidget(e, 'startf');
                                                            }
                                                        }, {
                                                            label: 'Continue',
                                                            icon: 'flag_yellow',
                                                            click: function (e) {
                                                                nx.tt.tagWidget(e, 'continue');
                                                            }
                                                        }, {
                                                            label: 'Show',
                                                            icon: 'date',
                                                            click: function (e) {
                                                                nx.tt.tagWidget(e, 'show', function (result) {
                                                                    // Tell user
                                                                    nx.util.runTool('Message', {
                                                                        caption: 'Time tracking',
                                                                        msg: result.value,
                                                                        caller: win
                                                                    });
                                                                });
                                                            }
                                                        }
                                                    ];

                                                    tdefs.push({
                                                        label: 'Time track',
                                                        icon: 'star',
                                                        items: ttitems
                                                    });
                                                }


                                                var sep = !!tdefs.length;

                                                if (nx.desktop.user.opAllowed(req.ds, 'o', (dsdef.orgAllow || ''))) {
                                                    if (!sep) {
                                                        sep = true;
                                                        tdefs.push('-');
                                                    }
                                                    tdefs.push({
                                                        label: 'Organizer',
                                                        icon: 'org',
                                                        click: function (e) {
                                                            var widget = nx.util.eventGetWidget(e);
                                                            var form = nx.bucket.getWin(widget);
                                                            var params = nx.bucket.getParams(form);
                                                            var ds = params.ds;
                                                            var view = params.view;
                                                            var id = params.id;

                                                            //
                                                            nx.util.serviceCall('Docs.Organizer', {
                                                                ds: ds,
                                                                id: id,
                                                                data: form.getFormData()
                                                            }, function (result) {
                                                                // Show
                                                                if (result && result.path) {
                                                                    nx.fs.viewPDF(result);
                                                                }
                                                            });
                                                        }
                                                    });
                                                }

                                                if (tdefs.length && !nx.desktop.user.getIsAccount()) {
                                                    tt.items.push({
                                                        label: 'Options',
                                                        icon: 'wrench',
                                                        choices: tdefs
                                                    });
                                                }

                                                // Make the wrapper
                                                var windef = {

                                                    nxid: req.nxid,
                                                    obj: data,
                                                    ds: ds,
                                                    id: id,
                                                    icon: dsdef.icon || 'application',
                                                    allowClose: false,
                                                    atSave: req.atSave,
                                                    sysmode: req.sysmode,
                                                    _dsfields: nx.util.removeSystemKeys(Object.keys(dsdef.fields)),

                                                    topToolbar: tt,
                                                    bottomToolbar: req.bottomToolbar,
                                                    caller: caller,
                                                    chain: req.chain
                                                };

                                                nx.bucket.setDataset(windef, dsdef);
                                                nx.bucket.setView(windef, viewdef);

                                                var commands = [];

                                                commands.push({
                                                    label: 'Close',
                                                    icon: 'cancel',
                                                    click: function (e) {

                                                        // Only if active
                                                        if (!req.sysmode) {
                                                            var self = this;

                                                            // Map window
                                                            var win = nx.bucket.getWin(self);

                                                            // Close
                                                            win.safeClose();
                                                        }

                                                    }
                                                });

                                                // Can we delete?
                                                if (nx.desktop.user.opAllowed(ds, 'x') && data.getField('_desc')) {
                                                    commands.push('>');
                                                    commands.push({
                                                        label: 'Delete',
                                                        icon: 'application_delete',
                                                        click: function (e) {

                                                            var self = this;

                                                            // Map window
                                                            var win = nx.bucket.getWin(self);

                                                            nx.util.confirm('Are you sure?', 'Delete ' + nx.util.localizeDesc(data.getField('_desc')) + '...', function (ok) {

                                                                if (ok) {

                                                                    //
                                                                    var ds = req.ds;

                                                                    // Fix ds name
                                                                    ds = nx.util.toDatasetName(ds);

                                                                    // Delete
                                                                    nx.util.serviceCall('AO.ObjectDelete', {
                                                                        ds: ds,
                                                                        id: id
                                                                    }, nx.util.noOp);

                                                                    // Close
                                                                    win.safeClose();

                                                                }

                                                            });
                                                        }
                                                    });
                                                }

                                                // Can we merge?
                                                if (nx.desktop.user.opAllowed(ds, 'm')) {
                                                    commands.push('>');
                                                    commands.push({
                                                        label: 'Merge',
                                                        icon: 'arrow_merge',
                                                        choices: []
                                                    });
                                                }

                                                // Can we run a task?
                                                if (nx.desktop.user.opAllowed(ds, 't')) {
                                                    commands.push('>');
                                                    commands.push({
                                                        label: 'Task',
                                                        icon: 'cog',
                                                        choices: []
                                                    });
                                                }

                                                commands.push('>');
                                                commands.push({
                                                    label: 'Ok',
                                                    icon: 'database_save',
                                                    click: function (e) {

                                                        var self = this;

                                                        // Map window
                                                        var win = nx.bucket.getWin(self);

                                                        // Get params
                                                        var params = nx.bucket.getParams(nx.bucket.getForm(win));

                                                        if (!params.sysmode) {

                                                            // Save
                                                            win.save(function (ok) {
                                                                // Close
                                                                if (ok) win.safeClose();
                                                            });
                                                        }
                                                    }
                                                });

                                                windef.commands = {
                                                    items: commands
                                                };

                                                windef.processSIO = function (win, event) {

                                                    //
                                                    switch (event.fn) {
                                                        case '$$object.init':
                                                            if (event.message.winid === req.nxid) {
                                                                var changes = win.getFormDataChanges();
                                                                Object.keys(changes).forEach(function (fld) {
                                                                    nx.desktop.user.SIOSend('$$object.data', {
                                                                        aofld: fld,
                                                                        winid: req.nxid,
                                                                        value: changes[fld]
                                                                    });
                                                                });
                                                            }
                                                            break;

                                                        case '$$object.data':
                                                            // Get the params
                                                            var params = nx.bucket.getParams(win);

                                                            // Get the message
                                                            var msg = event.message;
                                                            // Must have one
                                                            if (msg) {
                                                                // Check to see if it is our event
                                                                if (params.nxid === msg.winid) {
                                                                    // Set the value
                                                                    win.setValue(msg.aofld, msg.value);
                                                                }
                                                            }
                                                            break;

                                                        case '$$changed.dataset':
                                                        case '$$changed.picklist':
                                                        case '$$changed.view':
                                                            if (event.message && event.message.deleted !== 'y') {
                                                                if (req.sysmode) {
                                                                    // Get the params
                                                                    var params = nx.bucket.getParams(nx.bucket.getForm(win));
                                                                    // Fetch the dataset
                                                                    nx.desktop._loadDataset(params.ds, function (result) {
                                                                        // Save
                                                                        params._dsfields = Object.keys(result.fields);
                                                                        // Get the button
                                                                        var button = win.getButtonWithLabel(win.sysToolbar, 'Fields');
                                                                        // Set the menu
                                                                        if (button) {
                                                                            button.setMenu(nx.util.getFieldsContextMenu(win));
                                                                        }
                                                                    }, !!event.message);

                                                                    // Do we refresh ourselves?
                                                                    if (event.fn === '$$changed.view' &&
                                                                        event.message.ds === params.ds &&
                                                                        !win.getChildWindows().length) {
                                                                        // Close
                                                                        win.safeClose();
                                                                        // And reopen
                                                                        nx.util.runTool('object', {
                                                                            ds: params.ds,
                                                                            view: nx.bucket.getView(params)._id.substr(6),
                                                                            sysmode: true,
                                                                            caller: params.caller
                                                                        });
                                                                    }
                                                                }
                                                            }
                                                            break;

                                                        case '$$changed.document':
                                                            var ds = req.ds;
                                                            var id = nx.setup.templatesID;
                                                            // Is this the droid I am lookig for?
                                                            if (ds === event.message.ds && id === event.message.id) {

                                                                // Get datasets
                                                                nx.util.serviceCall('Docs.DocumentPickList', {
                                                                    ds: ds,
                                                                    id: id
                                                                }, function (result) {
                                                                    // Extend
                                                                    result.list.forEach(function (entry) {
                                                                        entry.click = function (e) {
                                                                            var widget = nx.util.eventGetWidget(e);
                                                                            var data = widget.getFormData();
                                                                            var params = nx.bucket.getParams(nx.util.eventGetWidget(e));
                                                                            var ds = params.ds;
                                                                            var id = params.id;
                                                                            var path = nx.bucket.getParams(e.getTarget()).path;
                                                                            nx.util.serviceCall('Docs.Merge', {
                                                                                ds: ds,
                                                                                id: id,
                                                                                path: path,
                                                                                data: data
                                                                            }, function (result) {
                                                                                if (result && result.path) {
                                                                                    result.path.forEach(function (path) {
                                                                                        nx.fs.editDOCX({
                                                                                            path: path
                                                                                        });
                                                                                    });
                                                                                }
                                                                            });
                                                                        };
                                                                    });
                                                                    // Make menu
                                                                    var menu = new c._menu();
                                                                    nx.util.createMenu(menu, result.list, win);

                                                                    // 
                                                                    var btn = win.getButtonWithLabel(win.commandToolbar, 'Merge');
                                                                    if (btn) {
                                                                        btn.setMenu(menu);
                                                                    }
                                                                });
                                                            }
                                                            break
                                                    }
                                                };

                                                windef.listeners = {

                                                    appear: function (e) {

                                                        //
                                                        if (req.sysmode) {
                                                            nx.desktop._enterDSEdit(ds, view);
                                                        }

                                                        // Get window
                                                        var win = nx.util.eventGetWidgetOfClass(e, 'c._window');

                                                        // Call
                                                        nx.desktop.sendSIO('$$object.init', {
                                                            winid: req.nxid
                                                        });

                                                        // Call
                                                        if (req.sysmode) {
                                                            win.processSIO(win, {
                                                                fn: '$$changed.dataset'
                                                            });
                                                        }

                                                        // Call
                                                        win.processSIO(win, {
                                                            fn: '$$changed.document',
                                                            message: {
                                                                ds: req.ds,
                                                                id: nx.setup.templatesID
                                                            }
                                                        });
                                                    }

                                                };

                                                if (req.sysmode) {

                                                    windef.syscommands = {

                                                        items: [
                                                            {
                                                                label: 'Close',
                                                                icon: 'cancel',
                                                                click: function (e) {

                                                                    var self = this;

                                                                    // Map window
                                                                    var win = nx.bucket.getWin(self);

                                                                    nx.desktop._leaveDSEdit(ds, view);

                                                                    // Close
                                                                    win.safeClose();

                                                                }

                                                            }, '>', {
                                                                label: 'Delete',
                                                                icon: 'database_delete',
                                                                click: function (e) {

                                                                    var self = this;

                                                                    // Map window
                                                                    var win = nx.bucket.getWin(self);

                                                                    nx.util.confirm('Are you sure?', 'Delete ' + req.ds + '/' + req.view + '...', function (ok) {

                                                                        if (ok) {

                                                                            //
                                                                            var ds = req.ds;
                                                                            var view = req.view;

                                                                            // Fix 
                                                                            ds = nx.util.toDatasetName(ds);
                                                                            view = nx.util.toViewName(view);

                                                                            // Delete
                                                                            nx.util.serviceCall('AO.ViewDelete', {
                                                                                ds: ds,
                                                                                view: view
                                                                            }, nx.util.noOp);

                                                                            nx.desktop._leaveDSEdit(ds, view);

                                                                            // Close
                                                                            win.safeClose();

                                                                        }

                                                                    });

                                                                }
                                                            }, '>', {
                                                                label: 'Add Fields',
                                                                icon: 'application_add',
                                                                click: function (e) {
                                                                    // Get the button
                                                                    var win = nx.util.eventGetWindow(e);

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
                                                                }
                                                                // choices: []
                                                            }, '>', {

                                                                label: 'Save',
                                                                icon: 'accept',
                                                                click: function (e) {

                                                                    var self = this;

                                                                    // Map window
                                                                    var win = nx.bucket.getWin(self);

                                                                    // Get the wiews
                                                                    var views = nx.bucket.getUsedViews(win);
                                                                    // Add ourselves
                                                                    views.push(view);
                                                                    // Loop thru
                                                                    views.forEach(function (view) {
                                                                        // Save view definition
                                                                        nx.desktop._updateView(windef.ds, view);
                                                                    });

                                                                    nx.desktop._leaveDSEdit(ds, view);

                                                                    // Close
                                                                    win.safeClose();
                                                                }
                                                            }
                                                        ]
                                                    }

                                                }

                                                // Handle if unique
                                                if (dsdef.isUnique === 'y') {
                                                    windef.nxid = ds;
                                                }

                                                // Handle caller
                                                if (caller) {
                                                    if (typeof caller === 'function') {
                                                        caller = nx.bucket.getWindowID(caller);
                                                    }
                                                    if (caller) {
                                                        windef.caller = caller;
                                                    }
                                                }

                                                // According to the mode
                                                switch (mode) {
                                                    case 'edit':
                                                        viewdef = self._viewEdit(viewdef);
                                                        windef.caption = caption || nx.util.localizeDesc(data.values._desc || caption || '');
                                                        windef.nxid = 'ao_' + ds + '_' + id;
                                                        // ID alias?
                                                        if (nx.util.hasValue(dsdef.idalias)) {
                                                            // Get
                                                            var vfld = viewdef.fields[dsdef.idalias];
                                                            if (vfld) {
                                                                vfld.ro = true;
                                                            }
                                                        }
                                                        // Set the fields
                                                        windef.items = self._viewFields(viewdef.fields, dsdef);
                                                        // Make
                                                        win = self.addWindow(windef);
                                                        break;
                                                    case 'add':
                                                        windef.caption = (caption || req.sysmode ? 'Dataset/View Editor ' : 'New ') +
                                                            (req.sysmode ? ds + '/' + viewdef._id.substr(6) : dsdef.caption || ds);
                                                        // Set the fields
                                                        windef.items = self._viewFields(viewdef.fields, dsdef);
                                                        // Make
                                                        win = self.addWindow(windef);
                                                        break;
                                                }

                                                // Save
                                                nx.bucket.setParams(win, req);
                                            }
                                        }
                                    }
                                }

                            }, nx.desktop.user.getIsSelector('TELE') ||
                            nx.desktop.user.getIsSelector('EMAIL') ||
                            nx.desktop.user.getIsSelector('BILLING'));
                        });
                    });
                }

            });
        }

    },

    _dsEdits: {},

    _enterDSEdit: function (ds, view) {

        var self = this;

        // Fix ds name
        ds = nx.util.toDatasetName(ds);

        // Make room
        var dsInfo = self._dsEdits[ds];
        if (!dsInfo) {
            dsInfo = [];
            self._dsEdits[ds] = dsInfo;
        }
        // Assure view
        view = view || '';
        // Check
        if (dsInfo.indexOf(view) === -1) dsInfo.push(view);

    },

    _leaveDSEdit: function (ds, view) {

        var self = this;

        // Fix ds name
        ds = nx.util.toDatasetName(ds);

        // Do we have?
        var dsInfo = self._dsEdits[ds];
        if (dsInfo) {
            // Assure view
            view = view || '';
            // Remove
            var pos = dsInfo.indexOf(view);
            if (pos !== -1) {
                dsInfo.splice(pos, 1);
            }
            // If no more, delete
            if (!dsInfo.length) {
                delete self._dsEdits[ds]
            }
            // Reload dataset
            self._loadDataset(ds, function () {
                // View?
                if (nx.util.hasValue(view)) {
                    // Reload
                    self._loadView(ds, view, nx.util.noOp, true);
                }
            }, true);
        }

    },

    _checkDSEdit: function (ds, view) {

        var self = this;

        var ans = false;

        // Fix ds name
        ds = nx.util.toDatasetName(ds);

        // Do we have?
        var dsInfo = self._dsEdits[ds];
        if (dsInfo) {
            if (nx.util.hasValue(view)) {
                ans = dsInfo.indexOf(view) !== -1;
            } else {
                ans = true;
            }
        }
        return ans;
    },

    _loadData: function (ds, id, cb, floataccount) {

        var self = this;

        // Fix ds name
        ds = nx.util.toDatasetName(ds);

        // And the information
        nx.desktop.aomanager[nx.util.hasValue(id) ? 'get' : 'create'](ds, id, function (data) {
            // Valid?
            if (!data || !data.values || !data.values._id) {
                // Null
                data = null;
            }
            // Do
            if (cb) cb(data);
        }, floataccount);
    },

    _loadDataset: function (ds, cb, force) {

        var self = this;

        // Fix ds name
        ds = nx.util.toDatasetName(ds);

        // Do we have it already?
        if (self.__ds[ds] && !force) {
            if (cb) cb(self.__ds[ds]);
        } else {
            // Get the view
            nx.util.serviceCall('AO.DatasetGet', {
                ds: ds
            }, function (result) {

                // Any?
                if (result && result._id) {
                    // Do we need to merge?
                    if (self._checkDSEdit(ds)) {
                        // Get current
                        var curr = self.__ds[ds];
                        // Any?
                        if (curr) {
                            // Loop thru
                            Object.keys(curr.fields).forEach(function (fldname) {
                                // Local?
                                if (curr.fields[fldname]._local) {
                                    // Copy
                                    result.fields[fldname] = curr.fields[fldname];
                                }
                            });
                        }
                    }
                    // Save
                    self.__ds[ds] = result;
                    // Get list
                    var list = nx.util.splitSpace(result.childdss, true);
                    // Assure
                    list.forEach(function (ds) {
                        if (ds) {
                            self._loadDataset(ds, nx.util.noOp);
                        }
                    });

                    // Get pick list
                    nx.util.serviceCall('AO.PickListGetAll', {
                        ds: ds
                    }, function (result) {
                        self.__pick[ds] = result.list;

                        if (cb) cb(self.__ds[ds]);
                    });
                }
            });
        }

    },

    _removeDataset: function (ds) {

        var self = this;

        // Fix ds name
        ds = nx.util.toDatasetName(ds);

        // remove
        delete self.__ds[ds];
    },

    _addDatasetFields: function (ds, flds) {

        var self = this;

        // Fix ds name
        ds = nx.util.toDatasetName(ds);

        // Assure array
        if (!Array.isArray(flds)) flds = [flds];
        // Assure dataset definition
        self._loadDataset(ds, function () {
            // Get dataset fields
            var fields = self.__ds[ds].fields;
            // Loop thru
            flds.forEach(function (fieldname) {
                if (!fields[fieldname]) {
                    fields[fieldname] = {
                        name: fieldname,
                        label: nx.util.ifEmpty(fieldname).replace(/(^|[^a-zA-Z\u00C0-\u017F'])([a-zA-Z\u00C0-\u017F])/g, function (m) {
                            return m.toUpperCase();
                        }),
                        nxtype: 'string',
                        _local: true
                    };
                }
            });
        });
    },

    _updateDataset: function (ds, data) {

        var self = this;

        // Fix ds name
        ds = nx.util.toDatasetName(ds);

        // Update
        if (data) {
            self.__ds[ds] = nx.util.merge(self.__ds[ds], data);
        }

        // Localize
        var curr = self.__ds[ds];
        // Loop thru
        Object.keys(curr.fields).forEach(function (fldname) {
            // Delete local flag
            delete curr.fields[fldname]._local;
        });

        // Save
        nx.util.serviceCall('AO.DatasetSet', {
            data: curr
        }, nx.util.noOp);

    },

    _setDatasetField: function (ds, def) {

        var self = this;

        // Adjust
        ds = nx.util.toDatasetName(ds);

        // Get dataset
        var ads = self.__ds[ds];
        // All kosher?
        if (ads && def && def.name) {
            // 
            var clean = {};
            Object.keys(def).forEach(function (key) {
                if (def[key]) clean[key] = def[key];
            });
            // Save
            ads.fields[def.name] = clean;
        }

        return ads;
    },

    _setViewField: function (ds, view, def) {

        var self = this;

        var aview;

        // Adjust
        ds = nx.util.toDatasetName(ds);
        view = nx.util.toViewName(view);

        // Get dataset
        var ads = self.__views[ds];
        // Valid?
        if (ads) {
            // Get view
            aview = ads[view];
            // All kosher?
            if (aview && def && def.aoFld) {
                // Save
                aview.fields[def.aoFld] = def;
            }
        }

        return aview;
    },

    _updateView: function (ds, viewname) {

        var self = this;

        // Fix ds name
        ds = nx.util.toDatasetName(ds);

        // Get the views
        var view = self.__views[ds][viewname];

        // Any views?
        if (view) {

            // Loo thru
            Object.keys(view.fields).forEach(function (fldname) {
                // Get the definiyion
                var def = view.fields[fldname];
                // Local?
                if (def._local && def.nxtype === 'hidden') {
                    // Delete
                    delete view.fields[fldname];
                } else {
                    // Delete local flag
                    delete view.fields[fldname]._local;
                }
            });

            // Force dataset
            self._updateDataset(ds);
            // Save
            nx.util.serviceCall('AO.ViewSet', {
                data: view
            }, nx.util.noOp);
        }

    },

    _loadView: function (ds, view, cb, force) {

        var self = this;

        // Fix ds name
        ds = nx.util.toDatasetName(ds);

        // Assure space
        if (!self.__views[ds]) {
            self.__views[ds] = {};
        }
        var defs = self.__views[ds];
        //self.__views[ds] = defs;

        // Do we have it already?
        if (defs[view] && !force) {
            if (cb) cb(defs[view]);
        } else {
            // Get the view
            nx.util.serviceCall('AO.ViewGet', {
                ds: ds,
                id: view
            }, function (result) {

                // Any?
                if (result) {

                    // Do we need to merge?
                    if (self._checkDSEdit(ds, view)) {
                        // Get current
                        var curr = self.__views[ds][view];
                        // Any?
                        if (curr) {
                            // Loop thru
                            Object.keys(curr.fields).forEach(function (fldname) {
                                // Local?
                                if (curr.fields[fldname]._local) {
                                    // Copy
                                    result.fields[fldname] = curr.fields[fldname];
                                }
                            });
                        }
                    }
                    // 
                    // Save
                    defs[view] = result;
                    if (cb) cb(defs[view]);
                }
            });
        }
    },

    _addViewFields: function (ds, view, flds) {

        var self = this;

        var ans = [];

        // Fix ds name
        ds = nx.util.toDatasetName(ds);

        // Assure array
        if (!Array.isArray(flds)) flds = [flds];
        // Assure dataset definition
        self._loadDataset(ds, function (dsdef) {
            // Assure view definition
            self._loadView(ds, view, function (viewdef) {

                // Set the position
                var row = 1;
                // Loop thru
                Object.keys(viewdef.fields).forEach(function (fname) {
                    // Get the definition
                    var def = viewdef.fields[fname];
                    // If not hidden
                    if (def.nxtype !== 'hidden') {
                        //  Get the top
                        var top = nx.util.toInt(def.top) + nx.util.toInt(def.height, 10, 1);
                        // Better one?
                        if (row < top) row = top;
                    }
                });

                // Get dataset fields
                var fields = self.__views[ds][view].fields;
                // Loop thru
                flds.forEach(function (fieldname) {
                    if (!fields[fieldname] || fields[fieldname].nxtype === 'hidden') {
                        fields[fieldname] = {
                            aoFld: fieldname,
                            nxtype: 'string',
                            top: row.toString(),
                            left: '1',
                            height: '1',
                            width: nx.default.get('default.fieldWidth').toString(),
                            label: nx.util.ifEmpty(fieldname).replace(/(^|[^a-zA-Z\u00C0-\u017F'])([a-zA-Z\u00C0-\u017F])/g, function (m) {
                                return m.toUpperCase();
                            }),
                            _local: true
                        };
                        // Next row
                        row++;
                        // Add
                        ans.push(fields[fieldname]);
                    }
                });
            });
        });

        return ans;
    },

    _deleteViewFields: function (ds, view, flds) {

        var self = this;

        var ans = [];

        // Fix ds name
        ds = nx.util.toDatasetName(ds);

        // Assure array
        if (!Array.isArray(flds)) flds = [flds];
        // Assure dataset definition
        self._loadDataset(ds, function (dsdef) {
            // Assure view definition
            self._loadView(ds, view, function (viewdef) {

                // Get dataset fields
                var fields = self.__views[ds][view].fields;
                // Loop thru
                flds.forEach(function (fieldname) {
                    var fdef = fields[fieldname];
                    if (fdef) {
                        fdef.nxtype = 'hidden';
                        fdef._local = true;
                        // Add
                        ans.push(fdef);
                    }
                });
            });
        });

        return ans;
    },

    _alterView: function (ds, view, field, data, cb) {

        var self = this;

        // Fix ds name
        ds = nx.util.toDatasetName(ds);
        view = nx.util.toViewName(view);

        // Load the dataset
        self._loadDataset(ds, function (result) {
            // Get
            self._loadView(ds, view, function (vdef) {
                // Must have a value
                if (data) {
                    var fields = self.__views[ds][view].fields;
                    // Get the real view
                    vdef = fields[field];
                    // If none, insert
                    if (!vdef) {
                        fields[field] = data;
                    } else {
                        fields[field] = nx.util.merge(vdef, data);
                    }
                    if (cb) cb(fields[field]);
                } else {
                    if (cb) cb(fields[field]);
                }
            });
        });
    },

    _viewExplode: function (ds, view, cb) {

        var self = this;

        // Fix ds name
        ds = nx.util.toDatasetName(ds);

        // Get dataset
        self._loadDataset(ds, function (dsdef) {

            // Clone
            view = JSON.parse(JSON.stringify(view));
            // Based on?
            if (nx.util.hasValue(view.basedon)) {

                // Get the base
                self._loadView(ds, view.basedon, function (baseview) {

                    // Explode
                    self._viewExplode(ds, baseview, function (exploded) {

                        // Flag
                        exploded._id = view._id;

                        // Get list of excluded
                        var excluded = [];
                        // Any?
                        if (nx.util.hasValue(view.exclude)) {
                            excluded = nx.util.splitSpace(view.exclude);
                        }

                        // And new/replacement
                        var c_New = [];

                        //Loop thru
                        Object.keys(view.fields).forEach(function (sField) {

                            // Get the field
                            var c_Field = view.fields[sField];
                            //
                            c_New.push(c_Field.aoFld);

                        });

                        // Create use table
                        var c_Use = {};

                        // Copy new
                        Object.keys(view.fields).forEach(function (sField) {

                            // Get new
                            var c_Replacement = view.fields[sField];
                            // Skip replacements
                            if (c_Replacement.exclude !== 'y') {
                                // Get orig
                                var c_Orig = exploded.fields[sField];

                                // New?
                                if (!c_Orig) {
                                    // Copy
                                    exploded.fields[sField] = c_Replacement;
                                }
                            }
                        });

                        // Loop thru
                        var c_SFields = Object.keys(exploded.fields);
                        c_SFields.forEach(function (sField) {
                            // Get the field
                            var c_Orig = exploded.fields[sField];
                            // Make new
                            var c_Replacement = view.fields[sField];
                            // Copy
                            if (c_Replacement && c_Replacement.exclude !== 'y') {
                                exploded.fields[sField] = c_Replacement;
                            }

                            // Get the row
                            var iRow = c_Orig.top.toString();
                            // Make room in map
                            if (!c_Use[iRow]) {
                                c_Use[iRow] = 0;
                            }

                            //
                            if (excluded.indexOf(c_Orig.aoFld) !== -1) {
                                c_Use[iRow]--;
                                delete exploded.fields[c_Orig.aoFld];
                            }
                            else {
                                c_Use[iRow]++;
                            }
                        });

                        // Find the empty lines
                        var c_Rows = Object.keys(c_Use);
                        // Sort
                        c_Rows.sort(function (a, b) { return nx.util.toInt(a) - nx.util.toInt(b); });
                        // Start with nothing deleted
                        var iDeleted = 0;
                        // Loop thru
                        c_Rows.forEach(function (iRow) {
                            // Get the count
                            if (c_Use[iRow] <= 0) {
                                iDeleted++;
                            }
                            // Set
                            c_Use[iRow] = iDeleted;
                        });

                        // Loop thru
                        Object.keys(exploded.fields).forEach(function (sField) {

                            // Get
                            var c_Field = exploded.fields[sField];

                            // Adjust top
                            c_Field.top -= c_Use[c_Field.top];
                        });

                        //
                        if (cb) cb(self._viewLocalize(dsdef, exploded));
                    });
                });
            } else {
                if (cb) cb(self._viewLocalize(dsdef, view));
            }
        });
    },

    _viewLocalize: function (ds, view) {

        var self = this;

        // Fix ds name
        ds = nx.util.toDatasetName(ds);

        // Use original
        var ans = view;
        // Get the list
        var list = ans.fields || ans.items;
        // A dataset?
        // Loop thru
        Object.keys(list).forEach(function (name) {
            // Get field
            var field = list[name];

            // Reference to dataset
            if (ds) {
                var dsdef = ds.fields[field.aoFld] || {};
                if (field.nxtype === 'usedataset') {
                    field.nxtype = dsdef.nxtype;
                }
                field.linkds = dsdef.linkds;
                if (dsdef.choices) field.choices = nx.util.splitSpace(dsdef.choices);
                if (dsdef.gridview) field.gridview = dsdef.gridview;
            }
            field.nxtype = field.nxtype || 'string';

            // Tabs?
            if (nx.util.hasValue(field.gridview)) {

                // Assure type
                if (field.nxtype !== 'grid') field.nxtype = 'tabs';

            }
        });

        return ans;
    },

    _viewFields: function (fdefs, ddefs) {

        var self = this;

        // Assume none
        var ans = [];
        // Loop thru
        Object.keys(fdefs).forEach(function (name) {
            // Get field
            var field = fdefs[name];
            // Assure aoFld
            field.aoFld = field.aoFld || name;
            // Look in dsdef
            var dfld = ddefs.fields[field.aoFld];
            // Any?
            if (dfld) {
                // Set the choices
                if (nx.util.hasValue(dfld.choices)) {
                    field.choices = nx.util.splitSpace(dfld.choices, true);
                }
            }
            // Add
            ans.push(field);
        });
        return ans;
    },

    _viewEdit: function (view) {

        var self = this;

        // Make close
        var ans = Object.assign({}, view);
        // Loop thru
        Object.keys(ans.fields).some(function (fieldname) {
            // Get
            var field = ans.fields[fieldname];
            // If it is the ID, use
            if (field.aoFld === '_id') {
                // Set readonly
                field.ro = 'y';

                return true;
            }
            return false;
        });

        return ans;
    },

    _loadPick: function (ds, pick, cb, force) {

        var self = this;

        // Fix ds name
        ds = nx.util.toDatasetName(ds);

        // Assure space
        var defs = self.__pick[ds] || {};
        self.__pick[ds] = defs;

        // Do we have it already?
        if (defs[pick] && !force) {
            if (cb) cb(defs[pick]);
        } else {
            // Get the view
            nx.util.serviceCall('AO.PickListGet', {
                ds: ds,
                id: pick
            }, function (result) {

                // Any?
                if (result) {
                    // Save
                    defs[pick] = result;
                    if (cb) cb(defs[pick]);
                }
            });
        }

    },

    _getPick: function (ds) {

        var self = this;

        // Fix ds name
        ds = nx.util.toDatasetName(ds);

        // Assure space
        var defs = self.__pick[ds] || {};
        self.__pick[ds] = defs;

        return defs;

    },

    _updatePick: function (ds, pick, data) {

        var self = this;

        // Fix ds name
        ds = nx.util.toDatasetName(ds);

        // Assure space
        var defs = self.__pick[ds] || {};
        self.__pick[ds] = defs;

        // Save
        defs[pick] = data;

        // Save
        nx.util.serviceCall('AO.PickListSet', {
            ds: ds,
            id: pick,
            data: data
        }, nx.util.noOp);

    },

    __ds: {},
    __views: {},
    __pick: {},

    getChildWindows: function () {

        var self = this;

        var ans = [];

        // Loop thru
        var list = self.windowButtons.getChildren();
        list.forEach(function (widget) {
            // A window button?
            if (widget.window && widget.window._rendered) {
                //  Save
                ans.push(widget.window);
            }
        });

        return ans;

    },

    // ---------------------------------------------------------
    //
    // SIO EVENTS
    // 
    // ---------------------------------------------------------

    sendSIO: function (fn, msg, options) {

        var self = this;

        self.user.SIOSend(fn, msg, options);

    },

    processSIO: function (event) {

        var self = this;

        // See if sent to specific
        if (event && event.toUser && event.toUser !== self.user.getName()) event = null;
        if (event && event.toUUID) {
            if (self.user.SIO) {
                if (event.toUUID = self.user.SIO.uuid) event = null;
            }
        }

        // Handle global
        event = nx.SIO.process(event);

        // Handle a specific window
        if (event && event.toWinID) {
            var win = self.findWindow(event.toWinID);
            // Can process?
            if (win && win.processSIO) {
                // Found
                win.processSIO(win, event);
            }
            event = null;
        }

        // Do we still have a message?
        if (event) {
            // Loop thru
            self.getChildWindows().forEach(function (win) {

                // Can process?
                if (win.processSIO) {
                    // Found
                    win.processSIO(win, event);
                }

            });

            // And callbacks
            Object.keys(self.callbacksSIO).forEach(function (name) {
                self.callbacksSIO[name](name, event);
            });
        }

    },

    callbacksSIO: {},

    addSIO: function (name, cb) {

        var self = this;

        self.callbacksSIO[name] = cb;

    },

    removeSIO: function (name) {

        var self = this;

        delete self.callbacksSIO[name];
    },

    // ---------------------------------------------------------
    //
    // TP SUPPORT
    // 
    // ---------------------------------------------------------

    storageTP: {},

    addTP: function (name, obj) {

        var self = this;

        self.storageTP[name] = obj;

    },

    removeTP: function (name) {

        var self = this;

        delete self.storageTP[name];
    },

    getTP: function (name) {

        var self = this;

        return self.storageTP[name];
    },

    // ---------------------------------------------------------
    //
    // MANAGEMENT
    // 
    // ---------------------------------------------------------

    resetUser: function () {

        var self = this;

        // Clear the user
        self.user.assure();
        // Reset
        self.reset();
    },

    reset: function () {

        var self = this;

        // Get list of windows
        var list = self.getChildWindows();
        // Loop thru
        while (list.length) {
            // Close
            list[0].safeClose();
            // Refresh list
            list = self.getChildWindows();
        }

        // Make the menus
        self.buildMenus();
    },

    buildMenus: function () {

        var self = this;

        // Make the menu
        nx.setup.__component(self.startButton, {

            menu: self.user.getStartMenu()

        });

        // Make the menu
        nx.setup.__component(self.commandButton, {

            menu: self.user.getCommandMenu()

        });

    },

    getWidth: function () {

        var self = this;

        return self.root.getWidth() || window.innerWidth;
    },

    getHeight: function () {

        var self = this;

        return self.root.getHeight() || window.innerHeight;
    },

    updateClock: function (self) {

        // Assure
        self = self || nx.desktop;
        // Get the button
        var button = self.clockArea;
        button.setRich(true);

        // Get the date
        var now = new Date();
        //
        button.setValue('<div align="center">' + moment().format('dddd MMMM Do YYYY*h:mm a').replace('*', '<br/>') + '</div>');
        // Compute seconds till next minute
        var till = 60 - now.getSeconds();
        // Redo
        setTimeout(self.updateClock, till * 1000);

    },

    // ---------------------------------------------------------
    //
    // USER
    // 
    // ---------------------------------------------------------

    user: null,

    // ---------------------------------------------------------
    //
    // UTILITIES
    // 
    // ---------------------------------------------------------

    isCtrl: function () {
        return kd.CTRL.isDown()
    }
};

nx.env = {


    // ---------------------------------------------------------
    //
    // Storage
    // 
    // ---------------------------------------------------------

    /**
     * 
     * Gets item from storage
     * 
     * @param {any} key
     */
    getStore: function (key) {
        // Get the raw value
        var ans = localStorage.getItem(key);
        // Any?
        if (ans) {
            // Parse
            ans = JSON.parse(ans);
            // Get the value
            ans = ans.value;
        }
        return ans;
    },

    /**
     * 
     * Sets an item into storgae
     * 
     * @param {any} key
     * @param {any} value
     */
    setStore: function (key, value) {
        // Any?
        if (value === null) {
            localStorage.setItem(key, value);
        } else {
            // Make into object
            var wkg = {
                value: value
            };
            // Save
            localStorage.setItem(key, JSON.stringify(wkg));
        }
    },

    // ---------------------------------------------------------
    //
    // Private
    // 
    // ---------------------------------------------------------

    /**
     * 
     * Sets the "remember me" token
     * 
     * @param {any} rows
     */
    setRM: function (value) {

        var self = this;

        // Save
        self.setStore('rm', value || '');
    },

    /**
     * 
     * Gets the "remember me" token
     * 
     */
    getRM: function () {

        var self = this;

        var ans = self.getStore('rm');
        // Clear
        self.setRM('');

        return ans;

    }

}