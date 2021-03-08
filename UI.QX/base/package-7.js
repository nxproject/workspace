(function () {
  var $$dbClassInfo = {
    "dependsOn": {
      "qx.Bootstrap": {
        "usage": "dynamic",
        "require": true
      },
      "qx.bom.Style": {},
      "qx.core.Environment": {
        "defer": "runtime"
      }
    },
    "environment": {
      "provided": ["css.transform", "css.transform.3d"],
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
       * Martin Wittemann (wittemann)
  
  ************************************************************************ */

  /**
   * Responsible for checking all relevant CSS transform properties.
   *
   * Specs:
   * http://www.w3.org/TR/css3-2d-transforms/
   * http://www.w3.org/TR/css3-3d-transforms/
   *
   * @internal
   */
  qx.Bootstrap.define("qx.bom.client.CssTransform", {
    statics: {
      /**
       * Main check method which returns an object if CSS animations are
       * supported. This object contains all necessary keys to work with CSS
       * animations.
       * <ul>
       *  <li><code>name</code> The name of the css transform style</li>
       *  <li><code>style</code> The name of the css transform-style style</li>
       *  <li><code>origin</code> The name of the transform-origin style</li>
       *  <li><code>3d</code> Whether 3d transforms are supported</li>
       *  <li><code>perspective</code> The name of the perspective style</li>
       *  <li><code>perspective-origin</code> The name of the perspective-origin style</li>
       *  <li><code>backface-visibility</code> The name of the backface-visibility style</li>
       * </ul>
       *
       * @internal
       * @return {Object|null} The described object or null, if animations are
       *   not supported.
       */
      getSupport: function getSupport() {
        var name = qx.bom.client.CssTransform.getName();

        if (name != null) {
          return {
            "name": name,
            "style": qx.bom.client.CssTransform.getStyle(),
            "origin": qx.bom.client.CssTransform.getOrigin(),
            "3d": qx.bom.client.CssTransform.get3D(),
            "perspective": qx.bom.client.CssTransform.getPerspective(),
            "perspective-origin": qx.bom.client.CssTransform.getPerspectiveOrigin(),
            "backface-visibility": qx.bom.client.CssTransform.getBackFaceVisibility()
          };
        }

        return null;
      },

      /**
       * Checks for the style name used to set the transform origin.
       * @internal
       * @return {String|null} The name of the style or null, if the style is
       *   not supported.
       */
      getStyle: function getStyle() {
        return qx.bom.Style.getPropertyName("transformStyle");
      },

      /**
       * Checks for the style name used to set the transform origin.
       * @internal
       * @return {String|null} The name of the style or null, if the style is
       *   not supported.
       */
      getPerspective: function getPerspective() {
        return qx.bom.Style.getPropertyName("perspective");
      },

      /**
       * Checks for the style name used to set the perspective origin.
       * @internal
       * @return {String|null} The name of the style or null, if the style is
       *   not supported.
       */
      getPerspectiveOrigin: function getPerspectiveOrigin() {
        return qx.bom.Style.getPropertyName("perspectiveOrigin");
      },

      /**
       * Checks for the style name used to set the backface visibility.
       * @internal
       * @return {String|null} The name of the style or null, if the style is
       *   not supported.
       */
      getBackFaceVisibility: function getBackFaceVisibility() {
        return qx.bom.Style.getPropertyName("backfaceVisibility");
      },

      /**
       * Checks for the style name used to set the transform origin.
       * @internal
       * @return {String|null} The name of the style or null, if the style is
       *   not supported.
       */
      getOrigin: function getOrigin() {
        return qx.bom.Style.getPropertyName("transformOrigin");
      },

      /**
       * Checks for the style name used for transforms.
       * @internal
       * @return {String|null} The name of the style or null, if the style is
       *   not supported.
       */
      getName: function getName() {
        return qx.bom.Style.getPropertyName("transform");
      },

      /**
       * Checks if 3D transforms are supported.
       * @internal
       * @return {Boolean} <code>true</code>, if 3D transformations are supported
       */
      get3D: function get3D() {
        return qx.bom.client.CssTransform.getPerspective() != null;
      }
    },
    defer: function defer(statics) {
      qx.core.Environment.add("css.transform", statics.getSupport);
      qx.core.Environment.add("css.transform.3d", statics.get3D);
    }
  });
  qx.bom.client.CssTransform.$$dbClassInfo = $$dbClassInfo;
})();

(function () {
  var $$dbClassInfo = {
    "dependsOn": {
      "qx.core.Environment": {
        "defer": "load",
        "usage": "dynamic",
        "require": true
      },
      "qx.Bootstrap": {
        "usage": "dynamic",
        "require": true
      },
      "qx.bom.client.CssTransform": {
        "require": true
      },
      "qx.bom.Style": {}
    },
    "environment": {
      "provided": [],
      "required": {
        "css.transform": {
          "load": true,
          "className": "qx.bom.client.CssTransform"
        },
        "css.transform.3d": {
          "className": "qx.bom.client.CssTransform"
        }
      }
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
       * Martin Wittemann (wittemann)
  
  ************************************************************************ */

  /**
   * This class is responsible for applying CSS3 transforms to plain DOM elements.
   * The implementation is mostly a cross browser wrapper for applying the
   * transforms.
   * The API is keep to the spec as close as possible.
   *
   * http://www.w3.org/TR/css3-3d-transforms/
   */
  qx.Bootstrap.define("qx.bom.element.Transform", {
    statics: {
      /** Internal storage of the CSS names */
      __cssKeys__P_161_0: qx.core.Environment.get("css.transform"),

      /**
       * Method to apply multiple transforms at once to the given element. It
       * takes a map containing the transforms you want to apply plus the values
       * e.g.<code>{scale: 2, rotate: "5deg"}</code>.
       * The values can be either singular, which means a single value will
       * be added to the CSS. If you give an array, the values will be split up
       * and each array entry will be used for the X, Y or Z dimension in that
       * order e.g. <code>{scale: [2, 0.5]}</code> will result in a element
       * double the size in X direction and half the size in Y direction.
       * The values can be either singular, which means a single value will
       * be added to the CSS. If you give an array, the values will be join to
       * a string.
       * 3d suffixed properties will be taken for translate and scale if they are
       * available and an array with three values is given.
       * Make sure your browser supports all transformations you apply.
       *
       * @param el {Element} The element to apply the transformation.
       * @param transforms {Map} The map containing the transforms and value.
       */
      transform: function transform(el, transforms) {
        var transformCss = this.getTransformValue(transforms);

        if (this.__cssKeys__P_161_0 != null) {
          var style = this.__cssKeys__P_161_0["name"];
          el.style[style] = transformCss;
        }
      },

      /**
       * Translates the given element by the given value. For further details, take
       * a look at the {@link #transform} method.
       * @param el {Element} The element to apply the transformation.
       * @param value {String|Array} The value to translate e.g. <code>"10px"</code>.
       */
      translate: function translate(el, value) {
        this.transform(el, {
          translate: value
        });
      },

      /**
       * Scales the given element by the given value. For further details, take
       * a look at the {@link #transform} method.
       * @param el {Element} The element to apply the transformation.
       * @param value {Number|Array} The value to scale.
       */
      scale: function scale(el, value) {
        this.transform(el, {
          scale: value
        });
      },

      /**
       * Rotates the given element by the given value. For further details, take
       * a look at the {@link #transform} method.
       * @param el {Element} The element to apply the transformation.
       * @param value {String|Array} The value to rotate e.g. <code>"90deg"</code>.
       */
      rotate: function rotate(el, value) {
        this.transform(el, {
          rotate: value
        });
      },

      /**
       * Skews the given element by the given value. For further details, take
       * a look at the {@link #transform} method.
       * @param el {Element} The element to apply the transformation.
       * @param value {String|Array} The value to skew e.g. <code>"90deg"</code>.
       */
      skew: function skew(el, value) {
        this.transform(el, {
          skew: value
        });
      },

      /**
       * Converts the given map to a string which could be added to a css
       * stylesheet.
       * @param transforms {Map} The transforms map. For a detailed description,
       * take a look at the {@link #transform} method.
       * @return {String} The CSS value.
       */
      getCss: function getCss(transforms) {
        var transformCss = this.getTransformValue(transforms);

        if (this.__cssKeys__P_161_0 != null) {
          var style = this.__cssKeys__P_161_0["name"];
          return qx.bom.Style.getCssName(style) + ":" + transformCss + ";";
        }

        return "";
      },

      /**
       * Sets the transform-origin property of the given element.
       *
       * Spec: http://www.w3.org/TR/css3-3d-transforms/#transform-origin-property
       * @param el {Element} The dom element to set the property.
       * @param value {String} CSS position values like <code>50% 50%</code> or
       *   <code>left top</code>.
       */
      setOrigin: function setOrigin(el, value) {
        if (this.__cssKeys__P_161_0 != null) {
          el.style[this.__cssKeys__P_161_0["origin"]] = value;
        }
      },

      /**
       * Returns the transform-origin property of the given element.
       *
       * Spec: http://www.w3.org/TR/css3-3d-transforms/#transform-origin-property
       * @param el {Element} The dom element to read the property.
       * @return {String} The set property, e.g. <code>50% 50%</code>
       */
      getOrigin: function getOrigin(el) {
        if (this.__cssKeys__P_161_0 != null) {
          return el.style[this.__cssKeys__P_161_0["origin"]];
        }

        return "";
      },

      /**
       * Sets the transform-style property of the given element.
       *
       * Spec: http://www.w3.org/TR/css3-3d-transforms/#transform-style-property
       * @param el {Element} The dom element to set the property.
       * @param value {String} Either <code>flat</code> or <code>preserve-3d</code>.
       */
      setStyle: function setStyle(el, value) {
        if (this.__cssKeys__P_161_0 != null) {
          el.style[this.__cssKeys__P_161_0["style"]] = value;
        }
      },

      /**
       * Returns the transform-style property of the given element.
       *
       * Spec: http://www.w3.org/TR/css3-3d-transforms/#transform-style-property
       * @param el {Element} The dom element to read the property.
       * @return {String} The set property, either <code>flat</code> or
       *   <code>preserve-3d</code>.
       */
      getStyle: function getStyle(el) {
        if (this.__cssKeys__P_161_0 != null) {
          return el.style[this.__cssKeys__P_161_0["style"]];
        }

        return "";
      },

      /**
       * Sets the perspective property of the given element.
       *
       * Spec: http://www.w3.org/TR/css3-3d-transforms/#perspective-property
       * @param el {Element} The dom element to set the property.
       * @param value {Number} The perspective layer. Numbers between 100
       *   and 5000 give the best results.
       */
      setPerspective: function setPerspective(el, value) {
        if (this.__cssKeys__P_161_0 != null) {
          el.style[this.__cssKeys__P_161_0["perspective"]] = value + "px";
        }
      },

      /**
       * Returns the perspective property of the given element.
       *
       * Spec: http://www.w3.org/TR/css3-3d-transforms/#perspective-property
       * @param el {Element} The dom element to read the property.
       * @return {String} The set property, e.g. <code>500</code>
       */
      getPerspective: function getPerspective(el) {
        if (this.__cssKeys__P_161_0 != null) {
          return el.style[this.__cssKeys__P_161_0["perspective"]];
        }

        return "";
      },

      /**
       * Sets the perspective-origin property of the given element.
       *
       * Spec: http://www.w3.org/TR/css3-3d-transforms/#perspective-origin-property
       * @param el {Element} The dom element to set the property.
       * @param value {String} CSS position values like <code>50% 50%</code> or
       *   <code>left top</code>.
       */
      setPerspectiveOrigin: function setPerspectiveOrigin(el, value) {
        if (this.__cssKeys__P_161_0 != null) {
          el.style[this.__cssKeys__P_161_0["perspective-origin"]] = value;
        }
      },

      /**
       * Returns the perspective-origin property of the given element.
       *
       * Spec: http://www.w3.org/TR/css3-3d-transforms/#perspective-origin-property
       * @param el {Element} The dom element to read the property.
       * @return {String} The set property, e.g. <code>50% 50%</code>
       */
      getPerspectiveOrigin: function getPerspectiveOrigin(el) {
        if (this.__cssKeys__P_161_0 != null) {
          var value = el.style[this.__cssKeys__P_161_0["perspective-origin"]];

          if (value != "") {
            return value;
          } else {
            var valueX = el.style[this.__cssKeys__P_161_0["perspective-origin"] + "X"];
            var valueY = el.style[this.__cssKeys__P_161_0["perspective-origin"] + "Y"];

            if (valueX != "") {
              return valueX + " " + valueY;
            }
          }
        }

        return "";
      },

      /**
       * Sets the backface-visibility property of the given element.
       *
       * Spec: http://www.w3.org/TR/css3-3d-transforms/#backface-visibility-property
       * @param el {Element} The dom element to set the property.
       * @param value {Boolean} <code>true</code> if the backface should be visible.
       */
      setBackfaceVisibility: function setBackfaceVisibility(el, value) {
        if (this.__cssKeys__P_161_0 != null) {
          el.style[this.__cssKeys__P_161_0["backface-visibility"]] = value ? "visible" : "hidden";
        }
      },

      /**
       * Returns the backface-visibility property of the given element.
       *
       * Spec: http://www.w3.org/TR/css3-3d-transforms/#backface-visibility-property
       * @param el {Element} The dom element to read the property.
       * @return {Boolean} <code>true</code>, if the backface is visible.
       */
      getBackfaceVisibility: function getBackfaceVisibility(el) {
        if (this.__cssKeys__P_161_0 != null) {
          return el.style[this.__cssKeys__P_161_0["backface-visibility"]] == "visible";
        }

        return true;
      },

      /**
       * Converts the given transforms map to a valid CSS string.
       *
       * @param transforms {Map} A map containing the transforms.
       * @return {String} The CSS transforms.
       */
      getTransformValue: function getTransformValue(transforms) {
        var value = "";
        var properties3d = ["translate", "scale"];

        for (var property in transforms) {
          var params = transforms[property]; // if an array is given

          if (qx.Bootstrap.isArray(params)) {
            // use 3d properties for translate and scale if all 3 parameter are given
            if (params.length === 3 && properties3d.indexOf(property) > -1 && qx.core.Environment.get("css.transform.3d")) {
              value += this._compute3dProperty(property, params);
            } // use axis related properties
            else {
                value += this._computeAxisProperties(property, params);
              } // case for single values given

          } else {
            // single value case
            value += property + "(" + params + ") ";
          }
        }

        return value.trim();
      },

      /**
       * Helper function to create 3d property.
       *
       * @param property {String} Property of transform, e.g. translate
       * @param params {Array} Array with three values, each one stands for an axis.
       *
       * @return {String} Computed property and its value
       */
      _compute3dProperty: function _compute3dProperty(property, params) {
        var cssValue = "";
        property += "3d";

        for (var i = 0; i < params.length; i++) {
          if (params[i] == null) {
            params[i] = 0;
          }
        }

        cssValue += property + "(" + params.join(", ") + ") ";
        return cssValue;
      },

      /**
       * Helper function to create axis related properties.
       *
       * @param property {String} Property of transform, e.g. rotate
       * @param params {Array} Array with values, each one stands for an axis.
       *
       * @return {String} Computed property and its value
       */
      _computeAxisProperties: function _computeAxisProperties(property, params) {
        var value = "";
        var dimensions = ["X", "Y", "Z"];

        for (var i = 0; i < params.length; i++) {
          if (params[i] == null || i == 2 && !qx.core.Environment.get("css.transform.3d")) {
            continue;
          }

          value += property + dimensions[i] + "(";
          value += params[i];
          value += ") ";
        }

        return value;
      }
    }
  });
  qx.bom.element.Transform.$$dbClassInfo = $$dbClassInfo;
})();

(function () {
  var $$dbClassInfo = {
    "dependsOn": {
      "qx.Class": {
        "usage": "dynamic",
        "require": true
      },
      "qx.event.type.Event": {
        "require": true
      }
    }
  };
  qx.Bootstrap.executePendingDefers($$dbClassInfo);

  /* ************************************************************************
  
     qooxdoo - the new era of web development
  
     http://qooxdoo.org
  
     Copyright:
       2004-2010 1&1 Internet AG, Germany, http://www.1und1.de
  
     License:
       MIT: https://opensource.org/licenses/MIT
       See the LICENSE file in the project's top-level directory for details.
  
     Authors:
       * Tino Butz (tbtz)
  
     ======================================================================
  
     This class contains code based on the following work:
  
     * Unify Project
  
       Homepage:
         http://unify-project.org
  
       Copyright:
         2009-2010 Deutsche Telekom AG, Germany, http://telekom.com
  
       License:
         MIT: http://www.opensource.org/licenses/mit-license.php
  
  ************************************************************************ */

  /**
   * Orientation event object.
   */
  qx.Class.define("qx.event.type.Orientation", {
    extend: qx.event.type.Event,

    /*
    *****************************************************************************
       MEMBERS
    *****************************************************************************
    */
    members: {
      __orientation__P_162_0: null,
      __mode__P_162_1: null,

      /**
       * Initialize the fields of the event. The event must be initialized before
       * it can be dispatched.
       *
       * @param orientation {String} One of <code>0</code>, <code>90</code> or <code>-90</code>
       * @param mode {String} <code>landscape</code> or <code>portrait</code>
       * @return {qx.event.type.Orientation} The initialized event instance
       */
      init: function init(orientation, mode) {
        qx.event.type.Orientation.prototype.init.base.call(this, false, false);
        this.__orientation__P_162_0 = orientation;
        this.__mode__P_162_1 = mode;
        return this;
      },

      /**
       * Get a copy of this object
       *
       * @param embryo {qx.event.type.Orientation?null} Optional event class, which will
       *     be configured using the data of this event instance. The event must be
       *     an instance of this event class. If the data is <code>null</code>,
       *     a new pooled instance is created.
       *
       * @return {qx.event.type.Orientation} a copy of this object
       */
      clone: function clone(embryo) {
        var clone = qx.event.type.Orientation.prototype.clone.base.call(this, embryo);
        clone.__orientation__P_162_0 = this.__orientation__P_162_0;
        clone.__mode__P_162_1 = this.__mode__P_162_1;
        return clone;
      },

      /**
       * Returns the current orientation of the viewport in degree.
       *
       * All possible values and their meaning:
       *
       * * <code>0</code>: "Portrait"
       * * <code>-90</code>: "Landscape (right, screen turned clockwise)"
       * * <code>90</code>: "Landscape (left, screen turned counterclockwise)"
       * * <code>180</code>: "Portrait (upside-down portrait)"
       *
       * @return {Integer} The current orientation in degree
       */
      getOrientation: function getOrientation() {
        return this.__orientation__P_162_0;
      },

      /**
       * Whether the viewport orientation is currently in landscape mode.
       *
       * @return {Boolean} <code>true</code> when the viewport orientation
       *     is currently in landscape mode.
       */
      isLandscape: function isLandscape() {
        return this.__mode__P_162_1 == "landscape";
      },

      /**
       * Whether the viewport orientation is currently in portrait mode.
       *
       * @return {Boolean} <code>true</code> when the viewport orientation
       *     is currently in portrait mode.
       */
      isPortrait: function isPortrait() {
        return this.__mode__P_162_1 == "portrait";
      }
    }
  });
  qx.event.type.Orientation.$$dbClassInfo = $$dbClassInfo;
})();

(function () {
  var $$dbClassInfo = {
    "dependsOn": {
      "qx.Class": {
        "usage": "dynamic",
        "require": true
      },
      "qx.event.type.Dom": {
        "require": true
      }
    }
  };
  qx.Bootstrap.executePendingDefers($$dbClassInfo);

  /* ************************************************************************
  
     qooxdoo - the new era of web development
  
     http://qooxdoo.org
  
     Copyright:
       2004-2010 1&1 Internet AG, Germany, http://www.1und1.de
  
     License:
       MIT: https://opensource.org/licenses/MIT
       See the LICENSE file in the project's top-level directory for details.
  
     Authors:
       * Martin Wittemann (martinwittemann)
       * Tino Butz (tbtz)
  
  ************************************************************************ */

  /**
   * Touch event object.
   *
   * For more information see:
   *     https://developer.apple.com/library/safari/#documentation/UserExperience/Reference/TouchEventClassReference/TouchEvent/TouchEvent.html
   */
  qx.Class.define("qx.event.type.Touch", {
    extend: qx.event.type.Dom,

    /*
    *****************************************************************************
       MEMBERS
    *****************************************************************************
    */
    members: {
      // overridden
      _cloneNativeEvent: function _cloneNativeEvent(nativeEvent, clone) {
        var clone = qx.event.type.Touch.prototype._cloneNativeEvent.base.call(this, nativeEvent, clone);

        clone.pageX = nativeEvent.pageX;
        clone.pageY = nativeEvent.pageY;
        clone.offsetX = nativeEvent.offsetX;
        clone.offsetY = nativeEvent.offsetY; // Workaround for BUG #6491

        clone.layerX = nativeEvent.offsetX || nativeEvent.layerX;
        clone.layerY = nativeEvent.offsetY || nativeEvent.layerY;
        clone.scale = nativeEvent.scale;
        clone.rotation = nativeEvent.rotation;
        clone._rotation = nativeEvent._rotation;
        clone.delta = nativeEvent.delta;
        clone.srcElement = nativeEvent.srcElement;
        clone.targetTouches = [];

        for (var i = 0; i < nativeEvent.targetTouches.length; i++) {
          clone.targetTouches[i] = nativeEvent.targetTouches[i];
        }

        clone.changedTouches = [];

        for (i = 0; i < nativeEvent.changedTouches.length; i++) {
          clone.changedTouches[i] = nativeEvent.changedTouches[i];
        }

        clone.touches = [];

        for (i = 0; i < nativeEvent.touches.length; i++) {
          clone.touches[i] = nativeEvent.touches[i];
        }

        return clone;
      },
      // overridden
      stop: function stop() {
        this.stopPropagation();
      },

      /**
       * Returns an array of native Touch objects representing all current
       * touches on the document.
       * Returns an empty array for the "touchend" event.
       *
       * @return {Object[]} Array of touch objects. For more information see:
       *     https://developer.apple.com/library/safari/#documentation/UserExperience/Reference/TouchClassReference/Touch/Touch.html
       */
      getAllTouches: function getAllTouches() {
        return this._native.touches;
      },

      /**
       * Returns an array of native Touch objects representing all touches
       * associated with the event target element.
       * Returns an empty array for the "touchend" event.
       *
       * @return {Object[]} Array of touch objects. For more information see:
       *     https://developer.apple.com/library/safari/#documentation/UserExperience/Reference/TouchClassReference/Touch/Touch.html
       */
      getTargetTouches: function getTargetTouches() {
        return this._native.targetTouches;
      },

      /**
       * Returns an array of native Touch objects representing all touches of
       * the target element that changed in this event.
       *
       * On the "touchstart" event the array contains all touches that were
       * added to the target element.
       * On the "touchmove" event the array contains all touches that were
       * moved on the target element.
       * On the "touchend" event the array contains all touches that used
       * to be on the target element.
       *
       * @return {Object[]} Array of touch objects. For more information see:
       *     https://developer.apple.com/library/safari/#documentation/UserExperience/Reference/TouchClassReference/Touch/Touch.html
       */
      getChangedTargetTouches: function getChangedTargetTouches() {
        return this._native.changedTouches;
      },

      /**
       * Checks whether more than one touch is associated with the event target
       * element.
       *
       * @return {Boolean} Is multi-touch
       */
      isMultiTouch: function isMultiTouch() {
        return this.__getEventSpecificTouches__P_158_0().length > 1;
      },

      /**
       * Returns the distance between two fingers since the start of the event.
       * The distance is a multiplier of the initial distance.
       * Initial value: 1.0.
       * Gestures:
       * < 1.0, pinch close / zoom out.
       * > 1.0, pinch open / to zoom in.
       *
       * @return {Float} The scale distance between two fingers
       */
      getScale: function getScale() {
        return this._native.scale;
      },

      /**
       * Returns the delta of the rotation since the start of the event, in degrees.
       * Initial value is 0.0
       * Clockwise > 0
       * Counter-clockwise < 0.
       *
       * @return {Float} The rotation delta
       */
      getRotation: function getRotation() {
        if (typeof this._native._rotation === "undefined") {
          return this._native.rotation;
        } else {
          return this._native._rotation;
        }
      },

      /**
       * Returns an array with the calculated delta coordinates of all active touches,
       * relative to the position on <code>touchstart</code> event.
       *
       * @return {Array} an array with objects for each active touch which contains the delta as <code>x</code> and
       * <code>y</code>, the touch identifier as <code>identifier</code> and the movement axis as <code>axis</code>.
       */
      getDelta: function getDelta() {
        return this._native.delta;
      },

      /**
       * Get the horizontal position at which the event occurred relative to the
       * left of the document. This property takes into account any scrolling of
       * the page.
       *
       * @param touchIndex {Integer ? 0} The index of the Touch object
       * @return {Integer} The horizontal position of the touch in the document.
       */
      getDocumentLeft: function getDocumentLeft(touchIndex) {
        return this.__getEventSpecificTouch__P_158_1(touchIndex).pageX;
      },

      /**
       * Get the vertical position at which the event occurred relative to the
       * top of the document. This property takes into account any scrolling of
       * the page.
       *
       * @param touchIndex {Integer ? 0} The index of the Touch object
       * @return {Integer} The vertical position of the touch in the document.
       */
      getDocumentTop: function getDocumentTop(touchIndex) {
        return this.__getEventSpecificTouch__P_158_1(touchIndex).pageY;
      },

      /**
       * Get the horizontal coordinate at which the event occurred relative to
       * the origin of the screen coordinate system.
       *
       * @param touchIndex {Integer ? 0} The index of the Touch object
       * @return {Integer} The horizontal position of the touch
       */
      getScreenLeft: function getScreenLeft(touchIndex) {
        return this.__getEventSpecificTouch__P_158_1(touchIndex).screenX;
      },

      /**
       * Get the vertical coordinate at which the event occurred relative to
       * the origin of the screen coordinate system.
       *
       * @param touchIndex {Integer ? 0} The index of the Touch object
       * @return {Integer} The vertical position of the touch
       */
      getScreenTop: function getScreenTop(touchIndex) {
        return this.__getEventSpecificTouch__P_158_1(touchIndex).screenY;
      },

      /**
       * Get the the horizontal coordinate at which the event occurred relative
       * to the viewport.
       *
       * @param touchIndex {Integer ? 0} The index of the Touch object
       * @return {Integer} The horizontal position of the touch
       */
      getViewportLeft: function getViewportLeft(touchIndex) {
        return this.__getEventSpecificTouch__P_158_1(touchIndex).clientX;
      },

      /**
       * Get the vertical coordinate at which the event occurred relative
       * to the viewport.
       *
       * @param touchIndex {Integer ? 0} The index of the Touch object
       * @return {Integer} The vertical position of the touch
       */
      getViewportTop: function getViewportTop(touchIndex) {
        return this.__getEventSpecificTouch__P_158_1(touchIndex).clientY;
      },

      /**
       * Returns the unique identifier for a certain touch object.
       *
       * @param touchIndex {Integer ? 0} The index of the Touch object
       * @return {Integer} Unique identifier of the touch object
       */
      getIdentifier: function getIdentifier(touchIndex) {
        return this.__getEventSpecificTouch__P_158_1(touchIndex).identifier;
      },

      /**
       * Returns an event specific touch on the target element. This function is
       * used as the "touchend" event only offers Touch objects in the
       * changedTouches array.
       *
       * @param touchIndex {Integer ? 0} The index of the Touch object to
       *     retrieve
       * @return {Object} A native Touch object
       */
      __getEventSpecificTouch__P_158_1: function __getEventSpecificTouch__P_158_1(touchIndex) {
        touchIndex = touchIndex == null ? 0 : touchIndex;
        return this.__getEventSpecificTouches__P_158_0()[touchIndex];
      },

      /**
       * Returns the event specific touches on the target element. This function
       * is used as the "touchend" event only offers Touch objects in the
       * changedTouches array.
       *
       * @return {Object[]} Array of native Touch objects
       */
      __getEventSpecificTouches__P_158_0: function __getEventSpecificTouches__P_158_0() {
        var touches = this._isTouchEnd() ? this.getChangedTargetTouches() : this.getTargetTouches();
        return touches;
      },

      /**
       * Indicates if the event occurs during the "touchend" phase. Needed to
       * determine the event specific touches. Override this method if you derive
       * from this class and want to indicate that the specific event occurred
       * during the "touchend" phase.
       *
       * @return {Boolean} Whether the event occurred during the "touchend" phase
       */
      _isTouchEnd: function _isTouchEnd() {
        return this.getType() == "touchend" || this.getType() == "touchcancel";
      }
    }
  });
  qx.event.type.Touch.$$dbClassInfo = $$dbClassInfo;
})();

(function () {
  var $$dbClassInfo = {
    "dependsOn": {
      "qx.Bootstrap": {
        "usage": "dynamic",
        "construct": true,
        "require": true
      },
      "qx.core.IDisposable": {
        "require": true
      },
      "qx.event.Emitter": {
        "construct": true
      },
      "qx.util.Uri": {},
      "qx.bom.client.Engine": {},
      "qx.bom.client.Browser": {}
    },
    "environment": {
      "provided": ["qx.debug.io"],
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
       * Tristan Koch (tristankoch)
  
  ************************************************************************ */

  /**
   * Script loader with interface similar to
   * <a href="http://www.w3.org/TR/XMLHttpRequest/">XmlHttpRequest</a>.
   *
   * The script loader can be used to load scripts from arbitrary sources.
   * <span class="desktop">
   * For JSONP requests, consider the {@link qx.bom.request.Jsonp} transport
   * that derives from the script loader.
   * </span>
   *
   * <div class="desktop">
   * Example:
   *
   * <pre class="javascript">
   *  var req = new qx.bom.request.Script();
   *  req.onload = function() {
   *    // Script is loaded and parsed and
   *    // globals set are available
   *  }
   *
   *  req.open("GET", url);
   *  req.send();
   * </pre>
   * </div>
   *
   * @ignore(qx.core, qx.core.Environment.*)
   * @require(qx.bom.request.Script#_success)
   * @require(qx.bom.request.Script#abort)
   * @require(qx.bom.request.Script#dispose)
   * @require(qx.bom.request.Script#isDisposed)
   * @require(qx.bom.request.Script#getAllResponseHeaders)
   * @require(qx.bom.request.Script#getResponseHeader)
   * @require(qx.bom.request.Script#setDetermineSuccess)
   * @require(qx.bom.request.Script#setRequestHeader)
   *
   * @group (IO)
   */
  qx.Bootstrap.define("qx.bom.request.Script", {
    implement: [qx.core.IDisposable],
    construct: function construct() {
      this.__initXhrProperties__P_342_0();

      this.__onNativeLoadBound__P_342_1 = qx.Bootstrap.bind(this._onNativeLoad, this);
      this.__onNativeErrorBound__P_342_2 = qx.Bootstrap.bind(this._onNativeError, this);
      this.__onTimeoutBound__P_342_3 = qx.Bootstrap.bind(this._onTimeout, this);
      this.__headElement__P_342_4 = document.head || document.getElementsByTagName("head")[0] || document.documentElement;
      this._emitter = new qx.event.Emitter(); // BUGFIX: Browsers not supporting error handler
      // Set default timeout to capture network errors
      //
      // Note: The script is parsed and executed, before a "load" is fired.

      this.timeout = this.__supportsErrorHandler__P_342_5() ? 0 : 15000;
    },
    events: {
      /** Fired at ready state changes. */
      "readystatechange": "qx.bom.request.Script",

      /** Fired on error. */
      "error": "qx.bom.request.Script",

      /** Fired at loadend. */
      "loadend": "qx.bom.request.Script",

      /** Fired on timeouts. */
      "timeout": "qx.bom.request.Script",

      /** Fired when the request is aborted. */
      "abort": "qx.bom.request.Script",

      /** Fired on successful retrieval. */
      "load": "qx.bom.request.Script"
    },
    members: {
      /**
       * @type {Number} Ready state.
       *
       * States can be:
       * UNSENT:           0,
       * OPENED:           1,
       * LOADING:          2,
       * LOADING:          3,
       * DONE:             4
       *
       * Contrary to {@link qx.bom.request.Xhr#readyState}, the script transport
       * does not receive response headers. For compatibility, another LOADING
       * state is implemented that replaces the HEADERS_RECEIVED state.
       */
      readyState: null,

      /**
       * @type {Number} The status code.
       *
       * Note: The script transport cannot determine the HTTP status code.
       */
      status: null,

      /**
       * @type {String} The status text.
       *
       * The script transport does not receive response headers. For compatibility,
       * the statusText property is set to the status casted to string.
       */
      statusText: null,

      /**
       * @type {Number} Timeout limit in milliseconds.
       *
       * 0 (default) means no timeout.
       */
      timeout: null,

      /**
       * @type {Function} Function that is executed once the script was loaded.
       */
      __determineSuccess__P_342_6: null,

      /**
       * Add an event listener for the given event name.
       *
       * @param name {String} The name of the event to listen to.
       * @param listener {Function} The function to execute when the event is fired
       * @param ctx {var?} The context of the listener.
       * @return {qx.bom.request.Script} Self for chaining.
       */
      on: function on(name, listener, ctx) {
        this._emitter.on(name, listener, ctx);

        return this;
      },

      /**
       * Initializes (prepares) request.
       *
       * @param method {String}
       *   The HTTP method to use.
       *   This parameter exists for compatibility reasons. The script transport
       *   does not support methods other than GET.
       * @param url {String}
       *   The URL to which to send the request.
       */
      open: function open(method, url) {
        if (this.__disposed__P_342_7) {
          return;
        } // Reset XHR properties that may have been set by previous request


        this.__initXhrProperties__P_342_0();

        this.__abort__P_342_8 = null;
        this.__url__P_342_9 = url;

        if (this.__environmentGet__P_342_10("qx.debug.io")) {
          qx.Bootstrap.debug(qx.bom.request.Script, "Open native request with url: " + url);
        }

        this._readyStateChange(1);
      },

      /**
       * Appends a query parameter to URL.
       *
       * This method exists for compatibility reasons. The script transport
       * does not support request headers. However, many services parse query
       * parameters like request headers.
       *
       * Note: The request must be initialized before using this method.
       *
       * @param key {String}
       *  The name of the header whose value is to be set.
       * @param value {String}
       *  The value to set as the body of the header.
       * @return {qx.bom.request.Script} Self for chaining.
       */
      setRequestHeader: function setRequestHeader(key, value) {
        if (this.__disposed__P_342_7) {
          return null;
        }

        var param = {};

        if (this.readyState !== 1) {
          throw new Error("Invalid state");
        }

        param[key] = value;
        this.__url__P_342_9 = qx.util.Uri.appendParamsToUrl(this.__url__P_342_9, param);
        return this;
      },

      /**
       * Sends request.
       * @return {qx.bom.request.Script} Self for chaining.
       */
      send: function send() {
        if (this.__disposed__P_342_7) {
          return null;
        }

        var script = this.__createScriptElement__P_342_11(),
            head = this.__headElement__P_342_4,
            that = this;

        if (this.timeout > 0) {
          this.__timeoutId__P_342_12 = window.setTimeout(this.__onTimeoutBound__P_342_3, this.timeout);
        }

        if (this.__environmentGet__P_342_10("qx.debug.io")) {
          qx.Bootstrap.debug(qx.bom.request.Script, "Send native request");
        } // Attach script to DOM


        head.insertBefore(script, head.firstChild); // The resource is loaded once the script is in DOM.
        // Assume HEADERS_RECEIVED and LOADING and dispatch async.

        window.setTimeout(function () {
          that._readyStateChange(2);

          that._readyStateChange(3);
        });
        return this;
      },

      /**
       * Aborts request.
       * @return {qx.bom.request.Script} Self for chaining.
       */
      abort: function abort() {
        if (this.__disposed__P_342_7) {
          return null;
        }

        this.__abort__P_342_8 = true;

        this.__disposeScriptElement__P_342_13();

        this._emit("abort");

        return this;
      },

      /**
       * Helper to emit events and call the callback methods.
       * @param event {String} The name of the event.
       */
      _emit: function _emit(event) {
        this["on" + event]();

        this._emitter.emit(event, this);
      },

      /**
       * Event handler for an event that fires at every state change.
       *
       * Replace with custom method to get informed about the communication progress.
       */
      onreadystatechange: function onreadystatechange() {},

      /**
       * Event handler for XHR event "load" that is fired on successful retrieval.
       *
       * Note: This handler is called even when an invalid script is returned.
       *
       * Warning: Internet Explorer < 9 receives a false "load" for invalid URLs.
       * This "load" is fired about 2 seconds after sending the request. To
       * distinguish from a real "load", consider defining a custom check
       * function using {@link #setDetermineSuccess} and query the status
       * property. However, the script loaded needs to have a known impact on
       * the global namespace. If this does not work for you, you may be able
       * to set a timeout lower than 2 seconds, depending on script size,
       * complexity and execution time.
       *
       * Replace with custom method to listen to the "load" event.
       */
      onload: function onload() {},

      /**
       * Event handler for XHR event "loadend" that is fired on retrieval.
       *
       * Note: This handler is called even when a network error (or similar)
       * occurred.
       *
       * Replace with custom method to listen to the "loadend" event.
       */
      onloadend: function onloadend() {},

      /**
       * Event handler for XHR event "error" that is fired on a network error.
       *
       * Note: Some browsers do not support the "error" event.
       *
       * Replace with custom method to listen to the "error" event.
       */
      onerror: function onerror() {},

      /**
      * Event handler for XHR event "abort" that is fired when request
      * is aborted.
      *
      * Replace with custom method to listen to the "abort" event.
      */
      onabort: function onabort() {},

      /**
      * Event handler for XHR event "timeout" that is fired when timeout
      * interval has passed.
      *
      * Replace with custom method to listen to the "timeout" event.
      */
      ontimeout: function ontimeout() {},

      /**
       * Get a single response header from response.
       *
       * Note: This method exists for compatibility reasons. The script
       * transport does not receive response headers.
       *
       * @param key {String}
       *  Key of the header to get the value from.
       * @return {String|null} Warning message or <code>null</code> if the request
       * is disposed
       */
      getResponseHeader: function getResponseHeader(key) {
        if (this.__disposed__P_342_7) {
          return null;
        }

        if (this.__environmentGet__P_342_10("qx.debug")) {
          qx.Bootstrap.debug("Response header cannot be determined for requests made with script transport.");
        }

        return "unknown";
      },

      /**
       * Get all response headers from response.
       *
       * Note: This method exists for compatibility reasons. The script
       * transport does not receive response headers.
       * @return {String|null} Warning message or <code>null</code> if the request
       * is disposed
       */
      getAllResponseHeaders: function getAllResponseHeaders() {
        if (this.__disposed__P_342_7) {
          return null;
        }

        if (this.__environmentGet__P_342_10("qx.debug")) {
          qx.Bootstrap.debug("Response headers cannot be determined forrequests made with script transport.");
        }

        return "Unknown response headers";
      },

      /**
       * Determine if loaded script has expected impact on global namespace.
       *
       * The function is called once the script was loaded and must return a
       * boolean indicating if the response is to be considered successful.
       *
       * @param check {Function} Function executed once the script was loaded.
       *
       */
      setDetermineSuccess: function setDetermineSuccess(check) {
        this.__determineSuccess__P_342_6 = check;
      },

      /**
       * Dispose object.
       */
      dispose: function dispose() {
        var script = this.__scriptElement__P_342_14;

        if (!this.__disposed__P_342_7) {
          // Prevent memory leaks
          if (script) {
            script.onload = script.onreadystatechange = null;

            this.__disposeScriptElement__P_342_13();
          }

          if (this.__timeoutId__P_342_12) {
            window.clearTimeout(this.__timeoutId__P_342_12);
          }

          this.__disposed__P_342_7 = true;
        }
      },

      /**
       * Check if the request has already beed disposed.
       * @return {Boolean} <code>true</code>, if the request has been disposed.
       */
      isDisposed: function isDisposed() {
        return !!this.__disposed__P_342_7;
      },

      /*
      ---------------------------------------------------------------------------
        PROTECTED
      ---------------------------------------------------------------------------
      */

      /**
       * Get URL of request.
       *
       * @return {String} URL of request.
       */
      _getUrl: function _getUrl() {
        return this.__url__P_342_9;
      },

      /**
       * Get script element used for request.
       *
       * @return {Element} Script element.
       */
      _getScriptElement: function _getScriptElement() {
        return this.__scriptElement__P_342_14;
      },

      /**
       * Handle timeout.
       */
      _onTimeout: function _onTimeout() {
        this.__failure__P_342_15();

        if (!this.__supportsErrorHandler__P_342_5()) {
          this._emit("error");
        }

        this._emit("timeout");

        if (!this.__supportsErrorHandler__P_342_5()) {
          this._emit("loadend");
        }
      },

      /**
       * Handle native load.
       */
      _onNativeLoad: function _onNativeLoad() {
        var script = this.__scriptElement__P_342_14,
            determineSuccess = this.__determineSuccess__P_342_6,
            that = this; // Aborted request must not fire load

        if (this.__abort__P_342_8) {
          return;
        } // BUGFIX: IE < 9
        // When handling "readystatechange" event, skip if readyState
        // does not signal loaded script


        if (this.__environmentGet__P_342_10("engine.name") === "mshtml" && this.__environmentGet__P_342_10("browser.documentmode") < 9) {
          if (!/loaded|complete/.test(script.readyState)) {
            return;
          } else {
            if (this.__environmentGet__P_342_10("qx.debug.io")) {
              qx.Bootstrap.debug(qx.bom.request.Script, "Received native readyState: loaded");
            }
          }
        }

        if (this.__environmentGet__P_342_10("qx.debug.io")) {
          qx.Bootstrap.debug(qx.bom.request.Script, "Received native load");
        } // Determine status by calling user-provided check function


        if (determineSuccess) {
          // Status set before has higher precedence
          if (!this.status) {
            this.status = determineSuccess() ? 200 : 500;
          }
        }

        if (this.status === 500) {
          if (this.__environmentGet__P_342_10("qx.debug.io")) {
            qx.Bootstrap.debug(qx.bom.request.Script, "Detected error");
          }
        }

        if (this.__timeoutId__P_342_12) {
          window.clearTimeout(this.__timeoutId__P_342_12);
        }

        window.setTimeout(function () {
          that._success();

          that._readyStateChange(4);

          that._emit("load");

          that._emit("loadend");
        });
      },

      /**
       * Handle native error.
       */
      _onNativeError: function _onNativeError() {
        this.__failure__P_342_15();

        this._emit("error");

        this._emit("loadend");
      },

      /*
      ---------------------------------------------------------------------------
        PRIVATE
      ---------------------------------------------------------------------------
      */

      /**
       * @type {Element} Script element
       */
      __scriptElement__P_342_14: null,

      /**
       * @type {Element} Head element
       */
      __headElement__P_342_4: null,

      /**
       * @type {String} URL
       */
      __url__P_342_9: "",

      /**
       * @type {Function} Bound _onNativeLoad handler.
       */
      __onNativeLoadBound__P_342_1: null,

      /**
       * @type {Function} Bound _onNativeError handler.
       */
      __onNativeErrorBound__P_342_2: null,

      /**
       * @type {Function} Bound _onTimeout handler.
       */
      __onTimeoutBound__P_342_3: null,

      /**
       * @type {Number} Timeout timer iD.
       */
      __timeoutId__P_342_12: null,

      /**
       * @type {Boolean} Whether request was aborted.
       */
      __abort__P_342_8: null,

      /**
       * @type {Boolean} Whether request was disposed.
       */
      __disposed__P_342_7: null,

      /*
      ---------------------------------------------------------------------------
        HELPER
      ---------------------------------------------------------------------------
      */

      /**
       * Initialize properties.
       */
      __initXhrProperties__P_342_0: function __initXhrProperties__P_342_0() {
        this.readyState = 0;
        this.status = 0;
        this.statusText = "";
      },

      /**
       * Change readyState.
       *
       * @param readyState {Number} The desired readyState
       */
      _readyStateChange: function _readyStateChange(readyState) {
        this.readyState = readyState;

        this._emit("readystatechange");
      },

      /**
       * Handle success.
       */
      _success: function _success() {
        this.__disposeScriptElement__P_342_13();

        this.readyState = 4; // By default, load is considered successful

        if (!this.status) {
          this.status = 200;
        }

        this.statusText = "" + this.status;
      },

      /**
       * Handle failure.
       */
      __failure__P_342_15: function __failure__P_342_15() {
        this.__disposeScriptElement__P_342_13();

        this.readyState = 4;
        this.status = 0;
        this.statusText = null;
      },

      /**
       * Looks up whether browser supports error handler.
       *
       * @return {Boolean} Whether browser supports error handler.
       */
      __supportsErrorHandler__P_342_5: function __supportsErrorHandler__P_342_5() {
        var isLegacyIe = this.__environmentGet__P_342_10("engine.name") === "mshtml" && this.__environmentGet__P_342_10("browser.documentmode") < 9;
        var isOpera = this.__environmentGet__P_342_10("engine.name") === "opera";
        return !(isLegacyIe || isOpera);
      },

      /**
       * Create and configure script element.
       *
       * @return {Element} Configured script element.
       */
      __createScriptElement__P_342_11: function __createScriptElement__P_342_11() {
        var script = this.__scriptElement__P_342_14 = document.createElement("script");
        script.src = this.__url__P_342_9;
        script.onerror = this.__onNativeErrorBound__P_342_2;
        script.onload = this.__onNativeLoadBound__P_342_1; // BUGFIX: IE < 9
        // Legacy IEs do not fire the "load" event for script elements.
        // Instead, they support the "readystatechange" event

        if (this.__environmentGet__P_342_10("engine.name") === "mshtml" && this.__environmentGet__P_342_10("browser.documentmode") < 9) {
          script.onreadystatechange = this.__onNativeLoadBound__P_342_1;
        }

        return script;
      },

      /**
       * Remove script element from DOM.
       */
      __disposeScriptElement__P_342_13: function __disposeScriptElement__P_342_13() {
        var script = this.__scriptElement__P_342_14;

        if (script && script.parentNode) {
          this.__headElement__P_342_4.removeChild(script);
        }
      },

      /**
       * Proxy Environment.get to guard against env not being present yet.
       *
       * @param key {String} Environment key.
       * @return {var} Value of the queried environment key
       * @lint environmentNonLiteralKey(key)
       */
      __environmentGet__P_342_10: function __environmentGet__P_342_10(key) {
        if (qx && qx.core && qx.core.Environment) {
          return qx.core.Environment.get(key);
        } else {
          if (key === "engine.name") {
            return qx.bom.client.Engine.getName();
          }

          if (key === "browser.documentmode") {
            return qx.bom.client.Browser.getDocumentMode();
          }

          if (key == "qx.debug.io") {
            return false;
          }

          throw new Error("Unknown environment key at this phase");
        }
      }
    },
    defer: function defer() {
      if (qx && qx.core && qx.core.Environment) {
        qx.core.Environment.add("qx.debug.io", false);
      }
    }
  });
  qx.bom.request.Script.$$dbClassInfo = $$dbClassInfo;
})();

(function () {
  var $$dbClassInfo = {
    "dependsOn": {
      "qx.core.Environment": {
        "defer": "load",
        "usage": "dynamic",
        "require": true
      },
      "qx.Bootstrap": {
        "usage": "dynamic",
        "require": true
      },
      "qx.bom.client.Engine": {
        "require": true
      },
      "qx.lang.Array": {},
      "qx.type.BaseArray": {}
    },
    "environment": {
      "provided": [],
      "required": {
        "engine.name": {
          "load": true,
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
       2009 Sebastian Werner, http://sebastian-werner.net
  
     License:
       MIT: https://opensource.org/licenses/MIT
       See the LICENSE file in the project's top-level directory for details.
  
     Authors:
       * Sebastian Werner (wpbasti)
  
     ======================================================================
  
     This class contains code based on the following work:
  
     * jQuery
       http://jquery.com
       Version 1.3.1
  
       Copyright:
         2009 John Resig
  
       License:
         MIT: http://www.opensource.org/licenses/mit-license.php
  
  ************************************************************************ */

  /**
   * This class is mainly a convenience wrapper for DOM elements to
   * qooxdoo's event system.
   *
   * @ignore(qxWeb)
   */
  qx.Bootstrap.define("qx.bom.Html", {
    /*
    *****************************************************************************
       STATICS
    *****************************************************************************
    */
    statics: {
      /**
       * Helper method for XHTML replacement.
       *
       * @param all {String} Complete string
       * @param front {String} Front of the match
       * @param tag {String} Tag name
       * @return {String} XHTML corrected tag
       */
      __fixNonDirectlyClosableHelper__P_347_0: function __fixNonDirectlyClosableHelper__P_347_0(all, front, tag) {
        return tag.match(/^(abbr|br|col|img|input|link|meta|param|hr|area|embed)$/i) ? all : front + "></" + tag + ">";
      },

      /** @type {Map} Contains wrap fragments for specific HTML matches */
      __convertMap__P_347_1: {
        opt: [1, "<select multiple='multiple'>", "</select>"],
        // option or optgroup
        leg: [1, "<fieldset>", "</fieldset>"],
        table: [1, "<table>", "</table>"],
        tr: [2, "<table><tbody>", "</tbody></table>"],
        td: [3, "<table><tbody><tr>", "</tr></tbody></table>"],
        col: [2, "<table><tbody></tbody><colgroup>", "</colgroup></table>"],
        def: qx.core.Environment.select("engine.name", {
          "mshtml": [1, "div<div>", "</div>"],
          "default": null
        })
      },

      /**
       * Fixes "XHTML"-style tags in all browsers.
       * Replaces tags which are not allowed to be closed directly such as
       * <code>div</code> or <code>p</code>. They are patched to use opening and
       * closing tags instead, e.g. <code>&lt;p&gt;</code> => <code>&lt;p&gt;&lt;/p&gt;</code>
       *
       * @param html {String} HTML to fix
       * @return {String} Fixed HTML
       */
      fixEmptyTags: function fixEmptyTags(html) {
        return html.replace(/(<(\w+)[^>]*?)\/>/g, this.__fixNonDirectlyClosableHelper__P_347_0);
      },

      /**
       * Translates a HTML string into an array of elements.
       *
       * @param html {String} HTML string
       * @param context {Document} Context document in which (helper) elements should be created
       * @return {Array} List of resulting elements
       */
      __convertHtmlString__P_347_2: function __convertHtmlString__P_347_2(html, context) {
        var div = context.createElement("div");
        html = qx.bom.Html.fixEmptyTags(html); // Trim whitespace, otherwise indexOf won't work as expected

        var tags = html.replace(/^\s+/, "").substring(0, 5).toLowerCase(); // Auto-wrap content into required DOM structure

        var wrap,
            map = this.__convertMap__P_347_1;

        if (!tags.indexOf("<opt")) {
          wrap = map.opt;
        } else if (!tags.indexOf("<leg")) {
          wrap = map.leg;
        } else if (tags.match(/^<(thead|tbody|tfoot|colg|cap)/)) {
          wrap = map.table;
        } else if (!tags.indexOf("<tr")) {
          wrap = map.tr;
        } else if (!tags.indexOf("<td") || !tags.indexOf("<th")) {
          wrap = map.td;
        } else if (!tags.indexOf("<col")) {
          wrap = map.col;
        } else {
          wrap = map.def;
        } // Omit string concat when no wrapping is needed


        if (wrap) {
          // Go to html and back, then peel off extra wrappers
          div.innerHTML = wrap[1] + html + wrap[2]; // Move to the right depth

          var depth = wrap[0];

          while (depth--) {
            div = div.lastChild;
          }
        } else {
          div.innerHTML = html;
        } // Fix IE specific bugs


        if (qx.core.Environment.get("engine.name") == "mshtml") {
          // Remove IE's autoinserted <tbody> from table fragments
          // String was a <table>, *may* have spurious <tbody>
          var hasBody = /<tbody/i.test(html); // String was a bare <thead> or <tfoot>

          var tbody = !tags.indexOf("<table") && !hasBody ? div.firstChild && div.firstChild.childNodes : wrap[1] == "<table>" && !hasBody ? div.childNodes : [];

          for (var j = tbody.length - 1; j >= 0; --j) {
            if (tbody[j].tagName.toLowerCase() === "tbody" && !tbody[j].childNodes.length) {
              tbody[j].parentNode.removeChild(tbody[j]);
            }
          } // IE completely kills leading whitespace when innerHTML is used


          if (/^\s/.test(html)) {
            div.insertBefore(context.createTextNode(html.match(/^\s*/)[0]), div.firstChild);
          }
        }

        return qx.lang.Array.fromCollection(div.childNodes);
      },

      /**
       * Cleans-up the given HTML and append it to a fragment
       *
       * When no <code>context</code> is given the global document is used to
       * create new DOM elements.
       *
       * When a <code>fragment</code> is given the nodes are appended to this
       * fragment except the script tags. These are returned in a separate Array.
       *
       * Please note: HTML coming from user input must be validated prior
       * to passing it to this method. HTML is temporarily inserted to the DOM
       * using <code>innerHTML</code>. As a consequence, scripts included in
       * attribute event handlers may be executed.
       *
       * @param objs {Element[]|String[]} Array of DOM elements or HTML strings
       * @param context {Document?document} Context in which the elements should be created
       * @param fragment {Element?null} Document fragment to appends elements to
       * @return {Element[]} Array of elements (when a fragment is given it only contains script elements)
       */
      clean: function clean(objs, context, fragment) {
        context = context || document; // !context.createElement fails in IE with an error but returns typeof 'object'

        if (typeof context.createElement === "undefined") {
          context = context.ownerDocument || context[0] && context[0].ownerDocument || document;
        } // Fast-Path:
        // If a single string is passed in and it's a single tag
        // just do a createElement and skip the rest


        if (!fragment && objs.length === 1 && typeof objs[0] === "string") {
          var match = /^<(\w+)\s*\/?>$/.exec(objs[0]);

          if (match) {
            return [context.createElement(match[1])];
          }
        } // Iterate through items in incoming array


        var obj,
            ret = [];

        for (var i = 0, l = objs.length; i < l; i++) {
          obj = objs[i]; // Convert HTML string into DOM nodes

          if (typeof obj === "string") {
            obj = this.__convertHtmlString__P_347_2(obj, context);
          } // Append or merge depending on type


          if (obj.nodeType) {
            ret.push(obj);
          } else if (obj instanceof qx.type.BaseArray || typeof qxWeb !== "undefined" && obj instanceof qxWeb) {
            ret.push.apply(ret, Array.prototype.slice.call(obj, 0));
          } else if (obj.toElement) {
            ret.push(obj.toElement());
          } else {
            ret.push.apply(ret, obj);
          }
        } // Append to fragment and filter out scripts... or...


        if (fragment) {
          return qx.bom.Html.extractScripts(ret, fragment);
        } // Otherwise return the array of all elements


        return ret;
      },

      /**
       * Extracts script elements from an element list. Optionally
       * attaches them to a given document fragment
       *
       * @param elements {Element[]} list of elements
       * @param fragment {Document?} document fragment
       * @return {Element[]} Array containing the script elements
       */
      extractScripts: function extractScripts(elements, fragment) {
        var scripts = [],
            elem;

        for (var i = 0; elements[i]; i++) {
          elem = elements[i];

          if (elem.nodeType == 1 && elem.tagName.toLowerCase() === "script" && (!elem.type || elem.type.toLowerCase() === "text/javascript")) {
            // Trying to remove the element from DOM
            if (elem.parentNode) {
              elem.parentNode.removeChild(elements[i]);
            } // Store in script list


            scripts.push(elem);
          } else {
            if (elem.nodeType === 1) {
              // Recursively search for scripts and append them to the list of elements to process
              var scriptList = qx.lang.Array.fromCollection(elem.getElementsByTagName("script"));
              elements.splice.apply(elements, [i + 1, 0].concat(scriptList));
            } // Finally append element to fragment


            if (fragment) {
              fragment.appendChild(elem);
            }
          }
        }

        return scripts;
      }
    }
  });
  qx.bom.Html.$$dbClassInfo = $$dbClassInfo;
})();

(function () {
  var $$dbClassInfo = {
    "dependsOn": {
      "qx.bom.Stylesheet": {
        "require": true,
        "defer": "runtime"
      },
      "qx.Bootstrap": {
        "usage": "dynamic",
        "require": true
      },
      "qx.bom.Style": {},
      "qx.bom.Event": {},
      "qx.core.Environment": {
        "defer": "runtime"
      }
    },
    "environment": {
      "provided": ["css.animation", "css.animation.requestframe"],
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
       * Martin Wittemann (wittemann)
  
  ************************************************************************ */

  /**
   * Responsible for checking all relevant animation properties.
   *
   * Spec: http://www.w3.org/TR/css3-animations/
   *
   * @require(qx.bom.Stylesheet)
   * @internal
   */
  qx.Bootstrap.define("qx.bom.client.CssAnimation", {
    statics: {
      /**
       * Main check method which returns an object if CSS animations are
       * supported. This object contains all necessary keys to work with CSS
       * animations.
       * <ul>
       *  <li><code>name</code> The name of the css animation style</li>
       *  <li><code>play-state</code> The name of the play-state style</li>
       *  <li><code>start-event</code> The name of the start event</li>
       *  <li><code>iteration-event</code> The name of the iteration event</li>
       *  <li><code>end-event</code> The name of the end event</li>
       *  <li><code>fill-mode</code> The fill-mode style</li>
       *  <li><code>keyframes</code> The name of the keyframes selector.</li>
       * </ul>
       *
       * @internal
       * @return {Object|null} The described object or null, if animations are
       *   not supported.
       */
      getSupport: function getSupport() {
        var name = qx.bom.client.CssAnimation.getName();

        if (name != null) {
          return {
            "name": name,
            "play-state": qx.bom.client.CssAnimation.getPlayState(),
            "start-event": qx.bom.client.CssAnimation.getAnimationStart(),
            "iteration-event": qx.bom.client.CssAnimation.getAnimationIteration(),
            "end-event": qx.bom.client.CssAnimation.getAnimationEnd(),
            "fill-mode": qx.bom.client.CssAnimation.getFillMode(),
            "keyframes": qx.bom.client.CssAnimation.getKeyFrames()
          };
        }

        return null;
      },

      /**
       * Checks for the 'animation-fill-mode' CSS style.
       * @internal
       * @return {String|null} The name of the style or null, if the style is
       *   not supported.
       */
      getFillMode: function getFillMode() {
        return qx.bom.Style.getPropertyName("AnimationFillMode");
      },

      /**
       * Checks for the 'animation-play-state' CSS style.
       * @internal
       * @return {String|null} The name of the style or null, if the style is
       *   not supported.
       */
      getPlayState: function getPlayState() {
        return qx.bom.Style.getPropertyName("AnimationPlayState");
      },

      /**
       * Checks for the style name used for animations.
       * @internal
       * @return {String|null} The name of the style or null, if the style is
       *   not supported.
       */
      getName: function getName() {
        return qx.bom.Style.getPropertyName("animation");
      },

      /**
       * Checks for the event name of animation start.
       * @internal
       * @return {String} The name of the event.
       */
      getAnimationStart: function getAnimationStart() {
        // special handling for mixed prefixed / unprefixed implementations
        if (qx.bom.Event.supportsEvent(window, "webkitanimationstart")) {
          return "webkitAnimationStart";
        }

        var mapping = {
          "msAnimation": "MSAnimationStart",
          "WebkitAnimation": "webkitAnimationStart",
          "MozAnimation": "animationstart",
          "OAnimation": "oAnimationStart",
          "animation": "animationstart"
        };
        return mapping[this.getName()];
      },

      /**
       * Checks for the event name of animation end.
       * @internal
       * @return {String} The name of the event.
       */
      getAnimationIteration: function getAnimationIteration() {
        // special handling for mixed prefixed / unprefixed implementations
        if (qx.bom.Event.supportsEvent(window, "webkitanimationiteration")) {
          return "webkitAnimationIteration";
        }

        var mapping = {
          "msAnimation": "MSAnimationIteration",
          "WebkitAnimation": "webkitAnimationIteration",
          "MozAnimation": "animationiteration",
          "OAnimation": "oAnimationIteration",
          "animation": "animationiteration"
        };
        return mapping[this.getName()];
      },

      /**
       * Checks for the event name of animation end.
       * @internal
       * @return {String} The name of the event.
       */
      getAnimationEnd: function getAnimationEnd() {
        // special handling for mixed prefixed / unprefixed implementations
        if (qx.bom.Event.supportsEvent(window, "webkitanimationend")) {
          return "webkitAnimationEnd";
        }

        var mapping = {
          "msAnimation": "MSAnimationEnd",
          "WebkitAnimation": "webkitAnimationEnd",
          "MozAnimation": "animationend",
          "OAnimation": "oAnimationEnd",
          "animation": "animationend"
        };
        return mapping[this.getName()];
      },

      /**
       * Checks what selector should be used to add keyframes to stylesheets.
       * @internal
       * @return {String|null} The name of the selector or null, if the selector
       *   is not supported.
       */
      getKeyFrames: function getKeyFrames() {
        var prefixes = qx.bom.Style.VENDOR_PREFIXES;
        var keyFrames = [];

        for (var i = 0; i < prefixes.length; i++) {
          var key = "@" + qx.bom.Style.getCssName(prefixes[i]) + "-keyframes";
          keyFrames.push(key);
        }

        ;
        keyFrames.unshift("@keyframes");
        var sheet = qx.bom.Stylesheet.createElement();

        for (var i = 0; i < keyFrames.length; i++) {
          try {
            qx.bom.Stylesheet.addRule(sheet, keyFrames[i] + " name", "");
            return keyFrames[i];
          } catch (e) {}
        }

        ;
        return null;
      },

      /**
       * Checks for the requestAnimationFrame method and return the prefixed name.
       * @internal
       * @return {String|null} A string the method name or null, if the method
       *   is not supported.
       */
      getRequestAnimationFrame: function getRequestAnimationFrame() {
        var choices = ["requestAnimationFrame", "msRequestAnimationFrame", "webkitRequestAnimationFrame", "mozRequestAnimationFrame", "oRequestAnimationFrame" // currently unspecified, so we guess the name!
        ];

        for (var i = 0; i < choices.length; i++) {
          if (window[choices[i]] != undefined) {
            return choices[i];
          }
        }

        ;
        return null;
      }
    },
    defer: function defer(statics) {
      qx.core.Environment.add("css.animation", statics.getSupport);
      qx.core.Environment.add("css.animation.requestframe", statics.getRequestAnimationFrame);
    }
  });
  qx.bom.client.CssAnimation.$$dbClassInfo = $$dbClassInfo;
})();

(function () {
  var $$dbClassInfo = {
    "dependsOn": {
      "qx.core.Environment": {
        "defer": "load",
        "usage": "dynamic",
        "require": true
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
      "qx.core.IDisposable": {
        "require": true
      },
      "qx.lang.Function": {
        "construct": true
      },
      "qx.event.Registration": {
        "defer": "runtime",
        "require": true
      },
      "qx.bom.client.Engine": {
        "require": true
      },
      "qx.core.ObjectRegistry": {},
      "qx.bom.Event": {},
      "qx.event.GlobalError": {
        "usage": "dynamic",
        "require": true
      },
      "qx.event.type.Event": {},
      "qx.bom.client.CssAnimation": {
        "defer": "runtime"
      },
      "qx.bom.client.CssTransition": {
        "defer": "runtime"
      }
    },
    "environment": {
      "provided": [],
      "required": {
        "engine.name": {
          "load": true,
          "className": "qx.bom.client.Engine"
        },
        "css.animation": {
          "defer": true,
          "className": "qx.bom.client.CssAnimation"
        },
        "css.transition": {
          "defer": true,
          "className": "qx.bom.client.CssTransition"
        }
      }
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
  
     ======================================================================
  
     This class contains code based on the following work:
  
     * Unify Project
  
       Homepage:
         http://unify-project.org
  
       Copyright:
         2009-2010 Deutsche Telekom AG, Germany, http://telekom.com
  
       License:
         MIT: http://www.opensource.org/licenses/mit-license.php
  
  ************************************************************************ */

  /**
   *
   * This class provides support for HTML5 transition and animation events.
   * Currently only WebKit and Firefox are supported.
   * 
   * NOTE: Instances of this class must be disposed of after use
   *
   */
  qx.Class.define("qx.event.handler.Transition", {
    extend: qx.core.Object,
    implement: [qx.event.IEventHandler, qx.core.IDisposable],

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
      qx.core.Object.constructor.call(this);
      this.__registeredEvents__P_344_0 = {};
      this.__onEventWrapper__P_344_1 = qx.lang.Function.listener(this._onNative, this);
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
        transitionEnd: 1,
        animationEnd: 1,
        animationStart: 1,
        animationIteration: 1
      },

      /** @type {Integer} Which target check to use */
      TARGET_CHECK: qx.event.IEventHandler.TARGET_DOMNODE,

      /** @type {Integer} Whether the method "canHandleEvent" must be called */
      IGNORE_CAN_HANDLE: true,

      /** Mapping of supported event types to native event types */
      TYPE_TO_NATIVE: null,

      /** Mapping of native event types to supported event types */
      NATIVE_TO_TYPE: null
    },

    /*
    *****************************************************************************
       MEMBERS
    *****************************************************************************
    */
    members: {
      __onEventWrapper__P_344_1: null,
      __registeredEvents__P_344_0: null,

      /*
      ---------------------------------------------------------------------------
        EVENT HANDLER INTERFACE
      ---------------------------------------------------------------------------
      */
      // interface implementation
      canHandleEvent: function canHandleEvent(target, type) {// Nothing needs to be done here
      },
      // interface implementation

      /**
       * This method is called each time an event listener, for one of the
       * supported events, is added using {@link qx.event.Manager#addListener}.
       *
       * @param target {var} The target to, which the event handler should
       *     be attached
       * @param type {String} event type
       * @param capture {Boolean} Whether to attach the event to the
       *         capturing phase or the bubbling phase of the event.
       * @signature function(target, type, capture)
       */
      registerEvent: qx.core.Environment.select("engine.name", {
        "webkit": function webkit(target, type, capture) {
          var hash = qx.core.ObjectRegistry.toHashCode(target) + type;
          var nativeType = qx.event.handler.Transition.TYPE_TO_NATIVE[type];
          this.__registeredEvents__P_344_0[hash] = {
            target: target,
            type: nativeType
          };
          qx.bom.Event.addNativeListener(target, nativeType, this.__onEventWrapper__P_344_1);
        },
        "gecko": function gecko(target, type, capture) {
          var hash = qx.core.ObjectRegistry.toHashCode(target) + type;
          var nativeType = qx.event.handler.Transition.TYPE_TO_NATIVE[type];
          this.__registeredEvents__P_344_0[hash] = {
            target: target,
            type: nativeType
          };
          qx.bom.Event.addNativeListener(target, nativeType, this.__onEventWrapper__P_344_1);
        },
        "mshtml": function mshtml(target, type, capture) {
          var hash = qx.core.ObjectRegistry.toHashCode(target) + type;
          var nativeType = qx.event.handler.Transition.TYPE_TO_NATIVE[type];
          this.__registeredEvents__P_344_0[hash] = {
            target: target,
            type: nativeType
          };
          qx.bom.Event.addNativeListener(target, nativeType, this.__onEventWrapper__P_344_1);
        },
        "default": function _default() {}
      }),
      // interface implementation

      /**
       * This method is called each time an event listener, for one of the
       * supported events, is removed by using {@link qx.event.Manager#removeListener}
       * and no other event listener is listening on this type.
       *
       * @param target {var} The target from, which the event handler should
       *     be removed
       * @param type {String} event type
       * @param capture {Boolean} Whether to attach the event to the
       *         capturing phase or the bubbling phase of the event.
       * @signature function(target, type, capture)
       */
      unregisterEvent: qx.core.Environment.select("engine.name", {
        "webkit": function webkit(target, type, capture) {
          var events = this.__registeredEvents__P_344_0;

          if (!events) {
            return;
          }

          var hash = qx.core.ObjectRegistry.toHashCode(target) + type;

          if (events[hash]) {
            delete events[hash];
          }

          qx.bom.Event.removeNativeListener(target, qx.event.handler.Transition.TYPE_TO_NATIVE[type], this.__onEventWrapper__P_344_1);
        },
        "gecko": function gecko(target, type, capture) {
          var events = this.__registeredEvents__P_344_0;

          if (!events) {
            return;
          }

          var hash = qx.core.ObjectRegistry.toHashCode(target) + type;

          if (events[hash]) {
            delete events[hash];
          }

          qx.bom.Event.removeNativeListener(target, qx.event.handler.Transition.TYPE_TO_NATIVE[type], this.__onEventWrapper__P_344_1);
        },
        "mshtml": function mshtml(target, type, capture) {
          var events = this.__registeredEvents__P_344_0;

          if (!events) {
            return;
          }

          var hash = qx.core.ObjectRegistry.toHashCode(target) + type;

          if (events[hash]) {
            delete events[hash];
          }

          qx.bom.Event.removeNativeListener(target, qx.event.handler.Transition.TYPE_TO_NATIVE[type], this.__onEventWrapper__P_344_1);
        },
        "default": function _default() {}
      }),

      /*
      ---------------------------------------------------------------------------
        EVENT-HANDLER
      ---------------------------------------------------------------------------
      */

      /**
       * Global handler for the transition event.
       *
       * @signature function(domEvent)
       * @param domEvent {Event} DOM event
       */
      _onNative: qx.event.GlobalError.observeMethod(function (nativeEvent) {
        qx.event.Registration.fireEvent(nativeEvent.target, qx.event.handler.Transition.NATIVE_TO_TYPE[nativeEvent.type], qx.event.type.Event);
      })
    },

    /*
    *****************************************************************************
       DESTRUCTOR
    *****************************************************************************
    */
    destruct: function destruct() {
      var event;
      var events = this.__registeredEvents__P_344_0;

      for (var id in events) {
        event = events[id];

        if (event.target) {
          qx.bom.Event.removeNativeListener(event.target, event.type, this.__onEventWrapper__P_344_1);
        }
      }

      this.__registeredEvents__P_344_0 = this.__onEventWrapper__P_344_1 = null;
    },

    /*
    *****************************************************************************
       DEFER
    *****************************************************************************
    */
    defer: function defer(statics) {
      var aniEnv = qx.core.Environment.get("css.animation") || {};
      var transEnv = qx.core.Environment.get("css.transition") || {};
      var n2t = qx.event.handler.Transition.NATIVE_TO_TYPE = {};
      var t2n = qx.event.handler.Transition.TYPE_TO_NATIVE = {
        transitionEnd: transEnv["end-event"] || null,
        animationStart: aniEnv["start-event"] || null,
        animationEnd: aniEnv["end-event"] || null,
        animationIteration: aniEnv["iteration-event"] || null
      };

      for (var type in t2n) {
        var nate = t2n[type];
        n2t[nate] = type;
      }

      qx.event.Registration.addHandler(statics);
    }
  });
  qx.event.handler.Transition.$$dbClassInfo = $$dbClassInfo;
})();

(function () {
  var $$dbClassInfo = {
    "dependsOn": {
      "qx.Bootstrap": {
        "usage": "dynamic",
        "require": true
      }
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
   * Christopher Zuendorf (czuendorf)
  
   ************************************************************************ */

  /**
   * Contains property maps for the usage with qx.bom.element.Animation {@link qx.bom.element.Animation}.
   * These animations can be used for page transitions for example.
   */
  qx.Bootstrap.define("qx.util.Animation", {
    statics: {
      /** Target slides in from right. */
      SLIDE_LEFT_IN: {
        duration: 350,
        timing: "linear",
        origin: "bottom center",
        keyFrames: {
          0: {
            translate: ["100%"]
          },
          100: {
            translate: ["0%"]
          }
        }
      },

      /** Target slides out from right.*/
      SLIDE_LEFT_OUT: {
        duration: 350,
        timing: "linear",
        origin: "bottom center",
        keyFrames: {
          0: {
            translate: ["0px"]
          },
          100: {
            translate: ["-100%"]
          }
        }
      },

      /** Target slides in from left.*/
      SLIDE_RIGHT_IN: {
        duration: 350,
        timing: "linear",
        origin: "bottom center",
        keyFrames: {
          0: {
            translate: ["-100%"]
          },
          100: {
            translate: ["0%"]
          }
        }
      },

      /** Target slides out from left.*/
      SLIDE_RIGHT_OUT: {
        duration: 350,
        timing: "linear",
        origin: "bottom center",
        keyFrames: {
          0: {
            translate: ["0px"]
          },
          100: {
            translate: ["100%"]
          }
        }
      },

      /** Target fades in. */
      FADE_IN: {
        duration: 350,
        timing: "linear",
        origin: "bottom center",
        keyFrames: {
          0: {
            opacity: ["0"]
          },
          100: {
            opacity: ["1"]
          }
        }
      },

      /** Target fades out. */
      FADE_OUT: {
        duration: 350,
        timing: "linear",
        origin: "bottom center",
        keyFrames: {
          0: {
            opacity: ["1"]
          },
          100: {
            opacity: ["0"]
          }
        }
      },

      /** Target pops in from center. */
      POP_IN: {
        duration: 350,
        timing: "linear",
        origin: "center",
        keyFrames: {
          0: {
            scale: [".2", ".2"]
          },
          100: {
            scale: ["1", "1"]
          }
        }
      },

      /** Target pops out from center. */
      POP_OUT: {
        duration: 350,
        timing: "linear",
        origin: "center",
        keyFrames: {
          0: {
            scale: ["1", "1"]
          },
          100: {
            scale: [".2", ".2"]
          }
        }
      },

      /** Target shrinks its height. */
      SHRINK_HEIGHT: {
        duration: 400,
        timing: "linear",
        origin: "top center",
        keep: 100,
        keyFrames: {
          0: {
            scale: ["1", "1"],
            opacity: 1
          },
          100: {
            scale: ["1", "0"],
            opacity: 0
          }
        }
      },

      /** Target grows its height. */
      GROW_HEIGHT: {
        duration: 400,
        timing: "linear",
        origin: "top center",
        keep: 100,
        keyFrames: {
          0: {
            scale: ["1", "0"],
            opacity: 0
          },
          100: {
            scale: ["1", "1"],
            opacity: 1
          }
        }
      },

      /** Target shrinks its width. */
      SHRINK_WIDTH: {
        duration: 400,
        timing: "linear",
        origin: "left center",
        keep: 100,
        keyFrames: {
          0: {
            scale: ["1", "1"],
            opacity: 1
          },
          100: {
            scale: ["0", "1"],
            opacity: 0
          }
        }
      },

      /** Target grows its width. */
      GROW_WIDTH: {
        duration: 400,
        timing: "linear",
        origin: "left center",
        keep: 100,
        keyFrames: {
          0: {
            scale: ["0", "1"],
            opacity: 0
          },
          100: {
            scale: ["1", "1"],
            opacity: 1
          }
        }
      },

      /** Target shrinks in both width and height. */
      SHRINK: {
        duration: 400,
        timing: "linear",
        origin: "left top",
        keep: 100,
        keyFrames: {
          0: {
            scale: ["1", "1"],
            opacity: 1
          },
          100: {
            scale: ["0", "0"],
            opacity: 0
          }
        }
      },

      /** Target grows in both width and height. */
      GROW: {
        duration: 400,
        timing: "linear",
        origin: "left top",
        keep: 100,
        keyFrames: {
          0: {
            scale: ["0", "0"],
            opacity: 0
          },
          100: {
            scale: ["1", "1"],
            opacity: 1
          }
        }
      },

      /** Target slides in to top. */
      SLIDE_UP_IN: {
        duration: 350,
        timing: "linear",
        origin: "center",
        keyFrames: {
          0: {
            translate: ["0px", "100%"]
          },
          100: {
            translate: ["0px", "0px"]
          }
        }
      },

      /** Target slides out to top.*/
      SLIDE_UP_OUT: {
        duration: 350,
        timing: "linear",
        origin: "center",
        keyFrames: {
          0: {
            translate: ["0px", "0px"]
          },
          100: {
            translate: ["0px", "0px"]
          }
        }
      },

      /** Target slides out to bottom.*/
      SLIDE_DOWN_IN: {
        duration: 350,
        timing: "linear",
        origin: "center",
        keyFrames: {
          0: {
            translate: ["0px", "0px"]
          },
          100: {
            translate: ["0px", "0px"]
          }
        }
      },

      /** Target slides down to bottom.*/
      SLIDE_DOWN_OUT: {
        duration: 350,
        timing: "linear",
        origin: "center",
        keyFrames: {
          0: {
            translate: ["0px", "0px"]
          },
          100: {
            translate: ["0px", "100%"]
          }
        }
      },

      /** Target flips (turns) left from back side to front side. */
      FLIP_LEFT_IN: {
        duration: 350,
        timing: "linear",
        origin: "center",
        keyFrames: {
          0: {
            opacity: 0
          },
          49: {
            opacity: 0
          },
          50: {
            rotate: ["0deg", "90deg"],
            scale: [".8", "1"],
            opacity: 1
          },
          100: {
            rotate: ["0deg", "0deg"],
            scale: ["1", "1"],
            opacity: 1
          }
        }
      },

      /** Target flips (turns) left from front side to back side. */
      FLIP_LEFT_OUT: {
        duration: 350,
        timing: "linear",
        origin: "center center",
        keyFrames: {
          0: {
            rotate: ["0deg", "0deg"],
            scale: ["1", "1"]
          },
          100: {
            rotate: ["0deg", "-180deg"],
            scale: [".8", "1"]
          }
        }
      },

      /** Target flips (turns) right from back side to front side. */
      FLIP_RIGHT_IN: {
        duration: 350,
        timing: "linear",
        origin: "center center",
        keyFrames: {
          0: {
            opacity: 0
          },
          49: {
            opacity: 0
          },
          50: {
            rotate: ["0deg", "-90deg"],
            scale: [".8", "1"],
            opacity: 1
          },
          100: {
            rotate: ["0deg", "0deg"],
            scale: ["1", "1"],
            opacity: 1
          }
        }
      },

      /** Target flips (turns) right from front side to back side. */
      FLIP_RIGHT_OUT: {
        duration: 350,
        timing: "linear",
        origin: "center center",
        keyFrames: {
          0: {
            rotate: ["0deg", "0deg"],
            scale: ["1", "1"]
          },
          100: {
            rotate: ["0deg", "180deg"],
            scale: [".8", "1"]
          }
        }
      },

      /** Target moves in to left. */
      SWAP_LEFT_IN: {
        duration: 700,
        timing: "ease-out",
        origin: "center center",
        keyFrames: {
          0: {
            rotate: ["0deg", "-70deg"],
            translate: ["0px", "0px", "-800px"],
            opacity: "0"
          },
          35: {
            rotate: ["0deg", "-20deg"],
            translate: ["-180px", "0px", "-400px"],
            opacity: "1"
          },
          100: {
            rotate: ["0deg", "0deg"],
            translate: ["0px", "0px", "0px"],
            opacity: "1"
          }
        }
      },

      /** Target moves out to left.  */
      SWAP_LEFT_OUT: {
        duration: 700,
        timing: "ease-out",
        origin: "center center",
        keyFrames: {
          0: {
            rotate: ["0deg", "0deg"],
            translate: ["0px", "0px", "0px"],
            opacity: "1"
          },
          35: {
            rotate: ["0deg", "20deg"],
            translate: ["-180px", "0px", "-400px"],
            opacity: ".5"
          },
          100: {
            rotate: ["0deg", "70deg"],
            translate: ["0px", "0px", "-800px"],
            opacity: "0"
          }
        }
      },

      /** Target moves in to right. */
      SWAP_RIGHT_IN: {
        duration: 700,
        timing: "ease-out",
        origin: "center center",
        keyFrames: {
          0: {
            rotate: ["0deg", "70deg"],
            translate: ["0px", "0px", "-800px"],
            opacity: "0"
          },
          35: {
            rotate: ["0deg", "20deg"],
            translate: ["-180px", "0px", "-400px"],
            opacity: "1"
          },
          100: {
            rotate: ["0deg", "0deg"],
            translate: ["0px", "0px", "0px"],
            opacity: "1"
          }
        }
      },

      /** Target moves out to right. */
      SWAP_RIGHT_OUT: {
        duration: 700,
        timing: "ease-out",
        origin: "center center",
        keyFrames: {
          0: {
            rotate: ["0deg", "0deg"],
            translate: ["0px", "0px", "0px"],
            opacity: "1"
          },
          35: {
            rotate: ["0deg", "-20deg"],
            translate: ["180px", "0px", "-400px"],
            opacity: ".5"
          },
          100: {
            rotate: ["0deg", "-70deg"],
            translate: ["0px", "0px", "-800px"],
            opacity: "0"
          }
        }
      },

      /** Target moves in with cube animation from right to left.  */
      CUBE_LEFT_IN: {
        duration: 550,
        timing: "linear",
        origin: "100% 50%",
        keyFrames: {
          0: {
            rotate: ["0deg", "90deg"],
            scale: ".5",
            translate: ["0", "0", "0px"],
            opacity: [".5"]
          },
          100: {
            rotate: ["0deg", "0deg"],
            scale: "1",
            translate: ["0", "0", "0"],
            opacity: ["1"]
          }
        }
      },

      /** Target moves out with cube animation from right to left.  */
      CUBE_LEFT_OUT: {
        duration: 550,
        timing: "linear",
        origin: "0% 50%",
        keyFrames: {
          0: {
            rotate: ["0deg", "0deg"],
            scale: "1",
            translate: ["0", "0", "0px"],
            opacity: ["1"]
          },
          100: {
            rotate: ["0deg", "-90deg"],
            scale: ".5",
            translate: ["0", "0", "0"],
            opacity: [".5"]
          }
        }
      },

      /** Target moves in with cube animation from left to right.  */
      CUBE_RIGHT_IN: {
        duration: 550,
        timing: "linear",
        origin: "0% 50%",
        keyFrames: {
          0: {
            rotate: ["0deg", "-90deg"],
            scale: ".5",
            translate: ["0", "0", "0px"],
            opacity: [".5"]
          },
          100: {
            rotate: ["0deg", "0deg"],
            scale: "1",
            translate: ["0", "0", "0"],
            opacity: ["1"]
          }
        }
      },

      /** Target moves out with cube animation from left to right.  */
      CUBE_RIGHT_OUT: {
        duration: 550,
        timing: "linear",
        origin: "100% 50%",
        keyFrames: {
          0: {
            rotate: ["0deg", "0deg"],
            scale: "1",
            translate: ["0", "0", "0px"],
            opacity: ["1"]
          },
          100: {
            rotate: ["0deg", "90deg"],
            scale: ".5",
            translate: ["0", "0", "0"],
            opacity: [".5"]
          }
        }
      }
    }
  });
  qx.util.Animation.$$dbClassInfo = $$dbClassInfo;
})();

function _typeof(obj) { "@babel/helpers - typeof"; if (typeof Symbol === "function" && typeof Symbol.iterator === "symbol") { _typeof = function _typeof(obj) { return typeof obj; }; } else { _typeof = function _typeof(obj) { return obj && typeof Symbol === "function" && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj; }; } return _typeof(obj); }

(function () {
  var $$dbClassInfo = {
    "dependsOn": {
      "qx.Class": {
        "usage": "dynamic",
        "require": true
      },
      "qx.log.Logger": {},
      "qx.lang.Object": {},
      "qx.lang.Type": {},
      "qx.data.IListData": {},
      "qx.lang.String": {}
    }
  };
  qx.Bootstrap.executePendingDefers($$dbClassInfo);

  /* ************************************************************************
  
     qooxdoo - the new era of web development
  
     http://qooxdoo.org
  
     Copyright:
       2006, 2007 Derrell Lipman
       2011 1&1 Internet AG, Germany, http://www.1und1.de
  
     License:
       MIT: https://opensource.org/licenses/MIT
       See the LICENSE file in the project's top-level directory for details.
  
     Authors:
       * Derrell Lipman (derrell)
       * Daniel Wagner (d_wagner)
  
  ************************************************************************ */

  /**
   * Useful debug capabilities
   * @ignore(qx.ui.decoration.IDecorator)
   * @ignore(qx.theme.manager.Decoration)
   * @ignore(qx.ui.core.queue.Dispose)
   * @ignore(qx.bom.Font)
   * @ignore(qx.theme.manager.Font)
   */
  qx.Class.define("qx.dev.Debug", {
    statics: {
      /**
       * Flag that shows whether dispose profiling is currently active
       * @internal
       */
      disposeProfilingActive: false,

      /**
       * Recursively display an object (as a debug message)
       *
       *
       * @param obj {Object}
       *   The object to be recursively displayed
       *
       * @param initialMessage {String|null}
       *   The initial message to be displayed.
       *
       * @param maxLevel {Integer ? 10}
       *   The maximum level of recursion.  Objects beyond this level will not
       *   be displayed.
       *
       */
      debugObject: function debugObject(obj, initialMessage, maxLevel) {
        // We've compiled the complete message.  Give 'em what they came for!
        qx.log.Logger.debug(this, qx.dev.Debug.debugObjectToString(obj, initialMessage, maxLevel, false));
      },

      /**
       * Recursively display an object (into a string)
       *
       *
       * @param obj {Object}
       *   The object to be recursively displayed
       *
       * @param initialMessage {String|null}
       *   The initial message to be displayed.
       *
       * @param maxLevel {Integer ? 10}
       *   The maximum level of recursion.  Objects beyond this level will not
       *   be displayed.
       *
       * @param bHtml {Boolean ? false}
       *   If true, then render the debug message in HTML;
       *   Otherwise, use spaces for indentation and "\n" for end of line.
       *
       * @return {String}
       *   The string containing the recursive display of the object
       *
       * @lint ignoreUnused(prop)
       */
      debugObjectToString: function debugObjectToString(obj, initialMessage, maxLevel, bHtml) {
        // If a maximum recursion level was not specified...
        if (!maxLevel) {
          // ... then create one arbitrarily
          maxLevel = 10;
        } // If they want html, the differences are "<br>" instead of "\n"
        // and how we do the indentation.  Define the end-of-line string
        // and a start-of-line function.


        var eol = bHtml ? "</span><br>" : "\n";

        var sol = function sol(currentLevel) {
          var indentStr;

          if (!bHtml) {
            indentStr = "";

            for (var i = 0; i < currentLevel; i++) {
              indentStr += "  ";
            }
          } else {
            indentStr = "<span style='padding-left:" + currentLevel * 8 + "px;'>";
          }

          return indentStr;
        }; // Initialize an empty message to be displayed


        var message = ""; // Function to recursively display an object

        var displayObj = function displayObj(obj, level, maxLevel) {
          // If we've exceeded the maximum recursion level...
          if (level > maxLevel) {
            // ... then tell 'em so, and get outta dodge.
            message += sol(level) + "*** TOO MUCH RECURSION: not displaying ***" + eol;
            return;
          } // Is this an ordinary non-recursive item?


          if (_typeof(obj) != "object") {
            // Yup.  Just add it to the message.
            message += sol(level) + obj + eol;
            return;
          } // We have an object  or array.  For each child...


          for (var prop in obj) {
            // Is this child a recursive item?
            if (_typeof(obj[prop]) == "object") {
              try {
                // Yup.  Determine the type and add it to the message
                if (obj[prop] instanceof Array) {
                  message += sol(level) + prop + ": " + "Array" + eol;
                } else if (obj[prop] === null) {
                  message += sol(level) + prop + ": " + "null" + eol;
                  continue;
                } else if (obj[prop] === undefined) {
                  message += sol(level) + prop + ": " + "undefined" + eol;
                  continue;
                } else {
                  message += sol(level) + prop + ": " + "Object" + eol;
                } // Recurse into it to display its children.


                displayObj(obj[prop], level + 1, maxLevel);
              } catch (e) {
                message += sol(level) + prop + ": EXCEPTION expanding property" + eol;
              }
            } else {
              // We have an ordinary non-recursive item.  Add it to the message.
              message += sol(level) + prop + ": " + obj[prop] + eol;
            }
          }
        }; // Was an initial message provided?


        if (initialMessage) {
          // Yup.  Add it to the displayable message.
          message += sol(0) + initialMessage + eol;
        }

        if (obj instanceof Array) {
          message += sol(0) + "Array, length=" + obj.length + ":" + eol;
        } else if (_typeof(obj) == "object") {
          var count = 0;

          for (var prop in obj) {
            count++;
          }

          message += sol(0) + "Object, count=" + count + ":" + eol;
        }

        message += sol(0) + "------------------------------------------------------------" + eol;

        try {
          // Recursively display this object
          displayObj(obj, 0, maxLevel);
        } catch (ex) {
          message += sol(0) + "*** EXCEPTION (" + ex + ") ***" + eol;
        }

        message += sol(0) + "============================================================" + eol;
        return message;
      },

      /**
       * Get the name of a member/static function or constructor defined using the new style class definition.
       * If the function could not be found <code>null</code> is returned.
       *
       * This function uses a linear search, so don't use it in performance critical
       * code.
       *
       * @param func {Function} member function to get the name of.
       * @param functionType {String?"all"} Where to look for the function. Possible values are "members", "statics", "constructor", "all"
       * @return {String|null} Name of the function (null if not found).
       */
      getFunctionName: function getFunctionName(func, functionType) {
        var clazz = func.self;

        if (!clazz) {
          return null;
        } // unwrap


        while (func.wrapper) {
          func = func.wrapper;
        }

        switch (functionType) {
          case "construct":
            return func == clazz ? "construct" : null;

          case "members":
            return qx.lang.Object.getKeyFromValue(clazz, func);

          case "statics":
            return qx.lang.Object.getKeyFromValue(clazz.prototype, func);

          default:
            // constructor
            if (func == clazz) {
              return "construct";
            }

            return qx.lang.Object.getKeyFromValue(clazz.prototype, func) || qx.lang.Object.getKeyFromValue(clazz, func) || null;
        }
      },

      /**
       * Returns a string representing the given model. The string will include
       * all model objects to a given recursive depth.
       *
       * @param model {qx.core.Object} The model object.
       * @param maxLevel {Number ? 10} The amount of max recursive depth.
       * @param html {Boolean ? false} If the returned string should have \n\r as
       *   newline of <br>.
       * @param indent {Number ? 1} The indentation level.
       *   (Needed for the recursion)
       *
       * @return {String} A string representation of the given model.
       */
      debugProperties: function debugProperties(model, maxLevel, html, indent) {
        // set the default max depth of the recursion
        if (maxLevel == null) {
          maxLevel = 10;
        } // set the default startin indent


        if (indent == null) {
          indent = 1;
        }

        var newLine = "";
        html ? newLine = "<br>" : newLine = "\r\n";
        var message = "";

        if (qx.lang.Type.isNumber(model) || qx.lang.Type.isString(model) || qx.lang.Type.isBoolean(model) || model == null || maxLevel <= 0) {
          return model;
        } else if (qx.Class.hasInterface(model.constructor, qx.data.IListData)) {
          // go threw the data structure
          for (var i = 0; i < model.length; i++) {
            // print out the indentation
            for (var j = 0; j < indent; j++) {
              message += "-";
            }

            message += "index(" + i + "): " + this.debugProperties(model.getItem(i), maxLevel - 1, html, indent + 1) + newLine;
          }

          return message + newLine;
        } else if (model.constructor != null) {
          // go threw all properties
          var properties = model.constructor.$$properties;

          for (var key in properties) {
            message += newLine; // print out the indentation

            for (var j = 0; j < indent; j++) {
              message += "-";
            }

            message += " " + key + ": " + this.debugProperties(model["get" + qx.lang.String.firstUp(key)](), maxLevel - 1, html, indent + 1);
          }

          return message;
        }

        return "";
      },

      /**
       * Starts a dispose profiling session. Use {@link #stopDisposeProfiling} to
       * get the results
       *
       * @return {Number|undefined}
       *   Returns a handle which may be passed to {@link #stopDisposeProfiling}
       *   indicating the start point for searching for undisposed objects.
       */
      startDisposeProfiling: function startDisposeProfiling() {},

      /**
       * Returns a list of any (qx) objects that were created but not disposed
       * since {@link #startDisposeProfiling} was called. Also returns a stack
       * trace recorded at the time the object was created. The starting point
       * of dispose tracking is reset, so to do further dispose profiling, a new
       * call to {@link #startDisposeProfile} must be issued.
       *
       * @signature function(checkFunction)
       * @param checkFunction {Function} Custom check function. It is called once
       * for each object that was created after dispose profiling was started,
       * with the object as the only parameter. If it returns false, the object
       * will not be included in the returned list
       * @return {Map[]} List of maps. Each map contains two keys:
       * <code>object</code> and <code>stackTrace</code>
       */
      stopDisposeProfiling: function stopDisposeProfiling() {},

      /**
       * Returns a list of any (qx) objects that were created but not disposed
       * since {@link #startDisposeProfiling} was called. Also returns a stack
       * trace recorded at the time the object was created. Does not restart the
       * tracking point, so subsequent calls to this method will continue to
       * show undisposed objects since {@link #startDisposeProfiling} was
       * called.
       *
       * @signature function(checkFunction)
       * @param checkFunction {Function} Custom check function. It is called once
       * for each object that was created after dispose profiling was started,
       * with the object as the only parameter. If it returns false, the object
       * will not be included in the returned list
       * @return {Map[]} List of maps. Each map contains two keys:
       * <code>object</code> and <code>stackTrace</code>
       */
      showDisposeProfiling: function showDisposeProfiling() {}
    }
  });
  qx.dev.Debug.$$dbClassInfo = $$dbClassInfo;
})();

(function () {
  var $$dbClassInfo = {
    "dependsOn": {
      "qx.core.Environment": {
        "defer": "load",
        "usage": "dynamic",
        "require": true
      },
      "qx.Bootstrap": {
        "usage": "dynamic",
        "require": true
      },
      "qx.bom.client.CssAnimation": {
        "require": true
      },
      "qx.bom.Stylesheet": {},
      "qx.bom.Event": {},
      "qx.bom.element.Style": {},
      "qx.log.Logger": {},
      "qx.lang.String": {},
      "qx.bom.element.AnimationHandle": {},
      "qx.bom.element.Transform": {},
      "qx.bom.Style": {},
      "qx.bom.client.OperatingSystem": {
        "defer": "runtime"
      }
    },
    "environment": {
      "provided": [],
      "required": {
        "css.animation": {
          "load": true,
          "className": "qx.bom.client.CssAnimation"
        },
        "os.name": {
          "defer": true,
          "className": "qx.bom.client.OperatingSystem"
        },
        "os.version": {
          "defer": true,
          "className": "qx.bom.client.OperatingSystem"
        }
      }
    }
  };
  qx.Bootstrap.executePendingDefers($$dbClassInfo);

  /* ************************************************************************
  
     qooxdoo - the new era of web development
  
     http://qooxdoo.org
  
     Copyright:
       2011 1&1 Internet AG, Germany, http://www.1und1.de
  
     License:
       MIT: https://opensource.org/licenses/MIT
       See the LICENSE file in the project's top-level directory for details.
  
     Authors:
       * Martin Wittemann (martinwittemann)
  
  ************************************************************************ */

  /**
   * This class is responsible for applying CSS3 animations to plain DOM elements.
   *
   * The implementation is mostly a cross-browser wrapper for applying the
   * animations, including transforms. If the browser does not support
   * CSS animations, but you have set a keep frame, the keep frame will be applied
   * immediately, thus making the animations optional.
   *
   * The API aligns closely to the spec wherever possible.
   *
   * http://www.w3.org/TR/css3-animations/
   *
   * {@link qx.bom.element.Animation} is the class, which takes care of the
   * feature detection for CSS animations and decides which implementation
   * (CSS or JavaScript) should be used. Most likely, this implementation should
   * be the one to use.
   */
  qx.Bootstrap.define("qx.bom.element.AnimationCss", {
    statics: {
      // initialization
      __sheet__P_154_0: null,
      __rulePrefix__P_154_1: "Anni",
      __id__P_154_2: 0,

      /** Static map of rules */
      __rules__P_154_3: {},

      /** The used keys for transforms. */
      __transitionKeys__P_154_4: {
        "scale": true,
        "rotate": true,
        "skew": true,
        "translate": true
      },

      /** Map of cross browser CSS keys. */
      __cssAnimationKeys__P_154_5: qx.core.Environment.get("css.animation"),

      /**
       * This is the main function to start the animation in reverse mode.
       * For further details, take a look at the documentation of the wrapper
       * {@link qx.bom.element.Animation}.
       * @param el {Element} The element to animate.
       * @param desc {Map} Animation description.
       * @param duration {Integer?} The duration of the animation which will
       *   override the duration given in the description.
       * @return {qx.bom.element.AnimationHandle} The handle.
       */
      animateReverse: function animateReverse(el, desc, duration) {
        return this._animate(el, desc, duration, true);
      },

      /**
       * This is the main function to start the animation. For further details,
       * take a look at the documentation of the wrapper
       * {@link qx.bom.element.Animation}.
       * @param el {Element} The element to animate.
       * @param desc {Map} Animation description.
       * @param duration {Integer?} The duration of the animation which will
       *   override the duration given in the description.
       * @return {qx.bom.element.AnimationHandle} The handle.
       */
      animate: function animate(el, desc, duration) {
        return this._animate(el, desc, duration, false);
      },

      /**
       * Internal method to start an animation either reverse or not.
       * {@link qx.bom.element.Animation}.
       * @param el {Element} The element to animate.
       * @param desc {Map} Animation description.
       * @param duration {Integer?} The duration of the animation which will
       *   override the duration given in the description.
       * @param reverse {Boolean} <code>true</code>, if the animation should be
       *   reversed.
       * @return {qx.bom.element.AnimationHandle} The handle.
       */
      _animate: function _animate(el, desc, duration, reverse) {
        this.__normalizeDesc__P_154_6(desc); // debug validation


        {
          this.__validateDesc__P_154_7(desc);
        } // reverse the keep property if the animation is reverse as well

        var keep = desc.keep;

        if (keep != null && (reverse || desc.alternate && desc.repeat % 2 == 0)) {
          keep = 100 - keep;
        }

        if (!this.__sheet__P_154_0) {
          this.__sheet__P_154_0 = qx.bom.Stylesheet.createElement();
        }

        var keyFrames = desc.keyFrames;

        if (duration == undefined) {
          duration = desc.duration;
        } // if animations are supported


        if (this.__cssAnimationKeys__P_154_5 != null) {
          var name = this.__addKeyFrames__P_154_8(keyFrames, reverse);

          var style = name + " " + duration + "ms " + desc.timing + " " + (desc.delay ? desc.delay + "ms " : "") + desc.repeat + " " + (desc.alternate ? "alternate" : "");
          qx.bom.Event.addNativeListener(el, this.__cssAnimationKeys__P_154_5["start-event"], this.__onAnimationStart__P_154_9);
          qx.bom.Event.addNativeListener(el, this.__cssAnimationKeys__P_154_5["iteration-event"], this.__onAnimationIteration__P_154_10);
          qx.bom.Event.addNativeListener(el, this.__cssAnimationKeys__P_154_5["end-event"], this.__onAnimationEnd__P_154_11);
          {
            if (qx.bom.element.Style.get(el, "display") == "none") {
              qx.log.Logger.warn(el, "Some browsers will not animate elements with display==none");
            }
          }
          el.style[qx.lang.String.camelCase(this.__cssAnimationKeys__P_154_5["name"])] = style; // use the fill mode property if available and suitable

          if (keep && keep == 100 && this.__cssAnimationKeys__P_154_5["fill-mode"]) {
            el.style[this.__cssAnimationKeys__P_154_5["fill-mode"]] = "forwards";
          }
        }

        var animation = new qx.bom.element.AnimationHandle();
        animation.desc = desc;
        animation.el = el;
        animation.keep = keep;
        el.$$animation = animation; // additional transform keys

        if (desc.origin != null) {
          qx.bom.element.Transform.setOrigin(el, desc.origin);
        } // fallback for browsers not supporting animations


        if (this.__cssAnimationKeys__P_154_5 == null) {
          window.setTimeout(function () {
            qx.bom.element.AnimationCss.__onAnimationEnd__P_154_11({
              target: el
            });
          }, 0);
        }

        return animation;
      },

      /**
       * Handler for the animation start.
       * @param e {Event} The native event from the browser.
       */
      __onAnimationStart__P_154_9: function __onAnimationStart__P_154_9(e) {
        if (e.target.$$animation) {
          e.target.$$animation.emit("start", e.target);
        }
      },

      /**
       * Handler for the animation iteration.
       * @param e {Event} The native event from the browser.
       */
      __onAnimationIteration__P_154_10: function __onAnimationIteration__P_154_10(e) {
        // It could happen that an animation end event is fired before an
        // animation iteration appears [BUG #6928]
        if (e.target != null && e.target.$$animation != null) {
          e.target.$$animation.emit("iteration", e.target);
        }
      },

      /**
       * Handler for the animation end.
       * @param e {Event} The native event from the browser.
       */
      __onAnimationEnd__P_154_11: function __onAnimationEnd__P_154_11(e) {
        var el = e.target;
        var animation = el.$$animation; // ignore events when already cleaned up

        if (!animation) {
          return;
        }

        var desc = animation.desc;

        if (qx.bom.element.AnimationCss.__cssAnimationKeys__P_154_5 != null) {
          // reset the styling
          var key = qx.lang.String.camelCase(qx.bom.element.AnimationCss.__cssAnimationKeys__P_154_5["name"]);
          el.style[key] = "";
          qx.bom.Event.removeNativeListener(el, qx.bom.element.AnimationCss.__cssAnimationKeys__P_154_5["name"], qx.bom.element.AnimationCss.__onAnimationEnd__P_154_11);
        }

        if (desc.origin != null) {
          qx.bom.element.Transform.setOrigin(el, "");
        }

        qx.bom.element.AnimationCss.__keepFrame__P_154_12(el, desc.keyFrames[animation.keep]);

        el.$$animation = null;
        animation.el = null;
        animation.ended = true;
        animation.emit("end", el);
      },

      /**
       * Helper method which takes an element and a key frame description and
       * applies the properties defined in the given frame to the element. This
       * method is used to keep the state of the animation.
       * @param el {Element} The element to apply the frame to.
       * @param endFrame {Map} The description of the end frame, which is basically
       *   a map containing CSS properties and values including transforms.
       */
      __keepFrame__P_154_12: function __keepFrame__P_154_12(el, endFrame) {
        // keep the element at this animation step
        var transforms;

        for (var style in endFrame) {
          if (style in qx.bom.element.AnimationCss.__transitionKeys__P_154_4) {
            if (!transforms) {
              transforms = {};
            }

            transforms[style] = endFrame[style];
          } else {
            el.style[qx.lang.String.camelCase(style)] = endFrame[style];
          }
        } // transform keeping


        if (transforms) {
          qx.bom.element.Transform.transform(el, transforms);
        }
      },

      /**
       * Preprocessing of the description to make sure every necessary key is
       * set to its default.
       * @param desc {Map} The description of the animation.
       */
      __normalizeDesc__P_154_6: function __normalizeDesc__P_154_6(desc) {
        if (!desc.hasOwnProperty("alternate")) {
          desc.alternate = false;
        }

        if (!desc.hasOwnProperty("keep")) {
          desc.keep = null;
        }

        if (!desc.hasOwnProperty("repeat")) {
          desc.repeat = 1;
        }

        if (!desc.hasOwnProperty("timing")) {
          desc.timing = "linear";
        }

        if (!desc.hasOwnProperty("origin")) {
          desc.origin = null;
        }
      },

      /**
       * Debugging helper to validate the description.
       * @signature function(desc)
       * @param desc {Map} The description of the animation.
       */
      __validateDesc__P_154_7: function __validateDesc__P_154_7(desc) {
        var possibleKeys = ["origin", "duration", "keep", "keyFrames", "delay", "repeat", "timing", "alternate"]; // check for unknown keys

        for (var name in desc) {
          if (!(possibleKeys.indexOf(name) != -1)) {
            qx.Bootstrap.warn("Unknown key '" + name + "' in the animation description.");
          }
        }

        ;

        if (desc.keyFrames == null) {
          qx.Bootstrap.warn("No 'keyFrames' given > 0");
        } else {
          // check the key frames
          for (var pos in desc.keyFrames) {
            if (pos < 0 || pos > 100) {
              qx.Bootstrap.warn("Keyframe position needs to be between 0 and 100");
            }
          }
        }
      },

      /**
       * Helper to add the given frames to an internal CSS stylesheet. It parses
       * the description and adds the key frames to the sheet.
       * @param frames {Map} A map of key frames that describe the animation.
       * @param reverse {Boolean} <code>true</code>, if the key frames should
       *   be added in reverse order.
       * @return {String} The generated name of the keyframes rule.
       */
      __addKeyFrames__P_154_8: function __addKeyFrames__P_154_8(frames, reverse) {
        var rule = ""; // for each key frame

        for (var position in frames) {
          rule += (reverse ? -(position - 100) : position) + "% {";
          var frame = frames[position];
          var transforms; // each style

          for (var style in frame) {
            if (style in this.__transitionKeys__P_154_4) {
              if (!transforms) {
                transforms = {};
              }

              transforms[style] = frame[style];
            } else {
              var propName = qx.bom.Style.getPropertyName(style);
              var prefixed = propName !== null ? qx.bom.Style.getCssName(propName) : "";
              rule += (prefixed || style) + ":" + frame[style] + ";";
            }
          } // transform handling


          if (transforms) {
            rule += qx.bom.element.Transform.getCss(transforms);
          }

          rule += "} ";
        } // cached shorthand


        if (this.__rules__P_154_3[rule]) {
          return this.__rules__P_154_3[rule];
        }

        var name = this.__rulePrefix__P_154_1 + this.__id__P_154_2++;
        var selector = this.__cssAnimationKeys__P_154_5["keyframes"] + " " + name;
        qx.bom.Stylesheet.addRule(this.__sheet__P_154_0, selector, rule);
        this.__rules__P_154_3[rule] = name;
        return name;
      },

      /**
       * Internal helper to reset the cache.
       */
      __clearCache__P_154_13: function __clearCache__P_154_13() {
        this.__id__P_154_2 = 0;

        if (this.__sheet__P_154_0) {
          this.__sheet__P_154_0.ownerNode.remove();

          this.__sheet__P_154_0 = null;
          this.__rules__P_154_3 = {};
        }
      }
    },
    defer: function defer(statics) {
      // iOS 8 seems to stumble over the old sheet object on tab
      // changes or leaving the browser [BUG #8986]
      if (qx.core.Environment.get("os.name") === "ios" && parseInt(qx.core.Environment.get("os.version")) >= 8) {
        document.addEventListener("visibilitychange", function () {
          if (!document.hidden) {
            statics.__clearCache__P_154_13();
          }
        }, false);
      }
    }
  });
  qx.bom.element.AnimationCss.$$dbClassInfo = $$dbClassInfo;
})();

(function () {
  var $$dbClassInfo = {
    "dependsOn": {
      "qx.Bootstrap": {
        "usage": "dynamic",
        "require": true
      },
      "qx.lang.Object": {},
      "qx.bom.element.AnimationHandle": {},
      "qx.bom.Style": {},
      "qx.bom.element.Transform": {},
      "qx.util.ColorUtil": {},
      "qx.bom.AnimationFrame": {},
      "qx.lang.String": {}
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
       * Martin Wittemann (wittemann)
  
  ************************************************************************ */

  /**
   * This class offers the same API as the CSS3 animation layer in
   * {@link qx.bom.element.AnimationCss} but uses JavaScript to fake the behavior.
   *
   * {@link qx.bom.element.Animation} is the class, which takes care of the
   * feature detection for CSS animations and decides which implementation
   * (CSS or JavaScript) should be used. Most likely, this implementation should
   * be the one to use.
   *
   * @ignore(qx.bom.element.Style.*)
   * @use(qx.bom.element.AnimationJs#play)
   */
  qx.Bootstrap.define("qx.bom.element.AnimationJs", {
    statics: {
      /**
       * The maximal time a frame should take.
       */
      __maxStepTime__P_155_0: 30,

      /**
       * The supported CSS units.
       */
      __units__P_155_1: ["%", "in", "cm", "mm", "em", "ex", "pt", "pc", "px"],

      /** The used keys for transforms. */
      __transitionKeys__P_155_2: {
        "scale": true,
        "rotate": true,
        "skew": true,
        "translate": true
      },

      /**
       * This is the main function to start the animation. For further details,
       * take a look at the documentation of the wrapper
       * {@link qx.bom.element.Animation}.
       * @param el {Element} The element to animate.
       * @param desc {Map} Animation description.
       * @param duration {Integer?} The duration of the animation which will
       *   override the duration given in the description.
       * @return {qx.bom.element.AnimationHandle} The handle.
       */
      animate: function animate(el, desc, duration) {
        return this._animate(el, desc, duration, false);
      },

      /**
       * This is the main function to start the animation in reversed mode.
       * For further details, take a look at the documentation of the wrapper
       * {@link qx.bom.element.Animation}.
       * @param el {Element} The element to animate.
       * @param desc {Map} Animation description.
       * @param duration {Integer?} The duration of the animation which will
       *   override the duration given in the description.
       * @return {qx.bom.element.AnimationHandle} The handle.
       */
      animateReverse: function animateReverse(el, desc, duration) {
        return this._animate(el, desc, duration, true);
      },

      /**
       * Helper to start the animation, either in reversed order or not.
       *
       * @param el {Element} The element to animate.
       * @param desc {Map} Animation description.
       * @param duration {Integer?} The duration of the animation which will
       *   override the duration given in the description.
       * @param reverse {Boolean} <code>true</code>, if the animation should be
       *   reversed.
       * @return {qx.bom.element.AnimationHandle} The handle.
       */
      _animate: function _animate(el, desc, duration, reverse) {
        // stop if an animation is already running
        if (el.$$animation) {
          return el.$$animation;
        }

        desc = qx.lang.Object.clone(desc, true);

        if (duration == undefined) {
          duration = desc.duration;
        }

        var keyFrames = desc.keyFrames;

        var keys = this.__getOrderedKeys__P_155_3(keyFrames);

        var stepTime = this.__getStepTime__P_155_4(duration, keys);

        var steps = parseInt(duration / stepTime, 10);

        this.__normalizeKeyFrames__P_155_5(keyFrames, el);

        var delta = this.__calculateDelta__P_155_6(steps, stepTime, keys, keyFrames, duration, desc.timing);

        var handle = new qx.bom.element.AnimationHandle();
        handle.jsAnimation = true;

        if (reverse) {
          delta.reverse();
          handle.reverse = true;
        }

        handle.desc = desc;
        handle.el = el;
        handle.delta = delta;
        handle.stepTime = stepTime;
        handle.steps = steps;
        el.$$animation = handle;
        handle.i = 0;
        handle.initValues = {};
        handle.repeatSteps = this.__applyRepeat__P_155_7(steps, desc.repeat);
        var delay = desc.delay || 0;
        var self = this;
        handle.delayId = window.setTimeout(function () {
          handle.delayId = null;
          self.play(handle);
        }, delay);
        return handle;
      },

      /**
       * Try to normalize the keyFrames by adding the default / set values of the
       * element.
       * @param keyFrames {Map} The map of key frames.
       * @param el {Element} The element to animate.
       */
      __normalizeKeyFrames__P_155_5: function __normalizeKeyFrames__P_155_5(keyFrames, el) {
        // collect all possible keys and its units
        var units = {};

        for (var percent in keyFrames) {
          for (var name in keyFrames[percent]) {
            // prefixed key calculation
            var prefixed = qx.bom.Style.getPropertyName(name);

            if (prefixed && prefixed != name) {
              var prefixedName = qx.bom.Style.getCssName(prefixed);
              keyFrames[percent][prefixedName] = keyFrames[percent][name];
              delete keyFrames[percent][name];
              name = prefixedName;
            } // check for the available units


            if (units[name] == undefined) {
              var item = keyFrames[percent][name];

              if (typeof item == "string") {
                units[name] = this.__getUnit__P_155_8(item);
              } else {
                units[name] = "";
              }
            }
          }

          ;
        } // add all missing keys


        for (var percent in keyFrames) {
          var frame = keyFrames[percent];

          for (var name in units) {
            if (frame[name] == undefined) {
              if (name in el.style) {
                // get the computed style if possible
                if (window.getComputedStyle) {
                  frame[name] = window.getComputedStyle(el, null)[name];
                } else {
                  frame[name] = el.style[name];
                }
              } else {
                frame[name] = el[name];
              } // if its a unit we know, set 0 as fallback


              if (frame[name] === "" && this.__units__P_155_1.indexOf(units[name]) != -1) {
                frame[name] = "0" + units[name];
              }
            }
          }

          ;
        }

        ;
      },

      /**
       * Checks for transform keys and returns a cloned frame
       * with the right transform style set.
       * @param frame {Map} A single key frame of the description.
       * @return {Map} A modified clone of the given frame.
       */
      __normalizeKeyFrameTransforms__P_155_9: function __normalizeKeyFrameTransforms__P_155_9(frame) {
        frame = qx.lang.Object.clone(frame);
        var transforms;

        for (var name in frame) {
          if (name in this.__transitionKeys__P_155_2) {
            if (!transforms) {
              transforms = {};
            }

            transforms[name] = frame[name];
            delete frame[name];
          }
        }

        ;

        if (transforms) {
          var transformStyle = qx.bom.element.Transform.getCss(transforms).split(":");

          if (transformStyle.length > 1) {
            frame[transformStyle[0]] = transformStyle[1].replace(";", "");
          }
        }

        return frame;
      },

      /**
       * Precalculation of the delta which will be applied during the animation.
       * The whole deltas will be calculated prior to the animation and stored
       * in a single array. This method takes care of that calculation.
       *
       * @param steps {Integer} The amount of steps to take to the end of the
       *   animation.
       * @param stepTime {Integer} The amount of milliseconds each step takes.
       * @param keys {Array} Ordered list of keys in the key frames map.
       * @param keyFrames {Map} The map of key frames.
       * @param duration {Integer} Time in milliseconds the animation should take.
       * @param timing {String} The given timing function.
       * @return {Array} An array containing the animation deltas.
       */
      __calculateDelta__P_155_6: function __calculateDelta__P_155_6(steps, stepTime, keys, keyFrames, duration, timing) {
        var delta = new Array(steps);
        var keyIndex = 1;
        delta[0] = this.__normalizeKeyFrameTransforms__P_155_9(keyFrames[0]);
        var last = keyFrames[0];
        var next = keyFrames[keys[keyIndex]];
        var stepsToNext = Math.floor(keys[keyIndex] / (stepTime / duration * 100));
        var calculationIndex = 1; // is used as counter for the timing calculation
        // for every step

        for (var i = 1; i < delta.length; i++) {
          // switch key frames if we crossed a percent border
          if (i * stepTime / duration * 100 > keys[keyIndex]) {
            last = next;
            keyIndex++;
            next = keyFrames[keys[keyIndex]];
            stepsToNext = Math.floor(keys[keyIndex] / (stepTime / duration * 100)) - stepsToNext;
            calculationIndex = 1;
          }

          delta[i] = {};
          var transforms; // for every property

          for (var name in next) {
            var nItem = next[name] + ""; // transform values

            if (name in this.__transitionKeys__P_155_2) {
              if (!transforms) {
                transforms = {};
              }

              if (qx.Bootstrap.isArray(last[name])) {
                if (!qx.Bootstrap.isArray(next[name])) {
                  next[name] = [next[name]];
                }

                transforms[name] = [];

                for (var j = 0; j < next[name].length; j++) {
                  var item = next[name][j] + "";
                  var x = calculationIndex / stepsToNext;
                  transforms[name][j] = this.__getNextValue__P_155_10(item, last[name], timing, x);
                }
              } else {
                var x = calculationIndex / stepsToNext;
                transforms[name] = this.__getNextValue__P_155_10(nItem, last[name], timing, x);
              } // color values

            } else if (nItem.charAt(0) == "#") {
              // get the two values from the frames as RGB arrays
              var value0 = qx.util.ColorUtil.cssStringToRgb(last[name]);
              var value1 = qx.util.ColorUtil.cssStringToRgb(nItem);
              var stepValue = []; // calculate every color channel

              for (var j = 0; j < value0.length; j++) {
                var range = value0[j] - value1[j];
                var x = calculationIndex / stepsToNext;
                var timingX = qx.bom.AnimationFrame.calculateTiming(timing, x);
                stepValue[j] = parseInt(value0[j] - range * timingX, 10);
              }

              delta[i][name] = qx.util.ColorUtil.rgbToHexString(stepValue);
            } else if (!isNaN(parseFloat(nItem))) {
              var x = calculationIndex / stepsToNext;
              delta[i][name] = this.__getNextValue__P_155_10(nItem, last[name], timing, x);
            } else {
              delta[i][name] = last[name] + "";
            }
          } // save all transformations in the delta values


          if (transforms) {
            var transformStyle = qx.bom.element.Transform.getCss(transforms).split(":");

            if (transformStyle.length > 1) {
              delta[i][transformStyle[0]] = transformStyle[1].replace(";", "");
            }
          }

          calculationIndex++;
        } // make sure the last key frame is right


        delta[delta.length - 1] = this.__normalizeKeyFrameTransforms__P_155_9(keyFrames[100]);
        return delta;
      },

      /**
       * Ties to parse out the unit of the given value.
       *
       * @param item {String} A CSS value including its unit.
       * @return {String} The unit of the given value.
       */
      __getUnit__P_155_8: function __getUnit__P_155_8(item) {
        return item.substring((parseFloat(item) + "").length, item.length);
      },

      /**
       * Returns the next value based on the given arguments.
       *
       * @param nextItem {String} The CSS value of the next frame
       * @param lastItem {String} The CSS value of the last frame
       * @param timing {String} The timing used for the calculation
       * @param x {Number} The x position of the animation on the time axis
       * @return {String} The calculated value including its unit.
       */
      __getNextValue__P_155_10: function __getNextValue__P_155_10(nextItem, lastItem, timing, x) {
        var range = parseFloat(nextItem) - parseFloat(lastItem);
        return parseFloat(lastItem) + range * qx.bom.AnimationFrame.calculateTiming(timing, x) + this.__getUnit__P_155_8(nextItem);
      },

      /**
       * Internal helper for the {@link qx.bom.element.AnimationHandle} to play
       * the animation.
       * @internal
       * @param handle {qx.bom.element.AnimationHandle} The hand which
       *   represents the animation.
       * @return {qx.bom.element.AnimationHandle} The handle for chaining.
       */
      play: function play(handle) {
        handle.emit("start", handle.el);
        var id = window.setInterval(function () {
          handle.repeatSteps--;
          var values = handle.delta[handle.i % handle.steps]; // save the init values

          if (handle.i === 0) {
            for (var name in values) {
              if (handle.initValues[name] === undefined) {
                // animate element property
                if (handle.el[name] !== undefined) {
                  handle.initValues[name] = handle.el[name];
                } // animate CSS property
                else if (qx.bom.element.Style) {
                    handle.initValues[name] = qx.bom.element.Style.get(handle.el, qx.lang.String.camelCase(name));
                  } else {
                    handle.initValues[name] = handle.el.style[qx.lang.String.camelCase(name)];
                  }
              }
            }
          }

          qx.bom.element.AnimationJs.__applyStyles__P_155_11(handle.el, values);

          handle.i++; // iteration condition

          if (handle.i % handle.steps == 0) {
            handle.emit("iteration", handle.el);

            if (handle.desc.alternate) {
              handle.delta.reverse();
            }
          } // end condition


          if (handle.repeatSteps < 0) {
            qx.bom.element.AnimationJs.stop(handle);
          }
        }, handle.stepTime);
        handle.animationId = id;
        return handle;
      },

      /**
       * Internal helper for the {@link qx.bom.element.AnimationHandle} to pause
       * the animation.
       * @internal
       * @param handle {qx.bom.element.AnimationHandle} The hand which
       *   represents the animation.
       * @return {qx.bom.element.AnimationHandle} The handle for chaining.
       */
      pause: function pause(handle) {
        // stop the interval
        window.clearInterval(handle.animationId);
        handle.animationId = null;
        return handle;
      },

      /**
       * Internal helper for the {@link qx.bom.element.AnimationHandle} to stop
       * the animation.
       * @internal
       * @param handle {qx.bom.element.AnimationHandle} The hand which
       *   represents the animation.
       * @return {qx.bom.element.AnimationHandle} The handle for chaining.
       */
      stop: function stop(handle) {
        var desc = handle.desc;
        var el = handle.el;
        var initValues = handle.initValues;

        if (handle.animationId) {
          window.clearInterval(handle.animationId);
        } // clear the delay if the animation has not been started


        if (handle.delayId) {
          window.clearTimeout(handle.delayId);
        } // check if animation is already stopped


        if (el == undefined) {
          return handle;
        } // if we should keep a frame


        var keep = desc.keep;

        if (keep != undefined && !handle.stopped) {
          if (handle.reverse || desc.alternate && desc.repeat && desc.repeat % 2 == 0) {
            keep = 100 - keep;
          }

          this.__applyStyles__P_155_11(el, this.__normalizeKeyFrameTransforms__P_155_9(desc.keyFrames[keep]));
        } else {
          this.__applyStyles__P_155_11(el, initValues);
        }

        el.$$animation = null;
        handle.el = null;
        handle.ended = true;
        handle.animationId = null;
        handle.emit("end", el);
        return handle;
      },

      /**
       * Takes care of the repeat key of the description.
       * @param steps {Integer} The number of steps one iteration would take.
       * @param repeat {Integer|String} It can be either a number how often the
       * animation should be repeated or the string 'infinite'.
       * @return {Integer} The number of steps to animate.
       */
      __applyRepeat__P_155_7: function __applyRepeat__P_155_7(steps, repeat) {
        if (repeat == undefined) {
          return steps;
        }

        if (repeat == "infinite") {
          return Number.MAX_VALUE;
        }

        return steps * repeat;
      },

      /**
       * Central method to apply css styles and element properties.
       * @param el {Element} The DOM element to apply the styles.
       * @param styles {Map} A map containing styles and values.
       */
      __applyStyles__P_155_11: function __applyStyles__P_155_11(el, styles) {
        for (var key in styles) {
          // ignore undefined values (might be a bad detection)
          if (styles[key] === undefined) {
            continue;
          } // apply element property value - only if a CSS property
          // is *not* available


          if (typeof el.style[key] === "undefined" && key in el) {
            el[key] = styles[key];
            continue;
          }

          var name = qx.bom.Style.getPropertyName(key) || key;

          if (qx.bom.element.Style) {
            qx.bom.element.Style.set(el, name, styles[key]);
          } else {
            el.style[name] = styles[key];
          }
        }
      },

      /**
       * Dynamic calculation of the steps time considering a max step time.
       * @param duration {Number} The duration of the animation.
       * @param keys {Array} An array containing the ordered set of key frame keys.
       * @return {Integer} The best suited step time.
       */
      __getStepTime__P_155_4: function __getStepTime__P_155_4(duration, keys) {
        // get min difference
        var minDiff = 100;

        for (var i = 0; i < keys.length - 1; i++) {
          minDiff = Math.min(minDiff, keys[i + 1] - keys[i]);
        }

        ;
        var stepTime = duration * minDiff / 100;

        while (stepTime > this.__maxStepTime__P_155_0) {
          stepTime = stepTime / 2;
        }

        return Math.round(stepTime);
      },

      /**
       * Helper which returns the ordered keys of the key frame map.
       * @param keyFrames {Map} The map of key frames.
       * @return {Array} An ordered list of keys.
       */
      __getOrderedKeys__P_155_3: function __getOrderedKeys__P_155_3(keyFrames) {
        var keys = Object.keys(keyFrames);

        for (var i = 0; i < keys.length; i++) {
          keys[i] = parseInt(keys[i], 10);
        }

        ;
        keys.sort(function (a, b) {
          return a - b;
        });
        return keys;
      }
    }
  });
  qx.bom.element.AnimationJs.$$dbClassInfo = $$dbClassInfo;
})();

(function () {
  var $$dbClassInfo = {
    "dependsOn": {
      "qx.Class": {
        "usage": "dynamic",
        "require": true
      },
      "qx.event.type.Mouse": {
        "require": true
      },
      "qx.util.Wheel": {}
    }
  };
  qx.Bootstrap.executePendingDefers($$dbClassInfo);

  /* ************************************************************************
  
     qooxdoo - the new era of web development
  
     http://qooxdoo.org
  
     Copyright:
       2004-2009 1&1 Internet AG, Germany, http://www.1und1.de
  
     License:
       MIT: https://opensource.org/licenses/MIT
       See the LICENSE file in the project's top-level directory for details.
  
     Authors:
       * Fabian Jakobs (fjakobs)
       * Martin Wittemann (martinwittemann)
  
  ************************************************************************ */

  /**
   * Mouse wheel event object.
   */
  qx.Class.define("qx.event.type.MouseWheel", {
    extend: qx.event.type.Mouse,
    members: {
      // overridden
      stop: function stop() {
        this.stopPropagation();
        this.preventDefault();
      },

      /**
       * Get the amount the wheel has been scrolled
       *
       * @param axis {String?} Optional parameter which defines the scroll axis.
       *   The value can either be <code>"x"</code> or <code>"y"</code>.
       * @return {Integer} Scroll wheel movement for the given axis. If no axis
       *   is given, the y axis is used.
       */
      getWheelDelta: function getWheelDelta(axis) {
        return qx.util.Wheel.getDelta(this._native, axis);
      }
    }
  });
  qx.event.type.MouseWheel.$$dbClassInfo = $$dbClassInfo;
})();

function _typeof(obj) { "@babel/helpers - typeof"; if (typeof Symbol === "function" && typeof Symbol.iterator === "symbol") { _typeof = function _typeof(obj) { return typeof obj; }; } else { _typeof = function _typeof(obj) { return obj && typeof Symbol === "function" && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj; }; } return _typeof(obj); }

(function () {
  var $$dbClassInfo = {
    "dependsOn": {
      "qx.Bootstrap": {
        "usage": "dynamic",
        "require": true
      },
      "qx.bom.client.Engine": {},
      "qx.bom.client.Browser": {},
      "qx.core.Environment": {
        "defer": "runtime"
      }
    },
    "environment": {
      "provided": ["plugin.gears", "plugin.quicktime", "plugin.quicktime.version", "plugin.windowsmedia", "plugin.windowsmedia.version", "plugin.divx", "plugin.divx.version", "plugin.silverlight", "plugin.silverlight.version", "plugin.pdf", "plugin.pdf.version", "plugin.activex", "plugin.skype"],
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
   * Contains detection for QuickTime, Windows Media, DivX, Silverlight and gears.
   * If no version could be detected the version is set to an empty string as
   * default.
   *
   * This class is used by {@link qx.core.Environment} and should not be used
   * directly. Please check its class comment for details how to use it.
   *
   * @internal
   */
  qx.Bootstrap.define("qx.bom.client.Plugin", {
    statics: {
      /**
       * Checks for the availability of google gears plugin.
       *
       * @internal
       * @return {Boolean} <code>true</code> if gears is available
       */
      getGears: function getGears() {
        return !!(window.google && window.google.gears);
      },

      /**
       * Checks for ActiveX availability.
       *
       * @internal
       * @return {Boolean} <code>true</code> if ActiveX is available
       *
       * @ignore(window.ActiveXObject)
       */
      getActiveX: function getActiveX() {
        if (typeof window.ActiveXObject === "function") {
          return true;
        }

        try {
          // in IE11 Preview, ActiveXObject is undefined but instances can
          // still be created
          return window.ActiveXObject !== undefined && (_typeof(new window.ActiveXObject("Microsoft.XMLHTTP")) === "object" || _typeof(new window.ActiveXObject("MSXML2.DOMDocument.6.0")) === "object");
        } catch (ex) {
          return false;
        }
      },

      /**
       * Checks for Skypes 'Click to call' availability.
       *
       * @internal
       * @return {Boolean} <code>true</code> if the plugin is available.
       */
      getSkype: function getSkype() {
        // IE Support
        if (qx.bom.client.Plugin.getActiveX()) {
          try {
            new window.ActiveXObject("Skype.Detection");
            return true;
          } catch (e) {}
        }

        var mimeTypes = navigator.mimeTypes;

        if (mimeTypes) {
          // FF support
          if ("application/x-skype" in mimeTypes) {
            return true;
          } // webkit support


          for (var i = 0; i < mimeTypes.length; i++) {
            var desc = mimeTypes[i];

            if (desc.type.indexOf("skype.click2call") != -1) {
              return true;
            }
          }

          ;
        }

        return false;
      },

      /**
       * Database of supported features.
       * Filled with additional data at initialization
       */
      __db__P_159_0: {
        quicktime: {
          plugin: ["QuickTime"],
          control: "QuickTimeCheckObject.QuickTimeCheck.1" // call returns boolean: instance.IsQuickTimeAvailable(0)

        },
        wmv: {
          plugin: ["Windows Media"],
          control: "WMPlayer.OCX.7" // version string in: instance.versionInfo

        },
        divx: {
          plugin: ["DivX Web Player"],
          control: "npdivx.DivXBrowserPlugin.1"
        },
        silverlight: {
          plugin: ["Silverlight"],
          control: "AgControl.AgControl" // version string in: instance.version (Silverlight 1.0)
          // version string in: instance.settings.version (Silverlight 1.1)
          // version check possible using instance.IsVersionSupported

        },
        pdf: {
          plugin: ["Chrome PDF Viewer", "Adobe Acrobat"],
          control: "AcroPDF.PDF" // this is detecting Acrobat PDF version > 7 and Chrome PDF Viewer

        }
      },

      /**
       * Fetches the version of the quicktime plugin.
       * @return {String} The version of the plugin, if available,
       *   an empty string otherwise
       * @internal
       */
      getQuicktimeVersion: function getQuicktimeVersion() {
        var entry = qx.bom.client.Plugin.__db__P_159_0["quicktime"];
        return qx.bom.client.Plugin.__getVersion__P_159_1(entry.control, entry.plugin);
      },

      /**
       * Fetches the version of the windows media plugin.
       * @return {String} The version of the plugin, if available,
       *   an empty string otherwise
       * @internal
       */
      getWindowsMediaVersion: function getWindowsMediaVersion() {
        var entry = qx.bom.client.Plugin.__db__P_159_0["wmv"];
        return qx.bom.client.Plugin.__getVersion__P_159_1(entry.control, entry.plugin, true);
      },

      /**
       * Fetches the version of the divx plugin.
       * @return {String} The version of the plugin, if available,
       *   an empty string otherwise
       * @internal
       */
      getDivXVersion: function getDivXVersion() {
        var entry = qx.bom.client.Plugin.__db__P_159_0["divx"];
        return qx.bom.client.Plugin.__getVersion__P_159_1(entry.control, entry.plugin);
      },

      /**
       * Fetches the version of the silverlight plugin.
       * @return {String} The version of the plugin, if available,
       *   an empty string otherwise
       * @internal
       */
      getSilverlightVersion: function getSilverlightVersion() {
        var entry = qx.bom.client.Plugin.__db__P_159_0["silverlight"];
        return qx.bom.client.Plugin.__getVersion__P_159_1(entry.control, entry.plugin);
      },

      /**
       * Fetches the version of the pdf plugin.
       *
       * There are two built-in PDF viewer shipped with browsers:
       *
       * <ul>
       *  <li>Chrome PDF Viewer</li>
       *  <li>PDF.js (Firefox)</li>
       * </ul>
       *
       * While the Chrome PDF Viewer is implemented as plugin and therefore
       * detected by this method PDF.js is <strong>not</strong>.
       *
       * See the dedicated environment key (<em>plugin.pdfjs</em>) instead,
       * which you might check additionally.
       *
       * @return {String} The version of the plugin, if available,
       *  an empty string otherwise
       * @internal
       */
      getPdfVersion: function getPdfVersion() {
        var entry = qx.bom.client.Plugin.__db__P_159_0["pdf"];
        return qx.bom.client.Plugin.__getVersion__P_159_1(entry.control, entry.plugin);
      },

      /**
       * Checks if the quicktime plugin is available.
       * @return {Boolean} <code>true</code> if the plugin is available
       * @internal
       */
      getQuicktime: function getQuicktime() {
        var entry = qx.bom.client.Plugin.__db__P_159_0["quicktime"];
        return qx.bom.client.Plugin.__isAvailable__P_159_2(entry.control, entry.plugin);
      },

      /**
       * Checks if the windows media plugin is available.
       * @return {Boolean} <code>true</code> if the plugin is available
       * @internal
       */
      getWindowsMedia: function getWindowsMedia() {
        var entry = qx.bom.client.Plugin.__db__P_159_0["wmv"];
        return qx.bom.client.Plugin.__isAvailable__P_159_2(entry.control, entry.plugin, true);
      },

      /**
       * Checks if the divx plugin is available.
       * @return {Boolean} <code>true</code> if the plugin is available
       * @internal
       */
      getDivX: function getDivX() {
        var entry = qx.bom.client.Plugin.__db__P_159_0["divx"];
        return qx.bom.client.Plugin.__isAvailable__P_159_2(entry.control, entry.plugin);
      },

      /**
       * Checks if the silverlight plugin is available.
       * @return {Boolean} <code>true</code> if the plugin is available
       * @internal
       */
      getSilverlight: function getSilverlight() {
        var entry = qx.bom.client.Plugin.__db__P_159_0["silverlight"];
        return qx.bom.client.Plugin.__isAvailable__P_159_2(entry.control, entry.plugin);
      },

      /**
       * Checks if the pdf plugin is available.
       *
       * There are two built-in PDF viewer shipped with browsers:
       *
       * <ul>
       *  <li>Chrome PDF Viewer</li>
       *  <li>PDF.js (Firefox)</li>
       * </ul>
       *
       * While the Chrome PDF Viewer is implemented as plugin and therefore
       * detected by this method PDF.js is <strong>not</strong>.
       *
       * See the dedicated environment key (<em>plugin.pdfjs</em>) instead,
       * which you might check additionally.
       *
       * @return {Boolean} <code>true</code> if the plugin is available
       * @internal
       */
      getPdf: function getPdf() {
        var entry = qx.bom.client.Plugin.__db__P_159_0["pdf"];
        return qx.bom.client.Plugin.__isAvailable__P_159_2(entry.control, entry.plugin);
      },

      /**
       * Internal helper for getting the version of a given plugin.
       *
       * @param activeXName {String} The name which should be used to generate
       *   the test ActiveX Object.
       * @param pluginNames {Array} The names with which the plugins are listed in
       *   the navigator.plugins list.
       * @param forceActiveX {Boolean?false} Force detection using ActiveX
       *   for IE11 plugins that aren't listed in navigator.plugins
       * @return {String} The version of the plugin as string.
       */
      __getVersion__P_159_1: function __getVersion__P_159_1(activeXName, pluginNames, forceActiveX) {
        var available = qx.bom.client.Plugin.__isAvailable__P_159_2(activeXName, pluginNames, forceActiveX); // don't check if the plugin is not available


        if (!available) {
          return "";
        } // IE checks


        if (qx.bom.client.Engine.getName() == "mshtml" && (qx.bom.client.Browser.getDocumentMode() < 11 || forceActiveX)) {
          try {
            var obj = new window.ActiveXObject(activeXName);
            var version; // pdf version detection

            if (obj.GetVersions && obj.GetVersions()) {
              version = obj.GetVersions().split(',');

              if (version.length > 1) {
                version = version[0].split('=');

                if (version.length === 2) {
                  return version[1];
                }
              }
            }

            version = obj.versionInfo;

            if (version != undefined) {
              return version;
            }

            version = obj.version;

            if (version != undefined) {
              return version;
            }

            version = obj.settings.version;

            if (version != undefined) {
              return version;
            }
          } catch (ex) {
            return "";
          }

          return ""; // all other browsers
        } else {
          var plugins = navigator.plugins;
          var verreg = /([0-9]\.[0-9])/g;

          for (var i = 0; i < plugins.length; i++) {
            var plugin = plugins[i];

            for (var j = 0; j < pluginNames.length; j++) {
              if (plugin.name.indexOf(pluginNames[j]) !== -1) {
                if (verreg.test(plugin.name) || verreg.test(plugin.description)) {
                  return RegExp.$1;
                }
              }
            }
          }

          return "";
        }
      },

      /**
       * Internal helper for getting the availability of a given plugin.
       *
       * @param activeXName {String} The name which should be used to generate
       *   the test ActiveX Object.
       * @param pluginNames {Array} The names with which the plugins are listed in
       *   the navigator.plugins list.
       * @param forceActiveX {Boolean?false} Force detection using ActiveX
       *   for IE11 plugins that aren't listed in navigator.plugins
       * @return {Boolean} <code>true</code>, if the plugin available
       */
      __isAvailable__P_159_2: function __isAvailable__P_159_2(activeXName, pluginNames, forceActiveX) {
        // IE checks
        if (qx.bom.client.Engine.getName() == "mshtml" && (qx.bom.client.Browser.getDocumentMode() < 11 || forceActiveX)) {
          if (!this.getActiveX()) {
            return false;
          }

          try {
            new window.ActiveXObject(activeXName);
          } catch (ex) {
            return false;
          }

          return true; // all other
        } else {
          var plugins = navigator.plugins;

          if (!plugins) {
            return false;
          }

          var name;

          for (var i = 0; i < plugins.length; i++) {
            name = plugins[i].name;

            for (var j = 0; j < pluginNames.length; j++) {
              if (name.indexOf(pluginNames[j]) !== -1) {
                return true;
              }
            }
          }

          return false;
        }
      }
    },
    defer: function defer(statics) {
      qx.core.Environment.add("plugin.gears", statics.getGears);
      qx.core.Environment.add("plugin.quicktime", statics.getQuicktime);
      qx.core.Environment.add("plugin.quicktime.version", statics.getQuicktimeVersion);
      qx.core.Environment.add("plugin.windowsmedia", statics.getWindowsMedia);
      qx.core.Environment.add("plugin.windowsmedia.version", statics.getWindowsMediaVersion);
      qx.core.Environment.add("plugin.divx", statics.getDivX);
      qx.core.Environment.add("plugin.divx.version", statics.getDivXVersion);
      qx.core.Environment.add("plugin.silverlight", statics.getSilverlight);
      qx.core.Environment.add("plugin.silverlight.version", statics.getSilverlightVersion);
      qx.core.Environment.add("plugin.pdf", statics.getPdf);
      qx.core.Environment.add("plugin.pdf.version", statics.getPdfVersion);
      qx.core.Environment.add("plugin.activex", statics.getActiveX);
      qx.core.Environment.add("plugin.skype", statics.getSkype);
    }
  });
  qx.bom.client.Plugin.$$dbClassInfo = $$dbClassInfo;
})();

(function () {
  var $$dbClassInfo = {
    "dependsOn": {
      "qx.core.Environment": {
        "defer": "load",
        "require": true
      },
      "qx.Bootstrap": {
        "usage": "dynamic",
        "require": true
      },
      "qx.bom.client.Plugin": {
        "defer": "runtime"
      },
      "qx.bom.client.Xml": {}
    },
    "environment": {
      "provided": [],
      "required": {
        "plugin.activex": {
          "className": "qx.bom.client.Plugin",
          "defer": true
        },
        "xml.implementation": {
          "className": "qx.bom.client.Xml"
        },
        "xml.domparser": {
          "className": "qx.bom.client.Xml"
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
  
  ************************************************************************ */

  /**
   * Cross browser XML document creation API
   *
   * The main purpose of this class is to allow you to create XML document objects in a
   * cross-browser fashion. Use <code>create</code> to create an empty document,
   * <code>fromString</code> to create one from an existing XML text. Both methods
   * return a *native DOM object*. That means you use standard DOM methods on such
   * an object (e.g. <code>createElement</code>).
   *
   * The following links provide further information on XML documents:
   *
   * * <a href="http://www.w3.org/TR/DOM-Level-2-Core/core.html#i-Document">W3C Interface Specification</a>
   * * <a href="http://msdn2.microsoft.com/en-us/library/ms535918.aspx">MS xml Object</a>
   * * <a href="http://msdn2.microsoft.com/en-us/library/ms764622.aspx">MSXML GUIDs and ProgIDs</a>
   * * <a href="https://developer.mozilla.org/en-US/docs/Parsing_and_serializing_XML">MDN Parsing and Serializing XML</a>
   */
  qx.Bootstrap.define("qx.xml.Document", {
    statics: {
      /** @type {String} ActiveX class name of DOMDocument (IE specific) */
      DOMDOC: null,

      /** @type {String} ActiveX class name of XMLHttpRequest (IE specific) */
      XMLHTTP: null,

      /**
       * Whether the given element is a XML document or element
       * which is part of a XML document.
       *
       * @param elem {Document|Element} Any DOM Document or Element
       * @return {Boolean} Whether the document is a XML document
       */
      isXmlDocument: function isXmlDocument(elem) {
        if (elem.nodeType === 9) {
          return elem.documentElement.nodeName !== "HTML";
        } else if (elem.ownerDocument) {
          return this.isXmlDocument(elem.ownerDocument);
        } else {
          return false;
        }
      },

      /**
       * Create an XML document.
       *
       * Returns a native DOM document object, set up for XML.
       *
       * @param namespaceUri {String ? null} The namespace URI of the document element to create or null.
       * @param qualifiedName {String ? null} The qualified name of the document element to be created or null.
       * @return {Document} empty XML object
       *
       * @ignore(ActiveXObject)
       */
      create: function create(namespaceUri, qualifiedName) {
        // ActiveX - This is the preferred way for IE9 as well since it has no XPath
        // support when using the native implementation.createDocument
        if (qx.core.Environment.get("plugin.activex")) {
          var obj = new ActiveXObject(this.DOMDOC); //The SelectionLanguage property is no longer needed in MSXML 6; trying
          // to set it causes an exception in IE9.

          if (this.DOMDOC == "MSXML2.DOMDocument.3.0") {
            obj.setProperty("SelectionLanguage", "XPath");
          }

          if (qualifiedName) {
            var str = '<\?xml version="1.0" encoding="utf-8"?>\n<';
            str += qualifiedName;

            if (namespaceUri) {
              str += " xmlns='" + namespaceUri + "'";
            }

            str += " />";
            obj.loadXML(str);
          }

          return obj;
        }

        if (qx.core.Environment.get("xml.implementation")) {
          return document.implementation.createDocument(namespaceUri || "", qualifiedName || "", null);
        }

        throw new Error("No XML implementation available!");
      },

      /**
       * The string passed in is parsed into a DOM document.
       *
       * @param str {String} the string to be parsed
       * @return {Document} XML document with given content
       * @signature function(str)
       *
       * @ignore(DOMParser)
       */
      fromString: function fromString(str) {
        // Legacy IE/ActiveX
        if (qx.core.Environment.get("plugin.activex")) {
          var dom = qx.xml.Document.create();
          dom.loadXML(str);
          return dom;
        }

        if (qx.core.Environment.get("xml.domparser")) {
          var parser = new DOMParser();
          return parser.parseFromString(str, "text/xml");
        }

        throw new Error("No XML implementation available!");
      }
    },

    /*
    *****************************************************************************
       DEFER
    *****************************************************************************
    */
    defer: function defer(statics) {
      // Detecting available ActiveX implementations.
      if (qx.core.Environment.get("plugin.activex")) {
        // According to information on the Microsoft XML Team's WebLog
        // it is recommended to check for availability of MSXML versions 6.0 and 3.0.
        // http://blogs.msdn.com/xmlteam/archive/2006/10/23/using-the-right-version-of-msxml-in-internet-explorer.aspx
        var domDoc = ["MSXML2.DOMDocument.6.0", "MSXML2.DOMDocument.3.0"];
        var httpReq = ["MSXML2.XMLHTTP.6.0", "MSXML2.XMLHTTP.3.0"];

        for (var i = 0, l = domDoc.length; i < l; i++) {
          try {
            // Keep both objects in sync with the same version.
            // This is important as there were compatibility issues detected.
            new ActiveXObject(domDoc[i]);
            new ActiveXObject(httpReq[i]);
          } catch (ex) {
            continue;
          } // Update static constants


          statics.DOMDOC = domDoc[i];
          statics.XMLHTTP = httpReq[i]; // Stop loop here

          break;
        }
      }
    }
  });
  qx.xml.Document.$$dbClassInfo = $$dbClassInfo;
})();

(function () {
  var $$dbClassInfo = {
    "dependsOn": {
      "qx.core.Environment": {
        "defer": "load",
        "usage": "dynamic",
        "require": true
      },
      "qx.Bootstrap": {
        "usage": "dynamic",
        "require": true
      },
      "qx.bom.client.Html": {
        "require": true
      },
      "qx.dom.Node": {},
      "qx.bom.Selection": {}
    },
    "environment": {
      "provided": [],
      "required": {
        "html.selection": {
          "load": true,
          "className": "qx.bom.client.Html"
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
       * Alexander Steitz (aback)
  
  ************************************************************************ */

  /**
   * Low-level Range API which is used together with the low-level Selection API.
   * This is especially useful whenever a developer want to work on text level,
   * e.g. for an editor.
   */
  qx.Bootstrap.define("qx.bom.Range", {
    /*
    *****************************************************************************
       STATICS
    *****************************************************************************
    */
    statics: {
      /**
       * Returns the range object of the given node.
       *
       * @signature function(node)
       * @param node {Node} node to get the range of
       * @return {Range} valid range of given selection
       */
      get: qx.core.Environment.select("html.selection", {
        "selection": function selection(node) {
          // check for the type of the given node
          // for legacy IE the nodes input, textarea, button and body
          // have access to own TextRange objects. Everything else is
          // gathered via the selection object.
          if (qx.dom.Node.isElement(node)) {
            switch (node.nodeName.toLowerCase()) {
              case "input":
                switch (node.type) {
                  case "text":
                  case "password":
                  case "hidden":
                  case "button":
                  case "reset":
                  case "file":
                  case "submit":
                    return node.createTextRange();

                  default:
                    return qx.bom.Selection.getSelectionObject(qx.dom.Node.getDocument(node)).createRange();
                }

                break;

              case "textarea":
              case "body":
              case "button":
                return node.createTextRange();

              default:
                return qx.bom.Selection.getSelectionObject(qx.dom.Node.getDocument(node)).createRange();
            }
          } else {
            if (node == null) {
              node = window;
            } // need to pass the document node to work with multi-documents


            return qx.bom.Selection.getSelectionObject(qx.dom.Node.getDocument(node)).createRange();
          }
        },
        // suitable for gecko, opera and webkit
        "default": function _default(node) {
          var doc = qx.dom.Node.getDocument(node); // get the selection object of the corresponding document

          var sel = qx.bom.Selection.getSelectionObject(doc);

          if (sel.rangeCount > 0) {
            return sel.getRangeAt(0);
          } else {
            return doc.createRange();
          }
        }
      })
    }
  });
  qx.bom.Range.$$dbClassInfo = $$dbClassInfo;
})();

(function () {
  var $$dbClassInfo = {
    "dependsOn": {
      "qx.Bootstrap": {
        "usage": "dynamic",
        "require": true
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
       * Adrian Olaru (adrianolaru)
  
     ======================================================================
  
     This class contains code based on the following work:
  
     * Cross-Browser Split
       http://blog.stevenlevithan.com/archives/cross-browser-split
       Version 1.0.1
  
       Copyright:
         (c) 2006-2007, Steven Levithan <http://stevenlevithan.com>
  
       License:
         MIT: http://www.opensource.org/licenses/mit-license.php
  
       Authors:
         * Steven Levithan
  
  ************************************************************************ */

  /**
   * Implements an ECMA-compliant, uniform cross-browser split method
   */
  qx.Bootstrap.define("qx.util.StringSplit", {
    statics: {
      /**
       * ECMA-compliant, uniform cross-browser split method
       *
       * @param str {String} Incoming string to split
       * @param separator {RegExp} Specifies the character to use for separating the string.
       *   The separator is treated as a string or a  regular expression. If separator is
       *   omitted, the array returned contains one element consisting of the entire string.
       * @param limit {Integer?} Integer specifying a limit on the number of splits to be found.
       * @return {String[]} split string
       */
      split: function split(str, separator, limit) {
        // if `separator` is not a regex, use the native `split`
        if (Object.prototype.toString.call(separator) !== "[object RegExp]") {
          return String.prototype.split.call(str, separator, limit);
        }

        var output = [],
            lastLastIndex = 0,
            flags = (separator.ignoreCase ? "i" : "") + (separator.multiline ? "m" : "") + (separator.sticky ? "y" : ""),
            separator = RegExp(separator.source, flags + "g"),
            // make `global` and avoid `lastIndex` issues by working with a copy
        separator2,
            match,
            lastIndex,
            lastLength,
            compliantExecNpcg = /()??/.exec("")[1] === undefined; // NPCG: nonparticipating capturing group

        str = str + ""; // type conversion

        if (!compliantExecNpcg) {
          separator2 = RegExp("^" + separator.source + "$(?!\\s)", flags); // doesn't need /g or /y, but they don't hurt
        }
        /* behavior for `limit`: if it's...
        - `undefined`: no limit.
        - `NaN` or zero: return an empty array.
        - a positive number: use `Math.floor(limit)`.
        - a negative number: no limit.
        - other: type-convert, then use the above rules. */


        if (limit === undefined || +limit < 0) {
          limit = Infinity;
        } else {
          limit = Math.floor(+limit);

          if (!limit) {
            return [];
          }
        }

        while (match = separator.exec(str)) {
          lastIndex = match.index + match[0].length; // `separator.lastIndex` is not reliable cross-browser

          if (lastIndex > lastLastIndex) {
            output.push(str.slice(lastLastIndex, match.index)); // fix browsers whose `exec` methods don't consistently return `undefined` for nonparticipating capturing groups

            if (!compliantExecNpcg && match.length > 1) {
              match[0].replace(separator2, function () {
                for (var i = 1; i < arguments.length - 2; i++) {
                  if (arguments[i] === undefined) {
                    match[i] = undefined;
                  }
                }
              });
            }

            if (match.length > 1 && match.index < str.length) {
              Array.prototype.push.apply(output, match.slice(1));
            }

            lastLength = match[0].length;
            lastLastIndex = lastIndex;

            if (output.length >= limit) {
              break;
            }
          }

          if (separator.lastIndex === match.index) {
            separator.lastIndex++; // avoid an infinite loop
          }
        }

        if (lastLastIndex === str.length) {
          if (lastLength || !separator.test("")) {
            output.push("");
          }
        } else {
          output.push(str.slice(lastLastIndex));
        }

        return output.length > limit ? output.slice(0, limit) : output;
      }
    }
  });
  qx.util.StringSplit.$$dbClassInfo = $$dbClassInfo;
})();

(function () {
  var $$dbClassInfo = {
    "dependsOn": {
      "qx.Bootstrap": {
        "usage": "dynamic",
        "require": true
      },
      "qx.lang.Type": {},
      "qx.lang.Object": {}
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
       * Martin Wittemann (wittemann)
  
  ************************************************************************ */

  /**
   * Define messages to react on certain channels.
   *
   * The channel names will be used in the {@link #on} method to define handlers which will
   * be called on certain channels and routes. The {@link #emit} method can be used
   * to execute a given route on a channel. {@link #onAny} defines a handler on any channel.
   *
   * *Example*
   *
   * Here is a little example of how to use the messaging.
   *
   * <pre class='javascript'>
   *   var m = new qx.event.Messaging();
   *
   *   m.on("get", "/address/{id}", function(data) {
   *     var id = data.params.id; // 1234
   *     // do something with the id...
   *   },this);
   *
   *   m.emit("get", "/address/1234");
   * </pre>
   */
  qx.Bootstrap.define("qx.event.Messaging", {
    construct: function construct() {
      this._listener = {}, this.__listenerIdCount__P_176_0 = 0;
      this.__channelToIdMapping__P_176_1 = {};
    },
    members: {
      _listener: null,
      __listenerIdCount__P_176_0: null,
      __channelToIdMapping__P_176_1: null,

      /**
       * Adds a route handler for the given channel. The route is called
       * if the {@link #emit} method finds a match.
       *
       * @param channel {String} The channel of the message.
       * @param type {String|RegExp} The type, used for checking if the executed path matches.
       * @param handler {Function} The handler to call if the route matches the executed path.
       * @param scope {var ? null} The scope of the handler.
       * @return {String} The id of the route used to remove the route.
       */
      on: function on(channel, type, handler, scope) {
        return this._addListener(channel, type, handler, scope);
      },

      /**
       * Adds a handler for the "any" channel. The "any" channel is called
       * before all other channels.
       *
       * @param type {String|RegExp} The route, used for checking if the executed path matches
       * @param handler {Function} The handler to call if the route matches the executed path
       * @param scope {var ? null} The scope of the handler.
       * @return {String} The id of the route used to remove the route.
       */
      onAny: function onAny(type, handler, scope) {
        return this._addListener("any", type, handler, scope);
      },

      /**
       * Adds a listener for a certain channel.
       *
       * @param channel {String} The channel the route should be registered for
       * @param type {String|RegExp} The type, used for checking if the executed path matches
       * @param handler {Function} The handler to call if the route matches the executed path
       * @param scope {var ? null} The scope of the handler.
       * @return {String} The id of the route used to remove the route.
       */
      _addListener: function _addListener(channel, type, handler, scope) {
        var listeners = this._listener[channel] = this._listener[channel] || {};
        var id = this.__listenerIdCount__P_176_0++;
        var params = [];
        var param = null; // Convert the route to a regular expression.

        if (qx.lang.Type.isString(type)) {
          var paramsRegexp = /\{([\w\d]+)\}/g;

          while ((param = paramsRegexp.exec(type)) !== null) {
            params.push(param[1]);
          }

          type = new RegExp("^" + type.replace(paramsRegexp, "([^\/]+)") + "$");
        }

        listeners[id] = {
          regExp: type,
          params: params,
          handler: handler,
          scope: scope
        };
        this.__channelToIdMapping__P_176_1[id] = channel;
        return id;
      },

      /**
       * Removes a registered listener by the given id.
       *
       * @param id {String} The id of the registered listener.
       */
      remove: function remove(id) {
        var channel = this.__channelToIdMapping__P_176_1[id];
        var listener = this._listener[channel];
        delete listener[id];
        delete this.__channelToIdMapping__P_176_1[id];
      },

      /**
       * Checks if a listener is registered for the given path in the given channel.
       *
       * @param channel {String} The channel of the message.
       * @param path {String} The path to check.
       * @return {Boolean} Whether a listener is registered.
       */
      has: function has(channel, path) {
        var listeners = this._listener[channel];

        if (!listeners || qx.lang.Object.isEmpty(listeners)) {
          return false;
        }

        for (var id in listeners) {
          var listener = listeners[id];

          if (listener.regExp.test(path)) {
            return true;
          }
        }

        return false;
      },

      /**
       * Sends a message on the given channel and informs all matching route handlers.
       *
       * @param channel {String} The channel of the message.
       * @param path {String} The path to execute
       * @param params {Map} The given parameters that should be propagated
       * @param customData {var} The given custom data that should be propagated
       */
      emit: function emit(channel, path, params, customData) {
        this._emit(channel, path, params, customData);
      },

      /**
       * Executes a certain channel with a given path. Informs all
       * route handlers that match with the path.
       *
       * @param channel {String} The channel to execute.
       * @param path {String} The path to check
       * @param params {Map} The given parameters that should be propagated
       * @param customData {var} The given custom data that should be propagated
       */
      _emit: function _emit(channel, path, params, customData) {
        var listenerMatchedAny = false;
        var listener = this._listener["any"];
        listenerMatchedAny = this._emitListeners(channel, path, listener, params, customData);
        var listenerMatched = false;
        listener = this._listener[channel];
        listenerMatched = this._emitListeners(channel, path, listener, params, customData);

        if (!listenerMatched && !listenerMatchedAny) {
          qx.Bootstrap.info("No listener found for " + path);
        }
      },

      /**
       * Executes all given listener for a certain channel. Checks all listeners if they match
       * with the given path and executes the stored handler of the matching route.
       *
       * @param channel {String} The channel to execute.
       * @param path {String} The path to check
       * @param listeners {Map[]} All routes to test and execute.
       * @param params {Map} The given parameters that should be propagated
       * @param customData {var} The given custom data that should be propagated
       *
       * @return {Boolean} Whether the route has been executed
       */
      _emitListeners: function _emitListeners(channel, path, listeners, params, customData) {
        if (!listeners || qx.lang.Object.isEmpty(listeners)) {
          return false;
        }

        var listenerMatched = false;

        for (var id in listeners) {
          var listener = listeners[id];
          listenerMatched |= this._emitRoute(channel, path, listener, params, customData);
        }

        return listenerMatched;
      },

      /**
       * Executes a certain listener. Checks if the listener matches the given path and
       * executes the stored handler of the route.
       *
       * @param channel {String} The channel to execute.
       * @param path {String} The path to check
       * @param listener {Map} The route data.
       * @param params {Map} The given parameters that should be propagated
       * @param customData {var} The given custom data that should be propagated
       *
       * @return {Boolean} Whether the route has been executed
       */
      _emitRoute: function _emitRoute(channel, path, listener, params, customData) {
        var match = listener.regExp.exec(path);

        if (match) {
          var params = params || {};
          var param = null;
          var value = null;
          match.shift(); // first match is the whole path

          for (var i = 0; i < match.length; i++) {
            value = match[i];
            param = listener.params[i];

            if (param) {
              params[param] = value;
            } else {
              params[i] = value;
            }
          }

          listener.handler.call(listener.scope, {
            path: path,
            params: params,
            customData: customData
          });
        }

        return match != undefined;
      }
    }
  });
  qx.event.Messaging.$$dbClassInfo = $$dbClassInfo;
})();

(function () {
  var $$dbClassInfo = {
    "dependsOn": {
      "qx.core.Environment": {
        "defer": "load",
        "usage": "dynamic",
        "require": true
      },
      "qx.Class": {
        "usage": "dynamic",
        "require": true
      },
      "qx.core.Object": {
        "construct": true,
        "require": true
      },
      "qx.bom.client.Event": {
        "require": true
      },
      "qx.bom.client.Browser": {},
      "qx.bom.HashHistory": {},
      "qx.bom.client.Engine": {},
      "qx.bom.IframeHistory": {},
      "qx.bom.NativeHistory": {},
      "qx.lang.Type": {},
      "qx.event.Timer": {}
    },
    "environment": {
      "provided": [],
      "required": {
        "event.hashchange": {
          "load": true,
          "className": "qx.bom.client.Event"
        },
        "browser.documentmode": {
          "className": "qx.bom.client.Browser"
        },
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
       2004-2008 1&1 Internet AG, Germany, http://www.1und1.de
  
     License:
       MIT: https://opensource.org/licenses/MIT
       See the LICENSE file in the project's top-level directory for details.
  
     Authors:
       * Sebastian Werner (wpbasti)
       * Andreas Ecker (ecker)
       * Fabian Jakobs (fjakobs)
  
     ======================================================================
  
     This class contains code based on the following work:
  
     * Yahoo! UI Library
       http://developer.yahoo.com/yui
       Version 2.2.0
  
       Copyright:
         (c) 2007, Yahoo! Inc.
  
       License:
         BSD: http://developer.yahoo.com/yui/license.txt
  
     ----------------------------------------------------------------------
  
       http://developer.yahoo.com/yui/license.html
  
       Copyright (c) 2009, Yahoo! Inc.
       All rights reserved.
  
       Redistribution and use of this software in source and binary forms,
       with or without modification, are permitted provided that the
       following conditions are met:
  
       * Redistributions of source code must retain the above copyright
         notice, this list of conditions and the following disclaimer.
       * Redistributions in binary form must reproduce the above copyright
         notice, this list of conditions and the following disclaimer in
         the documentation and/or other materials provided with the
         distribution.
       * Neither the name of Yahoo! Inc. nor the names of its contributors
         may be used to endorse or promote products derived from this
         software without specific prior written permission of Yahoo! Inc.
  
       THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
       "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
       LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
       FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE
       COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
       INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
       (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
       SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
       HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
       STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
       ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED
       OF THE POSSIBILITY OF SUCH DAMAGE.
  
  ************************************************************************ */

  /* ************************************************************************
  
  
  ************************************************************************ */

  /**
   * A helper for using the browser history in JavaScript Applications without
   * reloading the main page.
   *
   * Adds entries to the browser history and fires a "request" event when one of
   * the entries was requested by the user (e.g. by clicking on the back button).
   *
   * This class is an abstract template class. Concrete implementations have to
   * provide implementations for the {@link #_readState} and {@link #_writeState}
   * methods.
   *
   * Browser history support is currently available for Internet Explorer 6/7,
   * Firefox, Opera 9 and WebKit. Safari 2 and older are not yet supported.
   *
   * This module is based on the ideas behind the YUI Browser History Manager
   * by Julien Lecomte (Yahoo), which is described at
   * http://yuiblog.com/blog/2007/02/21/browser-history-manager/. The Yahoo
   * implementation can be found at http://developer.yahoo.com/yui/history/.
   * The original code is licensed under a BSD license
   * (http://developer.yahoo.com/yui/license.txt).
   *
   * @asset(qx/static/blank.html)
   */
  qx.Class.define("qx.bom.History", {
    extend: qx.core.Object,
    type: "abstract",

    /*
    *****************************************************************************
       CONSTRUCTOR
    *****************************************************************************
    */
    construct: function construct() {
      qx.core.Object.constructor.call(this);
      this._baseUrl = window.location.href.split('#')[0] + '#';
      this._titles = {};

      this._setInitialState();
    },

    /*
    *****************************************************************************
       EVENTS
    *****************************************************************************
    */
    events: {
      /**
       * Fired when the user moved in the history. The data property of the event
       * holds the state, which was passed to {@link #addToHistory}.
       */
      "request": "qx.event.type.Data"
    },

    /*
    *****************************************************************************
       STATICS
    *****************************************************************************
    */
    statics: {
      /**
       * @type {Boolean} Whether the browser supports the 'hashchange' event natively.
       */
      SUPPORTS_HASH_CHANGE_EVENT: qx.core.Environment.get("event.hashchange"),

      /**
       * Get the singleton instance of the history manager.
       *
       * @return {History}
       */
      getInstance: function getInstance() {
        var runsInIframe = !(window == window.top);

        if (!this.$$instance) {
          // in iframe + IE9
          if (runsInIframe && qx.core.Environment.get("browser.documentmode") == 9) {
            this.$$instance = new qx.bom.HashHistory();
          } // in iframe + IE<9
          else if (runsInIframe && qx.core.Environment.get("engine.name") == "mshtml" && qx.core.Environment.get("browser.documentmode") < 9) {
              this.$$instance = new qx.bom.IframeHistory();
            } // browser with hashChange event
            else if (this.SUPPORTS_HASH_CHANGE_EVENT) {
                this.$$instance = new qx.bom.NativeHistory();
              } // IE without hashChange event
              else if (qx.core.Environment.get("engine.name") == "mshtml") {
                  this.$$instance = new qx.bom.IframeHistory();
                } // fallback
                else {
                    this.$$instance = new qx.bom.NativeHistory();
                  }
        }

        return this.$$instance;
      }
    },

    /*
    *****************************************************************************
       PROPERTIES
    *****************************************************************************
    */
    properties: {
      /**
       * Property holding the current title
       */
      title: {
        check: "String",
        event: "changeTitle",
        nullable: true,
        apply: "_applyTitle"
      },

      /**
       * Property holding the current state of the history.
       */
      state: {
        check: "String",
        event: "changeState",
        nullable: true,
        apply: "_applyState"
      }
    },

    /*
    *****************************************************************************
       MEMBERS
    *****************************************************************************
    */
    members: {
      _titles: null,
      // property apply
      _applyState: function _applyState(value, old) {
        this._writeState(value);
      },

      /**
       * Populates the 'state' property with the initial state value
       */
      _setInitialState: function _setInitialState() {
        this.setState(this._readState());
      },

      /**
       * Encodes the state value into a format suitable as fragment identifier.
       *
       * @param value {String} The string to encode
       * @return {String} The encoded string
       */
      _encode: function _encode(value) {
        if (qx.lang.Type.isString(value)) {
          return encodeURIComponent(value);
        }

        return "";
      },

      /**
       * Decodes a fragment identifier into a string
       *
       * @param value {String} The fragment identifier
       * @return {String} The decoded fragment identifier
       */
      _decode: function _decode(value) {
        if (qx.lang.Type.isString(value)) {
          return decodeURIComponent(value);
        }

        return "";
      },
      // property apply
      _applyTitle: function _applyTitle(title) {
        if (title != null) {
          document.title = title || "";
        }
      },

      /**
       * Adds an entry to the browser history.
       *
       * @param state {String} a string representing the state of the
       *          application. This command will be delivered in the data property of
       *          the "request" event.
       * @param newTitle {String ? null} the page title to set after the history entry
       *          is done. This title should represent the new state of the application.
       */
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
       * Navigates back in the browser history.
       * Simulates a back button click.
       */
      navigateBack: function navigateBack() {
        qx.event.Timer.once(function () {
          history.back();
        }, this, 100);
      },

      /**
       * Navigates forward in the browser history.
       * Simulates a forward button click.
       */
      navigateForward: function navigateForward() {
        qx.event.Timer.once(function () {
          history.forward();
        }, this, 100);
      },

      /**
       * Called on changes to the history using the browser buttons.
       *
       * @param state {String} new state of the history
       */
      _onHistoryLoad: function _onHistoryLoad(state) {
        this.setState(state);
        this.fireDataEvent("request", state);

        if (this._titles[state] != null) {
          this.setTitle(this._titles[state]);
        }
      },

      /**
       * Browser dependent function to read the current state of the history
       *
       * @return {String} current state of the browser history
       */
      _readState: function _readState() {
        throw new Error("Abstract method call");
      },

      /**
       * Save a state into the browser history.
       *
       * @param state {String} state to save
       */
      _writeState: function _writeState(state) {
        throw new Error("Abstract method call");
      },

      /**
       * Sets the fragment identifier of the window URL
       *
       * @param value {String} the fragment identifier
       */
      _setHash: function _setHash(value) {
        var url = this._baseUrl + (value || "");
        var loc = window.location;

        if (url != loc.href) {
          loc.href = url;
        }
      },

      /**
       * Returns the fragment identifier of the top window URL. For gecko browsers we
       * have to use a regular expression to avoid encoding problems.
       *
       * @return {String} the fragment identifier
       */
      _getHash: function _getHash() {
        var hash = /#(.*)$/.exec(window.location.href);
        return hash && hash[1] ? hash[1] : "";
      }
    }
  });
  qx.bom.History.$$dbClassInfo = $$dbClassInfo;
})();

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
      "qx.bom.client.Engine": {},
      "qx.util.ResourceManager": {}
    },
    "environment": {
      "provided": [],
      "required": {
        "engine.name": {
          "className": "qx.bom.client.Engine"
        },
        "engine.version": {
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
       * Fabian Jakobs (fjakobs)
       * Sebastian Werner (wpbasti)
  
  ************************************************************************ */

  /**
   * The background class contains methods to compute and set the background image
   * of a DOM element.
   *
   * It fixes a background position issue in Firefox 2.
   */
  qx.Class.define("qx.bom.element.Background", {
    statics: {
      /** @type {Array} Internal helper to improve compile performance */
      __tmpl__P_135_0: ["background-image:url(", null, ");", "background-position:", null, ";", "background-repeat:", null, ";"],

      /** @type {Map} Empty styles when no image is given */
      __emptyStyles__P_135_1: {
        backgroundImage: null,
        backgroundPosition: null,
        backgroundRepeat: null
      },

      /**
       * Computes the background position CSS value
       *
       * @param left {Integer|String} either an integer pixel value or a CSS
       *    string value
       * @param top {Integer|String} either an integer pixel value or a CSS
       *    string value
       * @return {String} The background position CSS value
       */
      __computePosition__P_135_2: function __computePosition__P_135_2(left, top) {
        // Correcting buggy Firefox background-position implementation
        // Have problems with identical values
        var engine = qx.core.Environment.get("engine.name");
        var version = qx.core.Environment.get("engine.version");

        if (engine == "gecko" && version < 1.9 && left == top && typeof left == "number") {
          top += 0.01;
        }

        if (left) {
          var leftCss = typeof left == "number" ? left + "px" : left;
        } else {
          leftCss = "0";
        }

        if (top) {
          var topCss = typeof top == "number" ? top + "px" : top;
        } else {
          topCss = "0";
        }

        return leftCss + " " + topCss;
      },

      /**
       * Compiles the background into a CSS compatible string.
       *
       * @param source {String?null} The URL of the background image
       * @param repeat {String?null} The background repeat property. valid values
       *     are <code>repeat</code>, <code>repeat-x</code>,
       *     <code>repeat-y</code>, <code>no-repeat</code>
       * @param left {Integer|String?null} The horizontal offset of the image
       *      inside of the image element. If the value is an integer it is
       *      interpreted as pixel value otherwise the value is taken as CSS value.
       *      CSS the values are "center", "left" and "right"
       * @param top {Integer|String?null} The vertical offset of the image
       *      inside of the image element. If the value is an integer it is
       *      interpreted as pixel value otherwise the value is taken as CSS value.
       *      CSS the values are "top", "bottom" and "center"
       * @return {String} CSS string
       */
      compile: function compile(source, repeat, left, top) {
        var position = this.__computePosition__P_135_2(left, top);

        var backgroundImageUrl = qx.util.ResourceManager.getInstance().toUri(source); // Updating template

        var tmpl = this.__tmpl__P_135_0;
        tmpl[1] = "'" + backgroundImageUrl + "'"; // Put in quotes so spaces work

        tmpl[4] = position;
        tmpl[7] = repeat;
        return tmpl.join("");
      },

      /**
       * Get standard css background styles
       *
       * @param source {String} The URL of the background image
       * @param repeat {String?null} The background repeat property. valid values
       *     are <code>repeat</code>, <code>repeat-x</code>,
       *     <code>repeat-y</code>, <code>no-repeat</code>
       * @param left {Integer|String?null} The horizontal offset of the image
       *      inside of the image element. If the value is an integer it is
       *      interpreted as pixel value otherwise the value is taken as CSS value.
       *      CSS the values are "center", "left" and "right"
       * @param top {Integer|String?null} The vertical offset of the image
       *      inside of the image element. If the value is an integer it is
       *      interpreted as pixel value otherwise the value is taken as CSS value.
       *      CSS the values are "top", "bottom" and "center"
       * @return {Map} A map of CSS styles
       */
      getStyles: function getStyles(source, repeat, left, top) {
        if (!source) {
          return this.__emptyStyles__P_135_1;
        }

        var position = this.__computePosition__P_135_2(left, top);

        var backgroundImageUrl = qx.util.ResourceManager.getInstance().toUri(source);
        var backgroundImageCssString = "url('" + backgroundImageUrl + "')"; // Put in quotes so spaces work

        var map = {
          backgroundPosition: position,
          backgroundImage: backgroundImageCssString
        };

        if (repeat != null) {
          map.backgroundRepeat = repeat;
        }

        return map;
      },

      /**
       * Set the background on the given DOM element
       *
       * @param element {Element} The element to modify
       * @param source {String?null} The URL of the background image
       * @param repeat {String?null} The background repeat property. valid values
       *     are <code>repeat</code>, <code>repeat-x</code>,
       *     <code>repeat-y</code>, <code>no-repeat</code>
       * @param left {Integer?null} The horizontal offset of the image inside of
       *     the image element.
       * @param top {Integer?null} The vertical offset of the image inside of
       *     the image element.
       */
      set: function set(element, source, repeat, left, top) {
        var styles = this.getStyles(source, repeat, left, top);

        for (var prop in styles) {
          element.style[prop] = styles[prop];
        }
      }
    }
  });
  qx.bom.element.Background.$$dbClassInfo = $$dbClassInfo;
})();

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
      "qx.core.Object": {
        "construct": true,
        "require": true
      },
      "qx.util.ResourceManager": {},
      "qx.bom.client.Engine": {},
      "qx.bom.client.Browser": {},
      "qx.event.Timer": {},
      "qx.lang.Array": {},
      "qx.bom.client.OperatingSystem": {},
      "qx.bom.Stylesheet": {},
      "qx.bom.webfonts.Validator": {}
    },
    "environment": {
      "provided": [],
      "required": {
        "engine.name": {
          "className": "qx.bom.client.Engine"
        },
        "engine.version": {
          "className": "qx.bom.client.Engine"
        },
        "browser.documentmode": {
          "className": "qx.bom.client.Browser"
        },
        "browser.name": {
          "className": "qx.bom.client.Browser"
        },
        "browser.version": {
          "className": "qx.bom.client.Browser"
        },
        "os.name": {
          "className": "qx.bom.client.OperatingSystem"
        },
        "os.version": {
          "className": "qx.bom.client.OperatingSystem"
        }
      }
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
  
  ************************************************************************ */

  /**
   * Manages font-face definitions, making sure that each rule is only applied
   * once. It supports adding fonts of the same family but with different style
   * and weight. For instance, the following declaration uses 4 different source
   * files and combine them in a single font family.
   *
   * <pre class='javascript'>
   *   sources: [
   *     {
   *       family: "Sansation",
   *       source: [
   *         "fonts/Sansation-Regular.ttf"
   *       ]
   *     },
   *     {
   *       family: "Sansation",
   *       fontWeight: "bold",
   *       source: [
   *         "fonts/Sansation-Bold.ttf",
   *       ]
   *     },
   *     {
   *       family: "Sansation",
   *       fontStyle: "italic",
   *       source: [
   *         "fonts/Sansation-Italic.ttf",
   *       ]
   *     },
   *     {
   *       family: "Sansation",
   *       fontWeight: "bold",
   *       fontStyle: "italic",
   *       source: [
   *         "fonts/Sansation-BoldItalic.ttf",
   *       ]
   *     }
   *   ]
   * </pre>
   * 
   * This class does not need to be disposed, except when you want to abort the loading
   * and validation process.
   */
  qx.Class.define("qx.bom.webfonts.Manager", {
    extend: qx.core.Object,
    type: "singleton",

    /*
    *****************************************************************************
       CONSTRUCTOR
    *****************************************************************************
    */
    construct: function construct() {
      qx.core.Object.constructor.call(this);
      this.__createdStyles__P_132_0 = [];
      this.__validators__P_132_1 = {};
      this.__queue__P_132_2 = [];
      this.__preferredFormats__P_132_3 = this.getPreferredFormats();
    },

    /*
    *****************************************************************************
       STATICS
    *****************************************************************************
    */
    statics: {
      /**
       * List of known font definition formats (i.e. file extensions). Used to
       * identify the type of each font file configured for a web font.
       */
      FONT_FORMATS: ["eot", "woff2", "woff", "ttf", "svg"],

      /**
       * Timeout (in ms) to wait before deciding that a web font was not loaded.
       */
      VALIDATION_TIMEOUT: 5000
    },

    /*
    *****************************************************************************
       MEMBERS
    *****************************************************************************
    */
    members: {
      __createdStyles__P_132_0: null,
      __styleSheet__P_132_4: null,
      __validators__P_132_1: null,
      __preferredFormats__P_132_3: null,
      __queue__P_132_2: null,
      __queueInterval__P_132_5: null,

      /*
      ---------------------------------------------------------------------------
        PUBLIC API
      ---------------------------------------------------------------------------
      */

      /**
       * Adds the necessary font-face rule for a web font to the document. Also
       * creates a web font Validator ({@link qx.bom.webfonts.Validator}) that
       * checks if the webFont was applied correctly.
       *
       * @param familyName {String} Name of the web font
       * @param sourcesList {Object} List of source URLs along with their style
       * (e.g. fontStyle: "italic") and weight (e.g. fontWeight: "bold").
       * For maximum compatibility, this should include EOT, WOFF and TTF versions
       * of the font.
       * @param callback {Function?} Optional event listener callback that will be
       * executed once the validator has determined whether the webFont was
       * applied correctly.
       * See {@link qx.bom.webfonts.Validator#changeStatus}
       * @param context {Object?} Optional context for the callback function
       */
      require: function require(familyName, sourcesList, callback, context) {
        var sourceUrls = sourcesList.source;
        var comparisonString = sourcesList.comparisonString;
        var version = sourcesList.version;
        var fontWeight = sourcesList.fontWeight;
        var fontStyle = sourcesList.fontStyle;
        var sources = [];

        for (var i = 0, l = sourceUrls.length; i < l; i++) {
          var split = sourceUrls[i].split("#");
          var src = qx.util.ResourceManager.getInstance().toUri(split[0]);

          if (split.length > 1) {
            src = src + "#" + split[1];
          }

          sources.push(src);
        } // old IEs need a break in between adding @font-face rules


        if (qx.core.Environment.get("engine.name") == "mshtml" && (parseInt(qx.core.Environment.get("engine.version")) < 9 || qx.core.Environment.get("browser.documentmode") < 9)) {
          if (!this.__queueInterval__P_132_5) {
            this.__queueInterval__P_132_5 = new qx.event.Timer(100);

            this.__queueInterval__P_132_5.addListener("interval", this.__flushQueue__P_132_6, this);
          }

          if (!this.__queueInterval__P_132_5.isEnabled()) {
            this.__queueInterval__P_132_5.start();
          }

          this.__queue__P_132_2.push([familyName, sources, fontWeight, fontStyle, comparisonString, version, callback, context]);
        } else {
          this.__require__P_132_7(familyName, sources, fontWeight, fontStyle, comparisonString, version, callback, context);
        }
      },

      /**
       * Removes a font's font-face definition from the style sheet. This means
       * the font will no longer be available and any elements using it will
       * fall back to the their regular font-families.
       *
       * @param familyName {String} font-family name
       * @param fontWeight {String} the font-weight.
       * @param fontStyle {String} the font-style.
       */
      remove: function remove(familyName, fontWeight, fontStyle) {
        var fontLookupKey = this.__createFontLookupKey__P_132_8(familyName, fontWeight, fontStyle);

        var index = null;

        for (var i = 0, l = this.__createdStyles__P_132_0.length; i < l; i++) {
          if (this.__createdStyles__P_132_0[i] == fontLookupKey) {
            index = i;

            this.__removeRule__P_132_9(familyName, fontWeight, fontStyle);

            break;
          }
        }

        if (index !== null) {
          qx.lang.Array.removeAt(this.__createdStyles__P_132_0, index);
        }

        if (familyName in this.__validators__P_132_1) {
          this.__validators__P_132_1[familyName].dispose();

          delete this.__validators__P_132_1[familyName];
        }
      },

      /**
       * Returns the preferred font format(s) for the currently used browser. Some
       * browsers support multiple formats, e.g. WOFF and TTF or WOFF and EOT. In
       * those cases, WOFF is considered the preferred format.
       *
       * @return {String[]} List of supported font formats ordered by preference
       * or empty Array if none could be determined
       */
      getPreferredFormats: function getPreferredFormats() {
        var preferredFormats = [];
        var browser = qx.core.Environment.get("browser.name");
        var browserVersion = qx.core.Environment.get("browser.version");
        var os = qx.core.Environment.get("os.name");
        var osVersion = qx.core.Environment.get("os.version");

        if (browser == "edge" && browserVersion >= 14 || browser == "firefox" && browserVersion >= 69 || browser == "chrome" && browserVersion >= 36) {
          preferredFormats.push("woff2");
        }

        if (browser == "ie" && qx.core.Environment.get("browser.documentmode") >= 9 || browser == "edge" && browserVersion >= 12 || browser == "firefox" && browserVersion >= 3.6 || browser == "chrome" && browserVersion >= 6) {
          preferredFormats.push("woff");
        }

        if (browser == "edge" && browserVersion >= 12 || browser == "opera" && browserVersion >= 10 || browser == "safari" && browserVersion >= 3.1 || browser == "firefox" && browserVersion >= 3.5 || browser == "chrome" && browserVersion >= 4 || browser == "mobile safari" && os == "ios" && osVersion >= 4.2) {
          preferredFormats.push("ttf");
        }

        if (browser == "ie" && browserVersion >= 4) {
          preferredFormats.push("eot");
        }

        if (browser == "mobileSafari" && os == "ios" && osVersion >= 4.1) {
          preferredFormats.push("svg");
        }

        return preferredFormats;
      },

      /**
       * Removes the styleSheet element used for all web font definitions from the
       * document. This means all web fonts declared by the manager will no longer
       * be available and elements using them will fall back to their regular
       * font-families
       */
      removeStyleSheet: function removeStyleSheet() {
        this.__createdStyles__P_132_0 = [];

        if (this.__styleSheet__P_132_4) {
          qx.bom.Stylesheet.removeSheet(this.__styleSheet__P_132_4);
        }

        this.__styleSheet__P_132_4 = null;
      },

      /*
      ---------------------------------------------------------------------------
        PRIVATE API
      ---------------------------------------------------------------------------
      */

      /**
       * Creates a lookup key to index the created fonts.
       * @param familyName {String} font-family name
       * @param fontWeight {String} the font-weight.
       * @param fontStyle {String} the font-style.
       * @return {string} the font lookup key
       */
      __createFontLookupKey__P_132_8: function __createFontLookupKey__P_132_8(familyName, fontWeight, fontStyle) {
        var lookupKey = familyName + "_" + (fontWeight ? fontWeight : "normal") + "_" + (fontStyle ? fontStyle : "normal");
        return lookupKey;
      },

      /**
       * Does the actual work of adding stylesheet rules and triggering font
       * validation
       *
       * @param familyName {String} Name of the web font
       * @param sources {String[]} List of source URLs. For maximum compatibility,
       * this should include EOT, WOFF and TTF versions of the font.
       * @param fontWeight {String} the web font should be registered using a
       * fontWeight font weight.
       * @param fontStyle {String} the web font should be registered using an
       * fontStyle font style.
       * @param comparisonString {String} String to check whether the font has loaded or not
       * @param version {String?} Optional version that is appended to the font URL to be able to override caching
       * @param callback {Function?} Optional event listener callback that will be
       * executed once the validator has determined whether the webFont was
       * applied correctly.
       * @param context {Object?} Optional context for the callback function
       */
      __require__P_132_7: function __require__P_132_7(familyName, sources, fontWeight, fontStyle, comparisonString, version, callback, context) {
        var fontLookupKey = this.__createFontLookupKey__P_132_8(familyName, fontWeight, fontStyle);

        if (!this.__createdStyles__P_132_0.includes(fontLookupKey)) {
          var sourcesMap = this.__getSourcesMap__P_132_10(sources);

          var rule = this.__getRule__P_132_11(familyName, fontWeight, fontStyle, sourcesMap, version);

          if (!rule) {
            throw new Error("Couldn't create @font-face rule for WebFont " + familyName + "!");
          }

          if (!this.__styleSheet__P_132_4) {
            this.__styleSheet__P_132_4 = qx.bom.Stylesheet.createElement();
          }

          try {
            this.__addRule__P_132_12(rule);
          } catch (ex) {
            {
              this.warn("Error while adding @font-face rule:", ex.message);
              return;
            }
          }

          this.__createdStyles__P_132_0.push(fontLookupKey);
        }

        if (!this.__validators__P_132_1[familyName]) {
          this.__validators__P_132_1[familyName] = new qx.bom.webfonts.Validator(familyName, comparisonString);

          this.__validators__P_132_1[familyName].setTimeout(qx.bom.webfonts.Manager.VALIDATION_TIMEOUT);

          this.__validators__P_132_1[familyName].addListenerOnce("changeStatus", this.__onFontChangeStatus__P_132_13, this);
        }

        if (callback) {
          var cbContext = context || window;

          this.__validators__P_132_1[familyName].addListenerOnce("changeStatus", callback, cbContext);
        }

        this.__validators__P_132_1[familyName].validate();
      },

      /**
       * Processes the next item in the queue
       */
      __flushQueue__P_132_6: function __flushQueue__P_132_6() {
        if (this.__queue__P_132_2.length == 0) {
          this.__queueInterval__P_132_5.stop();

          return;
        }

        var next = this.__queue__P_132_2.shift();

        this.__require__P_132_7.apply(this, next);
      },

      /**
       * Removes the font-face declaration if a font could not be validated
       *
       * @param ev {qx.event.type.Data} qx.bom.webfonts.Validator#changeStatus
       */
      __onFontChangeStatus__P_132_13: function __onFontChangeStatus__P_132_13(ev) {
        var result = ev.getData();

        if (result.valid === false) {
          qx.event.Timer.once(function () {
            this.remove(result.family);
          }, this, 250);
        }
      },

      /**
       * Uses a naive regExp match to determine the format of each defined source
       * file for a webFont. Returns a map with the format names as keys and the
       * corresponding source URLs as values.
       *
       * @param sources {String[]} Array of source URLs
       * @return {Map} Map of formats and URLs
       */
      __getSourcesMap__P_132_10: function __getSourcesMap__P_132_10(sources) {
        var formats = qx.bom.webfonts.Manager.FONT_FORMATS;
        var sourcesMap = {};
        var reg = new RegExp("\.(" + formats.join("|") + ")");

        for (var i = 0, l = sources.length; i < l; i++) {
          var match = reg.exec(sources[i]);

          if (match) {
            var type = match[1];
            sourcesMap[type] = sources[i];
          }
        }

        return sourcesMap;
      },

      /**
       * Assembles the body of a font-face rule for a single webFont.
       *
       * @param familyName {String} Font-family name
       * @param fontWeight {String} the web font should be registered using a
       * fontWeight font weight.
       * @param fontStyle {String} the web font should be registered using an
       * fontStyle font style.
       * @param sourcesMap {Map} Map of font formats and sources
       * @param version {String?} Optional version to be appended to the URL
       * @return {String} The computed CSS rule
       */
      __getRule__P_132_11: function __getRule__P_132_11(familyName, fontWeight, fontStyle, sourcesMap, version) {
        var rules = [];
        var formatList = this.__preferredFormats__P_132_3.length > 0 ? this.__preferredFormats__P_132_3 : qx.bom.webfonts.Manager.FONT_FORMATS;

        for (var i = 0, l = formatList.length; i < l; i++) {
          var format = formatList[i];

          if (sourcesMap[format]) {
            rules.push(this.__getSourceForFormat__P_132_14(format, sourcesMap[format], version));
          }
        }

        var rule = "src: " + rules.join(",\n") + ";";
        rule = "font-family: " + familyName + ";\n" + rule;
        rule = rule + "\nfont-style: " + (fontStyle ? fontStyle : "normal") + ";";
        rule = rule + "\nfont-weight: " + (fontWeight ? fontWeight : "normal") + ";";
        return rule;
      },

      /**
       * Returns the full src value for a given font URL depending on the type
        * @param format {String} The font format, one of eot, woff2, woff, ttf, svg
       * @param url {String} The font file's URL
       * @param version {String?} Optional version to be appended to the URL
       * @return {String} The src directive
       */
      __getSourceForFormat__P_132_14: function __getSourceForFormat__P_132_14(format, url, version) {
        if (version) {
          url += "?" + version;
        }

        switch (format) {
          case "eot":
            return "url('" + url + "');" + "src: url('" + url + "?#iefix') format('embedded-opentype')";

          case "woff2":
            return "url('" + url + "') format('woff2')";

          case "woff":
            return "url('" + url + "') format('woff')";

          case "ttf":
            return "url('" + url + "') format('truetype')";

          case "svg":
            return "url('" + url + "') format('svg')";

          default:
            return null;
        }
      },

      /**
       * Adds a font-face rule to the document
       *
       * @param rule {String} The body of the CSS rule
       */
      __addRule__P_132_12: function __addRule__P_132_12(rule) {
        var completeRule = "@font-face {" + rule + "}\n";

        if (qx.core.Environment.get("browser.name") == "ie" && qx.core.Environment.get("browser.documentmode") < 9) {
          var cssText = this.__fixCssText__P_132_15(this.__styleSheet__P_132_4.cssText);

          cssText += completeRule;
          this.__styleSheet__P_132_4.cssText = cssText;
        } else {
          this.__styleSheet__P_132_4.insertRule(completeRule, this.__styleSheet__P_132_4.cssRules.length);
        }
      },

      /**
       * Removes the font-face declaration for the given font-family from the
       * stylesheet
       *
       * @param familyName {String} The font-family name
       * @param fontWeight {String} fontWeight font-weight.
       * @param fontStyle {String} fontStyle font-style.
       */
      __removeRule__P_132_9: function __removeRule__P_132_9(familyName, fontWeight, fontStyle) {
        // In IE and edge even if the rule was added with font-style first
        // and font-weight second, it is not guaranteed that the attributes
        // remain in that order. Therefore we check for both version,
        // style first, weight second and weight first, style second.
        // Without this fix the rule isn't found and removed reliable. 
        var regtext = "@font-face.*?" + familyName + "(.*font-style: *" + (fontStyle ? fontStyle : "normal") + ".*font-weight: *" + (fontWeight ? fontWeight : "normal") + ")|" + "(.*font-weight: *" + (fontWeight ? fontWeight : "normal") + ".*font-style: *" + (fontStyle ? fontStyle : "normal") + ")";
        var reg = new RegExp(regtext, "m");

        for (var i = 0, l = document.styleSheets.length; i < l; i++) {
          var sheet = document.styleSheets[i];

          if (sheet.cssText) {
            var cssText = sheet.cssText.replace(/\n/g, "").replace(/\r/g, "");
            cssText = this.__fixCssText__P_132_15(cssText);

            if (reg.exec(cssText)) {
              cssText = cssText.replace(reg, "");
            }

            sheet.cssText = cssText;
          } else if (sheet.cssRules) {
            for (var j = 0, m = sheet.cssRules.length; j < m; j++) {
              var cssText = sheet.cssRules[j].cssText.replace(/\n/g, "").replace(/\r/g, "");

              if (reg.exec(cssText)) {
                this.__styleSheet__P_132_4.deleteRule(j);

                return;
              }
            }
          }
        }
      },

      /**
       * IE 6 and 7 omit the trailing quote after the format name when
       * querying cssText. This needs to be fixed before cssText is replaced
       * or all rules will be invalid and no web fonts will work any more.
       *
       * @param cssText {String} CSS text
       * @return {String} Fixed CSS text
       */
      __fixCssText__P_132_15: function __fixCssText__P_132_15(cssText) {
        return cssText.replace("'eot)", "'eot')").replace("('embedded-opentype)", "('embedded-opentype')");
      }
    },

    /*
    *****************************************************************************
      DESTRUCTOR
    *****************************************************************************
    */
    destruct: function destruct() {
      if (this.__queueInterval__P_132_5) {
        this.__queueInterval__P_132_5.stop();

        this.__queueInterval__P_132_5.dispose();
      }

      delete this.__createdStyles__P_132_0;
      this.removeStyleSheet();

      for (var prop in this.__validators__P_132_1) {
        this.__validators__P_132_1[prop].dispose();
      }

      qx.bom.webfonts.Validator.removeDefaultHelperElements();
    }
  });
  qx.bom.webfonts.Manager.$$dbClassInfo = $$dbClassInfo;
})();

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
      "qx.core.Assert": {},
      "qx.bom.client.OperatingSystem": {},
      "qx.locale.Manager": {
        "defer": "runtime"
      }
    },
    "environment": {
      "provided": [],
      "required": {
        "os.name": {
          "className": "qx.bom.client.OperatingSystem"
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
  
  ************************************************************************ */

  /**
   * Static class, which contains functionality to localize the names of keyboard keys.
   */
  qx.Class.define("qx.locale.Key", {
    /*
    *****************************************************************************
       STATICS
    *****************************************************************************
    */
    statics: {
      /**
       * Return localized name of a key identifier
       * {@link qx.event.type.KeySequence}
       *
       * @param size {String} format of the key identifier.
       *       Possible values: "short", "full"
       * @param keyIdentifier {String} key identifier to translate {@link qx.event.type.KeySequence}
       * @param locale {String} optional locale to be used
       * @return {String} localized key name
       */
      getKeyName: function getKeyName(size, keyIdentifier, locale) {
        {
          qx.core.Assert.assertInArray(size, ["short", "full"]);
        }
        var key = "key_" + size + "_" + keyIdentifier; // Control is always named control on a mac and not Strg in German e.g.

        if (qx.core.Environment.get("os.name") == "osx" && keyIdentifier == "Control") {
          key += "_Mac";
        }

        var localizedKey = qx.locale.Manager.getInstance().translate(key, [], locale);

        if (localizedKey == key) {
          return qx.locale.Key._keyNames[key] || keyIdentifier;
        } else {
          return localizedKey;
        }
      }
    },

    /*
    *****************************************************************************
       DEFER
    *****************************************************************************
    */
    defer: function defer(statics) {
      var keyNames = {};
      var Manager = qx.locale.Manager; // TRANSLATION: short representation of key names

      keyNames[Manager.marktr("key_short_Backspace")] = "Backspace";
      keyNames[Manager.marktr("key_short_Tab")] = "Tab";
      keyNames[Manager.marktr("key_short_Space")] = "Space";
      keyNames[Manager.marktr("key_short_Enter")] = "Enter";
      keyNames[Manager.marktr("key_short_Shift")] = "Shift";
      keyNames[Manager.marktr("key_short_Control")] = "Ctrl";
      keyNames[Manager.marktr("key_short_Control_Mac")] = "Ctrl";
      keyNames[Manager.marktr("key_short_Alt")] = "Alt";
      keyNames[Manager.marktr("key_short_CapsLock")] = "Caps";
      keyNames[Manager.marktr("key_short_Meta")] = "Meta";
      keyNames[Manager.marktr("key_short_Escape")] = "Esc";
      keyNames[Manager.marktr("key_short_Left")] = "Left";
      keyNames[Manager.marktr("key_short_Up")] = "Up";
      keyNames[Manager.marktr("key_short_Right")] = "Right";
      keyNames[Manager.marktr("key_short_Down")] = "Down";
      keyNames[Manager.marktr("key_short_PageUp")] = "PgUp";
      keyNames[Manager.marktr("key_short_PageDown")] = "PgDn";
      keyNames[Manager.marktr("key_short_End")] = "End";
      keyNames[Manager.marktr("key_short_Home")] = "Home";
      keyNames[Manager.marktr("key_short_Insert")] = "Ins";
      keyNames[Manager.marktr("key_short_Delete")] = "Del";
      keyNames[Manager.marktr("key_short_NumLock")] = "Num";
      keyNames[Manager.marktr("key_short_PrintScreen")] = "Print";
      keyNames[Manager.marktr("key_short_Scroll")] = "Scroll";
      keyNames[Manager.marktr("key_short_Pause")] = "Pause";
      keyNames[Manager.marktr("key_short_Win")] = "Win";
      keyNames[Manager.marktr("key_short_Apps")] = "Apps"; // TRANSLATION: full/long representation of key names

      keyNames[Manager.marktr("key_full_Backspace")] = "Backspace";
      keyNames[Manager.marktr("key_full_Tab")] = "Tabulator";
      keyNames[Manager.marktr("key_full_Space")] = "Space";
      keyNames[Manager.marktr("key_full_Enter")] = "Enter";
      keyNames[Manager.marktr("key_full_Shift")] = "Shift";
      keyNames[Manager.marktr("key_full_Control")] = "Control";
      keyNames[Manager.marktr("key_full_Control_Mac")] = "Control";
      keyNames[Manager.marktr("key_full_Alt")] = "Alt";
      keyNames[Manager.marktr("key_full_CapsLock")] = "CapsLock";
      keyNames[Manager.marktr("key_full_Meta")] = "Meta";
      keyNames[Manager.marktr("key_full_Escape")] = "Escape";
      keyNames[Manager.marktr("key_full_Left")] = "Left";
      keyNames[Manager.marktr("key_full_Up")] = "Up";
      keyNames[Manager.marktr("key_full_Right")] = "Right";
      keyNames[Manager.marktr("key_full_Down")] = "Down";
      keyNames[Manager.marktr("key_full_PageUp")] = "PageUp";
      keyNames[Manager.marktr("key_full_PageDown")] = "PageDown";
      keyNames[Manager.marktr("key_full_End")] = "End";
      keyNames[Manager.marktr("key_full_Home")] = "Home";
      keyNames[Manager.marktr("key_full_Insert")] = "Insert";
      keyNames[Manager.marktr("key_full_Delete")] = "Delete";
      keyNames[Manager.marktr("key_full_NumLock")] = "NumLock";
      keyNames[Manager.marktr("key_full_PrintScreen")] = "PrintScreen";
      keyNames[Manager.marktr("key_full_Scroll")] = "Scroll";
      keyNames[Manager.marktr("key_full_Pause")] = "Pause";
      keyNames[Manager.marktr("key_full_Win")] = "Win";
      keyNames[Manager.marktr("key_full_Apps")] = "Apps"; // Save

      statics._keyNames = keyNames;
    }
  });
  qx.locale.Key.$$dbClassInfo = $$dbClassInfo;
})();

(function () {
  var $$dbClassInfo = {
    "dependsOn": {
      "qx.Bootstrap": {
        "usage": "dynamic",
        "require": true
      }
    }
  };
  qx.Bootstrap.executePendingDefers($$dbClassInfo);

  /* ************************************************************************
  
     qooxdoo - the new era of web development
  
     http://qooxdoo.org
  
     Copyright:
       2009 1&1 Internet AG, Germany, http://www.1und1.de
  
     License:
       MIT: https://opensource.org/licenses/MIT
       See the LICENSE file in the project's top-level directory for details.
  
     Authors:
       * Fabian Jakobs (fjakobs)
       * Christian Hagendorn (chris_schmidt)
  
  ************************************************************************ */

  /**
   * Abstract class to compute the position of an object on one axis.
   */
  qx.Bootstrap.define("qx.util.placement.AbstractAxis", {
    extend: Object,
    statics: {
      /**
       * Computes the start of the object on the axis
       *
       * @param size {Integer} Size of the object to align
       * @param target {Map} Location of the object to align the object to. This map
       *   should have the keys <code>start</code> and <code>end</code>.
       * @param offsets {Map} Map with all offsets on each side.
       *   Comes with the keys <code>start</code> and <code>end</code>.
       * @param areaSize {Integer} Size of the axis.
       * @param position {String} Alignment of the object on the target. Valid values are
       *   <ul>
       *   <li><code>edge-start</code> The object is placed before the target</li>
       *   <li><code>edge-end</code> The object is placed after the target</li>
       *   <li><code>align-start</code>The start of the object is aligned with the start of the target</li>
       *   <li><code>align-center</code>The center of the object is aligned with the center of the target</li>
       *   <li><code>align-end</code>The end of the object is aligned with the end of the object</li>
       *   </ul>
       * @return {Integer} The computed start position of the object.
       * @abstract
       */
      computeStart: function computeStart(size, target, offsets, areaSize, position) {
        throw new Error("abstract method call!");
      },

      /**
       * Computes the start of the object by taking only the attachment and
       * alignment into account. The object by be not fully visible.
       *
       * @param size {Integer} Size of the object to align
       * @param target {Map} Location of the object to align the object to. This map
       *   should have the keys <code>start</code> and <code>end</code>.
       * @param offsets {Map} Map with all offsets on each side.
       *   Comes with the keys <code>start</code> and <code>end</code>.
       * @param position {String} Accepts the same values as the <code> position</code>
       *   argument of {@link #computeStart}.
       * @return {Integer} The computed start position of the object.
       */
      _moveToEdgeAndAlign: function _moveToEdgeAndAlign(size, target, offsets, position) {
        switch (position) {
          case "edge-start":
            return target.start - offsets.end - size;

          case "edge-end":
            return target.end + offsets.start;

          case "align-start":
            return target.start + offsets.start;

          case "align-center":
            return target.start + parseInt((target.end - target.start - size) / 2, 10) + offsets.start;

          case "align-end":
            return target.end - offsets.end - size;
        }
      },

      /**
       * Whether the object specified by <code>start</code> and <code>size</code>
       * is completely inside of the axis' range..
       *
       * @param start {Integer} Computed start position of the object
       * @param size {Integer} Size of the object
       * @param areaSize {Integer} The size of the axis
       * @return {Boolean} Whether the object is inside of the axis' range
       */
      _isInRange: function _isInRange(start, size, areaSize) {
        return start >= 0 && start + size <= areaSize;
      }
    }
  });
  qx.util.placement.AbstractAxis.$$dbClassInfo = $$dbClassInfo;
})();

(function () {
  var $$dbClassInfo = {
    "dependsOn": {
      "qx.Bootstrap": {
        "usage": "dynamic",
        "require": true
      },
      "qx.util.placement.AbstractAxis": {
        "require": true
      }
    }
  };
  qx.Bootstrap.executePendingDefers($$dbClassInfo);

  /* ************************************************************************
  
     qooxdoo - the new era of web development
  
     http://qooxdoo.org
  
     Copyright:
       2009 1&1 Internet AG, Germany, http://www.1und1.de
  
     License:
       MIT: https://opensource.org/licenses/MIT
       See the LICENSE file in the project's top-level directory for details.
  
     Authors:
       * Fabian Jakobs (fjakobs)
       * Christian Hagendorn (chris_schmidt)
  
  ************************************************************************ */

  /**
   * Places the object directly at the specified position. It is not moved if
   * parts of the object are outside of the axis' range.
   */
  qx.Bootstrap.define("qx.util.placement.DirectAxis", {
    statics: {
      /**
       * Computes the start of the object by taking only the attachment and
       * alignment into account. The object by be not fully visible.
       *
       * @param size {Integer} Size of the object to align
       * @param target {Map} Location of the object to align the object to. This map
       *   should have the keys <code>start</code> and <code>end</code>.
       * @param offsets {Map} Map with all offsets on each side.
       *   Comes with the keys <code>start</code> and <code>end</code>.
       * @param position {String} Accepts the same values as the <code> position</code>
       *   argument of {@link #computeStart}.
       * @return {Integer} The computed start position of the object.
       */
      _moveToEdgeAndAlign: qx.util.placement.AbstractAxis._moveToEdgeAndAlign,

      /**
       * Computes the start of the object on the axis
       *
       * @param size {Integer} Size of the object to align
       * @param target {Map} Location of the object to align the object to. This map
       *   should have the keys <code>start</code> and <code>end</code>.
       * @param offsets {Map} Map with all offsets on each side.
       *   Comes with the keys <code>start</code> and <code>end</code>.
       * @param areaSize {Integer} Size of the axis.
       * @param position {String} Alignment of the object on the target. Valid values are
       *   <ul>
       *   <li><code>edge-start</code> The object is placed before the target</li>
       *   <li><code>edge-end</code> The object is placed after the target</li>
       *   <li><code>align-start</code>The start of the object is aligned with the start of the target</li>
       *   <li><code>align-center</code>The center of the object is aligned with the center of the target</li>
       *   <li><code>align-end</code>The end of the object is aligned with the end of the object</li>
       *   </ul>
       * @return {Integer} The computed start position of the object.
       */
      computeStart: function computeStart(size, target, offsets, areaSize, position) {
        return this._moveToEdgeAndAlign(size, target, offsets, position);
      }
    }
  });
  qx.util.placement.DirectAxis.$$dbClassInfo = $$dbClassInfo;
})();

(function () {
  var $$dbClassInfo = {
    "dependsOn": {
      "qx.Bootstrap": {
        "usage": "dynamic",
        "require": true
      },
      "qx.util.placement.AbstractAxis": {
        "require": true
      }
    }
  };
  qx.Bootstrap.executePendingDefers($$dbClassInfo);

  /* ************************************************************************
  
     qooxdoo - the new era of web development
  
     http://qooxdoo.org
  
     Copyright:
       2009 1&1 Internet AG, Germany, http://www.1und1.de
  
     License:
       MIT: https://opensource.org/licenses/MIT
       See the LICENSE file in the project's top-level directory for details.
  
     Authors:
       * Fabian Jakobs (fjakobs)
       * Christian Hagendorn (chris_schmidt)
  
  ************************************************************************ */

  /**
   * Places the object to the target. If parts of the object are outside of the
   * range this class places the object at the best "edge", "alignment"
   * combination so that the overlap between object and range is maximized.
   */
  qx.Bootstrap.define("qx.util.placement.KeepAlignAxis", {
    statics: {
      /**
       * Computes the start of the object by taking only the attachment and
       * alignment into account. The object by be not fully visible.
       *
       * @param size {Integer} Size of the object to align
       * @param target {Map} Location of the object to align the object to. This map
       *   should have the keys <code>start</code> and <code>end</code>.
       * @param offsets {Map} Map with all offsets on each side.
       *   Comes with the keys <code>start</code> and <code>end</code>.
       * @param position {String} Accepts the same values as the <code> position</code>
       *   argument of {@link #computeStart}.
       * @return {Integer} The computed start position of the object.
       */
      _moveToEdgeAndAlign: qx.util.placement.AbstractAxis._moveToEdgeAndAlign,

      /**
       * Whether the object specified by <code>start</code> and <code>size</code>
       * is completely inside of the axis' range..
       *
       * @param start {Integer} Computed start position of the object
       * @param size {Integer} Size of the object
       * @param areaSize {Integer} The size of the axis
       * @return {Boolean} Whether the object is inside of the axis' range
       */
      _isInRange: qx.util.placement.AbstractAxis._isInRange,

      /**
       * Computes the start of the object on the axis
       *
       * @param size {Integer} Size of the object to align
       * @param target {Map} Location of the object to align the object to. This map
       *   should have the keys <code>start</code> and <code>end</code>.
       * @param offsets {Map} Map with all offsets on each side.
       *   Comes with the keys <code>start</code> and <code>end</code>.
       * @param areaSize {Integer} Size of the axis.
       * @param position {String} Alignment of the object on the target. Valid values are
       *   <ul>
       *   <li><code>edge-start</code> The object is placed before the target</li>
       *   <li><code>edge-end</code> The object is placed after the target</li>
       *   <li><code>align-start</code>The start of the object is aligned with the start of the target</li>
       *   <li><code>align-center</code>The center of the object is aligned with the center of the target</li>
       *   <li><code>align-end</code>The end of the object is aligned with the end of the object</li>
       *   </ul>
       * @return {Integer} The computed start position of the object.
       */
      computeStart: function computeStart(size, target, offsets, areaSize, position) {
        var start = this._moveToEdgeAndAlign(size, target, offsets, position);

        var range1End, range2Start;

        if (this._isInRange(start, size, areaSize)) {
          return start;
        }

        if (position == "edge-start" || position == "edge-end") {
          range1End = target.start - offsets.end;
          range2Start = target.end + offsets.start;
        } else {
          range1End = target.end - offsets.end;
          range2Start = target.start + offsets.start;
        }

        if (range1End > areaSize - range2Start) {
          start = Math.max(0, range1End - size);
        } else {
          start = range2Start;
        }

        return start;
      }
    }
  });
  qx.util.placement.KeepAlignAxis.$$dbClassInfo = $$dbClassInfo;
})();

(function () {
  var $$dbClassInfo = {
    "dependsOn": {
      "qx.Bootstrap": {
        "usage": "dynamic",
        "require": true
      },
      "qx.util.placement.AbstractAxis": {
        "require": true
      }
    }
  };
  qx.Bootstrap.executePendingDefers($$dbClassInfo);

  /* ************************************************************************
  
     qooxdoo - the new era of web development
  
     http://qooxdoo.org
  
     Copyright:
       2009 1&1 Internet AG, Germany, http://www.1und1.de
  
     License:
       MIT: https://opensource.org/licenses/MIT
       See the LICENSE file in the project's top-level directory for details.
  
     Authors:
       * Fabian Jakobs (fjakobs)
       * Christian Hagendorn (chris_schmidt)
  
  ************************************************************************ */

  /**
   * Places the object according to the target. If parts of the object are outside
   * of the axis' range the object's start is adjusted so that the overlap between
   * the object and the axis is maximized.
   */
  qx.Bootstrap.define("qx.util.placement.BestFitAxis", {
    statics: {
      /**
       * Whether the object specified by <code>start</code> and <code>size</code>
       * is completely inside of the axis' range..
       *
       * @param start {Integer} Computed start position of the object
       * @param size {Integer} Size of the object
       * @param areaSize {Integer} The size of the axis
       * @return {Boolean} Whether the object is inside of the axis' range
       */
      _isInRange: qx.util.placement.AbstractAxis._isInRange,

      /**
       * Computes the start of the object by taking only the attachment and
       * alignment into account. The object by be not fully visible.
       *
       * @param size {Integer} Size of the object to align
       * @param target {Map} Location of the object to align the object to. This map
       *   should have the keys <code>start</code> and <code>end</code>.
       * @param offsets {Map} Map with all offsets on each side.
       *   Comes with the keys <code>start</code> and <code>end</code>.
       * @param position {String} Accepts the same values as the <code> position</code>
       *   argument of {@link #computeStart}.
       * @return {Integer} The computed start position of the object.
       */
      _moveToEdgeAndAlign: qx.util.placement.AbstractAxis._moveToEdgeAndAlign,

      /**
       * Computes the start of the object on the axis
       *
       * @param size {Integer} Size of the object to align
       * @param target {Map} Location of the object to align the object to. This map
       *   should have the keys <code>start</code> and <code>end</code>.
       * @param offsets {Map} Map with all offsets on each side.
       *   Comes with the keys <code>start</code> and <code>end</code>.
       * @param areaSize {Integer} Size of the axis.
       * @param position {String} Alignment of the object on the target. Valid values are
       *   <ul>
       *   <li><code>edge-start</code> The object is placed before the target</li>
       *   <li><code>edge-end</code> The object is placed after the target</li>
       *   <li><code>align-start</code>The start of the object is aligned with the start of the target</li>
       *   <li><code>align-center</code>The center of the object is aligned with the center of the target</li>
       *   <li><code>align-end</code>The end of the object is aligned with the end of the object</li>
       *   </ul>
       * @return {Integer} The computed start position of the object.
       */
      computeStart: function computeStart(size, target, offsets, areaSize, position) {
        var start = this._moveToEdgeAndAlign(size, target, offsets, position);

        if (this._isInRange(start, size, areaSize)) {
          return start;
        }

        if (start < 0) {
          start = Math.min(0, areaSize - size);
        }

        if (start + size > areaSize) {
          start = Math.max(0, areaSize - size);
        }

        return start;
      }
    }
  });
  qx.util.placement.BestFitAxis.$$dbClassInfo = $$dbClassInfo;
})();

(function () {
  var $$dbClassInfo = {
    "dependsOn": {
      "qx.core.Environment": {
        "defer": "load",
        "usage": "dynamic",
        "require": true
      },
      "qx.Class": {
        "usage": "dynamic",
        "require": true
      },
      "qx.core.Assert": {},
      "qx.dom.Element": {},
      "qx.log.Logger": {},
      "qx.lang.Object": {},
      "qx.bom.client.Engine": {
        "defer": "runtime",
        "require": true
      },
      "qx.bom.client.Browser": {},
      "qx.bom.Event": {
        "defer": "runtime"
      },
      "qx.event.GlobalError": {
        "usage": "dynamic",
        "require": true
      }
    },
    "environment": {
      "provided": [],
      "required": {
        "engine.name": {
          "className": "qx.bom.client.Engine",
          "load": true,
          "defer": true
        },
        "browser.documentmode": {
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
       * Christian Hagendorn (chris_schmidt)
  
     ======================================================================
  
     This class contains code based on the following work:
  
     * SWFFix
       http://code.google.com/p/swffix/
       Version 0.3 (r17)
  
       Copyright:
         (c) 2007 SWFFix developers
  
       License:
         MIT: http://www.opensource.org/licenses/mit-license.php
  
       Authors:
         * Geoff Stearns
         * Michael Williams
         * Bobby van der Sluis
  
  ************************************************************************ */

  /**
   * Flash(TM) embed via script
   *
   * Include:
   *
   * * Simple movie embedding (returning a cross-browser working DOM node)
   * * Support for custom parameters and attributes
   * * Support for Flash(TM) variables
   *
   * Does not include the following features from SWFFix:
   *
   * * Active content workarounds for already inserted movies (via markup)
   * * Express install support
   * * Transformation of standard conformance markup to cross browser support
   * * Support for alternative content (alt text)
   */
  qx.Class.define("qx.bom.Flash", {
    /*
    *****************************************************************************
       STATICS
    *****************************************************************************
    */
    statics: {
      /**
       * Saves the references to the flash objects to delete the flash objects
       * before the browser is closed. Note: it is only used in IE.
       */
      _flashObjects: {},

      /*
      ---------------------------------------------------------------------------
        CREATION
      ---------------------------------------------------------------------------
      */

      /**
       * Creates an DOM element
       *
       * The dimension of the movie should define through CSS styles {@link qx.bom.element.Style}
       *
       * It is possible to add these parameters as supported by Flash movies:
       * http://helpx.adobe.com/flash/kb/flash-object-embed-tag-attributes.html
       *
       * @param element {Element} Parent DOM element node to add flash movie
       * @param attributes {Map} attributes for the object tag like id or mayscript
       * @param variables {Map?null} Flash variable data (these are available in the movie later)
       * @param params {Map?null} Flash parameter data (these are used to configure the movie itself)
       * @param win {Window?null} Window to create the element for
       * @return {Element} The created Flash element
       */
      create: function create(element, attributes, variables, params, win) {
        if (!win) {
          win = window;
        } //Check parameters and check if element for flash is in DOM, before call creates swf.


        {
          qx.core.Assert.assertElement(element, "Invalid parameter 'element'.");
          qx.core.Assert.assertMap(attributes, "Invalid parameter 'attributes'.");
          qx.core.Assert.assertString(attributes.movie, "Invalid attribute 'movie'.");
          qx.core.Assert.assertString(attributes.id, "Invalid attribute 'id'.");

          if (!qx.dom.Element.isInDom(element, win)) {
            qx.log.Logger.warn(this, "The parent DOM element isn't in DOM! The External Interface doesn't work in IE!");
          }
        }

        if (!attributes.width) {
          attributes.width = "100%";
        }

        if (!attributes.height) {
          attributes.height = "100%";
        } // Work on param copy


        params = params ? qx.lang.Object.clone(params) : {};

        if (!params["movie"]) {
          params["movie"] = attributes.movie;
        }

        attributes["data"] = attributes.movie;
        delete attributes.movie; // Copy over variables (into params)

        if (variables) {
          for (var name in variables) {
            if (typeof params.flashvars != "undefined") {
              params.flashvars += "&" + name + "=" + variables[name];
            } else {
              params.flashvars = name + "=" + variables[name];
            }
          }
        } // Finally create the SWF


        var flash = this.__createSwf__P_345_0(element, attributes, params, win);

        this._flashObjects[attributes.id] = flash;
        return flash;
      },

      /**
       * Destroys the flash object from DOM, but not the parent DOM element.
       *
       * Note: Removing the flash object like this:
       * <pre>
       *  var div = qx.dom.Element.create("div");
       *  document.body.appendChild(div);
       *
       *  var flashObject = qx.bom.Flash.create(div, { movie : "Flash.swf", id : "id" });
       *  div.removeChild(div.firstChild);
       * </pre>
       * involve memory leaks in Internet Explorer.
       *
       * @param element {Element} Either the DOM element that contains
       *              the flash object or the flash object itself.
       * @param win {Window?} Window that the element, which is to be destroyed,
                      belongs to.
       * @signature function(element, win)
       */
      destroy: function destroy(element, win) {
        if (qx.core.Environment.get("engine.name") == "mshtml" && qx.core.Environment.get("browser.documentmode") < 11) {
          element = this.__getFlashObject__P_345_1(element);

          if (element.readyState == 4) {
            this.__destroyObjectInIE__P_345_2(element);
          } else {
            if (!win) {
              win = window;
            }

            qx.bom.Event.addNativeListener(win, "load", function () {
              qx.bom.Flash.__destroyObjectInIE__P_345_2(element);
            });
          }
        } else {
          element = this.__getFlashObject__P_345_1(element);

          if (element.parentNode) {
            element.parentNode.removeChild(element);
          }

          delete this._flashObjects[element.id];
        }
      },

      /**
       * Return the flash object element from DOM node.
       *
       * @param element {Element} The element to look.
       * @return {Element} Flash object element
       */
      __getFlashObject__P_345_1: function __getFlashObject__P_345_1(element) {
        if (!element) {
          throw new Error("DOM element is null or undefined!");
        }

        if (element.tagName.toLowerCase() !== "object") {
          element = element.firstChild;
        }

        if (!element || element.tagName.toLowerCase() !== "object") {
          throw new Error("DOM element has or is not a flash object!");
        }

        return element;
      },

      /**
       * Destroy the flash object and remove from DOM, to fix memory leaks.
       *
       * @signature function(element)
       * @param element {Element} Flash object element to destroy.
       */
      __destroyObjectInIE__P_345_2: qx.core.Environment.select("engine.name", {
        "mshtml": qx.event.GlobalError.observeMethod(function (element) {
          for (var i in element) {
            if (typeof element[i] == "function") {
              element[i] = null;
            }
          }

          if (element.parentNode) {
            element.parentNode.removeChild(element);
          }

          delete this._flashObjects[element.id];
        }),
        "default": null
      }),

      /**
       * Internal helper to prevent leaks in IE
       *
       * @signature function()
       */
      __fixOutOfMemoryError__P_345_3: qx.event.GlobalError.observeMethod(function () {
        // IE Memory Leak Fix
        for (var key in qx.bom.Flash._flashObjects) {
          qx.bom.Flash.destroy(qx.bom.Flash._flashObjects[key]);
        }

        window.__flash_unloadHandler__P_345_4 = function () {};

        window.__flash_savedUnloadHandler__P_345_5 = function () {}; // Remove listener again


        qx.bom.Event.removeNativeListener(window, "beforeunload", qx.bom.Flash.__fixOutOfMemoryError__P_345_3);
      }),

      /**
       * Creates a DOM element with a flash movie.
       *
       * @param element {Element} DOM element node where the Flash element node will be added.
       * @param attributes {Map} Flash attribute data.
       * @param params {Map} Flash parameter data.
       * @param win {Window} Window to create the element for.
       * @signature function(element, attributes, params, win)
       */
      __createSwf__P_345_0: function __createSwf__P_345_0(element, attributes, params, win) {
        if (qx.core.Environment.get("engine.name") == "mshtml" && qx.core.Environment.get("browser.documentmode") < 11) {
          // Move data from params to attributes
          params.movie = attributes.data;
          delete attributes.data; // Cleanup classid

          delete attributes.classid; // Prepare parameters

          var paramsStr = "";

          for (var name in params) {
            paramsStr += '<param name="' + name + '" value="' + params[name] + '" />';
          } // Create element, but set attribute "id" first and not later.


          if (attributes.id) {
            element.innerHTML = '<object id="' + attributes.id + '" classid="clsid:D27CDB6E-AE6D-11cf-96B8-444553540000">' + paramsStr + '</object>';
            delete attributes.id;
          } else {
            element.innerHTML = '<object classid="clsid:D27CDB6E-AE6D-11cf-96B8-444553540000">' + paramsStr + '</object>';
          } // Apply attributes


          for (var name in attributes) {
            // IE doesn't like dollar signs in attribute names.
            if (name.substring(0, 1) == "$") {
              element.firstChild[name] = attributes[name];
            } else {
              element.firstChild.setAttribute(name, attributes[name]);
            }
          }

          return element.firstChild;
        } // Cleanup


        delete attributes.classid;
        delete params.movie;
        var swf = qx.dom.Element.create("object", attributes, win);
        swf.setAttribute("type", "application/x-shockwave-flash"); // Add parameters

        var param;

        for (var name in params) {
          param = qx.dom.Element.create("param", {}, win);
          param.setAttribute("name", name);
          param.setAttribute("value", params[name]);
          swf.appendChild(param);
        }

        element.appendChild(swf);
        return swf;
      }
    },

    /*
    *****************************************************************************
       DEFER
    *****************************************************************************
    */
    defer: function defer(statics) {
      if (qx.core.Environment.get("engine.name") == "mshtml") {
        qx.bom.Event.addNativeListener(window, "beforeunload", statics.__fixOutOfMemoryError__P_345_3);
      }
    }
  });
  qx.bom.Flash.$$dbClassInfo = $$dbClassInfo;
})();

(function () {
  var $$dbClassInfo = {
    "dependsOn": {
      "qx.core.Environment": {
        "defer": "load",
        "usage": "dynamic",
        "require": true
      },
      "qx.Bootstrap": {
        "usage": "dynamic",
        "require": true
      },
      "qx.core.Assert": {},
      "qx.lang.Object": {},
      "qx.dom.Element": {},
      "qx.lang.Type": {},
      "qx.bom.client.Engine": {
        "require": true
      }
    },
    "environment": {
      "provided": [],
      "required": {
        "engine.name": {
          "className": "qx.bom.client.Engine",
          "load": true
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
  
     ======================================================================
  
     This class contains code based on the following work:
  
     * jQuery
       http://jquery.com
       Version 1.3.1
  
       Copyright:
         2009 John Resig
  
       License:
         MIT: http://www.opensource.org/licenses/mit-license.php
  
  ************************************************************************ */

  /**
   * Cross browser abstractions to work with input elements.
   */
  qx.Bootstrap.define("qx.bom.Input", {
    /*
    *****************************************************************************
       STATICS
    *****************************************************************************
    */
    statics: {
      /** @type {Map} Internal data structures with all supported input types */
      __types__P_346_0: {
        text: 1,
        textarea: 1,
        select: 1,
        checkbox: 1,
        radio: 1,
        password: 1,
        hidden: 1,
        submit: 1,
        image: 1,
        file: 1,
        search: 1,
        reset: 1,
        button: 1
      },

      /**
       * Creates an DOM input/textarea/select element.
       *
       * Attributes may be given directly with this call. This is critical
       * for some attributes e.g. name, type, ... in many clients.
       *
       * Note: <code>select</code> and <code>textarea</code> elements are created
       * using the identically named <code>type</code>.
       *
       * @param type {String} Any valid type for HTML, <code>select</code>
       *   and <code>textarea</code>
       * @param attributes {Map} Map of attributes to apply
       * @param win {Window} Window to create the element for
       * @return {Element} The created input node
       */
      create: function create(type, attributes, win) {
        {
          qx.core.Assert.assertKeyInMap(type, this.__types__P_346_0, "Unsupported input type.");
        } // Work on a copy to not modify given attributes map

        var attributes = attributes ? qx.lang.Object.clone(attributes) : {};
        var tag;

        if (type === "textarea" || type === "select") {
          tag = type;
        } else {
          tag = "input";
          attributes.type = type;
        }

        return qx.dom.Element.create(tag, attributes, win);
      },

      /**
       * Applies the given value to the element.
       *
       * Normally the value is given as a string/number value and applied
       * to the field content (textfield, textarea) or used to
       * detect whether the field is checked (checkbox, radiobutton).
       *
       * Supports array values for selectboxes (multiple-selection)
       * and checkboxes or radiobuttons (for convenience).
       *
       * Please note: To modify the value attribute of a checkbox or
       * radiobutton use {@link qx.bom.element.Attribute#set} instead.
       *
       * @param element {Element} element to update
       * @param value {String|Number|Array} the value to apply
       */
      setValue: function setValue(element, value) {
        var tag = element.nodeName.toLowerCase();
        var type = element.type;
        var Type = qx.lang.Type;

        if (typeof value === "number") {
          value += "";
        }

        if (type === "checkbox" || type === "radio") {
          if (Type.isArray(value)) {
            element.checked = value.includes(element.value);
          } else {
            element.checked = element.value == value;
          }
        } else if (tag === "select") {
          var isArray = Type.isArray(value);
          var options = element.options;
          var subel, subval;

          for (var i = 0, l = options.length; i < l; i++) {
            subel = options[i];
            subval = subel.getAttribute("value");

            if (subval == null) {
              subval = subel.text;
            }

            subel.selected = isArray ? value.includes(subval) : value == subval;
          }

          if (isArray && value.length == 0) {
            element.selectedIndex = -1;
          }
        } else if ((type === "text" || type === "textarea") && qx.core.Environment.get("engine.name") == "mshtml") {
          // These flags are required to detect self-made property-change
          // events during value modification. They are used by the Input
          // event handler to filter events.
          element.$$inValueSet = true;
          element.value = value;
          element.$$inValueSet = null;
        } else {
          element.value = value;
        }
      },

      /**
       * Returns the currently configured value.
       *
       * Works with simple input fields as well as with
       * select boxes or option elements.
       *
       * Returns an array in cases of multi-selection in
       * select boxes but in all other cases a string.
       *
       * @param element {Element} DOM element to query
       * @return {String|Array} The value of the given element
       */
      getValue: function getValue(element) {
        var tag = element.nodeName.toLowerCase();

        if (tag === "option") {
          return (element.attributes.value || {}).specified ? element.value : element.text;
        }

        if (tag === "select") {
          var index = element.selectedIndex; // Nothing was selected

          if (index < 0) {
            return null;
          }

          var values = [];
          var options = element.options;
          var one = element.type == "select-one";
          var clazz = qx.bom.Input;
          var value; // Loop through all the selected options

          for (var i = one ? index : 0, max = one ? index + 1 : options.length; i < max; i++) {
            var option = options[i];

            if (option.selected) {
              // Get the specific value for the option
              value = clazz.getValue(option); // We don't need an array for one selects

              if (one) {
                return value;
              } // Multi-Selects return an array


              values.push(value);
            }
          }

          return values;
        } else {
          return (element.value || "").replace(/\r/g, "");
        }
      },

      /**
       * Sets the text wrap behaviour of a text area element.
       * This property uses the attribute "wrap" respectively
       * the style property "whiteSpace"
       *
       * @signature function(element, wrap)
       * @param element {Element} DOM element to modify
       * @param wrap {Boolean} Whether to turn text wrap on or off.
       */
      setWrap: qx.core.Environment.select("engine.name", {
        "mshtml": function mshtml(element, wrap) {
          var wrapValue = wrap ? "soft" : "off"; // Explicitly set overflow-y CSS property to auto when wrapped,
          // allowing the vertical scroll-bar to appear if necessary

          var styleValue = wrap ? "auto" : "";
          element.wrap = wrapValue;
          element.style.overflowY = styleValue;
        },
        "gecko": function gecko(element, wrap) {
          var wrapValue = wrap ? "soft" : "off";
          var styleValue = wrap ? "" : "auto";
          element.setAttribute("wrap", wrapValue);
          element.style.overflow = styleValue;
        },
        "webkit": function webkit(element, wrap) {
          var wrapValue = wrap ? "soft" : "off";
          var styleValue = wrap ? "" : "auto";
          element.setAttribute("wrap", wrapValue);
          element.style.overflow = styleValue;
        },
        "default": function _default(element, wrap) {
          element.style.whiteSpace = wrap ? "normal" : "nowrap";
        }
      })
    }
  });
  qx.bom.Input.$$dbClassInfo = $$dbClassInfo;
})();

(function () {
  var $$dbClassInfo = {
    "dependsOn": {
      "qx.core.Environment": {
        "defer": "load",
        "construct": true,
        "require": true
      },
      "qx.Bootstrap": {
        "usage": "dynamic",
        "require": true
      },
      "qx.event.Emitter": {
        "require": true
      },
      "qx.bom.client.CssAnimation": {
        "construct": true
      }
    },
    "environment": {
      "provided": [],
      "required": {
        "css.animation": {
          "construct": true,
          "className": "qx.bom.client.CssAnimation"
        }
      }
    }
  };
  qx.Bootstrap.executePendingDefers($$dbClassInfo);

  /* ************************************************************************
  
     qooxdoo - the new era of web development
  
     http://qooxdoo.org
  
     Copyright:
       2011 1&1 Internet AG, Germany, http://www.1und1.de
  
     License:
       MIT: https://opensource.org/licenses/MIT
       See the LICENSE file in the project's top-level directory for details.
  
     Authors:
       * Martin Wittemann (wittemann)
  
  ************************************************************************ */

  /**
   * This is a simple handle, which will be returned when an animation is
   * started using the {@link qx.bom.element.Animation#animate} method. It
   * basically controls the animation.
   *
   * @ignore(qx.bom.element.AnimationJs)
   */
  qx.Bootstrap.define("qx.bom.element.AnimationHandle", {
    extend: qx.event.Emitter,
    construct: function construct() {
      var css = qx.core.Environment.get("css.animation");
      this.__playState__P_160_0 = css && css["play-state"];
      this.__playing__P_160_1 = true;
      this.addListenerOnce("end", this.__setEnded__P_160_2, this);
    },
    events: {
      /** Fired when the animation started via {@link qx.bom.element.Animation}. */
      "start": "Element",

      /**
       * Fired when the animation started via {@link qx.bom.element.Animation} has
       * ended.
       */
      "end": "Element",

      /** Fired on every iteration of the animation. */
      "iteration": "Element"
    },
    members: {
      __playState__P_160_0: null,
      __playing__P_160_1: false,
      __ended__P_160_3: false,

      /**
       * Accessor of the playing state.
       * @return {Boolean} <code>true</code>, if the animations is playing.
       */
      isPlaying: function isPlaying() {
        return this.__playing__P_160_1;
      },

      /**
       * Accessor of the ended state.
       * @return {Boolean} <code>true</code>, if the animations has ended.
       */
      isEnded: function isEnded() {
        return this.__ended__P_160_3;
      },

      /**
       * Accessor of the paused state.
       * @return {Boolean} <code>true</code>, if the animations is paused.
       */
      isPaused: function isPaused() {
        return this.el.style[this.__playState__P_160_0] == "paused";
      },

      /**
       * Pauses the animation, if running. If not running, it will be ignored.
       */
      pause: function pause() {
        if (this.el) {
          this.el.style[this.__playState__P_160_0] = "paused";
          this.el.$$animation.__playing__P_160_1 = false; // in case the animation is based on JS

          if (this.animationId && qx.bom.element.AnimationJs) {
            qx.bom.element.AnimationJs.pause(this);
          }
        }
      },

      /**
       * Resumes an animation. This does not start the animation once it has ended.
       * In this case you need to start a new Animation.
       */
      play: function play() {
        if (this.el) {
          this.el.style[this.__playState__P_160_0] = "running";
          this.el.$$animation.__playing__P_160_1 = true; // in case the animation is based on JS

          if (this.i != undefined && qx.bom.element.AnimationJs) {
            qx.bom.element.AnimationJs.play(this);
          }
        }
      },

      /**
       * Stops the animation if running.
       */
      stop: function stop() {
        if (this.el && qx.core.Environment.get("css.animation") && !this.jsAnimation) {
          this.el.style[this.__playState__P_160_0] = "";
          this.el.style[qx.core.Environment.get("css.animation").name] = "";
          this.el.$$animation.__playing__P_160_1 = false;
          this.el.$$animation.__ended__P_160_3 = true;
        } // in case the animation is based on JS
        else if (this.jsAnimation) {
            this.stopped = true;
            qx.bom.element.AnimationJs.stop(this);
          }
      },

      /**
       * Set the animation state to ended
       */
      __setEnded__P_160_2: function __setEnded__P_160_2() {
        this.__playing__P_160_1 = false;
        this.__ended__P_160_3 = true;
      }
    }
  });
  qx.bom.element.AnimationHandle.$$dbClassInfo = $$dbClassInfo;
})();

(function () {
  var $$dbClassInfo = {
    "dependsOn": {
      "qx.Bootstrap": {
        "usage": "dynamic",
        "require": true
      },
      "qx.xml.Document": {},
      "qx.core.Environment": {
        "defer": "runtime"
      }
    },
    "environment": {
      "provided": ["xml.implementation", "xml.domparser", "xml.selectsinglenode", "xml.selectnodes", "xml.getelementsbytagnamens", "xml.domproperties", "xml.attributens", "xml.createelementns", "xml.createnode", "xml.getqualifieditem"],
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
       * Daniel Wagner (d_wagner)
  
  ************************************************************************ */

  /**
   * Internal class which contains the checks used by {@link qx.core.Environment}.
   * All checks in here are marked as internal which means you should never use
   * them directly.
   *
   * This class should contain all XML-related checks
   *
   * @internal
   */
  qx.Bootstrap.define("qx.bom.client.Xml", {
    statics: {
      /**
       * Checks if XML is supported
       *
       * @internal
       * @return {Boolean} <code>true</code> if XML is supported
       */
      getImplementation: function getImplementation() {
        return document.implementation && document.implementation.hasFeature && document.implementation.hasFeature("XML", "1.0");
      },

      /**
       * Checks if an XML DOMParser is available
       *
       * @internal
       * @return {Boolean} <code>true</code> if DOMParser is supported
       */
      getDomParser: function getDomParser() {
        return typeof window.DOMParser !== "undefined";
      },

      /**
       * Checks if the proprietary selectSingleNode method is available on XML DOM
       * nodes.
       *
       * @internal
       * @return {Boolean} <code>true</code> if selectSingleNode is available
       */
      getSelectSingleNode: function getSelectSingleNode() {
        return typeof qx.xml.Document.create().selectSingleNode !== "undefined";
      },

      /**
       * Checks if the proprietary selectNodes method is available on XML DOM
       * nodes.
       *
       * @internal
       * @return {Boolean} <code>true</code> if selectSingleNode is available
       */
      getSelectNodes: function getSelectNodes() {
        return typeof qx.xml.Document.create().selectNodes !== "undefined";
      },

      /**
       * Checks availability of the getElementsByTagNameNS XML DOM method.
       *
       * @internal
       * @return {Boolean} <code>true</code> if getElementsByTagNameNS is available
       */
      getElementsByTagNameNS: function getElementsByTagNameNS() {
        return typeof qx.xml.Document.create().getElementsByTagNameNS !== "undefined";
      },

      /**
       * Checks if MSXML-style DOM Level 2 properties are supported.
       *
       * @internal
       * @return {Boolean} <code>true</code> if DOM Level 2 properties are supported
       */
      getDomProperties: function getDomProperties() {
        var doc = qx.xml.Document.create();
        return "getProperty" in doc && typeof doc.getProperty("SelectionLanguage") === "string";
      },

      /**
       * Checks if the getAttributeNS and setAttributeNS methods are supported on
       * XML DOM elements
       *
       * @internal
       * @return {Boolean} <code>true</code> if get/setAttributeNS is supported
       */
      getAttributeNS: function getAttributeNS() {
        var docElem = qx.xml.Document.fromString("<a></a>").documentElement;
        return typeof docElem.getAttributeNS === "function" && typeof docElem.setAttributeNS === "function";
      },

      /**
       * Checks if the createElementNS method is supported on XML DOM documents
       *
       * @internal
       * @return {Boolean} <code>true</code> if createElementNS is supported
       */
      getCreateElementNS: function getCreateElementNS() {
        return typeof qx.xml.Document.create().createElementNS === "function";
      },

      /**
       * Checks if the proprietary createNode method is supported on XML DOM
       * documents
       *
       * @internal
       * @return {Boolean} <code>true</code> if DOM Level 2 properties are supported
       */
      getCreateNode: function getCreateNode() {
        return typeof qx.xml.Document.create().createNode !== "undefined";
      },

      /**
       * Checks if the proprietary getQualifiedItem method is supported for XML
       * element attributes
       *
       * @internal
       * @return {Boolean} <code>true</code> if DOM Level 2 properties are supported
       */
      getQualifiedItem: function getQualifiedItem() {
        var docElem = qx.xml.Document.fromString("<a></a>").documentElement;
        return typeof docElem.attributes.getQualifiedItem !== "undefined";
      }
    },
    defer: function defer(statics) {
      qx.core.Environment.add("xml.implementation", statics.getImplementation);
      qx.core.Environment.add("xml.domparser", statics.getDomParser);
      qx.core.Environment.add("xml.selectsinglenode", statics.getSelectSingleNode);
      qx.core.Environment.add("xml.selectnodes", statics.getSelectNodes);
      qx.core.Environment.add("xml.getelementsbytagnamens", statics.getElementsByTagNameNS);
      qx.core.Environment.add("xml.domproperties", statics.getDomProperties);
      qx.core.Environment.add("xml.attributens", statics.getAttributeNS);
      qx.core.Environment.add("xml.createelementns", statics.getCreateElementNS);
      qx.core.Environment.add("xml.createnode", statics.getCreateNode);
      qx.core.Environment.add("xml.getqualifieditem", statics.getQualifiedItem);
    }
  });
  qx.bom.client.Xml.$$dbClassInfo = $$dbClassInfo;
})();

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

(function () {
  var $$dbClassInfo = {
    "dependsOn": {
      "qx.core.Environment": {
        "defer": "load",
        "usage": "dynamic",
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
      "qx.lang.Function": {},
      "qx.event.GlobalError": {},
      "qx.bom.Event": {},
      "qx.event.Idle": {},
      "qx.lang.Type": {},
      "qx.bom.client.Engine": {
        "require": true
      },
      "qx.event.Timer": {}
    },
    "environment": {
      "provided": [],
      "required": {
        "engine.name": {
          "load": true,
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
       * Andreas Ecker (ecker)
       * Fabian Jakobs (fjakobs)
  
  ************************************************************************ */

  /**
   * Default history manager implementation. Either polls for URL fragment
   * identifier (hash) changes or uses the native "hashchange" event.
   *
   * NOTE: Instances of this class must be disposed of after use
   *
   * @internal
   */
  qx.Class.define("qx.bom.NativeHistory", {
    extend: qx.bom.History,
    implement: [qx.core.IDisposable],
    construct: function construct() {
      qx.bom.History.constructor.call(this);

      this.__attachListeners__P_179_0();
    },
    members: {
      __checkOnHashChange__P_179_1: null,

      /**
       * Attach hash change listeners
       */
      __attachListeners__P_179_0: function __attachListeners__P_179_0() {
        if (qx.bom.History.SUPPORTS_HASH_CHANGE_EVENT) {
          var boundFunc = qx.lang.Function.bind(this.__onHashChange__P_179_2, this);
          this.__checkOnHashChange__P_179_1 = qx.event.GlobalError.observeMethod(boundFunc);
          qx.bom.Event.addNativeListener(window, "hashchange", this.__checkOnHashChange__P_179_1);
        } else {
          qx.event.Idle.getInstance().addListener("interval", this.__onHashChange__P_179_2, this);
        }
      },

      /**
       * Remove hash change listeners
       */
      __detatchListeners__P_179_3: function __detatchListeners__P_179_3() {
        if (qx.bom.History.SUPPORTS_HASH_CHANGE_EVENT) {
          qx.bom.Event.removeNativeListener(window, "hashchange", this.__checkOnHashChange__P_179_1);
        } else {
          qx.event.Idle.getInstance().removeListener("interval", this.__onHashChange__P_179_2, this);
        }
      },

      /**
       * hash change event handler
       */
      __onHashChange__P_179_2: function __onHashChange__P_179_2() {
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
        return this._decode(this._getHash());
      },

      /**
       * Save a state into the browser history.
       *
       * @param state {String} state to save
       */
      _writeState: qx.core.Environment.select("engine.name", {
        "opera": function opera(state) {
          qx.event.Timer.once(function () {
            this._setHash(this._encode(state));
          }, this, 0);
        },
        "default": function _default(state) {
          this._setHash(this._encode(state));
        }
      })
    },
    destruct: function destruct() {
      this.__detatchListeners__P_179_3();
    }
  });
  qx.bom.NativeHistory.$$dbClassInfo = $$dbClassInfo;
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
      "qx.event.Timer": {},
      "qx.bom.element.Dimension": {},
      "qx.lang.Object": {},
      "qx.bom.element.Style": {}
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
  
  ************************************************************************ */

  /**
   * Checks whether a given font is available on the document and fires events
   * accordingly.
   * 
   * This class does not need to be disposed, unless you want to abort the validation
   * early
   */
  qx.Class.define("qx.bom.webfonts.Validator", {
    extend: qx.core.Object,

    /*
    *****************************************************************************
       CONSTRUCTOR
    *****************************************************************************
    */

    /**
     * @param fontFamily {String} The name of the font to be verified
     * @param comparisonString {String?} String to be used to detect whether a font was loaded or not
     * whether the font has loaded properly
     */
    construct: function construct(fontFamily, comparisonString) {
      qx.core.Object.constructor.call(this);

      if (comparisonString) {
        this.setComparisonString(comparisonString);
      }

      if (fontFamily) {
        this.setFontFamily(fontFamily);
        this.__requestedHelpers__P_152_0 = this._getRequestedHelpers();
      }
    },

    /*
    *****************************************************************************
       STATICS
    *****************************************************************************
    */
    statics: {
      /**
       * Sets of serif and sans-serif fonts to be used for size comparisons.
       * At least one of these fonts should be present on any system.
       */
      COMPARISON_FONTS: {
        sans: ["Arial", "Helvetica", "sans-serif"],
        serif: ["Times New Roman", "Georgia", "serif"]
      },

      /**
       * Map of common CSS attributes to be used for all  size comparison elements
       */
      HELPER_CSS: {
        position: "absolute",
        margin: "0",
        padding: "0",
        top: "-1000px",
        left: "-1000px",
        fontSize: "350px",
        width: "auto",
        height: "auto",
        lineHeight: "normal",
        fontVariant: "normal",
        visibility: "hidden"
      },

      /**
       * The string to be used in the size comparison elements. This is the default string
       * which is used for the {@link #COMPARISON_FONTS} and the font to be validated. It
       * can be overridden for the font to be validated using the {@link #comparisonString}
       * property.
       */
      COMPARISON_STRING: "WEei",
      __defaultSizes__P_152_1: null,
      __defaultHelpers__P_152_2: null,

      /**
       * Removes the two common helper elements used for all size comparisons from
       * the DOM
       */
      removeDefaultHelperElements: function removeDefaultHelperElements() {
        var defaultHelpers = qx.bom.webfonts.Validator.__defaultHelpers__P_152_2;

        if (defaultHelpers) {
          for (var prop in defaultHelpers) {
            document.body.removeChild(defaultHelpers[prop]);
          }
        }

        delete qx.bom.webfonts.Validator.__defaultHelpers__P_152_2;
      }
    },

    /*
    *****************************************************************************
       PROPERTIES
    *****************************************************************************
    */
    properties: {
      /**
       * The font-family this validator should check
       */
      fontFamily: {
        nullable: true,
        init: null,
        apply: "_applyFontFamily"
      },

      /**
       * Comparison string used to check whether the font has loaded or not.
       */
      comparisonString: {
        nullable: true,
        init: null
      },

      /**
       * Time in milliseconds from the beginning of the check until it is assumed
       * that a font is not available
       */
      timeout: {
        check: "Integer",
        init: 5000
      }
    },

    /*
    *****************************************************************************
       EVENTS
    *****************************************************************************
    */
    events: {
      /**
       * Fired when the status of a web font has been determined. The event data
       * is a map with the keys "family" (the font-family name) and "valid"
       * (Boolean).
       */
      "changeStatus": "qx.event.type.Data"
    },

    /*
    *****************************************************************************
       MEMBERS
    *****************************************************************************
    */
    members: {
      __requestedHelpers__P_152_0: null,
      __checkTimer__P_152_3: null,
      __checkStarted__P_152_4: null,

      /*
      ---------------------------------------------------------------------------
        PUBLIC API
      ---------------------------------------------------------------------------
      */

      /**
       * Validates the font
       */
      validate: function validate() {
        this.__checkStarted__P_152_4 = new Date().getTime();

        if (this.__checkTimer__P_152_3) {
          this.__checkTimer__P_152_3.restart();
        } else {
          this.__checkTimer__P_152_3 = new qx.event.Timer(100);

          this.__checkTimer__P_152_3.addListener("interval", this.__onTimerInterval__P_152_5, this); // Give the browser a chance to render the new elements


          qx.event.Timer.once(function () {
            this.__checkTimer__P_152_3.start();
          }, this, 0);
        }
      },

      /*
      ---------------------------------------------------------------------------
        PROTECTED API
      ---------------------------------------------------------------------------
      */

      /**
       * Removes the helper elements from the DOM
       */
      _reset: function _reset() {
        if (this.__requestedHelpers__P_152_0) {
          for (var prop in this.__requestedHelpers__P_152_0) {
            var elem = this.__requestedHelpers__P_152_0[prop];
            document.body.removeChild(elem);
          }

          this.__requestedHelpers__P_152_0 = null;
        }
      },

      /**
       * Checks if the font is available by comparing the widths of the elements
       * using the generic fonts to the widths of the elements using the font to
       * be validated
       *
       * @return {Boolean} Whether or not the font caused the elements to differ
       * in size
       */
      _isFontValid: function _isFontValid() {
        if (!qx.bom.webfonts.Validator.__defaultSizes__P_152_1) {
          this.__init__P_152_6();
        }

        if (!this.__requestedHelpers__P_152_0) {
          this.__requestedHelpers__P_152_0 = this._getRequestedHelpers();
        } // force rerendering for chrome


        this.__requestedHelpers__P_152_0.sans.style.visibility = "visible";
        this.__requestedHelpers__P_152_0.sans.style.visibility = "hidden";
        this.__requestedHelpers__P_152_0.serif.style.visibility = "visible";
        this.__requestedHelpers__P_152_0.serif.style.visibility = "hidden";
        var requestedSans = qx.bom.element.Dimension.getWidth(this.__requestedHelpers__P_152_0.sans);
        var requestedSerif = qx.bom.element.Dimension.getWidth(this.__requestedHelpers__P_152_0.serif);
        var cls = qx.bom.webfonts.Validator;

        if (requestedSans !== cls.__defaultSizes__P_152_1.sans || requestedSerif !== cls.__defaultSizes__P_152_1.serif) {
          return true;
        }

        return false;
      },

      /**
       * Creates the two helper elements styled with the font to be checked
       *
       * @return {Map} A map with the keys <pre>sans</pre> and <pre>serif</pre>
       * and the created span elements as values
       */
      _getRequestedHelpers: function _getRequestedHelpers() {
        var fontsSans = [this.getFontFamily()].concat(qx.bom.webfonts.Validator.COMPARISON_FONTS.sans);
        var fontsSerif = [this.getFontFamily()].concat(qx.bom.webfonts.Validator.COMPARISON_FONTS.serif);
        return {
          sans: this._getHelperElement(fontsSans, this.getComparisonString()),
          serif: this._getHelperElement(fontsSerif, this.getComparisonString())
        };
      },

      /**
       * Creates a span element with the comparison text (either {@link #COMPARISON_STRING} or
       * {@link #comparisonString}) and styled with the default CSS ({@link #HELPER_CSS}) plus
       * the given font-family value and appends it to the DOM
       *
       * @param fontFamily {String} font-family string
       * @param comparisonString {String?} String to be used to detect whether a font was loaded or not
       * @return {Element} the created DOM element
       */
      _getHelperElement: function _getHelperElement(fontFamily, comparisonString) {
        var styleMap = qx.lang.Object.clone(qx.bom.webfonts.Validator.HELPER_CSS);

        if (fontFamily) {
          if (styleMap.fontFamily) {
            styleMap.fontFamily += "," + fontFamily.join(",");
          } else {
            styleMap.fontFamily = fontFamily.join(",");
          }
        }

        var elem = document.createElement("span");
        elem.innerHTML = comparisonString || qx.bom.webfonts.Validator.COMPARISON_STRING;
        qx.bom.element.Style.setStyles(elem, styleMap);
        document.body.appendChild(elem);
        return elem;
      },
      // property apply
      _applyFontFamily: function _applyFontFamily(value, old) {
        if (value !== old) {
          this._reset();
        }
      },

      /*
      ---------------------------------------------------------------------------
        PRIVATE API
      ---------------------------------------------------------------------------
      */

      /**
       * Creates the default helper elements and gets their widths
       */
      __init__P_152_6: function __init__P_152_6() {
        var cls = qx.bom.webfonts.Validator;

        if (!cls.__defaultHelpers__P_152_2) {
          cls.__defaultHelpers__P_152_2 = {
            sans: this._getHelperElement(cls.COMPARISON_FONTS.sans),
            serif: this._getHelperElement(cls.COMPARISON_FONTS.serif)
          };
        }

        cls.__defaultSizes__P_152_1 = {
          sans: qx.bom.element.Dimension.getWidth(cls.__defaultHelpers__P_152_2.sans),
          serif: qx.bom.element.Dimension.getWidth(cls.__defaultHelpers__P_152_2.serif)
        };
      },

      /**
       * Triggers helper element size comparison and fires a ({@link #changeStatus})
       * event with the result.
       */
      __onTimerInterval__P_152_5: function __onTimerInterval__P_152_5() {
        if (this._isFontValid()) {
          this.__checkTimer__P_152_3.stop();

          this._reset();

          this.fireDataEvent("changeStatus", {
            family: this.getFontFamily(),
            valid: true
          });
        } else {
          var now = new Date().getTime();

          if (now - this.__checkStarted__P_152_4 >= this.getTimeout()) {
            this.__checkTimer__P_152_3.stop();

            this._reset();

            this.fireDataEvent("changeStatus", {
              family: this.getFontFamily(),
              valid: false
            });
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
      this._reset();

      this.__checkTimer__P_152_3.stop();

      this.__checkTimer__P_152_3.removeListener("interval", this.__onTimerInterval__P_152_5, this);

      this._disposeObjects("__checkTimer__P_152_3");
    }
  });
  qx.bom.webfonts.Validator.$$dbClassInfo = $$dbClassInfo;
})();

(function () {
  var $$dbClassInfo = {
    "dependsOn": {
      "qx.core.Environment": {
        "defer": "load",
        "usage": "dynamic",
        "require": true
      },
      "qx.Theme": {
        "usage": "dynamic",
        "require": true
      },
      "qx.bom.client.Css": {
        "require": true
      }
    },
    "environment": {
      "provided": [],
      "required": {
        "css.rgba": {
          "load": true,
          "className": "qx.bom.client.Css"
        }
      }
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
       * Tristan Koch (trkoch)
  
  ************************************************************************ */

  /**
   * Indigo color theme
   */
  qx.Theme.define("qx.theme.indigo.Color", {
    colors: {
      // main
      "background": "white",
      "dark-blue": "#323335",
      "light-background": "#F4F4F4",
      "font": "#262626",
      "highlight": "#3D72C9",
      // bright blue
      "highlight-shade": "#5583D0",
      // bright blue
      // backgrounds
      "background-selected": "#3D72C9",
      "background-selected-disabled": "#CDCDCD",
      "background-selected-dark": "#323335",
      "background-disabled": "#F7F7F7",
      "background-disabled-checked": "#BBBBBB",
      "background-pane": "white",
      // tabview
      "tabview-unselected": "#1866B5",
      "tabview-button-border": "#134983",
      "tabview-label-active-disabled": "#D9D9D9",
      // text colors
      "link": "#24B",
      // scrollbar
      "scrollbar-bright": "#F1F1F1",
      "scrollbar-dark": "#EBEBEB",
      // form
      "button": "#E8F0E3",
      "button-border": "#BBB",
      "button-border-hovered": "#939393",
      "invalid": "#C00F00",
      "button-box-bright": "#F9F9F9",
      "button-box-dark": "#E3E3E3",
      "button-box-bright-pressed": "#BABABA",
      "button-box-dark-pressed": "#EBEBEB",
      "border-lead": "#888888",
      // window
      "window-border": "#dddddd",
      "window-border-inner": "#F4F4F4",
      // group box
      "white-box-border": "#dddddd",
      // shadows
      "shadow": qx.core.Environment.get("css.rgba") ? "rgba(0, 0, 0, 0.4)" : "#666666",
      // borders
      "border-main": "#dddddd",
      "border-light": "#B7B7B7",
      "border-light-shadow": "#686868",
      // separator
      "border-separator": "#808080",
      // text
      "text": "#262626",
      "text-disabled": "#A7A6AA",
      "text-selected": "white",
      "text-placeholder": "#CBC8CD",
      // tooltip
      "tooltip": "#FE0",
      "tooltip-text": "black",
      // table
      "table-header": [242, 242, 242],
      "table-focus-indicator": "#3D72C9",
      // used in table code
      "table-header-cell": [235, 234, 219],
      "table-row-background-focused-selected": "#3D72C9",
      "table-row-background-focused": "#F4F4F4",
      "table-row-background-selected": [51, 94, 168],
      "table-row-background-even": "white",
      "table-row-background-odd": "white",
      "table-row-selected": [255, 255, 255],
      "table-row": [0, 0, 0],
      "table-row-line": "#EEE",
      "table-column-line": "#EEE",
      // used in progressive code
      "progressive-table-header": "#AAAAAA",
      "progressive-table-row-background-even": [250, 248, 243],
      "progressive-table-row-background-odd": [255, 255, 255],
      "progressive-progressbar-background": "gray",
      "progressive-progressbar-indicator-done": "#CCCCCC",
      "progressive-progressbar-indicator-undone": "white",
      "progressive-progressbar-percent-background": "gray",
      "progressive-progressbar-percent-text": "white"
    }
  });
  qx.theme.indigo.Color.$$dbClassInfo = $$dbClassInfo;
})();
//# sourceMappingURL=package-7.js.map?dt=1598051439986
qx.$$packageData['7'] = {
  "locales": {},
  "resources": {},
  "translations": {}
};
