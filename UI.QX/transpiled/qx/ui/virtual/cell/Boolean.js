(function () {
  var $$dbClassInfo = {
    "dependsOn": {
      "qx.Class": {
        "usage": "dynamic",
        "require": true
      },
      "qx.ui.virtual.cell.AbstractImage": {
        "construct": true,
        "require": true
      },
      "qx.util.AliasManager": {
        "construct": true
      }
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
       * Jonathan Weiß (jonathan_rass)
  
  ************************************************************************ */
  qx.Class.define("qx.ui.virtual.cell.Boolean", {
    extend: qx.ui.virtual.cell.AbstractImage,

    /*
    *****************************************************************************
       CONSTRUCTOR
    *****************************************************************************
    */
    construct: function construct() {
      qx.ui.virtual.cell.AbstractImage.constructor.call(this);
      this.__aliasManager__P_308_0 = qx.util.AliasManager.getInstance();
    },

    /*
    *****************************************************************************
       PROPERTIES
    *****************************************************************************
    */
    properties: {
      appearance: {
        refine: true,
        init: "cell-boolean"
      },

      /**
      * The icon used to indicate the true state
      */
      iconTrue: {
        check: "String",
        themeable: true,
        apply: "_applyIconTrue"
      },

      /**
      * The icon used to indicate the false state
      */
      iconFalse: {
        check: "String",
        themeable: true,
        apply: "_applyIconFalse"
      }
    },

    /*
    *****************************************************************************
       MEMBERS
    *****************************************************************************
    */
    members: {
      __imageTrue__P_308_1: null,
      __imageFalse__P_308_2: null,
      __aliasManager__P_308_0: null,
      // property apply
      _applyIconTrue: function _applyIconTrue(value) {
        this.__imageTrue__P_308_1 = this.__aliasManager__P_308_0.resolve(value);
      },
      // property apply
      _applyIconFalse: function _applyIconFalse(value) {
        this.__imageFalse__P_308_2 = this.__aliasManager__P_308_0.resolve(value);
      },
      // overridden
      _identifyImage: function _identifyImage(value) {
        return value == true ? this.__imageTrue__P_308_1 : this.__imageFalse__P_308_2;
      }
    }
  });
  qx.ui.virtual.cell.Boolean.$$dbClassInfo = $$dbClassInfo;
})();

//# sourceMappingURL=Boolean.js.map?dt=1598051420200