(function () {
  var $$dbClassInfo = {
    "dependsOn": {
      "qx.util.DeferredCallManager": {
        "require": true,
        "construct": true
      },
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
       2007-2008 1&1 Internet AG, Germany, http://www.1und1.de
  
     License:
       MIT: https://opensource.org/licenses/MIT
       See the LICENSE file in the project's top-level directory for details.
  
     Authors:
       * Fabian Jakobs (fjakobs)
  
  ************************************************************************ */

  /**
   * This class represents a wrapper for functions, which should be called after
   * the current thread of JavaScript has finished and the control is returned to
   * the browser. The wrapped function will at most be called once after the control
   * has been given back to the browser, independent of the number of {@link #call}
   * calls.
   * 
   * This class does not need to be disposed, although doing so will cancel any
   * pending call
   *
   * @require(qx.util.DeferredCallManager)
   */
  qx.Class.define("qx.util.DeferredCall", {
    extend: qx.core.Object,

    /*
    *****************************************************************************
       CONSTRUCTOR
    *****************************************************************************
    */

    /**
     * @param callback {Function} The callback
     * @param context {Object?window} the context in which the function will be called.
     */
    construct: function construct(callback, context) {
      qx.core.Object.constructor.call(this);
      this.__callback__P_107_0 = callback;
      this.__context__P_107_1 = context || null;
      this.__manager__P_107_2 = qx.util.DeferredCallManager.getInstance();
    },

    /*
    *****************************************************************************
       MEMBERS
    *****************************************************************************
    */
    members: {
      __callback__P_107_0: null,
      __context__P_107_1: null,
      __manager__P_107_2: null,

      /**
       * Prevent the callback from being called.
       */
      cancel: function cancel() {
        this.__manager__P_107_2.cancel(this);
      },

      /**
       * Issue a deferred call of the callback.
       */
      schedule: function schedule() {
        this.__manager__P_107_2.schedule(this);
      },

      /**
       * Calls the callback directly.
       */
      call: function call() {
        {
          // warn if the context is disposed
          var context = this.__context__P_107_1;

          if (context && context.isDisposed && context.isDisposed()) {
            this.warn("The context object '" + context + "' of the defered call '" + this + "'is already disposed.");
          }
        }
        this.__context__P_107_1 ? this.__callback__P_107_0.apply(this.__context__P_107_1) : this.__callback__P_107_0();
      }
    },

    /*
    *****************************************************************************
       DESTRUCTOR
    *****************************************************************************
    */
    destruct: function destruct() {
      this.cancel();
      this.__context__P_107_1 = this.__callback__P_107_0 = this.__manager__P_107_2 = null;
    }
  });
  qx.util.DeferredCall.$$dbClassInfo = $$dbClassInfo;
})();

//# sourceMappingURL=DeferredCall.js.map?dt=1598045175219