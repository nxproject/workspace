(function () {
  var $$dbClassInfo = {
    "dependsOn": {
      "qx.event.handler.Touch": {
        "require": true,
        "defer": "runtime"
      },
      "qx.event.handler.Pointer": {
        "require": true,
        "defer": "runtime"
      },
      "qx.event.dispatch.DomBubbling": {
        "require": true,
        "defer": "runtime"
      },
      "qx.ui.mobile.core.Widget": {
        "require": true,
        "defer": "runtime"
      },
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
        "construct": true,
        "defer": "runtime",
        "require": true
      },
      "qx.bom.Viewport": {},
      "qx.bom.element.Attribute": {},
      "qx.bom.element.Class": {},
      "qx.event.handler.GestureCore": {},
      "qx.event.type.Event": {},
      "qx.event.Pool": {}
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
   * Connects the widgets to the browser DOM events.
   *
   * @require(qx.event.handler.Touch)
   * @require(qx.event.handler.Pointer)
   * @require(qx.event.dispatch.DomBubbling)
   * @require(qx.ui.mobile.core.Widget)
   */
  qx.Class.define("qx.ui.mobile.core.EventHandler", {
    extend: qx.core.Object,
    implement: qx.event.IEventHandler,

    /*
    *****************************************************************************
       CONSTRUCTOR
    *****************************************************************************
    */
    construct: function construct() {
      qx.core.Object.constructor.call(this);
      this.__manager__P_182_0 = qx.event.Registration.getManager(window);
    },

    /*
    *****************************************************************************
       STATICS
    *****************************************************************************
    */
    statics: {
      /** @type {Integer} Priority of this handler */
      PRIORITY: qx.event.Registration.PRIORITY_FIRST,

      /** @type {Map} Supported event types. Identical to events map of qx.ui.core.Widget */
      SUPPORTED_TYPES: {
        // mouse events
        mousemove: 1,
        mouseover: 1,
        mouseout: 1,
        mousedown: 1,
        mouseup: 1,
        click: 1,
        dblclick: 1,
        contextmenu: 1,
        mousewheel: 1,
        // key events
        keyup: 1,
        keydown: 1,
        keypress: 1,
        keyinput: 1,
        // mouse capture
        capture: 1,
        losecapture: 1,
        // focus events
        focusin: 1,
        focusout: 1,
        focus: 1,
        blur: 1,
        activate: 1,
        deactivate: 1,
        // appear events
        appear: 1,
        disappear: 1,
        // resize event
        // resize : 1,
        // drag drop events
        dragstart: 1,
        dragend: 1,
        dragover: 1,
        dragleave: 1,
        drop: 1,
        drag: 1,
        dragchange: 1,
        droprequest: 1,
        // scroll events
        roll: 1,
        // touch events
        touchstart: 1,
        touchend: 1,
        touchmove: 1,
        touchcancel: 1,
        // gestures
        tap: 1,
        longtap: 1,
        swipe: 1,
        dbltap: 1,
        track: 1,
        trackend: 1,
        trackstart: 1,
        pinch: 1,
        rotate: 1,
        // pointer events
        pointermove: 1,
        pointerover: 1,
        pointerout: 1,
        pointerdown: 1,
        pointerup: 1,
        pointercancel: 1
      },

      /** @type {Integer} Whether the method "canHandleEvent" must be called */
      IGNORE_CAN_HANDLE: false,
      __activeTarget__P_182_1: null,
      __scrollLeft__P_182_2: null,
      __scrollTop__P_182_3: null,
      __startY__P_182_4: null,
      __timer__P_182_5: null,

      /**
       * Event handler. Called when the pointerdown event occurs.
       * Sets the <code>active</class> class to the event target after a certain
       * time.
       *
       * @param domEvent {qx.event.type.Pointer} The pointerdown event
       */
      __onPointerDown__P_182_6: function __onPointerDown__P_182_6(domEvent) {
        if (!domEvent.isPrimary()) {
          return;
        }

        var EventHandler = qx.ui.mobile.core.EventHandler;
        EventHandler.__scrollLeft__P_182_2 = qx.bom.Viewport.getScrollLeft();
        EventHandler.__scrollTop__P_182_3 = qx.bom.Viewport.getScrollTop();
        EventHandler.__startY__P_182_4 = domEvent.getScreenTop();

        EventHandler.__cancelActiveStateTimer__P_182_7();

        var target = domEvent.getTarget();

        while (target && target.parentNode && target.parentNode.nodeType == 1 && qx.bom.element.Attribute.get(target, "data-activatable") != "true") {
          target = target.parentNode;
        }

        EventHandler.__activeTarget__P_182_1 = target;
        EventHandler.___timer__P_182_8 = window.setTimeout(function () {
          EventHandler.___timer__P_182_8 = null;
          var target = EventHandler.__activeTarget__P_182_1;

          if (target && qx.bom.element.Attribute.get(target, "data-selectable") != "false") {
            qx.bom.element.Class.add(target, "active");
          }
        }, 100);
      },

      /**
       * Event handler. Called when the pointerup event occurs.
       * Removes the <code>active</class> class from the event target.
       *
       * @param domEvent {qx.event.type.Pointer} The pointerup event
       */
      __onPointerUp__P_182_9: function __onPointerUp__P_182_9(domEvent) {
        qx.ui.mobile.core.EventHandler.__removeActiveState__P_182_10();
      },

      /**
       * Event handler. Called when the pointermove event occurs.
       * Removes the <code>active</class> class from the event target
       * when the viewport was scrolled.
       *
       * @param domEvent {qx.event.type.Pointer} The pointermove event
       */
      __onPointerMove__P_182_11: function __onPointerMove__P_182_11(domEvent) {
        if (!domEvent.isPrimary()) {
          return;
        }

        var EventHandler = qx.ui.mobile.core.EventHandler;

        var deltaY = domEvent.getScreenTop() - EventHandler.__startY__P_182_4;

        if (EventHandler.__activeTarget__P_182_1 && Math.abs(deltaY) >= qx.event.handler.GestureCore.TAP_MAX_DISTANCE[domEvent.getPointerType()]) {
          EventHandler.__removeActiveState__P_182_10();
        }

        if (EventHandler.__activeTarget__P_182_1 && (EventHandler.__scrollLeft__P_182_2 != qx.bom.Viewport.getScrollLeft() || EventHandler.__scrollTop__P_182_3 != qx.bom.Viewport.getScrollTop())) {
          EventHandler.__removeActiveState__P_182_10();
        }
      },

      /**
       * Cancels the active state timer.
       */
      __cancelActiveStateTimer__P_182_7: function __cancelActiveStateTimer__P_182_7() {
        var EventHandler = qx.ui.mobile.core.EventHandler;

        if (EventHandler.___timer__P_182_8) {
          window.clearTimeout(EventHandler.___timer__P_182_8);
          EventHandler.___timer__P_182_8 = null;
        }
      },

      /**
       * Removes the <code>active</class> class from the active target.
       */
      __removeActiveState__P_182_10: function __removeActiveState__P_182_10() {
        var EventHandler = qx.ui.mobile.core.EventHandler;

        EventHandler.__cancelActiveStateTimer__P_182_7();

        var activeTarget = EventHandler.__activeTarget__P_182_1;

        if (activeTarget) {
          qx.bom.element.Class.remove(activeTarget, "active");
        }

        EventHandler.__activeTarget__P_182_1 = null;
      }
    },

    /*
    *****************************************************************************
       MEMBERS
    *****************************************************************************
    */
    members: {
      __manager__P_182_0: null,
      // interface implementation
      canHandleEvent: function canHandleEvent(target, type) {
        return target instanceof qx.ui.mobile.core.Widget;
      },
      // interface implementation
      registerEvent: function registerEvent(target, type, capture) {
        var element = target.getContainerElement();
        qx.event.Registration.addListener(element, type, this._dispatchEvent, this, capture);
      },
      // interface implementation
      unregisterEvent: function unregisterEvent(target, type, capture) {
        var element = target.getContainerElement();
        qx.event.Registration.removeListener(element, type, this._dispatchEvent, this, capture);
      },

      /**
       * Dispatches a DOM event on a widget.
       *
       * @param domEvent {qx.event.type.Event} The event object to dispatch.
       */
      _dispatchEvent: function _dispatchEvent(domEvent) {
        // EVENT TARGET
        var domTarget = domEvent.getTarget();

        if (!domTarget || domTarget.id == null) {
          return;
        }

        var widgetTarget = qx.ui.mobile.core.Widget.getWidgetById(domTarget.id); // EVENT RELATED TARGET

        if (domEvent.getRelatedTarget) {
          var domRelatedTarget = domEvent.getRelatedTarget();

          if (domRelatedTarget && domRelatedTarget.id) {
            var widgetRelatedTarget = qx.ui.mobile.core.Widget.getWidgetById(domRelatedTarget.id);
          }
        } // EVENT CURRENT TARGET


        var currentTarget = domEvent.getCurrentTarget();
        var currentWidget = qx.ui.mobile.core.Widget.getWidgetById(currentTarget.id);

        if (!currentWidget) {
          return;
        } // PROCESS LISTENERS
        // Load listeners


        var capture = domEvent.getEventPhase() == qx.event.type.Event.CAPTURING_PHASE;
        var type = domEvent.getType();

        var listeners = this.__manager__P_182_0.getListeners(currentWidget, type, capture);

        if (!listeners || listeners.length === 0) {
          return;
        } // Create cloned event with correct target


        var widgetEvent = qx.event.Pool.getInstance().getObject(domEvent.constructor);
        domEvent.clone(widgetEvent);
        widgetEvent.setTarget(widgetTarget);
        widgetEvent.setRelatedTarget(widgetRelatedTarget || null);
        widgetEvent.setCurrentTarget(currentWidget); // Keep original target of DOM event, otherwise map it to the original

        var orig = domEvent.getOriginalTarget();

        if (orig && orig.id) {
          var widgetOriginalTarget = qx.ui.mobile.core.Widget.getWidgetById(orig.id);
          widgetEvent.setOriginalTarget(widgetOriginalTarget);
        } else {
          widgetEvent.setOriginalTarget(domTarget);
        } // Dispatch it on all listeners


        for (var i = 0, l = listeners.length; i < l; i++) {
          var context = listeners[i].context || currentWidget;
          listeners[i].handler.call(context, widgetEvent);
        } // Synchronize propagation stopped/prevent default property


        if (widgetEvent.getPropagationStopped()) {
          domEvent.stopPropagation();
        }

        if (widgetEvent.getDefaultPrevented()) {
          domEvent.preventDefault();
        } // Release the event instance to the event pool


        qx.event.Pool.getInstance().poolObject(widgetEvent);
      }
    },

    /*
    *****************************************************************************
       DESTRUCTOR
    *****************************************************************************
    */
    destruct: function destruct() {
      this.__manager__P_182_0 = null;
    },

    /*
    *****************************************************************************
       DEFER
    *****************************************************************************
    */
    defer: function defer(statics) {
      qx.event.Registration.addHandler(statics);
      qx.event.Registration.addListener(document, "pointerdown", statics.__onPointerDown__P_182_6);
      qx.event.Registration.addListener(document, "pointerup", statics.__onPointerUp__P_182_9);
      qx.event.Registration.addListener(document, "pointercancel", statics.__onPointerUp__P_182_9);
      qx.event.Registration.addListener(document, "pointermove", statics.__onPointerMove__P_182_11);
    }
  });
  qx.ui.mobile.core.EventHandler.$$dbClassInfo = $$dbClassInfo;
})();

//# sourceMappingURL=EventHandler.js.map?dt=1598045573097