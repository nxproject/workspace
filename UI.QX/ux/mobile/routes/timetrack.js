/* ************************************************************************

   Framework7 - a dynamic web interface

   https://framework7.io/

   Copyright:
     2020-2021 Jose E. Gonzalez (nxoffice2021@gmail.com)

   License:
     MIT: https://opensource.org/licenses/MIT
     See the LICENSE file in the project's top-level directory for details.

   Authors:
     * Jose E. Gonzalez

************************************************************************ */

// Set the call
nx.calls.ttstart = function (req) {
    // Get the object
    var obj = nx.env.getBucketItem('_obj');

    //
    nx.tt.tagWidget(obj, 'start');

    nx.office.panelRightClose();
};

// Set the call
nx.calls.ttstartfrozen = function (req) {
    // Get the object
    var obj = nx.env.getBucketItem('_obj');

    //
    nx.tt.tagWidget(obj, 'startf');

    nx.office.panelRightClose();
};

// Set the call
nx.calls.ttfreeze = function (req) {
    // Get the object
    var obj = nx.env.getBucketItem('_obj');

    //
    nx.tt.tagWidget(obj, 'freeze');

    nx.office.panelRightClose();
};

// Set the call
nx.calls.ttcontinue = function (req) {
    // Get the object
    var obj = nx.env.getBucketItem('_obj');

    //
    nx.tt.tagWidget(obj, 'continue');

    nx.office.panelRightClose();
};

// Set the call
nx.calls.ttend = function (req) {
    // Get the object
    var obj = nx.env.getBucketItem('_obj');

    //
    nx.tt.tagWidget(obj, 'end');

    nx.office.panelRightClose();
};

// Set the call
nx.calls.ttshow = function (req) {
    // Get the object
    var obj = nx.env.getBucketItem('_obj');

    //
    nx.tt.tagWidget(obj, 'show', function (result) {
        nx.util.notifyDisplay('<p>' + result.value.replace(/\n/g, '<br/>') + '</p>');
    });


    nx.office.panelRightClose();
};