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
    name: 'menu',
    path: '/menu/',
    async: function (routeTo, routeFrom, resolve, reject) {

        if (nx.env.isNextBucket(routeTo.url)) {

            var page, data = nx.env.getBucket(routeTo.url);

            //
            var title = 'Select';
            nx.office.storeHistory(routeTo.url, title, '+office', 'Select', '');

            page = nx.builder.page(title, false, null, nx.builder.contentBlock(nx.builder.menu(nx.user._menu)));

            resolve({
                template: page
            }, {
                context: {}
            });
        }
    }
});

// Set the call
nx.calls.menu = function (req) {
    // Call
    nx.office.goTo('menu', req);
};