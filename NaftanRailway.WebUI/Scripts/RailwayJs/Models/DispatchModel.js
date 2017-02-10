'use strict';

//namespace
var appRail = window.appRail || {};

//constuctor
appRail.Dispach = function (data) {
    //private cost

    var self = this;

    self = $.extend(true, self, ko.mapping.fromJS(data));

    //check if we have information about shipping
    self.shInfoExists = ko.pureComputed(function () {
        var result = (typeof self.VOtpr === "undefined" ? false : true);

        return result;
    });

    //dynamic change bootsrap glyphicon css
    self.directionGlyphicon = ko.pureComputed(function () {
        var shInfo = self.VOtpr;

        var result = 'glyphicon' + (typeof shInfo === "undefined" ? 'glyphicon-usd' : shInfo.oper() === 1 ? 'glyphicon-road' : 'glyphicon-home');

        return result;
    });

    self.fullNameSender = ko.pureComputed(function () {
        var result = self.VOtpr.adr_otpr() + ' ' + self.VOtpr.nam_otpr();

        return result;
    });

    self.fullNameReceiver = ko.pureComputed(function () {
        var result = self.VOtpr.adr_pol() + ' ' + self.VOtpr.nam_pol();

        return result;
    });
};