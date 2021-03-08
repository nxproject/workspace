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

qx.Class.define('tools.Chat', {

    type: 'static',

    statics: {

        caption: 'Chat',
        icon: 'user_comment',
        startindex: 'hidden',

        // This is what you override
        do: function (req) {

            // Assure id
            var nxid = 'chat_' + req.win;
            var winid = req.win;

            // The top toolbar
            var tb = [];

            if (!nx.desktop.user.getIsAccount()) {
                tb.push({
                    label: 'Invite',
                    icon: 'user_comment',
                    choices: [{
                        label: 'Via link',
                        icon: 'comments',
                        click: function (e) {
                            nx.util.runTool('Input', {
                                label: 'Name',
                                atOk: function (value) {
                                    if (value) {
                                        nx.util.serviceCall('AO.IEntrySet', {
                                            type: 'chat',
                                            limit: 15,
                                            uses: 1,
                                            value: {
                                                name: value,
                                                tool: 'Chat',
                                                toolparams: {
                                                    win: req.win,
                                                    desc: req.desc
                                                }
                                            }
                                        }, function (result) {
                                            if (result && result.id) {
                                                var url = nx.util.loopbackURL() + '?id=' + result.id;
                                                nx.util.copy(url);
                                                nx.util.notifyInfo('Link copied to clipboard - ' + result.id);
                                            }
                                        });
                                    }
                                }
                            });
                        }
                    }]
                });
            }

            if (!req.caller) {
                tb.push('>');
                tb.push({
                    label: 'View',
                    icon: 'application',
                    click: function (e) {
                        var caller = nx.util.eventGetWindow(e);
                        if (nx.util.startsWith(winid, 'ao_')) {
                            var pieces = winid.split('_');
                            // View
                            nx.desktop.addWindowDS({
                                ds: pieces[1],
                                id: pieces[2],
                                view: nx.desktop.user.getDSInfo(pieces[1]).view,
                                caller: caller
                            });
                        } else if (nx.util.startsWith(winid, 'doc_')) {
                            nx.fs.editDOCX({
                                path: winid.substr(4),
                                adjustWidth: '-default.pickAdjust@1',
                                noCenter: true,
                                caller: caller,
                                noChat: true
                            });
                        } else if (nx.util.startsWith(winid, 'pdf_')) {
                            nx.fs.viewPDF({
                                path: winid.substr(4),
                                adjustWidth: '-default.pickAdjust@1',
                                noCenter: true,
                                caller: caller,
                                noChat: true
                            });
                        } else if (nx.util.startsWith(winid, 'img_')) {
                            nx.fs.viewImage({
                                path: winid.substr(4),
                                adjustWidth: '-default.pickAdjust@1',
                                noCenter: true,
                                caller: caller,
                                noChat: true
                            });
                        } else if (nx.util.startsWith(winid, 'video_')) {
                            nx.fs.viewVideo({
                                path: winid.substr(4),
                                adjustWidth: '-default.pickAdjust@1',
                                noCenter: true,
                                caller: caller,
                                noChat: true
                            });
                        } else if (nx.util.startsWith(winid, 'fc_')) {
                            var ds = winid.substr(3);
                            nx.desktop._loadDataset(ds, function (dsdef) {
                                nx.fs.calendar({
                                    ds: ds,
                                    desc: dsdef.caption,
                                    caller: caller,
                                    adjustWidth: '-default.pickAdjust@1',
                                    noCenter: true,
                                    noChat: true
                                });
                            });
                        }
                    }
                });
            }


            var win = nx.desktop.addWindow({

                nxid: nxid,

                caption: 'Chat ' + req.desc,
                icon: 'user_comment',
                caller: req.caller,
                adjust: req.adjust,

                defaultCommand: 'Send',

                items: [

                    {
                        nxtype: 'grid',
                        top: 1,
                        left: 1,
                        width: nx.default.get('default.pickWidth'),
                        height: nx.default.get('default.pickHeight@0.75'),
                        label: '',
                        columns: [
                            {
                                label: 'At',
                                aoFld: 'at',
                                width: 7
                            }, {
                                label: 'By',
                                aoFld: 'by',
                                width: 7
                            }, {
                                label: 'Message',
                                aoFld: 'msg',
                                autoresize: true
                            }
                        ],
                        data: [],
                        isPick: false,
                        aoFld: 'grid',
                        cookie: 'chat'
                    }, {
                        nxtype: 'string',
                        left: 1,
                        width: nx.default.get('default.pickWidth'),
                        label: '->'
                    }
                ],

                listeners: {

                    beforeClose: function (e) {

                        var self = this;

                        // Send
                        nx.desktop.user.SIOSend('$$chat.leave', {
                            chat: nxid
                        });

                    },

                    appear: function (e) {

                        var self = this;

                        // Send
                        if (!req.response) {
                            nx.desktop.user.SIOSend('$$chat.open', {
                                chat: nxid
                            });
                        }

                        // Send
                        nx.desktop.user.SIOSend('$$chat.join', {
                            chat: nxid
                        });

                        // Add current users
                        nx.desktop.user.SIOUsers.forEach(function (user) {
                            self.addUser(user);
                        });
                    },
                },

                topToolbar: {

                    items: tb

                },

                commands: {

                    items: [

                        '>', {

                            label: 'Send',
                            icon: 'accept',
                            click: function (e) {

                                var self = this;

                                // Map window
                                var win = nx.bucket.getWin(self);

                                // Get the message
                                var msg = win.getValue('->');
                                // Any?
                                if (msg) {

                                    // Send
                                    nx.desktop.user.SIOSend('$$chat.message', {
                                        text: msg,
                                        chat: nxid
                                    }, {
                                        allow: true
                                    });

                                    // Reset
                                    win.setValue('->', '');
                                }
                            }
                        }

                    ]
                },

                fns: {

                    addMessage: function (msg, user) {

                        var self = this;

                        var row = {
                            at: new Date().toTimeString().substr(0, 8),
                            by: user || '',
                            msg: msg
                        };


                        if (user) {
                            self.colors = self.colors || {};
                            if (!self.colors[user]) {
                                var pos = Object.keys(self.colors).length % nx.setup.colorPastel.length;
                                self.colors[user] = nx.setup.colorPastel[pos];
                            }
                            row._color = self.colors[user];
                        } else {
                            row._color = '#DCDCDC';
                        }

                        var grid = win.getField('grid');
                        nx.bucket.getParams(grid).data.unshift(row);
                        grid.refresh();

                    },

                    addUser: function (user) {

                        var self = this;

                        // Get the button
                        var btn = self.getButtonWithLabel(self.topToolbar, 'Invite');
                        if (btn) {
                            // Get users list
                            var menu = btn.getMenu();
                            // Look for entry
                            if (!menu.findEntry(user)) {
                                menu.addEntry(user, function (e) {
                                    var widget = nx.util.eventGetWidget(e);
                                    var user = widget.getLabel();

                                    // Send
                                    nx.desktop.user.SIOSend('$$chat.invite', {
                                        to: user,
                                        desc: req.desc,
                                        chat: nxid
                                    });
                                });
                            }
                        }

                    },

                    removeUser: function (user) {

                        var self = this;

                        // Get the button
                        var btn = self.getButtonWithLabel(self.topToolbar, 'Invite');
                        if (btn) {
                            // Get users list
                            var menu = btn.getMenu();
                            // Remove
                            menu.removeEntry(user);
                        }

                    },

                },

                processSIO: function (win, event) {

                    // According to type
                    switch (event.fn) {

                        case '$$chat.join':
                            if (event.message.chat === nxid) {
                                win.addMessage(event.user + ' has joined the chat');
                            }
                            break;

                        case '$$chat.leave':
                            if (event.message.chat === nxid) {
                                win.addMessage(event.user + ' has left the chat');
                            }
                            break;

                        case '$$chat.message':
                            if (event.message.chat === nxid) {
                                win.addMessage(event.message.text, event.user);
                            }
                            break;

                        case '$$user.added':
                            win.addUser(event.user);
                            break;

                        case '$$user.removed':
                            win.removeUser(event.user);
                            break;
                    }
                }
            });

            return win;
        }

    }

});