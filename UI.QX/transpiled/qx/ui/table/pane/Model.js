(function () {
  var $$dbClassInfo = {
    "dependsOn": {
      "qx.Class": {
        "usage": "dynamic",
        "require": true
      },
      "qx.core.Object": {
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
  
  ************************************************************************ */

  /**
   * The model of a table pane. This model works as proxy to a
   * {@link qx.ui.table.columnmodel.Basic} and manages the visual order of the columns shown in
   * a {@link Pane}.
   */
  qx.Class.define("qx.ui.table.pane.Model", {
    extend: qx.core.Object,

    /*
    *****************************************************************************
       CONSTRUCTOR
    *****************************************************************************
    */

    /**
     *
     * @param tableColumnModel {qx.ui.table.columnmodel.Basic} The TableColumnModel of which this
     *    model is the proxy.
     */
    construct: function construct(tableColumnModel) {
      qx.core.Object.constructor.call(this);
      this.setTableColumnModel(tableColumnModel);
    },

    /*
    *****************************************************************************
       EVENTS
    *****************************************************************************
    */
    events: {
      /** Fired when the model changed. */
      "modelChanged": "qx.event.type.Event"
    },

    /*
    *****************************************************************************
       STATICS
    *****************************************************************************
    */
    statics: {
      /** @type {string} The type of the event fired when the model changed. */
      EVENT_TYPE_MODEL_CHANGED: "modelChanged"
    },

    /*
    *****************************************************************************
       PROPERTIES
    *****************************************************************************
    */
    properties: {
      /** The visible x position of the first column this model should contain. */
      firstColumnX: {
        check: "Integer",
        init: 0,
        apply: "_applyFirstColumnX"
      },

      /**
       * The maximum number of columns this model should contain. If -1 this model will
       * contain all remaining columns.
       */
      maxColumnCount: {
        check: "Number",
        init: -1,
        apply: "_applyMaxColumnCount"
      }
    },

    /*
    *****************************************************************************
       MEMBERS
    *****************************************************************************
    */
    members: {
      __columnCount__P_296_0: null,
      __tableColumnModel__P_296_1: null,
      // property modifier
      _applyFirstColumnX: function _applyFirstColumnX(value, old) {
        this.__columnCount__P_296_0 = null;
        this.fireEvent(qx.ui.table.pane.Model.EVENT_TYPE_MODEL_CHANGED);
      },
      // property modifier
      _applyMaxColumnCount: function _applyMaxColumnCount(value, old) {
        this.__columnCount__P_296_0 = null;
        this.fireEvent(qx.ui.table.pane.Model.EVENT_TYPE_MODEL_CHANGED);
      },

      /**
       * Connects the table model to the column model
       *
       * @param tableColumnModel {qx.ui.table.columnmodel.Basic} the column model
       */
      setTableColumnModel: function setTableColumnModel(tableColumnModel) {
        if (this.__tableColumnModel__P_296_1) {
          this.__tableColumnModel__P_296_1.removeListener("visibilityChangedPre", this._onColVisibilityChanged, this);

          this.__tableColumnModel__P_296_1.removeListener("headerCellRendererChanged", this._onHeaderCellRendererChanged, this);
        }

        this.__tableColumnModel__P_296_1 = tableColumnModel;

        this.__tableColumnModel__P_296_1.addListener("visibilityChangedPre", this._onColVisibilityChanged, this);

        this.__tableColumnModel__P_296_1.addListener("headerCellRendererChanged", this._onHeaderCellRendererChanged, this);

        this.__columnCount__P_296_0 = null;
      },

      /**
       * Event handler. Called when the visibility of a column has changed.
       *
       * @param evt {Map} the event.
       */
      _onColVisibilityChanged: function _onColVisibilityChanged(evt) {
        this.__columnCount__P_296_0 = null;
        this.fireEvent(qx.ui.table.pane.Model.EVENT_TYPE_MODEL_CHANGED);
      },

      /**
       * Event handler. Called when the cell renderer of a column has changed.
       *
       * @param evt {Map} the event.
       */
      _onHeaderCellRendererChanged: function _onHeaderCellRendererChanged(evt) {
        this.fireEvent(qx.ui.table.pane.Model.EVENT_TYPE_MODEL_CHANGED);
      },

      /**
       * Returns the number of columns in this model.
       *
       * @return {Integer} the number of columns in this model.
       */
      getColumnCount: function getColumnCount() {
        if (this.__columnCount__P_296_0 == null) {
          var firstX = this.getFirstColumnX();
          var maxColCount = this.getMaxColumnCount();

          var totalColCount = this.__tableColumnModel__P_296_1.getVisibleColumnCount();

          if (maxColCount == -1 || firstX + maxColCount > totalColCount) {
            this.__columnCount__P_296_0 = totalColCount - firstX;
          } else {
            this.__columnCount__P_296_0 = maxColCount;
          }
        }

        return this.__columnCount__P_296_0;
      },

      /**
       * Returns the model index of the column at the position <code>xPos</code>.
       *
       * @param xPos {Integer} the x position in the table pane of the column.
       * @return {Integer} the model index of the column.
       */
      getColumnAtX: function getColumnAtX(xPos) {
        var firstX = this.getFirstColumnX();
        return this.__tableColumnModel__P_296_1.getVisibleColumnAtX(firstX + xPos);
      },

      /**
       * Returns the x position of the column <code>col</code>.
       *
       * @param col {Integer} the model index of the column.
       * @return {Integer} the x position in the table pane of the column.
       */
      getX: function getX(col) {
        var firstX = this.getFirstColumnX();
        var maxColCount = this.getMaxColumnCount();
        var x = this.__tableColumnModel__P_296_1.getVisibleX(col) - firstX;

        if (x >= 0 && (maxColCount == -1 || x < maxColCount)) {
          return x;
        } else {
          return -1;
        }
      },

      /**
       * Gets the position of the left side of a column (in pixels, relative to the
       * left side of the table pane).
       *
       * This value corresponds to the sum of the widths of all columns left of the
       * column.
       *
       * @param col {Integer} the model index of the column.
       * @return {var} the position of the left side of the column.
       */
      getColumnLeft: function getColumnLeft(col) {
        var left = 0;
        var colCount = this.getColumnCount();

        for (var x = 0; x < colCount; x++) {
          var currCol = this.getColumnAtX(x);

          if (currCol == col) {
            return left;
          }

          left += this.__tableColumnModel__P_296_1.getColumnWidth(currCol);
        }

        return -1;
      },

      /**
       * Returns the total width of all columns in the model.
       *
       * @return {Integer} the total width of all columns in the model.
       */
      getTotalWidth: function getTotalWidth() {
        var totalWidth = 0;
        var colCount = this.getColumnCount();

        for (var x = 0; x < colCount; x++) {
          var col = this.getColumnAtX(x);
          totalWidth += this.__tableColumnModel__P_296_1.getColumnWidth(col);
        }

        return totalWidth;
      }
    },

    /*
    *****************************************************************************
       DESTRUCTOR
    *****************************************************************************
    */
    destruct: function destruct() {
      if (this.__tableColumnModel__P_296_1) {
        this.__tableColumnModel__P_296_1.removeListener("visibilityChangedPre", this._onColVisibilityChanged, this);

        this.__tableColumnModel__P_296_1.removeListener("headerCellRendererChanged", this._onHeaderCellRendererChanged, this);
      }

      this.__tableColumnModel__P_296_1 = null;
    }
  });
  qx.ui.table.pane.Model.$$dbClassInfo = $$dbClassInfo;
})();

//# sourceMappingURL=Model.js.map?dt=1598051418326