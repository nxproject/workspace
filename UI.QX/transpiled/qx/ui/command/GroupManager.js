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
      "qx.ui.command.Group": {}
    }
  };
  qx.Bootstrap.executePendingDefers($$dbClassInfo);

  /* ************************************************************************
  
     Copyright:
       2012 1&1 Internet AG, Germany, http://www.1und1.de
  
     Authors:
       * Mustafa Sak (msak)
  
  
  ************************************************************************ */

  /**
   * Registrar for command groups to be able to active or deactive them.
   */
  qx.Class.define("qx.ui.command.GroupManager", {
    extend: qx.core.Object,
    construct: function construct() {
      qx.core.Object.constructor.call(this);
      this.__groups__P_185_0 = [];
    },
    members: {
      __groups__P_185_0: null,
      __activeGroup__P_185_1: null,

      /**
       * Add command group.
       *
       * @param group {qx.ui.command.Group} Command group
       *
       * @return {Boolean} <code>false</code> if group was already added before
       */
      add: function add(group) {
        {
          this.assertInstance(group, qx.ui.command.Group, "Given group is not an instance of qx.ui.command.Group");
        }

        if (this.__groups__P_185_0.includes(group)) {
          {
            this.debug("Group is already added!");
          }
          return false;
        }

        this.__groups__P_185_0.push(group); // deactivate added group to prevent collusions


        group.setActive(false);
        return true;
      },

      /**
       * Whether a command manager was added.
       *
       * @param group {qx.ui.command.Group} Command group
       *
       * @return {Boolean} <code>true</code> if group already added
       */
      has: function has(group) {
        {
          this.assertInstance(group, qx.ui.command.Group, "Given group is not an instance of qx.ui.command.Group");
        }
        return !!this._getGroup(group);
      },

      /**
       * Removes a command group from group manager. If removed group was the
       * active group, active group will be set to <code>null</code> Returns the
       * group.
       *
       * @param group {qx.ui.command.Group} Command group
       *
       * @return {qx.ui.command.Group | null} Command group or null if group was not added before
       */
      remove: function remove(group) {
        {
          this.assertInstance(group, qx.ui.command.Group, "Group must be an instance of qx.ui.command.Group");
        }

        var index = this.__groups__P_185_0.indexOf(group);

        if (index === -1) {
          {
            this.debug("Group was not added before. Please use 'add()' method to add the group.");
          }
        } // reset active group


        if (this.getActive() === group) {
          this.__activeGroup__P_185_1 = null;
        } // remove group from internal array


        this.__groups__P_185_0.splice(index, 1);

        return group;
      },

      /**
       * Activates a command group and deactivates all other added groups.
       *
       * @param group {qx.ui.command.Group} Command group
       *
       * @return {Boolean} <code>false</code> if group was not added before
       */
      setActive: function setActive(group) {
        {
          this.assertInstance(group, qx.ui.command.Group, "Given group is not an instance of qx.ui.command.Group");
        }

        if (!this.has(group)) {
          {
            this.debug("Group was not added before! You have to use 'addCommand()' method before activating!");
          }
          return false;
        } // iterate through all groups and deactivate all expect the given one


        for (var i = 0; i < this.__groups__P_185_0.length; i++) {
          var item = this.__groups__P_185_0[i];

          if (item == group) {
            item.setActive(true);
            this.__activeGroup__P_185_1 = item;
            continue;
          }

          item.setActive(false);
        }

        return true;
      },

      /**
       * Returns active command group.
       *
       * @return {qx.ui.command.Group | null} Active command group
       */
      getActive: function getActive() {
        return this.__activeGroup__P_185_1;
      },

      /**
       * Blocks the active command group.
       */
      block: function block() {
        if (this.__activeGroup__P_185_1) {
          this.__activeGroup__P_185_1.setActive(false);
        }
      },

      /**
       * Unblocks the active command group.
       */
      unblock: function unblock() {
        if (this.__activeGroup__P_185_1) {
          this.__activeGroup__P_185_1.setActive(true);
        }
      },

      /**
       * Helper function returns added command group.
       *
       * @param group {qx.ui.command.Group} Command group
       *
       * @return {qx.ui.command.Group | null} Command group or null
       */
      _getGroup: function _getGroup(group) {
        var index = this.__groups__P_185_0.indexOf(group);

        if (index === -1) {
          return null;
        }

        return this.__groups__P_185_0[index];
      }
    },
    destruct: function destruct() {
      this.__groups__P_185_0 = this.__activeGroup__P_185_1 = null;
    }
  });
  qx.ui.command.GroupManager.$$dbClassInfo = $$dbClassInfo;
})();

//# sourceMappingURL=GroupManager.js.map?dt=1598051399541