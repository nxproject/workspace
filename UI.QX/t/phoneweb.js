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

qx.Class.define('t.phoneweb', {

    extend: qx.core.Object,

    implement: i.iTool,

    members: {

        caption: nx.setup.viaWeb + 'Voice',

        icon: nx.setup.viaWebIcon,

        setup: function (widget, button) { },

        click: function (widget) {

            var value = widget.getValue();
            if (nx.util.hasValue(value) && nx.util.isPhone(value)) {

                var data = 'tel:' + value;

                nx.util.runTool('QR', {
                    data: data,
                    caption: nx.setup.viaWeb + 'Voice'
                });

            }

        }
    }

});