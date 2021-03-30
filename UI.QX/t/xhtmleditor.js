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

qx.Class.define('t.xhtmleditor', {

    extend: qx.core.Object,

    implement: i.iTool,

    members: {

        caption: 'Edit',

        icon: 'pencil',

        setup: function (widget, button) { },

        click: function (widget) {

            alert('ok');

        }
    }

});