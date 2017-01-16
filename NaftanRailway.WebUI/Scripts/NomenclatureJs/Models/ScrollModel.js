'use strict';

//namespace
var appNomenclature = window.appNomenclature || {};

//constuctor
appNomenclature.Scroll = function (data, asObservables) {
    var self = this;
    //for (var prop in parentObj) {
    //    if (parentObj.hasOwnProperty(prop)) {
    //        self[prop] = parentObj[prop];
    //    }
    //}
    if (asObservables) {
        self = $.extend(true, self, ko.mapping.fromJS(data));

        self.selected = ko.observable(false);
        self.proccessingState = ko.observable(false);

        /**** behaviors ***/
        self.srcKey = ko.pureComputed(function () {
            return self.NKRT() + '/' + moment(self.DTBUHOTCHET()).format('YYYY');
        }, self);
    }
    else {
        self = $.extend(true, self, data);
    }    
}

//prototype shared methods
//appNomenclature.Scroll.prototype.toggle = function () {
/// ...
//}