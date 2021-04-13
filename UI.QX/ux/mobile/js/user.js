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

nx.user = {

    /**
     * The underlying object
     * 
     */
    aoobject: null,

    _toolIcons: {},

    /**
     * 
     * Assures correct format
     *
     */
    assure: function (aoobj, cb) {

        var self = this;

        // disconnect Socket.IO
        if (self.SIO) {

            // Tell world
            self.SIOSend('$$user.removed');

            self.SIO.disconnect();
            // Remove
            self.SIO = null;
        }

        //
        self.aoobject = aoobj;
        self.siteInfo = null;

        // 
        if (self.aoobject) {
            nx.aoobject.assureField(self.aoobject, 'name', nx.aoobject.getField(self.aoobject, '_id'));
            nx.aoobject.assureField(self.aoobject, 'pwd', '');

            //
            self.assureJSField('commands', []);
            self.assureJSField('datasets', {});
            self.assureJSField('groups', {});
            self.assureJSField('icons', []);
            self.assureJSField('docs', []);
            self.assureJSField('tools', []);
            self.assureJSField('selectors', []);
            self.assureJSField('sio', []);
            self.assureJSField('menu', []);
        }

        self.getSiteInfo(function () {

            // Handle the socket.io
            if (self.aoobject && self.getField('name')) {

                //
                if (!self.SIO) {

                    self.SIO = io({
                        'reconnection delay': 100, // defaults to 500
                        'reconnection limit': Infinity,
                        'max reconnection attempts': Infinity // defaults to 10
                    });
                    self.SIO.uuid = nx.util.uuid();
                    self.SIO.on('connection', async (socket) => {
                        var name = self.getField('sioid');
                        if (name && socket && socket.join) {
                            socket.join(name);
                        }
                    });
                    self.SIO.on('reconnect', async (socket) => {
                        var name = self.getField('sioid');
                        if (name && socket && socket.join) {
                            socket.join(name);
                        }
                    });

                    self.getField('sio').forEach(function (code) {
                        self.SIO.on(code, function (data) {
                            if (typeof data === 'string') {
                                data = JSON.parse(data);
                            }
                            if (data.message && typeof data.message === 'string') {
                                data.message = JSON.parse(data.message);
                            }
                            if (data.uuid !== self.SIO.uuid || data.allow) {
                                self.SIOProcess(data);
                            }
                        });
                    });
                }

                // Register!
                self.SIOSend('$$user.added');

                // Get extra stuff
                if (!self._timezones) {
                    nx.util.serviceCall('Office.GetTimeZones', {}, function (result) {
                        if (result && result.list) {
                            self._timezones = result.list;
                        }
                    });
                }

                // Get the users
                if (!self.getIsSelector('ACCT')) {
                    nx.util.serviceCall('AO.GetUsers', {}, function (result) {
                        if (result.list) {
                            self._allusers = result.list;
                        }
                    });
                }

                // Make the menu
                self._menu = nx.builder.menuBlock(self.getMenu());

                if (cb) cb();

            }
        });

    },

    useVirtual: function (name) {

        var self = this;

        // Make ao
        var ao = new o.aoobject();
        // Phone values
        ao.values = {
            name: name,
            openmode: 'stack',
            openmodechild: 'right',
            _changes: []
        };
        // Set
        self.assure(ao);

    },

    // ---------------------------------------------------------
    //
    // Access
    // 
    // ---------------------------------------------------------

    getField: function (field) {

        var self = this;

        // Assume none
        var ans;
        // Do we have an object?
        if (self.aoobject) {
            // Get
            ans = nx.aoobject.getField(self.aoobject, field);
        }

        return ans;
    },

    setField: function (field, value) {

        var self = this;

        // Assume none
        var ans;
        // Do we have an object?
        if (self.aoobject) {
            // Get
            ans = nx.aoobject.setField(self.aoobject, field, value);
        }

        return ans;
    },

    loadField: function (field, value) {

        var self = this;

        // Assume none
        var ans;
        // Do we have an object?
        if (self.aoobject) {
            // Get
            ans = nx.aoobject.setField(self.aoobject, field, value || self.getField(field));
        }

        return ans;
    },

    getSIField: function (field) {

        var self = this;

        // Assume none
        var ans;
        // Do we have an object?
        if (self.siteInfo) {
            // Get
            ans = self.siteInfo[field];
        }

        return ans;
    },

    assureJSField: function (field, defval) {

        var self = this;

        // Get the field
        var wkg = self.getField(field);
        if (!wkg) {
            // If none, use default
            self.setField(field, defval);
        } else if (typeof wkg === 'string') {
            // Parse
            self.setField(field, JSON.parse(wkg));
        }
    },

    getName: function () {

        var self = this;

        return self.getField('name');

    },

    getOpenMode: function () {

        var self = this;

        return self.getField('openmode');

    },

    getOpenModeChild: function () {

        var self = this;

        return self.getField('openmodechild');

    },

    setValues: function (data) {

        var self = this;

        // Do we have an object?
        if (self.aoobject && data) {
            // Loop thru
            Object.keys(data).forEach(function (field) {
                // Set
                ans = nx.aoobject.setField(self.aoobject, field, data[field]);
            });
        }

    },

    getIcons: function () {

        var self = this;

        // Get the menu
        var ans = self.__prebuilticons;

        // Do we have a menu?
        if (!ans) {
            // Get list
            var list = self.getField('icons');
            if (list && list.length) {
                // Make
                self.__prebuilticons = ans = [];

                // Loop thru
                list.forEach(function (entry) {
                    ans.push({
                        label: entry,
                        icon: entry
                    });
                });
            }
        }
        return ans;
    },

    getDocs: function () {

        var self = this;

        // Get the menu
        var ans = self.__prebuiltdocs;

        // Do we have a menu?
        if (!ans) {
            // Make
            self.__prebuiltdocs = ans = [];
            // Get list
            var list = self.getField('docs');
            // Loop thru
            list.forEach(function (entry) {
                ans.push(entry);
            });
        }
        return ans;
    },

    getIsAccount: function () {

        var self = this;

        return self.getField('selectors').indexOf('ACCT') != -1;

    },

    getIsSelector: function (selector) {

        var self = this;

        var selectors = self.getField('selectors');

        return selectors.indexOf(selector) !== -1 || (selectors.indexOf('ALL') !== -1 && selector !== 'ACCT');
    },

    // ---------------------------------------------------------
    //
    // Options
    // 
    // ---------------------------------------------------------

    geteMailName: function () {
        return this.getField('emailname');
    },

    geteMailPwd: function () {
        return this.getField('emailpwd');
    },

    geteMailProvider: function () {
        return this.getField('emailprov');
    },

    getTwilioPhone: function () {
        return this.getField('twiliophone');
    },

    // ---------------------------------------------------------
    //
    // SocketIO
    // 
    // ---------------------------------------------------------

    SIOSend: function (fn, msg, options) {

        var self = this;

        if (self.SIO && self.getField('sioid')) {

            // Get the channel
            var poss = self.getField('sio');
            var channel;
            if (poss.length > 0) {
                if (nx.util.startsWith(fn, '$$user.')) {
                    channel = poss[0];
                } else if (poss.length > 1) {
                    channel = poss[1];
                }
            }
            // Can we send?
            if (channel) {
                // Make payload
                var payload = {
                    fn: fn,
                    uuid: self.SIO.uuid,
                    user: self.getField('sioid'),
                    message: msg || {}
                };
                if (options) {
                    payload = nx.util.merge(options, payload);
                }

                // Send
                self.SIO.emit(channel, payload);
            }

        }
    },

    SIOProcess: function (msg) {

        var self = this;

        // Must have a message
        if (msg && typeof msg === 'object') {

            // According to function
            switch (msg.fn) {

                case '$$noti':
                    // Is it for us?
                    if (msg.message.to === self.getField('sioid') || msg.message.to === '*') {
                        // Is it a valid type?
                        var fn = 'notify' + msg.message.type;
                        if (nx.util[fn]) {
                            // Make the message
                            var xmsg = (msg.user ? '[' + msg.user + '] ' : '') + msg.message.msg;
                            // Show
                            nx.util[fn](xmsg, {
                                from: msg.user
                            });
                        }
                    }
                    break;

                case '$$data.app':
                    // Is it ours?
                    if (msg.message.to === self.SIO.uuid) {
                        // Any?
                        if (msg.message.win == nx.env.getWinID()) {
                            // Set
                            nx.fields.setFormData(msg.message.data);
                        }
                    }
                    break;

                case '$$user.check':
                    if (self.SIOUsers.indexOf(msg.user) === -1) {
                        self.SIOUsers.push(msg.user);
                    }
                    break;

                case '$$user.added':
                    // Is it us?
                    if (msg.user === self.getField('sioid')) {

                        // Create null user
                        nx.env.reset();
                        //
                        nx.util.notifyInfo('You logged on elsewhere...');

                    } else {
                        // Add if new
                        if (self.SIOUsers.indexOf(msg.user) === -1) {
                            self.SIOUsers.push(msg.user);
                        }
                        // Let them know us
                        self.SIOSend('$$user.check', {
                            to: msg.user
                        });
                    }
                    break;

                case '$$user.removed':
                    // Remove if found
                    var pos = self.SIOUsers.indexOf(msg.user);
                    if (pos !== -1) self.SIOUsers.splice(pos, 1);
                    break;

                case '$$changed.dataset':
                    // Get the dataset
                    var ds = msg.message.ds;
                    // If none, just reload
                    if (msg.message.deleted === 'y') {
                        nx.db._removeDataset(ds);
                        // Load start menu
                        self._reloadUser();

                        nx.util.notifyInfo('Dataset ' + ds + ' has been deleted');
                    } else {
                        // Reload
                        nx.db._loadDataset(ds, function () {
                            // Load start menu
                            self._reloadUser();

                            nx.util.notifyInfo('Dataset ' + ds + ' has been updated');
                        }, true);
                    }
                    break;

                case '$$changed.view':
                    // Get the dataset and view
                    var ds = msg.message.ds;
                    var view = msg.message.view;
                    // If none, just reload
                    if (msg.message.deleted === 'y') {
                        self._reloadUser();

                        nx.util.notifyInfo('View ' + ds + '/' + view + ' has been deleted');
                    } else {
                        // Reload
                        nx.db._loadView(ds, view, function () {
                            // Load start menu
                            self._reloadUser();

                            nx.util.notifyInfo('View ' + ds + '/' + view + ' has been updated');
                        }, true);
                    }
                    break;

                case '$$changed.userprofile':
                    // Get the user
                    var user = msg.message.id;
                    // Us?
                    if (user === nx.user.getName()) {
                        // Load start menu
                        self._reloadUser();

                        nx.util.notifyInfo('Personal settings have been updated');
                    }
                    break;

                case '$$changed.systemprofile':
                    if (msg.message.id === '_info') {
                        self.getSiteInfo();
                        // Load start menu
                        self._reloadUser();
                        nx.util.notifyInfo('Site Settings have been updated');
                    }
                    break;

                case '$$changed.templates':
                    // Load start menu
                    self._reloadUser();

                    nx.util.notifyInfo('Document templates have been updated');
                    break;

                case '$$chat.open':
                    // Get the root window
                    var winid = msg.message.chat.substr(msg.message.chat.indexOf('_') + 1);
                    if (winid !== nx.env.getWinID()) {
                        // Comfirm
                        nx.util.confirm('You are being invited...', 'Join ' + msg.user + ' in a chat re: ' + win.getCaption(), function (ok) {
                            //
                            nx.calls.chat({
                                desc: win.getCaption(),
                                win: winid,
                                caller: win,
                                response: true
                            });
                        });
                    }
                    break;

                case '$$chat.invite':
                    // Us?
                    if (msg.message.to === nx.user.getName()) {
                        // Already on the chat?
                        if (msg.message.chat !== nx.env.getWinID()) {
                            // Get the root window
                            var winid = msg.message.chat.substr(msg.message.chat.indexOf('_') + 1);
                            if (winid === nx.env.getWinID()) {
                                // Comnirm
                                nx.util.confirm('You are being invited...', 'Join ' + msg.user + ' in a chat re: ' + win.getCaption(), function (ok) {
                                    //
                                    nx.calls.chat({
                                        desc: win.getCaption(),
                                        win: winid,
                                        caller: win,
                                        response: true
                                    });
                                });
                            } else {
                                // Comfirm
                                nx.util.confirm('You are being invited...', 'Join ' + msg.user + ' in a chat re: ' + msg.message.desc, function (ok) {
                                    //
                                    nx.calls.chat({
                                        desc: msg.message.desc,
                                        win: winid,
                                        response: true
                                    });
                                });
                            }
                        }
                    }
                    break;

                case '$$object.saved':
                    if (msg.message.ds === '_user') {
                        if (self._allusers.indexOf(msg.message.id) === -1) {
                            self._allusers.push(msg.message.id);
                        }
                    }

                    self.updatePick(msg.message);
                    break;

                case '$$object.deleted':
                    if (msg.message.ds === '_user') {
                        var pos = self._allusers.indexOf(msg.message.id);
                        if (pos !== -1) self._allusers.splice(pos, 1);
                    }

                    self.updatePick(msg.message);
                    break;

                case '$$object.init':
                    // Get the message
                    var msg = msg.message;
                    if (msg.winid === nx.env.getBucketItem('_ao')) {
                        // Get object
                        var obj = nx.env.getBucketItem('_obj');
                        if (obj) {
                            // Get the changes
                            var changes = obj._changes || [];
                            changes.forEach(function (fld) {
                                nx.user.SIOSend('$$object.data', {
                                    aofld: fld,
                                    winid: msg.winid,
                                    value: obj[fld]
                                });
                            });
                        }
                    }
                    break;

                case '$$object.data':
                    // Get the message
                    var msg = msg.message;
                    // Must have one
                    if (msg) {
                        // Parse 
                        var pieces = msg.winid.split('_');
                        // Do we have it?
                        nx.db.mapObj(pieces[1], pieces[2], function (obj) {
                            // Save
                            if (nx.db.objSetField(obj, msg.aofld, msg.value)) {

                                // Is is the active window?
                                if (msg.winid === nx.env.getBucketItem('_ao')) {
                                    // Map
                                    var poss = $('[name=' + msg.aofld + ']');
                                    // Set
                                    nx.fields.set(poss, msg.value);
                                }

                            }
                        });
                    }
                    break;

                case '$$changed.document':
                    //
                    self.updateDocuments(msg.message);
                    break;
            }

            // And callbacks
            self.processSIO(msg);
        }
    },

    processSIO: function (event) {

        var self = this;

        // Do we still have a message?
        if (event) {
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

    SIOUsers: [],

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
    // Support
    // 
    // ---------------------------------------------------------

    /**
     * 
     * Log into the system
     * 
     * @param {any} name
     * @param {any} pwd
     * @param {any} rm
     */
    login: function (name, pwd, rm) {

        var self = this;

        // Do the load
        if (name) {

            // Call
            nx.util.serviceCall('Access.Login', {
                user: name,
                pwd: pwd,
                rm: rm
            }, function (result) {

                self.processLogin(name, pwd, result);

            });

        }
    },

    processLogin: function (name, pwd, result) {

        var self = this;

        if (!result || !result._id) {

            // Reset user
            self.assure(null);

            nx.util.notifyError('Invalid name and/or password');

            // Call login
            nx.calls.login();

        } else {

            // Save the remember me token
            nx.env.setRM(result._rm);

            // Make the object
            var user = nx.aoobject.prep(result);
            // Fill
            self.assure(user, function () {

                // Say hello
                nx.util.notifyOK('Welcome ' + self.getName());

                // Call login
                nx.calls.login();

            });

        }

    },

    /**
     * 
     * Get list of datasets
     * 
     * @param {string} ds
     */
    getDSInfo: function (ds) {

        var self = this;

        // Assume none
        var ans = null;
        // Do we have a user?
        if (self.aoobject) {
            // Get
            ans = self.getField('datasets');
            // Assure object
            if (typeof ans === 'string') {
                ans = JSON.parse(ans);
            }
        }
        // Assure
        ans = ans || {};
        // Filter if dataset given
        if (ds) {
            ans = ans[ds] || {};
        }

        return ans;
    },

    /**
     * 
     * Returns the menu for Start button
     * 
     */
    getStartMenu: function () {

        var self = this;

        // Assume none
        var ans = [];
        // Do we have a user?
        if (self.aoobject) {

            // Do we have a name?
            if (self.getField('name')) {

                // From login template
                ans = self.getField('menu');
                if (ans) {
                    // Assure object
                    if (typeof ans === 'string') {
                        ans = JSON.parse(ans);
                    }
                }

            }

        } else {

            // The login menu
            ans = [
                {
                    label: 'Login',
                    tool: 'Login',
                    icon: 'accept'
                }
            ];

        }

        return ans;
    },

    _reloadUser: function () {

        var self = this;

        // Get the updated start menu
        nx.util.serviceCall('Office.GetStartMenu', {
            name: self.getField('name'),
            allowed: self.getField('allowed'),
            reload: 'y'
        }, function (result) {
            self.loadField('commands', result.commands);
            self.loadField('datasets', result.datasets);
            self.loadField('groups', result.groups);
            self.loadField('icons', result.icons);
            self.loadField('docs', result.docs);
            self.loadField('tools', result.tools);
            self.loadField('selectors', result.selectors);
            self.loadField('sio', result.sio);
            self.loadField('menu', result.menu);

            //
            self.assureJSField('commands', []);
            self.assureJSField('datasets', {});
            self.assureJSField('groups', {});
            self.assureJSField('icons', []);
            self.assureJSField('docs', []);
            self.assureJSField('tools', []);
            self.assureJSField('selectors', []);
            self.assureJSField('sio', []);
            self.assureJSField('menu', []);

            // Reload the menu
            self._menu = nx.builder.menuBlock(self.getMenu());
            //
            if (nx.env.getWinID() === 'menu') {
                // Reset
                nx.calls.menu();
            }
        });

    },

    /**
     * 
     * Returns a modified main menu
     * 
     */
    getMenu: function () {

        var self = this;

        //
        var ans = [].concat(self.getField('menu'));

        // Time track
        var tt = self.getField('commands');
        if (tt && tt.length) {
            // Look for last divider
            var pos = ans.length - 1;
            ans.forEach(function (entry, index) {
                if (typeof entry === 'string' && entry === '-') {
                    pos = index;
                }
            });

            // Add spacer
            ans.splice(pos++, 0, '-');
            // Loop thru
            tt.forEach(function (entry) {

                // Assure segs is object
                if (typeof entry.segs === 'string') {
                    entry.segs = JSON.parse(entry.segs);
                }

                var label = entry.desc;
                if (entry.user !== nx.user.getName()) {
                    label = nx.builder.badge(entry.user, 'red') + ' ' + label;
                }

                ans.splice(pos++, 0, {
                    timer: entry.from,
                    label: label,
                    icon: entry.icon,
                    obj: entry.obj
                });
            });

        }

        // Add settings
        ans.splice(ans.length - 1, 0, {
            tool: 'Settings',
            label: 'Settings',
            icon: 'cog'
        });

        return ans;
    },

    /**
     * 
     * 
     * Returns the time track entry for an object
     * 
     * @param {any} obj
     */
    getTTItem: function (obj) {

        var self = this;

        var ans;

        //
        var tt = self.getField('commands');
        // Loop thru
        tt.forEach(function (entry) {
            if (entry.obj === obj) {
                ans = entry;
            }
        });

        return ans;
    },

    /**
     * 
     * 
     * Returns true if operation is allowed
     * 
     * OPS:
     * 
     * a - Add
     * v - View
     * x - Delete
     * d - Documents
     * c - Calendar
     * r - Reports
     * o - Tools
     * t - Tasks
     * z - Analyze
     * h - Hide in Start menu
     * 
     * @param {string} ds
     * @param {string} op
     */
    opAllowed: function (ds, op, dsfld) {

        var self = this;

        // Fix ds name
        ds = nx.util.toDatasetName(ds);

        // Assume not
        var ans = false;

        //
        if (typeof dsfld !== 'undefined' && dsfld !== 'y') {
        } else {
            // Get the dataset
            var dss = self.getDSInfo();
            // Look for dataset
            if (dss[ds]) {
                // Get the privileges
                var ops = dss[ds].privileges;
                // Any?
                if (ops) {
                    // Match the op
                    ans = ops.indexOf(op) !== -1 || ops.indexOf('*') !== -1;
                }
            }
        }

        return ans;
    },

    /**
     * 
     * Returns site information
     * 
     * @param {any} cb
     */
    getSiteInfo: function (cb) {

        var self = this;

        // Get
        nx.db.getObj('_sys', '_info', function (result) {

            // Save
            self.siteInfo = result;

            // Do we have a help?
            if (nx.util.hasValue(self.getSIField('helproot'))) {
                // Add the call
                nx.calls.help = function (req) {
                    // Make the url
                    var url = '/help/' + self.getSIField('helproot') + '.md';
                    // Show
                    nx.util.urlPopup(url);
                }
            }

            // Call
            if (cb) cb(result);

        });

    },

    /**
     * 
     * Updates the pick list if parameters match message
     * 
     * @param {any} msg
     */
    updatePick: function (msg) {

        var self = this;

        // Only current
        self.refreshPickList(null, msg);
        //// Loop thru
        //nx.office.history().forEach(function(url) {
        //    self.refreshPickList(url, msg);
        //});
    },

    updateDocuments: function (msg) {

        var self = this;

        // Only current
        self.refreshDocuments(null, msg);
        //// Loop thru
        //nx.office.history().forEach(function(url) {
        //    self.refreshDocuments(url, msg);
        //});
    },

    /**
     * 
     * Updates the pick list
     * 
     * */
    refreshPickList: function (url, msg) {

        var self = this;

        // Do
        var win = nx.env.getBucketItem('winid', url);
        var ds = nx.env.getBucketItem('ds', url);
        var bucketid = nx.env.getBucketID(url);
        var search = nx.env.getBucketItem('_search', url);

        //
        var ds = nx.env.getBucketItem('ds');

        // Match?
        if (!msg || (win === 'pick' && ds === msg.ds)) {

            // Get the list
            var list = $('#' + nx.builder.pickListID(ds, bucketid));

            // Refresh
            nx.db._loadDataset(ds, function (dsdef) {

                // Setup the filter
                var filter = [];

                // Get the search
                if (search) {

                    filter.push({
                        field: '_desc',
                        op: 'Any',
                        value: search
                    });

                }

                // Per user?
                if (nx.util.hasValue(dsdef.privField) && dsdef.privAllow === 'y' && !self.getIsSelector('MGR')) {
                    // Assure
                    filter.push({
                        field: dsdef.privField,
                        op: 'Eq',
                        value: nx.user.getName()
                    });
                }

                // Pick list
                var pl = nx.db.getPick(ds);
                if (pl) {
                    // Loop thru
                    Object.keys(pl).forEach(function (key) {
                        // Process
                        var qry = nx.db.processPickToolbarItem(pl[key]);
                        if (qry) {
                            filter.push(qry);
                        }
                    })
                }

                // Get the data
                nx.db.get(ds, filter, 0, nx.env.getRows(), '_desc', null, '_id _desc', function (data) {

                    list.html(nx.builder.picklist(ds, data, bucketid));

                });

            });
        }

    },

    /**
     * 
     * Updates the documents tree
     * 
     * */
    refreshDocuments: function (url, msg) {

        var self = this;

        // Do
        var win = nx.env.getBucketItem('winid', url);
        var ds = nx.env.getBucketItem('ds', url);
        var id = nx.env.getBucketItem('id', url);
        var bucketid = nx.env.getBucketID(url);;

        // Match?
        if (win === 'documents' && ds === msg.ds && id === msg.id) {
            // Get the list
            var list = $('#' + nx.builder.pickListID(ds, bucketid))

            nx.util.serviceCall('Docs.DocumentList', {
                ds: ds,
                id: id
            }, function (result) {

                // Update
                list.html(nx.builder.documents(ds, result.list, bucketid));

            });
        }

    }
};