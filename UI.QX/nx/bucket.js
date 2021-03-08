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

nx.bucket = {

    get: function (widget) {

        return widget._nxbucket;
    },

    getItem: function (widget, key) {

        var self = this;

        var ans;

        // Get the bucket
        var bucket = self.get(widget);
        // Any?
        if (bucket) {
            // Get the item
            ans = bucket[key];
        }
        return ans;
    },

    setItem: function (widget, key, value) {

        var self = this;

        var ans;

        // Get the bucket
        var bucket = self.get(widget);
        // Any?
        if (!bucket) {
            // Make one
            widget._nxbucket = bucket = new qx.core.Object();
        }
        // Save
        bucket[key] = value;
    },

    deleteItem: function (widget, key) {

        var self = this;

        var ans;

        // Get the bucket
        var bucket = self.get(widget);
        // Any?
        if (bucket) {
            // Bye
            delete bucket[key];
        }
        return ans;
    },

    getBucketHolder: function (widget) {
        //
        while (widget) {
            if (widget._nxbucket) break;
            widget = widget.$$parent;
        }
        return widget;
    },

    getWidget: function (bucket) {

        return this.getItem(bucket, 'widget');

    },

    setWidget: function (bucket, widget) {

        this.setItem(bucket, 'widget', widget);

    },

    getWidgets: function (bucket) {

        return this.getItem(bucket, 'widgets');

    },

    setWidgets: function (bucket, widget) {

        this.setItem(bucket, 'widgets', widget);

    },

    getContainer: function (bucket) {

        return this.getItem(bucket, 'container');

    },

    setContainer: function (bucket, widget) {

        this.setItem(bucket, 'container', widget);

    },

    getLabel: function (bucket) {

        return this.getItem(bucket, 'label');

    },

    setLabel: function (bucket, widget) {

        this.setItem(bucket, 'label', widget);

    },

    getParent: function (bucket) {

        return this.getItem(bucket, 'parent');

    },

    setParent: function (bucket, widget) {

        this.setItem(bucket, 'parent', widget);

    },

    getChildren: function (bucket) {

        return this.getItem(bucket, 'children');

    },

    setChildren: function (bucket, widget) {

        this.setItem(bucket, 'children', widget);

    },

    getParams: function (bucket) {

        return this.getItem(bucket, 'params');

    },

    setParams: function (bucket, widget) {

        this.setItem(bucket, 'params', widget);

    },

    getForm: function (bucket) {

        return this.getItem(bucket, 'form');

    },

    setForm: function (bucket, widget) {

        this.setItem(bucket, 'form', widget);

    },

    getDataset: function (bucket) {

        return this.getItem(bucket, 'ds');

    },

    setDataset: function (bucket, widget) {

        this.setItem(bucket, 'ds', widget);

    },

    getView: function (bucket) {

        return this.getItem(bucket, 'view');

    },

    setView: function (bucket, widget) {

        this.setItem(bucket, 'view', widget);

    },

    getFields: function (bucket) {

        return this.getItem(bucket, 'fields');

    },

    setFields: function (bucket, widget) {

        this.setItem(bucket, 'fields', widget);

    },

    getChanges: function (bucket) {

        return this.getItem(bucket, 'changes');

    },

    setChanges: function (bucket, widget) {

        this.setItem(bucket, 'changes', widget);

    },

    getWin: function (bucket) {

        return this.getItem(bucket, 'win');

    },

    setWin: function (bucket, widget) {

        this.setItem(bucket, 'win', widget);

    },

    getPassed: function (bucket) {

        return this.getItem(bucket, 'passed');

    },

    setPassed: function (bucket, widget) {

        this.setItem(bucket, 'passed', widget);

    },

    getOptions: function (bucket) {

        return this.getItem(bucket, 'options');

    },

    setOptions: function (bucket, widget) {

        this.setItem(bucket, 'options', widget);

    },

    getWindowID: function (bucket) {

        return this.getItem(bucket, 'winid');

    },

    setWindowID: function (bucket, widget) {

        this.setItem(bucket, 'winid', widget);

    },

    getTools: function (bucket) {

        return this.getItem(bucket, 'tools');

    },

    setTools: function (bucket, widget) {

        this.setItem(bucket, 'tools', widget);

    },

    getFormatters: function (bucket) {

        return this.getItem(bucket, 'formatters');

    },

    setFormatters: function (bucket, widget) {

        this.setItem(bucket, 'formatters', widget);

    },

    getTable: function (bucket) {

        return this.getItem(bucket, 'table');

    },

    setTable: function (bucket, widget) {

        this.setItem(bucket, 'table', widget);

    },

    getInFilter: function (bucket) {

        return this.getItem(bucket, 'infilter');

    },

    setInFilter: function (bucket, widget) {

        this.setItem(bucket, 'infilter', widget);

    },

    getGrid: function (bucket) {

        return this.getItem(bucket, 'grid');

    },

    setGrid: function (bucket, widget) {

        this.setItem(bucket, 'grid', widget);

    },

    getRow: function (bucket) {

        return this.getItem(bucket, 'row');

    },

    setRow: function (bucket, widget) {

        this.setItem(bucket, 'row', widget);

    },

    getColumn: function (bucket) {

        return this.getItem(bucket, 'col');

    },

    setColumn: function (bucket, widget) {

        this.setItem(bucket, 'col', widget);

    },

    getDataModel: function (bucket) {

        return this.getItem(bucket, 'dm');

    },

    setDataModel: function (bucket, widget) {

        this.setItem(bucket, 'dm', widget);

    },

    getFilterToolbar: function (bucket) {

        return this.getItem(bucket, 'filtertb');

    },

    setFilterToolbar: function (bucket, widget) {

        this.setItem(bucket, 'filtertb', widget);

    },

    getFilters: function (bucket) {

        return this.getItem(bucket, 'filters');

    },

    setFilters: function (bucket, widget) {

        this.setItem(bucket, 'filters', widget);

    },

    getFilter: function (bucket) {

        return this.getItem(bucket, 'filter');

    },

    setFilter: function (bucket, widget) {

        this.setItem(bucket, 'filter', widget);

    },

    getCellInfo: function (bucket) {

        return this.getItem(bucket, 'dellinfo');

    },

    setCellInfo: function (bucket, widget) {

        this.setItem(bucket, 'dellinfo', widget);

    },

    getChain: function (bucket) {

        return this.getItem(bucket, 'chain');

    },

    setChain: function (bucket, widget) {

        this.setItem(bucket, 'chain', widget);

    },

    getTabs: function (bucket) {

        return this.getItem(bucket, 'tabs');

    },

    setTabs: function (bucket, widget) {

        this.setItem(bucket, 'tabs', widget);

    },

    getCaller: function (bucket) {

        var self = this;

        var ans;

        // Get the params
        var params = self.getParams(bucket);
        // Any?
        if (params) {
            ans = params.caller;
        }

        return ans;

    },

    setCaller: function (bucket, widget) {

        var self = this;

        var params = self.getParams(bucket);

        if (!params) {
            self.setParams(bucket, params = {});
        }

        params.caller = widget;

    },

    getParentCaller: function (bucket) {

        var self = this;

        var ans;

        var form = self.getForm(bucket);
        if (form) {
            ans = self.getCaller(form);
        }

        return ans;

    },

    getBeforeValue: function (bucket) {

        return this.getItem(bucket, 'before');

    },

    setBeforeValue: function (bucket, widget) {

        this.setItem(bucket, 'before', widget);

    },

    getBounds: function (bucket) {

        var ans;

        var wkg = this.getItem(bucket, 'bounds');
        if (nx.util.hasValue(wkg)) {
            ans = JSON.parse(wkg);
        }
        return ans || {};

    },

    setBounds: function (bucket, widget) {

        this.setItem(bucket, 'bounds', JSON.stringify( widget));

    },

    getUsedViews: function (bucket) {

        var ans;

        var wkg = this.getItem(bucket, 'uviews');
        if (!wkg) {
            wkg = [];
        }
        return wkg;

    },

    setUsedViews: function (bucket, widget) {

        var wkg = this.getItem(bucket, 'uviews');
        if (!wkg) {
            wkg = [];
        }
        if (wkg.indexOf(widget) === -1) {
            wkg.push(widget);
            this.setItem(bucket, 'uviews', wkg);
        }

    },

    getProcessSIO: function (bucket) {

        return this.getItem(bucket, 'psio');

    },

    setProcessSIO: function (bucket, widget) {

        this.setItem(bucket, 'psio', widget);

    },

};