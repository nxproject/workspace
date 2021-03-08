(function () {

    window.nx = window.nx || {};

    var $$dbClassInfo = {
        "dependsOn": {
            "qx.Class": {
                "usage": "dynamic",
                "require": true
            },
            "qx.application.Standalone": {
                "require": true
            },
            "qx.log.appender.Native": {},
            "qx.log.appender.Console": {},
            "qx.ui.form.Button": {},

            'qx.ui.core.Widget': {},
            'qx.ui.window.IDesktop': {},
            'qx.ui.window.Desktop': {}
        }
    };
    qx.Bootstrap.executePendingDefers($$dbClassInfo);

    /* ************************************************************************
    
       Copyright: 2020 undefined
    
       License: MIT license
    
       Authors: undefined
    
    ************************************************************************ */

    /**
     * This is the main application class of "app"
     *
     * @asset(app/*)
     */
    qx.Class.define("app.Application", {
        extend: qx.application.Standalone,

        /*
        *****************************************************************************
           MEMBERS
        *****************************************************************************
        */
        members: {
            /**
             * This method contains the initial application code and gets called 
             * during startup of the application
             * 
             * @lint ignoreDeprecated(alert)
             */
            main: function main() {

                // Call super class
                app.Application.prototype.main.base.call(this); // Enable logging in debug variant

                {
                    // support native logging capabilities, e.g. Firebug for Firefox
                    qx.log.appender.Native; // support additional cross-browser console. Press F7 to toggle visibility

                    qx.log.appender.Console;
                }

                /*
                -------------------------------------------------------------------------
                  Below is your actual application code...
                -------------------------------------------------------------------------
                */

                var self = this;

                nx.office.loadMany([

                    'node_modules.moment.moment',
                    'node_modules.blueimp-md5.js.md5',

                    'nx.util',
                    'nx.setup',
                    'nx.desktop'

                ], function () {

                    // Create the desktop
                    nx.desktop.init(self);

                });

            }
        }
    });
    app.Application.$$dbClassInfo = $$dbClassInfo;
})();

//# sourceMappingURL=Application.js.map?dt=1598025361265