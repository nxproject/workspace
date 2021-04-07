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
                        nxtype: 'tabs',
                        top: 1,
                        left: 1,
                        width: 'default.tabWidth',
                        height: 11,
                        items: [
                            {
                                caption: 'Creation',
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
                                        nxtype: 'boolean',
                                        top: 3,
                                        left: 1,
                                        width: 'default.fieldWidth',
                                        label: 'Include Site Settings'
                                    }, {
                                        nxtype: 'button',
                                        top: 6,
                                        left: 1,
                                        width: 'default.fieldWidth',
                                        label: '',
                                        value: 'Create',
                                        click: function (e) {

                                            // Get the button
                                            var widget = nx.util.eventGetWidget(e);

                                            // Map window
                                            var win = nx.bucket.getWin(widget);

                                            // Get values
                                            var name = win.getValue('Name');
                                            var dss = win.getValue('Datasets');

                                            if (win.getValue('Include Site Settings') === 'y') dss += ' ^^';

                                            // Verify
                                            if (name && dss) {
                                                //
                                                nx.fs.download('/package/' + name + '/' + dss);

                                                // Close
                                                win.safeClose();
                                            }
                                        }
                                    }
                                ]
                            }, {
                                caption: 'Use',
                                items: [
                                    {
                                        nxtype: 'upload',
                                        top: 1,
                                        left: 1,
                                        width: 'default.fieldWidth',
                                        label: '',
                                        path: '/package/ao/_pkgs'
                                    }
                                ]
                            }
                        ]
                    }
                ]

            });

        }

    }

});