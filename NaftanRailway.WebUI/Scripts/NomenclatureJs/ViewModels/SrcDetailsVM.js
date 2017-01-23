// use new ES5 syntax (IE8 => ES5-shim)
'use strict';

//namespace
var appNomenclature = window.appNomenclature || {};

appNomenclature.SrcDetailsVM = (function ($, ko, db) {
    //link to parent
    var _parent = undefined;

    var self = {
        pagging: ko.observable(),
        rowsPerPage: ko.observable(15),
        charges: ko.observableArray(undefined),
        filters: ko.observableArray(undefined),
        _filterState: ko.pureComputed(function () {
            var result = true;
            $.each(self.filters(), function (idx, item) {
                if (item.CheckedValues().length === 0) {
                    result = false;
                    console.log('counter');
                    return;
                }
            });

            //ok state (if all)
            return result;
        }, self)
    };

    //private 
    function _updateSrcByKey(income, parent) {
        var rows = income.ListDetails;
        var filters = income.Filters;

        var mappingOptions = {
            create: function (options) {
                return ko.mapping.fromJS(options.data);
            }
        };
        //filters
        ko.mapping.fromJS(filters, mappingOptions, self.filters);

        //paging
        self.pagging(new appNomenclature.Pagination(ko.mapping.fromJS(income.PagingInfo, { 'ignore': ["AjaxOptions"] }), ["controller", "action", "numberScroll", "reportYear"], _parent));

        var mappingOptions = {
            key: function (data) {
                return ko.utils.unwrapObservable(data.keysbor);
            },
            create: function (options) {
                return new appNomenclature.Charge(options.data);
            }
        };

        self.charges(ko.mapping.fromJS(rows, mappingOptions)());
    };

    //behavior
    function init(params, parent) {
        _parent = parent;

        //work with options
        var defaults = {
            data: { "initialSizeItem": self.rowsPerPage(), 'filters': self.filters() },
            beforeSend: function () { _parent.loadingState(true); },
            complete: function () {
                _parent.loadingState(false);
            }
        };

        var $merged = $.extend({}, defaults, params);

        db.getScr(function (opts) {
            _updateSrcByKey(opts, _parent);
        }, $merged);
    }

    function applyFilter(formNode) {
        var link = $(formNode).attr('action');

        if (!self._filterState()) {
            _parent.alert().statusMsg('Не все фильтры указаны!').alertType('alert-warning').mode(true);

            //exit
            return;
        }

        db.getScr(function (opts) {
            _updateSrcByKey(opts);
        }, {
            url: link,
            type: "Post",
            data: ko.mapping.toJSON({
                'filters': self.filters(),
                'asService': true
            }),
            beforeSend: function () { _parent.loadingState(true); },
            complete: function () {
                _parent.loadingState(false);
                _parent.alert().statusMsg('Результат фильтра: ' + self.charges().length + ' записей!').alertType('alert-success').mode(true);
            },
            error: function () { _parent.alert().statusMsg('Операция фильтрации завершилась ошибкой!').alertType('alert-danger').mode(true); }
        });
    };

    function changeCountPerPage(link, ev) {
        init({
            url: self.pagging().getPageUrl() + 1,
        }, _parent);
    };

    return {
        init: init,
        applyFilter: applyFilter,
        changeCountPerPage: changeCountPerPage,
        rowsPerPage: self.rowsPerPage,
        pagging: self.pagging,
        charges: self.charges,
        filters: self.filters
    };
})(jQuery, ko, appNomenclature.DataContext);