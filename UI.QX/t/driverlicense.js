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

qx.Class.define('t.driverlicense', {

    extend: qx.core.Object,

    implement: i.iTool,

    members: {

        caption: nx.setup.viaWeb + 'Scan',

        icon: nx.setup.viaWebIcon,

        setup: function (widget, button) { },

        click: function (widget) {

            // TBD

            //var value = widget.getValue();
            //if (nx.util.hasValue(value)) {
            //    var data = 'mailto:' + value;

            //    nx.util.runTool('QR', {
            //        data: data,
            //        caption: nx.setup.viaWeb + 'EMail'
            //    });
            //}

        }
    }

});