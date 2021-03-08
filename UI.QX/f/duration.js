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

qx.Class.define('f.duration', {

    extend: qx.core.Object,

    implement: i.iFormatter,

    members: {

        format: function (widget, value, cb) {

            if (value) {
                var re = /(?<days>\d+d)|(?<hours>\d+h)|(?<minutes>\d+m)|(?<secs>\d+s)/g;
                var p = value.toLowerCase().replace(/\s/g, '').match(re);
                var days = '';
                var hours = '';
                var mins = '';
                var secs = '';
                p.forEach(function (piece) {
                    var last = piece.substr(piece.length - 1, 1);
                    switch (last) {
                        case 'd':
                            days = piece;
                            break;
                        case 'h':
                            hours = piece;
                            break;
                        case 'm':
                            mins = piece;
                            break;
                        case 's':
                            secs = piece;
                            break;
                    }
                });
                value = '';
                if (days) value += ':' + days;
                if (hours) value += ':' + hours;
                if (mins) value += ':' + mins;
                if (secs) value += ':' + secs;
                if (value.length) value = value.substr(1);
            }
            cb(value);

        }
    }

});