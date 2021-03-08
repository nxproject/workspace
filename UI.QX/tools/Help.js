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

qx.Class.define('tools.Help', {

    type: 'static',

    statics: {

        startindex: '9999',
        startprivilege: 'HELP',
        caption: 'Help',
        icon: 'help',

        // This is what you override
        do: function (req) {

            // Start with basic
            var url = '/help';
            // Do we have an override
            var helproot = nx.desktop.user.getSIField('helproot');
            if (nx.util.hasValue(helproot)) url += '/' + helproot + '.md';

            window.open(nx.util.loopbackURL() + url, '_blank');

        }

    }

});