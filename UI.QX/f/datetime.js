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

qx.Class.define('f.datetime', {

    extend: qx.core.Object,

    implement: i.iFormatter,

    construct: function (fmt) {

        var self = this;

        // Save
        self._fmt = fmt;

        // Call base
        this.base(arguments);

    },

    members: {

        format: function (widget, value, cb) {

            var self = this;

            if (value) {
                value = moment(chrono.parseDate(value)).format(self._fmt);
            }
            cb(value);

        }
    }

});