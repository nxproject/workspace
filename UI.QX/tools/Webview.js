/* ************************************************************************

   UI.QX - a dynamic web interface

   http://qooxdoo.org

   Copyright:
     2020-2021 Jose E. Gonzalez (nxoffice2021@gmail.com)

   License:
     MIT: https://opensource.org/licenses/MIT
     See the LICENSE file in the project's top-level directory for details.

   Authors:
     * Jose E. Gonzalez

	@require(qx.core.Object)

************************************************************************ */

qx.Class.define('tools.Webview', {

    type: 'static',

    statics: {

        startindex: 'hidden',

        // This is what you override
        do: function (req) {

            // Assure
            req = req || {};

            // 
            if (req.chat) {
                req.topToolbar = req.topToolbar || {};
                req.topToolbar.items = req.topToolbar.items || [];
                req.topToolbar.items.unshift({
                    label: 'Chat',
                    icon: 'user_comment',
                    click: function (e) {
                        var widget = nx.util.eventGetWidget(e);
                        var win = nx.bucket.getWin(widget);
                        var winid = nx.bucket.getItem(win, 'winid');
                        //
                        nx.util.runTool('Chat', {
                            desc: win.getCaption(),
                            win: winid,
                            caller: win
                        });
                    }
                });
            }

            var winid = req.nxid || nx.util.uuid();

            var commands = req.commands || {

                items: [ 'X' ]
            };

            var wdef = {

                nxid: winid,
                caption: req.caption || 'View',
                icon: req.icon,
                allowClose: false,
                caller: req.caller,

                topToolbar: req.topToolbar,
                bottomToolbar: req.bottomToolbar,

                items: [

                    {
                        nxtype: 'webview',
                        top: 1,
                        left: 1,
                        width: 'default.screenWidth',
                        height: 'default.screenHeight',
                        adjustWidth: req.adjustWidth,
                        label: req.label,
                        value: req.value,
                        aoFld: 'view'
                    }
                ],

                commands: commands,

                fns: {

                    adjustWidth: function (adj) {

                        var self = this;

                        var bounds = nx.util.getAbsoluteBounds(self);
                        self.setWidth(bounds.width + adj);
                        var view = win.getField('view');
                        bounds = nx.util.getAbsoluteBounds(view);
                        view.setWidth(bounds.width + adj);

                    }

                }

            };

            if (!req.noCenter) {
                wdef.center = 'both';
            }

            var win = nx.desktop.addWindow(wdef);
        }

    }

});