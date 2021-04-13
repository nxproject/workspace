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

qx.Class.define('tools.PropertiesDataset', {

    type: 'static',

    statics: {

        startindex: 'hidden',

        // This is what you override
        do: function (req) {

            nx.desktop._loadDataset(req.ds, function (dsdef) {

                var pageH = 15;

                var nwin = nx.desktop.addWindow({

                    caption: 'Dataset Properties for ' + req.ds,
                    icon: dsdef.icon,
                    nxid: 'prop_' + req.ds,
                    caller: req.caller,
                    ds: req.ds,

                    items: [
                        {
                            nxtype: 'tabs',
                            top: 1,
                            left: 1,
                            width: 5 + nx.default.get('default.tabWidth'),
                            height: pageH,
                            tabsAt: 'right',
                            items: [
                                {
                                    caption: 'Info',
                                    items: [

                                        {
                                            nxtype: 'string',
                                            top: 1,
                                            left: 1,
                                            width: 'default.fieldWidth',
                                            label: 'Caption',
                                            aoFld: 'caption',
                                            value: dsdef.caption
                                        }, {
                                            nxtype: 'string',
                                            top: 2,
                                            left: 1,
                                            width: 'default.fieldWidth',
                                            label: 'Placeholder',
                                            aoFld: 'placeholder',
                                            value: dsdef.placeholder
                                        }, {
                                            nxtype: 'combobox',
                                            top: 3,
                                            left: 1,
                                            width: 'default.fieldWidth',
                                            label: 'Sort Order',
                                            aoFld: 'sortOrder',
                                            value: dsdef.sortOrder,
                                            choices: ['asc', 'desc']
                                        }, {
                                            nxtype: 'icon',
                                            top: 4,
                                            left: 1,
                                            width: 'default.fieldWidth',
                                            label: 'Icon',
                                            aoFld: 'icon',
                                            value: dsdef.icon
                                        }, {
                                            nxtype: 'combobox',
                                            top: 5,
                                            left: 1,
                                            width: 'default.fieldWidth',
                                            label: 'Hide in Start',
                                            aoFld: 'hidden',
                                            value: dsdef.hidden,
                                            choices: ['y', 'n', 'v', 'w', 'wv', 'm', 'mv']
                                        }, {
                                            nxtype: 'string',
                                            top: 6,
                                            left: 1,
                                            width: 'default.fieldWidth',
                                            label: 'Default privileges',
                                            aoFld: 'privileges',
                                            value: dsdef.privileges
                                        }, {
                                            nxtype: 'string',
                                            top: 7,
                                            left: 1,
                                            width: 'default.fieldWidth',
                                            label: 'ID field',
                                            aoFld: 'idalias',
                                            value: dsdef.idalias
                                        }, {
                                            nxtype: 'string',
                                            top: 8,
                                            left: 1,
                                            width: 'default.fieldWidth',
                                            label: 'Child DSS',
                                            aoFld: 'childdss',
                                            value: dsdef.childdss
                                        }, {
                                            nxtype: 'string',
                                            top: 9,
                                            left: 1,
                                            width: 'default.fieldWidth',
                                            label: 'Related DSS/field',
                                            aoFld: 'relateddss',
                                            value: dsdef.relateddss
                                        }, {
                                            nxtype: 'keyword',
                                            top: 10,
                                            left: 1,
                                            width: 'default.fieldWidth',
                                            label: 'Task@Save',
                                            aoFld: 'taskatsave',
                                            value: dsdef.taskatsave
                                        }
                                    ]
                                }, {
                                    caption: 'Options',
                                    items: [
                                        {
                                            nxtype: 'boolean',
                                            top: 1,
                                            left: 1,
                                            width: 'default.fieldWidth',
                                            label: 'Only one',
                                            aoFld: 'isUnique',
                                            value: dsdef.isUnique
                                        }, {
                                            nxtype: 'combobox',
                                            top: 2,
                                            left: 1,
                                            width: 'default.fieldWidth',
                                            label: 'Default Cmd',
                                            aoFld: 'defaultCommand',
                                            value: dsdef.defaultCommand,
                                            choices: ['Ok', 'Save', 'Close']
                                        }, {
                                            nxtype: 'string',
                                            top: 3,
                                            left: 1,
                                            width: 'default.fieldWidth',
                                            label: 'Selector',
                                            aoFld: 'selector',
                                            value: dsdef.selector
                                        }
                                    ]
                                }, {
                                    caption: 'Start',
                                    items: [
                                        {
                                            nxtype: 'string',
                                            top: 1,
                                            left: 1,
                                            width: 'default.fieldWidth',
                                            label: 'Group',
                                            aoFld: 'startgroup',
                                            value: dsdef.startgroup
                                        }, {
                                            nxtype: 'string',
                                            top: 2,
                                            left: 1,
                                            width: 'default.fieldWidth',
                                            label: 'Priority',
                                            aoFld: 'startpriority',
                                            value: dsdef.startpriority
                                        }, {
                                            nxtype: 'string',
                                            top: 3,
                                            left: 1,
                                            width: 'default.fieldWidth',
                                            label: 'Index',
                                            aoFld: 'startindex',
                                            value: dsdef.startindex
                                        }
                                    ]
                                }, {
                                    caption: 'SIO',
                                    items: [
                                        {
                                            nxtype: 'string',
                                            top: 1,
                                            left: 1,
                                            width: 'default.fieldWidth',
                                            label: 'At save',
                                            aoFld: 'sioeventsatsave',
                                            value: dsdef.sioeventsatsave
                                        }
                                    ]
                                }, {
                                    caption: 'Chat',
                                    items: [
                                        {
                                            nxtype: 'boolean',
                                            top: 1,
                                            left: 1,
                                            width: 'default.fieldWidth',
                                            label: 'Enable',
                                            aoFld: 'chatAllow',
                                            value: dsdef.chatAllow
                                        }
                                    ]
                                }, {
                                    caption: 'Time track',
                                    items: [
                                        {
                                            nxtype: 'boolean',
                                            top: 1,
                                            left: 1,
                                            width: 'default.fieldWidth',
                                            label: 'Enable',
                                            aoFld: 'ttAllow',
                                            value: dsdef.ttAllow
                                        }
                                    ]
                                }, {
                                    caption: 'Privacy',
                                    items: [
                                        {
                                            nxtype: 'boolean',
                                            top: 1,
                                            left: 1,
                                            width: 'default.fieldWidth',
                                            label: 'Enable',
                                            aoFld: 'privAllow',
                                            value: dsdef.privAllow
                                        }, {
                                            nxtype: 'keyword',
                                            top: 2,
                                            left: 1,
                                            width: 'default.fieldWidth',
                                            label: 'Private field',
                                            aoFld: 'privField',
                                            value: dsdef.privField
                                        }
                                    ]
                                }, {
                                    caption: 'Reports',
                                    icon: 'application_double',
                                    items: [
                                        {
                                            nxtype: 'boolean',
                                            top: 1,
                                            left: 1,
                                            width: 'default.fieldWidth',
                                            label: 'Enable',
                                            aoFld: 'rptAllow',
                                            value: dsdef.rptAllow
                                        }, {
                                            nxtype: 'reportable',
                                            top: 2,
                                            left: 1,
                                            width: 'default.fieldWidth',
                                            label: 'Fields',
                                            aoFld: 'rptby',
                                            value: dsdef.rptby
                                        }
                                    ]
                                }, {
                                    caption: 'Analyze',
                                    icon: 'chart_bar',
                                    items: [
                                        {
                                            nxtype: 'boolean',
                                            top: 1,
                                            left: 1,
                                            width: 'default.fieldWidth',
                                            label: 'Enable',
                                            aoFld: 'anaAllow',
                                            value: dsdef.anaAllow
                                        }, {
                                            nxtype: 'reportable',
                                            top: 2,
                                            left: 1,
                                            width: 'default.fieldWidth',
                                            label: 'Fields',
                                            aoFld: 'anaby',
                                            value: dsdef.anaby
                                        }, {
                                            nxtype: 'keyword',
                                            top: 3,
                                            left: 1,
                                            width: 'default.fieldWidth',
                                            label: 'Pick',
                                            aoFld: 'anapick',
                                            value: dsdef.anapick
                                        }
                                    ]
                                }, {
                                    caption: 'Calendar',
                                    icon: 'calendar',
                                    items: [
                                        {
                                            nxtype: 'boolean',
                                            top: 1,
                                            left: 1,
                                            width: 'default.fieldWidth',
                                            label: 'Enable',
                                            aoFld: 'calAllow',
                                            value: dsdef.calAllow
                                        }, {
                                            nxtype: 'boolean',
                                            top: 2,
                                            left: 1,
                                            width: 'default.fieldWidth',
                                            label: 'Read Only?',
                                            aoFld: 'calRO',
                                            value: dsdef.calRO
                                        }, {
                                            nxtype: 'string',
                                            top: 3,
                                            left: 1,
                                            width: 'default.fieldWidth',
                                            label: 'Subject',
                                            aoFld: 'calsubj',
                                            value: dsdef.calsubj
                                        }, {
                                            nxtype: 'string',
                                            top: 4,
                                            left: 1,
                                            width: 'default.fieldWidth',
                                            label: 'Start On',
                                            aoFld: 'calstart',
                                            value: dsdef.calstart
                                        }, {
                                            nxtype: 'string',
                                            top: 5,
                                            left: 1,
                                            width: 'default.fieldWidth',
                                            label: 'End on',
                                            aoFld: 'calend',
                                            value: dsdef.calend
                                        }, {
                                            nxtype: 'calendarable',
                                            top: 6,
                                            left: 1,
                                            width: 'default.fieldWidth',
                                            label: 'By fields',
                                            aoFld: 'calby',
                                            value: dsdef.calby
                                        }
                                    ]
                                }, {
                                    caption: 'Organizer',
                                    icon: 'org',
                                    items: [
                                        {
                                            nxtype: 'boolean',
                                            top: 1,
                                            left: 1,
                                            width: 'default.fieldWidth',
                                            label: 'Enable',
                                            aoFld: 'orgAllow',
                                            value: dsdef.orgAllow
                                        }, {
                                            nxtype: 'boolean',
                                            top: 2,
                                            left: 1,
                                            width: 'default.fieldWidth',
                                            label: 'As root',
                                            aoFld: 'orgRoot',
                                            value: dsdef.orgRoot
                                        }, {
                                            nxtype: 'boolean',
                                            top: 3,
                                            left: 1,
                                            width: 'default.fieldWidth',
                                            label: 'As Child',
                                            aoFld: 'orgChild',
                                            value: dsdef.orgChild
                                        }
                                    ]
                                }, {
                                    caption: 'Tasks',
                                    icon: 'bell',
                                    items: [
                                        {
                                            nxtype: 'boolean',
                                            top: 1,
                                            left: 1,
                                            width: 'default.fieldWidth',
                                            label: 'Enable',
                                            aoFld: 'tskAllow',
                                            value: dsdef.tskAllow
                                        }
                                    ]
                                }, {
                                    caption: 'Workflows',
                                    icon: 'plugin',
                                    items: [
                                        {
                                            nxtype: 'boolean',
                                            top: 1,
                                            left: 1,
                                            width: 'default.fieldWidth',
                                            label: 'Enable',
                                            aoFld: 'wfAllow',
                                            value: dsdef.wfAllow
                                        }, {
                                            nxtype: 'keyword',
                                            top: 2,
                                            left: 1,
                                            width: 'default.fieldWidth',
                                            label: 'Def. WF',
                                            aoFld: 'wfDefault',
                                            value: dsdef.wfDefault
                                        }, {
                                            nxtype: 'keyword',
                                            top: 3,
                                            left: 1,
                                            width: 'default.fieldWidth',
                                            label: 'Dataset',
                                            aoFld: 'wfDS',
                                            value: dsdef.wfDS
                                        }, {
                                            nxtype: 'keyword',
                                            top: 4,
                                            left: 1,
                                            width: 'default.fieldWidth',
                                            label: 'Description',
                                            aoFld: 'wfDescription',
                                            value: dsdef.wfDescription
                                        }, {
                                            nxtype: 'keyword',
                                            top: 5,
                                            left: 1,
                                            width: 'default.fieldWidth',
                                            label: 'Assigned To',
                                            aoFld: 'wfAssignedTo',
                                            value: dsdef.wfAssignedTo
                                        }, {
                                            nxtype: 'keyword',
                                            top: 6,
                                            left: 1,
                                            width: 'default.fieldWidth',
                                            label: 'Done By',
                                            aoFld: 'wfDoneBy',
                                            value: dsdef.wfDoneBy
                                        }, {
                                            nxtype: 'keyword',
                                            top: 7,
                                            left: 1,
                                            width: 'default.fieldWidth',
                                            label: 'Started On',
                                            aoFld: 'wfStartedOn',
                                            value: dsdef.wfStartedOn
                                        }, {
                                            nxtype: 'keyword',
                                            top: 8,
                                            left: 1,
                                            width: 'default.fieldWidth',
                                            label: 'Expected On',
                                            aoFld: 'wfExpectedOn',
                                            value: dsdef.wfExpectedOn
                                        }, {
                                            nxtype: 'keyword',
                                            top: 9,
                                            left: 1,
                                            width: 'default.fieldWidth',
                                            label: 'Ended On',
                                            aoFld: 'wfEndedOn',
                                            value: dsdef.wfEndedOn
                                        }, {
                                            nxtype: 'keyword',
                                            top: 10,
                                            left: 1,
                                            width: 'default.fieldWidth',
                                            label: 'Outcome',
                                            aoFld: 'wfOutcome',
                                            value: dsdef.wfOutcome
                                        }, {
                                            nxtype: 'keyword',
                                            top: 11,
                                            left: 1,
                                            width: 'default.fieldWidth',
                                            label: 'Message',
                                            aoFld: 'wfMessage',
                                            value: dsdef.wfMessage
                                        }
                                    ]
                                }
                            ]
                        }
                    ],

                    commands: {

                        items: [
                            '>', {
                                label: 'Delete',
                                icon: 'database_delete',
                                click: function (e) {

                                    nx.util.confirm('Are you sure?', 'Delete ' + req.ds + '...', function (ok) {

                                        if (ok) {

                                            //
                                            var ds = req.ds;

                                            // Fix ds name
                                            ds = nx.util.toDatasetName(ds);

                                            // Delete
                                            nx.util.serviceCall('AO.DatasetDelete', {
                                                ds: ds
                                            }, nx.util.noOp);

                                            // Close
                                            nwin.safeClose();

                                        }

                                    });

                                }
                            }, '>', {
                                label: 'Pick Fields',
                                icon: 'magnifier',
                                choices: []
                            }, '>', {

                                label: 'Save',
                                icon: 'accept',
                                click: function (e) {

                                    // Get the button
                                    var xwidget = nx.util.eventGetWidget(e);

                                    // Map window
                                    var xwin = nx.bucket.getWin(xwidget);

                                    // Get the for
                                    var data = xwin.getFormData();

                                    // Save
                                    nx.desktop._updateDataset(req.ds, data);

                                    // Close
                                    xwin.safeClose();

                                }

                            }

                        ]
                    },

                    listeners: {

                        appear: function (e) {

                            //  Flag
                            nx.desktop._enterDSEdit(req.ds);

                            // Get window
                            var win = nx.util.eventGetWidgetOfClass(e, 'c._window');
                            //
                            if (win) {
                                win.processSIO(win, {
                                    fn: '$$changed.picklist'
                                });
                            }
                        },

                        beforeclose: function () {

                            // Flag
                            nx.desktop._leaveDSEdit(req.ds);

                        }

                    },

                    processSIO: function (win, event) {

                        // According to fn
                        switch (event.fn) {
                            case '$$changed.picklist':
                                // Get list
                                nx.util.serviceCall('AO.PickListList', {
                                    ds: req.ds
                                }, function (result) {
                                    // Get the button
                                    var button = win.getButtonWithLabel(win.commandToolbar, 'Pick Fields');
                                    // Set the menu
                                    if (button) {
                                        button.setMenu(nx.util.getPickListContextMenu(nwin, req.ds, result.list));
                                    }
                                });
                                break;
                        }
                    }

                });

            });

        }

    }

});