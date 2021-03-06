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
    name: 'view',
    path: '/view/',
    async: function (routeTo, routeFrom, resolve, reject) {

        if (nx.env.isNextBucket(routeTo.url)) {

            var page, data = nx.env.getBucket(routeTo.url);

            //
            var ds = data.ds;
            var id = data.id;

            // In case of shorthand
            if (!ds && id) {
                // Split
                var parsed = nx.db.parseID(id);
                //
                if (parsed) {
                    ds = parsed.ds;
                    id = parsed.id;
                }
            }

            // Must have 
            if (ds) {

                // Get the dataset
                nx.db._loadDataset(ds, function (dsdef) {

                    if (dsdef) {

                        // Get object
                        nx.db.getObj(ds, id, function (obj) {

                            // Save
                            nx.env.setBucketItem('_obj', obj, routeTo.url);
                            nx.env.setBucketItem('_ao', 'ao_' + ds + '_' + id, routeTo.url);


                            // Make the title
                            var title = nx.db.localizeDesc(obj._desc) || ('New ' + dsdef.caption);
                            nx.office.storeHistory(routeTo.url, title, '+' + dsdef.icon, nx.builder.badge('View', 'red') + ' ', '_title');

                            // Make page
                            nx.builder.view(ds, nx.user.getDSInfo(ds).view, obj, function (rows) {

                                //
                                var rbl;
                                var rb = [];
                                var sep = true;

                                //
                                if (obj._parent) {
                                    // Only if not already being viewed
                                    if (!nx.db.inUse(obj._parent)) {
                                        // Parse
                                        var wkg = nx.db.parseID(obj._parent);
                                        if (nx.user.opAllowed(wkg.ds, 'v')) {
                                            var info = nx.user.getDSInfo(wkg.ds);
                                            if (wkg) {
                                                rb.push({
                                                    obj: obj._parent,
                                                    label: info.caption,
                                                    icon: info.icon
                                                });
                                                rb.push('-');
                                                sep = true;
                                            }
                                        }
                                    }
                                }

                                // Tools
                                var tools = nx.user.getDSInfo(ds).tools;
                                if (tools) {
                                // Get the list
                                    var list = Object.keys(tools);
                                    if (list.length) {
                                        // Loop thru
                                        Object.keys(tools).forEach(function (tname) {
                                            if (!sep && rb.length) {
                                                rb.push('-');
                                                sep = true;
                                            }
                                            // Add
                                            rb.push({
                                                label: tools[tname].caption,
                                                icon: tools[tname].icon,
                                                cb: 'nx.tools.' + tname + '();'
                                            });
                                        });
                                        sep = false;
                                    }
                                }

                                // Billing?
                                if (nx.user.getIsSelector('BILLING')) {
                                    // Can we do it?
                                    if (dsdef.isBillable === 'y' && obj._billat && obj._billto) {
                                        if (!sep && rb.length) {
                                            rb.push('-');
                                            sep = true;
                                        }
                                        // Fake it
                                        var acct = '';

                                        rb.push({
                                            label: '>> Charge',
                                            icon: 'coins',
                                            cb: "nx.calls.commBilling('" + acct + "','" + obj._billto + "','" + obj._billat + "')"
                                        });
                                        rb.push({
                                            label: '>> Subscriptions',
                                            icon: 'tag_red',
                                            cb: "nx.calls.commSubs('" + acct + "','" + obj._billto + "','" + obj._billat + "')"
                                        });
                                        rb.push({
                                            label: '>> Invoices',
                                            icon: 'money',
                                            cb: "nx.calls.commInvoices('" + acct + "','" + obj._billto + "','" + obj._billat + "')"
                                        });
                                    }
                                }

                                //
                                if (obj._account) {

                                    // 
                                    var acct = obj._account;

                                    // Extended?
                                    if (nx.user.getIsSelector('TELE') || nx.user.getIsSelector('EMAIL')) {
                                        if (!sep && rb.length) {
                                            rb.push('-');
                                            sep = true;
                                        }
                                        rb.push({
                                            label: '>> Quick',
                                            icon: 'lightning',
                                            cb: "nx.calls.commQuick('" + acct + "')"
                                        });
                                        if (nx.util.isPhone(acct)) {
                                            rb.push({
                                                label: '>> SMS',
                                                icon: 'phone',
                                                cb: "nx.calls.commSMS('" + acct + "')"
                                            });
                                        }
                                        if (nx.util.isEMail(acct)) {
                                            rb.push({
                                                label: '>> EMail',
                                                icon: 'email',
                                                cb: "nx.calls.commEMail('" + acct + "')"
                                            });
                                        }
                                        sep = false;
                                    }
                                }

                                // Localize
                                var childdss = dsdef.childdss;
                                var relateddss = dsdef.relateddss;

                                // Make list
                                var list = nx.util.splitSpace(relateddss, true);
                                //
                                if (list.length) {
                                    // Loop thru
                                    for (var i = 0; i < list.length; i += 2) {
                                        //
                                        var cds = list[i];
                                        var cfld = list[i + 1];
                                        //
                                        if (cds) {
                                            // Get
                                            var cdsdef = nx.user.getDSInfo(cds);
                                            if (cdsdef) {
                                                if (!sep && rb.length) {
                                                    rb.push('-');
                                                    sep = true;
                                                }
                                                rb.push({
                                                    childds: cds + ':' + cfld,
                                                    label: cdsdef.caption,
                                                    icon: cdsdef.icon
                                                });
                                            }
                                            sep = false;
                                        }
                                    }
                                }

                                // Make list
                                var list = nx.util.splitSpace(childdss, true);
                                //
                                if (list.length) {
                                    // Loop thru
                                    list.forEach(function (cds) {
                                        //
                                        if (cds) {
                                            // Get
                                            var cdsdef = nx.user.getDSInfo(cds);
                                            if (cdsdef) {
                                                if (!sep && rb.length) {
                                                    rb.push('-');
                                                    sep = true;
                                                }
                                                rb.push({
                                                    childds: cds,
                                                    label: cdsdef.caption,
                                                    icon: cdsdef.icon
                                                });
                                            }
                                            sep = false;
                                        }
                                    });
                                }

                                //
                                if (nx.user.opAllowed(ds, 'd')) {
                                    if (!sep && rb.length) {
                                        rb.push('-');
                                        sep = true;
                                    }
                                    rb.push({
                                        tool: 'Documents',
                                        label: 'Documents',
                                        icon: 'folder'
                                    });
                                }
                                //if (nx.user.opAllowed(ds, 'm')) {
                                //    if (!sep && rb.length) {
                                //        rb.push('-');
                                //        sep = true;
                                //    }
                                //   rb.push({
                                //        label: 'Merge',
                                //        icon: 'arrow_merge',
                                //        tool: 'Merge'
                                //    });
                                //}

                                sep = false;

                                if (nx.user.opAllowed(ds, 'v', (dsdef.chatAllow || ''))) {
                                    if (!sep && rb.length) {
                                        rb.push('-');
                                        sep = true;
                                    }
                                    rb.push({
                                        label: 'Chat',
                                        tool: 'Chat',
                                        icon: 'user_comment'
                                    });
                                }

                                //if (nx.user.opAllowed(ds, 'o', (dsdef.orgAllow || ''))) {
                                //    if (!sep && rb.length) {
                                //        rb.push('-');
                                //        sep = true;
                                //    }
                                //    rb.push({
                                //        label: 'Organizer',
                                //        icon: 'org',
                                //        tool: 'Organizer'
                                //    });
                                //}

                                if (nx.user.opAllowed(ds, 'v', (dsdef.ttAllow || ''))) {
                                    rb.push('-Time Tracking');
                                    rb = rb.concat([
                                        {
                                            label: 'Start',
                                            icon: 'flag_green',
                                            tool: 'TTStart'
                                        }, {
                                            label: 'Start Frozen',
                                            icon: 'flag_yellow',
                                            tool: 'TTStartFrozen'
                                        }, {
                                            label: 'Freeze',
                                            icon: 'flag_red',
                                            tool: 'TTFreeze'
                                        }, {
                                            label: 'Continue',
                                            icon: 'flag_yellow',
                                            tool: 'TTContinue'
                                        }, {
                                            label: 'End',
                                            icon: 'flag_purple',
                                            tool: 'TTEnd'
                                        }, {
                                            label: 'Show',
                                            icon: 'date',
                                            tool: 'TTShow'
                                        }
                                    ]);
                                }

                                if (nx.user.opAllowed(ds, 'x')) {
                                    if (!sep && rb.length) {
                                        rb.push('-');
                                        sep = true;
                                    }
                                    rb.push({
                                        label: 'Delete',
                                        icon: 'stop',
                                        tool: 'Delete'
                                    });
                                }

                                //
                                if (rb.length) {
                                    // Make options panel
                                    nx.office.panelRight(nx.builder.scrollable(nx.builder.menuSide(rb)));
                                    rbl = nx.builder.link('', 'reorder', 'nx.office.panelRightOpen();', 'Options', 'rightpanel', 'icon-large');
                                }

                                var page = nx.builder.page(title, true, rbl, rows,
                                    "nx.db.setObj();",
                                    "nx.db.clearObj()"
                                );

                                resolve({
                                    template: page
                                }, {
                                    context: {}
                                });

                            });

                        }, (nx.user.getIsSelector('TELE') ||
                            nx.user.getIsSelector('EMAIL') ||
                            nx.user.getIsSelector('BILLING')), data.chain);

                    } else {

                        page = nx.builder.oops();

                        resolve({
                            template: page
                        }, {
                            context: {}
                        });
                    }
                });

            } else {

                page = nx.builder.oops();

                resolve({
                    template: page
                }, {
                    context: {}
                });
            }
        }
    }
});

// Set the calls
nx.calls.view = function (req) {
    //
    if (typeof req === 'string') {
        req = {
            id: req
        }
    }

    // Chaining
    req.chain = nx.env.getBucketItem('chain');

    // Call
    nx.office.goTo('view', req);
};