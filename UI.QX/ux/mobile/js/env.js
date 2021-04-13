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
    _defaultBucket: null,

    /**
     * 
     * Gets the bucket ID
     * 
     * @param {any} url
     */
    getBucketID: function (url) {

        var self = this;

        var ans;

        // Do we have an url?
        if (url) {
            ans = nx._sys.utils.parseUrlQuery(url)._bucket;
        } else {
            ans = self.getBucketItem('_bucket', url);
        }

        return ans;
    },

    /**
     * 
     * The storage area for the current view or the view passed
     * 
     * @param {any} url
     */
    getBucket: function (url) {

        var self = this;

        var data = {};

        if (url) {
            // From url
            data = nx._sys.utils.parseUrlQuery(url);
            // Get the bucket ID
            var id = data._bucket;
            // Do we have it?
            if (id && self._buckets[id]) {
                // Merge
                data = self._buckets[id];
            }
        } else {
            // Get the default
            data = self._defaultBucket;
        }

        // Save
        self._buckets[data._bucket] = data;

        return data;
    },

    /**
     * 
     * Sets the default bucket and assures callbacks
     * 
     * @param {any} url
     */
    setDefaultBucket: function (url) {

        var self = this;

        // Save
        self._defaultBucket = self.getBucket(url);
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
     * Gets the bucket route
     * 
     * @param {any} url
     */
    getBucketRoute: function (url) {

        var self = this;

        var ans;

        // Get the id
        var id = self.getBucketID(url);
        // List the routes
        nx.office.history().forEach(function (route) {
            // Matches?
            if (id === self.getBucketID(route)) {
                ans = route;
            }
        })
        return ans;
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
        var bucket = self.getBucket(url);
        if (bucket) bucket[key] = value;
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
        self.setStore('rm', value);
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

};