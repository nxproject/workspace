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

nx.env = {

    // ---------------------------------------------------------
    //
    // Utilities
    // 
    // ---------------------------------------------------------

    reset: function () {

        var self = this;

        //
        nx.user.aoobject = null;

        // Call login
        nx.calls.login();

    },

    // ---------------------------------------------------------
    //
    // Call private storage
    //
    // ---------------------------------------------------------

    _buckets: {},

    /**
     * 
     * The storage area for the current view or the view passed
     * 
     * @param {any} url
     */
    getBucket: function (url, raw) {

        var self = this;

        var data = {};
        
        if (url) {
            data = nx._sys.utils.parseUrlQuery(url);
        } else {
            var history = nx.office.history();
            var i = history.length - 1;
            while (!data._bucket && i >= 0) {
                // Parse
                data = nx._sys.utils.parseUrlQuery(history[i]);
                i--;
            }
        }

        // Do we have a passed?
        if (data._bucket) {
            // Grab it
            var wkg = self._buckets[data._bucket];
            // Raw?
            if (raw) {
                data = wkg;
            } else {
                // Any?
                if (wkg) {
                    data = nx.util.merge(data, wkg);
                }
            }
        }

        return data;
    },

    /**
     * 
     * Gets the window ID
     * 
     * @param {any} url
     */
    getWinID: function (url) {

        var self = this;

        return self.getBucketItem('winid', url);
    },

    /**
     * 
     * Gets the bucket ID
     * 
     * @param {any} url
     */
    getBucketID: function (url) {

        var self = this;

        return self.getBucketItem('_bucket', url);
    },

    /**
     * 
     * Gets a value from the storage area
     * 
     * @param {any} key
     * @param {any} url
     */
    getBucketItem: function (key, url) {

        var self = this;

        return self.getBucket(url)[key];
    },

    /**
     * 
     * Sets a value in the storage area
     * 
     * @param {any} key
     * @param {any} value
     * @param {any} url
     */
    setBucketItem: function (key, value, url) {

        var self = this;

        // get bucket
        var bucket = self.getBucket(url, true);
        if (bucket) bucket[key] = value;
    },

    /**
     * 
     * Clears a bucket
     * 
     */
    clearBucket: function (url, nosave) {

        var self = this;

        // Parse
        var data = nx._sys.utils.parseUrlQuery(url || nx.office.history()[0]);

        // Do we have a passed?
        if (data._bucket) {

            // Get the bucket
            var bucket = self._buckets[data._bucket];
            // Any?
            if (bucket) {
                // Get the object
                var obj = bucket._obj;
                if (obj) {
                    if (nosave) {
                        nx.db.clearObj(obj, null, nx.util.noOp);
                    } else {
                        nx.db.setObj(obj, null, nx.util.noOp);
                    }
                }

                // Delete bucket
                delete self._buckets[data._bucket];
            }
        }

    },

    /**
     * 
     * Creates a bucket
     * 
     * @param {any} data
     */
    createBucket: function (data) {

        var self = this;

        // Make id
        var id = nx.util.localUUID('bucket');

        // Save
        self._buckets[id] = data || {};

        return id;
    },

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
    // Config
    // 
    // ---------------------------------------------------------

    /**
     * 
     * Sethe the theme (light/dark)
     * 
     * @param {any} theme
     */
    setTheme: function (theme) {

        var self = this;

        // Get cuurrent
        var wkg = self.getTheme();
        if (wkg) {
            //
            var body = $('body');
            body.attr('class', body.attr('class').replace(wkg.entry, 'theme-' + theme));
        }

        // Save
        self.setStore('theme', theme);
    },

    /**
     * 
     * Gets the current theme
     * 
     * */
    getTheme: function () {

        var self = this;

        var ans;

        //
        var body = $('body');
        var cls = body.attr('class').split(' ');
        cls.forEach(function (entry) {
            if (nx.util.startsWith(entry, 'theme-')) {
                ans = {
                    entry: entry,
                    theme: entry.substr(6)
                };
            }
        });

        return ans;

    },

    /**
     * 
     * Sets the color
     * 
     * @param {any} color
     */
    setColor: function (color) {

        var self = this;

        // Get cuurrent
        var wkg = self.getColor();
        if (wkg) {
            //
            var app = $('#app');
            app.attr('class', app.attr('class').replace(wkg.entry, 'color-theme-' + color));
        }

        // Save
        self.setStore('color', color);
    },

    /**
     * 
     * Gets the current color
     * 
     */
    getColor: function () {

        var self = this;

        var ans;

        //
        var app = $('#app');
        var cls = app.attr('class').split(' ');
        cls.forEach(function (entry) {
            if (nx.util.startsWith(entry, 'color-theme-')) {
                ans = {
                    entry: entry,
                    color: entry.substr(12)
                };
            }
        });

        return ans;

    },

    /**
     * 
     * Sets the rows
     * 
     * @param {any} rows
     */
    setRows: function (rows) {

        var self = this;

        // Save
        self.setStore('rows', rows);
    },

    /**
     * 
     * Gets the current rows
     * 
     */
    getRows: function () {

        var self = this;

        return nx.util.toInt(self.getStore('rows')) || 25;

    },

    /**
     * 
     * Initializes the theme and color
     * 
     */
    init: function () {

        var self = this;

        //
        var theme = self.getStore('theme');
        if (theme) self.setTheme(theme);

        var color = self.getStore('color');
        if (color) self.setColor(color);

        // Setup timed items
        setInterval(function () {

            nx.office.updateTimers();

        }, 1000);

    }
};