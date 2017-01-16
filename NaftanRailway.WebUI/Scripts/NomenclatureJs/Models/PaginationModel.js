﻿'use strict';

//namespace
var appNomenclature = window.appNomenclature || {};

//constuctor
appNomenclature.Pagination = function (parentObj, urlTemplate, parent) {
    var self = this, _parent = parent, _visibleLimit = 10;

    //динамически копируем свойства
    self = $.extend(true, self, ko.mapping.fromJS(parentObj));

    //self.TotalItems = totalItems;
    //self.ItemsPerPage = itemsPerPage;
    //self.CurrentPage = currentPage;
    self.getPageUrl = ko.pureComputed(function () {
        var result = '';

        $.each(ko.mapping.toJS(self.RoutingDictionary), function (index, value) {
            if ($.inArray(index, urlTemplate) > -1) {
                result = result + value + '/';
            };
        });

        return result;
    });

    self.allPages = ko.pureComputed(function () {
        var pages = [];
        for (var i = 0; i <= Math.floor((self.TotalItems() - 1) / self.ItemsPerPage()) ; i++) {
            pages.push({ pageNumber: (i + 1) });
        }

        return pages;
    });//.extend({ deferred: true });

    self.moveToPage = function (root, ev, parent) {
        var self = parent || this;

        var $requestlink = $(ev)[0].target.href;

        self.init({
            url: $requestlink
        }, _parent);
    };

    self.previousPage = function (parent, ev, link) {
        if (self.CurrentPage() > 1) {
            self.moveToPage(null, ev, parent);
        }
    };

    self.nextPage = function (parent, ev) {
        if (self.CurrentPage() < self.allPages().length) {
            self.moveToPage(null, ev, parent);
        }
    };
};