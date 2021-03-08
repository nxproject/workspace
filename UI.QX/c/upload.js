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

************************************************************************ */

qx.Class.define('c.upload', {

    extend: c._component,

    implement: i.iComponent,

    construct: function (req) {

        var self = this;

        // Make the button
        var button = new qxl.upload.UploadButton('Upload', nx.util.getIcon('database_connect'));

        // Call base
        self.base(arguments, button);

        // Make the upload manager
        var uploader = new qxl.upload.UploadMgr(button, '/f');
        // Save path
        button._path = req.path;

        uploader.addListener('addFile', function (evt) {

            // Get the file
            var file = evt.getData();

            // Get the path
            var lpath = button._path;

            // No path?
            if (!lpath) {
                // Get the aoobject
                var aoobj = nx.bucket.getForm(nx.bucket.getWidgets(self)[0])._aoobject;
                // Make the path
                lpath = '/f/ao/' + aoobj.values._ds + '/' + aoobj.values._id;
            }
            // Add the file
            var final = lpath + '/Upload/' + file.getFilename();
            // Set the path
            uploader.setUploadUrl(final);

            var progressListenerId = file.addListener('changeProgress', function (evt) {
                var file = evt.getTarget();
                var uploadedSize = evt.getData();

                button.setLabel('At ' + Math.round(uploadedSize / file.getSize() * 100) + '%');
            }, this);

            // All browsers can at least get changes in state
            var stateListenerId = file.addListener('changeState', function (evt) {
                var state = evt.getData();
                var file = evt.getTarget();

                if (state == 'uploading')
                    button.setLabel('Uploading ' + file.getFilename());
                else if (state == 'uploaded')
                    button.setLabel('Completed ' + file.getFilename());
                else if (state == 'cancelled')
                    button.setLabel('Cancelled ' + file.getFilename());

                // Remove the listeners
                if (state == 'uploaded' || state == 'cancelled') {
                    file.removeListenerById(progressListenerId);
                    file.removeListenerById(stateListenerId);
                }
            }, this);
        });

        // Link
        self._uploader = uploader;

    },

    statics: {

        makeSelf: function (req) {

            return new c.upload(req);

        }
    }

});