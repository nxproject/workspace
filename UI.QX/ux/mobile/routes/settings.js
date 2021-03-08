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
    name: 'settings',
    path: '/settings/',
    async: function (routeTo, routeFrom, resolve, reject) {

        nx.builder.setCallbackBucket(routeTo.url);

        var body = $('body');
        var app = $('#app');
        var theme = body.attr('class').replace('theme-', '');
        var color = app.attr('class').replace('color-theme-', '').replace('framework7-root', '').trim();
        var nrows = nx.env.getRows();

        var page = nx.builder.page('Settings', true, null,
            [
                nx.builder.contentBlock(nx.builder.form([
                    {
                        label: 'Descrition',
                        nxtype: 'string',
                        top: 1,
                        left: 1,
                        value: nx.env.getStore('desc'),
                        on: {
                            change: function (widget) {
                                var value = widget.val();
                                nx.env.setStore('desc', value);
                            }
                        }
                    }, {
                        label: 'Mode',
                        nxtype: 'combobox',
                        choices: ['light', 'dark'],
                        top: 2,
                        left: 1,
                        value: theme,
                        on: {
                            change: function (widget) {
                                var value = widget.val();
                                nx.env.setTheme(value);
                            }
                        }
                    }, {
                        label: 'Color',
                        nxtype: 'combobox',
                        choices: ['red',
                            'green',
                            'blue',
                            'pink',
                            'yellow',
                            'orange',
                            'purple',
                            'deeppurple',
                            'lightblue',
                            'teal',
                            'lime',
                            'deeporange',
                            'gray',
                            'black'],
                        top: 3,
                        left: 1,
                        value: color,
                        on: {
                            change: function (widget) {
                                var value = widget.val();
                                nx.env.setColor(value);
                            }
                        }
                    }, {
                        label: '# Rows',
                        nxtype: 'int',
                        top: 4,
                        left: 1,
                        value: nrows,
                        on: {
                            change: function (widget) {
                                var value = widget.val();
                                nx.env.setRows(value);
                            }
                        }
                    }
                ]))
            ],
            null,
            function () {
                //
                nx.office.goBack();
            });

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
nx.calls.settings = function (req) {
    // Assure
    req = req || {};
    // 
    nx.office.goTo('settings', req);
};