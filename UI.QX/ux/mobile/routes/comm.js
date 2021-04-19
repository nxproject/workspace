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

// Set the Framework7 route
nx._routes.push({
    name: 'comm',
    path: '/comm/',
    async: function (routeTo, routeFrom, resolve, reject) {

        if (nx.env.isNextBucket(routeTo.url)) {

            var page, data = nx.env.getBucket(routeTo.url);

            //
            var title = 'To ' + data.to;
            nx.office.storeHistory(routeTo.url, title, '+folder', nx.builder.badge('Message', 'pink') + ' ');

            // Body
            var row = 1;
            var items = [];

            if (data.askto) {
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
            var h = 1;
            items.push({
                nxtype: 'textarea',
                top: row,
                left: 1,
                width: 'default.fieldWidth',
                height: h,
                label: 'Message'
            });
            row += h;

            if (data.choices && data.choices.length) {
                items.push({
                    nxtype: 'combobox',
                    top: row,
                    left: 1,
                    width: 'default.fieldWidth',
                    label: 'Template',
                    choices: data.choices
                });
                row++;
            }

            if (data.campaigns && data.campaigns.length && data.useTelemetry === 'y') {
                items.push({
                    nxtype: 'combobox',
                    top: row,
                    left: 1,
                    width: 'default.fieldWidth',
                    label: 'Campaign',
                    choices: data.campaigns
                });
                row++;
            }

            if (data.fsfn === 'sms' && data.useTelemetry === 'y') {
                items.push({
                    nxtype: 'string',
                    top: row,
                    left: 1,
                    width: 'default.fieldWidth',
                    label: 'Message Link'
                });
                row++;
            }

            var page = nx.builder.page('Message', false, null,
                [
                    nx.builder.contentBlock(nx.builder.form(items))
                ],
                function () {
                    // Get the data
                    var info = nx.fields.getFormData();

                    // 
                    nx.util.serviceCall('Communication.Process', {
                        cmd: data.fsfn,
                        to: data.to,
                        subject: info.Subject,
                        message: info.Message,
                        att: data.att,
                        template: info.Template || '',
                        campaign: info.Campaign || '',
                        telemetry: data.useTelemetry,
                        mlink: info['Message Link'] || ''
                    }, function () {
                        // Go back
                        nx.office.goBack(2);
                    });
                }
            );

            resolve({
                template: page
            }, {
                context: {}
            });

        }
    }
});

// Set the call
nx.calls.comm = function (req) {
    // 
    nx.office.goTo('comm', req);
};

// Set the call
nx.calls.commQuick = function (req) {
    //
    nx.calls.pick({
        ds: '_quickmessages',
        onSelect: function (id) {
            nx.fs.quickMessage(req, id);
            nx.office.goBack();
        }
    });
};

// Set the call
nx.calls.commEMail = function (req) {
    // 
    nx.fs.comm(req, 'email');
};

// Set the call
nx.calls.commSMS = function (req) {
    // 
    nx.fs.comm(req, 'sms');
};

// Set the call
nx.calls.commBilling = function (req, to, at) {
    // Make chain
    var chain = nx.util.makeChain('All', 'acct', '=', to, 'at', '=', at);
    // 
    nx.calls.pick({
        ds: '_billcharge',
        chain: chain
    });
};

// Set the call
nx.calls.commSubs = function (req, to, at) {
    // Make chain
    var chain = nx.util.makeChain('All', 'acct', '=', to, 'at', '=', at);
    // 
    nx.calls.pick({
        ds: '_billsubs',
        chain: chain
    });
};

// Set the call
nx.calls.commInvoices = function (req, to, at) {
    // Make chain
    var chain = nx.util.makeChain('All', 'acct', '=', to, 'at', '=', at);
    // 
    nx.calls.pick({
        ds: '_billinvoice',
        chain: chain
    });
};