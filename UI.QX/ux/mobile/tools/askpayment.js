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

nx.tools.askpayment = function (req) {

    // Get the data
    nx.util.serviceCall('Stripe.AskPayment', req, function (result) {

        // Get the object
        var obj = nx.env.getBucketItem('_obj');
        // Build the call request
        var creq = {
            ds: obj._ds,
            id: obj._id,
            data: obj
        };

        // Call
        nx.util.serviceCall('Stripe.AskPayment', creq, function (result) {
            //
            nx.util.processToolReturn(result);
        });

    });

}