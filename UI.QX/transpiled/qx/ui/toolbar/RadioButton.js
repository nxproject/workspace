(function () {
  var $$dbClassInfo = {
    "dependsOn": {
      "qx.Class": {
        "usage": "dynamic",
        "require": true
      },
      "qx.ui.toolbar.CheckBox": {
        "require": true
      },
      "qx.ui.form.MModelProperty": {
        "require": true
      },
      "qx.ui.form.IModel": {
        "require": true
      },
      "qx.ui.form.IRadioItem": {
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
       * Andreas Ecker (ecker)
       * Martin Wittemann (martinwittemann)
  
  ************************************************************************ */

  /**
   * Radio buttons are used to manage a single selection. Radio buttons only
   * make sense used in a group of two or more of them. They are managed (connected)
   * to a {@link qx.ui.form.RadioGroup} to handle the selection.
   */
  qx.Class.define("qx.ui.toolbar.RadioButton", {
    extend: qx.ui.toolbar.CheckBox,
    include: [qx.ui.form.MModelProperty],
    implement: [qx.ui.form.IModel, qx.ui.form.IRadioItem],

    /*
    *****************************************************************************
       MEMBERS
    *****************************************************************************
    */
    members: {
      /*
      ---------------------------------------------------------------------------
        PROPERTY APPLY ROUTINES
      ---------------------------------------------------------------------------
      */
      // overridden
      _applyValue: function _applyValue(value, old) {
        qx.ui.toolbar.RadioButton.prototype._applyValue.base.call(this, value, old);

        if (value) {
          var grp = this.getGroup();

          if (grp) {
            grp.setSelection([this]);
          }
        }
      },
      // overridden
      _onExecute: function _onExecute(e) {
        var grp = this.getGroup();

        if (grp && grp.getAllowEmptySelection()) {
          this.toggleValue();
        } else {
          this.setValue(true);
        }
      }
    }
  });
  qx.ui.toolbar.RadioButton.$$dbClassInfo = $$dbClassInfo;
})();

//# sourceMappingURL=RadioButton.js.map?dt=1598051407427