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

qx.Class.define('tools.Datasets', {

    type: 'static',

    statics: {

        startindex: '300',
        startgroup: 'System',
        startpriority: 'K',
        startskipds: 'y',
        startprivilege: '*',

        // This is what you override
        do: function (req) {

            var win;

            win = nx.desktop.addWindow({

                nxid: 'datasets',
                caption: 'Datasets/Views',
                icon: 'house',

                items: [
                    {
                        nxtype: 'tree',
                        treeRoot: 'Dataset/View',
                        top: 1,
                        left: 1,
                        width: 'default.pickWidth',
                        height: 15,
                        choices: [],
                        allowEmpty: false,

                        listeners: {

                            changeSelection: function (e) {

                                //
                                var widget = nx.util.eventGetWidget(e);
                                if (widget && widget.getSelection) {
                                    var sel = widget.getSelection();
                                    sel.forEach(function (entry) {
                                        if (entry.onClick) {
                                            entry.onClick(e);
                                        }
                                    });
                                }
                                // Clear the selection
                                widget.resetSelection();

                            }

                        }
                    }
                ],

                listeners: {

                    appear: function () {

                        var self = this;

                        self.processSIO(self, {
                            fn: '$$changed.dataset'
                        });

                    }

                },

                fns: {

                    addDataset: function (e) {

                        // Get list
                        nx.util.runTool('Input', {

                            label: 'Dataset names',
                            caller: win,
                            atOk: function (list) {

                                // Call
                                nx.util.serviceCall('AO.DatasetAddList', {
                                    list: list
                                }, function () {

                                    self.processSIO(self, {
                                        fn: '$$changed.dataset'
                                    });

                                });
                            }
                        });

                    },

                    addView: function (e) {

                        var widget = nx.util.eventGetWidget(e);
                        var ds = widget.getLabel();
                        ds = ds.substr(0, ds.indexOf(' '));

                        // Get list
                        nx.util.runTool('Input', {

                            label: 'View names',
                            caller: win,
                            atOk: function (list) {

                                // Call
                                nx.util.serviceCall('AO.ViewAddList', {
                                    ds: ds,
                                    list: list
                                }, function () {

                                    self.processSIO(self, {
                                        fn: '$$changed.dataset'
                                    });

                                });
                            }
                        });
                    },

                    cloneView: function (e) {

                        var widget = nx.util.eventGetWidget(e);
                        var view = widget.getLabel();
                        var ds = widget.getParent().getLabel();
                        ds = ds.substr(0, ds.indexOf(' '));

                        // Get list
                        nx.util.runTool('Input', {

                            label: 'View name',
                            caller: win,
                            atOk: function (list) {

                                // Call
                                nx.util.serviceCall('AO.ViewClone', {
                                    ds: ds,
                                    view: view,
                                    list: list
                                }, function () {

                                    self.processSIO(self, {
                                        fn: '$$changed.dataset'
                                    });

                                });
                            }
                        });
                    }

                },

                processSIO: function (win, event) {

                    var self = this;

                    // According to function
                    switch (event.fn) {

                        case '$$changed.dataset':
                        case '$$changed.view':

                            // Get datasets
                            nx.util.serviceCall('AO.DSViewList', {}, function (result) {
                                //
                                var raw = result.list;
                                // Get list
                                var dss = Object.keys(raw);
                                // Sort
                                dss.sort();
                                // Make menu
                                var menu = [];
                                // Loop thru
                                dss.forEach(function (ds) {
                                    //
                                    var info = raw[ds];

                                    // Make dataset entry
                                    var entry = {
                                        label: ds + ' // ' + info.caption,
                                        icon: info.icon,
                                        click: function () {
                                            // Call tool
                                            nx.util.runTool('PropertiesDataset', {
                                                ds: ds,
                                                caller: win
                                            });
                                        },
                                        choices: [],
                                        contextMenu: {
                                            items: [
                                                {
                                                    label: 'Add dataset',
                                                    click: function (e) {
                                                        self.addDataset(e);
                                                    }
                                                }, {
                                                    label: 'Add view',
                                                    click: function (e) {
                                                        self.addView(e);
                                                    }
                                                }
                                            ]
                                        }
                                    };

                                    // Loop thru
                                    info.views.forEach(function (view) {
                                        entry.choices.push({
                                            label: view,
                                            click: function () {
                                                // Call tool
                                                nx.util.runTool('Object', {
                                                    ds: ds,
                                                    view: view,
                                                    sysmode: true,
                                                    caller: win
                                                });
                                            },
                                            contextMenu: {
                                                items: [
                                                    {
                                                        label: 'Add dataset',
                                                        click: function (e) {
                                                            self.addDataset(e);
                                                        }
                                                    }, {
                                                        label: 'Clone view',
                                                        click: function (e) {
                                                            self.cloneView(e);
                                                        }
                                                    }
                                                ]
                                            }
                                        });
                                    });

                                    menu.push(entry);
                                });

                                // 
                                var tree = win.getFieldsOfClass('c._tree')

                                // Update
                                if (tree.length) {
                                    nx.setup.choices(tree[0], {
                                        choices: menu,
                                        allowEmpty: false,
                                        treeRoot: 'Dataset/View'
                                    });
                                }

                            });
                            break;

                    }
                }

            });

        }

    }
});