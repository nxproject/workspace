(function () {
  var $$dbClassInfo = {
    "dependsOn": {
      "qx.Class": {
        "usage": "dynamic",
        "require": true
      },
      "qx.ui.virtual.cell.Abstract": {
        "construct": true,
        "require": true
      },
      "qx.ui.virtual.cell.CellStylesheet": {
        "construct": true
      },
      "qx.util.PropertyUtil": {},
      "qx.lang.Object": {},
      "qx.theme.manager.Color": {},
      "qx.theme.manager.Font": {},
      "qx.bom.element.Style": {},
      "qx.bom.Style": {},
      "qx.theme.manager.Appearance": {}
    }
  };
  qx.Bootstrap.executePendingDefers($$dbClassInfo);

  /* ************************************************************************
  
     qooxdoo - the new era of web development
  
     http://qooxdoo.org
  
     Copyright:
       2006 STZ-IDA, Germany, http://www.stz-ida.de
       2004-2009 1&1 Internet AG, Germany, http://www.1und1.de
  
     License:
       MIT: https://opensource.org/licenses/MIT
       See the LICENSE file in the project's top-level directory for details.
  
     Authors:
       * Fabian Jakobs (fjakobs)
  
  ************************************************************************ */

  /**
   * Themeable Cell renderer.
   *
   * This cell renderer can be styled by an appearance theme.
   */
  qx.Class.define("qx.ui.virtual.cell.Cell", {
    extend: qx.ui.virtual.cell.Abstract,
    construct: function construct() {
      qx.ui.virtual.cell.Abstract.constructor.call(this);
      this.__stylesheet__P_309_0 = qx.ui.virtual.cell.CellStylesheet.getInstance();
      this.__userStyles__P_309_1 = {};
      this.__themeStyles__P_309_2 = {};
      this.__userPaddings__P_309_3 = {};
      this.__themePaddings__P_309_4 = {};
      this.__states__P_309_5 = {};
      this.__themeValues__P_309_6 = {};
      this.initAppearance();

      this.__initializeThemableProperties__P_309_7();
    },

    /*
    *****************************************************************************
       PROPERTIES
    *****************************************************************************
    */
    properties: {
      /**
       * The appearance ID. This ID is used to identify the appearance theme
       * entry to use for this cell.
       */
      appearance: {
        check: "String",
        init: "cell",
        apply: "_applyAppearance"
      },

      /** The cell's background color */
      backgroundColor: {
        nullable: true,
        check: "Color",
        apply: "_applyBackgroundColor",
        themeable: true
      },

      /** The cell's text color */
      textColor: {
        nullable: true,
        check: "Color",
        apply: "_applyTextColor",
        themeable: true
      },

      /** The text alignment of the cell's content */
      textAlign: {
        check: ["left", "center", "right", "justify"],
        nullable: true,
        themeable: true,
        apply: "_applyTextAlign"
      },

      /**
       * The cell's font. The value is either a font name defined in the font
       * theme or an instance of {@link qx.bom.Font}.
       */
      font: {
        nullable: true,
        apply: "_applyFont",
        check: "Font",
        themeable: true
      },

      /*
      ---------------------------------------------------------------------------
        PADDING
      ---------------------------------------------------------------------------
      */

      /** Padding of the widget (top) */
      paddingTop: {
        check: "Integer",
        init: 0,
        apply: "_applyPadding",
        themeable: true
      },

      /** Padding of the widget (right) */
      paddingRight: {
        check: "Integer",
        nullable: true,
        apply: "_applyPadding",
        themeable: true
      },

      /** Padding of the widget (bottom) */
      paddingBottom: {
        check: "Integer",
        nullable: true,
        apply: "_applyPadding",
        themeable: true
      },

      /** Padding of the widget (left) */
      paddingLeft: {
        check: "Integer",
        nullable: true,
        apply: "_applyPadding",
        themeable: true
      },

      /**
       * The 'padding' property is a shorthand property for setting 'paddingTop',
       * 'paddingRight', 'paddingBottom' and 'paddingLeft' at the same time.
       *
       * If four values are specified they apply to top, right, bottom and left
       * respectively. If there is only one value, it applies to all sides, if
       * there are two or three, the missing values are taken from the opposite
       * side.
       */
      padding: {
        group: ["paddingTop", "paddingRight", "paddingBottom", "paddingLeft"],
        mode: "shorthand",
        themeable: true
      }
    },

    /*
    *****************************************************************************
       MEMBERS
    *****************************************************************************
    */
    members: {
      /** @type {Array} List of all non CSS themable properties */
      __themableProperties__P_309_8: null,

      /** @type {String} Unique key over the current set of states */
      __statesKey__P_309_9: null,
      __states__P_309_5: null,
      __themeValues__P_309_6: null,
      __themeStyles__P_309_2: null,
      __userStyles__P_309_1: null,
      __userPaddings__P_309_3: null,
      __themePaddings__P_309_4: null,
      __isThemed__P_309_10: false,
      __stylesheet__P_309_0: null,

      /**
       * Collect all themable properties, which are not CSS properties
       */
      __initializeThemableProperties__P_309_7: function __initializeThemableProperties__P_309_7() {
        var PropertyUtil = qx.util.PropertyUtil;
        var cssProperties = qx.lang.Object.fromArray(this._getCssProperties());
        this.__themableProperties__P_309_8 = [];
        var clazz = this.constructor;

        while (clazz) {
          var properties = PropertyUtil.getProperties(clazz);

          for (var prop in properties) {
            if (!cssProperties[prop]) {
              this.__themableProperties__P_309_8.push(prop);
            }
          }

          clazz = clazz.superclass;
        }
      },

      /**
       * Get a list of all properties, which should be applied as CSS styles.
       *
       * @return {Array} List of property names
       */
      _getCssProperties: function _getCssProperties() {
        return ["backgroundColor", "textColor", "font", "textAlign", "paddingTop", "paddingRight", "paddingBottom", "paddingLeft"];
      },
      // property apply
      _applyAppearance: function _applyAppearance(value, old) {
        if (old) {
          this.__themeStyles__P_309_2 = {};
        }
      },

      /**
       * Compute the value of the given property
       *
       * @param propertyName {String} Name of the property
       * @return {var} The Property value
       */
      _getValue: function _getValue(propertyName) {
        if (this.__isThemed__P_309_10) {
          return qx.util.PropertyUtil.getThemeValue(this, propertyName);
        } else {
          return qx.util.PropertyUtil.getUserValue(this, propertyName);
        }
      },

      /**
       * Store a properties computed style string either in the user or in the
       * theme values. User values will be applied as inline styles, while theme
       * values are stored in a stylesheet.
       *
       * @param propertyName {String} The property name
       * @param styles {String} String with computed CSS styles
       */
      _storeStyle: function _storeStyle(propertyName, styles) {
        var store;

        if (this.__isThemed__P_309_10) {
          store = this.__themeStyles__P_309_2;
        } else {
          store = this.__userStyles__P_309_1;
        }

        if (styles === null) {
          delete store[propertyName];
        } else {
          store[propertyName] = styles;
        }
      },
      // property apply
      _applyBackgroundColor: function _applyBackgroundColor(value, old, name) {
        var value = this._getValue(name);

        if (!value) {
          this._storeStyle(name, null);
        } else {
          this._storeStyle(name, "background-color:" + qx.theme.manager.Color.getInstance().resolve(value));
        }
      },
      // property apply
      _applyTextColor: function _applyTextColor(value, old, name) {
        var value = this._getValue(name);

        if (!value) {
          this._storeStyle(name, null);
        } else {
          this._storeStyle(name, "color:" + qx.theme.manager.Color.getInstance().resolve(value));
        }
      },
      // property apply
      _applyTextAlign: function _applyTextAlign(value, old, name) {
        var value = this._getValue(name);

        if (!value) {
          this._storeStyle(name, null);
        } else {
          this._storeStyle(name, "text-align:" + value);
        }
      },
      // property apply
      _applyFont: function _applyFont(value, old, name) {
        var value = this._getValue(name);

        if (!value) {
          this._storeStyle(name, null);
        } else {
          var font = qx.theme.manager.Font.getInstance().resolve(value);

          this._storeStyle(name, qx.bom.element.Style.compile(font.getStyles()));
        }
      },
      // property apply
      _applyPadding: function _applyPadding(value, old, name) {
        var value = this._getValue(name);

        if (this.__isThemed__P_309_10) {
          var paddingStore = this.__themePaddings__P_309_4;
        } else {
          paddingStore = this.__userPaddings__P_309_3;
        }

        if (value === null) {
          delete paddingStore[name];
        } else {
          paddingStore[name] = value;
        }

        if (value === null) {
          this._storeStyle(name, null);
        } else {
          var cssKey = qx.bom.Style.getCssName(name);

          this._storeStyle(name, cssKey + ":" + value + "px");
        }
      },

      /*
      ---------------------------------------------------------------------------
        IMPLEMENT CELL API
      ---------------------------------------------------------------------------
      */
      // overridden
      getCellProperties: function getCellProperties(value, states) {
        this.__setStates__P_309_11(states);

        return {
          classes: this.getCssClasses(value, states),
          style: this.getStyles(value, states),
          attributes: this.getAttributes(value, states),
          content: this.getContent(value, states),
          insets: this.getInsets(value, states)
        };
      },
      // overridden
      getAttributes: function getAttributes(value, states) {
        return "";
      },
      // overridden
      getContent: function getContent(value, states) {
        return value;
      },
      // overridden
      getCssClasses: function getCssClasses(value, states) {
        var cssClass = this.__stylesheet__P_309_0.getCssClass(this.__statesKey__P_309_9) || "";
        return "qx-cell " + cssClass;
      },

      /**
       * Set the cell states and set the correct CSS class for the given state
       * combination
       *
       * @param states {Object} A map containing the cell's state names as map keys.
       */
      __setStates__P_309_11: function __setStates__P_309_11(states) {
        // Avoid errors if no states are set
        if (!states) {
          states = {};
        }

        var appearance = this.getAppearance();
        var statesKey = appearance + "-" + Object.keys(states).sort().join(" ");

        if (this.__statesKey__P_309_9 == statesKey) {
          return;
        }

        this.__statesKey__P_309_9 = statesKey;
        var themeStyles = this.__states__P_309_5[this.__statesKey__P_309_9];

        if (!themeStyles) {
          this.__clearThemedPropertyValues__P_309_12();

          this.__updateThemeableProperties__P_309_13(states);

          this.__computeCssClassForStates__P_309_14(states);

          this.__cacheThemedValues__P_309_15();

          this.__states__P_309_5[this.__statesKey__P_309_9] = 1;
        }

        this.__applyThemeValues__P_309_16();
      },

      /**
       * Remove the themed value from all CSS properties
       */
      __clearThemedPropertyValues__P_309_12: function __clearThemedPropertyValues__P_309_12() {
        var PropertyUtil = qx.util.PropertyUtil;

        var themableProperties = this._getCssProperties();

        for (var i = 0; i < themableProperties.length; i++) {
          PropertyUtil.deleteThemeValue(this, themableProperties[i]);
        }
      },

      /**
       * Set the new themed value for all CSS properties given the set of states
       *
       * @param states {Object} A map containing the cell's state names as map keys.
       */
      __updateThemeableProperties__P_309_13: function __updateThemeableProperties__P_309_13(states) {
        this.__themeStyles__P_309_2 = {};
        this.__isThemed__P_309_10 = true;
        var appearance = this.getAppearance();
        var PropertyUtil = qx.util.PropertyUtil;
        var styles = qx.theme.manager.Appearance.getInstance().styleFrom(appearance, states);

        for (var prop in styles) {
          if (styles[prop] !== undefined) {
            PropertyUtil.setThemed(this, prop, styles[prop]);
          }
        }

        this.__isThemed__P_309_10 = false;
      },

      /**
       * Compute a CSS class for the current values of all CSS properties
       */
      __computeCssClassForStates__P_309_14: function __computeCssClassForStates__P_309_14() {
        var styleString = Object.values(this.__themeStyles__P_309_2).join(";");

        this.__stylesheet__P_309_0.computeClassForStyles(this.__statesKey__P_309_9, styleString);
      },

      /**
       * Cache the themed values for the current state combination
       */
      __cacheThemedValues__P_309_15: function __cacheThemedValues__P_309_15() {
        var properties = this.__themableProperties__P_309_8;
        var PropertyUtil = qx.util.PropertyUtil;
        var themeValues = {};

        for (var i = 0; i < properties.length; i++) {
          var key = properties[i];
          var value = PropertyUtil.getThemeValue(this, key);

          if (value !== undefined) {
            themeValues[key] = value;
          }
        }

        this.__themeValues__P_309_6[this.__statesKey__P_309_9] = themeValues;
      },

      /**
       * Apply the themed values to the properties
       */
      __applyThemeValues__P_309_16: function __applyThemeValues__P_309_16() {
        var PropertyUtil = qx.util.PropertyUtil;
        var themeValues = this.__themeValues__P_309_6[this.__statesKey__P_309_9] || {};

        for (var key in themeValues) {
          PropertyUtil.setThemed(this, key, themeValues[key]);
        }
      },
      // overridden
      getStyles: function getStyles(value, states) {
        return Object.values(this.__userStyles__P_309_1).join(";");
      },
      // overridden
      getInsets: function getInsets(value, states) {
        var user = this.__userPaddings__P_309_3;
        var theme = this.__themePaddings__P_309_4;
        var top = (user.paddingTop !== undefined ? user.paddingTop : theme.paddingTop) || 0;
        var right = (user.paddingRight !== undefined ? user.paddingRight : theme.paddingRight) || 0;
        var bottom = (user.paddingBottom !== undefined ? user.paddingBottom : theme.paddingBottom) || 0;
        var left = (user.paddingLeft !== undefined ? user.paddingLeft : theme.paddingLeft) || 0;
        return [left + right, top + bottom];
      }
    },
    destruct: function destruct() {
      this.__stylesheet__P_309_0 = this.__userStyles__P_309_1 = this.__themeStyles__P_309_2 = this.__userPaddings__P_309_3 = this.__themePaddings__P_309_4 = this.__states__P_309_5 = this.__themeValues__P_309_6 = this.__themableProperties__P_309_8 = null;
    }
  });
  qx.ui.virtual.cell.Cell.$$dbClassInfo = $$dbClassInfo;
})();

//# sourceMappingURL=Cell.js.map?dt=1598051420286