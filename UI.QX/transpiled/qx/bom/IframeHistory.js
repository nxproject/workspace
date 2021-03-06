(function () {
  var $$dbClassInfo = {
    "dependsOn": {
      "qx.core.Environment": {
        "defer": "load",
        "require": true
      },
      "qx.Class": {
        "usage": "dynamic",
        "require": true
      },
      "qx.bom.History": {
        "construct": true,
        "require": true
      },
      "qx.core.IDisposable": {
        "require": true
      },
      "qx.lang.Type": {},
      "qx.event.Timer": {},
      "qx.bom.client.Engine": {},
      "qx.bom.client.Browser": {},
      "qx.event.Idle": {},
      "qx.bom.Iframe": {},
      "qx.util.ResourceManager": {}
    },
    "environment": {
      "provided": [],
      "required": {
        "engine.name": {
          "className": "qx.bom.client.Engine"
        },
        "browser.version": {
          "className": "qx.bom.client.Browser"
        }
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
       * Sebastian Werner (wpbasti)
       * Andreas Ecker (ecker)
       * Fabian Jakobs (fjakobs)
       * Mustafa Sak (msak)
  
  ************************************************************************ */

  /**
   * Implements an iFrame based history manager for IE 6/7/8.
   *
   * Creates a hidden iFrame and uses document.write to store entries in the
   * history browser's stack.
   *
   * This class must be disposed of after use
   *
   * @internal
   */
  qx.Class.define("qx.bom.IframeHistory", {
    extend: qx.bom.History,
    implement: [qx.core.IDisposable],
    construct: function construct() {
      qx.bom.History.constructor.call(this);

      this.__initTimer__P_178_0();
    },
    members: {
      __iframe__P_178_1: null,
      __iframeReady__P_178_2: false,
      __writeStateTimner__P_178_3: null,
      __dontApplyState__P_178_4: null,
      __locationState__P_178_5: null,
      // overridden
      _setInitialState: function _setInitialState() {
        qx.bom.IframeHistory.prototype._setInitialState.base.call(this);

        this.__locationState__P_178_5 = this._getHash();
      },
      //overridden
      _setHash: function _setHash(value) {
        qx.bom.IframeHistory.prototype._setHash.base.call(this, value);

        this.__locationState__P_178_5 = this._encode(value);
      },
      //overridden
      addToHistory: function addToHistory(state, newTitle) {
        if (!qx.lang.Type.isString(state)) {
          state = state + "";
        }

        if (qx.lang.Type.isString(newTitle)) {
          this.setTitle(newTitle);
          this._titles[state] = newTitle;
        }

        if (this.getState() !== state) {
          this.setState(state);
        }

        this.fireDataEvent("request", state);
      },
      //overridden
      _onHistoryLoad: function _onHistoryLoad(state) {
        this._setState(state);

        this.fireDataEvent("request", state);

        if (this._titles[state] != null) {
          this.setTitle(this._titles[state]);
        }
      },

      /**
       * Helper function to set state property. This will only be called
       * by _onHistoryLoad. It determines, that no apply of state will be called.
       * @param state {String} State loaded from history
       */
      _setState: function _setState(state) {
        this.__dontApplyState__P_178_4 = true;
        this.setState(state);
        this.__dontApplyState__P_178_4 = false;
      },
      //overridden
      _applyState: function _applyState(value, old) {
        if (this.__dontApplyState__P_178_4) {
          return;
        }

        this._writeState(value);
      },

      /**
       * Get state from the iframe
       *
       * @return {String} current state of the browser history
       */
      _readState: function _readState() {
        if (!this.__iframeReady__P_178_2) {
          return this._decode(this._getHash());
        }

        var doc = this.__iframe__P_178_1.contentWindow.document;
        var elem = doc.getElementById("state");
        return elem ? this._decode(elem.innerText) : "";
      },

      /**
       * Store state to the iframe
       *
       * @param state {String} state to save
       */
      _writeState: function _writeState(state) {
        if (!this.__iframeReady__P_178_2) {
          this.__clearWriteSateTimer__P_178_6();

          this.__writeStateTimner__P_178_3 = qx.event.Timer.once(function () {
            this._writeState(state);
          }, this, 50);
          return;
        }

        this.__clearWriteSateTimer__P_178_6();

        var state = this._encode(state); // IE8 is sometimes recognizing a hash change as history entry. Cause of sporadic surface of this behavior, we have to prevent setting hash.


        if (qx.core.Environment.get("engine.name") == "mshtml" && qx.core.Environment.get("browser.version") != 8) {
          this._setHash(state);
        }

        var doc = this.__iframe__P_178_1.contentWindow.document;
        doc.open();
        doc.write('<html><body><div id="state">' + state + '</div></body></html>');
        doc.close();
      },

      /**
       * Helper function to clear the write state timer.
       */
      __clearWriteSateTimer__P_178_6: function __clearWriteSateTimer__P_178_6() {
        if (this.__writeStateTimner__P_178_3) {
          this.__writeStateTimner__P_178_3.stop();

          this.__writeStateTimner__P_178_3.dispose();
        }
      },

      /**
       * Initialize the polling timer
       */
      __initTimer__P_178_0: function __initTimer__P_178_0() {
        this.__initIframe__P_178_7(function () {
          qx.event.Idle.getInstance().addListener("interval", this.__onHashChange__P_178_8, this);
        });
      },

      /**
       * Hash change listener.
       *
       * @param e {qx.event.type.Event} event instance
       */
      __onHashChange__P_178_8: function __onHashChange__P_178_8(e) {
        // the location only changes if the user manually changes the fragment
        // identifier.
        var currentState = null;

        var locationState = this._getHash();

        if (!this.__isCurrentLocationState__P_178_9(locationState)) {
          currentState = this.__storeLocationState__P_178_10(locationState);
        } else {
          currentState = this._readState();
        }

        if (qx.lang.Type.isString(currentState) && currentState != this.getState()) {
          this._onHistoryLoad(currentState);
        }
      },

      /**
       * Stores the given location state.
       *
       * @param locationState {String} location state
       * @return {String}
       */
      __storeLocationState__P_178_10: function __storeLocationState__P_178_10(locationState) {
        locationState = this._decode(locationState);

        this._writeState(locationState);

        return locationState;
      },

      /**
       * Checks whether the given location state is the current one.
       *
       * @param locationState {String} location state to check
       * @return {Boolean}
       */
      __isCurrentLocationState__P_178_9: function __isCurrentLocationState__P_178_9(locationState) {
        return qx.lang.Type.isString(locationState) && locationState == this.__locationState__P_178_5;
      },

      /**
       * Initializes the iframe
       *
       * @param handler {Function?null} if given this callback is executed after iframe is ready to use
       */
      __initIframe__P_178_7: function __initIframe__P_178_7(handler) {
        this.__iframe__P_178_1 = this.__createIframe__P_178_11();
        document.body.appendChild(this.__iframe__P_178_1);

        this.__waitForIFrame__P_178_12(function () {
          this._writeState(this.getState());

          if (handler) {
            handler.call(this);
          }
        }, this);
      },

      /**
       * IMPORTANT NOTE FOR IE:
       * Setting the source before adding the iframe to the document.
       * Otherwise IE will bring up a "Unsecure items ..." warning in SSL mode
       *
       * @return {qx.bom.Iframe}
       */
      __createIframe__P_178_11: function __createIframe__P_178_11() {
        var iframe = qx.bom.Iframe.create({
          src: qx.util.ResourceManager.getInstance().toUri("qx/static/blank.html")
        });
        iframe.style.visibility = "hidden";
        iframe.style.position = "absolute";
        iframe.style.left = "-1000px";
        iframe.style.top = "-1000px";
        return iframe;
      },

      /**
       * Waits for the IFrame being loaded. Once the IFrame is loaded
       * the callback is called with the provided context.
       *
       * @param callback {Function} This function will be called once the iframe is loaded
       * @param context {Object?window} The context for the callback.
       * @param retry {Integer} number of tries to initialize the iframe
       */
      __waitForIFrame__P_178_12: function __waitForIFrame__P_178_12(callback, context, retry) {
        if (typeof retry === "undefined") {
          retry = 0;
        }

        if (!this.__iframe__P_178_1.contentWindow || !this.__iframe__P_178_1.contentWindow.document) {
          if (retry > 20) {
            throw new Error("can't initialize iframe");
          }

          qx.event.Timer.once(function () {
            this.__waitForIFrame__P_178_12(callback, context, ++retry);
          }, this, 10);
          return;
        }

        this.__iframeReady__P_178_2 = true;
        callback.call(context || window);
      }
    },
    destruct: function destruct() {
      this.__iframe__P_178_1 = null;

      if (this.__writeStateTimner__P_178_3) {
        this.__writeStateTimner__P_178_3.dispose();

        this.__writeStateTimner__P_178_3 = null;
      }

      qx.event.Idle.getInstance().removeListener("interval", this.__onHashChange__P_178_8, this);
    }
  });
  qx.bom.IframeHistory.$$dbClassInfo = $$dbClassInfo;
})();

//# sourceMappingURL=IframeHistory.js.map?dt=1598045572480