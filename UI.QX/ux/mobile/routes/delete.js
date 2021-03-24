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

// Set the call
nx.calls.delete = function (req) {

    //  Confirm
    nx.util.confirm('Are you sure?', 'Deting entry', function (ok) {
    //
        if (ok) {
            nx.db.deleteObj(nx.env.getBucketItem('_obj'), null, function () {
                nx.office.goBack();
            });
        }
    });
};