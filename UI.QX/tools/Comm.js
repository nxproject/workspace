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

qx.Class.define('tools.Comm', {

    type: 'static',

    statics: {

        startindex: 'hidden',

        // This is what you override
        do: function (req) {

            // Set the template
            switch (req.fsfn) {
                case 'email':
                case 'sms':
                    req.template = '_emailtemplates';
                    break;
            }

            // Do we have a template?
            if (req.template) {
                // Get the choices
                nx.util.serviceCall('AO.QueryGet', {
                    ds: req.template,
                    cols: 'code'
                }, function (result) {
                    // Any?
                    if (result && result.data && result.data.length) {
                        // Empty
                        var choices = [];
                        // Loop thru
                        result.data.forEach(function (entry) {
                            // Add
                            choices.push(entry.code);
                        });
                        // SOrt
                        choices.sort();
                        // Call
                        tools.Comm.campaign(req, choices);
                    } else {
                        // No choices
                        tools.Comm.campaign(req);
                    }
                });
            } else {
                // No choices allowed
                tools.Comm.campaign(req);
            }

        },

        campaign: function (req, choices) {

            if (nx.desktop.user.getSIField('teleenabled') === 'y') {
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
                            var campaigns = [];
                            // Loop thru
                            result.data.forEach(function (entry) {
                                // Add
                                campaigns.push(entry.code);
                            });
                            // Sort
                            campaigns.sort();
                            // Call
                            tools.Comm.display(req, choices, campaigns);
                        } else {
                            // No campaihgns found
                            tools.Comm.display(req, choices);
                        }
                });
            } else {
                // No campaihgns allowed
                tools.Comm.display(req, choices);
            }
        },

        display: function (req, choices, campaigns) {

            var row = 1;
            var items = [];

            if (req.askto) {
                items.push({
                    nxtype: 'string',
                    top: row,
                    left: 1,
                    width: 'default.fieldWidth',
                    label: 'To'
                });
                row++;
            }
            items.push({
                nxtype: 'string',
                top: row,
                left: 1,
                width: 'default.fieldWidth',
                label: 'Subject'
            });
            row++;
            var h = nx.default.get('default.textareaHeight');
            items.push({
                nxtype: 'textarea',
                top: row,
                left: 1,
                width: 'default.fieldWidth',
                height: h,
                label: 'Message'
            });
            row += h;

            if (choices && choices.length) {
                items.push({
                    nxtype: 'combobox',
                    top: row,
                    left: 1,
                    width: 'default.fieldWidth',
                    label: 'Template',
                    choices: choices
                });
                row++;
            }

            if (campaigns && campaigns.length && req.useTelemetry == 'y') {
                items.push({
                    nxtype: 'combobox',
                    top: row,
                    left: 1,
                    width: 'default.fieldWidth',
                    label: 'Campaign',
                    choices: campaigns
                });
                row++;
            }

            if (req.fsfn === 'sms' && req.useTelemetry == 'y') {
                items.push({
                    nxtype: 'string',
                    top: row,
                    left: 1,
                    width: 'default.fieldWidth',
                    label: 'Message Link'
                });
                row++;
            }

            nx.desktop.addWindow({

                caption: req.fslabel,
                defaultCommand: 'Ok',
                caller: req.caller,

                items: items,

                commands: {

                    items: [

                        '>', {

                            label: 'Ok',
                            icon: 'accept',
                            click: function (e) {

                                // Get the button
                                var widget = nx.util.eventGetWidget(e);

                                // Map window
                                var win = nx.bucket.getWin(widget);

                                // Get values
                                var subj = win.getValue('Subject') || req.to;
                                var msg = win.getValue('Message');
                                if (req.askto) {
                                    req.value = win.getValue('To');
                                }
                                var template = win.getValue('Template');
                                var campaign = win.getValue('Campaign');
                                var mlink = win.getValue('Message Link');

                                // 
                                nx.util.serviceCall('Communication.Process', {
                                    cmd: req.fsfn,
                                    to: req.value,
                                    subject: subj,
                                    message: msg,
                                    att: req.attachments,
                                    template: template || '',
                                    campaign: campaign,
                                    telemetry: req.useTelemetry,
                                    mlink: mlink,
                                    ds: req.ds,
                                    id: req.id
                                });

                                // Close
                                win.safeClose();
                            }

                        }

                    ]
                }

            });

        }

    }

});