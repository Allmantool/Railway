'use strict';

//namespace
var appAdmin = window.appAdmin || {};

//constructor
appAdmin.Message = function (data) {
    //private cost

    var self = this;

    self.currentTime = ko.observable(moment().format());
    self = $.extend(true, self, ko.mapping.fromJS(data));


    self.isOwn = function (id) {
        var result = (self.User.ConnectionId() !== id);

        return result;
    };

    self.timeAgo = ko.pureComputed(function () {
        var result = -moment(self.currentTime()).diff(moment(self.SendTime()).format(), 'minutes');

        //return a minute ago
        return moment.duration(result, "minutes").humanize(true);
    }, self);

    var _time = 0;
    //time ago update
    self._interval = setInterval(function () {
        if (_time <= 100) {
            self.currentTime(moment());
            console.log(self.timeAgo());
            _time++;
        }
        else {
            clearInterval(self._interval);
        }
    }, 25000);
}