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

qx.Class.define('tools.Pick', {

    type: 'static',

    statics: {

        startindex: 'hidden',

        // This is what you override
        do: function (req) {

            // Get dataset
            nx.desktop._loadDataset(req.ds, function (dsdef) {

                var viewdef = {

                    caption: 'Select ' + dsdef.caption,
                    defaultCommand: 'Ok',

                    items: [

                        {
                            nxtype: 'string',
                            top: 1,
                            left: 1,
                            width: 'default.fieldWidth',
                            label: 'Search for'
                        }, {
                            nxtype: 'grid',
                            top: 2,
                            left: 1,
                            width: 'default.pickWidth',
                            height: 'default.pickHeight',
                            label: '',
                            ds: req.ds,
                            dsdef: dsdef,
                            mode: 'edit',
                            columns: [{
                                label: 'Description',
                                aoFld: '_desc'
                            }],
                            listeners: {
                                selected: function (e) {
                                    // Get the widget
                                    var widget = nx.util.eventGetWidget(e);
                                    // And the parameters
                                    var params = nx.bucket.getParams(widget);
                                    // Get the data
                                    var data = nx.util.eventGetData(e);

                                    // Open all selected
                                    data.forEach(function (row) {
                                        nx.desktop.addWindowDS({
                                            ds: req.ds,
                                            id: row._id,
                                            view: req.view,
                                            sysmode: req.sysmode,
                                            caller: nx.util.eventGetWindow(e)
                                        });
                                    });
                                }
                            }
                        }
                    ]

                };

                nx.desktop.addWindow(viewdef);
            });
        }

    }

});