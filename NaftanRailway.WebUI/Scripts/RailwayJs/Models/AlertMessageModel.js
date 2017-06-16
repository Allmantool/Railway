'use strict';

//namespace
var appRail = window.appRail || {};

//constuctor
appRail.AlertMessage = function (opts) {
    //var self = this;
    var defaults = {
        statusMsg: ko.observable("I'm fine rail"),
        alertType: ko.observable('alert-info'),
        mode: ko.observable(false)
    };

    //динамически копируем свойства
    var result = $.extend(true, this, defaults, ko.mapping.fromJS(opts));

    return result;
}