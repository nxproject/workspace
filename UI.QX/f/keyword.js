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

qx.Class.define('f.keyword', {

    extend: qx.core.Object,

    implement: i.iFormatter,

    members: {

        format: function (widget, value, cb) {

            if (value) {
                value = value.toLowerCase().replace(/[^a-z0-9]/g, '');
            }
            cb(value);

        }
    }

});