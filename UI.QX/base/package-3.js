(function () {
  var $$dbClassInfo = {
    "dependsOn": {
      "qx.Class": {
        "usage": "dynamic",
        "require": true
      },
      "qx.core.Object": {
        "require": true
      },
      "qx.theme.manager.Color": {},
      "qx.theme.manager.Decoration": {},
      "qx.theme.manager.Font": {},
      "qx.theme.manager.Icon": {},
      "qx.theme.manager.Appearance": {},
      "qx.core.Environment": {},
      "qx.Theme": {}
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
  
  ************************************************************************ */

  /**
   * Manager for meta themes
   */
  qx.Class.define("qx.theme.manager.Meta", {
    type: "singleton",
    extend: qx.core.Object,
    events: {
      /** Fires if any theme manager has been changed. */
      "changeTheme": "qx.event.type.Event"
    },
    properties: {
      /**
       * Meta theme. Applies the defined color, decoration, ... themes to
       * the corresponding managers.
       */
      theme: {
        check: "Theme",
        nullable: false,
        apply: "_applyTheme"
      }
    },
    members: {
      // property apply
      _applyTheme: function _applyTheme(value, old) {
        // collect changes
        var colorChanged = true;
        var decorationChanged = true;
        var fontChanged = true;
        var iconChanged = true;
        var appearanceChanged = true;

        if (old) {
          colorChanged = value.meta.color !== old.meta.color;
          decorationChanged = value.meta.decoration !== old.meta.decoration;
          fontChanged = value.meta.font !== old.meta.font;
          iconChanged = value.meta.icon !== old.meta.icon;
          appearanceChanged = value.meta.appearance !== old.meta.appearance;
        }

        var colorMgr = qx.theme.manager.Color.getInstance();
        var decorationMgr = qx.theme.manager.Decoration.getInstance();
        var fontMgr = qx.theme.manager.Font.getInstance();
        var iconMgr = qx.theme.manager.Icon.getInstance();
        var appearanceMgr = qx.theme.manager.Appearance.getInstance(); // suspend listeners

        this._suspendEvents(); // apply meta changes


        if (colorChanged) {
          // color theme changed, but decorator not? force decorator
          if (!decorationChanged) {
            var dec = decorationMgr.getTheme();

            decorationMgr._applyTheme(dec);
          }

          colorMgr.setTheme(value.meta.color);
        }

        decorationMgr.setTheme(value.meta.decoration);
        fontMgr.setTheme(value.meta.font);
        iconMgr.setTheme(value.meta.icon);
        appearanceMgr.setTheme(value.meta.appearance); // fire change event only if at least one theme manager changed

        if (colorChanged || decorationChanged || fontChanged || iconChanged || appearanceChanged) {
          this.fireEvent("changeTheme");
        } // re add listener


        this._activateEvents();
      },
      __timer__P_33_0: null,

      /**
       * Fires <code>changeTheme</code> event.
       *
       * @param e {qx.event.type.Data} Data event.
       */
      _fireEvent: function _fireEvent(e) {
        if (e.getTarget() === qx.theme.manager.Color.getInstance()) {
          // force clearing all previously created CSS rules, to be able to
          // re-create decorator rules with changed color theme
          qx.theme.manager.Decoration.getInstance().refresh();
        }

        this.fireEvent("changeTheme");
      },

      /**
       * Removes listeners for <code>changeTheme</code> event of all
       * related theme managers.
       */
      _suspendEvents: function _suspendEvents() {
        var colorMgr = qx.theme.manager.Color.getInstance();
        var decorationMgr = qx.theme.manager.Decoration.getInstance();
        var fontMgr = qx.theme.manager.Font.getInstance();
        var iconMgr = qx.theme.manager.Icon.getInstance();
        var appearanceMgr = qx.theme.manager.Appearance.getInstance(); // suspend listeners

        if (colorMgr.hasListener("changeTheme")) {
          colorMgr.removeListener("changeTheme", this._fireEvent, this);
        }

        if (decorationMgr.hasListener("changeTheme")) {
          decorationMgr.removeListener("changeTheme", this._fireEvent, this);
        }

        if (fontMgr.hasListener("changeTheme")) {
          fontMgr.removeListener("changeTheme", this._fireEvent, this);
        }

        if (iconMgr.hasListener("changeTheme")) {
          iconMgr.removeListener("changeTheme", this._fireEvent, this);
        }

        if (appearanceMgr.hasListener("changeTheme")) {
          appearanceMgr.removeListener("changeTheme", this._fireEvent, this);
        }
      },

      /**
       * Activates listeners for <code>changeTheme</code> event of all related
       * theme managers, to forwards the event to this meta manager instance.
       */
      _activateEvents: function _activateEvents() {
        var colorMgr = qx.theme.manager.Color.getInstance();
        var decorationMgr = qx.theme.manager.Decoration.getInstance();
        var fontMgr = qx.theme.manager.Font.getInstance();
        var iconMgr = qx.theme.manager.Icon.getInstance();
        var appearanceMgr = qx.theme.manager.Appearance.getInstance(); // add listeners to check changes

        if (!colorMgr.hasListener("changeTheme")) {
          colorMgr.addListener("changeTheme", this._fireEvent, this);
        }

        if (!decorationMgr.hasListener("changeTheme")) {
          decorationMgr.addListener("changeTheme", this._fireEvent, this);
        }

        if (!fontMgr.hasListener("changeTheme")) {
          fontMgr.addListener("changeTheme", this._fireEvent, this);
        }

        if (!iconMgr.hasListener("changeTheme")) {
          iconMgr.addListener("changeTheme", this._fireEvent, this);
        }

        if (!appearanceMgr.hasListener("changeTheme")) {
          appearanceMgr.addListener("changeTheme", this._fireEvent, this);
        }
      },

      /**
       * Initialize the themes which were selected using the settings. Should only
       * be called from qooxdoo based application.
       */
      initialize: function initialize() {
        var env = qx.core.Environment;
        var theme, obj;
        theme = env.get("qx.theme");

        if (theme) {
          obj = qx.Theme.getByName(theme);

          if (!obj) {
            throw new Error("The theme to use is not available: " + theme);
          }

          this.setTheme(obj);
        }
      }
    },

    /*
    *****************************************************************************
       ENVIRONMENT SETTINGS
    *****************************************************************************
    */
    environment: {
      "qx.theme": "qx.theme.nx"
    }
  });
  qx.theme.manager.Meta.$$dbClassInfo = $$dbClassInfo;
})();

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
      "qx.event.Registration": {
        "construct": true
      },
      "qx.event.Timer": {
        "construct": true
      },
      "qx.ui.tooltip.ToolTip": {},
      "qx.ui.core.Widget": {},
      "qx.ui.form.IForm": {}
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
       * Adrian Olaru (adrianolaru)
  
  ************************************************************************ */

  /**
   * The tooltip manager globally manages the tooltips of all widgets. It will
   * display tooltips if the user hovers a widgets with a tooltip and hides all
   * other tooltips.
   */
  qx.Class.define("qx.ui.tooltip.Manager", {
    type: "singleton",
    extend: qx.core.Object,

    /*
    *****************************************************************************
       CONSTRUCTOR
    *****************************************************************************
    */
    construct: function construct() {
      qx.core.Object.constructor.call(this); // Register events

      qx.event.Registration.addListener(document.body, "pointerover", this.__onPointerOverRoot__P_34_0, this, true); // Instantiate timers

      this.__showTimer__P_34_1 = new qx.event.Timer();

      this.__showTimer__P_34_1.addListener("interval", this.__onShowInterval__P_34_2, this);

      this.__hideTimer__P_34_3 = new qx.event.Timer();

      this.__hideTimer__P_34_3.addListener("interval", this.__onHideInterval__P_34_4, this); // Init pointer position


      this.__pointerPosition__P_34_5 = {
        left: 0,
        top: 0
      };
    },

    /*
    *****************************************************************************
       PROPERTIES
    *****************************************************************************
    */
    properties: {
      /** Holds the current ToolTip instance */
      current: {
        check: "qx.ui.tooltip.ToolTip",
        nullable: true,
        apply: "_applyCurrent"
      },

      /** Show all invalid form fields tooltips . */
      showInvalidToolTips: {
        check: "Boolean",
        init: true
      },

      /** Show all tooltips. */
      showToolTips: {
        check: "Boolean",
        init: true
      }
    },

    /*
    *****************************************************************************
       MEMBERS
    *****************************************************************************
    */
    members: {
      __pointerPosition__P_34_5: null,
      __hideTimer__P_34_3: null,
      __showTimer__P_34_1: null,
      __sharedToolTip__P_34_6: null,
      __sharedErrorToolTip__P_34_7: null,

      /**
       * Get the shared tooltip, which is used to display the
       * {@link qx.ui.core.Widget#toolTipText} and
       * {@link qx.ui.core.Widget#toolTipIcon} properties of widgets.
       * You can use this public shared instance to e.g. customize the
       * look and feel.
       *
       * @return {qx.ui.tooltip.ToolTip} The shared tooltip
       */
      getSharedTooltip: function getSharedTooltip() {
        if (!this.__sharedToolTip__P_34_6) {
          this.__sharedToolTip__P_34_6 = new qx.ui.tooltip.ToolTip().set({
            rich: true
          });
        }

        return this.__sharedToolTip__P_34_6;
      },

      /**
       * Get the shared tooltip, which is used to display the
       * {@link qx.ui.core.Widget#toolTipText} and
       * {@link qx.ui.core.Widget#toolTipIcon} properties of widgets.
       * You can use this public shared instance to e.g. customize the
       * look and feel of the validation tooltips like
       * <code>getSharedErrorTooltip().getChildControl("atom").getChildControl("label").set({rich: true, wrap: true, width: 80})</code>
       *
       * @return {qx.ui.tooltip.ToolTip} The shared tooltip
       */
      getSharedErrorTooltip: function getSharedErrorTooltip() {
        if (!this.__sharedErrorToolTip__P_34_7) {
          this.__sharedErrorToolTip__P_34_7 = new qx.ui.tooltip.ToolTip().set({
            appearance: "tooltip-error",
            rich: true
          });

          this.__sharedErrorToolTip__P_34_7.setLabel(""); // trigger label widget creation


          this.__sharedErrorToolTip__P_34_7.syncAppearance();
        }

        return this.__sharedErrorToolTip__P_34_7;
      },

      /*
      ---------------------------------------------------------------------------
        PROPERTY APPLY ROUTINES
      ---------------------------------------------------------------------------
      */
      // property apply
      _applyCurrent: function _applyCurrent(value, old) {
        // Return if the new tooltip is a child of the old one
        if (old && qx.ui.core.Widget.contains(old, value)) {
          return;
        } // If old tooltip existing, hide it and clear widget binding


        if (old) {
          if (!old.isDisposed()) {
            old.exclude();
          }

          this.__showTimer__P_34_1.stop();

          this.__hideTimer__P_34_3.stop();
        }

        var Registration = qx.event.Registration;
        var el = document.body; // If new tooltip is not null, set it up and start the timer

        if (value) {
          this.__showTimer__P_34_1.startWith(value.getShowTimeout()); // Register hide handler


          Registration.addListener(el, "pointerout", this.__onPointerOutRoot__P_34_8, this, true);
          Registration.addListener(el, "focusout", this.__onFocusOutRoot__P_34_9, this, true);
          Registration.addListener(el, "pointermove", this.__onPointerMoveRoot__P_34_10, this, true);
        } else {
          // Deregister hide handler
          Registration.removeListener(el, "pointerout", this.__onPointerOutRoot__P_34_8, this, true);
          Registration.removeListener(el, "focusout", this.__onFocusOutRoot__P_34_9, this, true);
          Registration.removeListener(el, "pointermove", this.__onPointerMoveRoot__P_34_10, this, true);
        }
      },

      /*
      ---------------------------------------------------------------------------
        TIMER EVENT HANDLER
      ---------------------------------------------------------------------------
      */

      /**
       * Event listener for the interval event of the show timer.
       *
       * @param e {qx.event.type.Event} Event object
       */
      __onShowInterval__P_34_2: function __onShowInterval__P_34_2(e) {
        var current = this.getCurrent();

        if (current && !current.isDisposed()) {
          this.__hideTimer__P_34_3.startWith(current.getHideTimeout());

          if (current.getPlaceMethod() == "widget") {
            current.placeToWidget(current.getOpener());
          } else {
            current.placeToPoint(this.__pointerPosition__P_34_5);
          }

          current.show();
        }

        this.__showTimer__P_34_1.stop();
      },

      /**
       * Event listener for the interval event of the hide timer.
       *
       * @param e {qx.event.type.Event} Event object
       */
      __onHideInterval__P_34_4: function __onHideInterval__P_34_4(e) {
        var current = this.getCurrent();

        if (current && !current.getAutoHide()) {
          return;
        }

        if (current && !current.isDisposed()) {
          current.exclude();
        }

        this.__hideTimer__P_34_3.stop();

        this.resetCurrent();
      },

      /*
      ---------------------------------------------------------------------------
        POINTER EVENT HANDLER
      ---------------------------------------------------------------------------
      */

      /**
       * Global pointer move event handler
       *
       * @param e {qx.event.type.Pointer} The move pointer event
       */
      __onPointerMoveRoot__P_34_10: function __onPointerMoveRoot__P_34_10(e) {
        var pos = this.__pointerPosition__P_34_5;
        pos.left = Math.round(e.getDocumentLeft());
        pos.top = Math.round(e.getDocumentTop());
      },

      /**
       * Searches for the tooltip of the target widget. If any tooltip instance
       * is found this instance is bound to the target widget and the tooltip is
       * set as {@link #current}
       *
       * @param e {qx.event.type.Pointer} pointerover event
       */
      __onPointerOverRoot__P_34_0: function __onPointerOverRoot__P_34_0(e) {
        var target = qx.ui.core.Widget.getWidgetByElement(e.getTarget()); // take first coordinates as backup if no move event will be fired (e.g. touch devices)

        this.__onPointerMoveRoot__P_34_10(e);

        this.showToolTip(target);
      },

      /**
       * Explicitly show tooltip for particular form item.
       *
       * @param target {Object | null} widget to show tooltip for
       */
      showToolTip: function showToolTip(target) {
        if (!target) {
          return;
        }

        var tooltip, tooltipText, tooltipIcon, invalidMessage; // Search first parent which has a tooltip

        while (target != null) {
          tooltip = target.getToolTip();
          tooltipText = target.getToolTipText() || null;
          tooltipIcon = target.getToolTipIcon() || null;

          if (qx.Class.hasInterface(target.constructor, qx.ui.form.IForm) && !target.isValid()) {
            invalidMessage = target.getInvalidMessage();
          }

          if (tooltip || tooltipText || tooltipIcon || invalidMessage) {
            break;
          }

          target = target.getLayoutParent();
        } //do nothing if


        if (!target //don't have a target
        // tooltip is disabled and the value of showToolTipWhenDisabled is false
        || !target.getEnabled() && !target.isShowToolTipWhenDisabled() //tooltip is blocked
        || target.isBlockToolTip() //an invalid message isn't set and tooltips are disabled
        || !invalidMessage && !this.getShowToolTips() //an invalid message is set and invalid tooltips are disabled
        || invalidMessage && !this.getShowInvalidToolTips()) {
          return;
        }

        if (invalidMessage) {
          tooltip = this.getSharedErrorTooltip().set({
            label: invalidMessage
          });
        }

        if (!tooltip) {
          tooltip = this.getSharedTooltip().set({
            label: tooltipText,
            icon: tooltipIcon
          });
        }

        this.setCurrent(tooltip);
        tooltip.setOpener(target);
      },

      /**
       * Resets the property {@link #current} if there was a
       * tooltip and no new one is created.
       *
       * @param e {qx.event.type.Pointer} pointerout event
       */
      __onPointerOutRoot__P_34_8: function __onPointerOutRoot__P_34_8(e) {
        var target = qx.ui.core.Widget.getWidgetByElement(e.getTarget());

        if (!target) {
          return;
        }

        var related = qx.ui.core.Widget.getWidgetByElement(e.getRelatedTarget());

        if (!related && e.getPointerType() == "mouse") {
          return;
        }

        var tooltip = this.getCurrent(); // If there was a tooltip and
        // - the destination target is the current tooltip
        //   or
        // - the current tooltip contains the destination target

        if (tooltip && (related == tooltip || qx.ui.core.Widget.contains(tooltip, related))) {
          return;
        } // If the destination target exists and the target contains it


        if (related && target && qx.ui.core.Widget.contains(target, related)) {
          return;
        }

        if (tooltip && !tooltip.getAutoHide()) {
          return;
        } // If there was a tooltip and there is no new one


        if (tooltip && !related) {
          this.setCurrent(null);
        } else {
          this.resetCurrent();
        }
      },

      /*
      ---------------------------------------------------------------------------
        FOCUS EVENT HANDLER
      ---------------------------------------------------------------------------
      */

      /**
       * Reset the property {@link #current} if the
       * current tooltip is the tooltip of the target widget.
       *
       * @param e {qx.event.type.Focus} blur event
       */
      __onFocusOutRoot__P_34_9: function __onFocusOutRoot__P_34_9(e) {
        var target = qx.ui.core.Widget.getWidgetByElement(e.getTarget());

        if (!target) {
          return;
        }

        var tooltip = this.getCurrent();

        if (tooltip && !tooltip.getAutoHide()) {
          return;
        } // Only set to null if blurred widget is the
        // one which has created the current tooltip


        if (tooltip && tooltip == target.getToolTip()) {
          this.setCurrent(null);
        }
      }
    },

    /*
    *****************************************************************************
       DESTRUCTOR
    *****************************************************************************
    */
    destruct: function destruct() {
      // Deregister events
      qx.event.Registration.removeListener(document.body, "pointerover", this.__onPointerOverRoot__P_34_0, this, true); // Dispose timers

      this._disposeObjects("__showTimer__P_34_1", "__hideTimer__P_34_3", "__sharedToolTip__P_34_6");

      this.__pointerPosition__P_34_5 = null;
    }
  });
  qx.ui.tooltip.Manager.$$dbClassInfo = $$dbClassInfo;
})();

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
      "qx.bom.Stylesheet": {
        "construct": true
      }
    }
  };
  qx.Bootstrap.executePendingDefers($$dbClassInfo);

  /* ************************************************************************
  
     qooxdoo - the new era of web development
  
     http://qooxdoo.org
  
     Copyright:
       2013 1&1 Internet AG, Germany, http://www.1und1.de
  
     License:
       MIT: https://opensource.org/licenses/MIT
       See the LICENSE file in the project's top-level directory for details.
  
     Authors:
       * Martin Wittemann (wittemann)
       * Daniel Wagner (danielwagner)
  
  ************************************************************************ */

  /**
   * Global class which handles the single stylesheet used for qx.desktop.
   */
  qx.Class.define("qx.ui.style.Stylesheet", {
    type: "singleton",
    extend: qx.core.Object,
    construct: function construct() {
      qx.core.Object.constructor.call(this);
      this.__sheet__P_35_0 = qx.bom.Stylesheet.createElement();
      this.__rules__P_35_1 = [];
    },
    members: {
      __rules__P_35_1: null,
      __sheet__P_35_0: null,

      /**
       * Adds a rule to the global stylesheet.
       * @param selector {String} The CSS selector to add the rule for.
       * @param css {String} The rule's content.
       */
      addRule: function addRule(selector, css) {
        if (this.hasRule(selector)) {
          return;
        }

        qx.bom.Stylesheet.addRule(this.__sheet__P_35_0, selector, css);

        this.__rules__P_35_1.push(selector);
      },

      /**
       * Check if a rule exists.
       * @param selector {String} The selector to check.
       * @return {Boolean} <code>true</code> if the rule exists
       */
      hasRule: function hasRule(selector) {
        return this.__rules__P_35_1.indexOf(selector) != -1;
      },

      /**
       * Remove the rule for the given selector.
       * @param selector {String} The selector to identify the rule.
       */
      removeRule: function removeRule(selector) {
        delete this.__rules__P_35_1[this.__rules__P_35_1.indexOf(selector)];
        qx.bom.Stylesheet.removeRule(this.__sheet__P_35_0, selector);
      }
    }
  });
  qx.ui.style.Stylesheet.$$dbClassInfo = $$dbClassInfo;
})();

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
      "qx.event.Registration": {
        "defer": "runtime",
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
       * Sebastian Werner (wpbasti)
       * Fabian Jakobs (fjakobs)
  
  ************************************************************************ */

  /**
   * This handler accepts the useraction event fired by the keyboard, mouse and
   * pointer handlers after an user triggered action has occurred.
   */
  qx.Class.define("qx.event.handler.UserAction", {
    extend: qx.core.Object,
    implement: qx.event.IEventHandler,

    /*
    *****************************************************************************
       CONSTRUCTOR
    *****************************************************************************
    */

    /**
     * Create a new instance
     *
     * @param manager {qx.event.Manager} Event manager for the window to use
     */
    construct: function construct(manager) {
      qx.core.Object.constructor.call(this); // Define shorthands

      this.__manager__P_47_0 = manager;
      this.__window__P_47_1 = manager.getWindow();
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
        useraction: 1
      },

      /** @type {Integer} Which target check to use */
      TARGET_CHECK: qx.event.IEventHandler.TARGET_WINDOW,

      /** @type {Integer} Whether the method "canHandleEvent" must be called */
      IGNORE_CAN_HANDLE: true
    },

    /*
    *****************************************************************************
       MEMBERS
    *****************************************************************************
    */
    members: {
      __manager__P_47_0: null,
      __window__P_47_1: null,

      /*
      ---------------------------------------------------------------------------
        EVENT HANDLER INTERFACE
      ---------------------------------------------------------------------------
      */
      // interface implementation
      canHandleEvent: function canHandleEvent(target, type) {},
      // interface implementation
      registerEvent: function registerEvent(target, type, capture) {// Nothing needs to be done here
      },
      // interface implementation
      unregisterEvent: function unregisterEvent(target, type, capture) {// Nothing needs to be done here
      }
    },

    /*
    *****************************************************************************
       DESTRUCTOR
    *****************************************************************************
    */
    destruct: function destruct() {
      this.__manager__P_47_0 = this.__window__P_47_1 = null;
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
  qx.event.handler.UserAction.$$dbClassInfo = $$dbClassInfo;
})();

(function () {
  var $$dbClassInfo = {
    "dependsOn": {
      "qx.Bootstrap": {
        "usage": "dynamic",
        "require": true
      },
      "qx.bom.client.Engine": {},
      "qx.bom.Event": {},
      "qx.core.Environment": {
        "defer": "runtime"
      }
    },
    "environment": {
      "provided": ["event.touch", "event.mouseevent", "event.mousecreateevent", "event.dispatchevent", "event.customevent", "event.mspointer", "event.help", "event.hashchange", "event.mousewheel", "event.auxclick", "event.passive"],
      "required": {}
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
      * Martin Wittemann (martinwittemann)
  ************************************************************************ */

  /**
   * Internal class which contains the checks used by {@link qx.core.Environment}.
   * All checks in here are marked as internal which means you should never use
   * them directly.
   *
   * This class should contain all checks about events.
   *
   * @internal
   */
  qx.Bootstrap.define("qx.bom.client.Event", {
    statics: {
      /**
       * Checks if touch events are supported.
       *
       * @internal
       * @return {Boolean} <code>true</code> if touch events are supported.
       */
      getTouch: function getTouch() {
        return "ontouchstart" in window;
      },

      /**
       * Checks if MSPointer events are available.
       *
       * @internal
       * @return {Boolean} <code>true</code> if pointer events are supported.
       */
      getMsPointer: function getMsPointer() {
        // Fixes issue #9182: new unified pointer input model since Chrome 55
        // see https://github.com/qooxdoo/qooxdoo/issues/9182
        if ("PointerEvent" in window) {
          return true;
        }

        if ("pointerEnabled" in window.navigator) {
          return window.navigator.pointerEnabled;
        } else if ("msPointerEnabled" in window.navigator) {
          return window.navigator.msPointerEnabled;
        }

        return false;
      },

      /**
       * Checks if the proprietary <code>help</code> event is available.
       *
       * @internal
       * @return {Boolean} <code>true</code> if the "help" event is supported.
       */
      getHelp: function getHelp() {
        return "onhelp" in document;
      },

      /**
       * Checks if the <code>hashchange</code> event is available
       *
       * @internal
       * @return {Boolean} <code>true</code> if the "hashchange" event is supported.
       */
      getHashChange: function getHashChange() {
        // avoid false positive in IE7
        var engine = qx.bom.client.Engine.getName();
        var hashchange = ("onhashchange" in window);
        return engine !== "mshtml" && hashchange || engine === "mshtml" && "documentMode" in document && document.documentMode >= 8 && hashchange;
      },

      /**
       * Checks if the DOM2 dispatchEvent method is available
       * @return {Boolean} <code>true</code> if dispatchEvent is supported.
       */
      getDispatchEvent: function getDispatchEvent() {
        return typeof document.dispatchEvent == "function";
      },

      /**
       * Checks if the CustomEvent constructor is available and supports
       * custom event types.
       *
       * @return {Boolean} <code>true</code> if Custom Events are available
       */
      getCustomEvent: function getCustomEvent() {
        if (!window.CustomEvent) {
          return false;
        }

        try {
          new window.CustomEvent("foo");
          return true;
        } catch (ex) {
          return false;
        }
      },

      /**
       * Checks if the MouseEvent constructor is available and supports
       * custom event types.
       *
       * @return {Boolean} <code>true</code> if Mouse Events are available
       */
      getMouseEvent: function getMouseEvent() {
        if (!window.MouseEvent) {
          return false;
        }

        try {
          new window.MouseEvent("foo");
          return true;
        } catch (ex) {
          return false;
        }
      },

      /**
       * Returns the event type used in pointer layer to create mouse events.
       *
       * @return {String} Either <code>MouseEvents</code> or <code>UIEvents</code>
       */
      getMouseCreateEvent: function getMouseCreateEvent() {
        /* For instance, in IE9, the pageX property of synthetic MouseEvents is
        always 0 and cannot be overridden, so plain UIEvents have to be used with
        mouse event properties added accordingly. */
        try {
          var e = document.createEvent("MouseEvents");
          var orig = e.pageX;
          e.initMouseEvent("click", false, false, window, 0, 0, 0, orig + 1, 0, false, false, false, false, 0, null);

          if (e.pageX !== orig) {
            return "MouseEvents";
          }

          return "UIEvents";
        } catch (ex) {
          return "UIEvents";
        }
      },

      /**
       * Checks if the MouseWheel event is available and on which target.
       *
       * @param win {Window ? null} An optional window instance to check.
       * @return {Map} A map containing two values: type and target.
       */
      getMouseWheel: function getMouseWheel(win) {
        if (!win) {
          win = window;
        } // Fix for bug #3234


        var targets = [win, win.document, win.document.body];
        var target = win;
        var type = "DOMMouseScroll"; // for FF < 17

        for (var i = 0; i < targets.length; i++) {
          // check for the spec event (DOM-L3)
          if (qx.bom.Event.supportsEvent(targets[i], "wheel")) {
            type = "wheel";
            target = targets[i];
            break;
          } // check for the non spec event


          if (qx.bom.Event.supportsEvent(targets[i], "mousewheel")) {
            type = "mousewheel";
            target = targets[i];
            break;
          }
        }

        ;
        return {
          type: type,
          target: target
        };
      },

      /**
       * Detects if the engine/browser supports auxclick events
       * 
       * See https://github.com/qooxdoo/qooxdoo/issues/9268 
       *
       * @return {Boolean} <code>true</code> if auxclick events are supported.
       */
      getAuxclickEvent: function getAuxclickEvent() {
        var hasAuxclick = false;

        try {
          hasAuxclick = "onauxclick" in document.documentElement;
        } catch (ex) {}

        ;
        return hasAuxclick ? true : false;
      },

      /**
       * Checks whether the browser supports passive event handlers.
       */
      getPassive: function getPassive() {
        var passiveSupported = false;

        try {
          var options = Object.defineProperties({}, {
            passive: {
              get: function get() {
                // this function will be called when the browser
                // attempts to access the passive property.
                passiveSupported = true;
              }
            }
          });
          window.addEventListener("test", options, options);
          window.removeEventListener("test", options, options);
        } catch (err) {
          passiveSupported = false;
        }

        return passiveSupported;
      }
    },
    defer: function defer(statics) {
      qx.core.Environment.add("event.touch", statics.getTouch);
      qx.core.Environment.add("event.mouseevent", statics.getMouseEvent);
      qx.core.Environment.add("event.mousecreateevent", statics.getMouseCreateEvent);
      qx.core.Environment.add("event.dispatchevent", statics.getDispatchEvent);
      qx.core.Environment.add("event.customevent", statics.getCustomEvent);
      qx.core.Environment.add("event.mspointer", statics.getMsPointer);
      qx.core.Environment.add("event.help", statics.getHelp);
      qx.core.Environment.add("event.hashchange", statics.getHashChange);
      qx.core.Environment.add("event.mousewheel", statics.getMouseWheel);
      qx.core.Environment.add("event.auxclick", statics.getAuxclickEvent);
      qx.core.Environment.add("event.passive", statics.getPassive);
    }
  });
  qx.bom.client.Event.$$dbClassInfo = $$dbClassInfo;
})();

(function () {
  var $$dbClassInfo = {
    "dependsOn": {
      "qx.event.handler.UserAction": {
        "require": true,
        "defer": "runtime"
      },
      "qx.core.Environment": {
        "defer": "load",
        "require": true
      },
      "qx.Class": {
        "usage": "dynamic",
        "require": true
      },
      "qx.bom.AnimationFrame": {},
      "qx.ui.core.queue.Widget": {},
      "qx.log.Logger": {},
      "qx.ui.core.queue.Visibility": {},
      "qx.ui.core.queue.Appearance": {},
      "qx.ui.core.queue.Layout": {},
      "qx.html.Element": {
        "defer": "runtime"
      },
      "qx.ui.core.queue.Dispose": {},
      "qx.event.Registration": {
        "defer": "runtime"
      },
      "qx.bom.client.Event": {
        "defer": "runtime"
      }
    },
    "environment": {
      "provided": [],
      "required": {
        "event.touch": {
          "defer": true,
          "className": "qx.bom.client.Event"
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
  
  ************************************************************************ */

  /**
   * This class performs the auto flush of all layout relevant queues.
   *
   * @require(qx.event.handler.UserAction)
   */
  qx.Class.define("qx.ui.core.queue.Manager", {
    statics: {
      /** @type {Boolean} Whether a flush was scheduled */
      __scheduled__P_36_0: false,

      /** @type {Boolean} true, if the flush should not be executed */
      __canceled__P_36_1: false,

      /** @type {Map} Internal data structure for the current job list */
      __jobs__P_36_2: {},

      /** @type {Integer} Counts how often a flush failed due to exceptions */
      __retries__P_36_3: 0,

      /** @type {Integer} Maximum number of flush retries */
      MAX_RETRIES: 10,

      /**
       * Schedule a deferred flush of all queues.
       *
       * @param job {String} The job, which should be performed. Valid values are
       *     <code>layout</code>, <code>decoration</code> and <code>element</code>.
       */
      scheduleFlush: function scheduleFlush(job) {
        // Sometimes not executed in context, fix this
        var self = qx.ui.core.queue.Manager;
        self.__jobs__P_36_2[job] = true;

        if (!self.__scheduled__P_36_0) {
          self.__canceled__P_36_1 = false;
          qx.bom.AnimationFrame.request(function () {
            if (self.__canceled__P_36_1) {
              self.__canceled__P_36_1 = false;
              return;
            }

            self.flush();
          }, self);
          self.__scheduled__P_36_0 = true;
        }
      },

      /**
       * Flush all layout queues in the correct order. This function is called
       * deferred if {@link #scheduleFlush} is called.
       *
       */
      flush: function flush() {
        // Sometimes not executed in context, fix this
        var self = qx.ui.core.queue.Manager; // Stop when already executed

        if (self.__inFlush__P_36_4) {
          return;
        }

        self.__inFlush__P_36_4 = true; // Cancel timeout if called manually

        self.__canceled__P_36_1 = true;
        var jobs = self.__jobs__P_36_2;

        self.__executeAndRescheduleOnError__P_36_5(function () {
          // Process jobs
          while (jobs.visibility || jobs.widget || jobs.appearance || jobs.layout || jobs.element) {
            // No else blocks here because each flush can influence the following flushes!
            if (jobs.widget) {
              delete jobs.widget;
              {
                try {
                  qx.ui.core.queue.Widget.flush();
                } catch (e) {
                  qx.log.Logger.error(qx.ui.core.queue.Widget, "Error in the 'Widget' queue:" + e, e);
                }
              }
            }

            if (jobs.visibility) {
              delete jobs.visibility;
              {
                try {
                  qx.ui.core.queue.Visibility.flush();
                } catch (e) {
                  qx.log.Logger.error(qx.ui.core.queue.Visibility, "Error in the 'Visibility' queue:" + e, e);
                }
              }
            }

            if (jobs.appearance) {
              delete jobs.appearance;
              {
                try {
                  qx.ui.core.queue.Appearance.flush();
                } catch (e) {
                  qx.log.Logger.error(qx.ui.core.queue.Appearance, "Error in the 'Appearance' queue:" + e, e);
                }
              }
            } // Defer layout as long as possible


            if (jobs.widget || jobs.visibility || jobs.appearance) {
              continue;
            }

            if (jobs.layout) {
              delete jobs.layout;
              {
                try {
                  qx.ui.core.queue.Layout.flush();
                } catch (e) {
                  qx.log.Logger.error(qx.ui.core.queue.Layout, "Error in the 'Layout' queue:" + e, e);
                }
              }
            } // Defer element as long as possible


            if (jobs.widget || jobs.visibility || jobs.appearance || jobs.layout) {
              continue;
            }

            if (jobs.element) {
              delete jobs.element;
              qx.html.Element.flush();
            }
          }
        }, function () {
          self.__scheduled__P_36_0 = false;
        });

        self.__executeAndRescheduleOnError__P_36_5(function () {
          if (jobs.dispose) {
            delete jobs.dispose;
            {
              try {
                qx.ui.core.queue.Dispose.flush();
              } catch (e) {
                qx.log.Logger.error("Error in the 'Dispose' queue:" + e);
              }
            }
          }
        }, function () {
          // Clear flag
          self.__inFlush__P_36_4 = false;
        }); // flush succeeded successfully. Reset retries


        self.__retries__P_36_3 = 0;
      },

      /**
       * Executes the callback code. If the callback throws an error the current
       * flush is cleaned up and rescheduled. The finally code is called after the
       * callback even if it has thrown an exception.
       *
       * @signature function(callback, finallyCode)
       * @param callback {Function} the callback function
       * @param finallyCode {Function} function to be called in the finally block
       */
      __executeAndRescheduleOnError__P_36_5: function __executeAndRescheduleOnError__P_36_5(callback, finallyCode) {
        callback();
        finallyCode();
      },

      /**
       * Handler used on touch devices to prevent the queue from manipulating
       * the dom during the touch - mouse - ... event sequence. Usually, iOS
       * devices fire a click event 300ms after the touchend event. So using
       * 500ms should be a good value to be on the save side. This is necessary
       * due to the fact that the event chain is stopped if a manipulation in
       * the DOM is done.
       *
       * @param e {qx.event.type.Data} The user action data event.
       */
      __onUserAction__P_36_6: function __onUserAction__P_36_6(e) {
        qx.ui.core.queue.Manager.flush();
      }
    },

    /*
    *****************************************************************************
       DESTRUCT
    *****************************************************************************
    */
    defer: function defer(statics) {
      // Replace default scheduler for HTML element with local one.
      // This is quite a hack, but allows us to force other flushes
      // before the HTML element flush.
      qx.html.Element._scheduleFlush = statics.scheduleFlush; // Register to user action

      qx.event.Registration.addListener(window, "useraction", qx.core.Environment.get("event.touch") ? statics.__onUserAction__P_36_6 : statics.flush);
    }
  });
  qx.ui.core.queue.Manager.$$dbClassInfo = $$dbClassInfo;
})();

(function () {
  var $$dbClassInfo = {
    "dependsOn": {
      "qx.Bootstrap": {
        "usage": "dynamic",
        "require": true
      },
      "qx.dom.Node": {},
      "qx.bom.element.Dimension": {},
      "qx.bom.Document": {},
      "qx.bom.Viewport": {},
      "qx.bom.Stylesheet": {},
      "qxWeb": {
        "defer": "runtime"
      },
      "qx.bom.element.Location": {},
      "qx.lang.String": {},
      "qx.bom.element.Style": {},
      "qx.bom.element.Class": {}
    }
  };
  qx.Bootstrap.executePendingDefers($$dbClassInfo);

  /* ************************************************************************
  
     qooxdoo - the new era of web development
  
     http://qooxdoo.org
  
     Copyright:
       2011-2012 1&1 Internet AG, Germany, http://www.1und1.de
  
     License:
       MIT: https://opensource.org/licenses/MIT
       See the LICENSE file in the project's top-level directory for details.
  
     Authors:
       * Martin Wittemann (wittemann)
       * Daniel Wagner (danielwagner)
  
  ************************************************************************ */

  /**
   * CSS/Style property manipulation module
   * @group (Core)
   */
  qx.Bootstrap.define("qx.module.Css", {
    statics: {
      /**
       * INTERNAL
       *
       * Returns the rendered height of the first element in the collection.
       * @attach {qxWeb}
       * @param force {Boolean?false} When true also get the height of a <em>non displayed</em> element
       * @return {Number} The first item's rendered height
       */
      _getHeight: function _getHeight(force) {
        var elem = this[0];

        if (elem) {
          if (qx.dom.Node.isElement(elem)) {
            var elementHeight;

            if (force) {
              var stylesToSwap = {
                display: "block",
                position: "absolute",
                visibility: "hidden"
              };
              elementHeight = qx.module.Css.__swap__P_140_0(elem, stylesToSwap, "_getHeight", this);
            } else {
              elementHeight = qx.bom.element.Dimension.getHeight(elem);
            }

            return elementHeight;
          } else if (qx.dom.Node.isDocument(elem)) {
            return qx.bom.Document.getHeight(qx.dom.Node.getWindow(elem));
          } else if (qx.dom.Node.isWindow(elem)) {
            return qx.bom.Viewport.getHeight(elem);
          }
        }

        return null;
      },

      /**
       * INTERNAL
       *
       * Returns the rendered width of the first element in the collection
       * @attach {qxWeb}
       * @param force {Boolean?false} When true also get the width of a <em>non displayed</em> element
       * @return {Number} The first item's rendered width
       */
      _getWidth: function _getWidth(force) {
        var elem = this[0];

        if (elem) {
          if (qx.dom.Node.isElement(elem)) {
            var elementWidth;

            if (force) {
              var stylesToSwap = {
                display: "block",
                position: "absolute",
                visibility: "hidden"
              };
              elementWidth = qx.module.Css.__swap__P_140_0(elem, stylesToSwap, "_getWidth", this);
            } else {
              elementWidth = qx.bom.element.Dimension.getWidth(elem);
            }

            return elementWidth;
          } else if (qx.dom.Node.isDocument(elem)) {
            return qx.bom.Document.getWidth(qx.dom.Node.getWindow(elem));
          } else if (qx.dom.Node.isWindow(elem)) {
            return qx.bom.Viewport.getWidth(elem);
          }
        }

        return null;
      },

      /**
       * INTERNAL
       *
       * Returns the content height of the first element in the collection.
       * This is the maximum height the element can use, excluding borders,
       * margins, padding or scroll bars.
       * @attach {qxWeb}
       * @param force {Boolean?false} When true also get the content height of a <em>non displayed</em> element
       * @return {Number} Computed content height
       */
      _getContentHeight: function _getContentHeight(force) {
        var obj = this[0];

        if (qx.dom.Node.isElement(obj)) {
          var contentHeight;

          if (force) {
            var stylesToSwap = {
              position: "absolute",
              visibility: "hidden",
              display: "block"
            };
            contentHeight = qx.module.Css.__swap__P_140_0(obj, stylesToSwap, "_getContentHeight", this);
          } else {
            contentHeight = qx.bom.element.Dimension.getContentHeight(obj);
          }

          return contentHeight;
        }

        return null;
      },

      /**
       * INTERNAL
       *
       * Returns the content width of the first element in the collection.
       * This is the maximum width the element can use, excluding borders,
       * margins, padding or scroll bars.
       * @attach {qxWeb}
       * @param force {Boolean?false} When true also get the content width of a <em>non displayed</em> element
       * @return {Number} Computed content width
       */
      _getContentWidth: function _getContentWidth(force) {
        var obj = this[0];

        if (qx.dom.Node.isElement(obj)) {
          var contentWidth;

          if (force) {
            var stylesToSwap = {
              position: "absolute",
              visibility: "hidden",
              display: "block"
            };
            contentWidth = qx.module.Css.__swap__P_140_0(obj, stylesToSwap, "_getContentWidth", this);
          } else {
            contentWidth = qx.bom.element.Dimension.getContentWidth(obj);
          }

          return contentWidth;
        }

        return null;
      },

      /**
       * Maps HTML elements to their default "display" style values.
       */
      __displayDefaults__P_140_1: {},

      /**
       * Attempts tp determine the default "display" style value for
       * elements with the given tag name.
       *
       * @param tagName {String} Tag name
       * @param  doc {Document?} Document element. Default: The current document
       * @return {String} The default "display" value, e.g. <code>inline</code>
       * or <code>block</code>
       */
      __getDisplayDefault__P_140_2: function __getDisplayDefault__P_140_2(tagName, doc) {
        var defaults = qx.module.Css.__displayDefaults__P_140_1;

        if (!defaults[tagName]) {
          var docu = doc || document;
          var tempEl = qxWeb(docu.createElement(tagName)).appendTo(doc.body);
          defaults[tagName] = tempEl.getStyle("display");
          tempEl.remove();
        }

        return defaults[tagName] || "";
      },

      /**
       * Swaps the given styles of the element and execute the callback
       * before the original values are restored.
       *
       * Finally returns the return value of the callback.
       *
       * @param element {Element} the DOM element to operate on
       * @param styles {Map} the styles to swap
       * @param methodName {String} the callback functions name
       * @param context {Object} the context in which the callback should be called
       * @return {Object} the return value of the callback
       */
      __swap__P_140_0: function __swap__P_140_0(element, styles, methodName, context) {
        // get the current values
        var currentValues = {};

        for (var styleProperty in styles) {
          currentValues[styleProperty] = element.style[styleProperty];
          element.style[styleProperty] = styles[styleProperty];
        }

        var value = context[methodName]();

        for (var styleProperty in currentValues) {
          element.style[styleProperty] = currentValues[styleProperty];
        }

        return value;
      },

      /**
       * Includes a Stylesheet file
       *
       * @attachStatic {qxWeb}
       * @param uri {String} The stylesheet's URI
       * @param doc {Document?} Document to modify
       */
      includeStylesheet: function includeStylesheet(uri, doc) {
        qx.bom.Stylesheet.includeFile(uri, doc);
      }
    },
    members: {
      /**
       * Returns the rendered height of the first element in the collection.
       * @attach {qxWeb}
       * @param force {Boolean?false} When true also get the height of a <em>non displayed</em> element
       * @return {Number} The first item's rendered height
       */
      getHeight: function getHeight(force) {
        return this._getHeight(force);
      },

      /**
       * Returns the rendered width of the first element in the collection
       * @attach {qxWeb}
       * @param force {Boolean?false} When true also get the width of a <em>non displayed</em> element
       * @return {Number} The first item's rendered width
       */
      getWidth: function getWidth(force) {
        return this._getWidth(force);
      },

      /**
       * Returns the content height of the first element in the collection.
       * This is the maximum height the element can use, excluding borders,
       * margins, padding or scroll bars.
       * @attach {qxWeb}
       * @param force {Boolean?false} When true also get the content height of a <em>non displayed</em> element
       * @return {Number} Computed content height
       */
      getContentHeight: function getContentHeight(force) {
        return this._getContentHeight(force);
      },

      /**
       * Returns the content width of the first element in the collection.
       * This is the maximum width the element can use, excluding borders,
       * margins, padding or scroll bars.
       * @attach {qxWeb}
       * @param force {Boolean?false} When true also get the content width of a <em>non displayed</em> element
       * @return {Number} Computed content width
       */
      getContentWidth: function getContentWidth(force) {
        return this._getContentWidth(force);
      },

      /**
       * Shows any elements with "display: none" in the collection. If an element
       * was hidden by using the {@link #hide} method, its previous
       * "display" style value will be re-applied. Otherwise, the
       * default "display" value for the element type will be applied.
       *
       * @attach {qxWeb}
       * @return {qxWeb} The collection for chaining
       */
      show: function show() {
        this._forEachElementWrapped(function (item) {
          var currentVal = item.getStyle("display");
          var prevVal = item[0].$$qPrevDisp;
          var newVal;

          if (currentVal == "none") {
            if (prevVal && prevVal != "none") {
              newVal = prevVal;
            } else {
              var doc = qxWeb.getDocument(item[0]);
              newVal = qx.module.Css.__getDisplayDefault__P_140_2(item[0].tagName, doc);
            }

            item.setStyle("display", newVal);
            item[0].$$qPrevDisp = "none";
          }
        });

        return this;
      },

      /**
       * Hides all elements in the collection by setting their "display"
       * style to "none". The previous value is stored so it can be re-applied
       * when {@link #show} is called.
       *
       * @attach {qxWeb}
       * @return {qxWeb} The collection for chaining
       */
      hide: function hide() {
        this._forEachElementWrapped(function (item) {
          var prevStyle = item.getStyle("display");

          if (prevStyle !== "none") {
            item[0].$$qPrevDisp = prevStyle;
            item.setStyle("display", "none");
          }
        });

        return this;
      },

      /**
       * Returns the distance between the first element in the collection and its
       * offset parent
       *
       * @attach {qxWeb}
       * @return {Map} a map with the keys <code>left</code> and <code>top</code>
       * containing the distance between the elements
       */
      getPosition: function getPosition() {
        var obj = this[0];

        if (qx.dom.Node.isElement(obj)) {
          return qx.bom.element.Location.getPosition(obj);
        }

        return null;
      },

      /**
       * Returns the computed location of the given element in the context of the
       * document dimensions.
       *
       * Supported modes:
       *
       * * <code>margin</code>: Calculate from the margin box of the element (bigger than the visual appearance: including margins of given element)
       * * <code>box</code>: Calculates the offset box of the element (default, uses the same size as visible)
       * * <code>border</code>: Calculate the border box (useful to align to border edges of two elements).
       * * <code>scroll</code>: Calculate the scroll box (relevant for absolute positioned content).
       * * <code>padding</code>: Calculate the padding box (relevant for static/relative positioned content).
       *
       * @attach {qxWeb}
       * @param mode {String?box} A supported option. See comment above.
       * @return {Map} A map with the keys <code>left</code>, <code>top</code>,
       * <code>right</code> and <code>bottom</code> which contains the distance
       * of the element relative to the document.
       */
      getOffset: function getOffset(mode) {
        var elem = this[0];

        if (elem && qx.dom.Node.isElement(elem)) {
          return qx.bom.element.Location.get(elem, mode);
        }

        return null;
      },

      /**
       * Modifies the given style property on all elements in the collection.
       *
       * @attach {qxWeb}
       * @param name {String} Name of the style property to modify
       * @param value {var} The value to apply
       * @return {qxWeb} The collection for chaining
       */
      setStyle: function setStyle(name, value) {
        if (/\w-\w/.test(name)) {
          name = qx.lang.String.camelCase(name);
        }

        this._forEachElement(function (item) {
          qx.bom.element.Style.set(item, name, value);
        });

        return this;
      },

      /**
       * Returns the value of the given style property for the first item in the
       * collection.
       *
       * @attach {qxWeb}
       * @param name {String} Style property name
       * @return {var} Style property value
       */
      getStyle: function getStyle(name) {
        if (this[0] && qx.dom.Node.isElement(this[0])) {
          if (/\w-\w/.test(name)) {
            name = qx.lang.String.camelCase(name);
          }

          return qx.bom.element.Style.get(this[0], name);
        }

        return null;
      },

      /**
       * Sets multiple style properties for each item in the collection.
       *
       * @attach {qxWeb}
       * @param styles {Map} A map of style property name/value pairs
       * @return {qxWeb} The collection for chaining
       */
      setStyles: function setStyles(styles) {
        for (var name in styles) {
          this.setStyle(name, styles[name]);
        }

        return this;
      },

      /**
       * Returns the values of multiple style properties for each item in the
       * collection
       *
       * @attach {qxWeb}
       * @param names {String[]} List of style property names
       * @return {Map} Map of style property name/value pairs
       */
      getStyles: function getStyles(names) {
        var styles = {};

        for (var i = 0; i < names.length; i++) {
          styles[names[i]] = this.getStyle(names[i]);
        }

        return styles;
      },

      /**
       * Adds a class name to each element in the collection
       *
       * @attach {qxWeb}
       * @param name {String} Class name
       * @return {qxWeb} The collection for chaining
       */
      addClass: function addClass(name) {
        this._forEachElement(function (item) {
          qx.bom.element.Class.add(item, name);
        });

        return this;
      },

      /**
       * Adds multiple class names to each element in the collection
       *
       * @attach {qxWeb}
       * @param names {String[]} List of class names to add
       * @return {qxWeb} The collection for chaining
       */
      addClasses: function addClasses(names) {
        this._forEachElement(function (item) {
          qx.bom.element.Class.addClasses(item, names);
        });

        return this;
      },

      /**
       * Removes a class name from each element in the collection
       *
       * @attach {qxWeb}
       * @param name {String} The class name to remove
       * @return {qxWeb} The collection for chaining
       */
      removeClass: function removeClass(name) {
        this._forEachElement(function (item) {
          qx.bom.element.Class.remove(item, name);
        });

        return this;
      },

      /**
       * Removes multiple class names from each element in the collection.
       * Use {@link qx.module.Attribute#removeAttribute} to remove all classes.
       *
       * @attach {qxWeb}
       * @param names {String[]} List of class names to remove
       * @return {qxWeb} The collection for chaining
       */
      removeClasses: function removeClasses(names) {
        this._forEachElement(function (item) {
          qx.bom.element.Class.removeClasses(item, names);
        });

        return this;
      },

      /**
       * Checks if the first element in the collection has the given class name
       *
       * @attach {qxWeb}
       * @param name {String} Class name to check for
       * @return {Boolean} <code>true</code> if the first item has the given class name
       */
      hasClass: function hasClass(name) {
        if (!this[0] || !qx.dom.Node.isElement(this[0])) {
          return false;
        }

        return qx.bom.element.Class.has(this[0], name);
      },

      /**
       * Returns the class name of the first element in the collection
       *
       * @attach {qxWeb}
       * @return {String} Class name
       */
      getClass: function getClass() {
        if (!this[0] || !qx.dom.Node.isElement(this[0])) {
          return "";
        }

        return qx.bom.element.Class.get(this[0]);
      },

      /**
       * Toggles the given class name on each item in the collection
       *
       * @attach {qxWeb}
       * @param name {String} Class name
       * @return {qxWeb} The collection for chaining
       */
      toggleClass: function toggleClass(name) {
        var bCls = qx.bom.element.Class;

        this._forEachElement(function (item) {
          bCls.has(item, name) ? bCls.remove(item, name) : bCls.add(item, name);
        });

        return this;
      },

      /**
       * Toggles the given list of class names on each item in the collection
       *
       * @attach {qxWeb}
       * @param names {String[]} Class names
       * @return {qxWeb} The collection for chaining
       */
      toggleClasses: function toggleClasses(names) {
        for (var i = 0, l = names.length; i < l; i++) {
          this.toggleClass(names[i]);
        }

        return this;
      },

      /**
       * Replaces a class name on each element in the collection
       *
       * @attach {qxWeb}
       * @param oldName {String} Class name to remove
       * @param newName {String} Class name to add
       * @return {qxWeb} The collection for chaining
       */
      replaceClass: function replaceClass(oldName, newName) {
        this._forEachElement(function (item) {
          qx.bom.element.Class.replace(item, oldName, newName);
        });

        return this;
      }
    },
    defer: function defer(statics) {
      qxWeb.$attachAll(this); // manually attach private method which is ignored by attachAll

      qxWeb.$attach({
        "_getWidth": statics._getWidth,
        "_getHeight": statics._getHeight,
        "_getContentHeight": statics._getContentHeight,
        "_getContentWidth": statics._getContentWidth
      });
    }
  });
  qx.module.Css.$$dbClassInfo = $$dbClassInfo;
})();

(function () {
  var $$dbClassInfo = {
    "dependsOn": {
      "qx.bom.client.Engine": {
        "require": true
      },
      "qx.lang.normalize.Array": {
        "require": true
      },
      "qx.core.Environment": {
        "defer": "load",
        "require": true
      },
      "qx.Bootstrap": {
        "usage": "dynamic",
        "require": true
      }
    },
    "environment": {
      "provided": [],
      "required": {
        "engine.name": {
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
       2007-2009 1&1 Internet AG, Germany, http://www.1und1.de
  
     License:
       MIT: https://opensource.org/licenses/MIT
       See the LICENSE file in the project's top-level directory for details.
  
     Authors:
       * Sebastian Werner (wpbasti)
       * Fabian Jakobs (fjakobs)
  
     ======================================================================
  
     This class uses ideas and code snippets presented at
     http://webreflection.blogspot.com/2008/05/habemus-array-unlocked-length-in-ie8.html
     http://webreflection.blogspot.com/2008/05/stack-and-arrayobject-how-to-create.html
  
     Author:
       Andrea Giammarchi
  
     License:
       MIT: http://www.opensource.org/licenses/mit-license.php
  
     ======================================================================
  
     This class uses documentation of the native Array methods from the MDC
     documentation of Mozilla.
  
     License:
       CC Attribution-Sharealike License:
       http://creativecommons.org/licenses/by-sa/2.5/
  
  ************************************************************************ */

  /**
   * This class is the common superclass for most array classes in
   * qooxdoo. It supports all of the shiny 1.6 JavaScript array features
   * like <code>forEach</code> and <code>map</code>.
   *
   * This class may be instantiated instead of the native Array if
   * one wants to work with a feature-unified Array instead of the native
   * one. This class uses native features whereever possible but fills
   * all missing implementations with custom ones.
   *
   * Through the ability to extend from this class one could add even
   * more utility features on top of it.
   *
   * @require(qx.bom.client.Engine)
   * @require(qx.lang.normalize.Array)
   */
  qx.Bootstrap.define("qx.type.BaseArray", {
    extend: Array,

    /*
    *****************************************************************************
       CONSTRUCTOR
    *****************************************************************************
    */

    /**
     * Creates a new Array with the given length or the listed elements.
     *
     * <pre class="javascript">
     * var arr1 = new qx.type.BaseArray(arrayLength);
     * var arr2 = new qx.type.BaseArray(item0, item1, ..., itemN);
     * </pre>
     *
     * * <code>arrayLength</code>: The initial length of the array. You can access
     * this value using the length property. If the value specified is not a
     * number, an array of length 1 is created, with the first element having
     * the specified value. The maximum length allowed for an
     * array is 2^32-1, i.e. 4,294,967,295.
     * * <code>itemN</code>:  A value for the element in that position in the
     * array. When this form is used, the array is initialized with the specified
     * values as its elements, and the array's length property is set to the
     * number of arguments.
     *
     * @param length_or_items {Integer|var?null} The initial length of the array
     *        OR an argument list of values.
     */
    construct: function construct(length_or_items) {},

    /*
    *****************************************************************************
       MEMBERS
    *****************************************************************************
    */
    members: {
      /**
       * Converts a base array to a native Array
       *
       * @signature function()
       * @return {Array} The native array
       */
      toArray: null,

      /**
       * Returns the current number of items stored in the Array
       *
       * @signature function()
       * @return {Integer} number of items
       */
      valueOf: null,

      /**
       * Removes the last element from an array and returns that element.
       *
       * This method modifies the array.
       *
       * @signature function()
       * @return {var} The last element of the array.
       */
      pop: null,

      /**
       * Adds one or more elements to the end of an array and returns the new length of the array.
       *
       * This method modifies the array.
       *
       * @signature function(varargs)
       * @param varargs {var} The elements to add to the end of the array.
       * @return {Integer} The new array's length
       */
      push: null,

      /**
       * Reverses the order of the elements of an array -- the first becomes the last, and the last becomes the first.
       *
       * This method modifies the array.
       *
       * @signature function()
       * @return {Array} Returns the modified array (works in place)
       */
      reverse: null,

      /**
       * Removes the first element from an array and returns that element.
       *
       * This method modifies the array.
       *
       * @signature function()
       * @return {var} The first element of the array.
       */
      shift: null,

      /**
       * Sorts the elements of an array.
       *
       * This method modifies the array.
       *
       * @signature function(compareFunction)
       * @param compareFunction {Function?null} Specifies a function that defines the sort order. If omitted,
       *   the array is sorted lexicographically (in dictionary order) according to the string conversion of each element.
       * @return {Array} Returns the modified array (works in place)
       */
      sort: null,

      /**
       * Adds and/or removes elements from an array.
       *
       * @signature function(index, howMany, varargs)
       * @param index {Integer} Index at which to start changing the array. If negative, will begin
       *   that many elements from the end.
       * @param howMany {Integer} An integer indicating the number of old array elements to remove.
       *   If <code>howMany</code> is 0, no elements are removed. In this case, you should specify
       *   at least one new element.
       * @param varargs {var?null} The elements to add to the array. If you don't specify any elements,
       *   splice simply removes elements from the array.
       * @return {qx.type.BaseArray} New array with the removed elements.
       */
      splice: null,

      /**
       * Adds one or more elements to the front of an array and returns the new length of the array.
       *
       * This method modifies the array.
       *
       * @signature function(varargs)
       * @param varargs {var} The elements to add to the front of the array.
       * @return {Integer} The new array's length
       */
      unshift: null,

      /**
       * Returns a new array comprised of this array joined with other array(s) and/or value(s).
       *
       * This method does not modify the array and returns a modified copy of the original.
       *
       * @signature function(varargs)
       * @param varargs {Array|var} Arrays and/or values to concatenate to the resulting array.
       * @return {qx.type.BaseArray} New array built of the given arrays or values.
       */
      concat: null,

      /**
       * Joins all elements of an array into a string.
       *
       * @signature function(separator)
       * @param separator {String} Specifies a string to separate each element of the array. The separator is
       *   converted to a string if necessary. If omitted, the array elements are separated with a comma.
       * @return {String} The stringified values of all elements divided by the given separator.
       */
      join: null,

      /**
       * Extracts a section of an array and returns a new array.
       *
       * @signature function(begin, end)
       * @param begin {Integer} Zero-based index at which to begin extraction. As a negative index, start indicates
       *   an offset from the end of the sequence. slice(-2) extracts the second-to-last element and the last element
       *   in the sequence.
       * @param end {Integer?length} Zero-based index at which to end extraction. slice extracts up to but not including end.
       *   <code>slice(1,4)</code> extracts the second element through the fourth element (elements indexed 1, 2, and 3).
       *   As a negative index, end indicates an offset from the end of the sequence. slice(2,-1) extracts the third element through the second-to-last element in the sequence.
       *   If end is omitted, slice extracts to the end of the sequence.
       * @return {qx.type.BaseArray} An new array which contains a copy of the given region.
       */
      slice: null,

      /**
       * Returns a string representing the array and its elements. Overrides the Object.prototype.toString method.
       *
       * @signature function()
       * @return {String} The string representation of the array.
       */
      toString: null,

      /**
       * Returns the first (least) index of an element within the array equal to the specified value, or -1 if none is found.
       *
       * @signature function(searchElement, fromIndex)
       * @param searchElement {var} Element to locate in the array.
       * @param fromIndex {Integer?0} The index at which to begin the search. Defaults to 0, i.e. the
       *   whole array will be searched. If the index is greater than or equal to the length of the
       *   array, -1 is returned, i.e. the array will not be searched. If negative, it is taken as
       *   the offset from the end of the array. Note that even when the index is negative, the array
       *   is still searched from front to back. If the calculated index is less than 0, the whole
       *   array will be searched.
       * @return {Integer} The index of the given element
       */
      indexOf: null,

      /**
       * Returns the last (greatest) index of an element within the array equal to the specified value, or -1 if none is found.
       *
       * @signature function(searchElement, fromIndex)
       * @param searchElement {var} Element to locate in the array.
       * @param fromIndex {Integer?length} The index at which to start searching backwards. Defaults to
       *   the array's length, i.e. the whole array will be searched. If the index is greater than
       *   or equal to the length of the array, the whole array will be searched. If negative, it
       *   is taken as the offset from the end of the array. Note that even when the index is
       *   negative, the array is still searched from back to front. If the calculated index is
       *   less than 0, -1 is returned, i.e. the array will not be searched.
       * @return {Integer} The index of the given element
       */
      lastIndexOf: null,

      /**
       * Executes a provided function once per array element.
       *
       * <code>forEach</code> executes the provided function (<code>callback</code>) once for each
       * element present in the array.  <code>callback</code> is invoked only for indexes of the array
       * which have assigned values; it is not invoked for indexes which have been deleted or which
       * have never been assigned values.
       *
       * <code>callback</code> is invoked with three arguments: the value of the element, the index
       * of the element, and the Array object being traversed.
       *
       * If a <code>obj</code> parameter is provided to <code>forEach</code>, it will be used
       * as the <code>this</code> for each invocation of the <code>callback</code>.  If it is not
       * provided, or is <code>null</code>, the global object associated with <code>callback</code>
       * is used instead.
       *
       * <code>forEach</code> does not mutate the array on which it is called.
       *
       * The range of elements processed by <code>forEach</code> is set before the first invocation of
       * <code>callback</code>.  Elements which are appended to the array after the call to
       * <code>forEach</code> begins will not be visited by <code>callback</code>. If existing elements
       * of the array are changed, or deleted, their value as passed to <code>callback</code> will be
       * the value at the time <code>forEach</code> visits them; elements that are deleted are not visited.
       *
       * @signature function(callback, obj)
       * @param callback {Function} Function to execute for each element.
       * @param obj {Object} Object to use as this when executing callback.
       */
      forEach: null,

      /**
       * Creates a new array with all elements that pass the test implemented by the provided
       * function.
       *
       * <code>filter</code> calls a provided <code>callback</code> function once for each
       * element in an array, and constructs a new array of all the values for which
       * <code>callback</code> returns a true value.  <code>callback</code> is invoked only
       * for indexes of the array which have assigned values; it is not invoked for indexes
       * which have been deleted or which have never been assigned values.  Array elements which
       * do not pass the <code>callback</code> test are simply skipped, and are not included
       * in the new array.
       *
       * <code>callback</code> is invoked with three arguments: the value of the element, the
       * index of the element, and the Array object being traversed.
       *
       * If a <code>obj</code> parameter is provided to <code>filter</code>, it will
       * be used as the <code>this</code> for each invocation of the <code>callback</code>.
       * If it is not provided, or is <code>null</code>, the global object associated with
       * <code>callback</code> is used instead.
       *
       * <code>filter</code> does not mutate the array on which it is called. The range of
       * elements processed by <code>filter</code> is set before the first invocation of
       * <code>callback</code>. Elements which are appended to the array after the call to
       * <code>filter</code> begins will not be visited by <code>callback</code>. If existing
       * elements of the array are changed, or deleted, their value as passed to <code>callback</code>
       * will be the value at the time <code>filter</code> visits them; elements that are deleted
       * are not visited.
       *
       * @signature function(callback, obj)
       * @param callback {Function} Function to test each element of the array.
       * @param obj {Object} Object to use as <code>this</code> when executing <code>callback</code>.
       * @return {qx.type.BaseArray} The newly created array with all matching elements
       */
      filter: null,

      /**
       * Creates a new array with the results of calling a provided function on every element in this array.
       *
       * <code>map</code> calls a provided <code>callback</code> function once for each element in an array,
       * in order, and constructs a new array from the results.  <code>callback</code> is invoked only for
       * indexes of the array which have assigned values; it is not invoked for indexes which have been
       * deleted or which have never been assigned values.
       *
       * <code>callback</code> is invoked with three arguments: the value of the element, the index of the
       * element, and the Array object being traversed.
       *
       * If a <code>obj</code> parameter is provided to <code>map</code>, it will be used as the
       * <code>this</code> for each invocation of the <code>callback</code>. If it is not provided, or is
       * <code>null</code>, the global object associated with <code>callback</code> is used instead.
       *
       * <code>map</code> does not mutate the array on which it is called.
       *
       * The range of elements processed by <code>map</code> is set before the first invocation of
       * <code>callback</code>. Elements which are appended to the array after the call to <code>map</code>
       * begins will not be visited by <code>callback</code>.  If existing elements of the array are changed,
       * or deleted, their value as passed to <code>callback</code> will be the value at the time
       * <code>map</code> visits them; elements that are deleted are not visited.
       *
       * @signature function(callback, obj)
       * @param callback {Function} Function produce an element of the new Array from an element of the current one.
       * @param obj {Object} Object to use as <code>this</code> when executing <code>callback</code>.
       * @return {qx.type.BaseArray} A new array which contains the return values of every item executed through the given function
       */
      map: null,

      /**
       * Tests whether some element in the array passes the test implemented by the provided function.
       *
       * <code>some</code> executes the <code>callback</code> function once for each element present in
       * the array until it finds one where <code>callback</code> returns a true value. If such an element
       * is found, <code>some</code> immediately returns <code>true</code>. Otherwise, <code>some</code>
       * returns <code>false</code>. <code>callback</code> is invoked only for indexes of the array which
       * have assigned values; it is not invoked for indexes which have been deleted or which have never
       * been assigned values.
       *
       * <code>callback</code> is invoked with three arguments: the value of the element, the index of the
       * element, and the Array object being traversed.
       *
       * If a <code>obj</code> parameter is provided to <code>some</code>, it will be used as the
       * <code>this</code> for each invocation of the <code>callback</code>. If it is not provided, or is
       * <code>null</code>, the global object associated with <code>callback</code> is used instead.
       *
       * <code>some</code> does not mutate the array on which it is called.
       *
       * The range of elements processed by <code>some</code> is set before the first invocation of
       * <code>callback</code>.  Elements that are appended to the array after the call to <code>some</code>
       * begins will not be visited by <code>callback</code>. If an existing, unvisited element of the array
       * is changed by <code>callback</code>, its value passed to the visiting <code>callback</code> will
       * be the value at the time that <code>some</code> visits that element's index; elements that are
       * deleted are not visited.
       *
       * @signature function(callback, obj)
       * @param callback {Function} Function to test for each element.
       * @param obj {Object} Object to use as <code>this</code> when executing <code>callback</code>.
       * @return {Boolean} Whether at least one elements passed the test
       */
      some: null,

      /**
       * Tests whether all elements in the array pass the test implemented by the provided function.
       *
       * <code>every</code> executes the provided <code>callback</code> function once for each element
       * present in the array until it finds one where <code>callback</code> returns a false value. If
       * such an element is found, the <code>every</code> method immediately returns <code>false</code>.
       * Otherwise, if <code>callback</code> returned a true value for all elements, <code>every</code>
       * will return <code>true</code>.  <code>callback</code> is invoked only for indexes of the array
       * which have assigned values; it is not invoked for indexes which have been deleted or which have
       * never been assigned values.
       *
       * <code>callback</code> is invoked with three arguments: the value of the element, the index of
       * the element, and the Array object being traversed.
       *
       * If a <code>obj</code> parameter is provided to <code>every</code>, it will be used as
       * the <code>this</code> for each invocation of the <code>callback</code>. If it is not provided,
       * or is <code>null</code>, the global object associated with <code>callback</code> is used instead.
       *
       * <code>every</code> does not mutate the array on which it is called. The range of elements processed
       * by <code>every</code> is set before the first invocation of <code>callback</code>. Elements which
       * are appended to the array after the call to <code>every</code> begins will not be visited by
       * <code>callback</code>.  If existing elements of the array are changed, their value as passed
       * to <code>callback</code> will be the value at the time <code>every</code> visits them; elements
       * that are deleted are not visited.
       *
       * @signature function(callback, obj)
       * @param callback {Function} Function to test for each element.
       * @param obj {Object} Object to use as <code>this</code> when executing <code>callback</code>.
       * @return {Boolean} Whether all elements passed the test
       */
      every: null
    }
  });

  (function () {
    function createStackConstructor(stack) {
      // In IE don't inherit from Array but use an empty object as prototype
      // and copy the methods from Array
      if (qx.core.Environment.get("engine.name") == "mshtml") {
        Stack.prototype = {
          length: 0,
          $$isArray: true
        };
        var args = "pop.push.reverse.shift.sort.splice.unshift.join.slice".split(".");

        for (var length = args.length; length;) {
          Stack.prototype[args[--length]] = Array.prototype[args[length]];
        }
      }

      ; // Remember Array's slice method

      var slice = Array.prototype.slice; // Fix "concat" method

      Stack.prototype.concat = function () {
        var constructor = this.slice(0);

        for (var i = 0, length = arguments.length; i < length; i++) {
          var copy;

          if (arguments[i] instanceof Stack) {
            copy = slice.call(arguments[i], 0);
          } else if (arguments[i] instanceof Array) {
            copy = arguments[i];
          } else {
            copy = [arguments[i]];
          }

          constructor.push.apply(constructor, copy);
        }

        return constructor;
      }; // Fix "toString" method


      Stack.prototype.toString = function () {
        return slice.call(this, 0).toString();
      }; // Fix "toLocaleString"


      Stack.prototype.toLocaleString = function () {
        return slice.call(this, 0).toLocaleString();
      }; // Fix constructor


      Stack.prototype.constructor = Stack; // Add JS 1.6 Array features

      Stack.prototype.indexOf = Array.prototype.indexOf;
      Stack.prototype.lastIndexOf = Array.prototype.lastIndexOf;
      Stack.prototype.forEach = Array.prototype.forEach;
      Stack.prototype.some = Array.prototype.some;
      Stack.prototype.every = Array.prototype.every;
      var filter = Array.prototype.filter;
      var map = Array.prototype.map; // Fix methods which generates a new instance
      // to return an instance of the same class

      Stack.prototype.filter = function () {
        var ret = new this.constructor();
        ret.push.apply(ret, filter.apply(this, arguments));
        return ret;
      };

      Stack.prototype.map = function () {
        var ret = new this.constructor();
        ret.push.apply(ret, map.apply(this, arguments));
        return ret;
      };

      Stack.prototype.slice = function () {
        var ret = new this.constructor();
        ret.push.apply(ret, Array.prototype.slice.apply(this, arguments));
        return ret;
      };

      Stack.prototype.splice = function () {
        var ret = new this.constructor();
        ret.push.apply(ret, Array.prototype.splice.apply(this, arguments));
        return ret;
      }; // Add new "toArray" method for convert a base array to a native Array


      Stack.prototype.toArray = function () {
        return Array.prototype.slice.call(this, 0);
      }; // Add valueOf() to return the length


      Stack.prototype.valueOf = function () {
        return this.length;
      }; // Return final class


      return Stack;
    }

    function Stack(length) {
      if (arguments.length === 1 && typeof length === "number") {
        this.length = -1 < length && length === length >> .5 ? length : this.push(length);
      } else if (arguments.length) {
        this.push.apply(this, arguments);
      }
    }

    ;

    function PseudoArray() {}

    ;
    PseudoArray.prototype = [];
    Stack.prototype = new PseudoArray();
    Stack.prototype.length = 0;
    qx.type.BaseArray = createStackConstructor(Stack);
  })();

  qx.type.BaseArray.$$dbClassInfo = $$dbClassInfo;
})();
//# sourceMappingURL=package-3.js.map?dt=1598051433286
qx.$$packageData['3'] = {
  "locales": {},
  "resources": {},
  "translations": {}
};
