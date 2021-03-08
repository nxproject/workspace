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

/**
 * Create namespace
 */

window.nx = {

    office: {

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
                script.src = file.replace(/\./g, '/') + '.js';
                document.head.appendChild(script);
            } else {
                if (cb) cb();
            }
        },

        /**
         * 
         * Loads packageof scripts
         * 
         * @param {array} list
         * @param {function} cb
         */
        loadPackage: function (list, cb) {
            // Load
            var script = document.createElement('script');
            script.onload = function () {
                if (cb) cb();
            }
            script.src = '/nxpkg?list=' + encodeURI(JSON.stringify(list)) + "&nomin=" + encodeURI(JSON.stringify([]));
            document.head.appendChild(script);
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
        }

    }
};