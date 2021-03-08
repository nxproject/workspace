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

qx.Class.define('c._gridcellrenderer', {

    extend: qx.ui.table.cellrenderer.Default,

    members:
    {
        // overridden
        _getCellStyle: function _getCellStyle(cellInfo) {
            return 'white-space: break-spaces';
        },

    }

});