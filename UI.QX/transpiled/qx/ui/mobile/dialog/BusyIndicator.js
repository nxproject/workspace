(function () {
  var $$dbClassInfo = {
    "dependsOn": {
      "qx.Class": {
        "usage": "dynamic",
        "require": true
      },
      "qx.ui.mobile.basic.Atom": {
        "construct": true,
        "require": true
      },
      "qx.ui.mobile.basic.Image": {
        "construct": true
      },
      "qx.bom.element.Animation": {}
    }
  };
  qx.Bootstrap.executePendingDefers($$dbClassInfo);

  /* ************************************************************************
  
     qooxdoo - the new era of web development
  
     http://qooxdoo.org
  
     Copyright:
       2004-2011 1&1 Internet AG, Germany, http://www.1und1.de
  
     License:
       MIT: https://opensource.org/licenses/MIT
       See the LICENSE file in the project's top-level directory for details.
  
     Authors:
       * Tino Butz (tbtz)
  
  ************************************************************************ */

  /**
   * The widget displays a busy indicator.
   *
   * *Example*
   *
   * Here is a little example of how to use the widget.
   *
   * <pre class='javascript'>
   *   var busyIndicator = new qx.ui.mobile.dialog.BusyIndicator("Please wait");
   *   this.getRoot().add(busyIndicator);
   * </pre>
   *
   * This example create a widget to display the busy indicator.
   */
  qx.Class.define("qx.ui.mobile.dialog.BusyIndicator", {
    extend: qx.ui.mobile.basic.Atom,

    /**
     * @param label {String} Label to use
     */
    construct: function construct(label) {
      // the image passed as second argument is a blank 1px transparent png
      qx.ui.mobile.basic.Atom.constructor.call(this, label, qx.ui.mobile.basic.Image.PLACEHOLDER_IMAGE);
      this.addListener("appear", this._onAppear, this);
      this.addListener("disappear", this._onDisappear, this);
    },
    properties: {
      /**
       * The spinner css class to use.
       */
      spinnerClass: {
        apply: "_applySpinnerClass",
        nullable: false,
        check: "String",
        init: "spinner"
      }
    },
    statics: {
      SPINNER_ANIMATION: null
    },
    members: {
      __animationHandle__P_258_0: null,

      /**
       * Listener for appear event.
       */
      _onAppear: function _onAppear() {
        this.__animationHandle__P_258_0 = qx.bom.element.Animation.animate(this.getIconWidget().getContainerElement(), qx.ui.mobile.dialog.BusyIndicator.SPINNER_ANIMATION);
      },

      /**
       * Handler for disappear event.
       */
      _onDisappear: function _onDisappear() {
        this.__animationHandle__P_258_0.stop();
      },
      // overridden
      _createIconWidget: function _createIconWidget(iconUrl) {
        var iconWidget = qx.ui.mobile.dialog.BusyIndicator.prototype._createIconWidget.base.call(this, iconUrl);

        iconWidget.addCssClass(this.getSpinnerClass());
        return iconWidget;
      },
      // property apply
      _applySpinnerClass: function _applySpinnerClass(value, old) {
        if (old) {
          this.getIconWidget().removeCssClass(old);
        }

        if (value) {
          this.getIconWidget().addCssClass(value);
        }
      }
    },
    destruct: function destruct() {
      this.removeListener("appear", this._onAppear, this);
      this.removeListener("disappear", this._onDisappear, this);

      if (this.__animationHandle__P_258_0) {
        this.__animationHandle__P_258_0.stop();
      }

      this.__animationHandle__P_258_0 = null;
    },
    defer: function defer() {
      qx.ui.mobile.dialog.BusyIndicator.SPINNER_ANIMATION = {
        duration: 750,
        timing: "linear",
        origin: "center center",
        repeat: "infinite",
        keyFrames: {
          0: {
            rotate: "0deg"
          },
          100: {
            rotate: "359deg"
          }
        }
      };
    }
  });
  qx.ui.mobile.dialog.BusyIndicator.$$dbClassInfo = $$dbClassInfo;
})();

//# sourceMappingURL=BusyIndicator.js.map?dt=1598051413168