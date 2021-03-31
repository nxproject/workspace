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

qx.Class.define('o.user', {

    extend: o.object,

    construct: function () {

        // Remove the req
        var req = Array.prototype.shift.call(arguments);

        // Call base
        this.base(arguments);

    },

    members: {

        /**
         * The underlying object
         * 
         */
        aoobject: null,

        /**
         * 
         * Assures correct format
         *
         */
        assure: function (aoobj) {

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
                self.aoobject.assureField('name', self.aoobject.getField('_id'));
                self.aoobject.assureField('pwd', '');

                //
                self.assureJSField('commands', []);
                self.assureJSField('datasets', {});
                self.assureJSField('groups', {});
                self.assureJSField('icons', []);
                self.assureJSField('docs', []);
                self.assureJSField('tools', []);
                self.assureJSField('selectors', []);
                self.assureJSField('sio', []);

                self.getSiteInfo();
            }

            // Handle the socket.io
            if (self.aoobject && self.getField('name')) {

                //
                var sioid = self.getField('name');

                if (!self.SIO) {

                    self.SIO = io({
                        'reconnection delay': 100, // defaults to 500
                        'reconnection limit': Infinity,
                        'max reconnection attempts': Infinity // defaults to 10
                    });
                    self.SIO.uuid = nx.util.uuid();
                    self.SIO.on('connection', async (socket) => {
                        var name = self.getField('sioid');
                        if (name) {
                            socket.join(name);
                        }
                    });
                    self.SIO.on('reconnect', async (socket) => {
                        var name = self.getField('sioid');
                        if (name) {
                            //self.SIO.join(name);
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

                // Handle autostart tool
                var as = self.getField('tool');
                if (as) {
                    var asp = self.getField('toolparams') || {};
                    if (typeof asp === 'string') {
                        asp = JSON.parse(asp);
                    }
                    // Call
                    nx.util.runTool(as, asp);
                }

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

            } else {
                $('title').text('NX.Project');
            }

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
                ans = self.aoobject.getField(field);
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
                ans = self.aoobject.setField(field, value);
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
                ans = self.aoobject.setField(field, value || self.getField(field));
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
                ans = self.siteInfo.getField(field);
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
                    ans = self.aoobject.setField(field, data[field]);
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
                    case '$$document.view':
                        // Is it for us?
                        if (msg.message.to === '*' || msg.message.to === self.getField('sioid') || msg.message.to === self.getName()) {
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
                            // Display files
                            if (msg.message.att) {
                                // Assure array
                                var path = msg.message.att;
                                if (!Array.isArray(path)) path = [path];
                                // Loop thru
                                path.forEach(function (entry) {
                                    nx.fs.view({
                                        path: entry
                                    });
                                });
                            }
                        }
                        break;

                    case '$$data.app':
                        // Is it ours?
                        if (msg.message.to === self.SIO.uuid) {
                            // Find the window
                            var win = nx.desktop.findWindow(msg.message.win);
                            // Any?
                            if (win) {
                                // Set
                                win.setFormData(msg.message.data);
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
                            nx.desktop.resetUser();
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
                            // Remove from cache
                            nx.desktop._removeDataset(ds);
                            // Load start menu
                            self._loadStartMenu();

                            nx.util.notifyInfo('Dataset ' + ds + ' has been deleted');
                        } else {
                            // Reload
                            nx.desktop._loadDataset(ds, function () {
                                // Load start menu
                                self._loadStartMenu();

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
                            self._loadStartMenu();

                            nx.util.notifyInfo('View ' + ds + '/' + view + ' has been deleted');
                        } else {
                            // Reload
                            nx.desktop._loadView(ds, view, function () {
                                // Load start menu
                                self._loadStartMenu();

                                nx.util.notifyInfo('View ' + ds + '/' + view + ' has been updated');
                            }, true);
                        }
                        break;

                    case '$$changed.userprofile':
                        // Get the user
                        var user = msg.message.id;
                        // Us?
                        if (user === nx.desktop.user.getName()) {
                            // Load start menu
                            self._loadStartMenu();

                            nx.util.notifyInfo('Personal settings have been updated');
                        }
                        break;

                    case '$$changed.systemprofile':
                        if (msg.message.id === '_info') {
                            self.getSiteInfo();
                            // Load start menu
                            self._loadStartMenu();
                            nx.util.notifyInfo('Site Settings have been updated');
                        }
                        break;

                    case '$$changed.templates':
                        // Load start menu
                        self._loadStartMenu();

                        nx.util.notifyInfo('Document templates have been updated');
                        break;

                    case '$$chat.open':
                        // Get the root window
                        var winid = msg.message.chat.substr(msg.message.chat.indexOf('_') + 1);
                        var win = nx.desktop.findWindow(winid);
                        if (!win) {
                            // Comfirm
                            nx.util.confirm('You are being invited...', 'Join ' + msg.user + ' in a chat re: ' + win.getCaption(), function (ok) {
                                //
                                nx.util.runTool('Chat', {
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
                        if (msg.message.to === nx.desktop.user.getName()) {
                            // Already on the chat?
                            var win = nx.desktop.findWindow(msg.message.chat);
                            if (!win) {
                                // Get the root window
                                var winid = msg.message.chat.substr(msg.message.chat.indexOf('_') + 1);
                                win = nx.desktop.findWindow(winid);
                                if (win) {
                                    // Comfirm
                                    nx.util.confirm('You are being invited...', 'Join ' + msg.user + ' in a chat re: ' + win.getCaption(), function (ok) {
                                        //
                                        nx.util.runTool('Chat', {
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
                                        nx.util.runTool('Chat', {
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
                        break;

                    case '$$object.deleted':
                        if (msg.message.ds === '_user') {
                            var pos = self._allusers.indexOf(msg.message.id);
                            if (pos !== -1) self._allusers.splice(pos, 1);
                        }
                        break;

                    case '$$object.view':
                        break;
                }

                // Propagate
                nx.desktop.processSIO(msg);

            }
        },

        SIOUsers: [],

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

                    // Save remember me token
                    nx.env.setRM(result._rm);

                    self.processLogin(result);

                });

            }
        },

        processLogin: function (result) {

            var self = this;

            if (!result || !result._id) {

                // Reset user
                self.assure(null);

                nx.util.notifyError('Invalid name and/or password');

            } else {

                // Make the object
                var user = new o.aoobject();
                // Save the values
                user.prep(result);
                // Fill
                self.assure(user);

                // Say hello
                nx.util.notifyOK('Welcome ' + nx.desktop.user.getName());
            }

            // Reset desktop
            nx.desktop.reset();

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

                // Run it
                setTimeout(function () {
                    nx.util.runTool('Login');
                }, 200);

            }

            return ans;
        },

        _loadStartMenu: function () {

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

                nx.desktop.buildMenus();
            });

        },

        /**
         * 
         * Return the command menu
         * 
         */
        getCommandMenu: function () {

            var self = this;

            // Assume none
            var ans = [];

            // Do we have a user?
            if (self.aoobject) {
                // Get
                var wkg = self.getField('commands');
                // Process
                wkg.forEach(function (entry, index) {
                    // According to type
                    switch (entry.type) {
                        case 'pin':
                            var status = entry.status;
                            var elap = nx.util.elapsedTime(new Date(entry.ts), parseFloat(entry.size || '0') * 1000);
                            var tt = [];

                            if (entry.user === nx.desktop.user.getName()) {

                                if (entry.status === 'Frozen') {
                                    status += ' @ ' + elap;
                                    tt.push({
                                        label: 'Continue',
                                        icon: 'flag_green',
                                        click: function (e) {
                                            nx.tt.fromEntry(entry, 'continue');
                                        }
                                    });
                                } else {
                                    status = elap;
                                    tt.push({
                                        label: 'Freeze',
                                        icon: 'flag_red',
                                        click: function (e) {
                                            nx.tt.fromEntry(entry, 'freeze');
                                        }
                                    });
                                }
                                tt.push({
                                    label: 'End',
                                    icon: 'stop',
                                    click: function (e) {
                                        nx.tt.fromEntry(entry, 'stop');
                                    }
                                });
                            }

                            var items = [{
                                label: 'View',
                                icon: 'monitor',
                                click: function (e) {
                                    nx.desktop.addWindowDS({
                                        ds: entry.ds,
                                        id: entry.id
                                    });
                                }
                            }];

                            if (tt.length) {
                                items.push({
                                    label: 'Time track',
                                    icon: 'star',
                                    items: tt
                                });
                            }

                            var label = entry.desc;
                            if (entry.user !== nx.desktop.user.getName()) {
                                label = entry.user + ' -- ' + label;
                            }

                            ans.push({
                                label: label + ' -- ' + status,
                                icon: entry.icon,
                                items: items
                            });
                            break;
                    }
                });

                // Time track
                if (nx.desktop.user.getSIField('ttenabled') === 'y') {
                    if (ans.length) {
                        ans.push('-');
                        ans.push({
                            label: 'Freeze all',
                            icon: 'stop',
                            click: function (e) {
                                //
                                nx.util.serviceCall('AO.Tag', {
                                    user: nx.desktop.user.getName(),
                                    type: 'pin',
                                    action: 'eod'
                                }, function (result) {
                                    // Reload
                                    nx.desktop.user._loadStartMenu();
                                    // Tell user
                                    nx.util.notifyInfo('Tag ' + result.value);
                                });
                            }
                        });
                    }
                }
            }

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

        getSiteInfo: function (cb) {

            var self = this;

            // Get
            nx.desktop.aomanager.get('_sys', '_info', function (result) {

                // Save
                self.siteInfo = result;

                // Change the tab
                $('title').text(self.getSIField('name') || 'NX.Project');

                // Call
                if (cb) cb(result);

            });

        }

    }

});