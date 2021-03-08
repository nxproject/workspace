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

qx.Class.define('f.creditcardexp', {

    extend: qx.core.Object,

    implement: i.iFormatter,

    members: {

        format: function (widget, value, cb) {

            if (value) {
                var raw = value.replace(/[^0-9]+/g, '');
                if (raw.length < 4) raw = ('0000' + raw).slice(-4);
                value = raw.substr(0, 2) + '/' + raw.substr(2, 2);
            }
            cb(value);

        }
    }

});