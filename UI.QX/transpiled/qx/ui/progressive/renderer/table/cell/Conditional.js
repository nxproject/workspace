(function () {
  var $$dbClassInfo = {
    "dependsOn": {
      "qx.Class": {
        "usage": "dynamic",
        "require": true
      },
      "qx.ui.progressive.renderer.table.cell.Abstract": {
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
       2007 by Tartan Solutions, Inc, http://www.tartansolutions.com
       2008 Derrell Lipman
  
     License:
       MIT: https://opensource.org/licenses/MIT
       See the LICENSE file in the project's top-level directory for details.
  
     Authors:
       * Dan Hummon
       * Derrell Lipman (derrell)
  
  ************************************************************************ */

  /**
   * Table Cell Renderer for Progressive.
   */
  qx.Class.define("qx.ui.progressive.renderer.table.cell.Conditional", {
    extend: qx.ui.progressive.renderer.table.cell.Abstract,

    /**
     * @param align {String}
     *   The default alignment to format the cell with if the condition matches.
     *
     * @param color {String}
     *   The default color to format the cell with if the condition matches.
     *
     * @param style {String}
     *   The default style to format the cell with if the condition matches.
     *
     * @param weight {String}
     *   The default weight to format the cell with if the condition matches.
     */
    construct: function construct(align, color, style, weight) {
      qx.ui.progressive.renderer.table.cell.Abstract.constructor.call(this);
      this.__numericAllowed__P_331_0 = ["==", "!=", ">", "<", ">=", "<="];
      this.__betweenAllowed__P_331_1 = ["between", "!between"];
      this.__conditions__P_331_2 = [];
      this.__defaultTextAlign__P_331_3 = align || "";
      this.__defaultColor__P_331_4 = color || "";
      this.__defaultFontStyle__P_331_5 = style || "";
      this.__defaultFontWeight__P_331_6 = weight || "";
    },
    members: {
      __numericAllowed__P_331_0: null,
      __betweenAllowed__P_331_1: null,
      __conditions__P_331_2: null,
      __defaultTextAlign__P_331_3: null,
      __defaultColor__P_331_4: null,
      __defaultFontStyle__P_331_5: null,
      __defaultFontWeight__P_331_6: null,

      /**
       * Applies the cell styles to the style map.
       *
       * @param condition {Array}
       *   The matched condition
       *
       * @param style {Map}
       *   map of already applied styles.
       */
      __applyFormatting__P_331_7: function __applyFormatting__P_331_7(condition, style) {
        if (condition.align) {
          style["text-align"] = condition.align;
        }

        if (condition.color) {
          style["color"] = condition.color;
        }

        if (condition.style) {
          style["font-style"] = condition.style;
        }

        if (condition.weight) {
          style["font-weight"] = condition.weight;
        }
      },

      /**
       * The addNumericCondition method is used to add a basic numeric condition
       * to the cell renderer.
       *
       * Note: Passing null is different from passing an empty string in the
       * align, color, style and weight arguments. Null will allow pre-existing
       * formatting to pass through, where an empty string will clear it back to
       * the default formatting set in the constructor.
       *
       *
       *
       * @param condition {String}
       *   The type of condition. Accepted strings are "==", "!=", ">", "<",
       *   ">=", and "<=".
       *
       * @param value1 {Integer}
       *   The value to compare against.
       *
       * @param align {String}
       *   The alignment to format the cell with if the condition matches.
       *
       * @param color {String}
       *   The color to format the cell with if the condition matches.
       *
       * @param style {String}
       *   The style to format the cell with if the condition matches.
       *
       * @param weight {String}
       *   The weight to format the cell with if the condition matches.
       *
       * @param target {String}
       *   The text value of the column to compare against. If this is null,
       *   comparisons will be against the contents of this cell.
       *
       * @throws {Error} If the condition can not be recognized or the value
       * is null.
       */
      addNumericCondition: function addNumericCondition(condition, value1, align, color, style, weight, target) {
        if (!this.__numericAllowed__P_331_0.includes(condition) || value1 == null) {
          throw new Error("Condition not recognized or value is null!");
        }

        this.__conditions__P_331_2.push({
          condition: condition,
          align: align,
          color: color,
          style: style,
          weight: weight,
          value1: value1,
          target: target
        });
      },

      /**
       * The addBetweenCondition method is used to add a between condition to
       * the cell renderer.
       *
       * Note: Passing null is different from passing an empty string in the
       * align, color, style and weight arguments. Null will allow pre-existing
       * formatting to pass through, where an empty string will clear it back to
       * the default formatting set in the constructor.
       *
       *
       *
       * @param condition {String}
       *   The type of condition. Accepted strings are "between" and "!between".
       *
       * @param value1 {Integer}
       *   The first value to compare against.
       *
       * @param value2 {Integer}
       *   The second value to compare against.
       *
       * @param align {String}
       *   The alignment to format the cell with if the condition matches.
       *
       * @param color {String}
       *   The color to format the cell with if the condition matches.
       *
       * @param style {String}
       *   The style to format the cell with if the condition matches.
       *
       * @param weight {String}
       *   The weight to format the cell with if the condition matches.
       *
       * @param target {String}
       *   The text value of the column to compare against. If this is null,
       *   comparisons will be against the contents of this cell.
       *
       *
       * @throws {Error} If the condition can not recognized or one of the
       * values is null.
       */
      addBetweenCondition: function addBetweenCondition(condition, value1, value2, align, color, style, weight, target) {
        if (!this.__betweenAllowed__P_331_1.includes(condition) || value1 == null || value2 == null) {
          throw new Error("Condition not recognized or value1/value2 is null!");
        }

        this.__conditions__P_331_2.push({
          condition: condition,
          align: align,
          color: color,
          style: style,
          weight: weight,
          value1: value1,
          value2: value2,
          target: target
        });
      },

      /**
       * The addRegex method is used to add a regular expression condition to
       * the cell renderer.
       *
       * Note: Passing null is different from passing an empty string in the
       * align, color, style and weight arguments. Null will allow pre-existing
       * formatting to pass through, where an empty string will clear it back to
       * the default formatting set in the constructor.
       *
       *
       *
       * @param regex {String}
       *   The regular expression to match against.
       *
       * @param align {String}
       *   The alignment to format the cell with if the condition matches.
       *
       * @param color {String}
       *   The color to format the cell with if the condition matches.
       *
       * @param style {String}
       *   The style to format the cell with if the condition matches.
       *
       * @param weight {String}
       *   The weight to format the cell with if the condition matches.
       *
       * @param target {String}
       *   The text value of the column to compare against. If this is null,
       *   comparisons will be against the contents of this cell.
       *
       * @throws {Error} If the regex is null.
       */
      addRegex: function addRegex(regex, align, color, style, weight, target) {
        if (!regex) {
          throw new Error("regex cannot be null!");
        }

        this.__conditions__P_331_2.push({
          condition: "regex",
          align: align,
          color: color,
          style: style,
          weight: weight,
          regex: regex,
          target: target
        });
      },

      /**
       * Overridden; called whenever the cell updates. The cell will iterate
       * through each available condition and apply formatting for those that
       * match. Multiple conditions can match, but later conditions will
       * override earlier ones. Conditions with null values will stack with
       * other conditions that apply to that value.
       *
       *
       * @param cellInfo {Map}
       *   The information about the cell.  See {@link qx.ui.table.cellrenderer.Abstract#createDataCellHtml}.
       *
       * @return {String}
       */
      _getCellStyle: function _getCellStyle(cellInfo) {
        if (this.__conditions__P_331_2.length == 0) {
          return cellInfo.style || "";
        }

        var i;
        var bTestPassed;
        var compareValue;
        var style = {
          "text-align": this.__defaultTextAlign__P_331_3,
          "color": this.__defaultColor__P_331_4,
          "font-style": this.__defaultFontStyle__P_331_5,
          "font-weight": this.__defaultFontWeight__P_331_6
        };

        for (i = 0; i < this.__conditions__P_331_2.length; i++) {
          var test = this.__conditions__P_331_2[i];
          bTestPassed = false;

          if (this.__numericAllowed__P_331_0.includes(test.condition)) {
            if (test.target == null) {
              compareValue = cellInfo.cellData;
            } else {
              compareValue = cellInfo.element.data[test.target];
            }

            switch (test.condition) {
              case "==":
                if (compareValue == test.value1) {
                  bTestPassed = true;
                }

                break;

              case "!=":
                if (compareValue != test.value1) {
                  bTestPassed = true;
                }

                break;

              case ">":
                if (compareValue > test.value1) {
                  bTestPassed = true;
                }

                break;

              case "<":
                if (compareValue < test.value1) {
                  bTestPassed = true;
                }

                break;

              case ">=":
                if (compareValue >= test.value1) {
                  bTestPassed = true;
                }

                break;

              case "<=":
                if (compareValue <= test.value1) {
                  bTestPassed = true;
                }

                break;
            }
          } else if (this.__betweenAllowed__P_331_1.includes(test.condition)) {
            if (test.target == null) {
              compareValue = cellInfo.cellData;
            } else {
              compareValue = cellInfo.element.data[test.target];
            }

            switch (test.condition) {
              case "between":
                if (compareValue >= test.value1 && compareValue <= test.value2) {
                  bTestPassed = true;
                }

                break;

              case "!between":
                if (compareValue < test.value1 && compareValue > test.value2) {
                  bTestPassed = true;
                }

                break;
            }
          } else if (test.condition == "regex") {
            if (test.target == null) {
              compareValue = cellInfo.cellData;
            } else {
              compareValue = cellInfo.element.data[test.target];
            }

            var the_pattern = new RegExp(test.value1, 'g');
            bTestPassed = the_pattern.test(compareValue);
          } // Apply formatting, if any.


          if (bTestPassed) {
            this.__applyFormatting__P_331_7(test, style);
          }

          var styleString = [];

          for (var key in style) {
            if (style[key]) {
              styleString.push(key, ":", style[key], ";");
            }
          }
        }

        return styleString.join("");
      }
    },
    destruct: function destruct() {
      this.__numericAllowed__P_331_0 = this.__betweenAllowed__P_331_1 = this.__conditions__P_331_2 = null;
    }
  });
  qx.ui.progressive.renderer.table.cell.Conditional.$$dbClassInfo = $$dbClassInfo;
})();

//# sourceMappingURL=Conditional.js.map?dt=1598051423207