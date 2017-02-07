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
        dispatchs: ko.observableArray([]),
        loadingState: ko.observable(false),
        reportPeriod: ko.observable(moment().format()),
        searchInvoice: ko.observable()
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
                //firts initialization
                if (params === undefined) {
                    ko.applyBindings(appRail.DispatchsVM);
                }

                self.loadingState(false);
            }
        };

        //work with options
        var $merged = $.extend({}, defaults, params);

        db.getScr(function (data) {
            //dispatch (Collapse Wells)
            self.dispatchs($.map(data.Dispatchs, function (val, i) {
                return new appRail.Dispach(val);
            }));

            //pagging
            self.pagging(new appRail.Pagination(ko.mapping.fromJS(data.PagingInfo, { 'ignore': ["AjaxOptions"] }), ["controller"], self));
        }, $merged);
    };

    /*Sestem extensation for Jquery unbinding*/
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

    //work with ajax replace (reaplace dom is leaded to lose binding)
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
        loadingState: self.loadingState,
        dispatchs: self.dispatchs,
        itemsPerPage: self.itemsPerPage,
        alert: self.alert,
        pagging: self.pagging,
        reportPeriod: self.reportPeriod,
        searchInvoice: self.searchInvoice
    };
}(jQuery, ko, appRail.DataContext));