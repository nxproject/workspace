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

        nx.env.setDefaultBucket(routeTo.url);

        var page, data = nx.env.getBucket(routeTo.url);

        //
        var title = data.desc;
        nx.office.storeHistory(routeTo.url, title, '+folder', nx.builder.badge('Merge', 'pink') + ' ');

        // TBD

        resolve({
            template: page
        }, {
            context: {}
        });
    }
});

// Set the call
nx.calls.comm = function (req) {
    // 
    nx.office.goTo('comm', req);
};