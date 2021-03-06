(function () {
  var $$dbClassInfo = {
    "dependsOn": {
      "qx.Class": {
        "usage": "dynamic",
        "require": true
      },
      "qx.ui.container.Composite": {
        "construct": true,
        "require": true
      },
      "qx.ui.layout.Grow": {
        "construct": true
      },
      "qx.theme.manager.Decoration": {}
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
       * Fabian Jakobs (fjakobs)
  
  ************************************************************************ */

  /**
   * The focus indicator widget
   */
  qx.Class.define("qx.ui.table.pane.FocusIndicator", {
    extend: qx.ui.container.Composite,

    /**
     * @param scroller {qx.ui.table.pane.Scroller} The scroller, which contains this focus indicator
     */
    construct: function construct(scroller) {
      // use the grow layout to make sure that the editing control
      // always fills the focus indicator box.
      qx.ui.container.Composite.constructor.call(this, new qx.ui.layout.Grow());
      this.__scroller__P_294_0 = scroller;
      this.setKeepActive(true);
      this.addListener("keypress", this._onKeyPress, this);
    },
    properties: {
      // overridden
      visibility: {
        refine: true,
        init: "excluded"
      },

      /** Table row, where the indicator is placed. */
      row: {
        check: "Integer",
        nullable: true
      },

      /** Table column, where the indicator is placed. */
      column: {
        check: "Integer",
        nullable: true
      }
    },
    members: {
      __scroller__P_294_0: null,

      /**
       * Keypress handler. Suppress all key events but "Enter" and "Escape"
       *
       * @param e {qx.event.type.KeySequence} key event
       */
      _onKeyPress: function _onKeyPress(e) {
        var iden = e.getKeyIdentifier();

        if (iden !== "Escape" && iden !== "Enter") {
          e.stopPropagation();
        }
      },

      /**
       * Move the focus indicator to the given table cell.
       *
       * @param col {Integer?null} The table column
       * @param row {Integer?null} The table row
       */
      moveToCell: function moveToCell(col, row) {
        // check if the focus indicator is shown and if the new column is
        // editable. if not, just exclude the indicator because the pointer events
        // should go to the cell itself linked with HTML links [BUG #4250]
        if (!this.__scroller__P_294_0.getShowCellFocusIndicator() && !this.__scroller__P_294_0.getTable().getTableModel().isColumnEditable(col)) {
          this.exclude();
          return;
        } else {
          this.show();
        }

        if (col == null) {
          this.hide();
          this.setRow(null);
          this.setColumn(null);
        } else {
          var xPos = this.__scroller__P_294_0.getTablePaneModel().getX(col);

          if (xPos == -1) {
            this.hide();
            this.setRow(null);
            this.setColumn(null);
          } else {
            var table = this.__scroller__P_294_0.getTable();

            var columnModel = table.getTableColumnModel();

            var paneModel = this.__scroller__P_294_0.getTablePaneModel();

            var firstRow = this.__scroller__P_294_0.getTablePane().getFirstVisibleRow();

            var rowHeight = table.getRowHeight();
            var wt = 0;
            var wr = 0;
            var wb = 0;
            var wl = 0;
            var decoKey = this.getDecorator();

            if (decoKey) {
              var deco = qx.theme.manager.Decoration.getInstance().resolve(decoKey);

              if (deco) {
                wt = deco.getWidthTop();
                wr = deco.getWidthRight();
                wb = deco.getWidthBottom();
                wl = deco.getWidthLeft();
              }
            }

            this.setUserBounds(paneModel.getColumnLeft(col) - (wt - 1), (row - firstRow) * rowHeight - (wr - 1), columnModel.getColumnWidth(col) + (wt + wb - 3), rowHeight + (wl + wr - 2));
            this.show();
            this.setRow(row);
            this.setColumn(col);
          }
        }
      }
    },
    destruct: function destruct() {
      this.__scroller__P_294_0 = null;
    }
  });
  qx.ui.table.pane.FocusIndicator.$$dbClassInfo = $$dbClassInfo;
})();

//# sourceMappingURL=FocusIndicator.js.map?dt=1598051418173