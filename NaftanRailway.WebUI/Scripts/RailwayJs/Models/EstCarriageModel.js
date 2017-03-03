'use strict';

//namespace
var appRail = window.appRail || {};

//constuctor
appRail.EstCarriage = function (data) {
    //private cost

    var self = this;

    self = $.extend(true, self, ko.mapping.fromJS(data));
};