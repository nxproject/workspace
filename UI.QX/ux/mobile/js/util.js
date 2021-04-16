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

nx.util = {

    // ---------------------------------------------------------
    //
    // RPC
    // 
    // ---------------------------------------------------------

    _token: null,
    _rpcCount: 0,
    _ua: {},

    loopbackURL: function () {
        return window.location.protocol + '//' + window.location.host;
    },

    /**
     * 
     * Callls the server
     * 
     * @param {any} fn
     * @param {any} data
     * @param {any} cb
     */
    serviceCall: function (fn, data, cb) {

        var self = this;

        // Assure rpc
        if (!self.selfURL) {
            self.selfURL = self.loopbackURL() + '/service/';
        }

        // Add token to data
        data = data || {};
        data._token = self._token;
        data._user = nx.user.getName();
        if (nx.user.SIO) {
            data._uuid = nx.user.SIO.uuid;
        }
        // The active window
        data._winid = nx.env.getWinID();

        // Up
        self._rpcCount++;

        // Make JSON RPC payload
        var payload = {
            jsonrpc: "2.0",
            method: fn,
            id: self._rpcCount,
            params: [data]
        };

        // And call
        nx._sys.request.post(self.selfURL, JSON.stringify(payload), function (data, status, xhr) {
            //
            if (data && data.result) {
                // Parse
                data = data.result;
                // Save token
                self._token = data._token;
                // No, clean data
                if (cb) cb(data);
            } else {
                // Tell user
                if (cb) cb(null);
            }
        }, function (xhr, status, message) {
            // Tell user
            if (cb) cb(null);
        }, 'json');

    },

    isSecure: function () {

        var self = this;

        return window.location.protocol === 'https:'
    },

    isMobile: function () {

        var self = this;

        return self.userAgentHas('mobile');

    },

    isTablet: function () {

        var self = this;

        return self.userAgentHas('tablet');

    },

    isAndroid: function () {

        var self = this;

        return self.userAgentHas('android');

    },

    /**
     *  User agent check
     *  I know that this is dumb, but is all I need
     *  
     * @param {any} text
     */
    userAgentHas: function (text) {

        var self = this;

        var ans;

        // See if we already done
        if (typeof self._ua[text] === 'undefined') {

            // Make regex
            var re = new RegExp(text);
            // Match
            ans = re.exec(navigator.userAgent.toLowerCase()) != null;
            // Save
            self._ua[text] = ans;

        } else {

            // Get
            ans = self._ua[text];

        }

        return ans;
    },

    /**
     * 
     * Checks for camera
     * Must be calle once before any real use
     * 
     */
    hasCamera: function () {

        var self = this;

        //
        if (typeof self._ua._camera === 'undefined') {

            // Secure?
            if (!self.isSecure() && !self.isMobile()) {
                // Nope
                self._ua._camera = false;
            } else {
                if (navigator.getUserMedia && self.isSecure()) {
                    navigator.getUserMedia({ video: true, audio: false }, function () {
                        self._ua._camera = true;
                    }, function () {
                        self._ua._camera = false;
                    });
                }

                //
                return false;
            }
        }

        return self._ua._camera;
    },

    // ---------------------------------------------------------
    //
    // SUPPORT
    // 
    // ---------------------------------------------------------

    ifEmpty: function (value, defaultvalue) {
        return (!value) ? (defaultvalue || '') : value;
    },

    hasValue: function (value) {
        return value && !!value.length;
    },

    startsWith: function (value, prefix) {
        var ans = false;
        if (value && prefix && value.length >= prefix.length) {
            ans = prefix === value.substr(0, prefix.length);
        }
        return ans;
    },

    endsWith: function (value, suffix) {
        var ans = false;
        if (value && suffix && value.length >= suffix.length) {
            ans = suffix === value.substr(value.length - suffix.length, suffix.length);
        }
        return ans;
    },

    uuidCounter: 0,

    localUUID: function (prefix) {

        var self = this;

        return (prefix || '') + self.uuidCounter++;

    },

    // https://stackoverflow.com/questions/105034/how-to-create-guid-uuid#:~:text=UUIDs%20%28Universally%20Unique%20IDentifier%29%2C%20also%20known%20as%20GUIDs,%40broofa%27s%20answer%2C%20below%29%20there%20are%20several%20common%20pitfalls%3A
    // Public Domain/MIT
    uuid: function () {
        //Timestamp
        var d = new Date().getTime();
        //Time in microseconds since page-load or 0 if unsupported
        var d2 = (performance && performance.now && (performance.now() * 1000)) || 0;
        return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
            //random number between 0 and 16
            var r = Math.random() * 16;
            if (d > 0) {
                //Use timestamp until depleted
                r = (d + r) % 16 | 0;
                d = Math.floor(d / 16);
            } else {
                //Use microseconds since page-load if supported
                r = (d2 + r) % 16 | 0;
                d2 = Math.floor(d2 / 16);
            }
            return (c === 'x' ? r : (r & 0x3 | 0x8)).toString(16);
        });
    },

    isSameValue: function (v1, v2) {
        // Assume not
        var ans = false;
        // Arrays?
        if ((v1 && Array.isArray(v1)) || (v2 && Array.isArray(v2))) {
            // Adjust
            if (!v1) v1 = [];
            if (!Array.isArray(v1)) v1 = [v1];
            if (!v2) v2 = [];
            if (!Array.isArray(v2)) v1 = [v2];
            ans = v1.length === v2.length && v1.every((val, index) => val === v2[index]);
        } else {
            // Adjust
            v1 = v1 || '';
            v2 = v2 || '';
            ans = v1 === v2;
        }

        return ans;
    },

    joinSpace: function (values) {

        var self = this;

        var ans = '';
        values.forEach(function (value) {
            if (value.indexOf(' ') == -1) {
                ans += value + ' ';
            } else if (value.indexOf('\'') == -1) {
                ans += '\'' + value + '\' ';
            } else {
                ans += '"' + value + '" ';
            }
        });
        return ans.trim();
    },

    splitSpace: function (value, remdelim) {

        var self = this;

        var ans = [];
        if (!value || typeof value === 'object') value = '';
        var list = value.match(/[^\s\x23\x5B\x22\x27\x5D\x28\x29]+|\x23([^\x23]*)\x23|\x5B([^\x5D\x5B]*)\x5D|\x22([^\x22]*)\x22|\x27([^\x27]*)\x27|\x28([^\x28\x29]*)\x29/gi);
        if (list) {
            list.forEach(function (entry) {
                if (entry != '') {
                    if (remdelim) {
                        if (self.startsWith(entry, "'") && self.endsWith(entry, "'")) {
                            entry = entry.substr(1, entry.length - 2);
                        } else if (self.startsWith(entry, '"') && self.endsWith(entry, '"')) {
                            entry = entry.substr(1, entry.length - 2);
                        }
                    }

                    ans.push(entry);
                }
            });
        }
        return ans;
    },

    alphaNum: function (value) {
        return (value || '').replace(/[^a-z0-9]/gi, '');
    },

    elapsedTime: function (secs, extra) {

        var self = this;

        if (secs instanceof Date) secs = (new Date().getTime() - secs.getTime()) / 1000;
        if (typeof secs === 'string') {
            secs = parseFloat(secs);
        }
        // Add the extra
        if (extra) secs += extra;
        // Must have positive value
        if (secs < 0) secs = 0;

        // remove mili
        secs = Math.floor(secs);
        // Compute days
        var days = Math.floor(secs / 86400);
        secs -= days * 86400;
        // Compute hours
        var hours = Math.floor(secs / 3600);
        secs -= hours * 3600;
        // Minutes
        var minutes = Math.floor(secs / 60);
        // Seconds
        var seconds = secs - (minutes * 60);

        var ans = '';

        if (ans.length || days > 0) ans += ':' + days;
        if (ans.length || hours > 0) ans += ':' + self.paddy(hours, 2, '0');
        if (ans.length || minutes > 0) ans += ':' + self.paddy(minutes, 2, '0');
        if (ans.length || seconds > 0) ans += ':' + self.paddy(seconds, 2, '0');

        if (ans.length) ans = ans.substr(1);

        return ans;
    },

    // From https://stackoverflow.com/questions/1267283/how-can-i-pad-a-value-with-leading-zeros
    paddy: function (num, padlen, padchar) {
        var pad_char = typeof padchar !== 'undefined' ? padchar : '0';
        var pad = new Array(1 + padlen).join(pad_char);
        return (pad + num).slice(-pad.length);
    },

    /**
     * Deep merge to objects
     * 
     * @param {object} obj1
     * @param {object} obj2
     */
    merge: function (obj1, obj2) {

        var self = this;

        // Make result
        var ans = new Object();
        // Assure input
        obj1 = obj1 || {};
        obj2 = obj2 || {};
        // List of to be done later
        var tbd = Object.keys(obj2);
        // Loop thru obj1
        Object.keys(obj1).forEach(function (key) {
            // Is it in obj2?
            if (!tbd.includes(key)) {
                // No simple copy
                ans[key] = obj1[key];
            } else {
                // Remove from tbd later
                tbd.splice(tbd.indexOf(key), 1);
                // Is it an object?
                if (typeof obj1[key] === 'object' && typeof obj2[key] === 'object') {
                    // Deep merge
                    ans[key] = self.merge(obj1[key], obj2[key]);
                } else {
                    // Use obj2
                    ans[key] = obj2[key];
                }
            }
        });
        // Loop thru
        tbd.forEach(function (key) {
            // Not in obj1, so obj2 is it
            ans[key] = obj2[key];
        });


        return ans;
    },

    noOp: function () { },

    isEMail: function (value) {

        var self = this;

        var ans = false;

        if (self.hasValue(value)) {
            ans = value.match(/^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$/);
        }

        return ans;
    },

    isPhone: function (value) {

        var self = this;

        var ans = false;

        if (self.hasValue(value)) {
            ans = value.match(/\(\d{3}\)\s*\d{3}-\d{4}/);
        }

        return ans;
    },

    asBoolean: function (value) {
        return ((!!value) ? 'y' : 'n');
    },

    clone: function (value) {
        return JSON.parse(JSON.stringify(value));
    },

    // ---------------------------------------------------------
    //
    // Expression evaluator
    // 
    // ---------------------------------------------------------

    eval: function (expr, data, cb, at, result) {

        var self = this;

        // 
        result = result || '';
        at = at || 0;

        var docb = true;

        // Split
        if (!Array.isArray(expr)) {
            expr = nx.util.splitSpace(expr);
        }
        // Loop
        for (var i = at; at < expr.length; i++) {
            var ifld = expr[at];
            // Remove extra spaces
            if (nx.util.hasValue(ifld)) {
                if (nx.util.startsWith(ifld, "'")) {
                    result += ifld.substr(1, ifld.length - 2) + ' ';
                } else if (nx.util.startsWith(ifld, '[')) {
                    ifld = ifld.substr(1, ifld.length - 2);
                    // Get
                    var value = data[ifld] || '';
                    // Split
                    var poss = value.split(':');
                    // ID?
                    if (poss.length === 6) {
                        // Taking a turn
                        docb = false;
                        // Get object
                        nx.util.serviceCall('AO.ObjectGet', {
                            ds: poss[2],
                            id: poss[3]
                        }, function (obj) {
                            data[ifld] = obj._desc;
                            self.eval(expr, data, cb, result, at);
                        });
                    } else {
                        // And save in result
                        result += value + ' ';
                    }
                } else {
                    result += ifld + ' ';
                }
            }

            if (docb && cb) cb(result);
        }

    },

    evalJS: function (expr, data, dsdef) {

        var self = this;

        var ans;

        // Assure
        expr = self.ifEmpty(expr, '0');
        data = data || {};
        // RegEx for fields
        var re = /\x5B[a-z][a-z0-9]*\x5D/gi;
        // Is this a field?
        if (dsdef[expr]) {
            // Get the compute field
            expr = self.ifEmpty(dsdef[expr].compute, '0');
        }
        // Get all the fields
        var fields = expr.matchAll(re);
        // Only do each field once
        var done = [];
        // Loop thru
        for (var entry of fields) {
            // Get the field
            var field = entry[0];
            // Get the value
            var value = self.evalGetField(field, data, dsdef, done);
            // Replace
            expr = expr.replaceAll(field, value);
        }

        try {
            ans = eval(expr);
        } catch (e) {
            ans = 'ERROR: ' + e.message + ' - "' + expr + '"';
        }

        return ans || expr;
    },

    evalGetField: function (field, data, dsdef, done) {

        var self = this;

        // Handle if has delims
        if (self.startsWith(field, '[') && self.endsWith(field, ']')) {
            field = field.substr(1, field.length - 2);
        }

        // Check
        if (done.indexOf(field) === -1) {
            // Add to done
            done.push(field);
            // Get the value
            var value = data[field];
            // Get from dataset
            var def = null;
            if (dsdef) def = dsdef.fields[field];
            // Is it in the dataset?
            if (def && def.compute) {
                // Get the value
                value = self.evalGetField(field, data, dsdef, done);
            }
            // Save
            data[field] = value;
        }

        return data[field];
    },

    // ---------------------------------------------------------
    //
    // AO
    // 
    // ---------------------------------------------------------

    localizeDesc: function (desc) {

        var self = this;

        // 
        if (desc) {
            if (typeof desc === 'object') {
                desc = desc._desc;
            }
        }
        return desc;
    },

    objectToUUID: function (obj, ds) {

        var self = this;

        return '::' + self.ifEmpty(ds || obj._ds) + ':' + self.ifEmpty(obj._id) + '::';

    },

    uuidToObject: function (value, id) {

        var self = this;

        var ans = null;

        // Do we have an id?
        if (id) {
            // Make
            ans = {
                ds: value,
                id: id
            };
        } else {
            // Split
            var list = self.ifEmpty(value).split(':');
            // Make
            ans = {
                ds: list[2],
                id: list[3]
            };
        }

        return ans;
    },

    isUUID: function (value) {

        var self = this;

        var ans = false;

        if (value) {
            //
            ans = value.match(/\x3A\x3A[^\x3A]+\x3A[^\x3A]+\x3A\x3A/);
        }

        return ans;
    },

    // ---------------------------------------------------------
    //
    // NOTIFICATIONS
    // 
    // ---------------------------------------------------------

    confirm: function (title, msg, cb) {

        // Call
        Notiflix.Confirm.Init({
            borderRadius: '5px',
            titleFontSize: '12px',
            messageFontSize: '14px',
            buttonsFontSize: '12px'
        });

        Notiflix.Confirm.Show(title || 'Please confirm', msg, 'Yes', 'No', function () {
            if (cb) cb(true);
        }, function () {
            if (cb) cb(false);
        });

    },

    _notify: function (msg, options) {

        var self = nx.util;

        // Assure
        options = options || {};

        // Get the callback
        var callback = options.callback;
        // Remove
        delete options.callback;

        //if (!nx._sys.device.desktop && !options.width) {
        //    options.width = '75%%';
        //}

        // Make the options
        options = self.merge({
            style: 'Success',
            messageMaxLength: 3000,
            plainText: !nx.util.startsWith(msg, '<')
        }, options || {});

        // Do we have a callback?
        if (callback) {
            // Make the button
            Notiflix.Notify[options.style](msg, callback, options);
        } else {
            // Make the button
            Notiflix.Notify[options.style](msg);
        }
    },

    notifyInfo: function (msg, options) {

        var self = nx.util;

        // Make the options
        options = self.merge({
            style: 'Info'
        }, options || {});

        self._notify(msg, options);
    },

    notifyError: function (msg, options) {

        var self = nx.util;

        // Make the options
        options = self.merge({
            style: 'Failure'
        }, options || {});

        self._notify(msg, options);
    },

    notifyWarning: function (msg, options) {

        var self = nx.util;

        // Make the options
        options = self.merge({
            style: 'Warning'
        }, options || {});

        self._notify(msg, options);
    },

    notifyOK: function (msg, options) {

        var self = this;

        // Make the options
        options = self.merge({
            style: 'Success'
        }, options || {});

        self._notify(msg, options);
    },

    notifyQM: function (msg, options) {

        var self = nx.util;

        // Make the options
        options = self.merge({
            style: 'Info',
            callback: function () {
                if (options.from) {
                    nx.calls.qm({
                        to: options.from
                    });
                }
            },
            callbackOnTextClick: !!options.from
        }, options || {});

        self._notify(msg + (options.from ? ' // Click here to respond' : ''), options);
    },

    notifyDisplay: function (msg, options) {

        var self = this;

        // Make the options
        options = self.merge({
            style: 'Success',
            //clickToClose: true,
            //closeButton: true,
            callback: nx.util.noOp,
            callbackOnTextClick: true
        }, options || {});

        self._notify(msg, options);
    },

    sendNotify: function (to, msg, type) {
        // Send
        nx.user.SIOSend('$$noti', {
            to: to,
            type: type || 'QM',
            msg: msg
        }, {
            allow: true
        });
    },

    notifyLoadingStart: function (msg) {

        var self = this;

        Notiflix.Loading.Pulse(msg);

    },

    notifyLoadingEnd: function () {

        var self = this;

        Notiflix.Loading.Remove();

    },

    // ---------------------------------------------------------
    //
    // Popups
    // 
    // ---------------------------------------------------------

    popup: function (html, cb, closecb) {

        var self = this;

        // Make the contents
        var content = '<div class="popup"><p><a href="#" class="button button-small popup-close">Close</a></p>' + html + '</div>';

        // Make the request
        var req = {
            content: content,
            closeOnEscape: true,
            on: {}
        };

        // Habdle callbacks
        if (cb) {
            req.on.opened = cb;
        }
        if (closecb) {
            req.on.close = closecb;
        }

        nx._sys.popup.create(req).open();
    },

    urlPopup: function (url) {

        var self = this;

        var iframe = '<div class="popup"><p><a href="#" class="button button-small popup-close">Close</a></p><iframe src="' + url + '"></iframe></div>';

        nx._sys.popup.create({
            content: iframe,
            closeOnEscape: true
        }).open();
    },

    popupClose: function () {

        var self = this;

        var pu = nx._sys.popup.get();
        if (pu) pu.close();
    },

    // ---------------------------------------------------------
    //
    // CONVERSION
    // 
    // ---------------------------------------------------------

    numbersOnly: function (value) {

        var self = this;

        //
        value = value || '';
        return nx.util.ifEmpty(value.toString(), '0').replace(/[^0-9\.\-]/, '');
    },

    toInt: function (value, radix, defaultvalue) {

        var self = this;

        //
        var ans = value || defaultvalue || '0';
        if (typeof value !== 'number') {
            ans = parseInt(self.numbersOnly(value), radix || 10);
        }
        return ans;
    },

    toNumber: function (value) {

        var self = this;

        //
        var ans = value || '0';
        if (typeof value !== 'number') {
            ans = Number(self.numbersOnly(value));
        }
        return ans;
    },

    toBoolean: function (value) {

        var self = this;

        //
        var ans = false;
        if (value && typeof value !== 'boolean') {
            value = value.toString();
            ans = value.indexOf('y') != -1 ||
                value.indexOf('Y') != -1 ||
                value.indexOf('t') != -1 ||
                value.indexOf('T') != -1 ||
                value.indexOf('1') != -1
        }
        return ans;
    },

    toRelative: function (value, factor, offset) {

        var self = this;

        //
        value = value || '';
        var ans = value.toString();
        // If not factored, reset
        var pos = ans.indexOf('@');
        if (pos !== -1) {
            // Reset factor
            factor = parseFloat(ans.substr(pos + 1));
            // Clear flag
            ans = ans.substr(0, pos);
        }
        // Handle missing offset
        if (typeof offset === 'undefined') offset = 1;
        // Convert
        ans = (parseInt(self.numbersOnly(ans), 10) - offset) * factor;

        return ans;
    },

    fromRelative: function (value, factor, offset) {

        var self = this;

        // Handle missing offset
        if (typeof offset === 'undefined') offset = 1;
        // 
        value /= factor;
        value += offset;

        return Math.floor(value);
    },

    toDatasetName: function (value) {

        var self = this;

        //
        if (self.hasValue(value)) {
            value = value.toLowerCase().replace(/[^a-z_]/g, '');
            var issys = self.startsWith(value, '_');
            //
            value = (issys ? "_" : "") + value.toLowerCase().replace(/[^a-z]/g, '');
        }

        return value;
    },

    toViewName: function (value) {

        var self = this;

        //
        if (self.hasValue(value)) {
            //
            var pos = value.indexOf('_');
            if (pos !== -1) {
                value = value.substr(pos + 1);
            }
            //
            value = self.toDatasetName(value);
        }

        return value;
    },

    // ---------------------------------------------------------
    //
    // Database
    // 
    // ---------------------------------------------------------

    makeChain: function () {

        var self = this;

        var ans = {
            sop: arguments[0],
            queries: [],
            _cooked: true
        };

        // Loop thru
        for (var i = 1; i < arguments.length; i += 3) {
            //
            ans.queries.push({
                field: arguments[i],
                op: arguments[i + 1],
                value: arguments[i + 2]
            });
        }

        return ans;
    },
};