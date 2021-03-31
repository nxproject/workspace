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
    name: 'login',
    path: '/login/',
    async: function (routeTo, routeFrom, resolve, reject) {

        nx.builder.setCallbackBucket(routeTo.url);

        var page = nx.builder.page('Login', false, null,
            [
                nx.builder.contentBlock(nx.builder.form([
                    {
                        nxtype: 'keyword',
                        top: 1,
                        left: 1,
                        width: 10,
                        label: 'Name'
                    }, {
                        nxtype: 'password',
                        top: 2,
                        left: 1,
                        width: 10,
                        label: 'Password'
                    }, {
                        nxtype: 'boolean',
                        top: 3,
                        left: 1,
                        width: 10,
                        label: 'Remember me'
                    }
                ]))
            ],
            function () {
                // Get the data
                var data = nx.fields.getFormData();
                // Login
                nx.user.login(data.Name, data.Password, data['Remember me']);
            }
        );

        resolve({
            template: page
        }, {
            context: {}
        });

        // Must always be
        nx.fields.allowTabs();
    }
});

// Set the call
nx.calls.login = function (req) {
    // Assure
    req = req || {};
    // Reset?
    if (req.force) nx.env.reset();

    // Do we have a remember me token?
    if (!nx.user.getName()) {

        var rm = nx.env.getRM();

        if (nx.util.hasValue(rm)) {

            // Login
            nx.user.login(rm, '', 'y');

        } else {

            // Are we logged in?
            $('title').text('NX.Project');

            nx.office.goTo('login', req);

        }
    } else {

        $('title').text(nx.user.getSIField('name') || 'NX.Project');

        nx.calls.menu(req);
    }
};