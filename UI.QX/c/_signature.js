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

qx.Class.define('c._signature', {

    extend: qx.ui.embed.Canvas,

    construct: function () {

        var self = this;

        // Call base
        self.base(arguments);

        // Add handler
        self.addListener('appear', function ()
        {
            //
            self.makePad();

            // Do we have a starting value?
            if (self._stored) {

                if (self._pad) {

                    // Set
                    self.setValue(self._stored);

                    // Remove
                    delete self._stored;

                }

            }

        });
    },

    members: {

        makePad: function () {

            var self = this;

            // Create
            if (!self._pad) {

                var ce = self.getContentElement();
                if (ce) {
                    var de = ce.getDomElement();
                    if (de) {

                        self._pad = new SignaturePad(de);

                        // Create
                        var menu = new c._menu();
                        // And the definitions
                        var defs = [
                            {
                                label: 'Clear',
                                icon: 'cancel',
                                click: function (e) {

                                    if (self._pad) {

                                        self._pad.clear();

                                    }

                                }
                            }, {
                                label: nx.setup.viaWeb + 'Signature',
                                icon: nx.setup.viaWebIcon,
                                click: function (e) {
                                    // Get the widget
                                    var widget = nx.util.eventGetWidget(e);
                                    // Get the patams
                                    var wparams = nx.bucket.getParams(widget);
                                    // Get the form
                                    var form = nx.bucket.getForm(widget);
                                    // Get the params
                                    var params = nx.bucket.getParams(form);
                                    // Call
                                    nx.fs.remoteLink(
                                        {
                                            type: 'signature',
                                            limit: 1 * 60,
                                            uses: 25,
                                            value: {
                                                ds: params.ds,
                                                id: params.id,
                                                winid: params.nxid,
                                                fld: wparams.aoFld
                                            }
                                        }, 'ux/signature', nx.bucket.getWin(widget), nx.setup.viaWeb + 'Signature')
                                }
                            }
                        ];

                        nx.util.createMenu(menu, defs, self);
                        nx.util.setContextMenu(self, menu);
                    }
                }

            }

            return self._pad;
        },

        getValue: function () {

            var self = this;

            var ans;

            // Assure
            var pad = self.makePad();
            if (pad) {
                // Get
                ans = self._pad.toDataURL();
            } else {
                ans = self._stored;
            }

            return ans;

        },

        setValue: function (value) {

            var self = this;

            // Assure
            var pad = self.makePad();
            if (pad) {
                // Set
                if (nx.util.hasValue(value)) {
                    pad.fromDataURL(value);
                } else {
                    pad.clear();
                }
            } else {
                self._stored = value;
            }

        }

    }

});