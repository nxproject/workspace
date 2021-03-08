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

nx.web = {

    urlStart: function (key, value) {

        var self = this;

        return self.urlAdd(key, value);
    },

    urlAdd: function (key, value, prev) {

        var self = this;

        // Set delim
        var delim = (prev ? '&' : '?');

        // 
        return (prev || '') + delim + key + '=' + encodeURIComponent(value);

    },

    _lookupVIN: function (widget) {

        var self = this;

        nx.cm.map(widget, 'relvinnum', function (map) {

            if (map.relvinnum) {

                var url = 'https://vpic.nhtsa.dot.gov/api/vehicles/decodevinvalues/' + map.relvinnum + '?format=json';

                $.get(url, {}, function (nhtsa) {

                    if (nhtsa && nhtsa.Results && nhtsa.Results.length) {

                        var info = nhtsa.Results[0];

                        // Fill
                        nx.cm.set(map, 'relvinyear', info.ModelYear);
                        nx.cm.set(map, 'relvinmake', info.Make);
                        nx.cm.set(map, 'relvinmodel', info.Model);
                        nx.cm.set(map, 'relvinseries', info.Series);
                        nx.cm.set(map, 'relvintype', info.VehicleType);
                        nx.cm.set(map, 'relvintrim', info.Trim);
                        nx.cm.set(map, 'relvindoors', info.Doors);
                        nx.cm.set(map, 'relvincyl', info.EngineCylinders);
                        nx.cm.set(map, 'relvindisp', info.DisplacementL);
                        nx.cm.set(map, 'relvinfuel', info.FuelTypePrimary);
                        nx.cm.set(map, 'relvinspeed', info.TransmissionSpeed);
                        nx.cm.set(map, 'relvintrans', info.TransmissionStyle);

                    }

                }, 'json');
            }
        });
    },

    _selectVideoFeed: function (videoInputDevices) {

        var self = this;

        // Assume none
        var selectedDeviceId, backDeviceId, selectedDeviceLabel, backDeviceLabel;

        //Loop thru
        videoInputDevices.forEach(function (device) {
            // Get the label
            var label = device.label.toLowerCase();
            // Camera?
            if (label.match(/camera/) || label.match(/webcam/)) {
                selectedDeviceId = device.deviceId;
                selectedDeviceLabel = device.label;
                if (label.match(/back/)) {
                    backDeviceId = device.deviceId;
                    backDeviceLabel = device.label;
                }
            }
        });

        if (backDeviceId) {
            selectedDeviceId = backDeviceId;
            selectedDeviceLabel = backDeviceLabel;
        }

        if (selectedDeviceLabel) {
            nx.util.notifyOK('Using ' + selectedDeviceLabel);
        }

        return selectedDeviceId;
    },

    _scanForBarcode: function (widget, cb, fmt) {

        var self = this;

        // Code
        var codeReader;

        // DO the popup
        nx.util.popup('<video id="reader"></video>', function () {

            // To handle rotation
            const hints = new Map();
            hints.set(3, true); // ZXing.DecodeHintType.TRY_HARDER = 3

            // Use the widest one
            if (fmt) {
                codeReader = new ZXing[fmt](hints);
            } else {
                codeReader = new ZXing.BrowserMultiFormatReader(hints);
            }

            // Get the devices
            codeReader.listVideoInputDevices()
                .then((videoInputDevices) => {

                    // 
                    var selectedDeviceId = self._selectVideoFeed(videoInputDevices);

                    // Any?
                    if (selectedDeviceId) {

                        // 
                        codeReader.decodeFromVideoDevice(selectedDeviceId, 'reader', (result, err) => {

                            // Did we get a result?
                            if (result) {

                                // Pass to callback
                                if (cb) cb(result.text);

                                // Close
                                nx.util.popupClose();
                            }
                        });
                    }
                });

        }, function() {
        //
        if (codeReader) {
            codeReader.reset();
        }
    });
}

}