(function () {
  var $$dbClassInfo = {
    "dependsOn": {
      "qx.Theme": {
        "usage": "dynamic",
        "require": true
      },
      "app.theme.Color": {
        "require": true
      },
      "app.theme.Decoration": {
        "require": true
      },
      "app.theme.Font": {
        "require": true
      },
      "qx.theme.icon.Tango": {
        "require": true
      },
      "app.theme.Appearance": {
        "require": true
      }
    }
  };
  qx.Bootstrap.executePendingDefers($$dbClassInfo);

  /* ************************************************************************
  
     Copyright: 2020 undefined
  
     License: MIT license
  
     Authors: undefined
  
  ************************************************************************ */
  qx.Theme.define("app.theme.Theme", {
    meta: {
      color: app.theme.Color,
      decoration: app.theme.Decoration,
      font: app.theme.Font,
      icon: qx.theme.icon.Tango,
      appearance: app.theme.Appearance
    }
  });
  app.theme.Theme.$$dbClassInfo = $$dbClassInfo;
})();

//# sourceMappingURL=Theme.js.map?dt=1598025361307