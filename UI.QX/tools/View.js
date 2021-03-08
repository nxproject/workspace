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

qx.Class.define('tools.View', {

    type: 'static',

    statics: {

        startindex: 'hidden',

        // This is what you override
        do: function (req) {

            var win;

            // Get dataset
            nx.desktop._loadDataset(req.ds, function (dsdef) {

                var prefix = req.idprefix || ((req.prefixfield || '') + (req.prefixvalue || '')) || '';

                var viewdef = {

                    nxid: 'pick_' + req.ds + '_' + prefix + md5(JSON.stringify(req.chain || {})),
                    caption: 'Select ' + dsdef.caption + (req.caption || ''),
                    icon: req.icon || dsdef.icon,
                    defaultCommand: 'Ok',
                    caller: req.caller,

                    items: [

                        {
                            nxtype: 'grid',
                            top: 1,
                            left: 1,
                            width: 'default.pickWidth',
                            height: 'default.pickHeight',
                            label: '',
                            ds: req.ds,
                            idprefix: req.idprefix,
                            dsdef: dsdef,
                            isPick: true,
                            allowFilter: true,
                            columns: [{
                                label: 'Description',
                                aoFld: '_desc'
                            }],
                            listeners: {

                                selected: function (e) {
                                    // Get the widget
                                    var widget = nx.util.eventGetWidget(e);
                                    // And the parameters
                                    var params = nx.bucket.getParams(widget);
                                    // Get the data
                                    var data = nx.util.eventGetData(e);

                                    // Callback
                                    if (req.onSelect) {

                                        req.onSelect(e, data);

                                        nx.bucket.getForm(widget).safeClose();

                                    } else {

                                        // Open all selected
                                        data.forEach(function (row) {
                                            nx.desktop.addWindowDS({
                                                ds: req.ds,
                                                id: row._id,
                                                view: req.view,
                                                sysmode: req.sysmode,
                                                caller: nx.util.eventGetWindow(e),
                                                chain: req.chain
                                            });
                                        });

                                        widget.resetSelection();

                                    }
                                }

                            },
                            chain: req.chain
                        }
                    ],

                    listeners: {

                        appear: function () {


                            // Get the grid
                            var grid = win.getFieldsOfClass('c._grid');

                            // Do we have a filter toolbar?
                            if (win.bottomToolbar) {
                                if (grid.length) {
                                    grid[0].setFilterToolbar(win.bottomToolbar);
                                }
                            }
                        }
                    },

                    processSIO: function (win, event) {
                        // Object events
                        if (nx.util.startsWith(event.fn, '$$object') &&
                            (event.message.ds === req.ds ||
                                event.message.ds === ('#' + req.ds))) {
                            // Get renderer
                            var rend = nx.util.getChildOfClass(win, 'nx.renderer');
                            // Any?
                            if (rend) {
                                // Get the grid
                                var grid = nx.util.getChildOfClass(rend, 'c._grid');
                                // Any?
                                if (grid) {
                                    // Refresh
                                    grid.refresh();
                                }
                            }
                        }
                    }

                };

                // Make possible commands
                var commands = [];

                // None if prefix
                if (!req.idprefix) {
//
                    viewdef.bottomToolbar = nx.util.createPickToolbar(req.ds, function (e) {
                        // Refresh the grid
                        nx.util.eventGetWindow(e).getFieldsOfClass('c._grid')[0].refresh();
                    });

                    // Can we report?
                    if (nx.desktop.user.opAllowed(req.ds, 'c', (dsdef.calAllow || ''))) {
                        if (!commands.length) commands.push('>');
                        commands.push({
                            label: 'Calendar',
                            icon: 'calendar',
                            click: function (e) {
                                nx.fs.calendar({
                                    ds: req.ds,
                                    desc: dsdef.caption,
                                    caller: nx.util.eventGetWindow(e)
                                });
                            }
                        });
                    }

                    // Can we report?
                    if (nx.desktop.user.opAllowed(req.ds, 'r', (dsdef.rptAllow || ''))) {
                        if (!commands.length) commands.push('>');
                        commands.push({
                            label: 'Reports',
                            icon: 'application_double',
                            click: function (e) {
                                nx.util.runTool('View', {
                                    ds: req.ds,
                                    icon: 'application_double',
                                    idprefix: '#rpt_',
                                    caption: ' Report',
                                    caller: win,
                                    tool: 'TPTool',
                                    tptool: nx.fs.report,
                                    onSelect: function (e, selected) {
                                        selected.forEach(function (row) {
                                            nx.fs.report({
                                                ds: req.ds,
                                                id: row._id,
                                                caller: nx.util.eventGetWindow(e)
                                            });
                                        });
                                    }
                                });
                            }
                        });
                    }

                    // Add documents
                    if (nx.desktop.user.opAllowed(req.ds, 'l')) {
                        if (!commands.length) commands.push('>');
                        commands.push({
                            label: 'Templates',
                            icon: 'folder',
                            click: function (e) {
                                nx.util.runTool('Documents', {
                                    ds: req.ds,
                                    id: nx.setup.templatesID,
                                    caption: 'Templates',
                                    caller: nx.util.eventGetWindow(e)
                                });
                            }
                        });
                    }

                    // Add documents
                    if (nx.desktop.user.opAllowed(req.ds, 't', (dsdef.tskAllow || ''))) {
                        if (!commands.length) commands.push('>');
                        commands.push({
                            label: 'Tasks',
                            icon: 'cog',
                            click: function (e) {
                                nx.util.runTool('View', {
                                    ds: req.ds,
                                    icon: 'cog',
                                    idprefix: '#tsk_',
                                    caption: ' Task',
                                    caller: win,
                                    tool: 'TPTool',
                                    tptool: nx.fs.task,
                                    onSelect: function (e, selected) {
                                        selected.forEach(function (row) {
                                            nx.fs.task({
                                                ds: req.ds,
                                                id: row._id,
                                                caller: nx.util.eventGetWindow(e)
                                            });
                                        });
                                    }
                                });
                            }
                        });
                    }

                    // Can we analyze?
                    if (nx.desktop.user.opAllowed(req.ds, 'z', (dsdef.anaAllow || ''))) {
                        if (!commands.length) commands.push('>');
                        commands.push({
                            label: 'Analyze',
                            icon: 'chart_bar',
                            click: function (e) {
                                nx.util.runTool('View', {
                                    ds: req.ds,
                                    icon: 'chart_bar',
                                    idprefix: '#ana_',
                                    caption: ' Analyze',
                                    caller: win,
                                    tool: 'TPTool',
                                    tptool: nx.fs.analyze,
                                    onSelect: function (e, selected) {
                                        selected.forEach(function (row) {
                                            nx.fs.analyze({
                                                ds: req.ds,
                                                id: row._id,
                                                caller: nx.util.eventGetWindow(e)
                                            });
                                        });
                                    }
                                });
                            }
                        });
                    }

                    // Can we workflow?
                    if (nx.desktop.user.opAllowed(req.ds, 'w', (dsdef.wfAllow || ''))) {
                        if (!commands.length) commands.push('>');
                        commands.push({
                            label: 'Workflows',
                            icon: 'plugin',
                            click: function (e) {
                                nx.util.runTool('View', {
                                    ds: req.ds,
                                    icon: 'plugin',
                                    idprefix: '#wfs_',
                                    caption: ' Workflow',
                                    caller: win,
                                    tool: 'TPTool',
                                    tptool: nx.fs.wf,
                                    onSelect: function (e, selected) {
                                        selected.forEach(function (row) {
                                            nx.fs.wf({
                                                ds: req.ds,
                                                id: row._id,
                                                caller: nx.util.eventGetWindow(e)
                                            });
                                        });
                                    }
                                });
                            }
                        });
                    }

                }

                // Can we add?
                if (nx.desktop.user.opAllowed(req.ds, 'a')) {
                    commands.push('>');
                    commands.push({
                        label: 'Add',
                        icon: 'application_add',
                        click: function (e) {
                            nx.util.runTool(req.tool || 'Object', {
                                ds: req.ds,
                                view: nx.desktop.user.getDSInfo(req.ds).view,
                                caller: nx.util.eventGetWindow(e),
                                chain: req.chain,
                                idprefix: req.idprefix,
                                tptool: req.tptool
                            });
                        }
                    });
                }

                // Any commands?
                if (commands.length) {
                    viewdef.commands = {
                        items: commands
                    };
                }

                // Add the window
                win = nx.desktop.addWindow(viewdef);
            });
        }

    }

});