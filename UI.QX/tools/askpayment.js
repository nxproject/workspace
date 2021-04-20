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

qx.Class.define('tools.askpayment', {

    type: 'static',

    statics: {

        startindex: '200',
        startpriority: 'K',
        startprivilege: 'BILLING',
        ds: '_billinvoice',
        caption: 'Request Payment',
        icon: 'money',

        // This is what you override
        do: function (req) {

            // Get the data
            nx.util.serviceCall('Stripe.AskPayment', req, function (result) {

                //
                nx.util.processToolReturn(result);

            });
        }
    }

});