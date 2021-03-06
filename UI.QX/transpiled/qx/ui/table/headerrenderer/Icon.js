(function () {
  var $$dbClassInfo = {
    "dependsOn": {
      "qx.Class": {
        "usage": "dynamic",
        "require": true
      },
      "qx.ui.table.headerrenderer.Default": {
        "construct": true,
        "require": true
      }
    }
  };
  qx.Bootstrap.executePendingDefers($$dbClassInfo);

  /* ************************************************************************
  
     qooxdoo - the new era of web development
  
     http://qooxdoo.org
  
     Copyright:
       2006 STZ-IDA, Germany, http://www.stz-ida.de
  
     License:
       MIT: https://opensource.org/licenses/MIT
       See the LICENSE file in the project's top-level directory for details.
  
     Authors:
       * Til Schneider (til132)
       * Carsten Lergenmueller (carstenl)
  
  ************************************************************************ */

  /**
   * A header cell renderer which renders an icon (only). The icon cannot be combined
   * with text.
   */
  qx.Class.define("qx.ui.table.headerrenderer.Icon", {
    extend: qx.ui.table.headerrenderer.Default,

    /*
    *****************************************************************************
       CONSTRUCTOR
    *****************************************************************************
    */

    /**
     * @param iconUrl {String} URL to the icon to show
     * @param tooltip {String ? ""} Text of the tooltip to show if the pointer hovers over the
     *                             icon
     */
    construct: function construct(iconUrl, tooltip) {
      qx.ui.table.headerrenderer.Default.constructor.call(this);

      if (iconUrl == null) {
        iconUrl = "";
      }

      this.setIconUrl(iconUrl);

      if (tooltip) {
        this.setToolTip(tooltip);
      }
    },

    /*
    *****************************************************************************
       PROPERTIES
    *****************************************************************************
    */
    properties: {
      /**
       * URL of the icon to show
       */
      iconUrl: {
        check: "String",
        init: ""
      }
    },

    /*
    *****************************************************************************
       MEMBERS
    *****************************************************************************
    */
    members: {
      // overridden
      updateHeaderCell: function updateHeaderCell(cellInfo, cellWidget) {
        qx.ui.table.headerrenderer.Icon.prototype.updateHeaderCell.base.call(this, cellInfo, cellWidget);
        cellWidget.setIcon(this.getIconUrl());
      }
    }
  });
  qx.ui.table.headerrenderer.Icon.$$dbClassInfo = $$dbClassInfo;
})();

//# sourceMappingURL=Icon.js.map?dt=1598051417534