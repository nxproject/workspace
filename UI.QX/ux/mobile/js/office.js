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

/**
 * Create namespace
 */

window.nx = {

    _routes: [],

    calls: {},

    //_callbacks: {}

};

// Self
nx.office = {

    /**
     *
     * Initialize page
     *
     */
    init: function () {

        // Params
        nx._params = new URLSearchParams(window.location.search);

        // And load rest of system
        nx.office.loadMany([

            'tp/underscore-min',
            'tp/notiflix-aio',
            'tp/socket-io',
            'tp/moment',
            'tp/md5-min',
            'tp/parser-min',

            'js/env',
            'js/util',
            'js/aomanager',
            'js/aoobject',
            'js/user',
            'js/builder',
            'js/fmts',
            'js/fields',
            'js/db',
            'js/tt',
            'js/cm',
            'js/web',
            'js/fs',

            'routes/atstart',
            'routes/login',
            'routes/pick',
            'routes/view',
            'routes/menu',
            'routes/chat',
            'routes/usersettings',
            'routes/settings',
            'routes/logout',
            'routes/documents',
            'routes/timetrack',
            'routes/organizer',
            'routes/merge',
            'routes/atend'

        ], function () {

            // Look for camera to setup
            nx.util.hasCamera();

            // If your using custom DOM library, then save it to $$ variable
            nx.$$ = Dom7;

            // Initialize Framework7
            nx._sys = new Framework7(
                {
                    root: '#app',                      // From index.html
                    name: 'NX.Workspace',                  // App name
                    theme: 'auto',                      // Automatic theme detection

                    // Routing
                    routes: nx._routes,

                    on: {

                        // History
                        routeChanged: function (newRoute, previousRoute, router) {

                            // Do the setup
                            var fn = nx.env.getBucketItem('_onSetup', newRoute.url);
                            if (fn) {
                                fn(newRoute.url);
                            }

                            // Only if logged in
                            if (nx.user._menu) {
                                // History
                                var history = [].concat(nx.user._menu);
                                var delim = nx.util.isAndroid();
                                var added = false;
                                //
                                var drops = nx.office.history();
                                for (var i = drops.length - 2; i >= 0; i--) {
                                    // 
                                    var url = drops[i];
                                    // Get info
                                    var icon = nx.env.getBucketItem('_hicon', url);
                                    var prefix = nx.env.getBucketItem('_hprefix', url);
                                    var fld = nx.env.getBucketItem('_hfld', url);
                                    var label;
                                    // Build
                                    if (fld) {
                                        label = (nx.env.getBucketItem(fld, url) || '');
                                    }
                                    // Any?
                                    if (prefix || label) {
                                        if (!delim) {
                                            history.push(nx.builder.menuDivider());
                                        }
                                        delim = true;
                                        // Add
                                        history.push(nx.builder.menuItemCall(prefix, icon, "nx.office.goBack('" + url + "');", null, label));
                                        added = true;
                                    }
                                }

                                if (nx.util.isAndroid() && added) {
                                    history.push(nx.builder.menuDivider());
                                }

                                // And add to panel
                                nx.office.panelLeft(nx.builder.scrollable(nx.builder.menu(history)));

                                // Call for changes
                                var win = nx.env.getBucketItem('_ao');
                                if (win) {
                                    nx.user.SIOSend('$$object.init', {
                                        winid: win
                                    });
                                }

                                // Find all of the fields
                                $('[_onsetup=link]').each(function (index, ele) {
                                    //
                                    var poss = $(ele);
                                    // Only once
                                    poss.removeAttr('_onsetup');
                                    // Get the value
                                    var value = poss.attr('_value');
                                    // Parse
                                    var wkg = nx.db.parseID(value);
                                    // Call
                                    nx.db.get(wkg.ds, [{
                                        field: '_id',
                                        op: 'Eq',
                                        value: wkg.id
                                    }], 0, 1, '_desc', null, '_id _desc', function (data) {
                                        if (data && data.length) {
                                            // Fill
                                            poss.val(data[0]._desc);
                                        }
                                    });
                                });

                                // Find all of the fields
                                $('[_onsetup=textarea]').each(function (index, ele) {
                                    //
                                    var poss = $(ele);
                                    // Only once
                                    poss.removeAttr('_onSetup');
                                    // Get the value
                                    var value = poss.attr('_value');
                                    // Fill
                                    poss.val(value);
                                });

                                nx.office.updateTimers();

                            }

                        }
                    }
                }
            );

            // Setup
            nx.env.init();

            // Show the login
            nx.calls.login();
        });
    },

    /**
     * 
     * Displays a page
     * 
     * @param {any} page
     */
    goTo: function (name, req) {

        var self = nx.office;

        // Close panel
        self.panelRightClose();
        self.panelLeftClose();

        // Clear 
        if (nx._sys.popup) {
            // Get it
            var pu = nx._sys.popup.get();
            if (pu) pu.close();
        }
        if (nx._sys.dialog) {
            //Get
            var dg = nx._sys.dialog.get();
            if (dg) dg.close();
        }
        if (nx._sys.popover) {
            // Get it
            var pu = nx._sys.popover.get();
            if (pu) pu.close();
        }

        // Save
        req = req || {};
        req._bucket = nx.env.createBucket(req);

        // Assure request
        req = req || {};
        // Addure winid
        if (!req.winid) req.winid = name;

        // The page comes first
        var url = '/' + name + '/?_bucket=' + req._bucket;

        // 
        if (!nx._view) {
            // Create
            nx._view = nx._sys.views.create('.view-main', {
                url: url
            });
        } else {
            // Navigate
            nx._sys.views.main.router.navigate(url, {
                //reloadCurrent: nx.env.getWinID() === name,
                clearPreviousHistory: name === 'menu',
                ignoreCache: true
            });
        }
    },

    /**
     * 
     * Goes back a page
     * 
     */
    goBack: function (url) {

        var self = nx.office;

        // Close panel
        self.panelRightClose();
        self.panelLeftClose();

        // Single go back?
        if (url) {
            // Get history
            var history = self.history();
            // Find target
            var pos = history.indexOf(url);
            // Loop thru
            for (var i = pos + 1; i < history.length; i++) {
                //
                var xurl = history[i];
                // Clear
                nx.env.clearBucket(xurl);
                nx.user.removeTP(xurl);
                nx.user.removeSIO(xurl);
            }
        } else {
            //
            var xurl = nx._sys.views.current.router.url;
            // Clear
            nx.env.clearBucket(xurl);
            nx.user.removeTP(xurl);
            nx.user.removeSIO(xurl);
        }

        // And go
        nx._sys.views.main.router.back(url, {
            force: true
        });
    },

    /**
     * 
     * Syores history information
     * 
     * @param {any} url
     * @param {any} title
     * @param {any} icon
     * @param {any} prefix
     * @param {any} fld
     */
    storeHistory: function (url, title, icon, prefix, fld) {

        var self = this;

        // Save the title
        nx.env.setBucketItem('_title', title, url);
        nx.env.setBucketItem('_hicon', icon, url);
        nx.env.setBucketItem('_hprefix', prefix, url);
        nx.env.setBucketItem('_hfld', fld, url);

    },

    history: function () {

        var self = this;

        return nx._sys.views.current.history;
    },

    panelRight: function (menu) {
        $('.panel-right').html(nx.builder.contentBlock(menu));
    },

    /**
     * 
     * Opens the menu
     * 
     */
    panelRightOpen: function () {
        //
        nx._sys.panel.open('.panel-right');
    },

    /**
     * 
     * Closes the menu
     * 
     */
    panelRightClose: function () {
        //
        nx._sys.panel.close('.panel-right');
    },

    panelLeft: function (menu) {
        $('.panel-left').html(nx.builder.contentBlock(menu));
    },

    /**
     * 
     * Opens the menu
     * 
     */
    panelLeftOpen: function () {
        //
        nx._sys.panel.open('.panel-left');
    },

    /**
     * 
     * Closes the menu
     * 
     */
    panelLeftClose: function () {
        //
        nx._sys.panel.close('.panel-left');
    },

    raiseEvent: function (elem, event) {
        // Make event
        var event = new CustomEvent(event, { source: elem });
        // Tell the world
        document.dispatchEvent(event);
    },

    /**
     *  
     * Loads one script
     * 
     * @param {string} file
     * @param {function} cb
     */
    load: function (file, cb) {
        // Force?
        var force = file.substr(0, 1) === '+';
        if (force) {
            file = file.substr(1);
        }
        // Split the name
        var sections = file.split('.');
        // Start at the DOM
        var at = window;
        // Flag as found
        var found = true;
        // And pass over the keys in the name
        sections.forEach(function (key) {
            // Found?
            if (!at[key]) {
                // Flag
                found = false;
                // Make room
                at[key] = {};
            }
            // Move
            at = at[key];
        });
        // Not found?
        if (force || !found || !Object.keys(at).length) {
            // Load
            var script = document.createElement('script');
            script.onload = function () {
                if (cb) cb();
            };
            script.src = 'mobile/' + file.replace(/\./g, '/') + '.js';
            document.head.appendChild(script);
        } else {
            if (cb) cb();
        }
    },

    /**
     * 
     * Loads multiple items
     * 
     * @param {array} list
     * @param {function} cb
     */
    loadMany: function (list, cb) {
        // End?
        if (!list || !list.length) {
            if (cb) cb();
        } else {
            var entry = list.shift();
            nx.office.load(entry, function () {
                nx.office.loadMany(list, cb);
            });
        }
    },

    updateTimers: function () {

        var self = this;

        // Timer entries
        $('.timer').each(function (index, ele) {

            // Get the widget
            var widget = $(ele);
            // Get the ID
            var id = widget.attr('onclick');
            id = id.substr(1 + id.indexOf("'"));
            id = id.substr(0, id.indexOf("'"));
            // Get the info
            var info = nx.user.getTTItem(id);
            // Any?
            if (info) {

                var elap = parseInt(info.size || '0');
                if (info.status === 'Active') {
                    elap = nx.util.elapsedTime(new Date(info.ts), elap, true);
                } else {
                    elap = nx.util.elapsedTime(0, elap, true);
                    if (elap) elap = ' -- ' + elap;
                    elap = info.status + elap;
                }
                // Get target
                var target = $('[timerid=' + widget.attr('refid') + ']');
                target.text(elap);
            }

        });
    }

};

// Initialize
nx.office.init();

// Mods

// From: ow.com/questions/6545086/html-why-does-android-browser-show-go-instead-of-next-in-keyboard
(function ($) {
    $.fn.enterAsTab = function (options) {
        var settings = $.extend({
            'allowSubmit': false
        }, options);
        $(this).find('input, select, textarea, button').on("keydown", { localSettings: settings }, function (event) {
            if (settings.allowSubmit) {
                var type = $(this).attr("type");
                if (type == "submit") {
                    return true;
                }
            }
            if (event.keyCode == 13 || event.keyCode == 9) {
                var inputs = $(this).parents("form").eq(0).find(":input:visible:not(:disabled):not([readonly])");
                var idx = inputs.index(this);
                if (idx == inputs.length - 1) {
                    idx = -1;
                } else {
                    inputs[idx + 1].focus(); // handles submit buttons
                }
                try {
                    inputs[idx + 1].select();
                }
                catch (err) {

                }
                return false;
            }
        });
        return this;
    };
})(jQuery);