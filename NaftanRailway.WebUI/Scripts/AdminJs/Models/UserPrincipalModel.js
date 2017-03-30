'use strict';

//namespace
var appAdmin = window.appAdmin || {};

//constructor
appAdmin.UserPrincipal = function (data) {
    //private cost

    var self = this;

    self = $.extend(true, self, ko.mapping.fromJS(data));
}