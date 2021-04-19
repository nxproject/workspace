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

qx.Class.define('c._gridheaderrenderer', {

	extend: qx.ui.table.headerrenderer.Default,

	members:
	{
		// overridden
		createHeaderCell: function createHeaderCell(cellInfo) {

			var self = this;

			var widget = new c._gridheadercell();
			// Link
			nx.bucket.setCellInfo(widget, cellInfo);
			

			// Update
			self.updateHeaderCell(cellInfo, widget);

			return widget;
		}
	}
});