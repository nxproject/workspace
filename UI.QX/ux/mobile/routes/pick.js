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
    name: 'pick',
    path: '/pick/',
    async: function (routeTo, routeFrom, resolve, reject) {

        if (nx.env.isNextBucket(routeTo.url)) {

            var page, data = nx.env.getBucket(routeTo.url);

            //
            var ds = data.ds;

            // Get the dataset
            nx.db._loadDataset(ds, function (dsdef) {

                if (dsdef) {

                    //
                    var title = 'Select ' + dsdef.caption;
                    nx.office.storeHistory(routeTo.url, title, '+' + dsdef.icon, nx.builder.badge('Select', 'green') + ' ', '_search');

                    // Setup 
                    nx.user.createFilter(routeTo.url, function (filter) {

                        // Get the data
                        nx.db.get(ds, filter.queries, 0, nx.env.getRows(), '_desc', null, '_id _desc', function (data) {

                            //
                            var rb = [];
                            var rbl;

                            if (nx.user.opAllowed(ds, 'c', (dsdef.calAllow || ''))) {
                                rb.push({
                                    label: 'Calendar',
                                    icon: 'calendar',
                                    cb: "nx.office.panelRightClose(); nx.fs.calendar('" + ds + "');"
                                });
                            }

                            // Pick
                            var pl = nx.db.getPick(ds);
                            if (pl) {
                                if (rb.length) rb.push('-');

                                // Loop thru
                                Object.keys(pl).forEach(function (key) {
                                    var def = pl[key];
                                    rb.push({
                                        html: nx.builder.toggle(def.label, def.selected === 'y', function (ele) {
                                            def.selected = (ele.checked ? 'y' : 'n');
                                            nx.user.refreshPickList();
                                        })
                                    });
                                });
                            }

                            //
                            if (rb.length) {
                                // Make options panel
                                nx.office.panelRight(nx.builder.scrollable(nx.builder.menuSide(rb)));
                                rbl = nx.builder.link('', 'reorder', 'nx.office.panelRightOpen();', 'Options', 'rightpanel', 'icon-large');
                            }

                            page = nx.builder.page(title,
                                true,
                                rbl,
                                [
                                    nx.builder.searchbar(nx.env.getBucketItem('_search', routeTo.url)),
                                    nx.builder.picklist(ds, data, nx.env.getBucketID(routeTo.url), nx.env.getBucketItem('onSelect', routeTo.url))
                                ],
                                ((nx.user.opAllowed(ds, 'a') && ((dsdef.hidden || '').indexOf('v') === -1 || data.chain)) ?
                                    {
                                        label: 'Add',
                                        icon: 'add_circle_outline',
                                        cb: function () {

                                            nx.calls.view({
                                                ds: ds,
                                                chain: data.chain
                                            });
                                        }
                                    } : null),
                                'nx.office.goBack()'
                            );

                            // Finalize
                            nx.env.setBucketItem('_onSetup', function (url) {

                                // Do we have a value?
                                if (nx.env.getBucketItem('_search', url)) {
                                    // Show
                                    $('.input-clear-button').css('opacity', 1).css('visibility', 'visible').css('pointer-events', 'all');
                                }

                                // Get searchbar
                                var sb = $('input[type="search"]');

                                // Get the event
                                sb.on('input', function (e) {
                                    // Get the value
                                    var value = $(e.target).val();
                                    // Save
                                    nx.env.setBucketItem('_search', value, url);

                                    // Clear previous
                                    clearTimeout(nx.env.getBucketItem('_lookup', url));
                                    // And reset
                                    nx.env.setBucketItem('_lookup', setTimeout(nx.user.refreshPickList, 800), url)
                                });

                            }, routeTo.url);

                            resolve({
                                template: page
                            }, {
                                context: {}
                            });

                        });

                    });
                }

            });
        }
    }
});

// Set the call
nx.calls.pick = function (req) {
    //
    if (typeof req === 'string') {
        req = {
            ds: req
        }
    }
    // Call
    nx.office.goTo('pick', req);
};

// Set the call
nx.calls.pickchild = function (req) {

    // Force to latest bucket
    //nx.env.setDefaultBucket();

    // Get the object
    var obj = nx.env.getBucketItem('_obj');

    // Must have one
    if (obj) {
        // Save but keep
        nx.db.setObj(obj, null, nx.util.noOp, true);

        var cfld, cid;

        //
        req = req || {};
        if (typeof req === 'string') {
            var pos = req.indexOf(':');
            if (pos !== -1) {
                cfld = req.substr(1 + pos);
                req = req.substr(0, req.indexOf(':'));
            }
            req = {
                ds: req
            }
        }

        // Add chain
        req.chain = nx.util.makeChain('Any', (cfld || '_parent'), '=', nx.db.makeID(obj._ds, obj._id));

        // Call
        nx.office.goTo('pick', req);
    }
};

nx.calls.pickselect = function (req) {

    // Get the callbact
    var cb = nx.env.getBucketItem('onSelect');

    // Any?
    if (cb) {
        cb(req);
    }

}