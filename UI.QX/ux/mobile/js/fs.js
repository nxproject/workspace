/* ************************************************************************

   Framework7 - a dynamic web interface

   https://framework7.io/

   Copyright:
     2020-2021 Jose E. Gonzalez (nxoffice2021@gmail.com)

   License:
     MIT: https://opensource.org/licenses/MIT
     See the LICENSE file in the project's top-level directory for details.

   Authors:
     * Jose E. Gonzalez

************************************************************************ */

nx.fs = {

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

    viewpdf: function (path) {

        var self = this;

        // Assure proper start
        if (!nx.util.startsWith(path, '/')) path = '/' + path;

        // make caption
        var caption = path;
        var pos = caption.lastIndexOf('/');
        if (pos !== -1) caption = caption.substr(pos + 1);

        //
        var urlParams = nx.web.urlStart('file', '/f' + path);

        //
        nx.util.urlPopup(nx.util.loopbackURL() + '/viewers/pdf/web/viewer.html' + urlParams);

    },

    viewaspdf: function (path) {

        var self = this;

        // Assure proper start
        if (!nx.util.startsWith(path, '/')) path = '/' + path;

        // make caption
        var caption = path;
        var pos = caption.lastIndexOf('/');
        if (pos !== -1) caption = caption.substr(pos + 1);

        //
        var urlParams = nx.web.urlStart('file', '/fx/pdf' + path);

        //
        nx.util.urlPopup(nx.util.loopbackURL() + '/viewers/pdf/web/viewer.html' + urlParams);

    },

    editdocx: function (req) {

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
        var urlParams = nx.web.urlStart('file', '/fx/html' + path);
        urlParams = nx.web.urlAdd('winid', winid, urlParams);

        //
        nx.util.urlPopup(nx.util.loopbackURL() + '/viewers/tinymce/index.html' + urlParams);

    },

    viewimage: function (path) {

        var self = this;

        // Assure proper start
        if (!nx.util.startsWith(path, '/')) path = '/' + path;

        // make caption
        var caption = path;
        var pos = caption.lastIndexOf('/');
        if (pos !== -1) caption = caption.substr(pos + 1);

        //
        var urlParams = nx.web.urlStart('file', '/f' + path);

        //
        nx.util.urlPopup(nx.util.loopbackURL() + '/viewers/image/viewer.html' + urlParams);

    },

    viewvideo: function (path, ext) {

        var self = this;

        // Assure proper start
        if (!nx.util.startsWith(path, '/')) path = '/' + path;

        // make caption
        var caption = path;
        var pos = caption.lastIndexOf('/');
        if (pos !== -1) caption = caption.substr(pos + 1);

        //
        var urlParams = nx.web.urlStart('file', '/f' + path);
        urlParams = nx.web.urlAdd('ext', ext, urlParams);

        //
        nx.util.urlPopup(nx.util.loopbackURL() + '/viewers/video/viewer.html' + urlParams);

    },

    calendar: function (ds) {

        var self = this;

        //
        var winid = 'fc_' + ds;

        nx.db._loadDataset(ds, function (dsdef) {

            // 
            var oo = moment(nx.user.getSIField('officeopen') || '08:00 AM').format('HH:mm');
            var oc = moment(nx.user.getSIField('officeclose') || '05:00 PM').format('HH:mm');
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

            // Build URL
            var urlParams = nx.web.urlStart('officeopen', oo);
            urlParams = nx.web.urlAdd('officeclose', oc, urlParams);
            urlParams = nx.web.urlAdd('startdate', td, urlParams);
            urlParams = nx.web.urlAdd('ds', ds, urlParams);
            urlParams = nx.web.urlAdd('winid', winid, urlParams);
            urlParams = nx.web.urlAdd('mgr', (nx.user.getIsSelector('MGR') ? 'y' : 'n'), urlParams);
            if (startfld) urlParams = nx.web.urlAdd('startfld', startfld, urlParams);
            if (endfld) urlParams = nx.web.urlAdd('endfld', endfld, urlParams);
            urlParams = nx.web.urlAdd('ro', (dsdef.calRO || 'n'), urlParams);

            //
            nx.util.urlPopup(nx.util.loopbackURL() + '/viewers/fullcalendar/fc.html' + urlParams);
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

    removeNODE: function (node) {
        if (node) {
            if (node.remove) {
                node.remove()
            } else if (node.removeNode) {
                node.removeNode(true);
            }
        }
    },

    quickMessage: function (to, id) {

        // Parse
        var parsed = nx.db.parseID(id);

        // Get the qm
        nx.db.getObj(parsed.ds, parsed.id, function (data) {

            var fn = (nx.util.isPhone(to) ? 'sms' : 'email');

            // 
            nx.util.serviceCall('Communication.Process', {
                cmd: fn,
                to: to,
                subject: data.subj,
                message: data.msg,
                att: [],
                template: data.temp || '',
                campaign: '',
                telemetry: 'n',
                mlink: ''
            });
        });
    },

    comm: function (widget, fn) {

        var self = this;

        // Get the value
        var value = widget.val();
        // Do we have a value?
        if (value) {
            // Get the object
            var bucket = nx.env.getBucket();
            // Valid?
            if (bucket && bucket._obj) {
                // Make the request
                var req = {
                    ds: bucket._obj._ds,
                    id: bucket._obj._id,
                    desc: bucket._obj._desc,
                    fsfn: fn,
                    route: nx.env.getBucketRoute(),
                    onSelect: 'nx.fs.commCampaign',
                    skip: nx.fs.commCampaign,
                    to: value,
                    useTelemetry: nx.user.getSIField('teleenabled')
                };

                // Get the choices
                nx.util.serviceCall('AO.QueryGet', {
                    ds: '_emailtemplates',
                    cols: 'code'
                }, function (result) {
                    // Any?
                    if (result && result.data && result.data.length) {
                        // Empty
                        var choices = [''];
                        // Loop thru
                        result.data.forEach(function (entry) {
                            // Add
                            choices.push(entry.code);
                        });
                        // Save
                        req.choices = choices;
                        // Call
                        nx.calls.documents(req);
                    } else {
                        // No choices
                        nx.calls.documents(req);
                    }
                });
            }
        }
    },

    commCampaign: function (path) {

        var self = this;

        // Get the bucket
        var req = nx.env.getBucket();
        // Do we have a path?
        if (path) {
            req.att = [path];
        }

        if (nx.user.getSIField('teleenabled') === 'y') {
            // Fetch
            nx.util.serviceCall('AO.QueryGet', {
                ds: '_telemetrycampaign',
                cols: 'code',
                query: [
                    {
                        field: 'active',
                        op: 'Eq',
                        value: 'y'
                    }
                ]
            }, function (result) {
                // Any?
                if (result && result.data && result.data.length) {
                    var campaigns = [''];
                    // Loop thru
                    result.data.forEach(function (entry) {
                        // Add
                        campaigns.push(entry.code);
                    });
                    req.campaigns = campaigns;
                    // Call
                    nx.calls.comm(req);
                } else {
                    // No campaihgns found
                    nx.calls.comm(req);
                }
            });
        } else {
            // No campaihgns allowed
            nx.calls.comm(req);
        }
    }

};