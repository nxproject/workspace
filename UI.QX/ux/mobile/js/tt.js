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

nx.tt = {

    tagWidget: function (obj, fn, cb) {

        //
        if (obj) {

            var ds = obj._ds;
            var id = obj._id;

            //
            nx.util.serviceCall('AO.Tag', {
                user: nx.user.getName(),
                type: 'pin',
                ds: ds,
                id: id,
                action: fn
            }, function (result) {
                // Reload
                nx.user._reloadUser();
                //
                if (cb) {
                    cb(result);
                } else {
                    // Tell user
                    nx.util.notifyInfo('Tracking ' + result.value);
                }
            });
        }
    }

};