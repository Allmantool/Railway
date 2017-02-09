/// <reference path="../jquery-1.11.3.js" />
'use strict';

//namespace
var appRail = window.appRail || {};

appRail.DispatchsVM = (function ($, ko, db) {
    var self = {
        _containerName: undefined,
        itemsPerPage: ko.observable(7),
        alert: ko.observable(new appRail.AlertMessage({ statusMsg: 'Инициализация' })),
        pagging: ko.observable(),

        shReview: ko.observableArray([]),
        dispatchs: ko.observableArray([]),

        loadingState: ko.observable(false),
        reportPeriod: ko.observable(moment()._d),
        invoice: ko.observable(),
        notFoundModal: ko.observable(false),
        previewModal: ko.observable(false)
    };

    //behavior
    function init(params) {
        //get container element
        self._containerName = $("#koContainer")[0];

        //work with options
        var defaults = {
            data: { "pageSize": self.itemsPerPage() },
            beforeSend: function () { self.loadingState(true); },
            complete: function () {
                //firts initialization avoid multibinding
                if (params === undefined) {
                    ko.applyBindings(appRail.DispatchsVM);
                }

                self.loadingState(false);
            }
        };

        //work with options
        var $merged = $.extend({}, defaults, params);

        //close modals
        self.previewModal(false);
        self.notFoundModal(false);

        db.getScr(function (data) {
            //dispatch (Collapse Wells)
            self.dispatchs($.map(data.Dispatchs, function (val, i) {
                return new appRail.Dispach(val);
            }));

            //pagging
            self.pagging(new appRail.Pagination(ko.mapping.fromJS(data.PagingInfo, { 'ignore': ["AjaxOptions"] }), ["controller"], self));
        }, $merged);
    };

    function searchInvoice(node, ev) {
        //work with options
        var defaults = {
            url: $(node).attr('action'),
            type: "Post",
            data: ko.mapping.toJSON({
                'menuView':
                    {
                        'ShippingChoise': self.invoice(),
                        'ReportPeriod': moment(self.reportPeriod()).format('YYYY-MM-01'),
                    },
                'asService': true
            }),
            beforeSend: function () { self.loadingState(true); },
            complete: function () {
                //firts initialization
                self.loadingState(false);
            }
        };

        //work with options
        var $merged = $.extend({}, defaults);

        db.getScr(function (data) {
            if (data.length === 0) {
                self.notFoundModal(true)
            } else {
                self.shReview($.map(data, function (val, i) {
                    return new appRail.ShReview(val)
                }));

                self.previewModal(true);
            }

        }, $merged);
    };

    function addInvoice(node, ev) {
        //work with options
        var defaults = {
            url: $(node).attr('action'),
            type: "Post",
            data: ko.mapping.toJSON({
                'reportPeriod': moment(self.reportPeriod()).format('YYYY-MM-01'),
                'docInfo': self.shReview,
                'asService': true
            }),
            beforeSend: function () { self.loadingState(true); },
            complete: function () {
                //firts initialization
                self.loadingState(false);
            }
        };

        //work with options
        var $merged = $.extend({}, defaults);

        db.getScr(function (data) {
            //update (avoid set undefined as param => multy binding)
            init({
                complete: function () {
                    if (data) {
                        self.alert().statusMsg('Накладная добавлена!').alertType('alert-success').mode(true);
                    } else {
                        self.alert().statusMsg('Операция добавления завершилась ошибкой!').alertType('alert-danger').mode(true);
                    }

                    self.loadingState(false);
                }
            });
        });
    }

    function updateExists(params, context, ev) {
        //work with options
        var defaults = {
            type: "Post",
            data: ko.mapping.toJSON({
                'reportPeriod': moment(self.reportPeriod()).format('YYYY-MM-01'),
                'asService': true
            }),
            beforeSend: function () { self.loadingState(true); },
            complete: function () {
                //firts initialization
                self.loadingState(false);
            }
        };

        //work with options
        var $merged = $.extend({}, defaults, params);

        db.getScr(function (data) {
            //update (avoid set undefined as param => multy binding)
            init({
                complete: function () {
                    if (data) {
                        self.alert().statusMsg('Данные по накладным обновлены!').alertType('alert-success').mode(true);
                    } else {
                        self.alert().statusMsg('Операция обновления информации по накладным завершилась ошибкой!').alertType('alert-danger').mode(true);
                    }

                    self.loadingState(false);
                }
            });
        });
    };

    function deleteInvoice(node, ev) {
        //work with options
        var defaults = {
            url: $(node).attr('action'),
            type: "Post",
            data: ko.mapping.toJSON({
                'reportPeriod': moment(self.reportPeriod()).format('YYYY-MM-01'),
                'idInvoice': '',
                'asService': true
            }),
            beforeSend: function () { self.loadingState(true); },
            complete: function () {
                //firts initialization
                self.loadingState(false);
            }
        };

        //work with options
        var $merged = $.extend({}, defaults);

        db.getScr(function (data) {
            //update (avoid set undefined as param => multy binding)
            init({
                complete: function () {
                    if (data) {
                        self.alert().statusMsg('Информация по накладной была успешно удалена!').alertType('alert-success').mode(true);
                    } else {
                        self.alert().statusMsg('Операция удаления завершилась ошибкой!').alertType('alert-danger').mode(true);
                    }

                    self.loadingState(false);
                }
            });
        });
    };

    /*System extensation for Jquery unbinding*/
    ko.unapplyBindings = function ($node, remove) {
        // unbind events
        $node.find("*").each(function () {
            $(this).unbind();
        });

        // Remove KO subscriptions and references
        if (remove) {
            ko.removeNode($node[0]);
        } else {
            ko.cleanNode($node[0]);
        }
    };

    //work with ajax replace (replace dom is leaded to lose binding)
    function containerRebind(opts, ev) {
        self.loadingState(true);

        var defaults = {
            container: self.containerName,
            link: ev ? $(ev.target).attr('href') : '',
            callback: function () { }
        };

        var $merged = $.extend({}, defaults, opts);

        var container = $merged.container;
        var requestLink = $merged.link;

        //Unbind
        if (ko.dataFor(self.containerName)) {
            ko.unapplyBindings($(self.containerName));
        }

        $(self.containerName).load(requestLink, function (partialHtml, statusTxt, xhr) {
            if (statusTxt === "success") {
                //bind
                //ko.options.useOnlyNativeEvents = true;
                ko.applyBindings(appRail.DispatchsVM, self.containerName);
                self.loadingState(false);

                $merged.callback();
            };

            if (statusTxt === "error")
                alert("Error: containerRebind " + xhr.status + ": " + xhr.statusText);
        });
    };

    return {
        init: init,
        searchInvoice: searchInvoice,
        addInvoice: addInvoice,
        updateExists: updateExists,

        loadingState: self.loadingState,
        dispatchs: self.dispatchs,
        itemsPerPage: self.itemsPerPage,
        alert: self.alert,
        pagging: self.pagging,
        reportPeriod: self.reportPeriod,
        invoice: self.invoice,
        notFoundModal: self.notFoundModal,
        previewModal: self.previewModal,
        shReview: self.shReview
    };
}(jQuery, ko, appRail.DataContext));