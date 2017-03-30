/// <reference path="../jquery-1.11.3.js" />
/// <reference path="../jquery-2.2.4.js" />
/// <reference path="../jquery-3.4.2.debug.js" />

'use strict';
//ready
$(document).ready(function () {
    //localisation
    moment.locale('ru');

    //upper case in Month's names
    moment.updateLocale('ru', {
        months: [
            "Январь", "Февраль", "Март", "Апрель", "Май", "Июнь", "Июль",
            "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь"
        ]
    });

    //wait ajax populate module prop
    appAdmin.Engage.init();
});



//namespace
var appAdmin = window.appAdmin || {};

appAdmin.Engage = (function ($, ko, db) {
    var self = {
        _containerName: undefined,
        itemsPerPage: ko.observable(12),
        alert: ko.observable(new appAdmin.AlertMessage({ statusMsg: 'Инициализация' })),
        pagging: ko.observable(),

        userPrincipals: ko.observableArray([]),
        groupPrincipals: ko.observableArray([]),

        loadingState: ko.observable(false),

        adminPrincipal: ko.observable(),
    };

    //behavior
    function init(params) {
        //get container element
        self._containerName = $("#koContainer")[0];

        //work with options
        var defaults = {
            data: {
                "pageSize": self.itemsPerPage(),
            },
            beforeSend: function () { self.loadingState(true); },
            complete: function () {
                //firts initialization avoid multibinding
                if (params === undefined) {
                    ko.applyBindings(appAdmin.Engage);
                }

                self.loadingState(false);
            }
        };

        //work with options
        var $merged = $.extend(true, defaults, params);

        db.request(function (data) {
            self.dispatchs($.map(data, function (val, i) {
                return new appAdmin.UserPrincipal(val);
            }));

            //pagging
            //self.pagging(new appAdmin.Pagination(ko.mapping.fromJS(data.PagingInfo, { 'ignore': ["AjaxOptions"] }), { prefix: "Page" }, self));
        }, $merged);
    };


    return {
        init: init
    };
})(jQuery, ko, appAdmin.DataContext);