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

qx.Class.define('tools.SystemSettings', {

    type: 'static',

    statics: {

        startindex: '100',
        startgroup: 'System',
        startpriority: 'K',
        startskipds: 'y',
        startprivilege: '*',

        // This is what you override
        do: function (req) {

            nx.desktop.addWindowDS({
                nxid: 'sitesett',
                ds: '_sys',
                id: '_info',
                view: '_info',
                icon: 'computer',
                caption: 'Site Settings'
            });

        }

    }

});