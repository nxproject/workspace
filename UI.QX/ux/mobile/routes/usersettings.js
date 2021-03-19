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
    name: 'usersettings',
    path: '/usersettings/',
    async: function (routeTo, routeFrom, resolve, reject) {

        nx.builder.setCallbackBucket(routeTo.url);

        var page, data = nx.env.getBucket(routeTo.url);

        var ds = (nx.user.getIsSelector('ACCT') ? '_billaccess' : '_user');
        var id = nx.user.getName();

        // Get object
        nx.db.getObj(ds, id, function (result) {

            // Save
            nx.env.setBucketItem('_obj', result, routeTo.url);
            nx.env.setBucketItem('_ao', 'ao_' + ds + '_' + id, routeTo.url);

            //
            var title = 'User Settings';
            nx.office.storeHistory(routeTo.url, title, '+user', title, '');

            // Make page
            nx.builder.view(ds, '_usersettings', result, function (rows) {

                var page = nx.builder.page(title, true, null, rows,
                    function () {

                    // Acct?
                        if (nx.user.getIsSelector('ACCT')) {
                            // Update
                            nx.util.serviceCall('AO.AccessSet', {
                                ds: ds,
                                id: user,
                                data: data
                            }, function () {
                                nx.util.notifyOK('Password reset');
                            });
                        } else {
                            // Save
                            nx.db.setObj(result, null, function () {
                                // And go back
                                nx.office.goBack();
                            });
                        }
                    }, function () {
                        // Release object
                        nx.db.clearObj(result, null, function () {
                            // And go back
                            nx.office.goBack();
                        });
                    }
                );

                resolve({
                    template: page
                }, {
                    context: {}
                });

            });
        });
    }
});

// Set the call
nx.calls.usersettings = function (req) {
    // 
    nx.office.goTo('usersettings', req);
};