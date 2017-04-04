'use strict';

//namespace
var appAdmin = window.appAdmin || {};

//constructor
appAdmin.GroupPrincipal = function (data) {
    //private cost
    var self = this;

    self = $.extend(true, self, ko.mapping.fromJS(data));

    //self.isSelected = ko.observable(false);
}