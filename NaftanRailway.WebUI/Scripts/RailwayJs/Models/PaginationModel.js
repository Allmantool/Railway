'use strict';

//namespace
var appRail = window.appRail || {};

//constuctor
appRail.Pagination = function (parentObj, urlTemplate, parent) {
    var self = this, _parent = parent, _visibleLimit = 15;

    //динамически копируем свойства
    self = $.extend(true, self, ko.mapping.fromJS(parentObj));

    //self.TotalItems = totalItems;
    //self.ItemsPerPage = itemsPerPage;
    //self.CurrentPage = currentPage;
    self.getPageUrl = ko.pureComputed(function () {
        var result = '';

        var defaults = {
            prefix: '',
            node: [""]
        };

        var $merged = $.extend(true, defaults, urlTemplate);

        $.each(ko.mapping.toJS(self.RoutingDictionary), function (index, value) {
            if ($.inArray(index, $merged.node) > -1) {
                result = result + value + '/';
            };
        });

        //prefix
        result = $merged.prefix + result;

        return result
    });

    self.allPages = ko.pureComputed(function () {
        var pages = [];

        for (var i = 0; i <= Math.floor((self.TotalItems() - 1) / self.ItemsPerPage()) ; i++) {
            pages.push({ pageNumber: (i + 1) });
        }

        //default value
        if (pages.length === 0) {
            pages.push({ pageNumber: 1 });
        };

        return pages;
    });//.extend({ deferred: true });

    self.moveToPage = function (root, ev, parent) {
        var self = parent || this;
        var $el = $(ev.target)
        var $requestlink = $el.is('a') ? $el.attr('href') : $el.parents('a').attr('href');

        self.init({
            url: $requestlink
        }, _parent);
    };

    self.previousPage = function (parent, ev, link) {
        if (self.CurrentPage() > 1) {
            self.moveToPage(null, ev, parent);
        }
    };

    self.nextPage = function (parent, ev, link) {
        if (self.CurrentPage() < self.allPages().length) {
            self.moveToPage(null, ev, parent);
        }
    };
};