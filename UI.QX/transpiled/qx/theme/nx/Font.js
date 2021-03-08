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
      "qx.bom.client.OperatingSystem": {
        "require": true
      }
    },
    "environment": {
      "provided": [],
      "required": {
        "os.name": {
          "load": true,
          "className": "qx.bom.client.OperatingSystem"
        },
        "os.version": {
          "load": true,
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
  
  ************************************************************************* */

	var largeFont = 16;
	var medFont = 14;
	var smallFont = 12;

  /**
   * The nx font theme.
   */
  qx.Theme.define("qx.theme.nx.Font", {
    fonts: {
      "default": {
        size: qx.core.Environment.get("os.name") == "win" && (qx.core.Environment.get("os.version") == "7" || qx.core.Environment.get("os.version") == "vista") ? largeFont : medFont,
        lineHeight: 1.4,
        family: qx.core.Environment.get("os.name") == "osx" ? ["Lucida Grande"] : qx.core.Environment.get("os.name") == "win" && (qx.core.Environment.get("os.version") == "7" || qx.core.Environment.get("os.version") == "vista") ? ["Segoe UI", "Candara"] : ["Liberation Sans", "Tahoma", "Arial", "sans-serif"]
      },
      "bold": {
        size: qx.core.Environment.get("os.name") == "win" && (qx.core.Environment.get("os.version") == "7" || qx.core.Environment.get("os.version") == "vista") ? largeFont : medFont,
        lineHeight: 1.4,
        family: qx.core.Environment.get("os.name") == "osx" ? ["Lucida Grande"] : qx.core.Environment.get("os.name") == "win" && (qx.core.Environment.get("os.version") == "7" || qx.core.Environment.get("os.version") == "vista") ? ["Segoe UI", "Candara"] : ["Liberation Sans", "Tahoma", "Arial", "sans-serif"],
        bold: true
      },
      "small": {
        size: qx.core.Environment.get("os.name") == "win" && (qx.core.Environment.get("os.version") == "7" || qx.core.Environment.get("os.version") == "vista") ? medFont : smallFont,
        lineHeight: 1.4,
        family: qx.core.Environment.get("os.name") == "osx" ? ["Lucida Grande"] : qx.core.Environment.get("os.name") == "win" && (qx.core.Environment.get("os.version") == "7" || qx.core.Environment.get("os.version") == "vista") ? ["Segoe UI", "Candara"] : ["Liberation Sans", "Tahoma", "Arial", "sans-serif"]
      },
      "monospace": {
        size: medFont,
        lineHeight: 1.4,
        family: qx.core.Environment.get("os.name") == "osx" ? ["Lucida Console", "Monaco"] : qx.core.Environment.get("os.name") == "win" && (qx.core.Environment.get("os.version") == "7" || qx.core.Environment.get("os.version") == "vista") ? ["Consolas"] : ["Consolas", "DejaVu Sans Mono", "Courier New", "monospace"]
      }
    }
  });
  qx.theme.nx.Font.$$dbClassInfo = $$dbClassInfo;
})();
//# sourceMappingURL=package-9.js.map?dt=1598213499061