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

var queryString = window.location.search;
var urlParams = new URLSearchParams(queryString);
window.nx = {
    file: urlParams.get('file'),
    winid: urlParams.get('winid'),
    user: urlParams.get('user'),
};