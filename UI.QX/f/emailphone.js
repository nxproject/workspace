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

qx.Class.define('f.emailphone', {

    extend: qx.core.Object,

    implement: i.iFormatter,

    members: {

        format: function (widget, value, cb) {

            if (value) {

                // EMail?
                if (value.indexOf('@') !== -1) {
                    value = nx.util.ifEmpty(value).replace(/ /g, '').toLowerCase();
                } else {
                    var raw = value;
                    if (!nx.util.startsWith(raw, '+')) {
                        raw = raw.replace(/[^0-9]+/g, '');
                        if (raw.length < 10) raw = ('0000000000' + raw).slice(-10);
                        value = '(' + raw.substr(0, 3) + ') ' + raw.substr(3, 3) + '-' + raw.substr(6, 4);
                    }
                }
            }
            cb(value);

        }
    }

});