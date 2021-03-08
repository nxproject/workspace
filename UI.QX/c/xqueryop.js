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

qx.Class.define('c.xqueryop', {

    extend: c.combobox,

    statics: {

        makeSelf: function (req) {

            var widget = new c.xqueryop(req);

            // Force list
            widget.setChoices(['Eq', 'Ne',
                    'Gt', 'Gte',
                    'Lt', 'Lte',
                    'In', 'Nin',
                    'Like', 'Notlike',
                    'Exists', 'Notexists',
                    'Any']);

            return widget;

        }
    }

});