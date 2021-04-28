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

	@require(qx.core.Object)

************************************************************************ */

qx.Class.define('tools.invaspdf', {

    type: 'static',

    statics: {

        startindex: '210',
        startpriority: 'K',
        startprivilege: 'BILLING',
        ds: '_billinvoice',
        caption: 'Invoice as PDF',
        icon: 'printer',

        // This is what you override
        do: function (req) {

            // Map
            var win = req.win;
            // Save the window
            win.save(function () {
                // Get the data
                var data = win.getFormData();

                // Make the request
                nx.util.serviceCall('Stripe.AsPDF', {
                    ds: req.ds,
                    id: req.id
                }, function (result) {

                    //
                    nx.util.processToolReturn(result);

                });

            });
        }
    }

});