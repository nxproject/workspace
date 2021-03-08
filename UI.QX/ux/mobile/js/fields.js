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

nx.fields = {

    /**
     * 
     * Get the fields
     */
    getFormFields: function () {

        var self = this;

        return $('input[nxtype]');
    },

    /**
     * 
     * Gets the data in the form
     */
    getFormData: function () {

        var self = this;

        var ans = {};

        // Get the fields
        var fields = self.getFormFields();
        // Loop thru
        fields.each(function (index, ele) {
            // Convert
            ele = $(ele);
            // Get the field name
            var aofld = ele.attr('name');
            // Get the value
            var value = ele.val();
            // Save
            ans[aofld] = value;
        });

        return ans;
    },

    /**
     * 
     * Allow tabs to move from field to field
     */
    allowTabs: function () {
        // Run mod
        $("form").enterAsTab({
            allowSubmit: false
        });
        // Give focus to first field
        $('input[tabindex="1"]').focus();
    },

    /**
     * 
     * Generate a field
     * 
     * @param {object} def
     */
    _generate: function (def, xdef, index, data) {

        var self = this;

        var ans;

        // The type
        var type = def.nxtype || 'string';

        // From dataset
        if (xdef) {
            def.choices = xdef.choices;
        }

        // Get the definition
        var mapping = nx.fmts._getMap(type);
        // If we don't have it, skip
        if (mapping) {
            // Do we have a handler?
            if (mapping.handler) {
                // Call it
                ans = self.handlers[mapping.handler](def, mapping, index, data);
            } else {

                // Get the value
                var value = def.value;
                if (def.aoFld && data) {
                    value = data[def.aoFld] || value;
                }

                // Make
                ans = self._skeleton(def.label,
                    mapping.as,
                    (def.aoFld || def.label),
                    type,
                    value,
                    mapping.fmt,
                    index,
                    mapping.tag,
                    (mapping.choices || def.choices),
                    def.on,
                    def.as,
                    def.ro);
            }
        }

        return ans;
    },

    /**
     * 
     * A generic field
     * 
     * @param {any} label
     * @param {any} type
     * @param {any} aoFld
     * @param {any} nxtype
     * @param {any} value
     * @param {any} fmtr
     * @param {int} index
     */
    _skeleton: function (label, type, aoFld, nxtype, value, fmtr, index, tag, choices, on, as, ro) {

        var self = this;

        var ans;

        //
        var id = 'F' + nx.util.localUUID();

        // Use default formatter if needed
        fmtr = fmtr || nxtype;

        // Build the element attributes
        var attr = [
            'type', (type || 'text'),
            'name', aoFld,
            'nxtype', nxtype,
            'nxfmt', fmtr,
            'tabIndex', index,
            'id', id,
            'onfocus', 'nx.fields.onfocus(this);',
            'onchange', 'nx.fields.onblur(this);'
        ];

        //
        if (on && on.change) {
            attr.push('nxonchange');
            attr.push(nx.builder.callback(on.change));
        }

        //
        //
        var fmt = nx.fmts._loader[nxtype];
        if (fmt) {
            value = fmt(value, id, attr);
        }

        // Choices?
        if (choices) {

            //
            var options = [];
            choices.forEach(function (entry) {
                var attrs = ['value', entry];
                if (entry === value) attrs.push('selected');
                if (as) {
                    entry = nx.builder[as](entry, entry);
                }
                options.push(nx.builder.tag('option', entry, attrs));
            });

            var attrl = [
                'class', 'item-after',
                'name', aoFld
            ];

            var attrsl = [
                'name', '_' + aoFld + '_',
                'onchange', 'nx.fields.onblur(this);'
            ]

            if (on && on.change) {
                attrsl.push('nxonchange');
                attrsl.push(nx.builder.callback(on.change));
            }

            ans = nx.builder.tag('a', [
                nx.builder.tag('select', options, attrsl),
                nx.builder.tag('div',
                    nx.builder.tag('div', [
                        nx.builder.tag('div', label, ['class', 'item-title item-label item-label-short']),
                        nx.builder.tag('div', value, attrl)
                    ], ['class', 'item-inner']),
                    ['class', 'item-content'])],
                ['href', '#', 'class', 'item-link smart-select smart-select-init', 'data-close-on-select', 'true']);

        } else {

            //
            if (value) {
                attr.push('value');
                attr.push(value);
            }

            if (ro === 'y') {
                attr.push('disabled');
            }

            //
            ans = nx.builder.tag('li',
                nx.builder.tag('div', [
                    nx.builder.tag('div', label, ['class', 'item-title item-label']),
                    nx.builder.tag('div', [
                        nx.builder.tag(tag || 'input', null, attr),
                        nx.builder.tag('span', null, ['class', 'input-clear-button']),
                        nx.builder.tag('span', null, ['class', 'input-clear-button']),

                    ], ['class', 'item-input-wrap'])
                ],
                    ['class', 'item-inner']),
                ['class', 'item-content item-input']);

            //
            var extras = nx.fmts._getExtras(nxtype);
            if (extras && extras.length) {

                var list = [];
                extras.forEach(function (entry) {
                    // Assume ok
                    var valid = nx.util.hasValue(entry.label);
                    //if (entry.mobile && !nx.util.isMobile()) valid = false;
                    //if (entry.secure && !nx.util.isSecure()) valid = false;
                    //if (entry.camera && !nx.util.hasCamera()) valid = false;

                    if (valid) {
                        list.push(nx.builder.chip(entry.label, 'primary', entry.cb));
                    }
                });

                //
                if (list.length) {
                    ans += nx.builder.tag('li', nx.builder.tag('div', list, ['class', 'button']));
                }
            }

        }

        return ans;

    },

    /**
     * 
     * Handles onfocus event
     * 
     * @param {any} ele
     */
    onfocus: function (ele) {

        var self = this;

        // Save
        var widget = $(ele);
        // Assure fingerprint
        if (!widget.attr('_fp')) {
            // Save
            widget.attr('_fp', md5(widget.val()));
        }
    },

    /**
     * 
     * Handles onblur event
     * 
     * @param {any} ele
     */
    onblur: function (ele) {

        var self = this;

        // Get widget
        var widget = $(ele);
        // Get value
        var value = widget.val();
        // Save
        self.set(widget, value);

        // Internal
        self.internalOnChange(widget);
    },

    /**
     * 
     * Sets a field value
     * widget is the jQuery selector result
     * 
     * @param {any} widget 
     * @param {any} value
     */
    set: function (widget, value) {

        var self = this;

        // Save
        if (typeof widget === 'string') widget = $(ele);

        // Get fingerprint
        var fp = md5(value);
        // Assure fingerprint
        if (!widget.attr('_fp')) {
            // Save
            widget.attr('_fp', md5(''));
        }
        // Changed?
        if (fp !== widget.attr('_fp')) {
            // Save
            widget.attr('_fp', fp);
            // Flag
            widget.attr('_changed', 'y');
            // Get 
            var obj = nx.env.getBucketItem('_obj');
            var name = widget.attr('name');
            var type = widget.attr('nxtype');
            // Adjust for select
            if (nx.util.startsWith(name, '_') && nx.util.endsWith(name, '_')) {
                name = name.substr(1, name.length - 2);
            }
            // Format?
            var fmtr = nx.fmts._getFormatter(type);
            if (fmtr) {
                // Do
                fmtr(widget, value, function (final) {
                    // Save
                    self.internalSet(widget, final);
                    //
                    nx.db.objSetField(obj, name, final);
                    //
                    self.signalChange(widget);
                });
            } else {
                // Set
                self.internalSet(widget, value);
                //
                nx.db.objSetField(obj, name, value);
                //
                self.signalChange(widget);
            }
        }
    },

    internalSet: function (widget, value) {

        var self = this;

        if (widget.prop('tagName') === 'DIV') {
            widget.text(value);
        }
        widget.val(value);
    },

    internalOnChange: function (widget) {

        var self = this;

        // Get the extended change
        var nxchange = widget.attr('nxonchange');
        if (nxchange) {
            // Get the fn
            var fn = eval(nxchange.substr(0, nxchange.indexOf('(')));
            // Call
            fn(widget);
        }

        // Get the name
        var name = widget.attr('name');
        if (nx.util.startsWith(name, '_') && nx.util.endsWith(name, '_')) {
            self.internalOnChange($('#' + name.substr(1, name.length - 2)));
        }
    },

    /**
     * 
     * Handles signaling a field change
     * 
     * @param {any} widget
     */
    signalChange: function (widget) {

        var self = this;

        // Save
        if (typeof widget === 'string') widget = $(ele);

        // Get value
        var value = widget.val();
        // Get 
        var win = nx.env.getBucketItem('_ao');
        var name = widget.attr('name');
        // Adjust for select
        if (nx.util.startsWith(name, '_') && nx.util.endsWith(name, '_')) {
            name = name.substr(1, name.length - 2);
        }

        if (win) {
            nx.user.SIOSend('$$object.data', {
                aofld: name,
                winid: win,
                value: value
            });
        }
    },


    // ---------------------------------------------------------
    //
    // Handlers
    // 
    // ---------------------------------------------------------

    handlers: {

        combo: function (def, mapping, index, data, choices) {

            var self = nx.fields;

            var ans;

            // Get the choices
            var choices = choices || mapping.choices || def.choices || [];

            // Assure
            if (typeof choices === 'string') {
                choices = nx.util.splitSpace(choices);
            }
            if (!Array.isArray(choices)) choices = [choices];
            // Do we allow empty?
            if (typeof def.allowEmpty === undefined || def.allowEmpty) {
                choices.unshift('');
            }

            // Get the value
            var value = def.value;
            if (def.aoFld && data) {
                value = data[def.aoFld] || value;
            }

            return self._skeleton(def.label,
                mapping.as,
                (def.aoFld || def.label),
                def.nxtype,
                value,
                mapping.fmt,
                index,
                mapping.tag,
                choices,
                def.on,
                def.as,
                def.ro);
        },

        image: function (def, mapping, index, data, choices) {

            // TBD

        },

        user: function (def, mapping, index, data, choices) {

            var self = this;

            //
            var choices = [].concat(nx.user.SIOUsers);
            choices.push(nx.user.getName());

            //
            return self.combo(def, mapping, index, data, choices);
        },

        users: function (def, mapping, index, data, choices) {

            var self = this;

            //
            var choices = [].concat(nx.user._allusers || []);

            //
            return self.combo(def, mapping, index, data, choices);

        }
    }
};