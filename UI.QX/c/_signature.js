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

    extend: qx.ui.embed.Html,

    construct: function () {

        var self = this;

        // Call base
        self.base(arguments);

        // Add pad maker
        self.addListener('changeHtml', function (e) {

            // Do when visible
            nx.util.eventInVisibleById(self._padid, function (id) {

                // Only once
                if (!self._pad) {

                    // Map
                    var el = $('#' + self._padid);
                    // Reset
                    el.attr('width', el.width()+'px');
                    el.attr('height', el.height() + 'px');

                    // Create
                    self._pad = new SignaturePad(el[0]);

                    // Do we have a starting value?
                    if (self._stored) {

                        // Set
                        self.setValue(self._stored);

                        // Remove
                        delete self._stored;
                    }
                }

            });

        });

        // Add handler
        self.addListener('appear', function (e) {

            // Do when visible
            nx.util.eventOnVisible(e, function (id) {

                // Save the id
                self._padid = id + '_canvas';

                // Build
                var html = '<div class="signature-pad"><div class="signature-pad--body"><canvas id="{0}" width="100%" style="touch-action: none;"></canvas></div><div>';
                // Set the id
                html = html.replace('{0}', self._padid);
                // Set
                self.setHtml(html);

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
                    //}, {
                    //    label: 'Upload',
                    //    icon: 'cancel',
                    //    click: function (e) {

                    //        // Make the button
                    //        var button = new qxl.upload.UploadButton('Upload', nx.util.getIcon('database_connect'));

                    //        // Make the upload manager
                    //        var uploader = new qxl.upload.UploadMgr(button, '/f');
                    //        // Save path
                    //        button._path = req.path;

                    //        uploader.addListener('addFile', function (evt) {

                    //            // Get the file
                    //            var file = evt.getData();

                    //            // Get the path
                    //            var lpath = button._path;

                    //            // No path?
                    //            if (!lpath) {
                    //                // Get the aoobject
                    //                var aoobj = nx.bucket.getForm(nx.bucket.getWidgets(self)[0])._aoobject;
                    //                // Make the path
                    //                lpath = '/f/ao/' + aoobj.values._ds + '/' + aoobj.values._id;
                    //            }
                    //            // Add the file
                    //            var final = lpath + '/Upload/' + file.getFilename();
                    //            // Set the path
                    //            uploader.setUploadUrl(final);

                    //            var progressListenerId = file.addListener('changeProgress', function (evt) {
                    //                var file = evt.getTarget();
                    //                var uploadedSize = evt.getData();

                    //                button.setLabel('At ' + Math.round(uploadedSize / file.getSize() * 100) + '%');
                    //            }, this);

                    //            // All browsers can at least get changes in state
                    //            var stateListenerId = file.addListener('changeState', function (evt) {
                    //                var state = evt.getData();
                    //                var file = evt.getTarget();

                    //                if (state == 'uploading')
                    //                    button.setLabel('Uploading ' + file.getFilename());
                    //                else if (state == 'uploaded')
                    //                    button.setLabel('Completed ' + file.getFilename());
                    //                else if (state == 'cancelled')
                    //                    button.setLabel('Cancelled ' + file.getFilename());

                    //                // Remove the listeners
                    //                if (state == 'uploaded' || state == 'cancelled') {
                    //                    file.removeListenerById(progressListenerId);
                    //                    file.removeListenerById(stateListenerId);
                    //                }
                    //            }, this);
                    //        });

                    //        // Click
                    //        button.execute();
                    //    }
                    },{
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

            });
        });
    },

    members: {

        getValue: function () {

            var self = this;

            var ans;

            // Assure
            if (self._pad) {
                // Get
                ans = self._pad.toDataURL();
            } else {
                ans = self._stored;
            }

            //
            var field = nx.bucket.getParams(self).aoFld;
            nx.bucket.getWin(self).setFormDataChange(field);

            return ans;

        },

        setValue: function (value) {

            var self = this;

            // Assure
            if (self._pad) {
                // Set
                if (nx.util.hasValue(value)) {
                    self._pad.fromDataURL(value);
                } else {
                    self._pad.clear();
                }
            } else {
                self._stored = value;
            }

        }

    }

});