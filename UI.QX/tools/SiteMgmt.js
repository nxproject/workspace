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

qx.Class.define('tools.SiteMgmt', {

    type: 'static',

    statics: {

        startindex: '200',
        startgroup: 'System',
        startpriority: 'K',
        startprivilege: 'HELP',

        // This is what you override
        do: function (req) {

            // Get the data
            nx.util.serviceCall('Office.ListContainers', {}, function (result) {

                //
                if (result && result.list) {

                    var win = nx.desktop.addWindow({

                        nxid: 'sitemgmt',
                        caption: 'Site Management',
                        defaultCommand: 'Ok',
                        center: 'both',
                        icon: 'house',

                        items: [

                            {
                                nxtype: 'grid',
                                top: 1,
                                left: 1,
                                width: 'default.screenWidth',
                                height: 'default.screenHeight',
                                label: '',
                                columns: [
                                    'Names',
                                    'State',
                                    'Status',
                                    'Created',
                                    'Command',
                                    'Ports',
                                    'Mounts',
                                    'ID'
                                ],
                                data: result.list,
                                isPick: true,
                                listeners: {

                                    selected: function (e) {
                                        // Get the widget
                                        var widget = nx.util.eventGetWidget(e);
                                        // And the parameters
                                        var params = nx.bucket.getParams(widget);
                                        // Get the data
                                        var data = nx.util.eventGetData(e);

                                        // Open all selected
                                        data.forEach(function (row) {

                                            // Get logs
                                            nx.util.serviceCall('Office.GetLogs', {
                                                id: row.ID
                                            }, function (result) {
                                                var win = nx.desktop.addWindow({
                                                    caption: row.Names,
                                                    caller: win,
                                                    items: [
                                                        {
                                                            nxtype: 'textarea',
                                                            top: 1,
                                                            left: 1,
                                                            width: 'default.pickWidth',
                                                            height: 'default.pickHeight@0.75',
                                                            label: '',
                                                            value: result.logs
                                                        }
                                                    ],
                                                    topToolbar: {
                                                        items: [
                                                            {
                                                                label: 'Copy',
                                                                icon: 'page_white_copy',
                                                                click: function (e) {
                                                                    nx.util.copy(result.logs);
                                                                    nx.util.notifyInfo('Copied to clipboard');
                                                                }
                                                            }
                                                        ]
                                                    }
                                                });
                                            });
                                        });

                                        widget.resetSelection();
                                    }

                                },
                                cookie: 'sitemgmt'
                            }
                        ]

                    });

                }

            });
        }
    }

});