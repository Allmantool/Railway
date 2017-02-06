// use new ES5 syntax (IE8 => ES5-shim)
'use strict';

//namespace
var appNomenclature = window.appNomenclature || {};

appNomenclature.SrcDetailsVM = (function ($, ko, db) {
    //link to parent
    var _parent = undefined;

    var self = {
        chainModal: ko.observable(false),
        editModal: ko.observable(false),
        viewWrong: ko.observable(false),
        pagging: ko.observable(),
        rowsPerPage: ko.observable(20),
        charges: ko.observableArray(undefined),
        filters: ko.observableArray(undefined),
        currChg: ko.observable(undefined),
        operationDialog: ko.observable(false),
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

    function _exist() {
        //match by key
        return ko.utils.arrayFirst(self.charges(), function (item) {
            return self.currChg() ? self.currChg().keysbor() === item.keysbor() : false;
        });
    }

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

        //count of rows (lopping error???)
        //http://knockoutjs.com/documentation/computed-dependency-tracking.html
        // peek - avoid the circular dependency,
        //self.rowsPerPage.peek(self.charges().length === 0 ? 20 : self.charges().length);

        //default charge
        if (self.charges.peek().length > 0 || !_exist) {
            self.currChg(self.charges()[0]);
        }
    };

    //behavior
    function init(params, parent) {
        _parent = parent;

        //work with options
        var defaults = {
            type: "Post",
            data: ko.mapping.toJSON({ "initialSizeItem": self.rowsPerPage(), "asService": true, "filters": self.filters(), "viewWrong": self.viewWrong() }),
            beforeSend: function () { _parent.loadingState(true); },
            complete: function () {
                _parent.loadingState(false);
            }
        };

        var $merged = $.extend({}, defaults, params);

        //close( quick menu)
        self.operationDialog(false);

        db.getScr(function (opts) {
            _updateSrcByKey(opts, _parent);
        }, $merged);
    }

    function setActive(el, ev) {
        //mark as selected
        self.currChg(el);

        //show menu
        self.operationDialog(true);

        return true;
    }

    function applyFilter(arg, ev) {
        //$(ev.target).parents('form').attr('action') || 
        var link = typeof arg === 'string' ? arg : $(arg).attr('action');

        if (!self._filterState()) {
            _parent.alert().statusMsg('Не все фильтры указаны!').alertType('alert-warning').mode(true);

            //exit
            return;
        }

        init({
            url: self.pagging().getPageUrl() + 1,
            beforeSend: function () { _parent.loadingState(true); },
            complete: function () {
                _parent.loadingState(false);
                _parent.alert().statusMsg('Результат фильтра: ' + self.charges().length + ' записей!').alertType('alert-success').mode(true);
            },
            error: function () { _parent.alert().statusMsg('Операция фильтрации завершилась ошибкой!').alertType('alert-danger').mode(true); }
        }, _parent);
    };

    function changeCountPerPage(link, ev) {
        init({
            url: self.pagging().getPageUrl() + 1,
        }, _parent);
    };

    function syncWithDB(link, context) {
        var defaults = {
            url: link,
            type: "Post",
            data: ko.mapping.toJSON({ "charge": context, "asService": true }),
            beforeSend: function () { _parent.loadingState(true); },
            complete: function () {
                _parent.loadingState(false);
            }
        }

        db.getScr(function (opts) {
            _parent.alert().statusMsg('Информация по сбору успешно изменена! ' + opts).alertType('alert-success').mode(true);
        }, defaults);

        self.editModal(false);
    }

    return {
        init: init,
        applyFilter: applyFilter,
        setActive: setActive,
        changeCountPerPage: changeCountPerPage,
        rowsPerPage: self.rowsPerPage,
        pagging: self.pagging,
        charges: self.charges,
        filters: self.filters,
        currChg: self.currChg,
        dialog: self.operationDialog,
        viewWrong: self.viewWrong,
        editModal: self.editModal,
        chainModal: self.chainModal,
        syncWithDB: syncWithDB
    };
})(jQuery, ko, appNomenclature.DataContext);