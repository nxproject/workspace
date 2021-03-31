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

nx.fmts = {

    autocaps: function (widget, value, cb) {

        value = nx.util.ifEmpty(value).replace(/(^|[^a-zA-Z\u00C0-\u017F'])([a-zA-Z\u00C0-\u017F])/g, function (m) {
            return m.toUpperCase();
        });
        cb(value);

    },

    calendarevent: function (widget, value, cb) {

        var self = this;

        if (value) {
            value = moment(value).format('YYYY-MM-DDTHH:mm:ss');
        }
        cb(value);

    },

    caps: function (widget, value, cb) {

        cb(nx.util.ifEmpty(value).toUpperCase());

    },

    creditcard: function (widget, value, cb) {

        cb(nx.util.ifEmpty(value).replace(/[^0-9\x2A]/gi, ''));

    },

    creditcardexp: function (widget, value, cb) {

        if (value) {
            var raw = value.replace(/[^0-9]+/g, '');
            if (raw.length < 4) raw = ('0000' + raw).slice(-4);
            value = raw.substr(0, 2) + '/' + raw.substr(2, 2);
        }
        cb(value);

    },

    currency: function (widget, value, cb) {

        if (value) {
            value = nx.util.toNumber(value);
            if (isNaN(value)) value = 0;
            value = value.toFixed(2);
        }
        cb(value);

    },

    date: function (widget, value, cb) {

        var self = this;

        if (value) {
            value = moment(value).format('YYYY-MM-DD');
        }
        cb(value);

    },

    datetime: function (widget, value, cb) {

        var self = this;

        if (value) {
            value = moment(value).format('YYYY-MM-DDTHH:mm:ss');
        }
        cb(value);

    },

    duration: function (widget, value, cb) {

        if (value) {
            var re = /(?<days>\d+d)|(?<hours>\d+h)|(?<minutes>\d+m)|(?<secs>\d+s)/g;
            var p = value.toLowerCase().replace(/\s/g, '').match(re);
            var days = '';
            var hours = '';
            var mins = '';
            var secs = '';
            p.forEach(function (piece) {
                var last = piece.substr(piece.length - 1, 1);
                switch (last) {
                    case 'd':
                        days = piece;
                        break;
                    case 'h':
                        hours = piece;
                        break;
                    case 'm':
                        mins = piece;
                        break;
                    case 's':
                        secs = piece;
                        break;
                }
            });
            value = '';
            if (days) value += ':' + days;
            if (hours) value += ':' + hours;
            if (mins) value += ':' + mins;
            if (secs) value += ':' + secs;
            if (value.length) value = value.substr(1);
        }
        cb(value);

    },

    email: function (widget, value, cb) {

        cb(nx.util.ifEmpty(value).replace(/ /g, '').toLowerCase());

    },

    emailphone: function (widget, value, cb) {

        if (value) {

            // EMail?
            if (value.indexOf('@') !== -1) {
                value = nx.util.ifEmpty(value).replace(/ /g, '').toLowerCase();
            } else {
                var raw = value;
                if (!nx.util.startsWith(raw, '+')) {
                    raw = raw.replace(/[^0-9]+/g, '');
                    if (raw.length < 10) raw = ('0000000000' + raw).slice(-10);
                    value = '(' + raw.substr(0, 3) + ') ' + raw.substr(3, 3) + '-' + raw.substr(6, 4);
                }
            }
        }
        cb(value);

    },

    float: function (widget, value, cb) {

        if (value) {
            value = parseFloat(value);
            if (isNaN(value)) value = '0';
        }
        cb(value);

    },

    int: function (widget, value, cb) {

        if (value) {
            value = parseInt(value);
            if (isNaN(value)) value = '0';
        }
        cb(value);

    },

    keyword: function (widget, value, cb) {

        if (value) {
            value = value.toLowerCase().replace(/[^a-z0-9]/g, '');
        }
        cb(value);

    },

    link: function (widget, value, cb) {

        // Must have a dataset
        var ds = nx.env.getBucketItem('_obj')._ds;
        if (ds && !nx.util.isUUID(value)) {

            // Phony a chain
            var chain = {
                queries: [{
                    field: '_desc',
                    value: '%' + value
                }]
            };
                // Get the dataset
            nx.db._loadDataset(ds, function (dsdef) {
            // Get the link ds
                var fdef = dsdef.fields[widget.attr('name')];
                if (fdef && fdef.linkds) {
                    // Call view
                    nx.calls.pick({
                        ds: fdef.linkds,
                        chain: chain,
                        onSelect: function (id) {
                            // The values
                            var values = {};
                            // Fill
                            values[widget.attr('name')] = id;
                            // And callback
                            nx.office.goBack(null, values);
                        }
                    });
                }
            });

        } else {
            // The values
            var values = {};
            // Fill
            values[widget.attr('name')] = id;
            // And callback
            nx.office.goBack(null, values);
        }

    },

    lower: function (widget, value, cb) {

        cb(nx.util.ifEmpty(value).toLowerCase());

    },

    lu: function (widget, value, cb) {

        // Must have a dataset
        var ds = nx.env.getBucketItem('ds');
        if (ds && value) {

            // Phony a chain
            var chain = {
                queries: [{
                    field: '_desc',
                    value: '%' + value
                }]
            };
            // Get the dataset
            nx.db._loadDataset(ds, function (dsdef) {
                // Get the field definiion
                var fdef = dsdef.fields[widget.attr('name')];
                if (fdef && fdef.linkds) {
                    // Call view
                    nx.calls.pick({
                        ds: fdef.linkds,
                        chain: chain,
                        onSelect: function (id) {
                            // Any?
                            if (id) {
                                // Parse
                                var parsed = nx.db.parseID(id);
                                // Get
                                nx.db.getObj(parsed.ds, parsed.id, function (data) {
                                    // The values
                                    var values = {};
                                    // Get map
                                    var map = nx.util.splitSpace(fdef.lumap);
                                    // Do the widget
                                    values[widget.attr('name')] = data[map[0]];
                                    // Fill rest
                                    for (var i = 1; i < map.length; i += 2) {
                                        values[map[i]] = data[map[i + 1]];
                                    }
                                    // Release
                                    nx.db.clearObj(data);
                                    // And callback
                                    nx.office.goBack(null, values);
                                });
                            } else {
                                nx.office.goBack();
                            }
                        }
                    });
                }
            });

        }
    },

    phone: function (widget, value, cb) {

        if (value) {
            var raw = value;
            if (!nx.util.startsWith(raw, '+')) {
                raw = raw.replace(/[^0-9]+/g, '');
                if (raw.length < 10) raw = ('0000000000' + raw).slice(-10);
                value = '(' + raw.substr(0, 3) + ') ' + raw.substr(3, 3) + '-' + raw.substr(6, 4);
            }
        }
        cb(value);

    },

    ssn: function (widget, value, cb) {

        if (value) {
            var raw = value.replace(/[^0-9]+/g, '');
            if (raw.length < 9) raw = ('0000000000' + raw).slice(-9);
            value = raw.substr(0, 3) + '-' + raw.substr(3, 2) + '-' + raw.substr(5, 4);
        }
        cb(value);

    },

    time: function (widget, value, cb) {

        var self = this;

        if (value) {
            value = moment(value).format('h:mm A');
        }
        cb(value);

    },

    zip: function (widget, value, cb) {

        if (value) {
            var raw = value.replace(/[^0-9]+/g, '');
            if (raw.length < 9) raw = raw + '0000000000';
            value = raw.substr(0, 5) + (raw.substr(5, 4) != '0000' ? '-' + raw.substr(5, 4) : '');
        }
        cb(value);

    },

    _getMap: function (type) {

        var self = this;

        // 
        return self._map[type];

    },

    _loader: {

        calendarevent: function (value) {

            var self = this;

            if (value) {
                value = moment(value).format('YYYY-MM-DDTHH:mm:ss');
            }

            return value;
        },

        date: function (value) {

            var self = this;

            if (value) {
                value = moment(value).format('YYYY-MM-DD');
            }

            return value;
        },

        datetime: function (value) {

            var self = this;

            if (value) {
                value = moment(value).format('YYYY-MM-DDTHH:mm:ss');
            }

            return value;
        },

        link: function (value, id, attr) {

            var self = this;

            if (value) {
                // Parse
                var wkg = nx.db.parseID(value);
                if (wkg) {
                    // Save in item
                    attr.push('_onsetup');
                    attr.push('link');
                    attr.push('_value');
                    attr.push(value);
                }
                // Reset
                value = '';
            }

            return value;
        },

        textarea: function (value, id, attr) {

            var self = this;

            if (value) {
                // Save in item
                attr.push('_onsetup');
                attr.push('textarea');
                attr.push('_value');
                attr.push(value);
                // Reset
                value = '';
            }

            return value;
        },

        time: function (value) {

            var self = this;

            if (value) {
                value = moment(value).format('h:mm A');
            }

            return value;
        }

    },

    _getFormatter: function (type) {

        var self = this;

        //
        return self[self._getActualFmt(type)];
    },

    /**
     * 
     * Returns the map for a type
     * 
     * @param {any} type
     */
    _getActualFmt: function (type) {

        var self = this;

        // 
        var map = self._map[type] || {};
        // Assure format
        type = map.fmt || type;

        return type;
    },

    /**
     * 
     * Generator map
     */
    _map: {

        id: { as: 'text' },
        account: { as: 'text', fmt: 'emailphone' },
        address: { as: 'text', fmt: 'autocaps' },
        addressee: { as: 'text', fmt: 'autocaps' },
        allowed: { as: 'text' },
        autocaps: { as: 'text' },
        boolean: { handler: 'combo', choices: ['', 'y', 'n'] },
        button: { as: 'text' },
        calendarevent: { as: 'datetime-local', fmt: 'datetime' },
        caps: { as: 'text' },
        city: { as: 'text', fmt: 'autocaps' },
        combobox: { handler: 'combo' },
        creditcard: { as: 'text' },
        creditcardexp: { as: 'text' },
        currency: { as: 'text' },
        date: { as: 'date' },
        datetime: { as: 'datetime-local' },
        driverlicense: { as: 'text' },
        duration: { as: 'text' },
        email: { as: 'email' },
        float: { as: 'text' },
        group: { as: 'text' },
        int: { as: 'text' },
        image: { handler: 'image' },
        keyword: { as: 'text' },
        label: { as: 'text' },
        link: { as: 'text' },
        listbox: { handler: 'combo' },
        lower: { as: 'text' },
        lu: { as: 'text' },
        name: { as: 'text', fmt: 'autocaps' },
        password: { as: 'password' },
        protected: { as: 'password' },
        phone: { as: 'tel' },
        pnoneemail: { as: 'text' },
        ssn: { as: 'text' },
        state: {
            handler: 'combo', choices: [
                'AL',
                'AK',
                'AZ',
                'AR',
                'CA',
                'CO',
                'CT',
                'DC',
                'DE',
                'FL',
                'GA',
                'HI',
                'ID',
                'IL',
                'IN',
                'IA',
                'KS',
                'KY',
                'LA',
                'ME',
                'MD',
                'MA',
                'MI',
                'MN',
                'MS',
                'MO',
                'MT',
                'NE',
                'NV',
                'NH',
                'NJ',
                'NM',
                'NY',
                'NC',
                'ND',
                'OH',
                'OK',
                'OR',
                'PA',
                'RI',
                'SC',
                'SD',
                'TN',
                'TX',
                'UT',
                'VT',
                'VA',
                'WA',
                'WV',
                'WI',
                'WY'
            ]
        },
        string: { as: 'text' },
        textarea: { as: 'text', tag: 'textarea' },
        time: { as: 'text' },
        timezone: { as: 'text' },
        twiliophone: { as: 'text', fmt: 'phone' },
        upload: { handler: 'upload' },
        user: { handler: 'user' },
        users: { handler: 'users' },
        vin: { as: 'text', fmt: 'caps' },
        zip: { as: 'text' }
    },

    _getExtras: function (type) {

        var self = this;

        //
        var ans = self._extras[type];
        if (!ans) {
            ans = self._extras[self._getActualFmt(type)];
        }

        return ans;
    },

    /**
     * 
     * Extras map
     * 
     */
    _extras: {

        address: [{
            label: 'Verify',
            icon: '+pin',
            cb: function (ele) {

                // 
                var widget = nx.cm.get(ele);

                nx.cm.map(widget, 'reladdr', function (map) {

                    var passed = (map.reladdr || '') + ' ' + (map.relcity || '') + ' ' + (map.relstate || '') + ' ' + (map.relzip || '');

                    $.ajax({
                        url: 'http://api.positionstack.com/v1/forward',
                        data: {
                            access_key: nx.user.getSIField('psapi'),
                            query: passed,
                            limit: 1
                        }
                    }).done(function (data) {

                        if (data.data && Array.isArray(data.data) && data.data.length) {

                            var info = data.data[0];

                            // Must be a match
                            if (info.confidence >= 0.9) {

                                // Fill
                                nx.cm.set(map, 'reladdr', info.name);
                                nx.cm.set(map, 'relcity', info.locality);
                                nx.cm.set(map, 'relstate', info.region_code);
                                nx.cm.set(map, 'relzip', info.postal_code);
                            } else {
                                nx.util.notifyWarning('Not enough information');
                            }
                        }
                    });
                });

            }
        }, {
            label: 'Map',
            mobile: true,
            cb: function (ele) {

                // 
                var widget = nx.cm.get(ele);

                nx.cm.map(widget, 'reladdr', function (map) {

                    var passed = (map.reladdr || '') + ' ' + (map.relcity || '') + ' ' + (map.relstate || '') + ' ' + (map.relzip || '');

                    $.ajax({
                        url: 'http://api.positionstack.com/v1/forward',
                        data: {
                            access_key: nx.user.getSIField('psapi'),
                            query: passed,
                            limit: 1
                        }
                    }).done(function (data) {

                        if (data.data && Array.isArray(data.data) && data.data.length) {

                            var info = data.data[0];

                            window.open('geo:' + info.latitude + ',' + info.longitude);
                        }
                    });
                });
            }
        }],

        driverlicense: [{
            label: 'Scan',
            mobile: true,
            camera: true,
            cb: function (ele) {

                var self = nx.fmts;

                // 
                var widget = nx.cm.get(ele);

                nx.cm.map(widget, 'reldl', function (map) {

                    // Do
                    nx.web._scanForBarcode(widget, function (data) {

                        // Split
                        var info = {};
                        var pieces = data.replace(/\r/g, '').split('\n');
                        // Loop thru
                        pieces.forEach(function (entry, index) {
                            // 
                            if (index === 1) {
                                info.DAQ = entry.substring(entry.indexOf('DAQ') + 3);
                            } else {
                                info[entry.substr(0, 3)] = entry.substr(3).trim();
                            }
                        });

                        // Fill
                        nx.cm.set(map, 'relname', nx.util.joinSpace(info.DAC, info.DAD, info.DCS, info.DCU).replace(/'/g, ''));
                        nx.cm.set(map, 'reladdr', info.DAG);
                        nx.cm.set(map, 'relcity', info.DAI);
                        nx.cm.set(map, 'relstate', info.DAJ);
                        nx.cm.set(map, 'relzip', (info.DAK || '').substr(0, 5));
                        nx.cm.set(map, 'reldob', new Date(info.strstr(4, 4) + '-' + info.substr(2, 2) + '-' + info.DBB.substr(0, 2)).toISOString());

                        nx.util.popupClose();

                    }, 'BrowserPDF417Reader');

                });
            }
        }],

        email: [
            {
                label: 'Send email',
                mobile: true,
                icon: '+email',
                cb: function (ele) {

                    // 
                    var widget = nx.cm.get(ele);
                    // Get the value
                    var value = widget.val();
                    // Call
                    if (value) {
                        window.open('mailto:' + value);
                    }

                }
            }],

        phone: [{
            label: 'Call',
            mobile: true,
            cb: function (ele) {

                // 
                var widget = nx.cm.get(ele);
                // Get the value
                var value = widget.val();
                // Call
                if (value) {
                    window.open('tel:+1' + nx.util.numbersOnly(value));
                }

            }
        }],

        vin: [{
            label: 'Scan',
            mobile: true,
            camera: true,
            cb: function (ele) {

                var self = nx.fmts;

                // 
                var widget = nx.cm.get(ele);

                // Do
                nx.web._scanForBarcode(widget, function (data) {

                    // Save
                    widget.val(data);
                    // Lookup
                    nx.web._lookupVIN(widget);

                    nx.util.popupClose();

                });
            }
        }, {
            label: 'Get data',
            icon: '+car',
            cb: function (ele) {

                // 
                var widget = nx.cm.get(ele);

                //
                nx.web._lookupVIN(widget);

            }
        }]
    }

};