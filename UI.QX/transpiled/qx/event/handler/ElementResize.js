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
      },
      "qx.event.IEventHandler": {
        "require": true
      },
      "qx.core.IDisposable": {
        "require": true
      },
      "qx.event.Timer": {
        "construct": true
      },
      "qx.event.Registration": {
        "defer": "runtime",
        "require": true
      },
      "qx.core.ObjectRegistry": {},
      "qx.bom.element.Dimension": {},
      "qx.lang.Object": {},
      "qx.event.type.Data": {}
    }
  };
  qx.Bootstrap.executePendingDefers($$dbClassInfo);

  /* ************************************************************************
  
     qooxdoo - the new era of web development
  
     http://qooxdoo.org
  
     Copyright:
       2007-2008 1&1 Internet AG, Germany, http://www.1und1.de
  
     License:
       MIT: https://opensource.org/licenses/MIT
       See the LICENSE file in the project's top-level directory for details.
  
     Authors:
       * Fabian Jakobs (fjakobs)
  
  ************************************************************************ */

  /**
   * This handler fires a <code>resize</code> event if the size of a DOM element
   * changes.
   * 
   * NOTE: Instances of this class must be disposed of after use
   *
   */
  qx.Class.define("qx.event.handler.ElementResize", {
    extend: qx.core.Object,
    implement: [qx.event.IEventHandler, qx.core.IDisposable],

    /*
    *****************************************************************************
       CONSTRUCTOR
    *****************************************************************************
    */

    /**
     * @param manager {qx.event.Manager} Event manager for the window to use
     */
    construct: function construct(manager) {
      qx.core.Object.constructor.call(this);
      this.__manager__P_340_0 = manager;
      this.__elements__P_340_1 = {};
      this.__timer__P_340_2 = new qx.event.Timer(200);

      this.__timer__P_340_2.addListener("interval", this._onInterval, this);
    },

    /*
    *****************************************************************************
       STATICS
    *****************************************************************************
    */
    statics: {
      /** @type {Integer} Priority of this handler */
      PRIORITY: qx.event.Registration.PRIORITY_NORMAL,

      /** @type {Map} Supported event types */
      SUPPORTED_TYPES: {
        resize: true
      },

      /** @type {Integer} Which target check to use */
      TARGET_CHECK: qx.event.IEventHandler.TARGET_DOMNODE,

      /** @type {Integer} Whether the method "canHandleEvent" must be called */
      IGNORE_CAN_HANDLE: false
    },

    /*
    *****************************************************************************
       MEMBERS
    *****************************************************************************
    */
    members: {
      __elements__P_340_1: null,
      __manager__P_340_0: null,
      __timer__P_340_2: null,

      /*
      ---------------------------------------------------------------------------
        EVENT HANDLER INTERFACE
      ---------------------------------------------------------------------------
      */
      // interface implementation
      canHandleEvent: function canHandleEvent(target, type) {
        return target.tagName.toLowerCase() !== "body";
      },
      // interface implementation
      registerEvent: function registerEvent(target, type, capture) {
        var hash = qx.core.ObjectRegistry.toHashCode(target);
        var elements = this.__elements__P_340_1;

        if (!elements[hash]) {
          elements[hash] = {
            element: target,
            width: qx.bom.element.Dimension.getWidth(target),
            height: qx.bom.element.Dimension.getHeight(target)
          };

          this.__timer__P_340_2.start();
        }
      },
      // interface implementation
      unregisterEvent: function unregisterEvent(target, type, capture) {
        var hash = qx.core.ObjectRegistry.toHashCode(target);
        var elements = this.__elements__P_340_1;

        if (elements[hash]) {
          delete elements[hash];

          if (qx.lang.Object.isEmpty(elements)) {
            this.__timer__P_340_2.stop();
          }
        }
      },

      /**
       * Checks elements for width and height changes and fires resize event
       * if needed.
       *
       * @param e {qx.event.type.Data} The incoming data event
       */
      _onInterval: function _onInterval(e) {
        var elements = this.__elements__P_340_1;

        for (var key in elements) {
          var data = elements[key];
          var el = data.element;
          var width = qx.bom.element.Dimension.getWidth(el);
          var height = qx.bom.element.Dimension.getHeight(el);

          if (data.height !== height || data.width !== width) {
            qx.event.Registration.fireNonBubblingEvent(el, "resize", qx.event.type.Data, [{
              width: width,
              oldWidth: data.width,
              height: height,
              oldHeight: data.height
            }]);
            data.width = width;
            data.height = height;
          }
        }
      }
    },

    /*
    *****************************************************************************
       DESTRUCTOR
    *****************************************************************************
    */
    destruct: function destruct() {
      this.__manager__P_340_0 = this.__elements__P_340_1 = null;

      this._disposeObjects("__timer__P_340_2");
    },

    /*
    *****************************************************************************
       DEFER
    *****************************************************************************
    */
    defer: function defer(statics) {
      qx.event.Registration.addHandler(statics);
    }
  });
  qx.event.handler.ElementResize.$$dbClassInfo = $$dbClassInfo;
})();

//# sourceMappingURL=ElementResize.js.map?dt=1598051424202