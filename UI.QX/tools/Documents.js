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

qx.Class.define('tools.Documents', {

    type: 'static',

    statics: {

        startindex: 'hidden',

        // This is what you override
        do: function (req) {

            var self = tools.Documents;

            // Meus
            var foldermenu = self.createMenuEntry(req, true);
            var docmenu = self.createMenuEntry(req, false);

            var items = [
                {
                    nxtype: 'tree',
                    top: 1,
                    left: 1,
                    width: 'default.pickWidth',
                    height: 'default.pickHeight@0.75',
                    choices: [],
                    allowEmpty: false,

                    listeners: {
                        changeSelection: function (e) {
                            //
                            var widget = nx.util.eventGetWidget(e);
                            // Get the renderer
                            var renderer = nx.util.eventGetWindow(e).getRenderer();
                            // Get the tree
                            var tree = renderer.getChildrenOfClass('c._tree')[0];


                            if (tree.getSelectionMode() === 'single') {
                                // Any?
                                if (widget) {
                                    // Must be single mode
                                    if (nx.bucket.getForm(widget).getRenderer().getChildrenOfClass('c._tree')[0].getSelectionMode() === 'single') {
                                        if (widget.getSelection) {
                                            var sel = widget.getSelection();
                                            sel.forEach(function (entry) {
                                                // Get the params
                                                var params = nx.bucket.getParams(entry);
                                                // Any?
                                                if (params) {
                                                    // Get the path
                                                    var path = params.path;
                                                    // Any?
                                                    if (path) {
                                                        // Get the callback
                                                        var cb = params.cb;
                                                        // Do the callback
                                                        if (cb) {
                                                            cb({
                                                                path: path,
                                                                caller: win
                                                            });
                                                        }
                                                    }
                                                }
                                            });
                                            widget.resetSelection();
                                        }
                                    }
                                }
                            }
                            //delete win._processed;
                        }
                    }

                }, {
                    nxtype: 'upload',
                    width: 'default.pickWidth',
                    height: 1,
                    left: 1,
                    label: '',
                    labelWidth: 0
                }
            ];

            if (req.id !== nx.setup.templatesID) {
                items.push({
                    nxtype: 'button',
                    width: 'default.pickWidth',
                    height: 1,
                    left: 1,
                    label: '',
                    labelWidth: 0,
                    icon: nx.setup.viaWebIcon,
                    value: nx.setup.viaWeb + 'Upload',
                    click: function () {
                        nx.fs.remoteLink(
                            {
                                type: 'upload',
                                limit: 3 * 60,
                                uses: 25,
                                value: {
                                    ds: req.ds,
                                    id: req.id
                                }
                            }, 'ux/capture', req.caller, nx.setup.viaWeb + 'Upload');
                    }
                });
            }

            var tb, cmds;

            if (!req.fsfn) {
                var tbitems = [{
                    label: 'Select',
                    icon: 'tick',
                    toggle: true,
                    click: function (e) {
                        // Get the widget
                        var widget = nx.util.eventGetWidget(e);
                        // Get the renderer
                        var renderer = nx.util.eventGetWindow(e).getRenderer();
                        // Get the tree
                        var tree = renderer.getChildrenOfClass('c._tree')[0];
                        //
                        var selMode = tree.getSelectionMode();
                        //
                        switch (selMode) {
                            case 'single':
                                tree.setSelectionMode('multi');
                                break;
                            default:
                                // Reset
                                tree.setSelectionMode('single');
                                break;
                        }
                    }
                }];

                if (req.id !== nx.setup.templatesID) {
                    tbitems.push('>');
                    tbitems.push({
                        label: 'Display Context',
                        icon: 'chart_organisation',
                        click: function (e) {
                            // Get the widget
                            var widget = nx.util.eventGetWidget(e);
                            var params = nx.bucket.getParams(widget);
                            //
                            var data;
                            if (req.caller) {
                                data = req.caller.getFormData();
                            }
                            // Process
                            nx.util.serviceCall('AO.ObjectGetExploded', {
                                ds: req.ds,
                                id: req.id,
                                data: data || {}
                            }, function (result) {
                                // Did we get a path?
                                if (result && result.schema) {
                                    // Call
                                    nx.util.runTool('Message', {
                                        caption: 'Context',
                                        msg: JSON.stringify(JSON.parse(result.schema), null, 2)
                                    });
                                }
                            });
                        }
                    });
                }

                tb = {
                    items: tbitems
                };
            } else {
                cmds = {

                    items: [
                        '>', {
                            label: req.fslabel,
                            icon: req.fsicon,
                            click: function (e) {// Get the widget
                                var widget = nx.util.eventGetWidget(e);
                                var win = nx.util.eventGetWindow(e);
                                // Get the renderer
                                var renderer = win.getRenderer();
                                // Get the tree
                                var tree = renderer.getChildrenOfClass('c._tree')[0];
                                // Get selection
                                var sel = tree.getSelection();
                                //
                                var list = [];
                                sel.forEach(function (entry) {
                                    list.push(nx.bucket.getParams(entry).path);
                                });
                                // Save
                                req.attachments = list;
                                // Close
                                win.safeClose();
                                // Call
                                nx.util.runTool('Comm', req);
                            }
                        }
                    ]

                }
            }

            var win = nx.desktop.addWindow({

                nxid: 'docs_' + req.ds + '_' + req.id,
                caption: req.fullcaption || ('Documents ' + req.caption),
                icon: 'folder',
                caller: req.caller,

                items: items,

                topToolbar: tb,
                commands: cmds,

                listeners: {

                    appear: function () {

                        var self = this;

                        // Set the path
                        var path = '/f/ao' + nx.bucket.getWindowID(self).substr(4).replace(/_/g, '/').replace(/\/\//g, '/_');
                        // Button
                        var button = self.getRenderer().getChildren()[1];
                        // Setup upload
                        button._path = path;

                        self.processSIO(self, {
                            fn: '$$changed.document',
                            message: {
                                ds: req.ds,
                                id: req.id
                            }
                        });

                        if (req.fsfn) {
                            // Get the renderer
                            var renderer = self.getRenderer();
                            // Get the tree
                            var tree = renderer.getChildrenOfClass('c._tree')[0];
                            tree.setSelectionMode('multi');
                        }

                    }

                },

                processSIO: function (win, event) {

                    var self = this;

                    // According to function
                    switch (event.fn) {

                        case '$$changed.document':

                            // Is this the droid I am lookig for?
                            if (req.ds === event.message.ds && (req.id === event.message.id || req.ds === event.message.id)) {

                                // Get datasets
                                nx.util.serviceCall('Docs.DocumentList', {
                                    ds: req.ds,
                                    id: req.id
                                }, function (result) {
                                    var path = '/ao/' + req.ds + '/' + req.id + '/';
                                    // Make menu
                                    var menu = tools.Documents.createFileMenu({
                                        label: 'Documents',
                                        items: result.list,
                                        path: path
                                    }, foldermenu, docmenu, win);

                                    // 
                                    var tree = self.getFieldsOfClass('c._tree')

                                    // Update
                                    if (tree.length) {
                                        nx.setup.choices(tree[0], {
                                            choices: menu,
                                            allowEmpty: false,

                                        });
                                    }

                                });
                            }
                            break;

                    }
                }

            });

        },

        processCall: function (win, widget, cb) {

            // Get the renderer
            var renderer = win.getRenderer();
            // Get the tree
            var tree = renderer.getChildrenOfClass('c._tree')[0];
            // Get the object info
            var objinfo = nx.bucket.getParams(nx.bucket.getCaller(win));
            // Get selection
            var sel = tree.getSelection();
            // List
            var list = [];
            // Room for the callback
            var xcb;

            // Any selected?
            if (sel.length) {
                sel.forEach(function (entry) {
                    // Get params
                    var params = nx.bucket.getParams(entry);
                    // Do we have a path?
                    if (params.path) {
                        // Add path
                        list.push(params.path);
                        // And the callback
                        if (!xcb) {
                            xcb = nx.bucket.getParams(entry).cb;
                        }
                    }
                });
                tree.resetSelection();
            }

            // Get the widget params
            var params = nx.bucket.getParams(widget);
            // Any?
            if (params) {
                // Add path
                if (params.path && list.indexOf(params.path) === -1) list.push(params.path);
                // And the callback
                xcb = params.cb;
            }

            // Any?
            if (list.length) {
                //
                cb(objinfo, list, xcb);
            }
        },

        createFileMenu: function (data, foldermenu, docmenu, win) {

            var ans = [];

            // Assure array
            if (!Array.isArray(data)) data = [data];

            //
            var fm = null;
            if (foldermenu) {
                fm = {
                    items: foldermenu
                };
            }
            var dm = null;
            if (docmenu) {
                dm = {
                    items: docmenu
                };
            }

            // Loop thru
            data.forEach(function (entry) {
                // Folder?
                if (entry.items) {
                    var folder = entry.path;
                    if (!nx.util.endsWith(folder, '/')) folder += '/';
                    if (!nx.util.startsWith(folder, '/')) folder = '/' + folder;
                    ans.push({
                        label: entry.name,
                        icon: 'folder',
                        win: win,
                        path: folder,
                        choices: tools.Documents.createFileMenu(entry.items, foldermenu, docmenu, win),
                        contextMenu: fm,

                        drag: {
                            onDrop: function (e) {
                                var widget = nx.util.eventGetWidget(e);
                                var source = e.getRelatedTarget();

                                // Process
                                nx.util.serviceCall('Docs.TDMoveTo', {
                                    path: nx.bucket.getParams(source).path,
                                    folder: nx.bucket.getParams(widget).path
                                }, nx.util.noOp);
                                var a = 1;
                            }
                        }

                    });
                }
            });

            // Loop thru
            data.forEach(function (entry) {
                // Folder?
                if (!entry.items) {

                    // Default icon
                    var icon = 'download';

                    // Get the extension
                    var pos = entry.name.lastIndexOf('.');
                    if (pos !== -1) {
                        var ext = entry.name.substr(pos + 1).toLowerCase();
                        switch (ext) {
                            case 'pdf':
                                icon = 'pdf';
                                break;
                            case 'odt':
                                icon = 'odt';
                                break;
                            case 'jpeg':
                            case 'jpg':
                            case 'png':
                            case 'gif':
                            case 'svg':
                                icon = 'photo';
                                break;
                            case 'mp4':
                            case 'webm':
                            case 'avi':
                            case 'mov':
                            case 'mpg':
                            case 'wmv':
                            case 'flv':
                            case 'mkv':
                            case 'ogv':
                            case '3gp':
                            case '3g2':
                                icon = 'drive_web';
                                break;
                        }
                    }

                    var folder = entry.path;
                    if (!nx.util.startsWith(folder, '/')) folder = '/' + folder;

                    ans.push({
                        label: entry.name,
                        path: folder,
                        icon: icon,
                        cb: function (req) {
                            nx.fs.view(req);
                        },
                        win: win,
                        contextMenu: dm,

                        drag: {
                            onStart: function (e) {
                                e.addAction('move');
                            }
                        }
                    });
                }
            });

            return ans;
        },

        createMenuEntry: function (req, asfolder) {

            var cm = [];

            if (asfolder) {
                cm.push({
                    label: 'Add Folder',
                    icon: 'folder_add',
                    click: function (e) {
                        //
                        var widget = nx.util.eventGetWidget(e);
                        var params = nx.bucket.getParams(widget);
                        //
                        nx.util.runTool('Input', {
                            label: 'Name',
                            atOk: function (name) {
                                // Call
                                nx.util.serviceCall('Docs.TDAddFolder', {
                                    ds: req.ds,
                                    id: req.id,
                                    name: name,
                                    folder: params.path
                                });
                            }
                        });
                    }
                });
                cm.push({
                    label: 'Add Document',
                    icon: 'page_add',
                    click: function (e) {
                        //
                        var widget = nx.util.eventGetWidget(e);
                        var params = nx.bucket.getParams(widget);
                        //
                        nx.util.runTool('NewDocument', {
                            label: 'Name',
                            ds: req.ds,
                            id: req.id,
                            atOk: function (name, template) {
                                // Call
                                nx.util.serviceCall('Docs.TDAddDocument', {
                                    ds: req.ds,
                                    id: req.id,
                                    name: name,
                                    template: template,
                                    folder: params.path,
                                    data: nx.bucket.getCaller(params.win).getFormData(),
                                    nomerge: req.id === nx.setup.templatesID
                                }, function (result) {
                                    if (result && result.path) {
                                        nx.fs.editDOCX({
                                            path: result.path
                                        });
                                    }
                                });
                            }
                        });
                    }
                });
                cm.push('-');
            }

            cm.push({
                label: 'View',
                icon: 'monitor',
                click: function (e) {
                    // Get the widget
                    var widget = nx.util.eventGetWidget(e);
                    var params = nx.bucket.getParams(widget);
                    // Call
                    tools.Documents.processCall(params.win, widget, function (objinfo, sel, cb) {
                        // Any?
                        if (sel) {
                            //
                            // Process
                            nx.util.serviceCall('Docs.TDAsPDF', {
                                ds: objinfo.ds,
                                id: objinfo.id,
                                list: sel
                            }, function (result) {
                                // Did we get a path?
                                if (result && result.path) {
                                    // Call
                                    nx.fs.viewPDF({
                                        path: result.path,
                                        caller: params.win
                                    });
                                }
                            });
                        }
                    });
                }
            });

            cm.push({
                label: 'Download',
                icon: 'download',
                click: function (e) {
                    // Get the widget
                    var widget = nx.util.eventGetWidget(e);
                    var params = nx.bucket.getParams(widget);
                    // Call
                    tools.Documents.processCall(params.win, widget, function (objinfo, sel, cb) {
                        // Any?
                        if (sel) {
                            // Loop thru
                            nx.fs.download('/f' + sel);
                        }
                    });
                }
            });

            cm.push({
                label: 'As PDF',
                icon: 'pdf',
                click: function (e) {
                    // Get the widget
                    var widget = nx.util.eventGetWidget(e);
                    var params = nx.bucket.getParams(widget);
                    // Call
                    tools.Documents.processCall(params.win, widget, function (objinfo, sel, cb) {
                        // Any?
                        if (sel) {
                            // Get the name
                            nx.util.runTool('Input', {
                                label: 'Name',
                                atOk: function (name) {
                                    //
                                    params.win._processed = true;
                                    // Process
                                    nx.util.serviceCall('Docs.TDAsPDF', {
                                        ds: objinfo.ds,
                                        id: objinfo.id,
                                        list: sel,
                                        name: name
                                    }, function (result) {
                                        // Did we get a path?
                                        if (result && result.path) {
                                            // Call
                                            nx.fs.viewPDF({
                                                path: result.path,
                                                caller: params.win
                                            });
                                        }
                                    });
                                }
                            });
                        }
                    });
                }
            })

            cm.push({
                label: 'Rename',
                icon: 'tag_blue_edit',
                click: function (e) {
                    // Get the widget
                    var widget = nx.util.eventGetWidget(e);
                    var params = nx.bucket.getParams(widget);
                    // Call
                    tools.Documents.processCall(params.win, widget, function (objinfo, sel, cb) {
                        // Any?
                        if (sel) {
                            nx.util.runTool('Input', {
                                label: 'New name',
                                atOk: function (result) {
                                    // Process
                                    nx.util.serviceCall('Docs.TDRename', {
                                        ds: objinfo.ds,
                                        id: objinfo.id,
                                        list: sel,
                                        path: '/ao/' + req.ds + '/' + req.id + '/',
                                        name: result
                                    }, function (result) {
                                    });
                                }
                            });
                        }
                    });
                }
            });

            cm.push({
                label: 'Restore',
                icon: 'database_gear',
                click: function (e) {
                    // Get the widget
                    var widget = nx.util.eventGetWidget(e);
                    var params = nx.bucket.getParams(widget);
                    // Call
                    tools.Documents.processCall(params.win, widget, function (objinfo, sel, cb) {
                        // Any?
                        if (sel) {
                            // Process
                            nx.util.serviceCall('Docs.TDRestore', {
                                ds: objinfo.ds,
                                id: objinfo.id,
                                list: sel,
                                path: '/ao/' + req.ds + '/' + req.id + '/'
                            }, function (result) {
                                var count = '0';
                                var plural = 's';
                                if (result) {
                                    count = result.count || count;
                                    plural = result.plural || plural;
                                }
                                nx.util.notifyInfo(count + ' document' + plural + ' restored');

                            });
                        }
                    });
                }
            });

            if (req.id === nx.setup.templatesID) {

                cm.push({
                    label: 'Merge map',
                    icon: 'page',
                    click: function (e) {
                        // Get the widget
                        var widget = nx.util.eventGetWidget(e);
                        var params = nx.bucket.getParams(widget);
                        // Call
                        tools.Documents.processCall(params.win, widget, function (objinfo, sel, cb) {
                            // Any?
                            if (sel) {
                                // Loop thru
                                sel.forEach(function (entry) {
                                    // Call
                                    nx.util.runTool('MergeMap', {
                                        path: entry,
                                        caller: params.win
                                    });
                                });
                            }
                        });
                    }
                });

            } else if (!asfolder) {
                cm.push();
            }

            var del = true;

            if (!req.fsfn) {
                if (nx.util.hasValue(nx.desktop.user.geteMailName()) && nx.desktop.user.getIsSelector('EMAIL')) {
                    // Make room
                    if (del) {
                        cm.push('-');
                        del = false;
                    }
                    // Add
                    cm.push({
                        label: 'EMail',
                        icon: 'email',
                        click: function (e) {
                            // Get the widget
                            var widget = nx.util.eventGetWidget(e);
                            var params = nx.bucket.getParams(widget);
                            // Call
                            tools.Documents.processCall(params.win, widget, function (objinfo, sel, cb) {
                                // Any?
                                if (sel) {
                                    nx.util.runTool('Comm', {
                                        fsfn: 'email',
                                        fslabel: 'EMail',
                                        fsicon: 'email',
                                        ds: params.ds,
                                        id: params.id,
                                        caller: params.win,
                                        fullcaption: 'Select documents to EMail',
                                        askto: true,
                                        attachments: sel
                                    });
                                }
                            });
                        }
                    });
                }

                if (nx.util.hasValue(nx.desktop.user.getTwilioPhone()) && nx.desktop.user.getIsSelector('TELE')) {
                    // Make room
                    if (del) {
                        cm.push('-');
                        del = false;
                    }
                    // Add
                    cm.push({
                        label: 'SMS',
                        icon: 'phone',
                        click: function (e) {
                            // Get the widget
                            var widget = nx.util.eventGetWidget(e);
                            var params = nx.bucket.getParams(widget);
                            // Call
                            tools.Documents.processCall(params.win, widget, function (objinfo, sel, cb) {
                                // Any?
                                if (sel) {
                                    nx.util.runTool('Comm', {
                                        fsfn: 'sms',
                                        fslabel: 'SMS',
                                        fsicon: 'phone',
                                        ds: params.ds,
                                        id: params.id,
                                        caller: win,
                                        fullcaption: 'Select documents to SMS',
                                        askto: true,
                                        attachments: sel
                                    });
                                }
                            });
                        }
                    });
                }
            }

            if (nx.desktop.user.opAllowed(req.ds, 'x')) {
                cm.push('-');
                cm.push({
                    label: 'Delete',
                    icon: 'cancel',
                    click: function (e) {
                        // Get the widget
                        var widget = nx.util.eventGetWidget(e);
                        var params = nx.bucket.getParams(widget);
                        // Call
                        tools.Documents.processCall(params.win, widget, function (objinfo, sel, cb) {
                            // Any?
                            if (sel) {
                                // Confirm
                                nx.util.runTool('Confirm', {
                                    caption: 'Deleting document' + (sel.length === 1 ? '' : 's') + '...',
                                    msg: 'Are you sure?',
                                    atOk: function () {
                                        //
                                        params.win._processed = true;
                                        // Process
                                        nx.util.serviceCall('Docs.TDDelete', {
                                            ds: objinfo.ds,
                                            id: objinfo.id,
                                            list: sel
                                        }, function () {

                                            params.win.processSIO(self, {
                                                fn: '$$changed.document',
                                                message: {
                                                    ds: req.ds,
                                                    id: req.id
                                                }
                                            });

                                        });
                                    }
                                });
                            }
                        });
                    }
                });
            }

            return cm;
        }

    }
});