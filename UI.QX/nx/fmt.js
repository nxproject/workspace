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

************************************************************************ */

nx.fmt = {


    lu: function (value, opts, cb) {
        // Do we have one?
        if (MyHelper.hasVALUE(value)) {
            MyDSDefs.getDefinition({}, opts.ds, function (dsdef) {

                var req = {
                    select: value,
                    objds: opts.ds,
                    ds: opts.caller,
                    pickType: 'dschtype',
                    keepOpen: dsdef.kschtype
                };
                //
                var icon = dsdef.icon;

                var link = opts.ds;
                // 
                MyGenerator.pickDS(req, function (selection) {
                    var newreq = {
                        ds: link,
                        id: selection.data.id
                    };
                    MyRPC.serviceCall('Object_Get', newreq, function (result) {
                        var values = [];

                        values.push(opts.field);
                        values.push(MyHelper.localizeDESC(result));

                        var suppflds = MyHelper.splitSpace(opts.suppflds);
                        for (var i = 0; i < suppflds.length; i += 2) {
                            values.push(suppflds[i]);
                            values.push(result[suppflds[i + 1]] || '');
                        }

                        cb(values);
                    });
                }, function (selection) {

                        var caller = selection.caller || req.caller;
                    //if (req.keepOpen) caller = selection.caller;
                    req.shared = selection.shared;

                    MyHelper.nextID(function (id) {
                        MyRPC.serviceCall('Object_New', { ds: req.ds }, function (obj) {
                            MyHelper.localizeDT(obj);
                            MyGenerator.showFORM({
                                ds: req.ds,
                                id: id,
                                obj: obj,
                                objdesc: MyResources.New,
                                menuIcon: 'eci' + icon,
                                caller: caller
                            }, function (ds, id, obj) {
                                var values = [];

                                values.push(opts.field);
                                values.push(MyHelper.localizeDESC(result));

                                var suppflds = MyHelper.splitSpace(opts.suppflds);
                                for (var i = 0; i < suppflds.length; i += 2) {
                                    values.push(suppflds[i]);
                                    values.push(result[suppflds[i + 1]] || '');
                                }

                                cb(values);
                            });
                        });
                    });

                });
            });
        }
        // Always return the original to skip processing
        cb(value);
    },

    link: function (value, opts, cb) {
        MyDSDefs.getDefinition({}, opts.ds, function (dsdef) {
            //
            var req = {
                select: value,
                ds: opts.ds,
                caller: opts.caller,
                pickType: 'dschtype',
                keepOpen: dsdef.kschtype
            };
            //
            var icon = dsdef.icon;
            // 
            MyGenerator.pickDS(req, function (selection) {
                cb(MyHelper.localizeDESC(selection.data));
            }, function (selection) {

                var caller = req.caller;
                if (req.keepOpen) caller = selection.caller;
                req.shared = selection.shared;

                MyHelper.nextID(function (id) {
                    MyRPC.serviceCall('Object_New', { ds: req.ds }, function (obj) {
                        MyHelper.localizeDT(obj);
                        MyGenerator.showFORM({
                            ds: req.ds,
                            id: id,
                            obj: obj,
                            objdesc: MyResources.New,
                            menuIcon: 'eci' + icon,
                            caller: caller
                        }, function (ds, id, ob) {
                            cb(MyHelper.localizeDESC(obj));
                        });
                    });
                });

            });
        });
    },


};