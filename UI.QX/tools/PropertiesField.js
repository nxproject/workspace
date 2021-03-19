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

qx.Class.define('tools.PropertiesField', {

    type: 'static',

    statics: {

        startindex: 'hidden',

        // This is what you override
        do: function (req) {

            // Get the wigets
            var widgets = req.widget;
            if (!Array.isArray(widgets)) widgets = [widgets];

            // Get the holder
            var holder = [];
            var params, form, view, ds, win, dsname, oviewname, viewname;

            // Spacing
            var rowSpacing = nx.setup.rowHeight + nx.setup.rowSpacing;
            var colSpacing = nx.setup.colWidth + nx.setup.colSpacing;

            var fldname;

            // Loop thru
            widgets.forEach(function (widget) {

                params = nx.bucket.getParams(widget);

                // Map
                if (!form) {
                    form = nx.bucket.getForm(widget);
                    view = nx.bucket.getView(form);
                    ds = nx.bucket.getDataset(form);
                    win = nx.bucket.getWin(widget);
                    dsname = nx.util.toDatasetName(ds._id);
                    oviewname = nx.util.toViewName(view._id);
                    viewname = params.inview || oviewname;
                }

                var bounds = nx.util.getRelativeBounds(widget);
                if (!bounds.top || !bounds.left) {
                    bounds.top = nx.util.toNumber(params.top);
                    bounds.left = nx.util.toNumber(params.left);
                    bounds.height = nx.util.toNumber(params.height);
                    bounds.width = nx.util.toNumber(params.width);
                } else {
                    bounds.top = nx.util.fromRelative(bounds.top, rowSpacing);
                    bounds.left = nx.util.fromRelative(bounds.left, colSpacing);
                    bounds.height = nx.util.fromRelative(bounds.height, nx.setup.rowHeight, 0);
                    bounds.width = nx.util.fromRelative(bounds.width, nx.setup.colWidth, 0);
                }

                if (req.params) {
                    bounds.top = req.params.top;
                    bounds.left = req.params.left;
                    bounds.height = req.params.height;
                    bounds.width = req.params.width;
                }

                if (bounds.top < 1) bounds.top = 1;
                if (bounds.left < 1) bounds.left = 1;
                if (bounds.height < 1) bounds.height = 1;
                if (bounds.width < 1) bounds.width = 10;

                fldname = params.aoFld;
                var sett = ds.fields[fldname] || {};

                var page = [

                    {
                        nxtype: 'tabs',
                        top: 1,
                        left: 1,
                        height: 16,
                        width: 30,
                        tabsAt: 'right',
                        items: [
                            {
                                caption: ' Info',
                                items: [
                                    {
                                        nxtype: 'string',
                                        top: 1,
                                        left: 1,
                                        width: nx.default.get('default.fieldWidth') - 6,
                                        label: 'Label',
                                        aoFld: fldname + '_label',
                                        value: params.label || ''
                                    }, {
                                        nxtype: 'int',
                                        top: 1,
                                        left: 1 + nx.default.get('default.fieldWidth') - 6,
                                        width: 5,
                                        label: 'Label Size',
                                        aoFld: fldname + '_labelWidth',
                                        value: params.labelWidth || ''
                                    }, {
                                        nxtype: 'string',
                                        top: 2,
                                        left: 1,
                                        width: 'default.fieldWidth',
                                        label: 'Placeholder',
                                        aoFld: fldname + '_placeholder',
                                        value: params.placeholder || ''
                                    }, {
                                        nxtype: 'int',
                                        top: 4,
                                        left: 1,
                                        width: 5,
                                        label: 'Top',
                                        labelWidth: 3,
                                        aoFld: fldname + '_top',
                                        value: bounds.top.toString()
                                    }, {
                                        nxtype: 'int',
                                        top: 4,
                                        left: 6,
                                        width: 5,
                                        label: 'Height',
                                        labelWidth: 3,
                                        aoFld: fldname + '_height',
                                        value: bounds.height.toString()
                                    }, {
                                        nxtype: 'int',
                                        top: 5,
                                        left: 1,
                                        width: 5,
                                        label: 'Left',
                                        labelWidth: 3,
                                        aoFld: fldname + '_left',
                                        value: bounds.left.toString()
                                    }, {
                                        nxtype: 'int',
                                        top: 5,
                                        left: 6,
                                        width: 5,
                                        label: 'Width',
                                        labelWidth: 3,
                                        aoFld: fldname + '_width',
                                        value: bounds.width.toString()
                                    }, {
                                        nxtype: 'xfieldtype',
                                        top: 7,
                                        left: 1,
                                        width: 9,
                                        label: 'Type',
                                        aoFld: fldname + '_nxtype',
                                        value: params.nxtype
                                    }, {
                                        nxtype: 'string',
                                        top: 8,
                                        left: 1,
                                        width: 11,
                                        label: 'Default Value',
                                        aoFld: fldname + '_defaultvalue',
                                        value: sett.defaultvalue
                                    }, {
                                        nxtype: 'boolean',
                                        top: 9,
                                        left: 1,
                                        width: 9,
                                        label: 'Read Only',
                                        aoFld: fldname + '_ro',
                                        value: params.ro === 'y'
                                    }, {
                                        nxtype: 'boolean',
                                        top: 10,
                                        left: 1,
                                        width: 9,
                                        label: 'Use Index',
                                        aoFld: fldname + '_isindex',
                                        value: sett.isindex === 'y'
                                    }
                                ]
                            }, {
                                caption: 'If combobox/list',
                                items: [
                                    {
                                        nxtype: 'textarea',
                                        top: 1,
                                        left: 1,
                                        width: 'default.fieldWidth',
                                        height: 4,
                                        label: 'Choices',
                                        aoFld: fldname + '_choices',
                                        value: sett.choices
                                    }
                                ]
                            }, {
                                caption: 'If computed',
                                items: [
                                    {
                                        nxtype: 'string',
                                        top: 1,
                                        left: 1,
                                        width: 'default.fieldWidth',
                                        height: 1,
                                        label: 'JS Statement',
                                        aoFld: fldname + '_compute',
                                        value: sett.compute
                                    }
                                ]
                            }, {
                                caption: 'If Link/LU',
                                items: [
                                    {
                                        nxtype: 'keyword',
                                        top: 1,
                                        left: 1,
                                        width: 'default.fieldWidth',
                                        label: 'Dataset',
                                        aoFld: fldname + '_linkds',
                                        value: sett.linkds
                                    }, {
                                        nxtype: 'string',
                                        top: 2,
                                        left: 1,
                                        width: 'default.fieldWidth',
                                        label: 'LU Map',
                                        aoFld: fldname + '_lumap',
                                        value: sett.lumap
                                    }
                                ]

                            }, {
                                caption: 'If grid/tab',
                                items: [
                                    {
                                        nxtype: 'lower',
                                        top: 1,
                                        left: 1,
                                        width: 'default.fieldWidth',
                                        label: 'View(s)',
                                        aoFld: fldname + '_gridview',
                                        value: sett.gridview
                                    }, {
                                        nxtype: 'combobox',
                                        top: 2,
                                        left: 1,
                                        width: 'default.fieldWidth',
                                        label: 'Align',
                                        aoFld: fldname + '_tabsAt',
                                        value: sett.tabsAt || 'top',
                                        choices: ['top', 'right', 'bottom', 'left']
                                    }
                                ]
                            }, {
                                caption: 'If comm',
                                items: [
                                    {
                                        nxtype: 'keyword',
                                        top: 1,
                                        left: 1,
                                        width: 'default.fieldWidth',
                                        label: 'Reference',
                                        aoFld: fldname + '_ref',
                                        value: sett.ref
                                    }
                                ]
                            }, {
                                caption: nx.setup.viaWeb + 'Person',
                                items: [
                                    {
                                        nxtype: 'string',
                                        top: 1,
                                        left: 1,
                                        width: 'default.fieldWidth',
                                        label: 'Name',
                                        aoFld: fldname + '_relname',
                                        value: sett.relname
                                    }, {
                                        nxtype: 'string',
                                        top: 2,
                                        left: 1,
                                        width: 'default.fieldWidth',
                                        label: 'Address',
                                        aoFld: fldname + '_reladdr',
                                        value: sett.reladdr
                                    }, {
                                        nxtype: 'string',
                                        top: 3,
                                        left: 1,
                                        width: 'default.fieldWidth',
                                        label: 'City',
                                        aoFld: fldname + '_relcity',
                                        value: sett.relcity
                                    }, {
                                        nxtype: 'string',
                                        top: 4,
                                        left: 1,
                                        width: 'default.fieldWidth',
                                        label: 'State',
                                        aoFld: fldname + '_relstate',
                                        value: sett.relstate
                                    }, {
                                        nxtype: 'string',
                                        top: 5,
                                        left: 1,
                                        width: 'default.fieldWidth',
                                        label: 'ZIP',
                                        aoFld: fldname + '_relzip',
                                        value: sett.relzip
                                    }, {
                                        nxtype: 'string',
                                        top: 6,
                                        left: 1,
                                        width: 'default.fieldWidth',
                                        label: 'Phone',
                                        aoFld: fldname + '_relphone',
                                        value: sett.relphone
                                    }, {
                                        nxtype: 'string',
                                        top: 7,
                                        left: 1,
                                        width: 'default.fieldWidth',
                                        label: 'EMail',
                                        aoFld: fldname + '_relemail',
                                        value: sett.relemail
                                    }, {
                                        nxtype: 'string',
                                        top: 8,
                                        left: 1,
                                        width: 'default.fieldWidth',
                                        label: 'DOB',
                                        aoFld: fldname + '_reldob',
                                        value: sett.reldob
                                    }, {
                                        nxtype: 'string',
                                        top: 9,
                                        left: 1,
                                        width: 'default.fieldWidth',
                                        label: 'DL',
                                        aoFld: fldname + '_reldl',
                                        value: sett.reldl
                                    }
                                ]
                            }, {
                                caption: nx.setup.viaWeb + 'Quorum',
                                items: [
                                    {
                                        nxtype: 'string',
                                        top: 1,
                                        left: 1,
                                        width: 'default.fieldWidth',
                                        label: 'Subject',
                                        aoFld: fldname + '_relsubj',
                                        value: sett.relsubj
                                    }, {
                                        nxtype: 'string',
                                        top: 2,
                                        left: 1,
                                        width: 'default.fieldWidth',
                                        label: 'Description',
                                        aoFld: fldname + '_reldesc',
                                        value: sett.reldesc
                                    }, {
                                        nxtype: 'string',
                                        top: 3,
                                        left: 1,
                                        width: 'default.fieldWidth',
                                        label: 'Location',
                                        aoFld: fldname + '_relloc',
                                        value: sett.relloc
                                    }, {
                                        nxtype: 'string',
                                        top: 4,
                                        left: 1,
                                        width: 'default.fieldWidth',
                                        label: 'Starts On',
                                        aoFld: fldname + '_relstarton',
                                        value: sett.relstarton
                                    }, {
                                        nxtype: 'string',
                                        top: 5,
                                        left: 1,
                                        width: 'default.fieldWidth',
                                        label: 'Ends On',
                                        aoFld: fldname + '_relendon',
                                        value: sett.relendon
                                    }
                                ]
                            }, {
                                caption: nx.setup.viaWeb + 'VIN',
                                items: [
                                    {
                                        nxtype: 'keyword',
                                        top: 1,
                                        left: 1,
                                        width: 'default.fieldWidth',
                                        label: 'Year',
                                        aoFld: fldname + '_relvinyear',
                                        value: sett.relvinyear
                                    }, {
                                        nxtype: 'keyword',
                                        top: 2,
                                        left: 1,
                                        width: 'default.fieldWidth',
                                        label: 'Make',
                                        aoFld: fldname + '_relvinmake',
                                        value: sett.relvinmake
                                    }, {
                                        nxtype: 'keyword',
                                        top: 3,
                                        left: 1,
                                        width: 'default.fieldWidth',
                                        label: 'Model',
                                        aoFld: fldname + '_relvinmodel',
                                        value: sett.relvinmodel
                                    }, {
                                        nxtype: 'keyword',
                                        top: 4,
                                        left: 1,
                                        width: 'default.fieldWidth',
                                        label: 'Series',
                                        aoFld: fldname + '_relvinseries',
                                        value: sett.relvinseries
                                    }, {
                                        nxtype: 'keyword',
                                        top: 5,
                                        left: 1,
                                        width: 'default.fieldWidth',
                                        label: 'Type',
                                        aoFld: fldname + '_relvintype',
                                        value: sett.relvintype
                                    }, {
                                        nxtype: 'keyword',
                                        top: 6,
                                        left: 1,
                                        width: 'default.fieldWidth',
                                        label: 'Trim',
                                        aoFld: fldname + '_relvintrim',
                                        value: sett.relvintrim
                                    }, {
                                        nxtype: 'keyword',
                                        top: 7,
                                        left: 1,
                                        width: 'default.fieldWidth',
                                        label: 'Doors',
                                        aoFld: fldname + '_relvindoors',
                                        value: sett.relvindoors
                                    }, {
                                        nxtype: 'keyword',
                                        top: 8,
                                        left: 1,
                                        width: 'default.fieldWidth',
                                        label: 'Cyls',
                                        aoFld: fldname + '_relvincyl',
                                        value: sett.relvincyl
                                    }, {
                                        nxtype: 'keyword',
                                        top: 9,
                                        left: 1,
                                        width: 'default.fieldWidth',
                                        label: 'Displacement',
                                        aoFld: fldname + '_relvindisp',
                                        value: sett.relvindisp
                                    }, {
                                        nxtype: 'keyword',
                                        top: 10,
                                        left: 1,
                                        width: 'default.fieldWidth',
                                        label: 'Fuel',
                                        aoFld: fldname + '_relvinfuel',
                                        value: sett.relvinfuel
                                    }, {
                                        nxtype: 'keyword',
                                        top: 11,
                                        left: 1,
                                        width: 'default.fieldWidth',
                                        label: 'Speed',
                                        aoFld: fldname + '_relvinspeed',
                                        value: sett.relvinspeed
                                    }, {
                                        nxtype: 'keyword',
                                        top: 12,
                                        left: 1,
                                        width: 'default.fieldWidth',
                                        label: 'Transmission',
                                        aoFld: fldname + '_relvintrans',
                                        value: sett.relvintrans
                                    }
                                ]
                            }
                        ]
                    }
                ];

                // Add tab
                holder.push({
                    caption: fldname,
                    items: page
                });
            });

            var inview = '';
            if (req.inview) inview = ' in ' + req.inview;

            var cmds = [];
            if (widgets.length === 1) {
                cmds.push('>');
                cmds.push({
                    label: 'Delete',
                    icon: 'database_delete',
                    click: function (e) {

                        // Get the button
                        var xwidget = nx.util.eventGetWidget(e);

                        // Map window
                        var xwin = nx.bucket.getWin(xwidget);

                        //var fldname = params.aoFld;

                        // Get the params
                        var viewdef = nx.bucket.getView(nx.util.eventGetWindow(e));
                        var ds = nx.util.toDatasetName(viewdef._ds);
                        var view = nx.util.toViewName(viewdef._id);

                        nx.util.confirm('Are you sure?', 'Delete ' + fldname + '?', function (ok) {

                            if (ok) {

                                // Delete
                                nx.desktop._deleteViewFields(ds, view, fldname);

                                // Save
                                var caller = nx.bucket.getCaller(win);

                                // Close
                                xwin.safeClose();

                                // Close editor
                                win.safeClose();

                                // And reopen
                                nx.util.runTool('Object', {
                                    ds: ds,
                                    view: oviewname, //view._id.substr(6),
                                    sysmode: true,
                                    caller: caller // nx.bucket.getParentCaller(req.caller)
                                });
                            }

                        });

                    }
                });
            }
            cmds.push('>');
            cmds.push({

                label: 'Ok',
                icon: 'accept',
                click: function (e) {

                    // Get the button
                    var xwidget = nx.util.eventGetWidget(e);

                    // Map window
                    var xwin = nx.bucket.getWin(xwidget);

                    // Parse
                    var ds = nx.bucket.getDataset(xwin);
                    var view = nx.bucket.getView(xwin);

                    // Get the for
                    var data = xwin.getFormData();
                    // Make room
                    var fdefs = {};
                    // Loop thru
                    Object.keys(data).forEach(function (fld) {
                        var pos = fld.indexOf('_');
                        if (pos !== -1) {
                            var fieldname = fld.substr(0, pos);
                            if (nx.util.hasValue(fieldname)) {
                                var subname = fld.substr(pos + 1);
                                if (nx.util.hasValue(subname)) {
                                    if (!fdefs[fieldname]) fdefs[fieldname] = {};
                                    fdefs[fieldname][subname] = data[fld];
                                }
                            }
                        }
                    });
                    // Get the field names
                    var fnames = Object.keys(fdefs);
                    // Loop thru
                    fnames.forEach(function (fieldname, index) {
                        //
                        // Pseudo data
                        var data = fdefs[fieldname];

                        // Create dataset entry
                        dsfield = {
                            name: fieldname,
                            label: data.label,
                            nxtype: data.nxtype,
                            defaultvalue: data.defaultvalue,
                            gridview: data.gridview,
                            linkds: data.linkds,
                            compute: data.compute,
                            lumap: data.lumap,
                            choices: data.choices,
                            ref: data.ref,
                            relname: data.relname,
                            reladdr: data.reladdr,
                            relcity: data.relcity,
                            relstate: data.relstate,
                            relzip: data.relzip,
                            relphone: data.relphone,
                            relemail: data.relemail,
                            relsubj: data.relsubj,
                            reldesc: data.reldesc,
                            relloc: data.relloc,
                            relstarton: data.relstarton,
                            relendon: data.relendon,
                            tabsAt: data.tabsAt,
                            isindex: data.isindex ? 'y' : 'n',
                            relvinyear: data.relvinyear,
                            relvinmake: data.relvinmake,
                            relvinmodel: data.relvinmodel,
                            relvinseries: data.relvinseries,
                            relvintype: data.relvintype,
                            relvintrim: data.relvintrim,
                            relvindoors: data.relvindoors,
                            relvincyl: data.relvincyl,
                            relvindisp: data.relvindisp,
                            relvinspeed: data.relvinspeed,
                            relvintrans: data.relvintrans,
                            relvinfuel: data.relvinfuel

                        };
                        // Save
                        nx.desktop._setDatasetField(ds._ds, dsfield);

                        // Create view entry
                        var viewfield = {
                            aoFld: fieldname,
                            top: nx.util.toInt(data.top),
                            left: nx.util.toInt(data.left),
                            height: nx.util.toInt(data.height),
                            width: nx.util.toInt(data.width),
                            label: data.label,
                            labelWidth: data.labelWidth,
                            nxtype: data.nxtype,
                            ro: data.ro,
                            placeholder: data.placeholder
                        };
                        // Save
                        nx.desktop._alterView(ds._ds, view._id, fieldname, viewfield, function (result) {

                            // Last one?
                            if (index >= (fnames.length - 1)) {
                                // Save
                                var caller = nx.bucket.getCaller(win);

                                // Close
                                xwin.safeClose();

                                // Close editor
                                win.safeClose();

                                // And reopen
                                nx.util.runTool('Object', {
                                    ds: ds._ds,
                                    view: oviewname,
                                    sysmode: true,
                                    caller: caller
                                });
                            }
                        });
                    });
                }

            });

            var nwin = nx.desktop.addWindow({

                caption: 'Field Properties',
                icon: 'application_form',
                defaultCommand: 'Ok',
                nxid: 'prop_' + ds,
                caller: req.caller || win,

                ds: dsname,
                view: oviewname,

                items: [
                    {
                        nxtype: 'tabs',
                        top: 1,
                        left: 1,
                        width: 35,
                        height: 22,
                        items: holder
                    }
                ],

                commands: {

                    items: cmds
                }

            });

            // Link
            nx.bucket.setDataset(nwin, ds);
            nx.bucket.setView(nwin, view);

        }

    }

});