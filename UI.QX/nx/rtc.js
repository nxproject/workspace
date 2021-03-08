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

qx.Class.define('nx.rtc', {

    extend: qx.core.Object,

    members: {

        winid: null,

        aoFld: null,

        init: function (aofld, winid) {

            var self = this;

            // Save
            self.aoFld = aofld;
            self.winid = winid;

        },

        SIOSend: function (value) {

            var self = this;

            // Send
            nx.desktop.user.SIOSend('$$object.data', {
                aofld: self.aoFld,
                winid: self.winid,
                value: value
            });

        }

    }

});