(function () {
  var $$dbClassInfo = {
    "dependsOn": {
      "qx.Class": {
        "usage": "dynamic",
        "require": true
      },
      "qx.ui.form.AbstractSelectBox": {
        "construct": true,
        "require": true
      },
      "qx.ui.form.IStringForm": {
        "require": true
      },
      "qx.event.type.Focus": {
        "construct": true
      },
      "qx.ui.form.TextField": {},
      "qx.ui.form.Button": {}
    }
  };
  qx.Bootstrap.executePendingDefers($$dbClassInfo);

  /* ************************************************************************
  
     qooxdoo - the new era of web development
  
     http://qooxdoo.org
  
     Copyright:
       2004-2008 1&1 Internet AG, Germany, http://www.1und1.de
  
     License:
       MIT: https://opensource.org/licenses/MIT
       See the LICENSE file in the project's top-level directory for details.
  
     Authors:
       * Martin Wittemann (martinwittemann)
       * Sebastian Werner (wpbasti)
       * Jonathan Weiß (jonathan_rass)
  
  ************************************************************************ */

  /**
   * Basically a text fields which allows a selection from a list of
   * preconfigured options. Allows custom user input. Public API is value
   * oriented.
   *
   * To work with selections without custom input the ideal candidates are
   * the {@link SelectBox} or the {@link RadioGroup}.
   *
   * @childControl textfield {qx.ui.form.TextField} textfield component of the combobox
   * @childControl button {qx.ui.form.Button} button to open the list popup
   * @childControl list {qx.ui.form.List} list inside the popup
   */
  qx.Class.define("qx.ui.form.ComboBox", {
    extend: qx.ui.form.AbstractSelectBox,
    implement: [qx.ui.form.IStringForm],

    /*
    *****************************************************************************
       CONSTRUCTOR
    *****************************************************************************
    */
    construct: function construct() {
      qx.ui.form.AbstractSelectBox.constructor.call(this);

      var textField = this._createChildControl("textfield");

      this._createChildControl("button");

      this.addListener("tap", this._onTap); // forward the focusin and focusout events to the textfield. The textfield
      // is not focusable so the events need to be forwarded manually.

      this.addListener("focusin", function (e) {
        textField.fireNonBubblingEvent("focusin", qx.event.type.Focus);
      }, this);
      this.addListener("focusout", function (e) {
        textField.fireNonBubblingEvent("focusout", qx.event.type.Focus);
      }, this);
    },

    /*
    *****************************************************************************
       PROPERTIES
    *****************************************************************************
    */
    properties: {
      // overridden
      appearance: {
        refine: true,
        init: "combobox"
      },

      /**
       * String value which will be shown as a hint if the field is all of:
       * unset, unfocused and enabled. Set to null to not show a placeholder
       * text.
       */
      placeholder: {
        check: "String",
        nullable: true,
        apply: "_applyPlaceholder"
      }
    },

    /*
    *****************************************************************************
       EVENTS
    *****************************************************************************
    */
    events: {
      /** Whenever the value is changed this event is fired
       *
       *  Event data: The new text value of the field.
       */
      "changeValue": "qx.event.type.Data"
    },

    /*
    *****************************************************************************
       MEMBERS
    *****************************************************************************
    */
    members: {
      __preSelectedItem__P_201_0: null,
      __onInputId__P_201_1: null,
      // property apply
      _applyPlaceholder: function _applyPlaceholder(value, old) {
        this.getChildControl("textfield").setPlaceholder(value);
      },

      /*
      ---------------------------------------------------------------------------
        WIDGET API
      ---------------------------------------------------------------------------
      */
      // overridden
      _createChildControlImpl: function _createChildControlImpl(id, hash) {
        var control;

        switch (id) {
          case "textfield":
            control = new qx.ui.form.TextField();
            control.setFocusable(false);
            control.addState("inner");
            control.addListener("changeValue", this._onTextFieldChangeValue, this);
            control.addListener("blur", this.close, this);

            this._add(control, {
              flex: 1
            });

            break;

          case "button":
            control = new qx.ui.form.Button();
            control.setFocusable(false);
            control.setKeepActive(true);
            control.addState("inner");
            control.addListener("execute", this.toggle, this);

            this._add(control);

            break;

          case "list":
            // Get the list from the AbstractSelectBox
            control = qx.ui.form.ComboBox.prototype._createChildControlImpl.base.call(this, id); // Change selection mode

            control.setSelectionMode("single");
            break;
        }

        return control || qx.ui.form.ComboBox.prototype._createChildControlImpl.base.call(this, id);
      },
      // overridden

      /**
       * @lint ignoreReferenceField(_forwardStates)
       */
      _forwardStates: {
        focused: true,
        invalid: true
      },
      // overridden
      tabFocus: function tabFocus() {
        var field = this.getChildControl("textfield");
        field.getFocusElement().focus();
        field.selectAllText();
      },
      // overridden
      focus: function focus() {
        qx.ui.form.ComboBox.prototype.focus.base.call(this);
        this.getChildControl("textfield").getFocusElement().focus();
      },
      // interface implementation
      setValue: function setValue(value) {
        var textfield = this.getChildControl("textfield");

        if (textfield.getValue() == value) {
          return;
        } // Apply to text field


        textfield.setValue(value);
      },
      // interface implementation
      getValue: function getValue() {
        return this.getChildControl("textfield").getValue();
      },
      // interface implementation
      resetValue: function resetValue() {
        this.getChildControl("textfield").setValue(null);
      },

      /*
      ---------------------------------------------------------------------------
        EVENT LISTENERS
      ---------------------------------------------------------------------------
      */
      // overridden
      _onKeyPress: function _onKeyPress(e) {
        var popup = this.getChildControl("popup");
        var iden = e.getKeyIdentifier();

        if (iden == "Down" && e.isAltPressed()) {
          this.getChildControl("button").addState("selected");
          this.toggle();
          e.stopPropagation();
        } else if (iden == "Enter") {
          if (popup.isVisible()) {
            this._setPreselectedItem();

            this.resetAllTextSelection();
            this.close();
            e.stop();
          }
        } else if (popup.isVisible()) {
          qx.ui.form.ComboBox.prototype._onKeyPress.base.call(this, e);
        }
      },

      /**
       * Toggles the popup's visibility.
       *
       * @param e {qx.event.type.Pointer} Pointer tap event
       */
      _onTap: function _onTap(e) {
        this.close();
      },
      // overridden
      _onListPointerDown: function _onListPointerDown(e) {
        this._setPreselectedItem();
      },

      /**
       * Apply pre-selected item
       */
      _setPreselectedItem: function _setPreselectedItem() {
        if (this.__preSelectedItem__P_201_0) {
          var label = this.__preSelectedItem__P_201_0.getLabel();

          if (this.getFormat() != null) {
            label = this.getFormat().call(this, this.__preSelectedItem__P_201_0);
          } // check for translation


          if (label && label.translate) {
            label = label.translate();
          }

          this.setValue(label);
          this.__preSelectedItem__P_201_0 = null;
        }
      },
      // overridden
      _onListChangeSelection: function _onListChangeSelection(e) {
        var current = e.getData();

        if (current.length > 0) {
          // Ignore quick context (e.g. pointerover)
          // and configure the new value when closing the popup afterwards
          var list = this.getChildControl("list");
          var ctx = list.getSelectionContext();

          if (ctx == "quick" || ctx == "key") {
            this.__preSelectedItem__P_201_0 = current[0];
          } else {
            var label = current[0].getLabel();

            if (this.getFormat() != null) {
              label = this.getFormat().call(this, current[0]);
            } // check for translation


            if (label && label.translate) {
              label = label.translate();
            }

            this.setValue(label);
            this.__preSelectedItem__P_201_0 = null;
          }
        }
      },
      // overridden
      _onPopupChangeVisibility: function _onPopupChangeVisibility(e) {
        qx.ui.form.ComboBox.prototype._onPopupChangeVisibility.base.call(this, e); // Synchronize the list with the current value on every
        // opening of the popup. This is useful because through
        // the quick selection mode, the list may keep an invalid
        // selection on close or the user may enter text while
        // the combobox is closed and reopen it afterwards.


        var popup = this.getChildControl("popup");

        if (popup.isVisible()) {
          var list = this.getChildControl("list");
          var value = this.getValue();
          var item = null;

          if (value) {
            item = list.findItem(value);
          }

          if (item) {
            list.setSelection([item]);
          } else {
            list.resetSelection();
          }
        } else {
          // When closing the popup text should selected and field should
          // have the focus. Identical to when reaching the field using the TAB key.
          //
          // Only focus if popup was visible before. Fixes [BUG #4453].
          if (e.getOldData() == "visible") {
            this.tabFocus();
          }
        } // In all cases: Remove focused state from button


        this.getChildControl("button").removeState("selected");
      },

      /**
       * Reacts on value changes of the text field and syncs the
       * value to the combobox.
       *
       * @param e {qx.event.type.Data} Change event
       */
      _onTextFieldChangeValue: function _onTextFieldChangeValue(e) {
        var value = e.getData();
        var list = this.getChildControl("list");

        if (value != null) {
          // Select item when possible
          var item = list.findItem(value, false);

          if (item) {
            list.setSelection([item]);
          } else {
            list.resetSelection();
          }
        } else {
          list.resetSelection();
        } // Fire event


        this.fireDataEvent("changeValue", value, e.getOldData());
      },

      /*
      ---------------------------------------------------------------------------
        TEXTFIELD SELECTION API
      ---------------------------------------------------------------------------
      */

      /**
       * Returns the current selection.
       * This method only works if the widget is already created and
       * added to the document.
       *
       * @return {String|null}
       */
      getTextSelection: function getTextSelection() {
        return this.getChildControl("textfield").getTextSelection();
      },

      /**
       * Returns the current selection length.
       * This method only works if the widget is already created and
       * added to the document.
       *
       * @return {Integer|null}
       */
      getTextSelectionLength: function getTextSelectionLength() {
        return this.getChildControl("textfield").getTextSelectionLength();
      },

      /**
       * Set the selection to the given start and end (zero-based).
       * If no end value is given the selection will extend to the
       * end of the textfield's content.
       * This method only works if the widget is already created and
       * added to the document.
       *
       * @param start {Integer} start of the selection (zero-based)
       * @param end {Integer} end of the selection
       */
      setTextSelection: function setTextSelection(start, end) {
        this.getChildControl("textfield").setTextSelection(start, end);
      },

      /**
       * Clears the current selection.
       * This method only works if the widget is already created and
       * added to the document.
       *
       */
      clearTextSelection: function clearTextSelection() {
        this.getChildControl("textfield").clearTextSelection();
      },

      /**
       * Selects the whole content
       *
       */
      selectAllText: function selectAllText() {
        this.getChildControl("textfield").selectAllText();
      },

      /**
       * Clear any text selection, then select all text
       *
       */
      resetAllTextSelection: function resetAllTextSelection() {
        this.clearTextSelection();
        this.selectAllText();
      }
    }
  });
  qx.ui.form.ComboBox.$$dbClassInfo = $$dbClassInfo;
})();

//# sourceMappingURL=ComboBox.js.map?dt=1598051402172