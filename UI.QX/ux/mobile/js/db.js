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

nx.db = {

    __ds: {},
    __views: {},
    __pick: {},

    /**
     * 
     * Loads a dataset
     * 
     * @param {any} ds
     * @param {any} cb
     * @param {any} force
     */
    _loadDataset: function (ds, cb, force) {

        var self = this;

        // Fix ds name
        ds = nx.util.toDatasetName(ds);

        // Do we have it already?
        if (self.__ds[ds] && !force) {
            if (cb) cb(self.__ds[ds]);
        } else {
            // Get the view
            nx.util.serviceCall('AO.DatasetStructure', {
                ds: ds
            }, function (result) {

                //
                if (result) {

                    // Find dataset
                    var dsdef = result.ds;
                    if (dsdef) {

                        // Save
                        self.__ds[ds] = dsdef;

                        // Save the views
                        var views = result.views;
                        if (views) {

                            // Assure space
                            var defs = self.__views[ds] || {};
                            self.__views[ds] = defs;

                            // Loop thru
                            Object.keys(views).forEach(function (vname) {

                                // Get the view
                                var vdef = views[vname];
                                // Save
                                defs[vname] = vdef;

                            });
                        }

                        // Get list
                        var list = nx.util.splitSpace(dsdef.childdss);
                        // Assure
                        list.forEach(function (ds) {
                            if (ds) {
                                self._loadDataset(ds, nx.util.noOp);
                            }
                        });

                        // Get pick list
                        nx.util.serviceCall('AO.PickListGetAll', {
                            ds: ds
                        }, function (presult) {
                            self.__pick[ds] = presult.list;

                            // Now the views
                            if (cb) cb(self.__ds[ds]);
                        });
                    }
                }

            });
        }

    },

    /**
     * 
     * Loads a view
     * 
     * @param {any} ds
     * @param {any} view
     * @param {any} cb
     * @param {any} force
     */
    _loadView: function (ds, view, cb, force) {

        var self = this;

        // Fix ds name
        ds = nx.util.toDatasetName(ds);

        // Assure space
        if (!self.__views[ds]) {
            self.__views[ds] = {};
        }
        var defs = self.__views[ds];

        // Do we have it already?
        if (defs[view] && !force) {
            if (cb) cb(defs[view]);
        } else {
            // Get the view
            nx.util.serviceCall('AO.ViewGet', {
                ds: ds,
                id: view
            }, function (result) {

                // Any?
                if (result) {

                    // Save
                    defs[view] = result;
                    if (cb) cb(defs[view]);
                }
            });
        }
    },

    /**
     * 
     * Gets data from dataset
     *
     * @param {any} ds
     * @param {any} filters
     * @param {any} from
     * @param {any} to
     * @param {any} sortyby
     * @param {any} prefix
     * @param {any} cb
     */
    get: function (ds, filters, from, to, sortyby, prefix, fields, cb) {

        var self = this;

        // Fix ds name
        ds = nx.util.toDatasetName(ds);

        // Get the dataset definition
        nx.db._loadDataset(ds, function (dsdef) {

            // Call
            nx.util.serviceCall('AO.QueryGet', {
                ds: ds, // source.ds,
                query: filters,
                firstRow: from, // firstRow,
                lastRow: to, // astRow,
                sortCol: sortyby || '_desc', // sortcol,
                sortOrder: dsdef.sortOrder || 'asc',
                idprefix: prefix, // source.idprefix
                fields: fields || ''
            }, function (result) {
                // Validate
                if (result) {
                    // Modify 
                    result.data.forEach(function (row) {
                        // Does it have a description?
                        if (row._desc) {
                            row._desc = self.localizeDesc(row._desc);
                        }
                    });
                    // Apply it to the model - the method "_onRowDataLoaded" has to be called
                    cb(result.data);
                }
            });

        });
    },

    /**
     * 
     * Local store
     * 
     */
    _objs: {},

    /**
     * 
     * Creates an ID from the dataset and id
     * 
     * @param {any} ds
     * @param {any} id
     */
    makeID: function (ds, id) {

        // Fix ds name
        ds = nx.util.toDatasetName(ds);

        return '::' + ds + ':' + id + '::';
    },

    /**
     * 
     * Parses an ID into dateset and id
     * 
     * @param {any} id
     */
    parseID: function (id) {
        // Split
        var pieces = id.split(':');
        // Must have siz pieces
        if (pieces.length === 6) {
            // Return
            return {
                ds: pieces[2],
                id: pieces[3]
            }
        } else {
            return null;
        }
    },

    /**
     * 
     * Gets an object
     * 
     * @param {any} ds
     * @param {any} id
     * @param {any} cb
     */
    getObj: function (ds, id, cb) {

        var self = this;

        // If no id, create
        if (!id) {
            //
            self.createObj(ds, cb);
        } else {
            // Make the object id
            var oid = self.makeID(ds, id);
            // Do we have it?
            if (self._objs[oid]) {
                // Call
                cb(self._objs[oid]);
            } else {
                // Piggyback
                self.get(ds, [{
                    field: '_id',
                    op: 'Eq',
                    value: id
                }], 0, 1, '', '', '', function (result) {
                    // Get
                    var data = {};
                    if (result && result.length) {
                        data = result[0];
                        data._ds = ds;
                    }
                    // Assure
                    data._changes = data._changes || [];
                    if (typeof data._changes === 'string') data._changes = JSON.parse(data._changes);
                    // Save
                    self._objs[oid] = data;
                    // Call
                    cb(data);
                });
            }
        }
    },

    /**
     * 
     * Sets an object
     * 
     * @param {any} ds
     * @param {any} id
     * @param {any} cb
     * @param {any} keep
     */
    setObj: function (ds, id, cb, keep) {

        var self = this;

        var obj, goback;

        // Handle view OK
        if (!ds) {
            // Get the current object
            ds = nx.env.getBucketItem('_obj');
            goback = true;
        }

        // Do we have an object?
        if (typeof ds === 'object') {
            //
            obj = ds;
            // Get info from object
            id = obj._id;
            ds = obj._ds;
        }

        // Need the object?
        if (!obj) {
            // Make the object id
            var oid = self.makeID(ds, id);
            // Get it
            obj = self._objs[oid];
        }

        // Do we have an object?
        if (obj) {
            // Map the changes
            var changes = {};
            if (obj._changes && obj._changes.length) {
                // Loop thru
                obj._changes.forEach(function (fld) {
                    changes[fld] = obj[fld];
                });
                // Save
                nx.util.serviceCall('AO.ObjectSet', {
                    ds: ds,
                    id: id,
                    data: changes
                }, function () {
                    // Remove
                    if (!keep) {
                        delete self._objs[oid];
                    } else {
                        // Reset
                        obj._changes = [];
                    }
                    //
                    if (goback) {
                        nx.office.goBack();
                    } else if (cb) {
                        cb(true);
                    }
                });
            } else {
                if (goback) {
                    nx.office.goBack();
                } else if (cb) {
                    cb(false);
                }
            }
        } else {
            if (goback) {
                nx.office.goBack();
            } else if (cb) {
                cb(false);
            }
        }

    },

    /**
     * 
     * 
     *Removes object
     * 
     * @param {any} ds
     * @param {any} id
     * @param {any} cb
     */
    clearObj: function (ds, id, cb) {

        var self = this;

        var goback;

        // Handle view Back
        if (!ds) {
            // Get the current object
            ds = nx.env.getBucketItem('_obj');
            goback = true;
        }

        // Do we have an object?
        if (typeof ds === 'object') {
            // Get info from object
            id = ds._id;
            ds = ds._ds;
        }

        // Make the object id
        var oid = self.makeID(ds, id);
        // Do we have it?
        if (self._objs[oid]) {
            // Remove
            delete self._objs[oid];
            //
            if (goback) {
                nx.office.goBack();
            } else if (cb) {
                cb(true);
            }
        } else {
            // Call
            if (goback) {
                nx.office.goBack();
            } else if (cb) {
                cb(false);
            }
        }
    },

    /**
     * 
     * Creates an object
     * 
     * @param {any} ds
     * @param {any} cb
     */
    createObj: function (ds, cb) {

        var self = this;

        // Call
        nx.util.serviceCall('AO.ObjectCreate', {
            ds: ds
        }, function (result) {
            // Do we have an id?
            if (result._id) {
                // Save
                self._objs[self.makeID(ds, result._id)] = result;
                //
                result._ds = ds;
                // Assure
                result._changes = result._changes || [];
                if (typeof result._changes === 'string') result._changes = JSON.parse(result._changes);
                // Call
                cb(result);
            } else {
                cb(null);
            }
        });

    },

    /**
     * 
     * Returns an object
     * 
     * @param {any} ds
     * @param {any} id
     * @param {any} cb
     */
    mapObj: function (ds, id, cb) {

        var self = this;

        // Make the object id
        var oid = self.makeID(ds, id);
        // Do we have it?
        if (self._objs[oid]) {
            // Call
            cb(self._objs[oid]);
        } else {
            // None
            cb(null);
        }
    },

    /**
     * 
     * Returns true if object is in memory
     * 
     * @param {any} id
     */
    inUse: function (id) {

        var self = this;

        return !!self._objs[id];
    },

    /**
     * 
     * Sets the field value in an object,
     * 
     * @param {any} obj
     * @param {any} fld
     * @param {any} value
     */
    objSetField: function (obj, fld, value) {

        var self = this;

        var ans = false;

        // 
        if (obj) {
            // Changed?
            if (value !== obj[fld]) {
                // Save
                obj[fld] = value;
                // Assure changes
                obj._changes = obj._changes || [];
                // Only once
                if (obj._changes.indexOf(fld) === -1) {
                    obj._changes.push(fld);
                }
                ans = true;
            }
        }

        return ans;

    },

    /**
     * 
     * Localizes description (future use)
     * 
     * @param {any} desc
     */
    localizeDesc: function (desc) {
        return desc;
    },

    getPick: function (ds) {

        var self = this;

        var ans;

        if (self.__pick && self.__pick[ds]) {
            ans = self.__pick[ds];
            if (ans && !Object.keys(ans).length) ans = null;
        }

        return ans;
    },

    processPickToolbarItem: function (passed) {

        var self = this;

        var ans;
        var queries = [];
        var i = 1;

        // Only if selected
        if (passed && passed.selected === 'y') {
            // Loop thru
            while (nx.util.hasValue(passed['field' + i])) {
                queries.push({
                    field: passed['field' + i],
                    op: passed['op' + i],
                    value: passed['value' + i]
                });
                i++;
            }

            // Any?
            if (queries.length) {
                // All or any?
                ans = {
                    sop: passed.AllAny,
                    queries: queries
                };
            }
        }

        return ans;

    },

};
