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

qx.Interface.define('i.iTool', {

    members: {

        caption: null,

        icon: null,

        allowed: function (widget, cb) { },

        when: function (widget, button) { },

        setup: function (widget, button) { },

        click: function (widget) { }
       
    }

});