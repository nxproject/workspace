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

qx.Class.define('c.combobox', {

    extend: c._component,

    implement: i.iComponent,

    construct: function () {

        // Call base
        this.base(arguments, new c._selectbox());

    },

    members: {

        setChoices: function (choices, allowempty) {

            var self = this;

            // Make the list
            var req = {
                choices: choices,
                allowEmpty: !!allowempty
            };

            // Loop thru
            nx.bucket.getWidgets(self).forEach(function (w) {
                nx.setup.choices(w, req);
            });

        }
    },

    statics: {

        makeSelf: function (req) {

            return new c.combobox(req);

        }
    }

});