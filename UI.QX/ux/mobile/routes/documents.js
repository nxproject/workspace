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
    name: 'documents',
    path: '/documents/',
    async: function (routeTo, routeFrom, resolve, reject) {

        nx.builder.setCallbackBucket(routeTo.url);

        var page, data = nx.env.getBucket(routeTo.url);

        //
        var ds = data.ds;
        var id = data.id;

        //
        var title = data.desc;
        nx.office.storeHistory(routeTo.url, title, '+folder', nx.builder.badge('Documents', 'yellow') + ' ', '_title');

        nx.util.serviceCall('Docs.DocumentList', {
            ds: ds,
            id: id
        }, function (result) {

            page = nx.builder.page(title,
                true,
                null,
                [
                    nx.builder.documents(ds, result.list, nx.env.getBucketID(routeTo.url)),
                    nx.builder.upload()
                ],
                (nx.user.opAllowed(ds, 'a') ?
                    {
                        label: 'Upload',
                        icon: '+logo',
                        cb: function () {

                            //
                            var uploader = $('#upload');

                            // Indirect
                            uploader.change(function () {

                                var fd = new FormData();
                                var files = uploader[0].files[0];
                                fd.append('file', files);

                                nx.util.notifyOK('Starting upload...');

                                //
                                var url = nx.util.loopbackURL() + '/fload/ao/' + ds + '/' + id + '/Upload/' + files.name;
                                files.text().then((contents) => {

                                    $.post(url, { content: contents }, function (response) {
                                        if (response && response.done ==='y') {
                                            nx.util.notifyOK('Done');
                                        }
                                        else {
                                            nx.util.notifyError('Unable to upload');
                                        }
                                    }, 'json');
                                });

                            }).click();

                        }
                    } : null),
                'nx.office.goBack()'
            );
            resolve({
                template: page
            }, {
                context: {}
            });
        });
    }
});

// Set the call
nx.calls.documents = function (req) {
    // Get the object
    var obj = nx.env.getBucketItem('_obj');

    // Assure
    req = nx.util.merge((req || {}), {
        ds: obj._ds,
        id: obj._id,
        desc: obj._desc
    });;
    // 
    nx.office.goTo('documents', req);
};