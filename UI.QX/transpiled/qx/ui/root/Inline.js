(function () {
  var $$dbClassInfo = {
    "dependsOn": {
      "qx.event.handler.ElementResize": {
        "defer": "runtime"
      },
      "qx.core.Environment": {
        "defer": "load",
        "construct": true,
        "require": true
      },
      "qx.Class": {
        "usage": "dynamic",
        "require": true
      },
      "qx.ui.root.Abstract": {
        "construct": true,
        "require": true
      },
      "qx.ui.core.MLayoutHandling": {
        "defer": "runtime",
        "require": true
      },
      "qx.ui.layout.Basic": {
        "construct": true
      },
      "qx.ui.core.queue.Layout": {
        "construct": true
      },
      "qx.ui.core.FocusHandler": {
        "construct": true
      },
      "qx.bom.client.Engine": {
        "construct": true
      },
      "qx.dom.Node": {
        "construct": true
      },
      "qx.event.Registration": {
        "construct": true,
        "defer": "runtime"
      },
      "qx.bom.element.Dimension": {},
      "qx.bom.element.Style": {},
      "qx.html.Root": {},
      "qx.event.Timer": {}
    },
    "environment": {
      "provided": [],
      "required": {
        "engine.name": {
          "construct": true,
          "className": "qx.bom.client.Engine"
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
       * Fabian Jakobs (fjakobs)
       * Alexander Steitz (aback)
  
  ************************************************************************ */

  /**
   * This classes could be used to insert qooxdoo islands into existing
   * web pages. You can use the isles to place any qooxdoo powered widgets
   * inside a layout made using traditional HTML markup and CSS.
   *
   * The size of the widget in each dimension can either be determined by the
   * size hint of the inline's children or by the size of the root DOM element. If
   * <code>dynamicX</code>/<code>dynamicY</code> is true the width/height of the DOM
   * element is used.
   *
   * This class uses {@link qx.ui.layout.Basic} as default layout. The layout
   * can be changed using the {@link #setLayout} method.
   *
   * To position popups and tooltips please have a look at {@link qx.ui.root.Page}.
   *
   * @use(qx.event.handler.ElementResize)
   * @ignore(qx.ui.popup, qx.ui.popup.Manager.*)
   * @ignore(qx.ui.menu, qx.ui.menu.Manager.*)
   */
  qx.Class.define("qx.ui.root.Inline", {
    extend: qx.ui.root.Abstract,
    include: [qx.ui.core.MLayoutHandling],

    /*
    *****************************************************************************
       CONSTRUCTOR
    *****************************************************************************
    */

    /**
     * @param el {Element} DOM element to use as isle for qooxdoo content. Please
     *    note that existing content gets removed on the first layout flush.
     * @param dynamicX {Boolean} If <code>true</code> the widget's width is
     *    determined by the DOM element's width. Otherwise the children's size hint
     *    is used.
     * @param dynamicY {Boolean} If <code>true</code> the widget's height is
     *    determined by the DOM element's height. Otherwise the children's size hint
     *    is used.
     */
    construct: function construct(el, dynamicX, dynamicY) {
      // check the parameter
      {
        this.assertElement(el, "Please use a DOM element to create an inline root.");
      } // Temporary storage of element to use

      this.__elem__P_222_0 = el; // Avoid any problems with dynamic resizing

      el.style.overflow = "hidden"; // Avoid any problems with broken layout

      el.style.textAlign = "left";
      this.__dynX__P_222_1 = dynamicX || false;
      this.__dynY__P_222_2 = dynamicY || false;

      this.__initDynamicMode__P_222_3();

      qx.ui.root.Abstract.constructor.call(this); // Use static layout

      this._setLayout(new qx.ui.layout.Basic()); // Directly schedule layout for root element


      qx.ui.core.queue.Layout.add(this); // Register as root

      qx.ui.core.FocusHandler.getInstance().connectTo(this); // Avoid the automatically scroll in to view.
      // See http://bugzilla.qooxdoo.org/show_bug.cgi?id=3236 for details.

      if (qx.core.Environment.get("engine.name") == "mshtml") {
        this.setKeepFocus(true);
      } // Resize handling for the window


      var window = qx.dom.Node.getWindow(el);
      qx.event.Registration.addListener(window, "resize", this._onWindowResize, this); // quick fix for [BUG #7680]

      this.getContentElement().setStyle("-webkit-backface-visibility", "hidden");
    },

    /*
    *****************************************************************************
       MEMBERS
    *****************************************************************************
    */
    members: {
      __dynX__P_222_1: false,
      __dynY__P_222_2: false,
      __elem__P_222_0: null,

      /**
       * Performs several checks for dynamic mode and adds the "resize" listener
       */
      __initDynamicMode__P_222_3: function __initDynamicMode__P_222_3() {
        if (this.__dynX__P_222_1 || this.__dynY__P_222_2) {
          // Check the DOM element for an usable width and height
          var elementDimensions = qx.bom.element.Dimension.getSize(this.__elem__P_222_0);

          if (this.__dynX__P_222_1 && elementDimensions.width < 1) {
            throw new Error("The root element " + this.__elem__P_222_0 + " of " + this + " needs a width when its width size should be used!");
          }

          if (this.__dynY__P_222_2) {
            if (elementDimensions.height < 1) {
              throw new Error("The root element " + this.__elem__P_222_0 + " of " + this + " needs a height when its height size should be used!");
            } // check for implicit height. Set the height explicit to prevent that
            // the element grows indefinitely


            if (elementDimensions.height >= 1 && qx.bom.element.Style.get(this.__elem__P_222_0, "height", 3) == "") {
              qx.bom.element.Style.set(this.__elem__P_222_0, "height", elementDimensions.height + "px");
            }
          }

          qx.event.Registration.addListener(this.__elem__P_222_0, "resize", this._onResize, this);
        }
      },
      // overridden
      _createContentElement: function _createContentElement() {
        var el = this.__elem__P_222_0;

        if (this.__dynX__P_222_1 || this.__dynY__P_222_2) {
          var rootEl = document.createElement("div");
          el.appendChild(rootEl);
        } else {
          rootEl = el;
        }

        var root = new qx.html.Root(rootEl); // Make relative

        rootEl.style.position = "relative"; // Store reference to the widget in the DOM element.

        root.connectWidget(this); // fire event asynchronously, otherwise the browser will fire the event
        // too early and no listener will be informed since they're not added
        // at this time

        qx.event.Timer.once(function (e) {
          this.fireEvent("appear");
        }, this, 0);
        return root;
      },

      /**
       * Listener for the element's resize event
       *
       * @param e {qx.event.type.Event} Event object
       */
      _onResize: function _onResize(e) {
        var data = e.getData();

        if (data.oldWidth !== data.width && this.__dynX__P_222_1 || data.oldHeight !== data.height && this.__dynY__P_222_2) {
          qx.ui.core.queue.Layout.add(this);
        }
      },

      /**
       * Listener for the window's resize event.
       */
      _onWindowResize: function _onWindowResize() {
        // close all popups
        if (qx.ui.popup && qx.ui.popup.Manager) {
          qx.ui.popup.Manager.getInstance().hideAll();
        } // close all menus


        if (qx.ui.menu && qx.ui.menu.Manager) {
          qx.ui.menu.Manager.getInstance().hideAll();
        }
      },
      // overridden
      _computeSizeHint: function _computeSizeHint() {
        var dynX = this.__dynX__P_222_1;
        var dynY = this.__dynY__P_222_2;

        if (!dynX || !dynY) {
          var hint = qx.ui.root.Inline.prototype._computeSizeHint.base.call(this);
        } else {
          hint = {};
        }

        var Dimension = qx.bom.element.Dimension;

        if (dynX) {
          var width = Dimension.getContentWidth(this.__elem__P_222_0);
          hint.width = width;
          hint.minWidth = width;
          hint.maxWidth = width;
        }

        if (dynY) {
          var height = Dimension.getContentHeight(this.__elem__P_222_0);
          hint.height = height;
          hint.minHeight = height;
          hint.maxHeight = height;
        }

        return hint;
      }
    },

    /*
    *****************************************************************************
       DEFER
    *****************************************************************************
    */
    defer: function defer(statics, members) {
      qx.ui.core.MLayoutHandling.remap(members);
    },

    /*
    *****************************************************************************
       DESTRUCT
    *****************************************************************************
    */
    destruct: function destruct() {
      qx.event.Registration.removeListener(this.__elem__P_222_0, "resize", this._onResize, this);
      this.__elem__P_222_0 = null;
    }
  });
  qx.ui.root.Inline.$$dbClassInfo = $$dbClassInfo;
})();

//# sourceMappingURL=Inline.js.map?dt=1598051405801