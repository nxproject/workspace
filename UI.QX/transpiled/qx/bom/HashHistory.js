(function () {
  var $$dbClassInfo = {
    "dependsOn": {
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
      "qx.bom.Iframe": {},
      "qx.util.ResourceManager": {},
      "qx.event.Timer": {},
      "qx.event.Idle": {}
    }
  };
  qx.Bootstrap.executePendingDefers($$dbClassInfo);

  /* ************************************************************************
  
     qooxdoo - the new era of web development
  
     http://qooxdoo.org
  
     Copyright:
       2004-2012 1&1 Internet AG, Germany, http://www.1und1.de
  
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
   * History manager implementation for IE greater 7. IE reloads iframe
   * content on history actions even just hash value changed. This
   * implementation forwards history states (hashes) to a helper iframe.
   *
   * This class must be disposed of after use
   *
   * @internal
   */
  qx.Class.define("qx.bom.HashHistory", {
    extend: qx.bom.History,
    implement: [qx.core.IDisposable],
    construct: function construct() {
      qx.bom.History.constructor.call(this);
      this._baseUrl = null;

      this.__initIframe__P_177_0();
    },
    members: {
      __checkOnHashChange__P_177_1: null,
      __iframe__P_177_2: null,
      __iframeReady__P_177_3: false,
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
          this._writeState(state);
        }
      },

      /**
       * Initializes the iframe
       *
       */
      __initIframe__P_177_0: function __initIframe__P_177_0() {
        this.__iframe__P_177_2 = this.__createIframe__P_177_4();
        document.body.appendChild(this.__iframe__P_177_2);

        this.__waitForIFrame__P_177_5(function () {
          this._baseUrl = this.__iframe__P_177_2.contentWindow.document.location.href;

          this.__attachListeners__P_177_6();
        }, this);
      },

      /**
       * IMPORTANT NOTE FOR IE:
       * Setting the source before adding the iframe to the document.
       * Otherwise IE will bring up a "Unsecure items ..." warning in SSL mode
       *
       * @return {Element}
       */
      __createIframe__P_177_4: function __createIframe__P_177_4() {
        var iframe = qx.bom.Iframe.create({
          src: qx.util.ResourceManager.getInstance().toUri("qx/static/blank.html") + "#"
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
      __waitForIFrame__P_177_5: function __waitForIFrame__P_177_5(callback, context, retry) {
        if (typeof retry === "undefined") {
          retry = 0;
        }

        if (!this.__iframe__P_177_2.contentWindow || !this.__iframe__P_177_2.contentWindow.document) {
          if (retry > 20) {
            throw new Error("can't initialize iframe");
          }

          qx.event.Timer.once(function () {
            this.__waitForIFrame__P_177_5(callback, context, ++retry);
          }, this, 10);
          return;
        }

        this.__iframeReady__P_177_3 = true;
        callback.call(context || window);
      },

      /**
       * Attach hash change listeners
       */
      __attachListeners__P_177_6: function __attachListeners__P_177_6() {
        qx.event.Idle.getInstance().addListener("interval", this.__onHashChange__P_177_7, this);
      },

      /**
       * Remove hash change listeners
       */
      __detatchListeners__P_177_8: function __detatchListeners__P_177_8() {
        qx.event.Idle.getInstance().removeListener("interval", this.__onHashChange__P_177_7, this);
      },

      /**
       * hash change event handler
       */
      __onHashChange__P_177_7: function __onHashChange__P_177_7() {
        var currentState = this._readState();

        if (qx.lang.Type.isString(currentState) && currentState != this.getState()) {
          this._onHistoryLoad(currentState);
        }
      },

      /**
       * Browser dependent function to read the current state of the history
       *
       * @return {String} current state of the browser history
       */
      _readState: function _readState() {
        var hash = !this._getHash() ? "" : this._getHash().substr(1);
        return this._decode(hash);
      },

      /**
       * Returns the fragment identifier of the top window URL. For gecko browsers we
       * have to use a regular expression to avoid encoding problems.
       *
       * @return {String|null} the fragment identifier or <code>null</code> if the
       * iframe isn't ready yet
       */
      _getHash: function _getHash() {
        if (!this.__iframeReady__P_177_3) {
          return null;
        }

        return this.__iframe__P_177_2.contentWindow.document.location.hash;
      },

      /**
       * Save a state into the browser history.
       *
       * @param state {String} state to save
       */
      _writeState: function _writeState(state) {
        this._setHash(this._encode(state));
      },

      /**
       * Sets the fragment identifier of the window URL
       *
       * @param value {String} the fragment identifier
       */
      _setHash: function _setHash(value) {
        if (!this.__iframe__P_177_2 || !this._baseUrl) {
          return;
        }

        var hash = !this.__iframe__P_177_2.contentWindow.document.location.hash ? "" : this.__iframe__P_177_2.contentWindow.document.location.hash.substr(1);

        if (value != hash) {
          this.__iframe__P_177_2.contentWindow.document.location.hash = value;
        }
      }
    },
    destruct: function destruct() {
      this.__detatchListeners__P_177_8();

      this.__iframe__P_177_2 = null;
    }
  });
  qx.bom.HashHistory.$$dbClassInfo = $$dbClassInfo;
})();

//# sourceMappingURL=HashHistory.js.map?dt=1598045572401