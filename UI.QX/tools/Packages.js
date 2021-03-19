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

	@require(qx.core.Object)

************************************************************************ */

qx.Class.define('tools.Packages', {

    type: 'static',

    statics: {

        startindex: '300',
        startgroup: 'System',
        startpriority: 'K',
        startprivilege: 'PKGS',

        // This is what you override
        do: function (req) {

            nx.desktop.addWindow({

                caption: 'Packages',
                icon: 'box',

                items: [

                    {
                        nxtype: 'string',
                        top: 1,
                        left: 1,
                        width: 'default.fieldWidth',
                        label: 'Name'
                    }, {
                        nxtype: 'string',
                        top: 2,
                        left: 1,
                        width: 'default.fieldWidth',
                        label: 'Datasets'
                    }, {
                        nxtype: 'label',
                        top: 3,
                        left: 1,
                        width: 'default.fieldWidth',
                        label: 'Include'
                    }, {
                        nxtype: 'boolean',
                        top: 4,
                        left: 1,
                        width: 'default.fieldWidth',
                        label: 'Time Track Enabled'
                    }, {
                        nxtype: 'boolean',
                        top: 5,
                        left: 1,
                        width: 'default.fieldWidth',
                        label: 'IOT Enabled'
                    }, {
                        nxtype: 'keyword',
                        top: 6,
                        left: 1,
                        width: 'default.fieldWidth',
                        label: 'Help Root'
                    }, {
                        nxtype: 'button',
                        top: 7,
                        left: 1,
                        width: 'default.fieldWidth',
                        label: '',
                        value:'Create',
                        click: function (e) {

                            // Get the button
                            var widget = nx.util.eventGetWidget(e);

                            // Map window
                            var win = nx.bucket.getWin(widget);

                            // Get values
                            var name = win.getValue('Name');
                            var dss = win.getValue('Datasets');

                            if (win.getValue('Time Track Enabled') === 'y') dss += ' ^ttenabled-' + nx.desktop.user.getSIField('ttenabled');
                            if (win.getValue('IOT Enabled') === 'y') dss += ' ^iotenabled-' + nx.desktop.user.getSIField('iotenabled');
                            if (win.getValue('Help Root')) dss += ' ^helproot-' + nx.desktop.user.getSIField('helproot');

                            // Verify
                            if (name && dss) {
                                //
                                nx.fs.download('/package/' + name + '/' + dss);

                                // Close
                                win.safeClose();
                            }
                        }
                    }, {
                        nxtype: 'upload',
                        top: 9,
                        left: 1,
                        width: 'default.fieldWidth',
                        label: '',
                        path: '/package/ao/_pkgs'
                    }
                ]

            });

        }

    }

});