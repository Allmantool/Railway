'use strict';

//namespace
var appAdmin = window.appAdmin || {};

//constructor
appAdmin.Message = function (data) {
    //private cost

    var self = this;

    self = $.extend(true, self, ko.mapping.fromJS(data));

    self.isOwn = function (id) {
        var result = (self.User.ConnectionId() !== id);

        return result;
    };
}