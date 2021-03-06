(function () {
  var $$dbClassInfo = {
    "dependsOn": {
      "qx.Class": {
        "usage": "dynamic",
        "require": true
      },
      "qx.ui.virtual.core.Scroller": {
        "construct": true,
        "require": true
      },
      "qx.ui.virtual.selection.MModel": {
        "require": true
      },
      "qx.data.controller.ISelection": {
        "require": true
      },
      "qx.data.Array": {
        "construct": true
      },
      "qx.ui.virtual.layer.Row": {},
      "qx.ui.list.provider.WidgetProvider": {},
      "qx.event.type.Data": {},
      "qx.util.DeferredCall": {},
      "qx.util.Delegate": {},
      "qx.lang.Type": {}
    }
  };
  qx.Bootstrap.executePendingDefers($$dbClassInfo);

  /* ************************************************************************
  
     qooxdoo - the new era of web development
  
     http://qooxdoo.org
  
     Copyright:
       2004-2010 1&1 Internet AG, Germany, http://www.1und1.de
  
     License:
       MIT: https://opensource.org/licenses/MIT
       See the LICENSE file in the project's top-level directory for details.
  
     Authors:
       * Christian Hagendorn (chris_schmidt)
  
  ************************************************************************ */

  /**
   * The <code>qx.ui.list.List</code> is based on the virtual infrastructure and
   * supports filtering, sorting, grouping, single selection, multi selection,
   * data binding and custom rendering.
   *
   * Using the virtual infrastructure has considerable advantages when there is a
   * huge amount of model items to render because the virtual infrastructure only
   * creates widgets for visible items and reuses them. This saves both creation
   * time and memory.
   *
   * With the {@link qx.ui.list.core.IListDelegate} interface it is possible
   * to configure the list's behavior (item and group renderer configuration,
   * filtering, sorting, grouping, etc.).
   *
   * Here's an example of how to use the widget:
   * <pre class="javascript">
   * //create the model data
   * var rawData = [];
   * for (var i = 0; i < 2500; i++) {
   *  rawData[i] = "Item No " + i;
   * }
   * var model = qx.data.marshal.Json.createModel(rawData);
   *
   * //create the list
   * var list = new qx.ui.list.List(model);
   *
   * //configure the lists's behavior
   * var delegate = {
   *   sorter : function(a, b) {
   *     return a > b ? 1 : a < b ? -1 : 0;
   *   }
   * };
   * list.setDelegate(delegate);
   *
   * //Pre-Select "Item No 20"
   * list.getSelection().push(model.getItem(20));
   *
   * //log selection changes
   * list.getSelection().addListener("change", function(e) {
   *   this.debug("Selection: " + list.getSelection().getItem(0));
   * }, this);
   * </pre>
   *
   * @childControl row-layer {qx.ui.virtual.layer.Row} layer for all rows
   */
  qx.Class.define("qx.ui.list.List", {
    extend: qx.ui.virtual.core.Scroller,
    include: [qx.ui.virtual.selection.MModel],
    implement: qx.data.controller.ISelection,

    /**
     * Creates the <code>qx.ui.list.List</code> with the passed model.
     *
     * @param model {qx.data.IListData|null?} model for the list.
     */
    construct: function construct(model) {
      qx.ui.virtual.core.Scroller.constructor.call(this, 0, 1, 20, 100);

      this._init();

      this.__defaultGroups__P_220_0 = new qx.data.Array();
      this.initGroups(this.__defaultGroups__P_220_0);

      if (model != null) {
        this.initModel(model);
      }

      this.initItemHeight();
    },
    events: {
      /**
       * Fired when the length of {@link #model} changes.
       */
      "changeModelLength": "qx.event.type.Data"
    },
    properties: {
      // overridden
      appearance: {
        refine: true,
        init: "virtual-list"
      },
      // overridden
      focusable: {
        refine: true,
        init: true
      },
      // overridden
      width: {
        refine: true,
        init: 100
      },
      // overridden
      height: {
        refine: true,
        init: 200
      },

      /** Data array containing the data which should be shown in the list. */
      model: {
        check: "qx.data.IListData",
        apply: "_applyModel",
        event: "changeModel",
        nullable: true,
        deferredInit: true
      },

      /** Default item height */
      itemHeight: {
        check: "Integer",
        init: 25,
        apply: "_applyRowHeight",
        themeable: true
      },

      /** Group item height */
      groupItemHeight: {
        check: "Integer",
        init: null,
        nullable: true,
        apply: "_applyGroupRowHeight",
        themeable: true
      },

      /**
       * The path to the property which holds the information that should be
       * displayed as a label. This is only needed if objects are stored in the
       * model.
       */
      labelPath: {
        check: "String",
        apply: "_applyLabelPath",
        nullable: true
      },

      /**
       * The path to the property which holds the information that should be
       * displayed as an icon. This is only needed if objects are stored in the
       * model and icons should be displayed.
       */
      iconPath: {
        check: "String",
        apply: "_applyIconPath",
        nullable: true
      },

      /**
       * The path to the property which holds the information that should be
       * displayed as a group label. This is only needed if objects are stored in the
       * model.
       */
      groupLabelPath: {
        check: "String",
        apply: "_applyGroupLabelPath",
        nullable: true
      },

      /**
       * A map containing the options for the label binding. The possible keys
       * can be found in the {@link qx.data.SingleValueBinding} documentation.
       */
      labelOptions: {
        apply: "_applyLabelOptions",
        nullable: true
      },

      /**
       * A map containing the options for the icon binding. The possible keys
       * can be found in the {@link qx.data.SingleValueBinding} documentation.
       */
      iconOptions: {
        apply: "_applyIconOptions",
        nullable: true
      },

      /**
       * A map containing the options for the group label binding. The possible keys
       * can be found in the {@link qx.data.SingleValueBinding} documentation.
       */
      groupLabelOptions: {
        apply: "_applyGroupLabelOptions",
        nullable: true
      },

      /**
       * Delegation object which can have one or more functions defined by the
       * {@link qx.ui.list.core.IListDelegate} interface.
       */
      delegate: {
        apply: "_applyDelegate",
        event: "changeDelegate",
        init: null,
        nullable: true
      },

      /**
       * Indicates that the list is managing the {@link #groups} automatically.
       */
      autoGrouping: {
        check: "Boolean",
        init: true
      },

      /**
       * Contains all groups for data binding, but do only manipulate the array
       * when the {@link #autoGrouping} is set to <code>false</code>.
       */
      groups: {
        check: "qx.data.Array",
        event: "changeGroups",
        nullable: false,
        deferredInit: true
      },

      /** 
       * Render list items with variable height, 
       * calculated from the individual item size. 
       */
      variableItemHeight: {
        check: "Boolean",
        apply: "_applyVariableItemHeight",
        nullable: false,
        init: true
      }
    },
    members: {
      /** @type {qx.ui.virtual.layer.Row} background renderer */
      _background: null,

      /** @type {qx.ui.list.provider.IListProvider} provider for cell rendering */
      _provider: null,

      /** @type {qx.ui.virtual.layer.Abstract} layer which contains the items. */
      _layer: null,

      /**
       * @type {Array} lookup table to get the model index from a row. To get the
       *   correct value after applying filter, sorter, group.
       *
       * Note the value <code>-1</code> indicates that the value is a group item.
       */
      __lookupTable__P_220_1: null,

      /** @type {Array} lookup table for getting the group index from the row */
      __lookupTableForGroup__P_220_2: null,

      /**
       * @type {Map} contains all groups with the items as children. The key is
       *   the group name and the value is an <code>Array</code> containing each
       *   item's model index.
       */
      __groupHashMap__P_220_3: null,

      /**
       * @type {Boolean} indicates when one or more <code>String</code> are used for grouping.
       */
      __groupStringsUsed__P_220_4: false,

      /**
       * @type {Boolean} indicates when one or more <code>Object</code> are used for grouping.
       */
      __groupObjectsUsed__P_220_5: false,

      /**
       * @type {Boolean} indicates when a default group is used for grouping.
       */
      __defaultGroupUsed__P_220_6: false,
      __defaultGroups__P_220_0: null,
      __deferredLayerUpdate__P_220_7: null,

      /**
       * Trigger a rebuild from the internal data structure.
       */
      refresh: function refresh() {
        this.__buildUpLookupTable__P_220_8();
      },
      // overridden
      _createChildControlImpl: function _createChildControlImpl(id, hash) {
        var control;

        switch (id) {
          case "row-layer":
            control = new qx.ui.virtual.layer.Row(null, null);
            break;
        }

        return control || qx.ui.list.List.prototype._createChildControlImpl.base.call(this, id);
      },

      /**
       * Initialize the virtual list provider.
       */
      _initWidgetProvider: function _initWidgetProvider() {
        this._provider = new qx.ui.list.provider.WidgetProvider(this);
      },

      /**
       * Initializes the virtual list.
       */
      _init: function _init() {
        this._initWidgetProvider();

        this.__lookupTable__P_220_1 = [];
        this.__lookupTableForGroup__P_220_2 = [];
        this.__groupHashMap__P_220_3 = {};
        this.__groupStringsUsed__P_220_4 = false;
        this.__groupObjectsUsed__P_220_5 = false;
        this.__defaultGroupUsed__P_220_6 = false;
        this.getPane().addListener("resize", this._onResize, this);

        this._initBackground();

        this._initLayer();
      },

      /**
       * Initializes the background renderer.
       */
      _initBackground: function _initBackground() {
        this._background = this.getChildControl("row-layer");
        this.getPane().addLayer(this._background);
      },

      /**
       * Initializes the layer for rendering.
       */
      _initLayer: function _initLayer() {
        this._layer = this._provider.createLayer();

        this._layer.addListener("updated", this._onLayerUpdated, this);

        this.getPane().addLayer(this._layer);
      },

      /*
      ---------------------------------------------------------------------------
        INTERNAL API
      ---------------------------------------------------------------------------
      */

      /**
       * Returns the model data for the given row.
       *
       * @param row {Integer} row to get data for.
       * @return {var|null} the row's model data.
       */
      _getDataFromRow: function _getDataFromRow(row) {
        var data = null;
        var model = this.getModel();

        if (model == null) {
          return null;
        }

        if (this._isGroup(row)) {
          data = this.getGroups().getItem(this._lookupGroup(row));
        } else {
          data = model.getItem(this._lookup(row));
        }

        if (data != null) {
          return data;
        } else {
          return null;
        }
      },

      /**
       * Return the internal lookup table. But do not manipulate the
       * lookup table!
       *
       * @return {Array} The internal lookup table.
       */
      _getLookupTable: function _getLookupTable() {
        return this.__lookupTable__P_220_1;
      },

      /**
       * Performs a lookup from row to model index.
       *
       * @param row {Number} The row to look at.
       * @return {Number} The model index or
       *   <code>-1</code> if the row is a group item.
       */
      _lookup: function _lookup(row) {
        return this.__lookupTable__P_220_1[row];
      },

      /**
       * Performs a lookup from row to group index.
       *
       * @param row {Number} The row to look at.
       * @return {Number} The group index or
       *   <code>-1</code> if the row is a not a group item.
       */
      _lookupGroup: function _lookupGroup(row) {
        return this.__lookupTableForGroup__P_220_2.indexOf(row);
      },

      /**
       * Performs a lookup from model index to row.
       *
       * @param index {Number} The index to look at.
       * @return {Number} The row or <code>-1</code>
       *  if the index is not a model index.
       */
      _reverseLookup: function _reverseLookup(index) {
        if (index < 0) {
          return -1;
        }

        return this.__lookupTable__P_220_1.indexOf(index);
      },

      /**
       * Checks if the passed row is a group or an item.
       *
       * @param row {Integer} row to check.
       * @return {Boolean} <code>true</code> if the row is a group element,
       *  <code>false</code> if the row is an item element.
       */
      _isGroup: function _isGroup(row) {
        return this._lookup(row) == -1;
      },

      /**
       * Returns the selectable model items.
       *
       * @return {qx.data.Array | null} The selectable items.
       */
      _getSelectables: function _getSelectables() {
        return this.getModel();
      },

      /*
      ---------------------------------------------------------------------------
        APPLY ROUTINES
      ---------------------------------------------------------------------------
      */
      // apply method
      _applyModel: function _applyModel(value, old) {
        if (value != null) {
          value.addListener("changeLength", this._onModelChange, this);
        }

        if (old != null) {
          old.removeListener("changeLength", this._onModelChange, this);
        }

        this._onModelChange();
      },
      // apply method
      _applyRowHeight: function _applyRowHeight(value, old) {
        this.getPane().getRowConfig().setDefaultItemSize(value);
      },
      // apply method
      _applyGroupRowHeight: function _applyGroupRowHeight(value, old) {
        this.__updateGroupRowHeight__P_220_9();
      },
      // apply method
      _applyLabelPath: function _applyLabelPath(value, old) {
        this._provider.setLabelPath(value);
      },
      // apply method
      _applyIconPath: function _applyIconPath(value, old) {
        this._provider.setIconPath(value);
      },
      // apply method
      _applyGroupLabelPath: function _applyGroupLabelPath(value, old) {
        this._provider.setGroupLabelPath(value);
      },
      // apply method
      _applyLabelOptions: function _applyLabelOptions(value, old) {
        this._provider.setLabelOptions(value);
      },
      // apply method
      _applyIconOptions: function _applyIconOptions(value, old) {
        this._provider.setIconOptions(value);
      },
      // apply method
      _applyGroupLabelOptions: function _applyGroupLabelOptions(value, old) {
        this._provider.setGroupLabelOptions(value);
      },
      // apply method
      _applyDelegate: function _applyDelegate(value, old) {
        this._provider.setDelegate(value);

        this.__buildUpLookupTable__P_220_8();
      },
      // property apply
      _applyVariableItemHeight: function _applyVariableItemHeight(value, old) {
        if (value) {
          this._setRowItemSize();
        } else {
          this.getPane().getRowConfig().resetItemSizes();
          this.getPane().fullUpdate();
        }
      },

      /*
      ---------------------------------------------------------------------------
        EVENT HANDLERS
      ---------------------------------------------------------------------------
      */

      /**
       * Event handler for the resize event.
       *
       * @param e {qx.event.type.Data} resize event.
       */
      _onResize: function _onResize(e) {
        this.getPane().getColumnConfig().setItemSize(0, e.getData().width);
      },

      /**
       * Event handler for the model change event.
       *
       * @param e {qx.event.type.Data} model change event.
       */
      _onModelChange: function _onModelChange(e) {
        // we have to remove the bindings before we rebuild the lookup table
        // otherwise bindings might be dispatched to wrong items
        // see: https://github.com/qooxdoo/qooxdoo/issues/196
        this._provider.removeBindings();

        this.__buildUpLookupTable__P_220_8();

        this._applyDefaultSelection();

        if (e instanceof qx.event.type.Data) {
          this.fireDataEvent("changeModelLength", e.getData(), e.getOldData());
        }
      },

      /**
       * Event handler for the updated event of the 
       * qx.ui.virtual.layer.WidgetCell layer.
       *
       * Recalculates the item sizes in a deffered call,
       * which only happens if we have variable item heights
       */
      _onLayerUpdated: function _onLayerUpdated() {
        if (this.isVariableItemHeight() === false) {
          return;
        }

        if (this.__deferredLayerUpdate__P_220_7 === null) {
          this.__deferredLayerUpdate__P_220_7 = new qx.util.DeferredCall(function () {
            this._setRowItemSize();
          }, this);
        }

        this.__deferredLayerUpdate__P_220_7.schedule();
      },

      /*
      ---------------------------------------------------------------------------
        HELPER ROUTINES
      ---------------------------------------------------------------------------
      */

      /**
       * Helper method to update the row count.
       */
      __updateRowCount__P_220_10: function __updateRowCount__P_220_10() {
        this.getPane().getRowConfig().setItemCount(this.__lookupTable__P_220_1.length);
        this.getPane().fullUpdate();
      },

      /**
       * Helper method to update group row heights.
       */
      __updateGroupRowHeight__P_220_9: function __updateGroupRowHeight__P_220_9() {
        /*
         * In case of having variableItemHeight set to true,
         * the group item height has a variable height as well
         * and will be set again in method _setRowItemSize 
         * which is a deferred call, being run after all changes.
         * Resetting the complete item sizes here and setting
         * the height of the group items, only leads to an
         * unnecessary flicker of the list items by shrinking and
         * growing them again.
         */
        if (this.isVariableItemHeight()) {
          return;
        }

        var rc = this.getPane().getRowConfig();
        var gh = this.getGroupItemHeight();
        rc.resetItemSizes();

        if (gh) {
          for (var i = 0, l = this.__lookupTable__P_220_1.length; i < l; ++i) {
            if (this.__lookupTable__P_220_1[i] == -1) {
              rc.setItemSize(i, gh);
            }
          }
        }
      },

      /**
       * Internal method for building the lookup table.
       */
      __buildUpLookupTable__P_220_8: function __buildUpLookupTable__P_220_8() {
        this.__lookupTable__P_220_1 = [];
        this.__lookupTableForGroup__P_220_2 = [];
        this.__groupHashMap__P_220_3 = {};

        if (this.isAutoGrouping()) {
          this.getGroups().removeAll();
        }

        var model = this.getModel();

        if (model != null) {
          this._runDelegateFilter(model);

          this._runDelegateSorter(model);

          this._runDelegateGroup(model);
        }

        this._updateSelection();

        this.__updateGroupRowHeight__P_220_9();

        this.__updateRowCount__P_220_10();
      },

      /**
       * Invokes filtering using the filter given in the delegate.
       *
       * @param model {qx.data.IListData} The model.
       */
      _runDelegateFilter: function _runDelegateFilter(model) {
        var filter = qx.util.Delegate.getMethod(this.getDelegate(), "filter");

        for (var i = 0, l = model.length; i < l; ++i) {
          if (filter == null || filter(model.getItem(i))) {
            this.__lookupTable__P_220_1.push(i);
          }
        }
      },

      /**
       * Invokes sorting using the sorter given in the delegate.
       *
       * @param model {qx.data.IListData} The model.
       */
      _runDelegateSorter: function _runDelegateSorter(model) {
        if (this.__lookupTable__P_220_1.length == 0) {
          return;
        }

        var sorter = qx.util.Delegate.getMethod(this.getDelegate(), "sorter");

        if (sorter != null) {
          this.__lookupTable__P_220_1.sort(function (a, b) {
            return sorter(model.getItem(a), model.getItem(b));
          });
        }
      },

      /**
       * Invokes grouping using the group result given in the delegate.
       *
       * @param model {qx.data.IListData} The model.
       */
      _runDelegateGroup: function _runDelegateGroup(model) {
        var groupMethod = qx.util.Delegate.getMethod(this.getDelegate(), "group");

        if (groupMethod != null) {
          for (var i = 0, l = this.__lookupTable__P_220_1.length; i < l; ++i) {
            var index = this.__lookupTable__P_220_1[i];
            var item = this.getModel().getItem(index);
            var group = groupMethod(item);

            this.__addGroup__P_220_11(group, index);
          }

          this.__lookupTable__P_220_1 = this.__createLookupFromGroup__P_220_12();
        }
      },

      /**
       * Adds a model index the the group.
       *
       * @param group {String|Object|null} the group.
       * @param index {Integer} model index to add.
       */
      __addGroup__P_220_11: function __addGroup__P_220_11(group, index) {
        // if group is null add to default group
        if (group == null) {
          this.__defaultGroupUsed__P_220_6 = true;
          group = "???";
        }

        var name = this.__getUniqueGroupName__P_220_13(group);

        if (this.__groupHashMap__P_220_3[name] == null) {
          this.__groupHashMap__P_220_3[name] = [];

          if (this.isAutoGrouping()) {
            this.getGroups().push(group);
          }
        }

        this.__groupHashMap__P_220_3[name].push(index);
      },

      /**
       * Creates a lookup table form the internal group hash map.
       *
       * @return {Array} the lookup table based on the internal group hash map.
       */
      __createLookupFromGroup__P_220_12: function __createLookupFromGroup__P_220_12() {
        this.__checkGroupStructure__P_220_14();

        var result = [];
        var row = 0;
        var groups = this.getGroups();

        for (var i = 0; i < groups.getLength(); i++) {
          var group = groups.getItem(i); // indicate that the value is a group

          result.push(-1);

          this.__lookupTableForGroup__P_220_2.push(row);

          row++;

          var key = this.__getUniqueGroupName__P_220_13(group);

          var groupMembers = this.__groupHashMap__P_220_3[key];

          if (groupMembers != null) {
            for (var k = 0; k < groupMembers.length; k++) {
              result.push(groupMembers[k]);
              row++;
            }
          }
        }

        return result;
      },

      /**
       * Returns an unique group name for the passed group.
       *
       * @param group {String|Object} Group to find unique group name.
       * @return {String} Unique group name.
       */
      __getUniqueGroupName__P_220_13: function __getUniqueGroupName__P_220_13(group) {
        var name = null;

        if (!qx.lang.Type.isString(group)) {
          var index = this.getGroups().indexOf(group);
          this.__groupObjectsUsed__P_220_5 = true;
          name = "group";

          if (index == -1) {
            name += this.getGroups().getLength();
          } else {
            name += index;
          }
        } else {
          this.__groupStringsUsed__P_220_4 = true;
          var name = group;
        }

        return name;
      },

      /**
       * Checks that <code>Object</code> and <code>String</code> are not mixed
       * as group identifier, otherwise an exception occurs.
       */
      __checkGroupStructure__P_220_14: function __checkGroupStructure__P_220_14() {
        if (this.__groupObjectsUsed__P_220_5 && this.__defaultGroupUsed__P_220_6 || this.__groupObjectsUsed__P_220_5 && this.__groupStringsUsed__P_220_4) {
          throw new Error("GroupingTypeError: You can't mix 'Objects' and 'Strings' as group identifier!");
        }
      },

      /**
       * Get the height of each visible item and set it as the
       * row size
       */
      _setRowItemSize: function _setRowItemSize() {
        var rowConfig = this.getPane().getRowConfig();
        var layer = this._layer;
        var firstRow = layer.getFirstRow();
        var lastRow = firstRow + layer.getRowSizes().length;

        for (var row = firstRow; row < lastRow; row++) {
          var widget = layer.getRenderedCellWidget(row, 0);

          if (widget !== null) {
            var height = widget.getSizeHint().height;
            rowConfig.setItemSize(row, height);
          }
        }
      }
    },
    destruct: function destruct() {
      this._disposeObjects("__deferredLayerUpdate__P_220_7");

      var model = this.getModel();

      if (model != null) {
        model.removeListener("changeLength", this._onModelChange, this);
      }

      var pane = this.getPane();

      if (pane != null) {
        pane.removeListener("resize", this._onResize, this);
      }

      this._background.dispose();

      this._provider.dispose();

      this._layer.dispose();

      this._background = this._provider = this._layer = this.__lookupTable__P_220_1 = this.__lookupTableForGroup__P_220_2 = this.__groupHashMap__P_220_3 = null;

      if (this.__defaultGroups__P_220_0) {
        this.__defaultGroups__P_220_0.dispose();
      }
    }
  });
  qx.ui.list.List.$$dbClassInfo = $$dbClassInfo;
})();

//# sourceMappingURL=List.js.map?dt=1598051405414