'use strict';

//namespace
var appRail = window.appRail || {};

//constuctor
appRail.ShReview = function (data) {
    //private cost

    var self = this;

    self = $.extend(true, self, ko.mapping.fromJS(data));

    //return string list carriage numbers
    self.carriagesList = ko.pureComputed(function () {
        var result = $.map(self.wagonsNumbers(), function (val, i) {
            return val.n_vag();
        });

        return result.join(', ');
    });//.extend({ deferred: true });
};