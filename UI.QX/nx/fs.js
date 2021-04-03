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

nx.fs = {

    viewObject: function (req) {

        var self = this;

        // View
        nx.desktop.addWindowDS({
            ds: req.ds,
            id: req.id,
            view: nx.desktop.user.getDSInfo(req.ds).view,
            caller:req. caller,
            chain: req.chain
        });

    },

    download: function (path) {

        var self = this;

        // Assure proper start
        if (!nx.util.startsWith(path, '/')) path = '/' + path;

        path = encodeURI(path);
        var pos = path.lastIndexOf('/');
        var filename = path.substr(pos + 1);
        var a = document.createElement('a');
        a.href = path;
        a.download = filename;
        a.click();
        self.removeNODE(a);

    },

    view: function (req) {

        var self = this;

        var path = req.path;

        // Get the extension
        var pos = path.lastIndexOf('.');
        if (pos !== -1) {
            var ext = path.substr(pos + 1).toLowerCase();
            switch (ext) {
                case 'pdf':
                    self.viewPDF(req);
                    break;
                case 'odt':
                    self.editDOCX(req);
                    break;
                case 'jpeg':
                case 'jpg':
                case 'png':
                case 'gif':
                case 'svg':
                    self.viewImage(req);
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
                    self.viewVideo(req, ext);
                    break;
                default:
                    self.download('/f' + req.path);
                    break;
            }
        }
    },

    viewPDF: function (req) {

        var self = this;

        var path = req.path;

        // Assure proper start
        if (!nx.util.startsWith(path, '/')) path = '/' + path;

        // make caption
        var caption = path;
        var pos = caption.lastIndexOf('/');
        if (pos !== -1) caption = caption.substr(pos + 1);

        //
        var urlParams = nx.web.urlStart('file', '/f' + path);

        // Show loader
        nx.util.notifyLoadingStart();

        //
        nx.util.runTool('Webview', {
            nxid: 'pdf_' + path,
            caption: caption,
            icon: 'pdf',
            value: nx.util.loopbackURL() + '/viewers/pdf/web/viewer.html' + urlParams,
            caller: req.caller,
            chat: !req.noChat,
            adjustWidth: req.adjustWidth,
            noCenter: req.noCenter,
            topToolbar: req.topToolbar,
            bottomToolbar: req.bottomToolbar
        });

    },

    viewAsPDF: function (req) {

        var self = this;

        var path = req.path;

        // Assure proper start
        if (!nx.util.startsWith(path, '/')) path = '/' + path;

        // make caption
        var caption = path;
        var pos = caption.lastIndexOf('/');
        if (pos !== -1) caption = caption.substr(pos + 1);

        //
        var urlParams = nx.web.urlStart('file', '/fx/pdf' + path);

        // Show loader
        nx.util.notifyLoadingStart();

        //
        nx.util.runTool('Webview', {
            nxid: 'pdfx_' + path,
            caption: caption,
            icon: 'pdf',
            value: nx.util.loopbackURL() + '/viewers/pdf/web/viewer.html' + urlParams,
            caller: req.caller,
            adjustWidth: req.adjustWidth,
            noCenter: req.noCenter,
            topToolbar: req.topToolbar,
            bottomToolbar: req.bottomToolbar
        });

    },

    editDOCX: function (req) {

        var self = this;

        var path = req.path;

        // Assure proper start
        if (!nx.util.startsWith(path, '/')) path = '/' + path;

        // make caption
        var caption = path;
        var pos = caption.lastIndexOf('/');
        if (pos !== -1) caption = caption.substr(pos + 1);

        //
        var winid = 'doc_' + path;

        //
        var urlParams = nx.web.urlStart('file', '/f' + path);
        //var urlParams = nx.web.urlStart('file', '/fx/html' + path);
        urlParams = nx.web.urlAdd('winid', winid, urlParams);
        urlParams = nx.web.urlAdd('user', nx.desktop.user.getName(), urlParams);

        // Show loader
        nx.util.notifyLoadingStart();

        //
        nx.util.runTool('Webview', {
            nxid: winid,
            caption: caption,
            icon: 'docx',
            value: nx.util.loopbackURL() + '/viewers/webodf/index.html' + urlParams,
            //value: nx.util.loopbackURL() + '/viewers/tinymce/index.html' + urlParams,
            caller: req.caller, 
            chat: !req.noChat,
            adjustWidth: req.adjustWidth,
            noCenter: req.noCenter,
            topToolbar: req.topToolbar,
            bottomToolbar: req.bottomToolbar
        });

    },

    viewImage: function (req) {

        var self = this;

        var path = req.path;

        // Assure proper start
        if (!nx.util.startsWith(path, '/')) path = '/' + path;

        // make caption
        var caption = path;
        var pos = caption.lastIndexOf('/');
        if (pos !== -1) caption = caption.substr(pos + 1);

        //
        var urlParams = nx.web.urlStart('file', '/f' + path);

        // Show loader
        nx.util.notifyLoadingStart();

        //
        nx.util.runTool('Webview', {
            nxid: 'img_' + path,
            caption: caption,
            icon: 'photo',
            value: nx.util.loopbackURL() + '/viewers/image/viewer.html' + urlParams,
            caller: req.caller,
            chat: !req.noChat,
            adjustWidth: req.adjustWidth,
            noCenter: req.noCenter,
            topToolbar: req.topToolbar,
            bottomToolbar: req.bottomToolbar
        });

    },

    viewVideo: function (req, ext) {

        var self = this;

        var path = req.path;

        // Assure proper start
        if (!nx.util.startsWith(path, '/')) path = '/' + path;

        // make caption
        var caption = path;
        var pos = caption.lastIndexOf('/');
        if (pos !== -1) caption = caption.substr(pos + 1);

        //
        var urlParams = nx.web.urlStart('file', '/f' + path);
        urlParams = nx.web.urlAdd('ext', ext, urlParams);

        // Show loader
        nx.util.notifyLoadingStart();

        //
        nx.util.runTool('Webview', {
            nxid: 'video_' + path,
            caption: caption,
            icon: 'drive_web',
            value: nx.util.loopbackURL() + '/viewers/video/viewer.html' + urlParams, //?file=' + encodeURIComponent('/f' + path) + '&ext=' + ext,
            caller: req.caller,
            chat: false,
            adjustWidth: req.adjustWidth,
            noCenter: req.noCenter,
            topToolbar: req.topToolbar,
            bottomToolbar: req.bottomToolbar
        });

    },

    calendar: function (req) {

        var self = this;

        //
        var winid = 'fc_' + req.ds;

        // Show loader
        nx.util.notifyLoadingStart();

        nx.desktop._loadDataset(req.ds, function (dsdef) {

            // 
            var oo = moment(chrono.parseDate(nx.desktop.user.getSIField('officeopen') || '08:00 AM')).format('HH:mm');
            var oc = moment(chrono.parseDate(nx.desktop.user.getSIField('officeclose') || '05:00 PM')).format('HH:mm');
            var td = moment().format('YYYY-MM-DD');

            var startfld = (dsdef.calstart || '').trim();
            if (startfld.indexOf(' ') === -1 && startfld.indexOf('#') === -1) {
                startfld = startfld.replace(/[^a-z0-9]/g, '');
            } else {
                startfld = '';
            }

            var endfld = (dsdef.calend || '').trim();
            if (endfld.indexOf(' ') === -1 && endfld.indexOf('#') === -1) {
                endfld = endfld.replace(/[^a-z0-9]/g, '');
            } else {
                endfld = '';
            }

            // Only the reportable
            nx.util.reportableFields(req.ds, (dsdef.calby || ''), function (calby) {

                // Build URL
                var urlParams = nx.web.urlStart('officeopen', oo);
                urlParams = nx.web.urlAdd('officeclose', oc, urlParams);
                urlParams = nx.web.urlAdd('startdate', td, urlParams);
                urlParams = nx.web.urlAdd('ds', req.ds, urlParams);
                urlParams = nx.web.urlAdd('winid', winid, urlParams);
                urlParams = nx.web.urlAdd('mgr', (nx.desktop.user.getIsSelector('MGR') ? 'y' : 'n'), urlParams);
                if (startfld) urlParams = nx.web.urlAdd('startfld', startfld, urlParams);
                if (endfld) urlParams = nx.web.urlAdd('endfld', endfld, urlParams);
                urlParams = nx.web.urlAdd('ro', (dsdef.calRO || 'n'), urlParams);

                var tb;

                if (nx.util.hasValue(calby)) {
                    tb = ['>'];
                    nx.util.splitSpace(dsdef.calby, true).forEach(function (fld) {
                        var fcaption = dsdef.fields[fld].label;
                        if (!fcaption) fcaption = fld;
                        tb.push({
                            label: fcaption,
                            passed: fld,
                            choices: []
                        });
                    });
                    tb.push('>');
                    tb = {
                        items: tb
                    };
                }

                //
                nx.util.runTool('Webview', {
                    nxid: winid,
                    caption: 'Calendar for ' + req.desc,
                    icon: 'calendar',
                    value: nx.util.loopbackURL() + '/viewers/fullcalendar/fc.html' + urlParams,
                    chat: !req.noChat,
                    adjustWidth: req.adjustWidth,
                    noCenter: req.noCenter,
                    topToolbar: tb || req.topToolbar,
                    bottomToolbar: req.bottomToolbar || nx.util.createPickToolbar(req.ds, function (e) {
                        var self = nx.util.eventGetWidgetParent(e);
                        var calendar = nx.desktop.getTP(winid);
                        if (calendar) {
                            var query = nx.util.processPickToolbar(self);
                            calendar._nxreset(query);
                        }
                    })
                });
            }, true);
        });

    },

    remoteLink: function (data, svc, win, caption) {
        nx.util.serviceCall('AO.IEntrySet', data, function (result) {
            if (result && result.id) {
                // Make the url
                var url = nx.util.loopbackURL();
                if (svc) url += '/' + svc;
                url += '?id=' + result.id;
                // Copy to clipboard
                nx.util.copy(url);
                nx.util.notifyInfo('Link copied to clipboard - ' + result.id);
                // Is there a window?
                if (win) {
                    nx.util.runTool('QR', {
                        data: url,
                        caption: caption || 'Link',
                        caller: win
                    });
                }
            }
        });
    },

    analyze: function (req) {

        var self = this;

        // Show loader
        nx.util.notifyLoadingStart();

        nx.desktop._loadDataset(req.ds, function (dsdef) {

            // Only the reportable
            nx.util.reportableFields(req.ds, (dsdef.analizeby || ''), function (rptby) {

                //
                var urlParams = nx.web.urlStart('ds', req.ds);
                urlParams = nx.web.urlAdd('winid', 'ana_' + req.id, urlParams);

                // Pick
                if (nx.util.hasValue(dsdef.anapick)) {
                    var pick = nx.desktop._getPick(req.ds);
                    if (pick) {
                        pick = pick[dsdef.anapick];
                        if (pick) {
                            // Area
                            var query = [];
                            //
                            var i = 1;
                            while (nx.util.hasValue(pick['field' + i])) {
                                query.push({
                                    field: pick['field' + i],
                                    op: pick['op' + i],
                                    value: pick['value' + i]
                                });
                                i++;
                            }
                            urlParams = nx.web.urlAdd('pick', JSON.stringify(query), urlParams);
                        }
                    }
                }

                //
                nx.util.runTool('Webview', {
                    nxid: 'ana_' + req.id,
                    caption: 'Analyze - ' + req.id.substr(5),
                    icon: 'chart_bar',
                    value: nx.util.loopbackURL() + '/viewers/pivot/index.html' + urlParams,
                    caller: req.caller,
                    chat: false,
                    adjustWidth: req.adjustWidth,
                    noCenter: req.noCenter,
                    topToolbar: req.topToolbar,
                    bottomToolbar: req.bottomToolbar,
                    commands: {
                        items: [
                            'X', '>', {
                                label: 'Delete',
                                icon: 'application_delete',
                                click: function (e) {
                                    var widget = nx.util.eventGetWidget(e);
                                    var win = nx.bucket.getWin(widget);

                                    nx.util.confirm('Are you sure?', 'Delete ' + req.id.substr(5) + '...', function (ok) {

                                        if (ok) {

                                            //
                                            var ds = req.ds;

                                            // Fix ds name
                                            ds = nx.util.toDatasetName(ds);

                                            // Delete
                                            nx.util.serviceCall('AO.ObjectDelete', {
                                                ds: req.ds,
                                                id: req.id
                                            }, nx.util.noOp);

                                            // Close
                                            win.safeClose();

                                        }

                                    });;
                                }
                            }, '>'
                        ]
                    }
                });
            });
        });

    },

    task: function (req) {

        var self = this;

        // Show loader
        nx.util.notifyLoadingStart();

        nx.desktop._loadDataset(req.ds, function (dsdef) {

            //
            var urlParams = nx.web.urlStart('ds', req.ds);
            urlParams = nx.web.urlAdd('winid', 'tsk_' + req.id, urlParams);

            //
            nx.util.runTool('Webview', {
                nxid: 'tsk_' + req.id,
                icon: 'cog',
                caption: 'Task - ' + req.id.substr(5),
                value: nx.util.loopbackURL() + '/viewers/elsa/task.html' + urlParams,
                caller: req.caller,
                chat: false,
                adjustWidth: req.adjustWidth,
                noCenter: req.noCenter,
                topToolbar: req.topToolbar,
                bottomToolbar: req.bottomToolbar,
                commands: {
                    items: [
                        'X', '>', {
                            label: 'Delete',
                            icon: 'application_delete',
                            click: function (e) {
                                var widget = nx.util.eventGetWidget(e);
                                var win = nx.bucket.getWin(widget);

                                nx.util.confirm('Are you sure?', 'Delete ' + req.id.substr(5) + '...', function (ok) {

                                    if (ok) {

                                        //
                                        var ds = req.ds;

                                        // Fix ds name
                                        ds = nx.util.toDatasetName(ds);

                                        // Delete
                                        nx.util.serviceCall('AO.ObjectDelete', {
                                            ds: req.ds,
                                            id: req.id
                                        }, nx.util.noOp);

                                        // Close
                                        win.safeClose();

                                    }

                                });;
                            }
                        }, '>'
                    ]
                }
            });
        });

    },

    wf: function (req) {

        var self = this;

        // Show loader
        nx.util.notifyLoadingStart();

        nx.desktop._loadDataset(req.ds, function (dsdef) {

            //
            var urlParams = nx.web.urlStart('ds', req.ds);
            urlParams = nx.web.urlAdd('winid', 'wfl_' + req.id, urlParams);

            //
            nx.util.runTool('Webview', {
                nxid: 'wfl_' + req.id,
                icon: 'plugin',
                caption: 'Workflow - ' + req.id.substr(5),
                value: nx.util.loopbackURL() + '/viewers/elsa/wf.html' + urlParams,
                caller: req.caller,
                chat: false,
                adjustWidth: req.adjustWidth,
                noCenter: req.noCenter,
                topToolbar: req.topToolbar,
                bottomToolbar: req.bottomToolbar,
                commands: {
                    items: [
                        'X', '>', {
                            label: 'Delete',
                            icon: 'application_delete',
                            click: function (e) {
                                var widget = nx.util.eventGetWidget(e);
                                var win = nx.bucket.getWin(widget);

                                nx.util.confirm('Are you sure?', 'Delete ' + req.id.substr(5) + '...', function (ok) {

                                    if (ok) {

                                        //
                                        var ds = req.ds;

                                        // Fix ds name
                                        ds = nx.util.toDatasetName(ds);

                                        // Delete
                                        nx.util.serviceCall('AO.ObjectDelete', {
                                            ds: req.ds,
                                            id: req.id
                                        }, nx.util.noOp);

                                        // Close
                                        win.safeClose();

                                    }

                                });;
                            }
                        }, '>'
                    ]
                }
            });
        });

    },

    removeNODE: function (node) {
        if (node) {
            if (node.remove) {
                node.remove()
            } else if (node.removeNode) {
                node.removeNode(true);
            }
        }
    },

    editEMAIL: function (req) {

        var self = this;

        //
        var path = '/ao/' + req.ds + '/' + req.id;
        var winid = 'email_' + req.ds + '_' + req.id;

        //
        var urlParams = nx.web.urlStart('images', nx.util.loopbackURL() + '/f' + path + '/Upload');
        urlParams = nx.web.urlAdd('winid', winid, urlParams);
        urlParams = nx.web.urlAdd('ds', req.ds, urlParams);
        urlParams = nx.web.urlAdd('id', req.id, urlParams);

        // Show loader
        nx.util.notifyLoadingStart();

        //
        nx.util.runTool('Webview', {
            nxid: winid,
            caption: req.caption,
            icon: 'docx',
            value: nx.util.loopbackURL() + '/viewers/automizy/index.html' + urlParams,
            caller: req.caller, 
            chat: false,
            adjustWidth: req.adjustWidth,
            noCenter: req.noCenter,
            topToolbar: req.topToolbar,
            bottomToolbar: req.bottomToolbar
        });
    },

};

nx.web = {

    urlStart: function (key, value) {

        var self = this;

        return self.urlAdd(key, value);
    },

    urlAdd: function (key, value, prev) {

        var self = this;

        // Set delim
        var delim = (prev ? '&' : '?');

        // 
        return (prev || '') + delim + key + '=' + encodeURIComponent(value);

    }
};

nx.SIO = {

    process: function (event) {

        var self = this;

        // Assume event processed
        var ans;

        // According to call
        switch (event.fn) {

            case '$$win.open':
                nx.util.runTool('Object', {
                    ds: self.get(event, 'ds'),
                    id: self.get(event, 'id'),
                    caller: self.getCaller(event)
                });
                break;

            case '$$win.pick':
                nx.util.runTool('View', {
                    ds: self.get(event, 'ds'),
                    caller: self.getCaller(event)
                });
                break;

            default:
                ans = event;
                break;
        }
        return ans;
    },

    getCaller: function (event) {
        // Assume no caller
        var ans;

        if (event && event.toWinID) {
            ans = nx.desktop.findWindow(event.toWinID);
        }
        return ans;
    },

    get: function (event, key) {
        // Assume no caller
        var ans;

        if (event && event.message) {
            ans = event.message[key];
        }
        return ans;
    }

};