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

qx.Class.define('tools.PropertiesPick', {

    type: 'static',

    statics: {

        startindex: 'hidden',

        // This is what you override
        do: function (req) {

            var lw = nx.setup.labelWidth;
            var ll = 8; //nx.default.get('default.fieldWidth') - nx.setup.labelWidth;

            nx.desktop._loadPick(req.ds, req.pick, function (def) {

                var nwin = nx.desktop.addWindow({

                    caption: 'Pick Properties for ' + req.pick,
                    icon: def.icon,
                    nxid: 'pick_' + req.ds + '_' + req.pick,
                    caller: req.caller,
                    labelWidth: 3,

                    items: [

                        {
                            nxtype: 'string',
                            top: 1,
                            left: 1,
                            width: 'default.fieldWidth',
                            label: 'Label',
                            aoFld: 'label',
                            value: def.label
                        }, {
                            nxtype: 'icon',
                            top: 2,
                            left: 1,
                            width: 'default.fieldWidth',
                            label: 'Icon',
                            aoFld: 'icon',
                            value: def.icon
                        }, {
                            nxtype: 'boolean',
                            top: 3,
                            left: 1,
                            width: 'default.fieldWidth',
                            label: 'Selected',
                            aoFld: 'selected',
                            value: def.selected
                        }, {
                            nxtype: 'combobox',
                            top: 4,
                            left: 1,
                            width: 'default.fieldWidth',
                            label: 'All or Any',
                            choices: ['All', 'Any'],
                            aoFld: 'allany',
                            value: def.allany
                        }, {
                            nxtype: 'tabs',
                            top: 5,
                            left: 1,
                            width: 'default.tabWidth',
                            height: 7,
                            items: [
                                {
                                    caption: '#1',
                                    items: [
                                        {
                                            nxtype: 'keyword',
                                            top: 1,
                                            left: 1,
                                            width: 'default.fieldWidth',
                                            label: 'Field',
                                            aoFld: 'field1',
                                            value: def.field1
                                        }, {
                                            nxtype: 'xqueryop',
                                            top: 2,
                                            left: lw,
                                            width: ll,
                                            label: '',
                                            aoFld: 'op1',
                                            value: def.op1
                                        }, {
                                            nxtype: 'string',
                                            top: 3,
                                            left: 1,
                                            width: 'default.fieldWidth',
                                            label: 'Value',
                                            aoFld: 'value1',
                                            value: def.value1
                                        }
                                    ]
                                }, {
                                    caption: '#2',
                                    items: [
                                        {
                                            nxtype: 'keyword',
                                            top: 1,
                                            left: 1,
                                            width: 'default.fieldWidth',
                                            label: 'Field',
                                            aoFld: 'field2',
                                            value: def.field2
                                        }, {
                                            nxtype: 'xqueryop',
                                            top: 2,
                                            left: lw,
                                            width: ll,
                                            label: '',
                                            aoFld: 'op2',
                                            value: def.op2
                                        }, {
                                            nxtype: 'string',
                                            top: 3,
                                            left: 1,
                                            width: 'default.fieldWidth',
                                            label: 'Value',
                                            aoFld: 'value2',
                                            value: def.value2
                                        }
                                    ]
                                }, {
                                    caption: '#3',
                                    items: [
                                        {
                                            nxtype: 'string',
                                            top: 1,
                                            left: 1,
                                            width: 'default.fieldWidth',
                                            label: 'Field',
                                            aoFld: 'field3',
                                            value: def.field3
                                        }, {
                                            nxtype: 'xqueryop',
                                            top: 2,
                                            left: lw,
                                            width: ll,
                                            label: '',
                                            aoFld: 'op3',
                                            value: def.op3
                                        }, {
                                            nxtype: 'string',
                                            top: 3,
                                            left: 1,
                                            width: 'default.fieldWidth',
                                            label: 'Value',
                                            aoFld: 'value3',
                                            value: def.value3
                                        }
                                    ]
                                }, {
                                    caption: '#4',
                                    items: [
                                        {
                                            nxtype: 'keyword',
                                            top: 1,
                                            left: 1,
                                            width: 'default.fieldWidth',
                                            label: 'Field',
                                            aoFld: 'field4',
                                            value: def.field4
                                        }, {
                                            nxtype: 'xqueryop',
                                            top: 2,
                                            left: lw,
                                            width: ll,
                                            label: '',
                                            aoFld: 'op4',
                                            value: def.op4
                                        }, {
                                            nxtype: 'string',
                                            top: 3,
                                            left: 1,
                                            width: 'default.fieldWidth',
                                            label: 'Value',
                                            aoFld: 'value4',
                                            value: def.value4
                                        }
                                    ]
                                }, {
                                    caption: '#5',
                                    items: [
                                        {
                                            nxtype: 'keyword',
                                            top: 1,
                                            left: 1,
                                            width: 'default.fieldWidth',
                                            label: 'Field',
                                            aoFld: 'field5',
                                            value: def.field5
                                        }, {
                                            nxtype: 'xqueryop',
                                            top: 2,
                                            left: lw,
                                            width: ll,
                                            label: '',
                                            aoFld: 'op5',
                                            value: def.op5
                                        }, {
                                            nxtype: 'string',
                                            top: 3,
                                            left: 1,
                                            width: 'default.fieldWidth',
                                            label: 'Value',
                                            aoFld: 'value5',
                                            value: def.value5
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

                                    nx.util.confirm('Are you sure?', 'Delete ' + req.pick + '...', function (ok) {

                                        if (ok) {

                                            //
                                            var ds = req.ds;

                                            // Fix ds name
                                            ds = nx.util.toDatasetName(ds);

                                            // Delete
                                            nx.util.serviceCall('AO.PickListDelete', {
                                                ds: req.ds,
                                                id: req.pick
                                            }, nx.util.noOp);

                                            // Close
                                            nwin.safeClose();

                                        }

                                    });

                                }
                            }, '>', {

                                label: 'Save',
                                icon: 'accept',
                                click: function (e) {

                                    // Get the for
                                    var data = nwin.getFormData();

                                    // Save
                                    nx.desktop._updatePick(req.ds, req.pick, data);

                                    // Close
                                    nwin.safeClose();

                                }

                            }

                        ]
                    }

                });

            });

        }

    }

});